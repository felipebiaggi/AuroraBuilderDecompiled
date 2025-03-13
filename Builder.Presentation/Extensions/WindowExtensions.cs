using System.Windows;
using Builder.Presentation;
using Builder.Presentation.ViewModels.Base;

namespace Builder.Presentation.Extensions
{
    public static class WindowExtensions
    {
        public static ViewModelBase GetViewModel(this Window window)
        {
            return (ViewModelBase)window.DataContext;
        }

        public static TViewModel GetViewModel<TViewModel>(this Window window) where TViewModel : ViewModelBase
        {
            return (TViewModel)window.DataContext;
        }

        public static void ApplyTheme(this Window window)
        {
            ApplicationManager.Current.SetWindowTheme(window);
        }
    }
}
