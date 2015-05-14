using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirectGraphResultFinder.Model
{
    public class DataPoint
    {
        public DataPoint() { }

        public DataPoint(string name):base()
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}
