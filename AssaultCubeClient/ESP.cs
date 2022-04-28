using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Forms;
using AssaultCubeClient.Model;
using ezOverLay;

namespace AssaultCubeClient
{
    public partial class ESP : Form
    {
        ez ezO = new ez();
        private static ESP _instance;
        Graphics g;
        private List<Player> Players;
        private Player LocalPlayer;

        Pen TeamPen = new Pen(Color.Blue, 3);

        private ESP()
        {
            InitializeComponent();
        }

        public static ESP Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ESP();
                }
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            ezO.SetInvi(this);
            ezO.DoStuff("AssaultCube", this);         
        }
        public void RefreshMyPanel()
        {
            panel1.Refresh();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            g = e.Graphics;

            if(Players == null) { return; }
            foreach (var player in Players)
            {
             
                  if((player.Team == LocalPlayer.Team)&&(player.Health > 1)&& player.Health < 101)
                    {
                       if(player.Bottom.X > 0 && player.Bottom.Y < Height && player.Bottom.X < Width && player.Bottom.Y > 0)
                        {
                            g.DrawRectangle(TeamPen, player.CreateRectangle());
                        }
                    } 
                    else
                    {
                       if (player.Bottom.X > 0 && player.Bottom.Y < Height && player.Bottom.X < Width && player.Bottom.Y > 0)
                       {
                            g.DrawRectangle(EnemyHp(player.Health), player.CreateRectangle());
                        }
                    }              
            }
        }

        public void passPlayers(Player p, List<Player> ps) 
        {
            if((p != null) && (ps != null))
            LocalPlayer = p;
            Players = ps;
        }

        Pen EnemyHp (int hp)
        {
            if (hp > 80) return new Pen(Color.FromArgb(16, 255, 0), 3);
            else if (hp > 60) return new Pen(Color.FromArgb(64, 204, 0), 3);
            else if (hp > 40) return new Pen(Color.FromArgb(112, 153, 0), 3);
            else if (hp > 20) return new Pen(Color.FromArgb(159, 102, 0), 3);
            else if (hp > 1) return new Pen(Color.FromArgb(207, 51, 0), 3);
            else if (hp > 1) return new Pen(Color.FromArgb(255, 0, 0), 3);
            else return new Pen(Color.Black, 3);
        }
    }
}
