using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace RealPhysics
{
    
   public class Platform : InPlay
        {
            private RectangleF hitbox;
            private double staticFriction;
            private double kineticFriction;
            private string name;
            private int inPlatCounter = 0;

            public Platform(float x, float y, float w, float h, double mewS, double mewK, string llamo)
            {
                name = llamo;
                hitbox = new RectangleF(x, y, w, h);
                staticFriction = mewS;
                kineticFriction = mewK;
            }

            public string getName()
            {
                return name;
            }

            public double getKineticFriction()
            {
                return kineticFriction;
            }

            public string toString()
            {
                return name;
            }

            //hopefully this will not be necessary
            public RectangleF gravityZone(float height, float width)
            {
                return new RectangleF(hitbox.X - width/2, hitbox.Y + height, hitbox.Width + width, height + hitbox.Height);
            }

            public RectangleF underside(float height, float width)
            {
                return new RectangleF(hitbox.X- width, hitbox.Y, hitbox.Width + width*2, height + hitbox.Height); 
            }

            public RectangleF getRekt()
            {
                return hitbox;
            }

            public void collision(InPlay other)
            {
                GameObject obj = (GameObject)other;
                string nome = "player";
                if (obj.getName().GetHashCode() == nome.GetHashCode())
                {
                    inPlatCounter++;
                    //Console.WriteLine("Collision between " + name + " and " + obj.getName() + " " + inPlatCounter);
                }
                RectangleF rect = obj.getRekt();
                RectangleF overside = gravityZone(rect.Height, rect.Width);
                RectangleF under_side = underside(rect.Height, rect.Width);
                bool inPlat = AdditionalMath.intersects(overside, rect); /*&& platform.Contains(rect)*/;
                bool underPlat = AdditionalMath.intersects(under_side, rect);
                if (underPlat)
                {
                    obj.removeForce("speed up");
                    RectangleF newRect = new RectangleF(rect.X, under_side.Y - under_side.Height, rect.Width, rect.Height);
                    obj.setRekt(newRect);
                }
                if (inPlat)
                {
                    //Console.WriteLine("Actually in Plat");
                    obj.InPlat = true;
                    rect = new RectangleF(rect.X, hitbox.Y + rect.Height, rect.Width, rect.Height);
                    obj.setRekt(rect);
                    obj.removeForce("gravity");
                    Vector v = obj.getVelocity();
                    double[] comps = v.getComponent();
                    comps[1] = 0;
                    v.setComponent(comps);
                    if (Universe.keysDown[0] && obj.getName().Equals("player"))
                    {
                        obj.addForce(new Vector(1000, Math.PI / 2, VectorType.FORCE, "jump up"));
                    }
                    //Console.WriteLine(obj.getVelocityMagnitude() + "-speed");
                    if (Math.Abs(obj.getVelocityMagnitude()) > .3)
                    {
                        //Console.WriteLine(obj.getName() + " friction direction " + (obj.getVelocityDirection() + Math.PI));
                        obj.addForce(new Vector(kineticFriction * Universe.g * obj.getMass(), obj.getVelocityDirection() + Math.PI, VectorType.FRICTION, "friction with " + name));
                    }
                    else
                    {
                        obj.removeForce("friction with " + name);
                    }
                }
            }

            public void determineMovement(int fps, double g)
            {
            }
        }
    
}
