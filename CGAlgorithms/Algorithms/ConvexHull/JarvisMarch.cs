using CGUtilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class JarvisMarch : Algorithm
    {
        private double CrossProduct(Point A, Point B, Point C)
        {
            // Vector AB
            Point vectorAB = new Point(B.X - A.X, B.Y - A.Y);
            // Vector AC
            Point vectorAC = new Point(C.X - A.X, C.Y - A.Y);
            return HelperMethods.CrossProduct(vectorAB, vectorAC);
        }
        private double Distance(Point A, Point B)
        {
            return Math.Sqrt((B.X - A.X) * (B.X - A.X) + (B.Y - A.Y) * (B.Y - A.Y));
        }

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            // base case
            if (points.Count < 4)
            {
                foreach (Point p in points) outPoints.Add(p);
                for (int i = 0; i < points.Count; i++)
                {
                    outLines.Add(new Line(points[i], points[(i + 1) % points.Count]));
                }
                outPolygons.Add(new Polygon(outLines));
                return;
            }

            // Rest of the algorithm for more than 3 points

            Point start = points[0];
            foreach (var point in points)
            {
                if (point.X < start.X || (point.X == start.X && point.Y < start.Y))
                    start = point;
            }

            List<Point> hull = new List<Point> { };
            Point current = start;

            do
            {
                Point next = null;
                foreach (var point in points)
                {
                    if (point.Equals(current)) continue;

                    if (next == null)
                    {
                        next = point;
                    }
                    else
                    {
                        double orientation = HelperMethods.Orientation(current, next, point);
                        if (orientation < 0 || (orientation == 0 && Distance(current, point) > Distance(current, next)))
                        {
                            next = point;
                        }
                    }
                }

                hull.Add(next);
                current = next;

            } while (!current.Equals(start));

            outPoints.AddRange(hull);
            for (int i = 0; i < hull.Count - 1; i++)
            {
                outLines.Add(new Line(hull[i], hull[i + 1]));
            }
            outLines.Add(new Line(hull[hull.Count - 1], hull[0]));
        }
        public override string ToString()
        {
            return "Convex Hull - Jarvis March";
        }
    }
}
