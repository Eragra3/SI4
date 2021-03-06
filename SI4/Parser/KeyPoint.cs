﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI4.Parser
{
    public class KeyPoint
    {
        public double X;
        public double Y;

        public double A;
        public double B;
        public double C;

        public int[] Features;

        public double GetDistance(KeyPoint kp)
        {
            return (X - kp.X) * (X - kp.X) + (Y - kp.Y) * (Y - kp.Y);
        }

        public double GetDistance(double otherX, double otherY)
        {
            return (X - otherX) * (X - otherX) + (Y - otherY) * (Y - otherY);
        }
    }
}
