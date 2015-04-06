using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
namespace RealPhysics
{
    public class GameObject : InPlay
    {
        private Vector velocity;
        private Vector acceleration;
        private RectangleF rekt;
        private double mass;
        private bool elastic;
        private bool flight = false;
        private double startingHeight = -1;
        private HashSet<Vector> hashedForces;
        private bool inAPlatform = false;
        public int quadLevel = 4;
        public bool InPlat
        {
            get { return inAPlatform; }
            set { inAPlatform = value;}
        }
        private bool[] keysDown = new bool[4];
        public List<Vector> forces = new List<Vector>();
        string name;

        public GameObject(Vector v, Vector xcelration, double catholicChurchService, bool elasticity, RectangleF hitbox, string llamo)
        {
            VectorEqualityComparer vectorComparer = new VectorEqualityComparer();
            hashedForces = new HashSet<Vector>(vectorComparer);
            name = llamo;
            rekt = hitbox;
            velocity = v;
            acceleration = xcelration;
            mass = catholicChurchService;
            elastic = elasticity;
        }

        public double getMass()
        {
            return mass;
        }

        public string getName()
        {
            return name;
        }

        public void addForce(Vector addedForce)
        {
            hashedForces.Remove(addedForce);
            hashedForces.Add(addedForce);
        }

        public void HandleKeyDownEvent(object sender, KeyEventArgs e)
        {
            string test = "player";
            if(name.GetHashCode() == test.GetHashCode())
            {
                if (e.KeyCode == Keys.D)
                {
                    keysDown[1] = true;
                }
                if (e.KeyCode == Keys.A)
                {
                    keysDown[3] = true;
                }
            }
        }

        public void HandleKeyUpEvent(object sender, KeyEventArgs e)
        {
            keysDown[0] = false;
            keysDown[1] = false;
            keysDown[2] = false;
            keysDown[3] = false;
        }

        public void removeForce(string name)
        {
            hashedForces.Remove(new Vector(0, 0, VectorType.FORCE, name));
        }

        public void removeAllForces(VectorType type)
        {
            hashedForces.RemoveWhere(x => x.getVectorType() == type);
        }

        public void accelerate(int fps)
        {
            double mag = acceleration.getMagnitude() / fps;
            Vector temp = new Vector(mag, acceleration.getDirection(), VectorType.ACCELERATION, "acceleration");
            velocity = velocity.resultantVector(temp);
        }

        public void addAcceleration(Vector xceleration)
        {
            acceleration = acceleration.resultantVector(xceleration);
        }

        public void ground()
        {
            flight = false;
            startingHeight = -1;
        }

        private void determineAcceleration()
        {
            Vector fnet = new Vector(0, 0, VectorType.FORCE, "resultantForce");
            foreach (Vector v in hashedForces)
            {
                fnet = fnet.resultantVector(v);
            }
            acceleration = new Vector(fnet.getMagnitude() / mass, fnet.getDirection(), VectorType.ACCELERATION, "acceleration");
        }

