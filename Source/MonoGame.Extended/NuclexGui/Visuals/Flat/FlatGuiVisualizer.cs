using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.NuclexGui.Controls;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.NuclexGui.Visuals.Flat
{
    // TODO: This implements the Drawable class but doesn't override Draw()
    //   Having two overloads of the Draw() method, one doing nothing, is confusing
    //   and should be avoided. Find a better solution. Perhaps we can rely completely
    //   on the virtualized graphics device here.

    /// <summary>Draws traditional flat GUIs using 2D bitmaps</summary>
    public class FlatGuiVisualizer : IGuiVisualizer, IDisposable
    {
        /// <summary>Helps draw the GUI controls in the hierarchically correct order</summary>
        /// <remarks>
        ///     This is a field and not a local variable because the stack allocates
        ///     heap memory and we don't want that to happen in a frame-by-frame basis on
        ///     the compact framework. By reusing the same stack over and over, the amount
        ///     of heap allocations required will amortize itself.
        /// </remarks>
        private readonly Stack<ControlWithBounds> _controlStack;

        /// <summary>Used to draw the individual building elements of the GUI</summary>
        private FlatGuiGraphics _flatGuiGraphics;

        /// <summary>Avaiable renderers</summary>
        private readonly Dictionary<Type, IControlRendererAdapter> _renderers;

        /// <summary>Initializes a new gui painter for traditional GUIs</summary>
        /// <param name="contentManager">Content manager that will be used to load the skin resources</param>
        /// <param name="skinStream">Stream from which the GUI Visualizer will read the skin description</param>
        protected FlatGuiVisualizer(ContentManager contentManager, Stream skinStream)
        {
            _renderers = new Dictionary<Type, IControlRendererAdapter>();

            // Obtain default GUI renderers
            FetchRenderers();

            _flatGuiGraphics = new FlatGuiGraphics(contentManager, skinStream);
            _controlStack = new Stack<ControlWithBounds>();
        }

        /// <summary>Immediately releases all resources owned by the instance</summary>
        public void Dispose()
        {
            if (_flatGuiGraphics != null)
            {
                _flatGuiGraphics.Dispose();
                _flatGuiGraphics = null;
            }
        }

        /// <summary>Draws an entire GUI hierarchy</summary>
        /// <param name="screen">Screen containing the GUI that will be drawn</param>
        public void Draw(GuiScreen screen)
        {
            _flatGuiGraphics.BeginDrawing();
            try
            {
                _controlStack.Push(ControlWithBounds.FromScreen(screen));

                while (_controlStack.Count > 0)
                {
                    var controlWithBounds = _controlStack.Pop();

                    var currentControl = controlWithBounds.Control;
                    var currentBounds = controlWithBounds.Bounds;

                    // Add the controls in normal order, so the first control in the collection will
                    // be taken off the stack last, ensuring it's rendered on top of all others.
                    for (var index = 0; index < currentControl.Children.Count; ++index)
                        _controlStack.Push(ControlWithBounds.FromControl(currentControl.Children[index], currentBounds));

                    RenderControl(currentControl);
                }
            }
            finally
            {
                _flatGuiGraphics.EndDrawing();
            }
        }

        /// <summary>Renders a single control</summary>
        /// <param name="controlToRender">Control that will be rendered</param>
        private void RenderControl(GuiControl controlToRender)
        {
            IControlRendererAdapter renderer = null;

            var controlType = controlToRender.GetType();

            // If this is an actual instance of the 'Control' class, don't render it.
            // Such instances can be used to construct invisible containers, and are most
            // prominently embodied in the 'desktop' control that hosts the whole GUI.
            if ((controlType == typeof(GuiControl)) || (controlType == typeof(GuiDesktopControl)))
                return;

            // Find a renderer for this control. If no renderer for the control itself can
            // be found, look for a renderer then can render its base class. This allows
            // controls to inherit from existing controls, remaining renderable (but also
            // gaining the ability to accept a specialized renderer for the new derived
            // control class!). Normally, this loop will finish without any repetitions.
            while (controlType != typeof(object))
            {
                var found = _renderers.TryGetValue(controlType, out renderer);
                if (found) break;

                // Next, try the base class of this type
                controlType = controlType.GetTypeInfo().BaseType;
            }

            // If we found a renderer, use it to render the control
            if (renderer != null)
                renderer.Render(controlToRender, _flatGuiGraphics);
        }

        private void FetchRenderers()
        {
            foreach (
                var typeinfo in
                typeof(FlatGuiVisualizer).GetTypeInfo().Assembly.DefinedTypes.Where(e => e.IsPublic && !e.IsAbstract))
            {
                // If the type doesn't implement the IFlatcontrolRenderer interface, there's
                // no chance that it will implement one of the generic control drawers
                if (!typeof(IFlatControlRenderer).GetTypeInfo().IsAssignableFrom(typeinfo))
                    continue;

                // We also need a default constructor in order to be able to create an instance of this renderer
                if (typeinfo.DeclaredConstructors.Count(e => e.IsPublic && (e.GetParameters().Length == 0)) == 0)
                    continue;

                // Look for the IFlatControlRenderer<> interface in all interfaces implemented this type
                var genericType =
                    typeinfo.ImplementedInterfaces.Where(
                        e =>
                            e.IsConstructedGenericType &&
                            (e.GetGenericTypeDefinition() == typeof(IFlatControlRenderer<>))).FirstOrDefault();
                if (genericType == default(Type)) continue;

                // Find out which control type the renderer is specialized for
                var controlType = genericType.GenericTypeArguments;

                // Do we already have a renderer for this control type?
                if (!_renderers.ContainsKey(controlType[0]))
                {
                    // Type of the downcast adapter we need to bring to life
                    var adapterType = typeof(ControlRendererAdapter<>).MakeGenericType(controlType[0]);
                    var adapterConstructor = adapterType.GetTypeInfo().DeclaredConstructors.FirstOrDefault();

                    // Now use that constructor to create an instance
                    var adapterInstance = adapterConstructor.Invoke(new[] {Activator.CreateInstance(typeinfo.AsType())});

                    // Employ the new adapter and thereby the control renderer it adapts
                    _renderers.Add(controlType[0], (IControlRendererAdapter) adapterInstance);
                }
            }
        }

        /// <summary>Initializes a new gui visualizer from a skin stored as a resource</summary>
        /// <param name="serviceProvider">Game service provider containing the graphics device service</param>
        /// <param name="skinJsonFile">Name of the Json file containing the skin description</param>
        public static FlatGuiVisualizer FromResource(IServiceProvider serviceProvider, string skinJsonFile)
        {
            var assembly = typeof(FlatGuiVisualizer).GetTypeInfo().Assembly;
            var resources = assembly.GetManifestResourceNames();

            if (!resources.Contains(skinJsonFile))
                throw new ArgumentException("Resource '" + skinJsonFile + "' not found", "skinJsonFile");

            using (var skinStream = assembly.GetManifestResourceStream(skinJsonFile))
            {
                var contentManager = new ContentManager(serviceProvider, "Content");

                try
                {
                    return new FlatGuiVisualizer(contentManager, skinStream);
                }
                catch (Exception)
                {
                    contentManager.Dispose();
                    throw;
                }
            }
        }

        /// <summary>Container for a control and its absolute boundaries</summary>
        private struct ControlWithBounds
        {
            /// <summary>Control stored in the container</summary>
            public readonly GuiControl Control;

            /// <summary>Absolute boundaries of the stored control</summary>
            public readonly RectangleF Bounds;

            /// <summary>Initializes a new control and absolute boundary container</summary>
            /// <param name="control">Control being store in the container</param>
            /// <param name="bounds">Absolute boundaries the control lives in</param>
            public ControlWithBounds(GuiControl control, RectangleF bounds)
            {
                Control = control;
                Bounds = bounds;
            }

            /// <summary>Builds an absolute boundary container from the provided control</summary>
            /// <param name="control">Control from which a container will be created</param>
            /// <param name="containerBounds">Absolute boundaries of the control's parent</param>
            /// <returns>A new container with the control</returns>
            public static ControlWithBounds FromControl(GuiControl control, RectangleF containerBounds)
            {
                containerBounds.X += control.Bounds.Location.X.Fraction*containerBounds.Width;
                containerBounds.X += control.Bounds.Location.X.Offset;
                containerBounds.Y += control.Bounds.Location.Y.Fraction*containerBounds.Height;
                containerBounds.Y += control.Bounds.Location.Y.Offset;

                containerBounds.Width = control.Bounds.Size.X.ToOffset(containerBounds.Width);
                containerBounds.Height = control.Bounds.Size.Y.ToOffset(containerBounds.Height);

                return new ControlWithBounds(control, containerBounds);
            }

            /// <summary>Builds a control and absolute boundary container from a screen</summary>
            /// <param name="screen">Screen whose desktop control and absolute boundaries are used to construct the container</param>
            /// <returns>A new container with the screen's desktop control</returns>
            public static ControlWithBounds FromScreen(GuiScreen screen)
            {
                return new ControlWithBounds(screen.Desktop, screen.Desktop.Bounds.ToOffset(screen.Width, screen.Height));
            }
        }

        /// <summary>Interface for a generic (typeless) control renderer</summary>
        internal interface IControlRendererAdapter
        {
            /// <summary>The type of the control renderer being adapted</summary>
            Type AdaptedType { get; }

            /// <summary>Renders the specified control using the provided graphics interface</summary>
            /// <param name="controlToRender">Control that will be rendered</param>
            /// <param name="graphics">Graphics interface that will be used to render the control</param>
            void Render(GuiControl controlToRender, IFlatGuiGraphics graphics);
        }

        /// <summary>
        ///     Adapter that automatically casts a control down to the renderer's supported
        ///     control type
        /// </summary>
        /// <typeparam name="ControlType">
        ///     Type of control the control renderer casts down to
        /// </typeparam>
        /// <remarks>
        ///     This is simply an optimization to avoid invoking the control renderer
        ///     by reflection (using the Invoke() method) which would require us to construct
        ///     an object[] array on the heap to pass its arguments.
        /// </remarks>
        private class ControlRendererAdapter<TControlType> : IControlRendererAdapter where TControlType : GuiControl
        {
            /// <summary>Control renderer this adapter is performing the downcast for</summary>
            private readonly IFlatControlRenderer<TControlType> _controlRenderer;

            /// <summary>Initializes a new control renderer adapter</summary>
            /// <param name="controlRenderer">Control renderer the adapter is used for</param>
            public ControlRendererAdapter(IFlatControlRenderer<TControlType> controlRenderer)
            {
                _controlRenderer = controlRenderer;
            }

            /// <summary>Renders the specified control using the provided graphics interface</summary>
            /// <param name="controlToRender">Control that will be rendered</param>
            /// <param name="graphics">Graphics interface that will be used to render the control</param>
            public void Render(GuiControl controlToRender, IFlatGuiGraphics graphics)
            {
                _controlRenderer.Render((TControlType) controlToRender, graphics);
            }

            /// <summary>The type of the control renderer being adapted</summary>
            public Type AdaptedType => _controlRenderer.GetType();
        }
    }
}