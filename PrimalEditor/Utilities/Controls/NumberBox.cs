﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PrimalEditor.Utilities.Controls{

    //自定义控件
    [TemplatePart(Name = "PART_textBlock", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_textBox", Type = typeof(TextBox))]
    class NumberBox : Control {

        private double _originalValue;
        private double _mouseXStart;
        private bool _captured = false;
        private bool _valueChanged = false;

        //依赖属性
        public string Value {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(NumberBox),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            if (GetTemplateChild("PART_textBlock") is TextBlock textBlock) {
                textBlock.MouseLeftButtonDown += OnTextBlock_Mouse_LBD;
                textBlock.MouseLeftButtonUp += OnTextBlock_Mouse_LBU;
                textBlock.MouseMove += OnTextBlock_Mouse_Move;
            }
        }

        private void OnTextBlock_Mouse_LBD(object sender, MouseButtonEventArgs e) {
            double.TryParse(Value, out _originalValue);

            Mouse.Capture(sender as UIElement);
            _captured = true;
            _valueChanged = false;
            e.Handled = true;

            _mouseXStart = e.GetPosition(this).X;
        }

        private void OnTextBlock_Mouse_LBU(object sender, MouseButtonEventArgs e) {
            if (_captured) { 
                Mouse.Capture(null);
                _captured = false;
                e.Handled= true;
                if (!_valueChanged && GetTemplateChild("PART_textBox") is TextBox textBox) { 
                    textBox.Visibility = Visibility.Visible;
                    textBox.Focus();
                    textBox.SelectAll();
                }
            }
        }

        private void OnTextBlock_Mouse_Move(object sender, MouseEventArgs e) {
            if (_captured) { 
                var mouseX = e.GetPosition(this).X;
                var d = mouseX - _mouseXStart;
                if (Math.Abs(d) > SystemParameters.MinimumHorizontalDragDistance) {
                    var newValue = _originalValue + (d);
                    Value = newValue.ToString("0.#####");
                    _valueChanged = true;
                }
            }

        }

        static NumberBox() { 
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumberBox),
                new FrameworkPropertyMetadata(typeof(NumberBox)));
        }

    }
}
