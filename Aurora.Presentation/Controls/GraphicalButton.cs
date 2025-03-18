using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Aurora.Presentation.Controls
{
    public class GraphicalButton : Button
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(GraphicalButton), new PropertyMetadata((object)null));
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(nameof(Description), typeof(string), typeof(GraphicalButton), new PropertyMetadata((object)null));
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(GraphicalButton), new PropertyMetadata((object)null));

        static GraphicalButton()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(GraphicalButton), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(GraphicalButton)));
        }

        public string Title
        {
            get => (string)this.GetValue(GraphicalButton.TitleProperty);
            set => this.SetValue(GraphicalButton.TitleProperty, (object)value);
        }

        public string Description
        {
            get => (string)this.GetValue(GraphicalButton.DescriptionProperty);
            set => this.SetValue(GraphicalButton.DescriptionProperty, (object)value);
        }

        public ImageSource ImageSource
        {
            get => (ImageSource)this.GetValue(GraphicalButton.ImageSourceProperty);
            set => this.SetValue(GraphicalButton.ImageSourceProperty, (object)value);
        }
    }
}
