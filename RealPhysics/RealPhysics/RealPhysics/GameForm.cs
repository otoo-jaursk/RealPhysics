using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;

namespace RealPhysics
{
    public partial class GameForm : Form
    {
        private Universe uni;
        private Camera camera;

        
        public GameForm()
        {
            Platform plat = new Platform(0, 3, 100, 3, .8, .4, "floor");
            Platform plat2 = new Platform(15, 15, 5, 2, .8, .6, "other");
            RectangleF game = new RectangleF(15, 5, 4, 2);
            Vector playerVelocity = new Vector(0, 0, VectorType.VELOCITY, "velocity");
            Vector playerAcceleration = new Vector(0, 0, VectorType.ACCELERATION, "acceleration");
            GameObject obj = new GameObject(playerVelocity, playerAcceleration, 5, false, game, "player");
            RectangleF game2 = new RectangleF(30, 7, 2, 4);
            Vector otherVelocity = new Vector(0, 0, VectorType.VELOCITY, "velocity");
            Vector otherAcceleration = new Vector(0, 0, VectorType.ACCELERATION, "acceleration");
            float metersHeight = (float)(SystemParameters.PrimaryScreenHeight * 100) / (float)SystemParameters.PrimaryScreenWidth;
            uni = new Universe(16, 9.8, obj, 100, metersHeight);
            camera = new Camera(100, uni);
            GameObject obj2 = new GameObject(otherVelocity, otherAcceleration, 10, true, game2, "second_obj");
            uni.add(obj);
            //uni.add(obj2);
            uni.add(plat);
            uni.add(plat2);
            InitializeComponent();
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            uni.doWork();
            repaint();
        }
        
        private void repaint()
        {
            Graphics g = this.CreateGraphics();
            g.Clear(Color.White);
            List<Rectangle> rects = camera.snapshot(null);
            foreach(Rectangle rect in rects)
            {
                g.DrawRectangle(new Pen(Color.Black), rect);
            }
        
        }
    }
}
