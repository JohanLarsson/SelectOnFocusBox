namespace SelectOnFocusBox
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    public static class TextBoxExt
    {
        public static readonly DependencyProperty SelectAllOnGotKeyboardFocusProperty = DependencyProperty.RegisterAttached(
            "SelectAllOnGotKeyboardFocus",
            typeof(bool),
            typeof(TextBoxExt),
            new FrameworkPropertyMetadata(
                false,
                FrameworkPropertyMetadataOptions.Inherits));

        private static readonly DependencyProperty IsSelectingProperty = DependencyProperty.RegisterAttached(
            "IsSelecting",
            typeof(bool),
            typeof(TextBoxExt),
            new PropertyMetadata(default(bool)));

        static TextBoxExt()
        {
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.GotKeyboardFocusEvent, new RoutedEventHandler(OnGotKeyboardFocus));
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.GotFocusEvent, new RoutedEventHandler(Dump), handledEventsToo: true);
            EventManager.RegisterClassHandler(typeof(TextBox), TextBoxBase.SelectionChangedEvent, new RoutedEventHandler(Dump), handledEventsToo: true);

            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.PreviewMouseDownEvent, new RoutedEventHandler(Dump), handledEventsToo: true);
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(Dump));
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.MouseLeftButtonDownEvent, new RoutedEventHandler(Dump), handledEventsToo: true);
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.MouseDownEvent, new RoutedEventHandler(Dump), handledEventsToo: true);
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.PreviewMouseUpEvent, new RoutedEventHandler(Dump), handledEventsToo: true);
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.MouseUpEvent, new RoutedEventHandler(OnMouseUp), handledEventsToo: true);
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.PreviewMouseLeftButtonUpEvent, new RoutedEventHandler(Dump), handledEventsToo: true);
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.MouseLeftButtonUpEvent, new RoutedEventHandler(Dump), handledEventsToo: true);

        }

        public static void SetSelectAllOnGotKeyboardFocus(this UIElement element, bool value)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValue(SelectAllOnGotKeyboardFocusProperty, value);
        }

        public static bool GetSelectAllOnGotKeyboardFocus(this UIElement element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return (bool)element.GetValue(SelectAllOnGotKeyboardFocusProperty);
        }

        private static void OnGotKeyboardFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBoxBase textBoxBase &&
                HasText() &&
                textBoxBase.GetSelectAllOnGotKeyboardFocus())
            {
                Debug.WriteLine("OnGotKeyboardFocus SelectAll");
                textBoxBase.SelectAll();
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    textBoxBase.SetValue(IsSelectingProperty, true);
                }
            }

            Dump(e);

            bool HasText()
            {
                if (sender is System.Windows.Controls.TextBox textBox)
                {
                    return !string.IsNullOrEmpty(textBox.Text);
                }

                return true;
            }
        }

        private static void OnMouseUp(object sender, RoutedEventArgs e)
        {
            Dump(e);
            if (sender is TextBoxBase textBoxBase &&
                textBoxBase.GetSelectAllOnGotKeyboardFocus() &&
                (bool)textBoxBase.GetValue(IsSelectingProperty))
            {
                Debug.WriteLine("OnMouseUp Handled = true");
                e.Handled = true;
                textBoxBase.ReleaseMouseCapture();
                textBoxBase.SetValue(IsSelectingProperty, false);
            }
        }

        private static void Dump(object sender, RoutedEventArgs e)
        {
            Dump(e);
        }

        private static void Dump(RoutedEventArgs e)
        {
            if (e.Source is System.Windows.Controls.TextBox source)
            {
                Debug.WriteLine($"{e.RoutedEvent.Name,-26} {e.Source.GetType().Name}/{e.OriginalSource.GetType().Name,-11} IsFocused: {source.IsFocused,-6} IsKeyboardFocused: {source.IsKeyboardFocused,-6} Handled: {e.Handled,-6} SelectedText: {source.SelectedText,-4}");
            }
        }
    }
}