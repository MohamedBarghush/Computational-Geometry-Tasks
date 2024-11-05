using CGUtilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremePoints : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            HelperMethods.RemoveDuplicatePoints(ref points);
            
            List<Point> resultPoints = new List<Point>(points);

            foreach (Point i in points)
            {
                bool flagCheck = InTriangle(i, points);

                if (flagCheck)
                    resultPoints.Remove(i);
            }

            outPoints = resultPoints;
        }

        private static bool InTriangle(Point i, List<Point> points)
        {
            foreach (Point j in points)
            {
                if (i == j) continue;
                foreach (Point k in points)
                {
                    if (i == k || j == k) continue;
                    foreach (Point l in points)
                    {
                        if (i == l || j == l || k == l) continue;
                        if (HelperMethods.PointInTriangle(i, j, k, l) != Enums.PointInPolygon.Outside)
                            return true;
                    }
                }
            }
            return false;
        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Points";
        }
    }
}
