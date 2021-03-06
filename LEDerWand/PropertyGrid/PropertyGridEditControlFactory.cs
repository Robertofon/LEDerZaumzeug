﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using LEDerWand.PropertyGrid._Attributes;
using ReactiveUI;

namespace LEDerWand.PropertyGrid
{
    public class PropertyGridEditControlFactory
    {
        public virtual Control? CreateControl(
            ConfigurablePropertyMetadata property, 
            IEnumerable<ConfigurablePropertyMetadata> allProperties)
        {
            Control? ctrlValueEdit = null;
            switch (property.ValueType)
            {
                case PropertyValueType.Bool:
                    ctrlValueEdit = this.CreateCheckBoxControl(property, allProperties);
                    break;

                case PropertyValueType.String:
                    ctrlValueEdit = this.CreateTextBoxControl(property, allProperties);
                    break;

                case PropertyValueType.Enum:
                    ctrlValueEdit = this.CreateEnumControl(property, allProperties);
                    break;

                case PropertyValueType.TextAndHexadecimalEdit:
                    ctrlValueEdit = this.CreateTextAndHexadecimalEditControl(property, allProperties);
                    break;

                case PropertyValueType.FixedPossibleValues:
                    ctrlValueEdit = this.CreateFixedPossibleValuesControl(property, allProperties);
                    break;

                case PropertyValueType.RGBPixel:
                    ctrlValueEdit = this.CreateTextBoxControl(property, allProperties);
                    break;

                case PropertyValueType.DetailSettings:
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"Unsupported value {property.ValueType}");
            }

            return ctrlValueEdit;
        }

        protected virtual Control CreateCheckBoxControl(
            ConfigurablePropertyMetadata property, 
            IEnumerable<ConfigurablePropertyMetadata> allProperties)
        {
            var ctrlCheckBox = new CheckBox();
            ctrlCheckBox[!ToggleButton.IsCheckedProperty] = new Binding(
                nameof(property.ValueAccessor),
                BindingMode.TwoWay);
            ctrlCheckBox.HorizontalAlignment = HorizontalAlignment.Left;
            ctrlCheckBox.IsEnabled = !property.IsReadOnly;
            return ctrlCheckBox;
        }

        protected virtual Control CreateTextBoxControl(
            ConfigurablePropertyMetadata property,
            IEnumerable<ConfigurablePropertyMetadata> allProperties)
        {
            var ctrlTextBox = new TextBox();
            ctrlTextBox[!TextBox.TextProperty] = new Binding(
                nameof(property.ValueAccessor),
                BindingMode.TwoWay);
            ctrlTextBox.Width = double.NaN;
            ctrlTextBox.IsReadOnly = property.IsReadOnly;

            if (null != property.GetCustomAttribute<LinkAttribute>())
            {
                ctrlTextBox.Classes.Add("Link");
                ctrlTextBox.IsReadOnly = true;
                ctrlTextBox.Cursor = new Cursor(StandardCursorType.Hand);
                ctrlTextBox.PointerReleased += (sender, args) =>
                {
                    //CommonUtil.OpenUrlInBrowser(ctrlTextBox.Text);
                };
            }

            return ctrlTextBox;
        }

        protected virtual Control CreateEnumControl(
            ConfigurablePropertyMetadata property,
            IEnumerable<ConfigurablePropertyMetadata> allProperties)
        {
            var ctrlComboBox = new ComboBox();
            ctrlComboBox.Items = property.GetEnumMembers();
            ctrlComboBox[!SelectingItemsControl.SelectedItemProperty] = new Binding(
                nameof(property.ValueAccessor),
                BindingMode.TwoWay);
            ctrlComboBox.Width = double.NaN;
            ctrlComboBox.IsEnabled = !property.IsReadOnly;
            return ctrlComboBox;
        }

        protected virtual Control CreateFixedPossibleValuesControl(
            ConfigurablePropertyMetadata property,
            IEnumerable<ConfigurablePropertyMetadata> allProperties)
        {
            var ctrlComboBox = new ComboBox();

            var possibleValueAttrib = property.GetCustomAttribute<FixedPossibleValuesAttribute>();
            if (possibleValueAttrib != null)
            {
                var getValuesMethod = property.HostObjectType.GetMethod(
                    possibleValueAttrib.ValueGetterMethodName,
                    BindingFlags.Static | BindingFlags.Public);
                if ((getValuesMethod != null) &&
                    (getValuesMethod.Invoke(null, null) is IEnumerable collection))
                {
                    ctrlComboBox.Items = collection;
                }
            }

            ctrlComboBox[!SelectingItemsControl.SelectedItemProperty] = new Binding(
                nameof(property.ValueAccessor),
                BindingMode.TwoWay);
            ctrlComboBox.Width = double.NaN;
            ctrlComboBox.IsEnabled = !property.IsReadOnly;

            return ctrlComboBox;
        }

        protected virtual Control CreateTextAndHexadecimalEditControl(
            ConfigurablePropertyMetadata property,
            IEnumerable<ConfigurablePropertyMetadata> allProperties)
        {
            var otherPropertyInfo = property.GetCustomAttribute<TextAndHexadecimalEditAttribute>();
            if (otherPropertyInfo == null)
            {
                throw new InvalidOperationException($"{nameof(TextAndHexadecimalEditAttribute)} not found on property {property.PropertyName}!");
            }

            ConfigurablePropertyMetadata otherProperty = allProperties
                .FirstOrDefault(actProperty => actProperty.PropertyName == otherPropertyInfo.EncodingWebNamePropertyName);
            if (otherProperty == null)
            {
                throw new InvalidOperationException($"Property {otherPropertyInfo.EncodingWebNamePropertyName} not found!");
            }

            var stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;

            var ctrlTextBox1 = new TextBox();
            ctrlTextBox1[!TextBox.TextProperty] = new Binding(
                nameof(property.ValueAccessor),
                BindingMode.TwoWay);
            ctrlTextBox1.Width = double.NaN;
            ctrlTextBox1.IsReadOnly = property.IsReadOnly;

            var hexTextBinding = new Binding(
                nameof(property.ValueAccessor),
                BindingMode.TwoWay)
            {
                Converter = new TextToHexConverter(),
                ConverterParameter = new Func<string?>(() => otherProperty.ValueAccessor as string)
            };

            var ctrlTextBox2Container = new Grid();
            ctrlTextBox2Container.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            ctrlTextBox2Container.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Parse("*")));

            var ctrlTextBox2Header = new TextBlock();
            ctrlTextBox2Header.Text = "HEX";
            ctrlTextBox2Header.SetValue(Grid.ColumnProperty, 0);
            ctrlTextBox2Container.Children.Add(ctrlTextBox2Header);

            var ctrlTextBox2 = new TextBox();
            ctrlTextBox2[!TextBox.TextProperty] = hexTextBinding;
            ctrlTextBox2.Width = double.NaN;
            ctrlTextBox2.IsReadOnly = property.IsReadOnly;

            otherProperty.ObservableForProperty(t=>t.ValueAccessor).Subscribe(
                (next) =>
                {
                    ctrlTextBox2[!TextBox.TextProperty] = hexTextBinding;
                });

            ctrlTextBox2.SetValue(Grid.ColumnProperty, 1);
            ctrlTextBox2Container.Children.Add(ctrlTextBox2);

            stackPanel.Children.Add(ctrlTextBox1);
            stackPanel.Children.Add(ctrlTextBox2Container);

            return stackPanel;
        }
    }
}
