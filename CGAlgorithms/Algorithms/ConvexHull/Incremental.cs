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

            
            List<Point> stk = new List<Point>(); // tracking the current polygon
            int startIdx = -1;

            // find non colinear 3 points to start with
            for(int i = 0; i < points.Count; i++)
            {
                if (HelperMethods.CheckTurn(new Line(points[i], points[(i + 1)%points.Count]), points[(i + 2)%points.Count]) == Enums.TurnType.Colinear) continue;

                // The current 3 points are valid
                stk.Add(points[i]);
                stk.Add(points[(1+i)%points.Count]);
                stk.Add(points[(2+i)%points.Count]);
                startIdx = i;
                

                break;
            }

            if (startIdx == -1) // they are all colinear
            {
                foreach (Point p in points) outPoints.Add(p);
                for (int i = 0; i < points.Count; i++)
                {
                    outLines.Add(new Line(points[i], points[(i + 1) % points.Count]));
                }
                outPolygons.Add(new Polygon(outLines));
                return;
            }

            for(int idx = 3; idx < points.Count; idx++)
            {
                int i = (idx + startIdx) % points.Count;
                Point curP = points[i];

            }


        }

        public static bool IsPointInsideConvexPolygon(Polygon polygon, Point point)
        {
            int n = polygon.lines.Count;

            // Handle trivial cases
            if (n < 3) return false;

            // Binary search to find the correct edge
            int left = 1, right = n - 1;
            while (left < right)
            {
                int mid = (left + right) / 2;
                // Check if the point is to the left or right of the edge
                if (HelperMethods.CrossProduct(polygon.lines[0], polygon.lines[mid], point) < 0)
                    right = mid; // Move to the left half
                else
                    left = mid + 1; // Move to the right half
            }

            // Check if the point is on the left or right of the found edge
            return CrossProduct(polygon[left - 1], polygon[left % n], point) >= 0;
        }

        public override string ToString()
        {
            return "Convex Hull - Incremental";
        }
    }
}
