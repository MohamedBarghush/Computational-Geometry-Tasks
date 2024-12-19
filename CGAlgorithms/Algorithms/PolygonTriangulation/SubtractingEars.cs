using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{

    class Handler
    {
        public void sort_counterclock(ref List<Point> points)
        {
            Point lower = points[0];
            foreach (Point p in points)
            {
                if (p.Y < lower.Y || (p.Y == lower.Y && p.X < lower.X))
                {
                    lower = p;
                }
            }

            points.Sort((a, b) =>
            {
                if (HelperMethods.Orientation(lower, a, b) == 0)
                {
                    return HelperMethods.distanceSq(lower, a).CompareTo(HelperMethods.distanceSq(lower, b));
                }

                return HelperMethods.Orientation(lower, a, b) < 0 ? -1 : 1;
            });
        }
        public bool IsCounterClockwise(ref List<Point> polygon)
        {
            // Calculate the signed area of the polygon
            double sum = 0;
            for (int i = 0; i < polygon.Count; i++)
            {
                var p1 = polygon[i];
                var p2 = polygon[(i + 1) % polygon.Count];
                sum += (p2.X - p1.X) * (p2.Y + p1.Y);
            }
            return sum < 0;
        }
        public double CrossProduct(Point p1, Point p2, Point p3)
        {
            return (p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X);
        }
        public bool IsEar(Point p1, Point p2, Point p3, ref List<Point> polygon)
        {
            // Check if the triangle is convex
            if (CrossProduct(p1, p2, p3) <= 0)
            {
                return false; // Not convex
            }
            Console.WriteLine("cross!");
            // Check if any other point lies inside the triangle
            foreach (var point in polygon)
            {

                Console.WriteLine("before is!");
                if (point != p1 && point != p2 && point != p3 && IsPointInTriangle(point, p1, p2, p3))
                {

                    Console.WriteLine("ret is!");
                    return false; // A point is inside, so not an ear
                }

                Console.WriteLine("after is!");

            }
            Console.WriteLine("is true!");

            return true;
        }

        public bool IsPointInTriangle(Point pt, Point p1, Point p2, Point p3)
        {
            // Use barycentric coordinates to check if the point is inside the triangle
            double area = Math.Abs(CrossProduct(p1, p2, p3));
            double area1 = Math.Abs(CrossProduct(pt, p2, p3));
            double area2 = Math.Abs(CrossProduct(p1, pt, p3));
            double area3 = Math.Abs(CrossProduct(p1, p2, pt));

            return Math.Abs(area - (area1 + area2 + area3)) < 1e-9;
        }
    };
    class SubtractingEars : Algorithm
    {
        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            Handler handler = new Handler();
            //handler.sort_counterclock(ref points);
            if (!handler.IsCounterClockwise(ref points))
            {
                handler.sort_counterclock(ref points);
            }
            // To show source polygon
            for (int i = 0; i < points.Count; i++)
            {
                outLines.Add(new Line(points[i], points[(i + 1) % points.Count]));
            }
            return;
            ////Console.WriteLine("Here");

            while (points.Count > 3)
            {
                bool earFound = false;
                Console.WriteLine("proc!");
                for (int i = 0; i < points.Count; i++)
                {
                    int prev = (i - 1 + points.Count) % points.Count;
                    int next = (i + 1) % points.Count;

                    var p1 = points[prev];
                    var p2 = points[i];
                    var p3 = points[next];
                    Console.WriteLine("H!");

                    if (handler.IsEar(p1, p2, p3, ref points))
                    {

                        //List<Line> triangle = new List<Line>();
                        outLines.Add(new Line(p1, p2));
                        outLines.Add(new Line(p2, p3));
                        outLines.Add(new Line(p3, p1));

                        //outPolygons.Add(new Polygon(triangle));

                        points.RemoveAt(i);

                        earFound = true;
                        break;
                    }
                }

                if (!earFound)
                {
                    Console.WriteLine("NO ears!");
                }
            }

            if (points.Count == 3)
            {
                List<Line> triangle = new List<Line>();
                outLines.Add(new Line(points[0], points[1]));
                outLines.Add(new Line(points[1], points[2]));
                outLines.Add(new Line(points[2], points[0]));

                //outPolygons.Add(new Polygon(triangle));

            }

        }

        public override string ToString()
        {
            return "Subtracting Ears";
        }
    }
}
