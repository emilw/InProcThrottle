using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InProcThrottle.Client
{
    public enum Status
    {
        Created,
        InProgress,
        ToBeRemoved
    }
    public class ActionQueueItem
    {
        public ActionQueueItem(Action action, string scopeKey)
        {
            ActionItem = action;
            ScopeKey = scopeKey;
            Status = Client.Status.Created;
        }
        public Action ActionItem { get; set; }
        public string ScopeKey { get; set; }
        public Status Status { get; set; }
    }
}
