using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealPhysics
{
    public class Vector
    {
        private double magnitude;
        private double direction;
        private double componentX;
        private double componentY;
        private VectorType vectorType;
        private string name;

        public Vector(double mag, double theta, VectorType type, string vectorName)
        {
            magnitude = mag;
            direction = theta;
            componentX = magnitude * Math.Cos(direction);
            componentY = magnitude * Math.Sin(direction);
            name = vectorName;
            vectorType = type;
        }

        public VectorType getVectorType()
        {
            return vectorType;
        }

        public string getName()
        {
            return name;
        }

        public void setName(string newName)
        {
            name = newName;
        }

        public double getMagnitude()
        {
            return magnitude;
        }

        public double getDirection()
        {
            return direction;
        }

        public double[] getComponent()
        {
            double[] array = {componentX, componentY};
            return array;
        }

        public void setComponent(double[] comp)
        {
            componentX = comp[0];
            componentY = comp[1];
            magnitude = Math.Sqrt(Math.Pow(componentY, 2) + Math.Pow(componentX, 2));
            direction = Math.Atan2(componentY, componentX);
        }

        public Vector resultantVector(Vector other)
        {
            double newCompX = componentX + other.componentX;
            double newCompY = componentY + other.componentY;
            return new Vector(Math.Sqrt(Math.Pow(newCompX, 2) + Math.Pow(newCompY, 2)), Math.Atan2(newCompY, newCompX), vectorType, "resultant of " + name + " " + other.name);
        }



    }
}
