using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssaultCubeClient.Model
{
    class Player
    {
        private int _health; 
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Magnitude { get; set; }

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


        public override string ToString()
        {
            return this.X + " " + this.Y + " " + this.Z;
        }
    }
}
