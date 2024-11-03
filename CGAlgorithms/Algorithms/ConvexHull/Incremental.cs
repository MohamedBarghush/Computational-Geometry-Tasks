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

            // Sort points by x-coordinate, and by y-coordinate if x-coordinates are the same
            points = points.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();

            List<Point> lower = new List<Point>();
            foreach (var p in points)
            {
                // Remove points from the lower hull if they cause a clockwise turn
                while (lower.Count >= 2 && HelperMethods.Orientation(lower[lower.Count - 2], lower[lower.Count - 1], p) <= 0)
                {
                    lower.RemoveAt(lower.Count - 1);
                }
                lower.Add(p);
            }
            List<Point> upper = new List<Point>();
            for (int i = points.Count - 1; i >= 0; i--)
            {
                var p = points[i];
                // Remove points from the upper hull if they cause a clockwise turn
                while (upper.Count >= 2 && HelperMethods.Orientation(upper[upper.Count - 2], upper[upper.Count - 1], p) <= 0)
                {
                    upper.RemoveAt(upper.Count - 1);
                }
                upper.Add(p);
            }

            // Remove the last point of each half as they are duplicated
            lower.RemoveAt(lower.Count - 1);
            upper.RemoveAt(upper.Count - 1);

            // Concatenate lower and upper hulls to get the complete convex hull
            lower.AddRange(upper);

            foreach (Point p in lower) outPoints.Add(p);
            for (int i = 0; i < lower.Count; i++)
            {
                outLines.Add(new Line(lower[i], lower[(i + 1) % lower.Count]));
            }
            outPolygons.Add(new Polygon(outLines));
        }



        public override string ToString()
        {
            return "Convex Hull - Incremental";
        }
    }
}
