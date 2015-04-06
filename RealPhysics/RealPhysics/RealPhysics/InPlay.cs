using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace RealPhysics
{
    public interface InPlay
    {
        void collision(InPlay otherOne);

        void determineMovement(int fps, double g);

        RectangleF getRekt();

        string toString();
    }
}
