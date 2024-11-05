using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremeSegments : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            // base case
            if (points.Count < 4)
            {
                foreach (Point p in points) outPoints.Add(p);
                for (int i = 0; i < points.Count; i++)
                    outLines.Add(new Line(points[i], points[(i + 1) % points.Count]));
                outPolygons.Add(new Polygon(outLines));
                return;
            }

            // Iterate over pairs of points to determine if they form part of the convex shape
            for (int i = 0; i < points.Count; i++)
            {
                for (int j = i + 1; j < points.Count; j++)
                {
                    bool allOnSameSide = true;
                    Enums.TurnType? firstTurnType = null;

                    // Check if all other points lie on the same side of the line formed by points[i] and points[j]
                    for (int k = 0; k < points.Count; k++)
                    {
                        if (k == i || k == j) continue;

                        Point vectorIJ = new Point(points[j].X - points[i].X, points[j].Y - points[i].Y);
                        Point vectorIK = new Point(points[k].X - points[i].X, points[k].Y - points[i].Y);

                        Enums.TurnType turnType = HelperMethods.CheckTurn(vectorIJ, vectorIK);

                        if (turnType != Enums.TurnType.Colinear)
                        {
                            if (firstTurnType == null)
                                firstTurnType = turnType;
                            else if (turnType != firstTurnType)
                            {
                                allOnSameSide = false;
                                break;
                            }
                        } 
                        else
                        {
                            if (!HelperMethods.PointOnSegment(points[k], points[i], points[j]))
                            {
                                allOnSameSide = false;
                                break;
                            }
                        }
                    }

                    // If all points are on the same side, add the line and points
                    if (allOnSameSide)
                    {
                        // Lines
                        Line line = new Line(points[i], points[j]);
                        if (!outLines.Contains(line))
                            outLines.Add(line);

                        // points
                        if (!outPoints.Contains(points[i]))
                            outPoints.Add(points[i]);

                        if (!outPoints.Contains(points[j]))
                            outPoints.Add(points[j]);
                    }
                }
            }
        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Segments";
        }
    }
}
