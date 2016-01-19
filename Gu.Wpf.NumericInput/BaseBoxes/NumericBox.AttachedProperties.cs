﻿namespace Gu.Wpf.NumericInput
{
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;

    public static partial class NumericBox
    {
        public static readonly DependencyProperty SelectAllOnGotKeyboardFocusProperty = DependencyProperty.RegisterAttached(
            "SelectAllOnGotKeyboardFocus",
            typeof(bool),
            typeof(NumericBox),
            new FrameworkPropertyMetadata(
                BooleanBoxes.False,
                FrameworkPropertyMetadataOptions.Inherits,
                OnSelectAllOnGotKeyboardFocusChanged));

        public static readonly DependencyProperty SelectAllOnClickProperty = DependencyProperty.RegisterAttached(
            "SelectAllOnClick",
            typeof(bool),
            typeof(NumericBox),
            new FrameworkPropertyMetadata(
                BooleanBoxes.False,
                FrameworkPropertyMetadataOptions.Inherits,
                OnSelectAllOnClickChanged));

        public static readonly DependencyProperty SelectAllOnDoubleClickProperty = DependencyProperty.RegisterAttached(
            "SelectAllOnDoubleClick",
            typeof(bool),
            typeof(NumericBox),
            new FrameworkPropertyMetadata(
                BooleanBoxes.False,
                FrameworkPropertyMetadataOptions.Inherits,
                OnSelectAllOnDoubleClickChanged));

        public static readonly DependencyProperty CultureProperty = DependencyProperty.RegisterAttached(
            "Culture",
            typeof(IFormatProvider),
            typeof(NumericBox),
            new FrameworkPropertyMetadata(
                Thread.CurrentThread.CurrentUICulture,
                FrameworkPropertyMetadataOptions.Inherits));

        private static readonly DependencyProperty IsSelectingProperty = DependencyProperty.RegisterAttached(
            "IsSelecting",
            typeof(bool),
            typeof(NumericBox),
            new PropertyMetadata(BooleanBoxes.False));

        public static void SetSelectAllOnGotKeyboardFocus(this UIElement element, bool value)
        {
            element.SetValue(SelectAllOnGotKeyboardFocusProperty, BooleanBoxes.Box(value));
        }

        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static bool GetSelectAllOnGotKeyboardFocus(this UIElement element)
        {
            return (bool)element.GetValue(SelectAllOnGotKeyboardFocusProperty);
        }

        public static void SetSelectAllOnClick(this UIElement o, bool value)
        {
            o.SetValue(SelectAllOnClickProperty, BooleanBoxes.Box(value));
        }

        [AttachedPropertyBrowsableForChildrenAttribute(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static bool GetSelectAllOnClick(this UIElement o)
        {
            return (bool)o.GetValue(SelectAllOnClickProperty);
        }

        public static void SetSelectAllOnDoubleClick(this UIElement element, bool value)
        {
            element.SetValue(SelectAllOnDoubleClickProperty, BooleanBoxes.Box(value));
        }

        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static bool GetSelectAllOnDoubleClick(this UIElement element)
        {
            return (bool)element.GetValue(SelectAllOnDoubleClickProperty);
        }

        public static void SetCulture(this UIElement element, CultureInfo value)
        {
            element.SetValue(CultureProperty, value);
        }

        public static CultureInfo GetCulture(this UIElement element)
        {
            return (CultureInfo)element.GetValue(CultureProperty);
        }

        private static void SetIsSelecting(this DependencyObject element, bool value)
        {
            element.SetValue(IsSelectingProperty, BooleanBoxes.Box(value));
        }

        private static bool GetIsSelecting(this DependencyObject element)
        {
            return (bool)element.GetValue(IsSelectingProperty);
        }

        private static void OnSelectAllOnGotKeyboardFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = d as TextBoxBase;
            if (box != null)
            {
                //if (Equals(e.NewValue, BooleanBoxes.True))
                //{
                //    box.AddWeakHandler(UIElement.GotKeyboardFocusEvent, OnKeyboardFocusSelectAllText);
                //    box.AddWeakHandler(UIElement.MouseUpEvent, OnMouseUpSelectAllTextHandler, true);
                //}
                //else
                //{
                //    box.RemoveWeakHandler(UIElement.GotKeyboardFocusEvent, OnKeyboardFocusSelectAllText);
                //    box.RemoveWeakHandler(UIElement.MouseUpEvent, OnMouseUpSelectAllTextHandler);
                //}
            }
        }

        private static void OnSelectAllOnClickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = d as TextBoxBase;
            if (box != null)
            {
                if (Equals(e.NewValue, BooleanBoxes.True))
                {
                    WeakEventManager<TextBoxBase, KeyboardFocusChangedEventArgs>.AddHandler(box, nameof(box.GotKeyboardFocus), OnKeyboardFocusSelectAllText);
                    WeakEventManager<TextBoxBase, MouseButtonEventArgs>.AddHandler(box, nameof(box.PreviewMouseLeftButtonDown), OnMouseLeftButtonDown);
                }
                else
                {
                    WeakEventManager<TextBoxBase, KeyboardFocusChangedEventArgs>.RemoveHandler(box, nameof(box.GotKeyboardFocus), OnKeyboardFocusSelectAllText);
                    WeakEventManager<TextBoxBase, MouseButtonEventArgs>.RemoveHandler(box, nameof(box.PreviewMouseLeftButtonDown), OnMouseLeftButtonDown);
                }
            }
        }

        private static void OnSelectAllOnDoubleClickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = d as TextBoxBase;
            if (box != null)
            {
                if (Equals(e.NewValue, BooleanBoxes.True))
                {
                    WeakEventManager<TextBoxBase, MouseButtonEventArgs>.AddHandler(box, nameof(box.MouseDoubleClick), OnMouseDoubleClick);
                }
                else
                {
                    WeakEventManager<TextBoxBase, MouseButtonEventArgs>.RemoveHandler(box, nameof(box.MouseDoubleClick), OnMouseDoubleClick);
                }
            }
        }

        private static void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var textBoxBase = GetParentFromVisualTree(e.OriginalSource);
            textBoxBase?.SelectAll();
        }

        private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var textBoxBase = GetParentFromVisualTree(e.OriginalSource);
            if (textBoxBase == null)
            {
                return;
            }

            var box = textBoxBase;
            if (!box.IsKeyboardFocusWithin)
            {
                box.Focus();
                e.Handled = true;
            }
        }

        private static TextBoxBase GetParentFromVisualTree(object source)
        {
            DependencyObject parent = source as UIElement;
            while (parent != null && !(parent is TextBoxBase))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as TextBoxBase;
        }

        private static void OnKeyboardFocusSelectAllText(object sender, RoutedEventArgs e)
        {
            if (ReferenceEquals(Keyboard.FocusedElement, sender))
            {
                var textBoxBase = (TextBoxBase)sender;
                if (Mouse.LeftButton == MouseButtonState.Pressed ||
                    Mouse.RightButton == MouseButtonState.Pressed)
                {
                    textBoxBase.SetIsSelecting(true);
                }
                else
                {
                    textBoxBase.SelectAll();
                }
            }
        }

        private static void OnMouseUpSelectAllTextHandler(object sender, RoutedEventArgs e)
        {
            var textBoxBase = (TextBoxBase)sender;
            if (textBoxBase.GetIsSelecting())
            {
                textBoxBase.SelectAll();
                textBoxBase.SetIsSelecting(false);
            }
        }
    }
}
