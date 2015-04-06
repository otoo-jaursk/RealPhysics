using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace RealPhysics
{
    public class Universe
    {
        private List<Platform> platforms = new List<Platform>();
        private List<GameObject> objects = new List<GameObject>();
        private int fps;
        public static double g;
        private GameObject player;
        public static bool[] keysDown = new bool[4];
        private QuadTree tree;
        private int tickNumber = 0;

        public Universe(int framesPerSecond, double gravity, GameObject plaay, float xMeters, float yMeters)
        {
            tree = new QuadTree(new RectangleF(0, 0, xMeters, yMeters), 5);
            fps = framesPerSecond;
            g = gravity;
            player = plaay;
        }

        public void setPlayerMovement(bool[] newPlayerMove)
        {
            keysDown = newPlayerMove;
        }

        public GameObject getPlayer()
        {
            return player;
        }

        public void addPlatform(Platform toAdd)
        {
            platforms.Add(toAdd);
        }

        public void addGameObject(GameObject toAdd)
        {
            objects.Add(toAdd);
        }

        public void add(InPlay toAdd)
        {
            tree.add(toAdd);
        }

        public List<GameObject> getObjects()
        {
            return objects;
        }

        public List<Platform> getPlatforms()
        {
            return platforms;
        }

        public bool operatePlats(GameObject obj)
        {
            bool inAnyPlat = false;
            foreach(Platform plat in platforms){
                RectangleF rect = obj.getRekt();
                RectangleF platform = plat.getRekt();
                RectangleF gravityZone = plat.gravityZone(rect.Height, rect.Width);
                RectangleF underside = plat.underside(rect.Height, rect.Width);
                bool inPlat = AdditionalMath.contains(gravityZone, rect); /*&& platform.Contains(rect)*/;
                bool underPlat = AdditionalMath.contains(underside, rect);
                if (underPlat)
                {
                    obj.removeForce("speed up");
                    RectangleF newRect = new RectangleF(rect.X, underside.Y - underside.Height, rect.Width, rect.Height);
                    obj.setRekt(newRect);
                }
                if (inPlat)
                {   
                    inAnyPlat = true;
                    rect = new RectangleF(rect.X, platform.Y + rect.Height, rect.Width, rect.Height);
                    obj.setRekt(rect);
                    obj.removeForce("gravity");
                    Vector v = obj.getVelocity();
                    double[] comps = v.getComponent();
                    comps[1] = 0;
                    v.setComponent(comps);
                    if (keysDown[0] && obj.getName().Equals("player"))
                    {
                        obj.addForce(new Vector(1000, Math.PI / 2, VectorType.FORCE, "jump up"));
                    }   
                    if (Math.Abs(obj.getVelocityMagnitude()) > .3)
                    {
                        //Console.WriteLine(obj.getName() + " friction direction " + (obj.getVelocityDirection() + Math.PI));
                        obj.addForce(new Vector(plat.getKineticFriction() * g * obj.getMass(), obj.getVelocityDirection() + Math.PI, VectorType.FRICTION, "friction with " + plat.getName()));
                    }
                    else
                    {
                        obj.removeForce("friction with " + plat.getName());
                    }
                }
            }
            return inAnyPlat;
        }

        public void quadCollisions(QuadNode node)
        {
            node.collisions();
        }

        public void quadMovement(InPlay obj)
        {
            obj.determineMovement(fps, g);
        }

        public void doWork()
        {
            NodeDelegate collisions = quadCollisions;
            ObjectDelegate movement = quadMovement;
            tree.traverse(collisions, false);
            tree.traverse(movement, true);
        }

        public QuadTree getTree()
        {
            return tree;
        }

        public void operateCollisions(GameObject obj, int x)
        {
            for (int y = x + 1; y < objects.Count; y++)
            {
                GameObject obj2 = objects.ElementAt(y);
                RectangleF rekt1 = obj.getRekt();
                RectangleF rekt2 = obj2.getRekt();
                if (AdditionalMath.intersects(rekt1, rekt2))
                {
                    obj.collision(obj2);
                }
            }
            
        }

        public void operate()
        {
            if (keysDown[1])
            {
                player.addForce(new Vector(25, 0, VectorType.FORCE, "move right"));
            }
            else
            {
                player.removeForce("move right");
            }
            if (keysDown[3])
            {
                player.addForce(new Vector(25, Math.PI, VectorType.FORCE, "move left"));
            }
            else
            {
                player.removeForce("move left");
            }
            bool inAnyPlat = operatePlats(player);
            if (!inAnyPlat)
            {
                player.removeForce("jump up");
                player.removeAllForces(VectorType.FRICTION);
                Vector gravity = new Vector(g * player.getMass(), 3 * Math.PI / 2, VectorType.GRAVITY, "gravity");
                player.addForce(gravity);
            }
            operateCollisions(player, -1);
            player.determineMovement(fps, g);
            //keysDown = new bool[4];
            for(int x = 0; x < objects.Count; x++)
            {
                GameObject obj = objects.ElementAt(x);
                bool inPlatform = operatePlats(obj);
                if (!inPlatform)
                {
                    Vector gravity = new Vector(g * obj.getMass(), 3 * Math.PI / 2, VectorType.GRAVITY, "gravity");
                    obj.addForce(gravity);
                }
                operateCollisions(obj, x);
                obj.determineMovement(fps, g);
            }

        }

    }
}
