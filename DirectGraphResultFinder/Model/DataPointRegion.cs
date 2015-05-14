using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirectGraphResultFinder.Model
{
    public class DataPointRegion
    {
        public DataPointRegion(LinkedData[] links)
        {
            this.links = links;
        }

        public DataPointRegion(string name, LinkedData[] links):this(links)
        {
            this.name = name;
        }

        public string name { get; set; }

        public LinkedData[] links { get; set; }
    }
}
