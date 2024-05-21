using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PrimalEditor.Utilities.Controls {
     class VectorBox : Control {

        public double Multiplier {
            get => (double)GetValue(MultiplierProperty);
            set => SetValue(MultiplierProperty, value);
        }

        public static readonly DependencyProperty MultiplierProperty =
            DependencyProperty.Register(nameof(Multiplier), typeof(double), typeof(NumberBox),
                new PropertyMetadata(1.0));

        // 向量X，Y，Z，W的依赖属性
        public string X {
            get => (string)GetValue(XProperty);
            set => SetValue(XProperty, value);
        }

        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register(nameof(X), typeof(string), typeof(NumberBox),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Y {
            get => (string)GetValue(YProperty);
            set => SetValue(YProperty, value);
        }

        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register(nameof(Y), typeof(string), typeof(NumberBox),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Z {
            get => (string)GetValue(ZProperty);
            set => SetValue(ZProperty, value);
        }

        public static readonly DependencyProperty ZProperty =
            DependencyProperty.Register(nameof(Z), typeof(string), typeof(NumberBox),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string W {
            get => (string)GetValue(WProperty);
            set => SetValue(WProperty, value);
        }

        public static readonly DependencyProperty WProperty =
            DependencyProperty.Register(nameof(W), typeof(string), typeof(NumberBox),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        static VectorBox() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VectorBox), 
                new FrameworkPropertyMetadata(typeof(VectorBox)));
        }

    }
}
