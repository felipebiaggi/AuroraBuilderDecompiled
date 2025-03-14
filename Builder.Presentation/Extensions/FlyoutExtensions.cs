using MahApps.Metro.Controls;

namespace Builder.Presentation.Extensions
{
    public static class FlyoutExtensions
    {
        public static ViewModelBase GetViewModel(this Flyout window)
        {
            return (ViewModelBase)window.DataContext;
        }

        public static TViewModel GetViewModel<TViewModel>(this Flyout window) where TViewModel : ViewModelBase
        {
            return (TViewModel)window.DataContext;
        }

        public static void Close(this Flyout flyout)
        {
            flyout.IsOpen = false;
        }
    }

}
