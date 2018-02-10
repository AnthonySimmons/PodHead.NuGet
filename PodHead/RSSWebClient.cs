using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodHead
{
    class RssWebClient : WebClient
    {
        public Subscription Subscription { get; set; }

        public int MaxItems { get; set; }

        public RssWebClient(Subscription subscription)
        {
            Subscription = subscription;
            MaxItems = subscription.MaxItems;
        }
    }
}
