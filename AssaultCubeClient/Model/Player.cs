using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum Flag { Red, Blue, notSpecified }

namespace AssaultCubeClient.Model
{
    public class Player
    {
        private int _health;
  
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Magnitude { get; set; }
        public Point Top { get; set; }
        public Point Bottom { get; set; }
        public Flag Team { get; set; }
        float[] ViewMatrix = new float[16];


        public int Health
        {
            get { return this._health; }
            set
            {
                if (value >= 0)
                {
                    this._health = value;
                }
            }
        }
        
        public Player() : this(0, 0.0, 0.0, 0.0, 0.0) { }
        public Player(int health, double x, double y, double z, double magnitude)
        {
            this.Health = health;
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Magnitude = magnitude;
        }

        public Rectangle CreateRectangle()
        {
            Point p = new Point(Bottom.X - (Bottom.Y - Top.Y) / 4, Top.Y);
            Size s = new Size((Bottom.Y - Top.Y) / 2, Bottom.Y - Top.Y);
            return new Rectangle()
            {
                Location = p,
                Size = s
            };
        }

        public override string ToString()
        {
            return this.X + " " + this.Y + " " + this.Z;
        }
    }
}
