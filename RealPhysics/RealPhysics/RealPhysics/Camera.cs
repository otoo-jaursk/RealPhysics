using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;

namespace RealPhysics
{
    public class Camera
    {
        private double meters_to_pixels;
        private Universe universe;
        private double XAXIS = SystemParameters.PrimaryScreenWidth;
        private double YAXIS = SystemParameters.PrimaryScreenHeight;

        public Camera(double metersPerScreen, Universe world)
        {
            meters_to_pixels = metersPerScreen / SystemParameters.PrimaryScreenWidth;
            universe = world;
        }

        public List<Rectangle> snapshot(GameObject focus)
        {
            QuadTree tree = universe.getTree();
            List<InPlay> objects = new List<InPlay>();
            objects = tree.queueUp();
            //objects.AddRange(universe.getObjects());
            //objects.AddRange(universe.getPlatforms());
            List<Rectangle> returnable = new List<Rectangle>();
            List<RectangleF> list = tree.quadQueue();//new List<Rectangle>();
            /*
            RectangleF player = universe.getPlayer().getRekt();
            int height = (int)(player.Height / meters_to_pixels);
            int width = (int)(player.Width / meters_to_pixels);
            int x = (int)(player.X / meters_to_pixels);
            int y = (int)YAXIS - (int)(player.Y / meters_to_pixels);
            returnable.Add(new Rectangle(x, y, width, height));
            */
            foreach(InPlay obj in objects)
            {
                RectangleF rect = obj.getRekt();
                int height = (int)(rect.Height / meters_to_pixels);
                int width = (int)(rect.Width / meters_to_pixels);
                int x = (int)(rect.X / meters_to_pixels);
                int y = (int)YAXIS - (int)(rect.Y / meters_to_pixels);
                returnable.Add(new Rectangle(x, y, width, height));
            }
            foreach (RectangleF rect in list)//obj in objects)
            {
                //RectangleF rect = obj.getRekt();
                int height = (int)(rect.Height / meters_to_pixels);
                int width = (int)(rect.Width / meters_to_pixels);
                int x = (int)(rect.X / meters_to_pixels);
                int y = (int)YAXIS - (int)(rect.Y / meters_to_pixels);
                returnable.Add(new Rectangle(x, y, width, height));
            }
            return returnable;
        }
    }
}
