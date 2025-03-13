using System.Windows;
using System.Windows.Controls;
using Builder.Presentation.Controls;

namespace Builder.Presentation.Controls
{
    public class AuroraExpander : Expander
    {
        public static readonly DependencyProperty AdditionalHeaderContentProperty;

        public static readonly DependencyProperty FooterContentProperty;

        public static readonly DependencyProperty FooterVisibilityProperty;

        public static readonly DependencyProperty AdditionalHeaderVisibilityProperty;

        public static readonly DependencyProperty AdditionalHeaderContentTemplateProperty;

        public FrameworkElement AdditionalHeaderContent
        {
            get
            {
                return (FrameworkElement)GetValue(AdditionalHeaderContentProperty);
            }
            set
            {
                SetValue(AdditionalHeaderContentProperty, value);
            }
        }

        public object FooterContent
        {
            get
            {
                return GetValue(FooterContentProperty);
            }
            set
            {
                SetValue(FooterContentProperty, value);
            }
        }

        public Visibility FooterVisibility
        {
            get
            {
                return (Visibility)GetValue(FooterVisibilityProperty);
            }
            set
            {
                SetValue(FooterVisibilityProperty, value);
            }
        }

        public Visibility AdditionalHeaderVisibility
        {
            get
            {
                return (Visibility)GetValue(AdditionalHeaderVisibilityProperty);
            }
            set
            {
                SetValue(AdditionalHeaderVisibilityProperty, value);
            }
        }

        public DataTemplate AdditionalHeaderContentTemplate
        {
            get
            {
                return (DataTemplate)GetValue(AdditionalHeaderContentTemplateProperty);
            }
            set
            {
                SetValue(AdditionalHeaderContentTemplateProperty, value);
            }
        }

        static AuroraExpander()
        {
            AdditionalHeaderContentProperty = DependencyProperty.Register("AdditionalHeaderContent", typeof(FrameworkElement), typeof(AuroraExpander), new PropertyMetadata((object)null));
            FooterContentProperty = DependencyProperty.Register("FooterContent", typeof(object), typeof(AuroraExpander), new PropertyMetadata((object)null));
            FooterVisibilityProperty = DependencyProperty.Register("FooterVisibility", typeof(Visibility), typeof(AuroraExpander), new PropertyMetadata(Visibility.Collapsed));
            AdditionalHeaderVisibilityProperty = DependencyProperty.Register("AdditionalHeaderVisibility", typeof(Visibility), typeof(AuroraExpander), new PropertyMetadata(Visibility.Visible));
            AdditionalHeaderContentTemplateProperty = DependencyProperty.Register("AdditionalHeaderContentTemplate", typeof(DataTemplate), typeof(AuroraExpander), new PropertyMetadata((object)null));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(AuroraExpander), new FrameworkPropertyMetadata(typeof(AuroraExpander)));
        }
    }
}
