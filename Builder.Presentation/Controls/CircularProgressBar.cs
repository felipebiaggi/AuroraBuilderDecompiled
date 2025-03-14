using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Builder.Presentation.Controls
{
    public class CircularProgressBar : ProgressBar
    {
        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register("Angle", typeof(double), typeof(CircularProgressBar), new PropertyMetadata(0.0));

        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(CircularProgressBar), new PropertyMetadata(10.0));

        public double Angle
        {
            get
            {
                return (double)GetValue(AngleProperty);
            }
            set
            {
                SetValue(AngleProperty, value);
            }
        }

        public double StrokeThickness
        {
            get
            {
                return (double)GetValue(StrokeThicknessProperty);
            }
            set
            {
                SetValue(StrokeThicknessProperty, value);
            }
        }

        public CircularProgressBar()
        {
            base.ValueChanged += CircularProgressBar_ValueChanged;
        }

        private void CircularProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CircularProgressBar circularProgressBar = sender as CircularProgressBar;
            double angle = circularProgressBar.Angle;
            double toValue = e.NewValue / circularProgressBar.Maximum * 359.999;
            DoubleAnimation animation = new DoubleAnimation(angle, toValue, TimeSpan.FromMilliseconds(500.0));
            circularProgressBar.BeginAnimation(AngleProperty, animation, HandoffBehavior.SnapshotAndReplace);
        }
    }
}
