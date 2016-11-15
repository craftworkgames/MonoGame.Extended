using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using MonoGame.Extended.InputListeners;

namespace MonoGame.Extended.NuclexGui.Controls.Desktop
{
    /// <summary>How the list lets the user select items</summary>
    public enum ListSelectionMode
    {
        /// <summary>The user is not allowed to select an item</summary>
        None,

        /// <summary>The user can select only one item</summary>
        Single,

        /// <summary>The user can pick any number of items</summary>
        Multi
    }

    /// <summary>List showing a sequence of items</summary>
    public class GuiListControl : GuiControl, IFocusable
    {
        /// <summary>Items contained in the list</summary>
        private readonly ObservableCollection<string> _items;

        /// <summary>
        ///     Row locator through which the list can detect which row the mouse has
        ///     been pressed down on
        /// </summary>
        private IListRowLocator _listRowLocator;

        /// <summary>Last known Y coordinate of the mouse</summary>
        private float _mouseY;

        /// <summary>Items currently selected in the list</summary>
        private readonly ObservableCollection<int> _selectedItems;

        /// <summary>How the list lets the user select from its items</summary>
        private ListSelectionMode _selectionMode;

        /// <summary>Slider the lists uses to scroll through its items</summary>
        private readonly GuiVerticalSliderControl _slider;

        public GuiListControl()
        {
            _items = new ObservableCollection<string>();
            _items.Cleared += ItemsCleared;
            _items.ItemAdded += ItemAdded;
            _items.ItemRemoved += ItemRemoved;

            _selectedItems = new ObservableCollection<int>();
            _selectedItems.Cleared += SelectionCleared;
            _selectedItems.ItemAdded += SelectionAdded;
            _selectedItems.ItemRemoved += SelectionRemoved;

            _slider = new GuiVerticalSliderControl();
            _slider.Bounds = new UniRectangle(
                new UniScalar(1.0f, -20.0f), new UniScalar(0.0f, 0.0f),
                new UniScalar(0.0f, 20.0f), new UniScalar(1.0f, 0.0f)
            );
            Children.Add(_slider);
        }

        /// <summary>How the user can select items in the list</summary>
        public ListSelectionMode SelectionMode
        {
            get { return _selectionMode; }
            set { _selectionMode = value; }
        }

        /// <summary>Slider the list uses to scroll through its items</summary>
        public GuiVerticalSliderControl Slider => _slider;

        /// <summary>Items being displayed in the list</summary>
        public IList<string> Items => _items;

        /// <summary>Indices of the items current selected in the list</summary>
        public IList<int> SelectedItems => _selectedItems;

        /// <summary>
        ///     Can be set by renderers to enable selection of list items by mouse
        /// </summary>
        public IListRowLocator ListRowLocator
        {
            get { return _listRowLocator; }
            set
            {
                if (value != _listRowLocator)
                {
                    _listRowLocator = value;
                    UpdateSlider();
                }
            }
        }

        /// <summary>Whether the control can currently obtain the input focus</summary>
        bool IFocusable.CanGetFocus => true;

        /// <summary>Triggered when the selected items in list have changed</summary>
        public event EventHandler SelectionChanged;

        /// <summary>Called when a mouse button has been pressed down</summary>
        /// <param name="button">Index of the button that has been pressed</param>
        /// <remarks>
        ///     If this method states that a mouse press is processed by returning
        ///     true, that means the control did something with it and the mouse press
        ///     should not be acted upon by any other listener.
        /// </remarks>
        protected override void OnMousePressed(MouseButton button)
        {
            if (_listRowLocator != null)
            {
                var row = _listRowLocator.GetRow(GetAbsoluteBounds(), _slider.ThumbPosition, _items.Count, _mouseY);
                if ((row >= 0) && (row < _items.Count))
                    OnRowClicked(row);
            }
        }

