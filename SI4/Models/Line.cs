using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI4.Models
{
    public struct KeyPointsPair
    {
        public double X1;
        public double Y1;

        public double X2;
        public double Y2;

        public KeyPointsPair(double x1, double y1, double x2, double y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }
    }
}
