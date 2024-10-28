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
            if (points.Count < 4)
            {
                foreach (Point p in points)
                {
                    outPoints.Add(p);
                }
                outLines.Add(new Line(points[0], points[1]));
                outLines.Add(new Line(points[1], points[2]));
                outLines.Add(new Line(points[2], points[0]));
                return;
            }

            Point lower = points[0];
            foreach (Point p in points)
            {
                if (p.Y < lower.Y || (p.Y == lower.Y && p.X < lower.X))
                {
                    lower = p;
                }
            }

            points.Sort((a, b) => calcAngle(lower.X, lower.Y, a.X, a.Y).CompareTo(calcAngle(lower.X, lower.Y, b.X, b.Y)));

            List<Point> uniquePoints = new List<Point> { lower };
            for (int i = 1; i < points.Count; i++)
            {
                while (i < points.Count - 1 &&
                       Math.Abs(calcAngle(lower.X, lower.Y, points[i].X, points[i].Y) - calcAngle(lower.X, lower.Y, points[i + 1].X, points[i + 1].Y)) < 1e-9)
                {
                    i++;
                }
                uniquePoints.Add(points[i]);
            }

            List<Point> stk = new List<Point> { uniquePoints[0], uniquePoints[1] };

            for (int i = 2; i < uniquePoints.Count; i++)
            {
                Point p3 = uniquePoints[i];
                while (stk.Count > 1 && HelperMethods.CheckTurn(new Line(stk[stk.Count - 2], stk[stk.Count - 1]), p3) != Enums.TurnType.Left)
                {
                    stk.RemoveAt(stk.Count - 1);
                }
                stk.Add(p3);
            }

            foreach (Point p in stk) outPoints.Add(p);
            stk.Add(stk[0]);

            for (int i = 0; i < stk.Count - 1; i++)
            {
                outLines.Add(new Line(stk[i], stk[i + 1]));
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
