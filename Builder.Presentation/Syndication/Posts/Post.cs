using Builder.Core;
using System;
using System.Windows.Input;
using System.Xml.Serialization;

namespace Builder.Presentation.Syndication.Posts
{
    [Serializable]
    [XmlType("post")]
    public class Post : ObservableObject
    {
        private bool _isNew;

        private bool _isDismissed;

        [XmlAttribute("id")]
        public string Identifier { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("content")]
        public string Content { get; set; }

        [XmlElement("url")]
        public string Url { get; set; }

        [XmlElement("date")]
        public string Date { get; set; }

        [XmlElement("image")]
        public string Image { get; set; }

        [XmlIgnore]
        public bool IsNew
        {
            get
            {
                return _isNew;
            }
            set
            {
                SetProperty(ref _isNew, value, "IsNew");
            }
        }

        [XmlAttribute("dismissed")]
        public bool IsDismissed
        {
            get
            {
                return _isDismissed;
            }
            set
            {
                SetProperty(ref _isDismissed, value, "IsDismissed");
            }
        }

        public ICommand DismissCommand => new RelayCommand(Dismiss, () => !IsDismissed);

        public ICommand ViewCommand => new RelayCommand(delegate
        {
            Process.Start(Url);
            MarkAsRead();
        });

        [XmlElement("meta")]
        public PostMeta Meta { get; set; } = new PostMeta();

        public Post()
        {
            _isNew = true;
        }

        public void Dismiss()
        {
            IsDismissed = true;
            MarkAsRead();
        }

        public void MarkAsRead()
        {
            IsNew = false;
        }

        public bool ShouldSerializeIsDismissed()
        {
            return IsDismissed;
        }
    }
}
