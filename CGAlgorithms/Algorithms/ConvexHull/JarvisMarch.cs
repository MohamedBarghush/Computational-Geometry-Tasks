using CGUtilities;
using System;
using System.Collections.Generic;
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
                } else if (points.Count == 2)
                {
                    outLines.Add(new Line(points[0], points[1]));
                }
                return;
            }

            Point start = points.Aggregate((min, p) =>
                p.Y < min.Y || (p.Y == min.Y && p.X < min.X) ? p : min);

            Point current = start;
            outPoints.Add(current);

            do
            {
                Point next = points[0];

                foreach (var candidate in points)
                {
                    if (candidate == current) continue;

                    double cross = CrossProduct(current, next, candidate);
                    if (next == current || cross > 0 || (cross == 0 && Distance(current, candidate) > Distance(current, next)))
                    {
                        next = candidate;
                    }
                }

                // Add the line from current to next
                outLines.Add(new Line(current, next));
                outPoints.Add(next);

                current = next;

            } while (current != start);
        }
        public override string ToString()
        {
            return "Convex Hull - Jarvis March";
        }
    }
}
