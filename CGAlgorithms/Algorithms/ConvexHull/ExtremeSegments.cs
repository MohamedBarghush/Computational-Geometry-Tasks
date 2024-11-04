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
            // Handle cases where the number of points is 1, 2, or 3 directly
            if (points.Count == 1)
            {
                outPoints.Add(points[0]);
                return;
            }
            else if (points.Count == 2)
            {
                outPoints.Add(points[0]);
                outPoints.Add(points[1]);
                outLines.Add(new Line(points[0], points[1]));
                return;
            }
            else if (points.Count == 3)
            {
                outPoints.Add(points[0]);
                outPoints.Add(points[1]);
                outLines.Add(new Line(points[0], points[1]));
                outPoints.Add(points[2]);
                outLines.Add(new Line(points[1], points[2]));
                outLines.Add(new Line(points[2], points[0]));
                return;
            }

            // Remove duplicate points from the input
            HelperMethods.RemoveDuplicatePoints(ref points);

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
                            {
                                firstTurnType = turnType;
                            }
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
                        Line line = new Line(points[i], points[j]);

                        if (!outLines.Contains(line))
                        {
                            outLines.Add(line);
                        }

                        if (!outPoints.Contains(points[i]))
                        {
                            outPoints.Add(points[i]);
                        }

                        if (!outPoints.Contains(points[j]))
                        {
                            outPoints.Add(points[j]);
                        }
                    }
                }
            }
            foreach (Point p in outPoints)
            {
                Console.WriteLine(p.X + " " + p.Y);
            }

        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Segments";
        }
    }
}
