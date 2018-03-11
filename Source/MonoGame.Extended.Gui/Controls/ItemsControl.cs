using System.Collections.Generic;

namespace MonoGame.Extended.Gui.Controls
{
    public abstract class ItemsControl : Control
    {
        protected ItemsControl()
        {
            Items = new ControlCollection(this);
            //{
            //    ItemAdded = x => UpdateRootIsLayoutRequired(),
            //    ItemRemoved = x => UpdateRootIsLayoutRequired()
            //};
        }

        public override IEnumerable<Control> Children => Items;

        public ControlCollection Items { get; }

        ///// <summary>
        ///// Recursive Method to find the root element and update the IsLayoutRequired property.  So that the screen knows that something in the controls
        ///// have had a change to their layout.  Also, it will reset the size of the element so that it can get a clean build so that the background patches
        ///// can be rendered with the updates.
        ///// </summary>
        //private void UpdateRootIsLayoutRequired()
        //{
        //    var parent = Parent as ItemsControl;

        //    if (parent == null)
        //        IsLayoutRequired = true;
        //    else
        //        parent.UpdateRootIsLayoutRequired();

        //    Size = Size2.Empty;
        //}

        //protected List<T> FindControls<T>()
        //    where T : Control
        //{
        //    return FindControls<T>(Items);
        //}

        //protected List<T> FindControls<T>(ControlCollection controls)
        //    where T : Control
        //{
        //    var results = new List<T>();
        //    foreach (var control in controls)
        //    {
        //        if (control is T)
        //            results.Add(control as T);

        //        var itemsControl = control as ItemsControl;

        //        if (itemsControl != null && itemsControl.Items.Any())
        //            results = results.Concat(FindControls<T>(itemsControl.Items)).ToList();
        //    }
        //    return results;
        //}
    }
}