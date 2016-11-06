using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.NuclexGui.Controls;
using MonoGame.Extended.Support.Plugins;
using PCLStorage;

namespace MonoGame.Extended.NuclexGui.Visuals.Flat
{
    // TODO: This implements the Drawable class but doesn't override Draw()
    //   Having two overloads of the Draw() method, one doing nothing, is confusing
    //   and should be avoided. Find a better solution. Perhaps we can rely completely
    //   on the virtualized graphics device here.

    /// <summary>Draws traditional flat GUIs using 2D bitmaps</summary>
    public class FlatGuiVisualizer : IGuiVisualizer, IDisposable
    {
        #region struct ControlWithBounds

        /// <summary>Container for a control and its absolute boundaries</summary>
        struct ControlWithBounds
        {
            /// <summary>Control stored in the container</summary>
            public GuiControl Control;
            /// <summary>Absolute boundaries of the stored control</summary>
            public RectangleF Bounds;

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
                containerBounds.X += control.Bounds.Location.X.Fraction * containerBounds.Width;
                containerBounds.X += control.Bounds.Location.X.Offset;
                containerBounds.Y += control.Bounds.Location.Y.Fraction * containerBounds.Height;
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

        #endregion // struct ControlWithBounds

        #region interface IControlRendererAdapter

        /// <summary>Interface for a generic (typeless) control renderer</summary>
        internal interface IControlRendererAdapter
        {
            /// <summary>Renders the specified control using the provided graphics interface</summary>
            /// <param name="controlToRender">Control that will be rendered</param>
            /// <param name="graphics">Graphics interface that will be used to render the control</param>
            void Render(GuiControl controlToRender, IFlatGuiGraphics graphics);

            /// <summary>The type of the control renderer being adapted</summary>
            Type AdaptedType { get; }
        }

        #endregion // interface IControlRendererAdapter

        #region class ControlRendererAdapter<>

        /// <summary>
        ///   Adapter that automatically casts a control down to the renderer's supported
        ///   control type
        /// </summary>
        /// <typeparam name="ControlType">
        ///   Type of control the control renderer casts down to
        /// </typeparam>
        /// <remarks>
        ///   This is simply an optimization to avoid invoking the control renderer
        ///   by reflection (using the Invoke() method) which would require us to construct
        ///   an object[] array on the heap to pass its arguments.
        /// </remarks>
        private class ControlRendererAdapter<ControlType> : IControlRendererAdapter where ControlType : GuiControl
        {
            /// <summary>Control renderer this adapter is performing the downcast for</summary>
            private IFlatControlRenderer<ControlType> _controlRenderer;

            /// <summary>Initializes a new control renderer adapter</summary>
            /// <param name="controlRenderer">Control renderer the adapter is used for</param>
            public ControlRendererAdapter(IFlatControlRenderer<ControlType> controlRenderer)
            {
                _controlRenderer = controlRenderer;
            }

            /// <summary>Renders the specified control using the provided graphics interface</summary>
            /// <param name="controlToRender">Control that will be rendered</param>
            /// <param name="graphics">Graphics interface that will be used to render the control</param>
            public void Render(GuiControl controlToRender, IFlatGuiGraphics graphics)
            {
                _controlRenderer.Render((ControlType)controlToRender, graphics);
            }

            /// <summary>The type of the control renderer being adapted</summary>
            public Type AdaptedType
            {
                get { return _controlRenderer.GetType(); }
            }

        }

        #endregion // class ControlRendererAdapter<>

        #region class ControlRendererEmployer

        /// <summary>
        ///   Employs concrete types implementing IFlatGuiControlRenderer&lt;&gt;
        /// </summary>
        /// <remarks>
        ///   This employer actually looks for concrete implementations using a variant
        ///   of the IFlatGuiControlRenderer&lt;&gt; interface, regardless of the
        ///   type it has been realized for.
        /// </remarks>
        internal class ControlRendererEmployer : Employer
        {

            /// <summary>Initializes a new control renderer employer</summary>
            public ControlRendererEmployer()
            {
                _renderers = new Dictionary<Type, IControlRendererAdapter>();
            }

