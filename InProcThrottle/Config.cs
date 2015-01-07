using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InProcThrottle
{
    public class Config
    {
        public decimal Percentage { get; set; }
        public int TimeSpan { get; set; }
        public IManagerCommunicationProvider Provider { get; set; }
    }
}
