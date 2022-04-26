using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

enum Flag { Red, Blue, notSpecified }

namespace AssaultCubeClient.Model
{
    class Player
    {
        private int _health;
  
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Magnitude { get; set; }
        public bool IsFriendly { get; set; }
        public Point Top { get; set; }
        public Point Bottom { get; set; }
        public Flag Team { get; set; }
        public Rectangle Rectangle { get; set; };


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

        public Player(int health, double x, double y, double z, double magnitude, bool isFriendly, Point top, Point bottom) 
        {
            this.Health = health;
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Magnitude = magnitude;
            this.IsFriendly = isFriendly;
            this.Top = top;
            this.Bottom = bottom;
        }

        public Rectangle CreateRectangle()
        {
            return new Rectangle()
            {
                Location = new Point(Bottom.X - (Bottom.Y - Top.Y) / 4, Top.Y),
                Size = new Size((Bottom.Y - Top.Y) / 2, Bottom.X - Top.X)
            };
        }

        public override string ToString()
        {
            return this.X + " " + this.Y + " " + this.Z;
        }
    }
}