            /// <summary>Determines whether the type suites the employer's requirements</summary>
            /// <param name="type">Type that is checked for employability</param>
            /// <returns>True if the type can be employed</returns>
            public override bool CanEmploy(Type type)
            {

                // If the type doesn't implement the IFlatcontrolRenderer interface, there's
                // no chance that it will implement one of the generic control drawers
                if (!typeof(IFlatControlRenderer).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
                {
                    return false;
                }

                // We also need a default constructor in order to be able to create an
                // instance of this renderer
                if (!PluginHelper.HasDefaultConstructor(type))
                {
                    return false;
                }

                // Look for the IFlatControlRenderer<> interface in all interfaces implemented
                // by this type
                foreach (var implementedInterface in type.GetTypeInfo().ImplementedInterfaces)
                {
                    // Only perform further check if this interface is actually generic
                    if (implementedInterface.IsConstructedGenericType)
                    {
                        Type genericType = implementedInterface.GetGenericTypeDefinition();
                        if (genericType == typeof(IFlatControlRenderer<>))
                            return true;
                    }
                }

                // The interface we were looking for was not found, therefore, this is
                // not an employable type
                return false;

            }

            /// <summary>Employs the specified plugin type</summary>
            /// <param name="type">Type to be employed</param>
            public override void Employ(Type type)
            {

                // Obtain all the interfaces of the employed type and search them one by one.
                // We need to take this route because there's no method that would allow us to
                // look up the generic interface in its unspecialized form with a simple call.
                foreach (var implementedInterface in type.GetTypeInfo().ImplementedInterfaces)
                {

                    // Only perform further checks if this interface is actually a generic one
                    if (implementedInterface.IsConstructedGenericType)
                    {

                        // Get the (unspecialized) generic form of this interface and see if it's
                        // the interface we're looking for
                        Type genericType = implementedInterface.GetGenericTypeDefinition();
                        if (genericType == typeof(IFlatControlRenderer<>))
                        {

                            // Find out which control type the renderer is specialized for
                            Type[] controlType = implementedInterface.GenericTypeArguments;

                            // Do we already have a renderer for this control type?
                            if (_renderers.ContainsKey(controlType[0]))
                            {
#if !(XBOX360 || WINDOWS_PHONE)
                                // We found another renderer for a control type that already has
                                // a renderer. At least print out a warning to the debug log about this.
                                string message = string.Format(
                                  "Warning: Control type '{0}' already using renderer '{1}'.\n" +
                                  "         Second renderer '{2}' will be ignored!",
                                  controlType[0].FullName.ToString(),
                                  _renderers[controlType[0]].AdaptedType.FullName.ToString(),
                                  type.FullName.ToString()
                                );
                                //System.Diagnostics.Trace.WriteLine(message);
#endif
                            }
                            else { // No, this is the first renderer we found for this control type

                                // Type of the downcast adapter we need to bring to life
                                var adapterType = typeof(ControlRendererAdapter<>).MakeGenericType(controlType[0]);
                                // Look up the constructor of the downcast adapter
                                //ConstructorInfo adapterConstructor = adapterType.GetTypeInfo().DeclaredConstructors.First(c => c.GetGenericArguments() == implementedInterface.GenericTypeArguments );
                                var adapterConstructor = adapterType.GetTypeInfo().DeclaredConstructors.FirstOrDefault();

                                // Now use that constructor to create an instance
                                object adapterInstance = adapterConstructor.Invoke(
                                  new object[] { Activator.CreateInstance(type) }
                                );

                                // Employ the new adapter and thereby the control renderer it adapts
                                _renderers.Add(controlType[0], (IControlRendererAdapter)adapterInstance);

                            }

                        }
                    }

                }

            }

            /// <summary>Renderers that were employed to the plugin host</summary>
            public Dictionary<Type, IControlRendererAdapter> Renderers
            {
                get { return _renderers; }
            }

            /// <summary>Employed renderers</summary>
            private Dictionary<Type, IControlRendererAdapter> _renderers;

        }

        #endregion // class ControlRendererEmployer

        #region Fields

        /// <summary>Holds the assemblies we have employed for our cause</summary>
        private PluginHost _pluginHost;

        /// <summary>Carries the employed control renderers</summary>
        private ControlRendererEmployer _employer;

        /// <summary>Used to draw the individual building elements of the GUI</summary>
        private FlatGuiGraphics _flatGuiGraphics;

        /// <summary>Helps draw the GUI controls in the hierarchically correct order</summary>
        /// <remarks>
        ///   This is a field and not a local variable because the stack allocates
        ///   heap memory and we don't want that to happen in a frame-by-frame basis on
        ///   the compact framework. By reusing the same stack over and over, the amount
        ///   of heap allocations required will amortize itself.
        /// </remarks>
        private Stack<ControlWithBounds> _controlStack;

        #endregion

        /// <summary>Initializes a new gui painter for traditional GUIs</summary>
        /// <param name="contentManager">Content manager that will be used to load the skin resources</param>
        /// <param name="skinStream">Stream from which the GUI Visualizer will read the skin description</param>
        protected FlatGuiVisualizer(ContentManager contentManager, Stream skinStream)
        {
            _employer = new ControlRendererEmployer();
            _pluginHost = new PluginHost(_employer);

            // Employ our own assembly in order to obtain the default GUI renderers
            _pluginHost.Repository.AddAssembly(Self);

            _flatGuiGraphics = new FlatGuiGraphics(contentManager, skinStream);
            _controlStack = new Stack<ControlWithBounds>();
        }

