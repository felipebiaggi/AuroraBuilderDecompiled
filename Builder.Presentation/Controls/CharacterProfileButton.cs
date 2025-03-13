using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Builder.Core.Logging;
using Builder.Presentation.Controls;

namespace Builder.Presentation.Controls
{
    public class CharacterProfileButton : Control
    {
        public static readonly DependencyProperty CharacterNameProperty;

        public static readonly DependencyProperty CharacterBuildProperty;

        public static readonly DependencyProperty CharacterImagePathProperty;

        public static readonly DependencyProperty PortraitScaleProperty;

        public static readonly DependencyProperty NewCharacterCommandButtonVisibilityProperty;

        public static readonly DependencyProperty PortraitVisibilityProperty;

        public static readonly DependencyProperty NewCharacterCommandProperty;

        public static readonly DependencyProperty ImageSizeProperty;

        public string CharacterName
        {
            get
            {
                return (string)GetValue(CharacterNameProperty);
            }
            set
            {
                SetValue(CharacterNameProperty, value);
            }
        }

        public string CharacterBuild
        {
            get
            {
                return (string)GetValue(CharacterBuildProperty);
            }
            set
            {
                SetValue(CharacterBuildProperty, value);
            }
        }

        public string CharacterImagePath
        {
            get
            {
                return (string)GetValue(CharacterImagePathProperty);
            }
            set
            {
                SetValue(CharacterImagePathProperty, value);
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

        public Visibility NewCharacterCommandButtonVisibility
        {
            get
            {
                return (Visibility)GetValue(NewCharacterCommandButtonVisibilityProperty);
            }
            set
            {
                SetValue(NewCharacterCommandButtonVisibilityProperty, value);
            }
        }

        public Visibility PortraitVisibility
        {
            get
            {
                return (Visibility)GetValue(PortraitVisibilityProperty);
            }
            set
            {
                SetValue(PortraitVisibilityProperty, value);
            }
        }

        public ICommand NewCharacterCommand
        {
            get
            {
                return (ICommand)GetValue(NewCharacterCommandProperty);
            }
            set
            {
                SetValue(NewCharacterCommandProperty, value);
            }
        }

        public double ImageSize
        {
            get
            {
                return (double)GetValue(ImageSizeProperty);
            }
            set
            {
                SetValue(ImageSizeProperty, value);
            }
        }

        public event EventHandler<RoutedEventArgs> Click;

        public event EventHandler<RoutedEventArgs> NewClick;

        static CharacterProfileButton()
        {
            CharacterNameProperty = DependencyProperty.Register("CharacterName", typeof(string), typeof(CharacterProfileButton), new PropertyMetadata((object)null));
            CharacterBuildProperty = DependencyProperty.Register("CharacterBuild", typeof(string), typeof(CharacterProfileButton), new PropertyMetadata((object)null));
            CharacterImagePathProperty = DependencyProperty.Register("CharacterImagePath", typeof(string), typeof(CharacterProfileButton), new PropertyMetadata((object)null));
            PortraitScaleProperty = DependencyProperty.Register("PortraitScale", typeof(double), typeof(CharacterProfileButton), new PropertyMetadata(0.0));
            NewCharacterCommandButtonVisibilityProperty = DependencyProperty.Register("NewCharacterCommandButtonVisibility", typeof(Visibility), typeof(CharacterProfileButton), new PropertyMetadata(Visibility.Visible));
            PortraitVisibilityProperty = DependencyProperty.Register("PortraitVisibility", typeof(Visibility), typeof(CharacterProfileButton), new PropertyMetadata(Visibility.Visible));
            NewCharacterCommandProperty = DependencyProperty.Register("NewCharacterCommand", typeof(ICommand), typeof(CharacterProfileButton), new PropertyMetadata((object)null));
            ImageSizeProperty = DependencyProperty.Register("ImageSize", typeof(double), typeof(CharacterProfileButton), new PropertyMetadata(0.0));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(CharacterProfileButton), new FrameworkPropertyMetadata(typeof(CharacterProfileButton)));
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            if (e.Delta > 0)
            {
                Logger.Warning($"scrolling up? {PortraitScale}");
                if (PortraitScale < 1.25)
                {
                    PortraitScale += 0.05;
                }
            }
            else
            {
                Logger.Warning($"scrolling down? {PortraitScale}");
                if (PortraitScale > 0.15)
                {
                    PortraitScale -= 0.05;
                }
            }
        }

        public override void OnApplyTemplate()
        {
            (GetTemplateChild("ProfileButton") as Button).Click += delegate (object s, RoutedEventArgs e)
            {
                this.Click?.Invoke(s, e);
            };
            (GetTemplateChild("NewCharacterButton") as Button).Click += delegate (object s, RoutedEventArgs e)
            {
                this.NewClick?.Invoke(s, e);
            };
            PortraitScale = 0.25;
        }
    }
}
