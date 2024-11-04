using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CGAlgorithms.Algorithms.ConvexHull
{

    public class GrahamScan : Algorithm
    {

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
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
                    return HelperMethods.distanceSq(lower,a).CompareTo(HelperMethods.distanceSq(lower,b));
                }

                return HelperMethods.Orientation(lower, a, b) < 0 ? -1 : 1;
            });

            

            List<Point> stk = new List<Point> { points[0] };

            for (int i = 1; i < points.Count; i++)
            {
                Point p3 = points[i];
                while (stk.Count > 1 && HelperMethods.CheckTurn(new Line(stk[stk.Count - 2], stk[stk.Count - 1]), p3) != Enums.TurnType.Left)
                {
                    stk.RemoveAt(stk.Count - 1);
                }
                stk.Add(p3);
            }

            if (stk.Count == 2 && stk[stk.Count - 1] == stk[stk.Count - 2]) stk.RemoveAt(stk.Count - 1);

            foreach (Point p in stk) outPoints.Add(p);

            for (int i = 0; i < stk.Count; i++)
            {
                outLines.Add(new Line(stk[i], stk[(i + 1)%stk.Count]));
            }

            outPolygons.Add(new Polygon(outLines));
        }

        public double calcAngle(double x1,double y1,double x2,double y2)
        {
            
            // Calculate the differences
            double deltaY = y2 - y1;
            double deltaX = x2 - x1;

            // Calculate the angle in radians
            double angleRadians = Math.Atan2(deltaY, deltaX);

            // Convert radians to degrees
            double angleDegrees = angleRadians * (180 / Math.PI);
            return angleDegrees;
        }

        public override string ToString()
        {
            return "Convex Hull - Graham Scan";
        }
    }
}
