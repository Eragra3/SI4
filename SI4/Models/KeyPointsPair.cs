using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SI4.Parser;

namespace SI4.Models
{
    public class KeyPointsPair
    {
        public KeyPoint KeyPoint1;
        public KeyPoint KeyPoint2;

        public KeyPointsPair(KeyPoint kp1, KeyPoint kp2)
        {
            KeyPoint1 = kp1;
            KeyPoint2 = kp2;
        }
    }
}
