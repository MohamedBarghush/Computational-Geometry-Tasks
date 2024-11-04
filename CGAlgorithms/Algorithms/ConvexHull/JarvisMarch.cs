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
            if (points.Count <= 3)
            {
                for (int i = 0; i < points.Count; i++)
                    outPoints.Add(points[i]);
                if (points.Count == 3)
                {
                    outLines.Add(new Line(points[0], points[1]));
                    outLines.Add(new Line(points[1], points[2]));
                    outLines.Add(new Line(points[2], points[0]));
                }
                else if (points.Count == 2)
                {
                    outLines.Add(new Line(points[0], points[1]));
                }
                return;
            }

            // Step 1: Find the leftmost point to start the convex hull (lexicographically smallest)
            Point start = points[0];
            foreach (var point in points)
            {
                if (point.X < start.X || (point.X == start.X && point.Y < start.Y))
                    start = point;
            }

            // Initialize the hull list and add the start point to it
            List<Point> hull = new List<Point> { start };
            Point current = start;

            do
            {
                // Step 2(a): Find the angularly rightmost point with respect to the current point
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
                        // Use orientation to check if 'point' is more "right" than 'next'
                        double orientation = HelperMethods.Orientation(current, next, point);
                        if (orientation < 0 || (orientation == 0 && Distance(current, point) > Distance(current, next)))
                        {
                            next = point;
                        }
                    }
                }

                // Step 2(b): Add the chosen point to the hull
                hull.Add(next);
                current = next;

            } while (!current.Equals(start)); // Continue until we loop back to the starting point

            // Step 3: Output the result
            outPoints.AddRange(hull);
            HelperMethods.RemoveDuplicatePoints(ref outPoints);
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
