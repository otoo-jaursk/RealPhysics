using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace RealPhysics
{
    public class QuadNode
    {
        private double minimum;
        private RectangleF Area;
        private QuadNode parent;
        private QuadNode[,] children = new QuadNode[2,2];
        private RectangleF[,] innerQuads = new RectangleF[2,2];
        private List<InPlay> dict = new List<InPlay>();
        private int quadLevel;

        public QuadNode(QuadNode daddy, RectangleF area, double min, int nome)
        {
            quadLevel = nome;
            minimum = min;
            parent = daddy;          
            innerQuads[0,0] = (new RectangleF(0, area.Height, area.Width / 2, area.Height / 2));
            innerQuads[0, 1] = (new RectangleF(area.Width / 2, area.Height, area.Width / 2, area.Height / 2));
            innerQuads[1, 0] = (new RectangleF(0, area.Height / 2, area.Width / 2, area.Height / 2));
            innerQuads[1, 1] = (new RectangleF(area.Width / 2, area.Height / 2, area.Width / 2, area.Height / 2));
          /*if((area.Height/2) * (area.Width / 2) < min)
            {
                for (int x = 0; x < 2; x++)
                {
                    for (int y = 0; y < 2; y++)
                    {
                        children[x, y] = new QuadNode(this, innerQuads[x, y], minimum);
                    }
                }
            }*/
            Area = area;
            
        }

        public void queueUpInPlays(List<InPlay> queue)
        {
            queue.AddRange(dict);
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    if (children[x, y] != null)
                    {
                        children[x, y].queueUpInPlays(queue);
                    }
                }
            }
        }

        public void queueUp(List<RectangleF> queue)
        {
            queue.Add(Area);
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    if (children[x, y] != null)
                    {
                        children[x, y].queueUp(queue);
                    }
                }
            }
        }

        public void collisions()
        {
            List<InPlay> allPossibleCollisions = new List<InPlay>();
            queueUpInPlays(allPossibleCollisions);
            for (int x = 0; x < dict.Count; x++)
            {
                for (int y = x + 1; y < allPossibleCollisions.Count; y++)
                {
                    InPlay obj1 = dict.ElementAt(x);
                    InPlay obj2 = allPossibleCollisions.ElementAt(y);
                    if (obj1 is GameObject && obj2 is GameObject)
                    {
                        if (AdditionalMath.intersects(obj1.getRekt(), obj2.getRekt()))
                        {
                            obj1.collision(obj2);
                        }
                    }
                    else
                    {
                        //Console.WriteLine("Comparison between {0} and {1}", obj1.toString(), obj2.toString());
                        if (obj1 is Platform)
                        {
                            Platform plat = (Platform)(obj1);
                            RectangleF rect = obj2.getRekt();
                            RectangleF overside = plat.gravityZone(rect.Height, rect.Width);
                            RectangleF underside = plat.gravityZone(rect.Height, rect.Width);
                            if(AdditionalMath.intersects(rect, overside))
                            {
                                plat.collision(obj2);
                            }
                        }
                        if (obj2 is Platform)
                        {
                            Platform plat = (Platform)(obj2);
                            RectangleF rect = obj1.getRekt();
                            RectangleF overside = plat.gravityZone(rect.Height, rect.Width);
                            RectangleF underside = plat.gravityZone(rect.Height, rect.Width);
                            if (AdditionalMath.intersects(rect, overside))
                            {
                                plat.collision(obj1);
                            }
                        }
                    }
                   
                }
            }
        }

        public QuadNode getNode(int x, int y)
        {
            return children[x, y];
        }

        public RectangleF getRectangle()
        {
            return Area;
        }

        public List<InPlay> getObjs()
        {
            return dict;
        }

        public void add(InPlay obj)
        {
            bool haveAdded = false;
            if(!AdditionalMath.contains(Area, obj.getRekt()) && parent != null)
            {
                parent.add(obj);
                return;
            }
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    RectangleF rect = obj.getRekt();
                    if(AdditionalMath.contains(innerQuads[x, y], rect))
                    {
                        haveAdded = true;
                        if (children[x, y] == null && Area.Height * Area.Width * .25 > minimum)
                        {
                            children[x, y] = new QuadNode(this, innerQuads[x, y], minimum, quadLevel + 1);
                        }

                        children[x, y].add(obj);
                    }
                }
            }
            if (!haveAdded)
            {
                dict.Add(obj);
            }
        }

        public void traverse(NodeDelegate del, List<QuadNode> queue, bool movementChange)
        {
            del(this);
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    if (children[x, y] != null)
                    {
                        queue.Add(children[x,y]);
                        children[x, y].traverse(del, queue, movementChange);
                    }
                }
            }
        }

        public void traverse(ObjectDelegate del, List<QuadNode> queue, bool movementChange)
        {
            for(int a = 0; a < dict.Count; a++)
            {
                InPlay obj = dict.ElementAt(a);
                del(obj);
                if(movementChange && !AdditionalMath.contains(Area, obj.getRekt()))
                {
                    if (parent != null)
                    {
                        dict.Remove(obj);
                        parent.add(obj);
                    }
                }
                for (int x = 0; x < 2; x++)
                {
                    for (int y = 0; y < 2; y++)
                    {
                        if (movementChange && AdditionalMath.contains(innerQuads[x, y], obj.getRekt()))
                        {
                            if(children[x, y] == null)
                            {
                                children[x, y] = new QuadNode(this, innerQuads[x, y], minimum, quadLevel + 1);
                            }
                            children[x, y].add(obj);
                            dict.Remove(obj);
                        }
                    }
                }

            }
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    if(children[x, y] != null)
                    {
                        queue.Add(children[x, y]);
                    }
                }
            }


        }
    }
}
