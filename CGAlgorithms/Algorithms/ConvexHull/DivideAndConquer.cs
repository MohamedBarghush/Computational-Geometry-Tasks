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



        private List<Line> Merge(List<Point> leftHull, List<Point> rightHull)
        {
            int upperLeftIndex = 0;
            int upperRightIndex = 0;

            for (int i = 0; i < leftHull.Count; i++)
            {
                if (leftHull[i].Y > leftHull[upperLeftIndex].Y)
                {
                    upperLeftIndex = i;
                }
            }

            for (int i = 0; i < rightHull.Count; i++)
            {
                if (rightHull[i].Y > rightHull[upperRightIndex].Y)
                {
                    upperRightIndex = i;
                }
            }

            // to get the lower tangent
            // we need to find the min Y point in the right hull and the min Y point in the left hull

            int lowerLeftIndex = 0;
            int lowerRightIndex = 0;

            for (int i = 0; i < leftHull.Count; i++)
            {
                if (leftHull[i].Y < leftHull[lowerLeftIndex].Y)
                {
                    lowerLeftIndex = i;
                }
            }

            for (int i = 0; i < rightHull.Count; i++)
            {
                if (rightHull[i].Y < rightHull[lowerRightIndex].Y)
                {
                    lowerRightIndex = i;
                }
            }

            // Points on the upper tangent
            Point upperLeftPoint = leftHull[upperLeftIndex];
            Point upperRightPoint = rightHull[upperRightIndex];

            // // Points on the lower tangent
            Point lowerLeftPoint = leftHull[lowerLeftIndex];
            Point lowerRightPoint = rightHull[lowerRightIndex];

            List<Line> tangents = new List<Line>
            {
                new Line(upperLeftPoint, upperRightPoint),
                new Line(lowerLeftPoint, lowerRightPoint)
            };

            return tangents;
        }


        private List<Point> SortPoints(List<Point> points)
        {
            // Use the centroid of the hull points to sort them in counterclockwise order
            Point center = new Point(points.Average(p => p.X), points.Average(p => p.Y));
            return points.OrderBy(p => Math.Atan2(p.Y - center.Y, p.X - center.X)).ToList();
        }


        private List<Point> ComputeConvexHull(List<Point> points)
        {
            if (points.Count < 3)
                return points;

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

            // Divide the points into two subsets: above and below the line formed by leftmost and rightmost points
            List<Point> RightSide = new List<Point>();
            List<Point> LeftSide = new List<Point>();

            for (int i = points.Count / 2; i < points.Count; i++)
            {
                LeftSide.Add(points[i]);
            }

            for (int i = 0; i < points.Count / 2; i++)
            {
                RightSide.Add(points[i]);
            }

            LeftSide = ComputeConvexHull(LeftSide);
            RightSide = ComputeConvexHull(RightSide);

            int upperLeftIndex = 0;
            int upperRightIndex = 0;

            int lowerLeftIndex = 0;
            int lowerRightIndex = 0;


            for (int i = 0; i < LeftSide.Count; i++)
            {
                if (LeftSide[i].Y > LeftSide[upperLeftIndex].Y)
                {
                    upperLeftIndex = i;
                }
            }

            for (int i = 0; i < RightSide.Count; i++)
            {
                if (RightSide[i].Y > RightSide[upperRightIndex].Y)
                {
                    upperRightIndex = i;
                }
            }


            for (int i = 0; i < LeftSide.Count; i++)
            {
                if (LeftSide[i].Y < LeftSide[lowerLeftIndex].Y)
                {
                    lowerLeftIndex = i;
                }
            }

            for (int i = 0; i < RightSide.Count; i++)
            {
                if (RightSide[i].Y < RightSide[lowerRightIndex].Y)
                {
                    lowerRightIndex = i;
                }
            }

            Point upperLeftPoint = LeftSide[upperLeftIndex];
            Point upperRightPoint = RightSide[upperRightIndex];
            Point lowerLeftPoint = LeftSide[lowerLeftIndex];
            Point lowerRightPoint = RightSide[lowerRightIndex];
             //hello world 
             //hello world 
            Line right = new Line(upperRightPoint, lowerRightPoint);
            for (int i = 0; i < RightSide.Count; i++)
            {
                Enums.TurnType cr = HelperMethods.CheckTurn(right, RightSide[i]);
                if (cr == Enums.TurnType.Right || cr == Enums.TurnType.Colinear || RightSide[i].Y > upperLeftPoint.Y || RightSide[i].Y < lowerLeftPoint.Y)
                    if (!outPut.Contains(RightSide[i]))
                        outPut.Add(RightSide[i]);
            }

            Line left = new Line(upperLeftPoint, lowerLeftPoint);
            for (int i = 0; i < LeftSide.Count; i++)
            {
                Enums.TurnType cl = HelperMethods.CheckTurn(left, LeftSide[i]);
                if (cl == Enums.TurnType.Left || cl == Enums.TurnType.Colinear)
                    if (!outPut.Contains(LeftSide[i]))
                        outPut.Add(LeftSide[i]);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            for (int i = 0; i < outPut.Count; i++)
            {
                if (outPut[i].X < upperLeftPoint.X && outPut[i].X > lowerLeftPoint.X)
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

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////




            return outPut;
        }


        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            // RemoveDuplicatePoints(ref points);

            if (points.Count < 3)
            {
                outPoints = points;
            }
            else
            {
                points = points.OrderBy(xa => xa.X).ToList();


                // Compute the convex hull for the left subset
                List<Point> result = ComputeConvexHull(points);

                outPoints = SortPoints(result);



                for (int i = 0; i < outPoints.Count; i++)
                {
                    Point start = outPoints[i];
                    Point end = outPoints[(i + 1) % outPoints.Count]; // Wrap around to form a closed loop
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
