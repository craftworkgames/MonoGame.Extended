using System;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;

namespace Gui.Features
{
    public class StackPanelView
    {
        public StackPanelView()
        {
        }

        public Control Content = new StackPanel
        {
            Items =
            {
                new Button
                {
                    Content = "Press Me",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Padding = new Thickness(5),
                    Margin = new Thickness(5)
                },
                new Button
                {
                    Content = "Press Me",
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Padding = new Thickness(5),
                    Margin = new Thickness(5)
                },
                new Button
                {
                    Content = "Press Me",
                    HorizontalAlignment = HorizontalAlignment.Centre,
                    VerticalAlignment = VerticalAlignment.Centre,
                    Padding = new Thickness(5),
                    Margin = new Thickness(5)
                },
                new Button
                {
                    Content = "Press Me",
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Padding = new Thickness(5),
                    Margin = new Thickness(5)
                },
            }
        };
    }
}
