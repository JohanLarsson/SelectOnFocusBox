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

        static TextBoxExt()
        {
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(OnPreviewMouseLeftButtonDown));
            EventManager.RegisterClassHandler(typeof(TextBoxBase), UIElement.GotKeyboardFocusEvent, new RoutedEventHandler(Select));
            EventManager.RegisterClassHandler(typeof(TextBoxBase), UIElement.MouseUpEvent, new RoutedEventHandler(Dump), handledEventsToo: true);
            EventManager.RegisterClassHandler(typeof(TextBoxBase), UIElement.PreviewMouseUpEvent, new RoutedEventHandler(Dump), handledEventsToo: true);
            EventManager.RegisterClassHandler(typeof(TextBoxBase), TextBoxBase.SelectionChangedEvent, new RoutedEventHandler(Dump), handledEventsToo: true);
            EventManager.RegisterClassHandler(typeof(TextBoxBase), UIElement.GotFocusEvent, new RoutedEventHandler(Dump), handledEventsToo: true);
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

        private static void Select(object sender, RoutedEventArgs e)
        {
            Dump(e);
            if (sender is TextBoxBase textBoxBase &&
                textBoxBase.GetSelectAllOnGotKeyboardFocus())
            {
                textBoxBase.SelectAll();
            }
        }

        private static void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Dump(e);
            if (sender is TextBoxBase { IsKeyboardFocusWithin: false, IsEnabled: true, Focusable: true } textBoxBase &&
                textBoxBase.GetSelectAllOnGotKeyboardFocus() &&
                textBoxBase.Focus())
            {
                e.Handled = true;
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
                Debug.WriteLine($"IsFocused: {source.IsFocused,-5} IsKeyboardFocused: {source.IsKeyboardFocused,-5} SelectedText: {source.SelectedText,-5} {e.RoutedEvent.Name}");
            }
        }
    }
}