        /// <summary>Called when the user has clicked on a row in the list</summary>
        /// <param name="row">Row the user has clicked on</param>
        /// <remarks>
        ///     The default behavior of the list control in multi select mode is to
        ///     toggle items that are clicked between selected and unselected. If you
        ///     need different behavior (for example, dragging a selected region or
        ///     selecting sequences of items by holding the shift key), you can override
        ///     this method and handle the selection behavior yourself.
        /// </remarks>
        protected virtual void OnRowClicked(int row)
        {
            switch (_selectionMode)
            {
                // The user isn't allowed to select items in the list
                case ListSelectionMode.None:
                {
                    break;
                }

                // Only a single item can be selected at a time
                case ListSelectionMode.Single:
                {
                    if (_selectedItems.Count == 1)
                    {
                        if (_selectedItems[0] == row)
                            break; // do not fire the SelectionChanged event

                        _selectedItems[0] = row;
                    }
                    else
                    {
                        _selectedItems.Clear();
                        _selectedItems.Add(row);
                    }
                    OnSelectionChanged();

                    break;
                }

                // Any number of items can be selected
                case ListSelectionMode.Multi:
                {
                    if (!_selectedItems.Remove(row))
                        _selectedItems.Add(row);
                    OnSelectionChanged();
                    break;
                }
            }
        }

        /// <summary>Called when the mouse wheel has been rotated</summary>
        /// <param name="ticks">Number of ticks that the mouse wheel has been rotated</param>
        protected override void OnMouseWheel(float ticks)
        {
            const float itemsPerTick = 1.0f;

            if (_listRowLocator != null)
            {
                var bounds = GetAbsoluteBounds();

                float totalitems = _items.Count;
                var itemsInView = bounds.Height/_listRowLocator.GetRowHeight(bounds);
                var scrollableItems = totalitems - itemsInView;

                _slider.ThumbPosition -= itemsPerTick/scrollableItems*ticks;
                _slider.ThumbPosition = MathHelper.Clamp(_slider.ThumbPosition, 0.0f, 1.0f);
            }
        }

        /// <summary>Called when the mouse position is updated</summary>
        /// <param name="x">X coordinate of the mouse cursor on the control</param>
        /// <param name="y">Y coordinate of the mouse cursor on the control</param>
        protected override void OnMouseMoved(float x, float y)
        {
            _mouseY = y;
        }

        /// <summary>Called when the selected items in the list have changed</summary>
        protected virtual void OnSelectionChanged()
        {
            if (SelectionChanged != null)
                SelectionChanged(this, EventArgs.Empty);
        }

        /// <summary>Called when an item is removed from the items list</summary>
        /// <param name="sender">List the item has been removed from</param>
        /// <param name="arguments">Contains the item that has been removed</param>
        private void ItemRemoved(object sender, ItemEventArgs<string> arguments)
        {
            UpdateSlider();
        }

        /// <summary>Called when an item is added to the items list</summary>
        /// <param name="sender">List the item has been added to</param>
        /// <param name="arguments">Contains the item that has been added</param>
        private void ItemAdded(object sender, ItemEventArgs<string> arguments)
        {
            UpdateSlider();
        }

        /// <summary>Called when the items list is about to clear itself</summary>
        /// <param name="sender">Items list that is about to clear itself</param>
        /// <param name="arguments">Not used</param>
        private void ItemsCleared(object sender, EventArgs arguments)
        {
            UpdateSlider();
        }

        /// <summary>Called when an entry is added to the list of selected items</summary>
        /// <param name="sender">List to which an item was added to</param>
        /// <param name="arguments">Contains the added item</param>
        private void SelectionAdded(object sender, ItemEventArgs<int> arguments)
        {
            OnSelectionChanged();
        }

        /// <summary>
        ///     Called when an entry is removed from the list of selected items
        /// </summary>
        /// <param name="sender">List from which an item was removed</param>
        /// <param name="arguments">Contains the removed item</param>
        private void SelectionRemoved(object sender, ItemEventArgs<int> arguments)
        {
            OnSelectionChanged();
        }

        /// <summary>Called when the selected items list is about to clear itself</summary>
        /// <param name="sender">List that is about to clear itself</param>
        /// <param name="arguments">Not Used</param>
        private void SelectionCleared(object sender, EventArgs arguments)
        {
            OnSelectionChanged();
        }

        /// <summary>Updates the size and position of the list's slider</summary>
        private void UpdateSlider()
        {
            if ((Screen != null) && (_listRowLocator != null))
            {
                var bounds = GetAbsoluteBounds();

                float totalitems = _items.Count;
                var itemsInView = bounds.Height/_listRowLocator.GetRowHeight(bounds);

                _slider.ThumbSize = Math.Min(1.0f, itemsInView/totalitems);
            }
        }
    }
}