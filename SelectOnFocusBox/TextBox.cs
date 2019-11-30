namespace SelectOnFocusBox
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls.Primitives;

    public static class TextBox
    {
        public static readonly DependencyProperty SelectAllOnGotKeyboardFocusProperty = DependencyProperty.RegisterAttached(
            "SelectAllOnGotKeyboardFocus",
            typeof(bool),
            typeof(TextBox),
            new FrameworkPropertyMetadata(
                false,
                FrameworkPropertyMetadataOptions.Inherits));

        static TextBox()
        {
            EventManager.RegisterClassHandler(typeof(TextBoxBase), UIElement.GotKeyboardFocusEvent, new RoutedEventHandler(Select));
            EventManager.RegisterClassHandler(typeof(TextBoxBase), UIElement.MouseUpEvent, new RoutedEventHandler(Dump), handledEventsToo: true);
            EventManager.RegisterClassHandler(typeof(TextBoxBase), UIElement.PreviewMouseUpEvent, new RoutedEventHandler(Dump), handledEventsToo: true);
            EventManager.RegisterClassHandler(typeof(TextBoxBase), TextBoxBase.SelectionChangedEvent, new RoutedEventHandler(Dump), handledEventsToo: true);
            EventManager.RegisterClassHandler(typeof(TextBoxBase), UIElement.GotFocusEvent, new RoutedEventHandler(Dump), handledEventsToo: true);
        }

        /// <summary>Helper for setting <see cref="SelectAllOnGotKeyboardFocusProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="SelectAllOnGotKeyboardFocusProperty"/> on.</param>
        /// <param name="value">SelectAllOnGotKeyboardFocus property value.</param>
        public static void SetSelectAllOnGotKeyboardFocus(this UIElement element, bool value)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValue(SelectAllOnGotKeyboardFocusProperty, value);
        }

        /// <summary>Helper for getting <see cref="SelectAllOnGotKeyboardFocusProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="SelectAllOnGotKeyboardFocusProperty"/> from.</param>
        /// <returns>SelectAllOnGotKeyboardFocus property value.</returns>
        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
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

        private static void Dump(object sender, RoutedEventArgs e)
        {
            Dump(e);
        }

        private static void Dump(RoutedEventArgs e)
        {
            if (e.Source is System.Windows.Controls.TextBox source)
            {
                Debug.WriteLine($"IsFocused: {source.IsFocused,-5} IsKeyboardFocused: {source.IsKeyboardFocused,-5} IsKeyboardFocusWithin: {source.IsKeyboardFocusWithin,-5} IsSelectionActive: {source.IsSelectionActive,-5} SelectedText: {source.SelectedText,-5} {e.RoutedEvent.Name}");
            }
        }
    }
}