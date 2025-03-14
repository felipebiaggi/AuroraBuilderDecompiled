using System.Windows.Controls;

namespace Builder.Presentation.Extensions
{
    public static class UserControlExtensions
    {
        public static ViewModelBase GetViewModel(this UserControl control)
        {
            return (ViewModelBase)(control?.DataContext);
        }

        public static TViewModel GetViewModel<TViewModel>(this UserControl control) where TViewModel : ViewModelBase
        {
            return (TViewModel)(control?.DataContext);
        }
    }
}
