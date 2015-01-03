using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InProcThrottle.Client
{
    public class ActionQueueItem
    {
        public Action ActionItem { get; set; }
        public string ScopeKey { get; set; }
    }
}
