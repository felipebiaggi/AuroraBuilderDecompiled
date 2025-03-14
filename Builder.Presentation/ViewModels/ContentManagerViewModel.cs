using Builder.Presentation.ViewModels.Base;
using System.Collections.ObjectModel;

namespace Builder.Presentation.ViewModels
{
    public sealed class ContentManagerViewModel : ViewModelBase
    {
        public ObservableCollection<ContentItem> Items { get; } = new ObservableCollection<ContentItem>();

        public ContentManagerViewModel()
        {
            for (int i = 0; i < 5; i++)
            {
                ContentItem contentItem = new ContentItem($"Index {i + 1}");
                for (int j = 0; j < 10; j++)
                {
                    contentItem.Items.Add(new ContentItem($"Elements {j + 1}"));
                }
                Items.Add(contentItem);
            }
        }
    }
}
