using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealPhysics
{
    class VectorEqualityComparer : IEqualityComparer<Vector>
    {
        public bool Equals(Vector v1, Vector v2)
        {
            int a = v1.getName().GetHashCode();
            int b = v2.getName().GetHashCode();
            return a == b;
        }

        public int GetHashCode(Vector v)
        {
            string a = v.getName();
            return a.GetHashCode();
        }
    }
}
