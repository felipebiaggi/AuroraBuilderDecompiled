using System.Windows;
using System.Windows.Controls;

namespace Aurora.Presentation.Controls
{
    public class CommandButton : Button
    {
        public static readonly DependencyProperty CommandTextProperty = DependencyProperty.Register(nameof(CommandText), typeof(string), typeof(CommandButton), new PropertyMetadata((object)null));
        public static readonly DependencyProperty CommandTextVisibilityProperty = DependencyProperty.Register(nameof(CommandTextVisibility), typeof(Visibility), typeof(CommandButton), new PropertyMetadata((object)Visibility.Visible));
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(CommandButton), new PropertyMetadata((object)new CornerRadius()));

        static CommandButton()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(CommandButton), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(CommandButton)));
        }

        public string CommandText
        {
            get => (string)this.GetValue(CommandButton.CommandTextProperty);
            set => this.SetValue(CommandButton.CommandTextProperty, (object)value);
        }

        public Visibility CommandTextVisibility
        {
            get => (Visibility)this.GetValue(CommandButton.CommandTextVisibilityProperty);
            set => this.SetValue(CommandButton.CommandTextVisibilityProperty, (object)value);
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)this.GetValue(CommandButton.CornerRadiusProperty);
            set => this.SetValue(CommandButton.CornerRadiusProperty, (object)value);
        }
    }
}