        /// <summary>Initializes a new gui visualizer from a skin stored in a file</summary>
        /// <param name="serviceProvider">
        ///   Game service provider containing the graphics device service
        /// </param>
        /// <param name="skinPath">
        ///   Path to the skin description this GUI visualizer will load
        /// </param>
        public static FlatGuiVisualizer FromFile(IServiceProvider serviceProvider, string skinPath)
        {
            IFolder folder = FileSystem.Current.GetFolderFromPathAsync(skinPath).Result;
            IFile file = FileSystem.Current.GetFileFromPathAsync(skinPath).Result;

            using (Stream skinStream = file.OpenAsync(FileAccess.Read).Result)
            {
                ContentManager contentManager = new ContentManager(serviceProvider, folder.Path);

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

        /// <summary>Initializes a new gui visualizer from a skin stored as a resource</summary>
        /// <param name="serviceProvider">Game service provider containing the graphics device service</param>
        /// <param name="skinJsonFile">Name of the Json file containing the skin description</param>
        public static FlatGuiVisualizer FromResource(IServiceProvider serviceProvider, string skinJsonFile)
        {
            var assembly = typeof(FlatGuiVisualizer).GetTypeInfo().Assembly;
            string[] resources = assembly.GetManifestResourceNames();

            if (!resources.Contains(skinJsonFile))
                throw new ArgumentException("Resource '" + skinJsonFile + "' not found", "skinJsonFile");

            using (Stream skinStream = assembly.GetManifestResourceStream(skinJsonFile))
            {
                var rootDirectory = Path.GetDirectoryName(resources.First(s => s == skinJsonFile));
                ContentManager contentManager = new ContentManager(serviceProvider, "Content");

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
                    ControlWithBounds controlWithBounds = _controlStack.Pop();

                    GuiControl currentControl = controlWithBounds.Control;
                    RectangleF currentBounds = controlWithBounds.Bounds;

                    // Add the controls in normal order, so the first control in the collection will
                    // be taken off the stack last, ensuring it's rendered on top of all others.
                    for (int index = 0; index < currentControl.Children.Count; ++index)
                        _controlStack.Push(ControlWithBounds.FromControl(currentControl.Children[index], currentBounds));
                    
                    renderControl(currentControl);
                }
            }
            finally
            {
                _flatGuiGraphics.EndDrawing();
            }
        }

        /// <summary>
        ///   Plugin repository from which renderers for GUI controls are taken
        /// </summary>
        public PluginRepository RendererRepository
        {
            get { return _pluginHost.Repository; }
        }

        /// <summary>Renders a single control</summary>
        /// <param name="controlToRender">Control that will be rendered</param>
        private void renderControl(GuiControl controlToRender)
        {
            IControlRendererAdapter renderer = null;

            Type controlType = controlToRender.GetType();

            // If this is an actual instance of the 'Control' class, don't render it.
            // Such instances can be used to construct invisible containers, and are most
            // prominently embodied in the 'desktop' control that hosts the whole GUI.
            if (
              (controlType == typeof(GuiControl)) ||
              (controlType == typeof(GuiDesktopControl))
            )
            {
                return;
            }

            // Find a renderer for this control. If no renderer for the control itself can
            // be found, look for a renderer then can render its base class. This allows
            // controls to inherit from existing controls, remaining renderable (but also
            // gaining the ability to accept a specialized renderer for the new derived
            // control class!). Normally, this loop will finish without any repetitions.
            while (controlType != typeof(object))
            {
                bool found = _employer.Renderers.TryGetValue(controlType, out renderer);
                if (found)
                {
                    break;
                }

                // Next, try the base class of this type
                controlType = controlType.GetTypeInfo().BaseType;
            }

            // If we found a renderer, use it to render the control
            if (renderer != null)
            {
                renderer.Render(controlToRender, _flatGuiGraphics);
            }
            else { // No renderer found, output a warning
#if WINDOWS
        Trace.WriteLine(
          string.Format(
            "Warning: No renderer found for control '{0}' or any of its base classes.\n" +
            "         Control will not be rendered.",
            controlToRender.GetType().FullName.ToString()
          )
        );
#endif
            }
        }

        /// <summary>Returns the assembly containing the GUI visualizer</summary>
        private static Assembly Self
        {
            get { return typeof(FlatGuiVisualizer).GetTypeInfo().Assembly; }
        }
    }
}
