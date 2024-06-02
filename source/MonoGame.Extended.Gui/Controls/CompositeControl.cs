using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Gui.Controls
{
    public abstract class CompositeControl : Control
    {
        protected CompositeControl()
        {
        }

        protected bool IsDirty { get; set; } = true;

        public abstract object Content { get; set; }

        protected abstract Control Template { get; }

        public override IEnumerable<Control> Children
        {
            get
            {
                var control = Template;

                if (control != null)
                    yield return control;
            }
        }

        public override void InvalidateMeasure()
        {
            base.InvalidateMeasure();
            IsDirty = true;
        }

        public override Size GetContentSize(IGuiContext context)
        {
            var control = Template;

            if (control != null)
                return Template.CalculateActualSize(context);

            return Size.Empty;
        }

        public override void Update(IGuiContext context, float deltaSeconds)
        {
            var control = Template;

            if (control != null)
            {
                if (IsDirty)
                {
                    control.Parent = this;
                    control.ActualSize = new Point(ContentRectangle.Width, ContentRectangle.Height);
                    control.Position = new Point(Padding.Left, Padding.Top);
                    control.InvalidateMeasure();
                    IsDirty = false;
                }
            }
        }

        public override void Draw(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            base.Draw(context, renderer, deltaSeconds);

            var control = Template;
            control?.Draw(context, renderer, deltaSeconds);
        }
    }
}