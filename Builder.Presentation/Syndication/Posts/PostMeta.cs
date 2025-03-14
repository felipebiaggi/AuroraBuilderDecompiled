using System;
using System.Xml.Serialization;

namespace Builder.Presentation.Syndication.Posts
{
    [Serializable]
    public class PostMeta
    {
        [XmlElement("categories")]
        public string Categories { get; set; }

        [XmlElement("tags")]
        public string Tags { get; set; }
    }
}
