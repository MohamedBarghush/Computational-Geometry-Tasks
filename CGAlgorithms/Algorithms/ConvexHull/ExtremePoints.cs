using CGUtilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremePoints : Algorithm
    {
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
            // Find the extreme points by exterminating the points that are inside the triangle formed by any 3 points
            HashSet<Point> extremePoints = new HashSet<Point>(points);
            for (int i = 0; i < points.Count; i++)
            {
                for (int j = 0; j < points.Count; j++)
                {
                    if (i == j) continue;
                    for (int k = 0; k < points.Count; k++)
                    {
                        if (i == k || j == k) continue;
                        for (int l = 0; l < points.Count; l++)
                        {
                            if (i == l || j == l || k == l) continue;
                            if (HelperMethods.PointInTriangle(points[i], points[j], points[k], points[l]) == Enums.PointInPolygon.Inside)
                            {
                                extremePoints.Remove(points[i]);
                                break;
                            }
                        }
                    }
                }
            }
            outPoints = extremePoints.ToList();
        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Points";
        }
    }
}
