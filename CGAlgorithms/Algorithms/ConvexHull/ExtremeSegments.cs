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
            //for (int i = 0; i < points.Count; i++)
            //{
            //    Debug.Print(points[i].X + " " + points[i].Y);
            //}

            for (int i = 0; i < points.Count; i++)
            {
                for (int j = 0; j < points.Count; j++)
                {
                    if (i == j) continue;

                    bool allOnSameSide = true;
                    Enums.TurnType? firstTurnType = null;

                    // Adding lines
                    //if (lines.Contains(new Line(points[i], points[j])) == false) lines.Add(new Line(points[i], points[j]));

                    for (int k = 0; k < points.Count; k++)
                    {
                        if (j == k || i == k) continue;

                        Point vectorIJ = new Point(points[j].X - points[i].X, points[j].Y - points[i].Y);
                        Point vectorIK = new Point(points[k].X - points[i].X, points[k].Y - points[i].Y);
                        
                        Enums.TurnType turnType = HelperMethods.CheckTurn(vectorIJ, vectorIK);

                        if (firstTurnType == null && turnType != Enums.TurnType.Colinear)
                        {
                            firstTurnType = turnType;
                        }
                        else if (turnType != Enums.TurnType.Colinear && turnType != firstTurnType)
                        {
                            allOnSameSide = false;
                            break;
                        }
                    }

                    if (allOnSameSide)
                    {
                        outLines.Add(new Line(points[i], points[j]));
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
