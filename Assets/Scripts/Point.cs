using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.Scripts
{
    public class Point
    {
        public int X;

        public int Y;

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }


        public static Point operator +(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }

        public static Point operator -(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }

        public static Point operator -(Point a)
        {
            return new Point(-a.X, -a.Y);
        }

        public static Point operator *(Point a, int d)
        {
            return new Point(a.X * d, a.Y * d);
        }

        public static Point operator *(int d, Point a)
        {
            return new Point(a.X * d, a.Y * d);
        }

        public static Point operator /(Point a, int d)
        {
            return new Point(a.X / d, a.Y / d);
        }

        public static implicit operator Vector2(Point p)
        {
            return new Vector2(p.X, p.Y);
        }
    }
}