        public void collision(InPlay otherOne)
        {
            if (otherOne is GameObject)
            {
                GameObject other = (GameObject)otherOne;
                double[] components = velocity.getComponent();
                double[] theirComponents = other.velocity.getComponent();
                double momentumX = components[0] * mass + theirComponents[0] * other.mass;
                double momentumY = components[1] * mass + theirComponents[1] * other.mass;
                double ourXVelocity, ourYVelocity, theirXVelocity, theirYVelocity;
                if (other.elastic || elastic)
                {
                    //kinetic energy when only taking into account velocity in the x direction
                    double xJoules = .5 * Math.Pow(components[0], 2) * mass + .5 * Math.Pow(theirComponents[0], 2) * other.mass;
                    //formula I figured out
                    double a = .5 * (other.mass + other.mass * other.mass / mass);
                    double b = -momentumX * other.mass / mass;
                    double c = .5 * momentumX * momentumX / mass - xJoules;
                    double[] theirPossibleVxs = AdditionalMath.quadraticFormula(a, b, c);
                    if (Math.Round(theirPossibleVxs[0], 5) == Math.Round(theirComponents[0], 5))
                    {
                        theirXVelocity = theirPossibleVxs[1];
                    }
                    else
                    {
                        theirXVelocity = theirPossibleVxs[0];
                        if (Double.IsNaN(theirXVelocity))
                        {
                            theirXVelocity = 0;
                        }
                    }
                    double yJoules = .5 * Math.Pow(components[1], 2) * mass + .5 * Math.Pow(theirComponents[1], 2) * other.mass;
                    double aa = .5 * (other.mass + other.mass * other.mass / mass);
                    double bb = -momentumY * other.mass / mass;
                    double cc = .5 * momentumY * momentumY / mass - yJoules;
                    //formula I figured out don't question it
                    double[] theirPossibleVys = AdditionalMath.quadraticFormula(aa, bb, cc);
                    if (Math.Round(theirPossibleVys[0], 5) == Math.Round(theirComponents[1], 5))
                    {
                        theirYVelocity = theirPossibleVys[1];
                    }
                    else
                    {
                        theirYVelocity = theirPossibleVys[0];
                        if (Double.IsNaN(theirYVelocity))
                        {
                            theirYVelocity = 0;
                        }
                    }
                    ourYVelocity = (momentumY - theirYVelocity * other.mass) / mass;
                    ourXVelocity = (momentumX - theirXVelocity * other.mass) / mass;
                }
                else
                {
                    ourXVelocity = theirXVelocity = momentumX / (mass + other.mass);
                    ourYVelocity = theirYVelocity = momentumY / (mass + other.mass);
                }
                double velocityMag = Math.Sqrt(Math.Pow(ourXVelocity, 2) + Math.Pow(ourYVelocity, 2));
                double otherVelocityMag = Math.Sqrt(Math.Pow(theirXVelocity, 2) + Math.Pow(theirYVelocity, 2));
                double otherDirection = Math.Atan2(theirYVelocity, theirXVelocity);
                double direction = Math.Atan2(ourYVelocity, ourXVelocity);
                velocity = new Vector(velocityMag, direction, VectorType.VELOCITY, "velocity");
                other.velocity = new Vector(otherVelocityMag, otherDirection, VectorType.VELOCITY, "velocity");
                if (momentumX > 0)
                {
                    if (rekt.X < other.rekt.X)
                    {
                        float change = ((rekt.X + rekt.Width) - other.rekt.X) / 2;
                        rekt = new RectangleF(rekt.X - change, rekt.Y, rekt.Width, rekt.Height);
                        other.rekt = new RectangleF(other.rekt.X + change, other.rekt.Y, other.rekt.Width, other.rekt.Height);
                    }
                    if (rekt.X > other.rekt.X)
                    {
                        float change = ((other.rekt.X + other.rekt.Width) - rekt.X) / 2;
                        other.rekt = new RectangleF(other.rekt.X - change, other.rekt.Y, other.rekt.Width, other.rekt.Height);
                        rekt = new RectangleF(rekt.X + change, rekt.Y, rekt.Width, rekt.Height);
                    }
                }
                if (momentumY > 0)
                {
                    if (rekt.Y < other.rekt.Y)
                    {
                        float change = (rekt.Y - (other.rekt.Y - other.rekt.Height)) / 2;
                        other.rekt = new RectangleF(other.rekt.X, other.rekt.Y + change, other.rekt.Width, other.rekt.Height);
                        rekt = new RectangleF(rekt.X + change, rekt.Y - change, rekt.Width, rekt.Height);
                    }
                    if (rekt.Y > other.rekt.Y)
                    {
                        float change = (other.rekt.Y - (rekt.Y - rekt.Height)) / 2;
                        other.rekt = new RectangleF(other.rekt.X, other.rekt.Y - change, other.rekt.Width, other.rekt.Height);
                        rekt = new RectangleF(rekt.X + change, rekt.Y + change, rekt.Width, rekt.Height);
                    }
                }
            }
        }

        public double getVelocityMagnitude()
        {
            return velocity.getMagnitude();
        }

        public void setVelocity(Vector v)
        {
            velocity = v;
        }

        public double getVelocityDirection()
        {
            return velocity.getDirection();
        }

        public Vector getVelocity()
        {
            return velocity;
        }

        public string toString()
        {
            return name;
        }

        public void determineMovement(int fps, double g)
        {
            if (keysDown[1])
            {
                this.addForce(new Vector(25, 0, VectorType.KEYBOARDFORCE, "force right"));
            }
            else
            {
                this.removeForce("force right");
            }
            if (keysDown[3])
            {
                this.addForce(new Vector(25, Math.PI, VectorType.KEYBOARDFORCE, "force left"));
            }
            else
            {
                this.removeForce("force left");
            }
            if (!inAPlatform)
            {
                removeForce("jump up");
                removeAllForces(VectorType.FRICTION);
                Vector gravity = new Vector(g * mass, 3 * Math.PI / 2, VectorType.GRAVITY, "gravity");
                addForce(gravity);
            }
            determineAcceleration();
            accelerate(fps);
            if (velocity.getMagnitude() < .3)
            {
                velocity = new Vector(0, 0, VectorType.VELOCITY, "velocity");
            }
            double[] comps = velocity.getComponent();
            float x = rekt.X;
            if (Math.Abs(comps[0]) > 0)
                x += (float)(comps[0] / fps);
            float y = rekt.Y;
            if (Math.Abs(comps[1]) > 0)
                y += (float)(comps[1] / fps);
            rekt = new RectangleF(x, y, rekt.Width, rekt.Height);
            //inAPlatform = false;
        }

        public RectangleF getRekt()
        {
            return rekt;
        }
        public void setRekt(RectangleF daRect)
        {
            rekt = daRect;
        }
        public bool getFlight()
        {
            return flight;
        }
        public void setFlight(bool toSet)
        {
            flight = toSet;
        }
        public double getStartingHeight()
        {
            return startingHeight;
        }
        public void setStartingHeight(double newStart)
        {
            startingHeight = newStart;
        }
    }
}