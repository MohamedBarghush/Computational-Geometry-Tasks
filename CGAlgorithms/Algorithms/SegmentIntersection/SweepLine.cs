using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using CGUtilities.DataStructures;
using CGUtilities;
using System.IO;

namespace CGAlgorithms.Algorithms.SegmentIntersection
{

    public class Equation
    {
        // Y = m*x -m*x1+y1
        // m = slope
        // C = -m*x1+y1
        // Y = mx + C

        private double C = 0;
        private double M = 0;
        private int idx = -1;
        public double Y = 0;
        public bool set_eq(Point a,Point b)
        {
            if (Math.Abs(a.X - b.X) < 0.000001) return false;
            this.M = (a.Y - b.Y) / (a.X - b.X);
            this.C = -M * a.X + a.Y;
            return true;
        }

        public void set_idx(int id)
        {
            this.idx = id;
        }

        public int get_idx()
        {
            return this.idx;
        }
        public void calc(double x)
        {
            this.Y = this.M * x + this.C;
        }

    };

    public class Events
    {
        public Point p;
        public int flag ; //0,1,2:start,intersection,end
        public int idx;

        public Events(Point pp, int flg,int id)
        {
            this.p = pp;
            this.flag = flg;
            this.idx = id;
        }
    };
    public class Handler
    {
        public bool fix_order(Point a, Point b)
        {
            return !(a.X < b.X || ((a.X == b.X) && (a.Y < b.Y)));
        }
        public bool point_cmp(Point a,Point b)
        {
            return (a.X == b.X && a.Y == b.Y);
        }
        public bool line_cmp(Line a,Line b)
        {
            return (point_cmp(a.Start,b.Start) && point_cmp(a.End, b.End)) ||
                       (point_cmp(a.Start, b.End) && point_cmp(a.End, b.Start));
            
        }
        public int ComparePoints(Point p1, Point p2)
        {
            if (p1.X < p2.X) return -1;
            if (p1.X > p2.X) return 1;

            if (p1.Y < p2.Y) return -1;
            if (p1.Y > p2.Y) return 1;

            return 0; 
        }
        public void remove_occurance(ref List<Line> lines)
        {
            lines.Sort((line1, line2) =>
            {
                int startComparison = ComparePoints(line1.Start, line2.Start);
                if (startComparison != 0)
                {
                    return startComparison;
                }

                return ComparePoints(line1.End, line2.End);
            });

            List<Line> nw = new List<Line>();
            if (lines.Count == 0)
            {
                return;
            }
            nw.Add(lines[0]);
            foreach(var line in lines)
            {
                if (line_cmp(nw[nw.Count - 1], line)) continue;
                nw.Add(line);
            }

            lines = new List<Line>(nw);
        }

        public int get_prev(ref List<Equation> Y,double y)
        {
            //Y.Sort();
            int ret = -1;

            int l = 0;
            int r = Y.Count - 1;

            while (l <= r)
            {
                int md = (l + r) / 2;
                if (Y[md].Y > y) r = md - 1;
                else
                {
                    //Console.WriteLine("in prev: \n" + Y[md].Y + "  " + y + "\n");

                    l = md + 1;
                    ret = md;
                }
            }

            return ret;
        }

        public int get_next(ref List<Equation> Y, double y)
        {
            //Y.Sort();

            int ret = -1;

            int l = 0;
            int r = Y.Count - 1;

            while (l <= r)
            {
                int md = (l + r) / 2;

                if (Y[md].Y < y) l = md + 1;
                else
                {
                    r = md - 1 ;
                    ret = md;
                }
            }

            return ret;
        }

        public bool remove_eq(ref List<Equation>list,int idx)
        {
            
            for(int i = 0; i < list.Count; i++)
            {
                if (idx == list[i].get_idx())
                {
                    list.RemoveAt(i);
                    return true;
                }
            }


            return false;
        }

        public void sort_event(ref List<Events> eventsList)
        {
            eventsList.Sort((e1, e2) =>
            {
                // Sort by X-coordinate
                int compareX = e1.p.X.CompareTo(e2.p.X);
                if (compareX != 0)
                    return compareX;

                // Sort by Y-coordinate
                int compareY = e1.p.Y.CompareTo(e2.p.Y);
                if (compareY != 0)
                    return compareY;

                // Sort by flag
                return e1.flag.CompareTo(e2.flag);
            });
        }

