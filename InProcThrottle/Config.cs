using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InProcThrottle
{
    public class Config
    {
        public int Percentage { get; set; }
        public int TimeSpan { get; set; }
        public ICommunicationProvider Provider { get; set; }
    }
}
