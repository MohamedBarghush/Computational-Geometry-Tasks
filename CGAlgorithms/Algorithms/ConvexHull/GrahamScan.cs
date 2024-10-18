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
            if (points.Count <4)
            {
                foreach(Point p in points)
                {
                    outPoints.Add(p);
                }
                return;
            }
            Point lower = new Point(0, 1e12);

            foreach( Point p in points)
            {
                if (p.Y <= lower.Y) lower = p;
            }

            points.Sort((a, b) => calcAngle(lower.X, lower.Y, a.X, a.Y).CompareTo(calcAngle(lower.X, lower.Y, b.X, b.Y)));

           
            string filePath = @"C:\Users\Bahnasy\Desktop\tmp.txt";

            // Create a StreamWriter instance to write to the file
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (Point p in points)
                {
                    writer.WriteLine(p.X+"  "+p.Y);
                }
            }

            
            List<Point> stk = new List<Point>();
            stk.Add(lower);
            stk.Add(points[1]);

            for(int i = 2; i < points.Count; )
            {
                Point p3 = points[i];
                Point p2 = stk[stk.Count - 1];
                Point p1 = stk[stk.Count - 2];

                if(HelperMethods.CheckTurn(new Line(p1,p2),p3)== Enums.TurnType.Left)
                {
                    stk.Add(p3);
                    i++;
                 
                }
                else
                {
                    stk.RemoveAt(stk.Count - 1);
                    
                }
            }

            foreach (Point p in stk) outPoints.Add(p);
            stk.Add(stk[0]);
            for (int i = 0; i < stk.Count-1; i++)
            {
                outLines.Add(new Line(stk[i], stk[i + 1]));
            }
            outPolygons.Add(new Polygon( outLines));
            return;
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
