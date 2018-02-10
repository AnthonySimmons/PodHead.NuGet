using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodHead
{
    class RssWebClient : WebClient
    {
        public PodcastFeed Subscription { get; set; }

        public int MaxItems { get; set; }

        public RssWebClient(PodcastFeed subscription)
        {
            Subscription = subscription;
            MaxItems = subscription.MaxItems;
        }
    }
}
