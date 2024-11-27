using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    

    public class Incremental : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            HelperMethods.RemoveDuplicatePoints(ref points);
            // base case
            if (points.Count < 4)
            {
                foreach (Point p in points) outPoints.Add(p);
                for (int i = 0;i < points.Count;i++)
                {
                    outLines.Add(new Line(points[i], points[(i + 1) % points.Count]));
                }
                outPolygons.Add(new Polygon(outLines));
                return;
            }

            double X = 0, Y = 0;
            foreach (var p in points)
            {
                X += p.X;
                Y += p.Y;
            }

            int N = points.Count;
            X /= N;
            Y /= N;

            points.Sort((a, b) =>
            {
                return (Math.Abs(X - a.X) + Math.Abs(Y - a.Y)).CompareTo(Math.Abs(X - b.X) + Math.Abs(Y - b.Y));
                
            });

            List<Point> active = new List<Point>();
            for(int i= 0; i < 3; i++)
            {
                active.Add(points[points.Count - 1]);
                points.RemoveAt(points.Count - 1);
            }
            Point pivot = active.OrderBy(po => po.Y).ThenBy(po => po.X).First();

            active.Sort((a, b) =>
            {
                if (HelperMethods.Orientation(pivot, a, b) == 0)
                {
                    return HelperMethods.distanceSq(pivot, a).CompareTo(HelperMethods.distanceSq(pivot, b));
                }

                return HelperMethods.Orientation(pivot, a, b) < 0 ? -1 : 1;
            });
            while (points.Count>0)
            {
                Point p = points[points.Count - 1];
                points.RemoveAt(points.Count - 1);

                if (!IsPointOutsideConvexPolygon(active, p))
                {

                    continue;
                }

                var (l, r) = FindSupportingPoints(active, p);

                if (l >= r)
                {
                    int tmp = l;
                    l = r;
                    r = tmp;

                }
                List<Point> newValues = new List<Point> { active[l], p, active[r] };

                active.RemoveRange(l, r - l + 1);
                active.InsertRange(l, newValues);

                pivot = active.OrderBy(po => po.Y).ThenBy(po => po.X).First();


                active.Sort((a, b) =>
                {
                    if (HelperMethods.Orientation(pivot, a, b) == 0)
                    {
                        return HelperMethods.distanceSq(pivot, a).CompareTo(HelperMethods.distanceSq(pivot, b));
                    }

                    return HelperMethods.Orientation(pivot, a, b) < 0 ? -1 : 1;
                });
                
            }

            active = HelperMethods.RemoveColliners(ref active);

            //Console.WriteLine(active.Count);
            for(int i = 0; i < active.Count; i++)
            {
                outPoints.Add(active[i]);
                //Console.WriteLine(active[i].X + " " + active[i].Y);
                outLines.Add(new Line(active[i], active[(i + 1) % active.Count]));
            }
            outPolygons.Add(new Polygon(outLines));
        }
        

        public (int, int) FindSupportingPoints(List<Point> vertices, Point point)
        {
            int n = vertices.Count;

            // Binary search for left supporting point
            int left = 0, right = n - 1;
            while (right - left > 1)
            {
                int mid = (left + right) / 2;
                double cross = CrossProduct(vertices[0], vertices[mid], point);

                if (cross > 0) // Point is to the left
                    right = mid;
                else
                    left = mid;
            }
            int leftSupport = right;

            // Binary search for right supporting point
            left = 0; right = n - 1;
            while (right - left > 1)
            {
                int mid = (left + right) / 2;
                double cross = CrossProduct(vertices[0], vertices[mid], point);

                if (cross < 0) // Point is to the right
                    left = mid;
                else
                    right = mid;
            }
            int rightSupport = left;

            return (leftSupport, rightSupport);
        }

        double CrossProduct(Point a, Point b, Point p)
        {
            return (b.X - a.X) * (p.Y - a.Y) - (b.Y - a.Y) * (p.X - a.X);
        }
        public bool IsPointOutsideConvexPolygon(List<Point> vertices, Point point)
        {
            int n = vertices.Count;
            bool isPositive = false;

            for (int i = 0; i < n; i++)
            {
                var P1 = vertices[i];
                var P2 = vertices[(i + 1) % n]; // Wrap around to the first vertex

                // Compute the cross product to determine the orientation
                double crossProduct = (P2.X - P1.X) * (point.Y - P1.Y) - (P2.Y - P1.Y) * (point.X - P1.X);

                // If the point is on the edge, it's neither inside nor outside (return false).
                if (crossProduct == 0)
                {
                    // Check if the point lies within the segment
                    if ((point.X >= Math.Min(P1.X, P2.X) && point.X <= Math.Max(P1.X, P2.X)) &&
                        (point.Y >= Math.Min(P1.Y, P2.Y) && point.Y <= Math.Max(P1.Y, P2.Y)))
                    {
                        return false; // Point is on the edge, it's not outside
                    }
                }

                // Update the orientation for the first edge
                if (i == 0)
                {
                    isPositive = crossProduct > 0;
                }
                else
                {
                    // If the cross product signs are different, the point is outside
                    if ((crossProduct > 0) != isPositive)
                        return true; // Point is outside the polygon
                }
            }

            // If all cross products have the same sign, the point is inside (not outside)
            return false;
        }




        public override string ToString()
        {
            return "Convex Hull - Incremental";
        }
    }
}