        public void update_eq(ref List<Equation> lst,double X)
        {
            for(int i= 0; i < lst.Count; i++)
            {
                lst[i].calc(X);
            }
        }
        public void sort_equation(ref List<Equation> lst)
        {
            lst.Sort((a,b)=>
            {
                return a.Y.CompareTo(b.Y);
            });
        }
        public bool IsPointOnLineSegment(Point start, Point end, Point point)
        {
            return point.X >= Math.Min(start.X, end.X) && point.X <= Math.Max(start.X, end.X) &&
                   point.Y >= Math.Min(start.Y, end.Y) && point.Y <= Math.Max(start.Y, end.Y);
        }
        public bool CheckIntersection(Line line1, Line line2, out Point intersectionPoint)
        {
            // Extract points
            Point p1 = line1.Start, p2 = line1.End;
            Point q1 = line2.Start, q2 = line2.End;

            // Line 1 (p1, p2): a1*x + b1*y = c1
            double a1 = p2.Y - p1.Y;
            double b1 = p1.X - p2.X;
            double c1 = a1 * p1.X + b1 * p1.Y;

            // Line 2 (q1, q2): a2*x + b2*y = c2
            double a2 = q2.Y - q1.Y;
            double b2 = q1.X - q2.X;
            double c2 = a2 * q1.X + b2 * q1.Y;

            // Determinant
            double determinant = a1 * b2 - a2 * b1;

            // If determinant is zero, the lines are parallel or collinear
            if (Math.Abs(determinant) < 1e-10)
            {
                intersectionPoint = null;
                return false;
            }

            // Solve for intersection point
            double x = (b2 * c1 - b1 * c2) / determinant;
            double y = (a1 * c2 - a2 * c1) / determinant;
            intersectionPoint = new Point(x, y);

            // Check if the intersection point lies on both line segments
            if (IsPointOnLineSegment(p1, p2, intersectionPoint) &&
                IsPointOnLineSegment(q1, q2, intersectionPoint))
            {
                return true;
            }

            intersectionPoint = null;
            return false;
        }
    };
    class SweepLine:Algorithm
    {
        /*
        validate lines before start.
        store points in sorted set according to the x then y.
        keep track with list of active y

        for each point check the follow:
        - start: insert it, check with upper and lower segment intersection.
          add resulting intersection point to the list
        - End: remove it, and check upper with lower intersects and add new point if needed
        - Intersection: flip lines on that point and check with upper over lines, and lower

        
         */ 


        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {

            // LOGIC:
            /*
            handle reversed segments in the input
            keep track of comming points in a sorted data structure, sort low X then low Y
            keep track of the previuos not closed segmements equation, no need to sort them.

            for the current X, sort the segments in Y intersecting with sweep line

            
            - when start:
            add its process intersecction of it, and up, lower segment
            add its equation

            - when end: 
            add its process intersecction of it, and up, lower segment
            remove its equation

            - when intersection:
            add it to the answer
            check up up and low low

            --> may store the equation with index it processed in input , to easily delete it

             */

            Handler handler = new Handler();
            for(int i = 0; i < lines.Count; i++)
            {
                if (handler.fix_order(lines[i].Start, lines[i].End))
                {
                    
                    lines[i] = new Line(lines[i].End, lines[i].Start);
                }
            }
            
            handler.remove_occurance(ref lines);
            List<Events> events = new List<Events>();
            for(int i = 0; i < lines.Count; i++)
            {
                events.Add(new Events(lines[i].Start, 0, i));
                events.Add(new Events(lines[i].End, 2, i));
            }

            handler.sort_event(ref events);

            List<Equation> sweep = new List<Equation>();

            List<double> vis = new List<double>();

            while(events.Count>0)
            {
                Events ev = new Events(events[0].p, events[0].flag, events[0].idx);
                Console.WriteLine(ev.p.X + " " + ev.p.Y + " "+ev.flag);
                events.RemoveAt(0);
                //Console.WriteLine("H");
                double cur_x = ev.p.X;

                handler.update_eq(ref sweep, cur_x + 0.1);

                //Console.WriteLine("F");
                handler.sort_equation(ref sweep);

                if (ev.flag == 0) // start point
                {
                    int prv = handler.get_prev(ref sweep, ev.p.Y);
                    int nxt = handler.get_next(ref sweep, ev.p.Y);

                    if (prv != -1)
                    {
                        Point intersect_p = new Point(0, 0);
                        if(handler.CheckIntersection(lines[ev.idx],lines[sweep[prv].get_idx()], out intersect_p))
                        {
                            if(!vis.Contains(intersect_p.Y))
                            {

                                //Console.WriteLine(intersect_p.Y + " " + ev.p.Y);
                                events.Add(new Events(intersect_p, 1, -1));
                                vis.Add(intersect_p.Y);
                            }
                            
                        }
                    }
                    if (nxt != -1)
                    {
                        Point intersect_p = new Point(0, 0);
                        if (handler.CheckIntersection(lines[ev.idx], lines[sweep[nxt].get_idx()], out intersect_p))
                        {
                            if (!vis.Contains(intersect_p.Y))
                            {
                                //Console.WriteLine(intersect_p.Y + " " + ev.p.Y);
                                events.Add(new Events(intersect_p, 1, -1));
                                vis.Add(intersect_p.Y);

                            }
                        }
                    }
                    Equation tmp = new Equation();
                    tmp.set_eq(lines[ev.idx].Start, lines[ev.idx].End);
                    tmp.set_idx(ev.idx);
                    sweep.Add(tmp);
                }
                else if (ev.flag == 1)// intersection
                {
                    outPoints.Add(new Point (ev.p.X,ev.p.Y));
                    int prv = handler.get_prev(ref sweep, ev.p.Y);
                    int nxt = handler.get_next(ref sweep, ev.p.Y);
                    Console.WriteLine(prv + " - " + nxt);
                    for (int i = 0; i < sweep.Count; i++)
                    {
                        Console.WriteLine(sweep[i].Y + " -- " + sweep[i].get_idx());
                    }
                    if (prv != -1)
                    {
                        if (prv > 0)
                        {
                            Point intersect_p = new Point(0, 0);
                            if (handler.CheckIntersection(lines[sweep[prv].get_idx()], lines[sweep[prv - 1].get_idx()], out intersect_p))
                            {
                                if (!vis.Contains(intersect_p.Y))
                                {
                                    events.Add(new Events(intersect_p, 1, -1));
                                    vis.Add(intersect_p.Y);

                                }
                            }

                            
                        }
                        if (prv-1 > 0)
                        {
                            Point intersect_p = new Point(0, 0);
                            if (handler.CheckIntersection(lines[sweep[prv-1].get_idx()], lines[sweep[prv - 2].get_idx()], out intersect_p))
                            {
                                if (!vis.Contains(intersect_p.Y))
                                {
                                    events.Add(new Events(intersect_p, 1, -1));
                                    vis.Add(intersect_p.Y);

                                }
                            }


                        }
                    }

                    if (nxt != -1)
                    {
                        if (nxt + 1 < sweep.Count)
                        {
                            Point intersect_p = new Point(0, 0);
                            if (handler.CheckIntersection(lines[sweep[nxt].get_idx()], lines[sweep[nxt + 1].get_idx()], out intersect_p))
                            {
                                if (!vis.Contains(intersect_p.Y))
                                {
                                    events.Add(new Events(intersect_p, 1, -1));
                                    vis.Add(intersect_p.Y);

                                }
                            }
                        }
                        if (nxt + 2 < sweep.Count)
                        {
                            Point intersect_p = new Point(0, 0);
                            if (handler.CheckIntersection(lines[sweep[nxt+1].get_idx()], lines[sweep[nxt + 2].get_idx()], out intersect_p))
                            {
                                if (!vis.Contains(intersect_p.Y))
                                {
                                    events.Add(new Events(intersect_p, 1, -1));
                                    vis.Add(intersect_p.Y);

                                }
                            }
                        }
                    }

                }
                else if (ev.flag == 2)// end point
                {
                    if (!handler.remove_eq(ref sweep, ev.idx))
                    {
                        Console.WriteLine("cannt remove omg!!");
                    }

                    int prv = handler.get_prev(ref sweep, ev.p.Y);
                    int nxt = handler.get_next(ref sweep, ev.p.Y);
                    
                    if (prv != -1 && nxt != -1)
                    {
                        Point intersect_p = new Point(0, 0);
                        if (handler.CheckIntersection(lines[sweep[prv].get_idx()], lines[sweep[nxt].get_idx()], out intersect_p))
                        {
                            events.Add(new Events(intersect_p, 1, -1));

                        }

                        //if (prv > 1)
                        //{
                        //    intersect_p = new Point(0, 0);
                        //    if (handler.CheckIntersection(lines[sweep[prv-2].get_idx()], lines[sweep[prv-1].get_idx()], out intersect_p))
                        //    {
                        //        events.Add(new Events(intersect_p, 1, -1));

                        //    }

                        //}

                        //if (nxt + 1 < sweep.Count)
                        //{
                        //    intersect_p = new Point(0, 0);
                        //    if (handler.CheckIntersection(lines[sweep[nxt + 1].get_idx()], lines[sweep[nxt].get_idx()], out intersect_p))
                        //    {
                        //        events.Add(new Events(intersect_p, 1, -1));

                        //    }
                        //}
                    }

                    
                }
                else
                {
                    Console.WriteLine("doz: undifined flag!!");
                }

                handler.sort_event(ref events);
            }


            HelperMethods.RemoveDuplicatePoints(ref outPoints);

        }

        public override string ToString()
        {
            return "Sweep Line";
        }
    }
}
