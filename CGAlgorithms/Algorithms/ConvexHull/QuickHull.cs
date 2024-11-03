using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class QuickHull : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count < 3)
            {
                for (int i = 0; i < points.Count; i++)
                    outPoints.Add(points[i]);
                return;
            }
                
            // Step 1: Get the extreme points
            Point minX = points.OrderBy(p => p.X).First();
            Point maxX = points.OrderByDescending(p => p.X).First();
            Point minY = points.OrderBy(p => p.Y).First();
            Point maxY = points.OrderByDescending(p => p.Y).First();

            // Add these points to the convex hull list (initial boundary)
            List<Point> convexHull = new List<Point> { minX, maxX };

            List<Point> above = new List<Point>();
            List<Point> below = new List<Point>();

            foreach (var point in points)
            {
                if (point != minX && point != maxX)
                {
                    if (IsAboveLine(minX, maxX, point))
                        above.Add(point);
                    else
                        below.Add(point);
                }
            }

            // Step 3: Find the convex hull on each side recursively
            FindHull(minX, maxX, above, convexHull);
            FindHull(maxX, minX, below, convexHull);
            convexHull = SortPoints(convexHull);

            // Step 4: Add the result to the output list
            outPoints = convexHull;

            for (int i = 0; i < convexHull.Count; i++)
            {
                Point start = convexHull[i];
                Point end = convexHull[(i + 1) % convexHull.Count]; // Wrap around to form a closed loop
                outLines.Add(new Line(start, end));
            }
        }
        private List<Point> SortPoints(List<Point> points)
        {
            // Use the centroid of the hull points to sort them in counterclockwise order
            Point center = new Point(points.Average(p => p.X), points.Average(p => p.Y));
            return points.OrderBy(p => Math.Atan2(p.Y - center.Y, p.X - center.X)).ToList();
        }
        private void FindHull(Point p1, Point p2, List<Point> points, List<Point> hull)
        {
            if (points.Count == 0)
                return;

            // Find the point farthest from the line segment p1-p2
            Point farthest = points.OrderByDescending(p => DistanceFromLine(p1, p2, p)).First();
            hull.Add(farthest);

            // Split into two sets: points that lie to the left of p1-farthest and farthest-p2
            List<Point> leftSet1 = new List<Point>();
            List<Point> leftSet2 = new List<Point>();

            foreach (var point in points)
            {
                if (point != farthest)
                {
                    if (IsAboveLine(p1, farthest, point))
                        leftSet1.Add(point);
                    else if (IsAboveLine(farthest, p2, point))
                        leftSet2.Add(point);
                }
            }

            // Recursively find the hull points on each side
            FindHull(p1, farthest, leftSet1, hull);
            FindHull(farthest, p2, leftSet2, hull);
        }

        private bool IsAboveLine(Point a, Point b, Point c)
        {
            return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X) > 0;
        }

        private double DistanceFromLine(Point a, Point b, Point c)
        {
            return Math.Abs((b.Y - a.Y) * c.X - (b.X - a.X) * c.Y + b.X * a.Y - b.Y * a.X) /
                   Math.Sqrt(Math.Pow(b.Y - a.Y, 2) + Math.Pow(b.X - a.X, 2));
        }

        public override string ToString()
        {
            return "Convex Hull - Quick Hull";
        }

    }
}
