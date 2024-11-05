using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class DivideAndConquer : Algorithm
    {
        public List<Point> divide(List<Point> points)
        {
            if (points.Count < 3)
            {
                return points;
            }
            List<Point> outPut = new List<Point>();

            Point a = points[0];
            Point b = points[1];
            Point c = points[2];
            Enums.TurnType coo = HelperMethods.CheckTurn(new Line(a, b), c);
            if (points.Count == 3 && coo != Enums.TurnType.Colinear)
            {
                outPut = points;
                return outPut;
            }
            List<Point> Rightside = new List<Point>();
            List<Point> Left = new List<Point>();
            for (int i = points.Count / 2; i < points.Count; i++)
            {
                Left.Add(points[i]);
            }

            for (int i = 0; i < points.Count / 2; i++)
            {
                Rightside.Add(points[i]);
            }
            Left = divide(Left);
            Rightside = divide(Rightside);

            // MERGE
            double max_Y_L = double.MinValue;
            Point max_L = new Point(2, 3);
            for (int i = 0; i < Left.Count; i++)
            {
                if (Left[i].Y > max_Y_L)
                {
                    max_Y_L = Left[i].Y;
                    max_L = Left[i];
                }
            }
            double max_Y_R = double.MinValue;
            Point max_R = new Point(2, 3);
            for (int i = 0; i < Rightside.Count; i++)
            {
                if (Rightside[i].Y > max_Y_R)
                {
                    max_Y_R = Rightside[i].Y;
                    max_R = Rightside[i];
                }
            }

            double min_Y_R = double.MaxValue;
            Point min_R = new Point(2, 3);
            for (int i = 0; i < Rightside.Count; i++)
            {
                if (Rightside[i].Y < min_Y_R)
                {
                    min_Y_R = Rightside[i].Y;
                    min_R = Rightside[i];
                }
            }
            double min_Y_Left = double.MaxValue;
            Point min_Left = new Point(2, 3);
            for (int i = 0; i < Left.Count; i++)
            {
                if (Left[i].Y < min_Y_Left)
                {
                    min_Y_Left = Left[i].Y;
                    min_Left = Left[i];
                }
            }

            Line right = new Line(max_R, min_R);
            for (int i = 0; i < Rightside.Count; i++)
            {
                Enums.TurnType cr = HelperMethods.CheckTurn(right, Rightside[i]);
                if (cr == Enums.TurnType.Right || cr == Enums.TurnType.Colinear || Rightside[i].Y > max_L.Y || Rightside[i].Y < min_Left.Y)
                    if (!outPut.Contains(Rightside[i]))
                        outPut.Add(Rightside[i]);

            }

            Line left = new Line(max_L, min_Left);
            for (int i = 0; i < Left.Count; i++)
            {
                Enums.TurnType cl = HelperMethods.CheckTurn(left, Left[i]);
                if (cl == Enums.TurnType.Left || cl == Enums.TurnType.Colinear)
                    if (!outPut.Contains(Left[i]))
                        outPut.Add(Left[i]);
            }


            for (int i = 0; i < outPut.Count; i++)
            {
                if (outPut[i].X < max_L.X && outPut[i].X > min_Left.X)
                    outPut.RemoveAt(i);
            }
            for (int i = 1; i < outPut.Count - 1; i++)
            {
                if (HelperMethods.PointOnSegment(outPut[i], outPut[i - 1], outPut[i + 1]))
                    outPut.RemoveAt(i);
            }
            for (int i = 2; i < outPut.Count; i++)
            {
                if (HelperMethods.PointOnSegment(outPut[i], outPut[i - 1], outPut[i - 2]))
                    outPut.RemoveAt(i);
            }
            for (int i = 1; i < outPut.Count - 1; i++)
            {
                if (HelperMethods.PointOnSegment(outPut[i], outPut[i - 1], outPut[outPut.Count - 1]))
                    outPut.RemoveAt(i);
            }

            return outPut;
        }
        private List<Point> SortPoints(List<Point> points)
        {
            // Use the centroid of the hull points to sort them in counterclockwise order
            Point center = new Point(points.Average(p => p.X), points.Average(p => p.Y));
            return points.OrderBy(p => Math.Atan2(p.Y - center.Y, p.X - center.X)).ToList();
        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count <= 3)
            {
                outPoints = points;
            }
            else
            {
                points = points.OrderBy(xa => xa.X).ToList();
                outPoints = divide(points);
                outPoints = SortPoints(outPoints);
                for (int i = 0; i < outPoints.Count; i++)
                {
                    Point start = outPoints[i];
                    Point end = outPoints[(i + 1) % outPoints.Count]; 
                    outLines.Add(new Line(start, end));
                }
            }
        }

        public override string ToString()
        {
            return "Convex Hull - Divide & Conquer";
        }

    }
}
