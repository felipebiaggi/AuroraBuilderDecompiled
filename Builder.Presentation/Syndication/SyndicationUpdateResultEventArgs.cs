using Builder.Presentation.Syndication.Posts;
using System;
using System.Collections.Generic;

namespace Builder.Presentation.Syndication
{
    public class SyndicationUpdateResultEventArgs : EventArgs
    {
        public IEnumerable<Post> NewPosts { get; }

        public SyndicationUpdateResultEventArgs(IEnumerable<Post> newPosts)
        {
            NewPosts = newPosts;
        }
    }
}
