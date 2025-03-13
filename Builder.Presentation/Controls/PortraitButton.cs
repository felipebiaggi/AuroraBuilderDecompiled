using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Builder.Presentation.Controls;

namespace Builder.Presentation.Controls
{
    public class PortraitButton : Button
    {
        public static readonly DependencyProperty ImageSourceProperty;

        public static readonly DependencyProperty PortraitScaleProperty;

        public static readonly DependencyProperty PortraitSizeProperty;

        public static readonly DependencyProperty PortaitSizeProperty;

        public static readonly DependencyProperty PortraitBorderThicknessProperty;

        public static readonly DependencyProperty SymbolProperty;

        public static readonly DependencyProperty PortraitStretchProperty;

        public ImageSource ImageSource
        {
            get
            {
                return (ImageSource)GetValue(ImageSourceProperty);
            }
            set
            {
                SetValue(ImageSourceProperty, value);
            }
        }

        public double PortraitScale
        {
            get
            {
                return (double)GetValue(PortraitScaleProperty);
            }
            set
            {
                SetValue(PortraitScaleProperty, value);
            }
        }

        public double PortraitSize
        {
            get
            {
                return (double)GetValue(PortraitSizeProperty);
            }
            set
            {
                SetValue(PortraitSizeProperty, value);
            }
        }

        public double PortaitSize
        {
            get
            {
                return (double)GetValue(PortaitSizeProperty);
            }
            set
            {
                SetValue(PortaitSizeProperty, value);
            }
        }

        public double PortraitBorderThickness
        {
            get
            {
                return (double)GetValue(PortraitBorderThicknessProperty);
            }
            set
            {
                SetValue(PortraitBorderThicknessProperty, value);
            }
        }

        public string Symbol
        {
            get
            {
                return (string)GetValue(SymbolProperty);
            }
            set
            {
                SetValue(SymbolProperty, value);
            }
        }

        public Stretch PortraitStretch
        {
            get
            {
                return (Stretch)GetValue(PortraitStretchProperty);
            }
            set
            {
                SetValue(PortraitStretchProperty, value);
            }
        }

        static PortraitButton()
        {
            ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(PortraitButton), new PropertyMetadata((object)null));
            PortraitScaleProperty = DependencyProperty.Register("PortraitScale", typeof(double), typeof(PortraitButton), new PropertyMetadata(0.0));
            PortraitSizeProperty = DependencyProperty.Register("PortraitSize", typeof(double), typeof(PortraitButton), new PropertyMetadata(0.0));
            PortaitSizeProperty = DependencyProperty.Register("PortaitSize", typeof(double), typeof(PortraitButton), new PropertyMetadata(0.0));
            PortraitBorderThicknessProperty = DependencyProperty.Register("PortraitBorderThickness", typeof(double), typeof(PortraitButton), new PropertyMetadata(0.0));
            SymbolProperty = DependencyProperty.Register("Symbol", typeof(string), typeof(PortraitButton), new PropertyMetadata((object)null));
            PortraitStretchProperty = DependencyProperty.Register("PortraitStretch", typeof(Stretch), typeof(PortraitButton), new PropertyMetadata(Stretch.None));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PortraitButton), new FrameworkPropertyMetadata(typeof(PortraitButton)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
    }
}
