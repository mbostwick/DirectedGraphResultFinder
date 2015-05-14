using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirectGraphResultFinder.Model
{
    public class LinkedData
    {
        public const char datapoint_column_separator = '|';
        public const char link_line_separator = '\n';

        public int edge_id { get; set; }

        public DataPoint[] linkedPoints { get; set; }

        public LinkedData(DataPoint pointA, DataPoint pointB)
        {
            linkedPoints = new DataPoint[] { pointA, pointB };
        }
        public LinkedData(int edge_id, DataPoint pointA, DataPoint pointB):this(pointA,pointB)
        {
            this.edge_id = edge_id;
        }

        public DataPoint pointA
        {
            get
            {
                return linkedPoints[0];
            }
        }

        public DataPoint pointB
        {
            get
            {
                return linkedPoints[1];
            }
        }
    }
}
