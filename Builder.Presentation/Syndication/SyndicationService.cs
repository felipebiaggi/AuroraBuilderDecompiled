using Builder.Presentation.Syndication.Posts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Builder.Presentation.Syndication
{
    public class SyndicationService
    {
        public string SyndicationUrl { get; set; }

        public string StorageDirectory { get; set; }

        public Feed Feed { get; private set; }

        public event EventHandler Updating;

        public event EventHandler<SyndicationUpdateProgressEventArgs> UpdateProgress;

        public event EventHandler<SyndicationUpdateResultEventArgs> UpdateCompleted;

        public SyndicationService(string syndicationUrl, string storageDirectory)
        {
            SyndicationUrl = syndicationUrl;
            if (Directory.Exists(storageDirectory))
            {
                StorageDirectory = storageDirectory;
            }
            else
            {
                Directory.CreateDirectory(storageDirectory);
                StorageDirectory = storageDirectory;
            }
            try
            {
                if (!Load())
                {
                    Feed = new Feed();
                }
            }
            catch (Exception)
            {
                if (File.Exists(GetFilepath()))
                {
                    File.Delete(GetFilepath());
                }
                Feed = new Feed();
            }
        }

        public bool Update(bool forced = false, bool storeThumbnail = false)
        {
            OnUpdating();
            if (forced)
            {
                Feed.Updated = string.Empty;
                Feed.Collection.Clear();
            }
            List<Post> list = new List<Post>();
            try
            {
                using (XmlReader reader = XmlReader.Create(SyndicationUrl, new XmlReaderSettings
                {
                    Async = true
                }))
                {
                    SyndicationFeed syndicationFeed = SyndicationFeed.Load(reader);
                    int num = syndicationFeed.Items.Count();
                    int num2 = 0;
                    Feed.Updated = syndicationFeed.LastUpdatedTime.ToString();
                    SyndicationUpdateProgressEventArgs syndicationUpdateProgressEventArgs = new SyndicationUpdateProgressEventArgs(0);
                    foreach (SyndicationItem item in syndicationFeed.Items.Reverse())
                    {
                        Post post = ParseSyndicationItem(item);
                        if (storeThumbnail)
                        {
                            StoreThumbnail(post);
                        }
                        num2++;
                        syndicationUpdateProgressEventArgs.ProgressPercentage = (int)((double)num2 / (double)num * 100.0);
                        if (!Feed.Collection.Any((Post x) => x.Identifier.Equals(post.Identifier)))
                        {
                            Feed.Collection.Insert(0, post);
                            list.Add(post);
                        }
                        OnUpdateProgress(syndicationUpdateProgressEventArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
            OnUpdateCompleted(new SyndicationUpdateResultEventArgs(list));
            return true;
        }

        private Post ParseSyndicationItem(SyndicationItem syndicationItem)
        {
            if (syndicationItem == null)
            {
                throw new ArgumentNullException("syndicationItem");
            }
            Post post = new Post();
            if (!string.IsNullOrWhiteSpace(syndicationItem.Id) && syndicationItem.Id.Contains("?p="))
            {
                post.Identifier = syndicationItem.Id.Split('?').Last().TrimStart('p', '=');
            }
            else
            {
                post.Identifier = Guid.NewGuid().ToString();
                Trace.WriteLine("post assigned with a guid: " + post.Identifier);
            }
            post.Title = WebUtility.HtmlDecode(syndicationItem.Title.Text);
            post.Content = WebUtility.HtmlDecode(syndicationItem.Summary.Text);
            post.Date = syndicationItem.PublishDate.ToString();
            post.Meta.Categories = string.Join(", ", syndicationItem.Categories.Select((SyndicationCategory x) => x.Name));
            post.Url = syndicationItem.Links.First().Uri.AbsoluteUri;
            foreach (SyndicationElementExtension elementExtension in syndicationItem.ElementExtensions)
            {
                XElement @object = elementExtension.GetObject<XElement>();
                if (elementExtension.OuterName.Equals("post-tags") && !string.IsNullOrWhiteSpace(@object.Value))
                {
                    post.Meta.Tags = @object.Value;
                }
                if (elementExtension.OuterName.Equals("post-thumbnail"))
                {
                    post.Image = @object.Element("url")?.Value;
                }
            }
            return post;
        }

        private void StoreThumbnail(Post post)
        {
            string extension = Path.GetExtension(post.Image);
            string fileName = Path.Combine(StorageDirectory, post.Identifier + extension);
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFileAsync(new Uri(post.Image), fileName);
            }
        }

        public string GetFilepath()
        {
            return Path.Combine(StorageDirectory, "posts.dat");
        }

        public void Save()
        {
            string outputFileName = Path.Combine(StorageDirectory, "posts.dat");
            XmlWriterSettings settings = new XmlWriterSettings
            {
                IndentChars = "\t",
                Indent = true,
                OmitXmlDeclaration = false,
                NamespaceHandling = NamespaceHandling.OmitDuplicates
            };
            using (XmlWriter xmlWriter = XmlWriter.Create(outputFileName, settings))
            {
                XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
                xmlSerializerNamespaces.Add(string.Empty, string.Empty);
                new XmlSerializer(typeof(Feed)).Serialize(xmlWriter, Feed, xmlSerializerNamespaces);
            }
        }

        public bool Load()
        {
            string path = Path.Combine(StorageDirectory, "posts.dat");
            if (!File.Exists(path))
            {
                return false;
            }
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Feed));
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                Feed = xmlSerializer.Deserialize(stream) as Feed;
            }
            return Feed != null;
        }

        public void Reload()
        {
            if (!File.Exists(Path.Combine(StorageDirectory, "posts.dat")))
            {
                Update(forced: true);
                Save();
            }
        }

        protected virtual void OnUpdating()
        {
            this.Updating?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnUpdateCompleted(SyndicationUpdateResultEventArgs args)
        {
            this.UpdateCompleted?.Invoke(this, args);
        }

        protected virtual void OnUpdateProgress(SyndicationUpdateProgressEventArgs e)
        {
            this.UpdateProgress?.Invoke(this, e);
        }
    }

}
