using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Builder.Presentation.Syndication.Posts
{
    [Serializable]
    [XmlRoot("feed")]
    public class Feed
    {
        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlElement("updated")]
        public string Updated { get; set; }

        [XmlArray("posts")]
        public ObservableCollection<Post> Collection { get; set; }

        public Feed()
        {
            Version = "1.19.309";
            Collection = new ObservableCollection<Post>();
        }

        public void Dismiss()
        {
            foreach (Post item in Collection)
            {
                item.Dismiss();
            }
        }

        public void MarkAsRead()
        {
            foreach (Post item in Collection)
            {
                item.MarkAsRead();
            }
        }
    }
}
