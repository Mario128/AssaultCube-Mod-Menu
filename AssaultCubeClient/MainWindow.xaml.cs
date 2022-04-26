using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using Memory;
using AssaultCubeClient.Model;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace AssaultCubeClient
{
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(Keys vkey);
  
        private static Mem Mem = new Mem();
        private Thread aimbotThread = new Thread(StartAimbot);
        private static ManualResetEvent aimbotMre = new ManualResetEvent(false);

        Graphics g;
        Pen teampen = new Pen(Color.Blue, 3);
        Pen enemypen = new Pen(Color.Red, 3);    

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Attach_Click(object sender, RoutedEventArgs e)
        { 
            int PID = Mem.GetProcIdFromName("ac_client");
            if(PID > 0)
            {
                Mem.OpenProcess(PID);
                System.Windows.MessageBox.Show("Found process, " + PID.ToString());
            }
            else
            {
                System.Windows.MessageBox.Show("Process not found");
            }
        }
        private void MyWindow_loaded(object sender, RoutedEventArgs e)
        {
        
            //CheckForIllegalCrossFieldCalls = false;
            int PID = Mem.GetProcIdFromName("ac_client");
            if (PID > 0)
            {
                Mem.OpenProcess(PID);
                System.Windows.MessageBox.Show("Found process, " + PID.ToString());
            }
        }

        private static void StartAimbot()
        {
            while(true)
            {
                if (GetAsyncKeyState(Keys.D1) < 0)
                {
                    aimbotMre.WaitOne();
                    Player localPlayer = GetLocal();
                    
                    List<Player> players = GetPlayers(localPlayer);
                   
                    players = players.OrderBy(o => o.Magnitude).ToList();

                    if(players.Count() > 0)
                    {
                        Aim(localPlayer, players[0]);
                    }

                    Thread.Sleep(5);
                }
                Thread.Sleep(3);

            }
        }  
        private void SetAmmo()
        {
                Mem.WriteMemory(Offsets.PlayerBase + Offsets.Ammo, "int", "999999");
                Mem.WriteMemory(Offsets.PlayerBase + Offsets.SecAmmo, "int", "999999");
                Mem.WriteMemory(Offsets.PlayerBase + Offsets.GrenadeAmmo, "int", "999999");     
        }
        private void SetHealth()
        {
                Mem.WriteMemory(Offsets.PlayerBase + Offsets.Health, "int", "999999");
           
        }

        private void Ammo_Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            SetAmmo(); 
        }
        private void Health_Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            SetHealth();
        }
        private void Aimbot_Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            if (!aimbotThread.IsAlive)
            {
                aimbotThread.Start();
            }
            aimbotMre.Set();
        }
        private void Aimbot_Checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            aimbotMre.Reset();
        }

        private static Player GetLocal()
        {
            Player player = new Player()
            {
                X = Mem.ReadFloat(Offsets.PlayerBase + Offsets.X),
                Y = Mem.ReadFloat(Offsets.PlayerBase + Offsets.Y),
                Z = Mem.ReadFloat(Offsets.PlayerBase + Offsets.Z),
                Team = Mem.ReadInt(Offsets.PlayerBase + Offsets.Team) == 1 ? Flag.Blue : Flag.Red
            };
            return player;
        }
        private static List<Player> GetPlayers(Player local)
        {
            List<Player> players = new List<Player>();
            int playerCount = Mem.ReadInt(Offsets.PlayerCount);

            for(int i = 0; i<playerCount; i++)
            {
                var currentStr = Offsets.EntityList + ",0x" + (i*0x4).ToString("X");

                Player player = new Player()
                {
                    X = Convert.ToDouble(Mem.ReadFloat(currentStr + Offsets.X)),
                    Y = Convert.ToDouble(Mem.ReadFloat(currentStr + Offsets.Y)),
                    Z = Convert.ToDouble(Mem.ReadFloat(currentStr + Offsets.Z)),
                    Health = Mem.ReadInt(currentStr + Offsets.Health),
                    Team = Mem.ReadInt(currentStr + Offsets.Team) == 1 ? Flag.Blue : Flag.Red
                };
                player.Magnitude = GetMag(local, player);

                if((player.Health > 0 && player.Health < 102)&& (player.Team != local.Team))
                {
                    players.Add(player);
                }
            }

            return players;
        }
        private static double GetMag(Player local, Player entity)
        {
            double mag = Math.Sqrt(Math.Pow(entity.X - local.X, 2) + Math.Pow(entity.Y - local.Y, 2) + Math.Pow(entity.Z - local.Z, 2));
            
            return mag;
        }
        private static void Aim (Player player, Player enemy)
        {
            double deltaX = enemy.X - player.X;
            double deltaY = enemy.Y - player.Y;
            double deltaZ = enemy.Z - player.Z;

            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            int viewX = Convert.ToInt32((Math.Atan2(deltaY, deltaX) * 180 / Math.PI) + 90);
           
            int viewY = Convert.ToInt32(Math.Atan2(deltaZ, distance) * 180 / Math.PI);

            Mem.WriteMemory(Offsets.PlayerBase + Offsets.ViewX, "float", Convert.ToString(viewX));
            Mem.WriteMemory(Offsets.PlayerBase + Offsets.ViewY, "float", Convert.ToString(viewY));

        }

        private void ESP ()
        {
            while (true)
            {
                Player player = GetLocal();
                List<Player> players = GetPlayers(player);
            }
        }

        private void ESP_Checked(object sender, RoutedEventArgs e)
        { 

        }
    }

}
