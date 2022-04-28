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
using ezOverLay;

namespace AssaultCubeClient
{
    public partial class MainWindow : Window
    {
       /* [System.ComponentModel.Browsable(false)]
        public static bool CheckForIllegalCrossThreadCalls { get; set; }
        */
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(Keys vkey);

        private static Mem Mem = new Mem();
        private Thread aimbotThread = new Thread(StartAimbot);
        private Thread espThread;
        private ManualResetEvent espMre = new ManualResetEvent(false);
        private static ManualResetEvent aimbotMre = new ManualResetEvent(false);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Attach_Click(object sender, RoutedEventArgs e)
        {
            int PID = Mem.GetProcIdFromName("ac_client");
            if (PID > 0)
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
         
           // CheckForIllegalCrossThreadCalls = false;
            int PID = Mem.GetProcIdFromName("ac_client");
            if (PID > 0)
            {
                Mem.OpenProcess(PID);
                System.Windows.MessageBox.Show("Found process, " + PID.ToString());
            }
        }

        private static void StartAimbot()
        {
            while (true)
            {
                if (GetAsyncKeyState(Keys.D1) < 0)
                {
                    aimbotMre.WaitOne();
                    Player localPlayer = GetLocal();
                    List<Player> enemyPlayers = new List<Player>();
                    List<Player> players = GetPlayers(localPlayer);

                    foreach (Player p in players)
                    {
                        if (p.Team != localPlayer.Team)
                        {
                            enemyPlayers.Add(p);
                        }
                    }

                    enemyPlayers = enemyPlayers.OrderBy(o => o.Magnitude).ToList();


                    if (enemyPlayers.Count() > 0)
                    {
                        Aim(localPlayer, enemyPlayers[0]);
                    }                
                }
                Thread.Sleep(5);

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

            for (int i = 0; i < playerCount; i++)
            {
                var currentStr = Offsets.EntityList + ",0x" + (i * 0x4).ToString("X");

                Player player = new Player()
                {
                    X = Convert.ToDouble(Mem.ReadFloat(currentStr + Offsets.X)),
                    Y = Convert.ToDouble(Mem.ReadFloat(currentStr + Offsets.Y)),
                    Z = Convert.ToDouble(Mem.ReadFloat(currentStr + Offsets.Z)),
                    Health = Mem.ReadInt(currentStr + Offsets.Health),
                    Team = Mem.ReadInt(currentStr + Offsets.Team) == 1 ? Flag.Blue : Flag.Red
                };
                player.Magnitude = GetMag(local, player);

                if (player.Health > 0 && player.Health < 102)
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
        private static void Aim(Player player, Player enemy)
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

        private void ESP()
        {
            while (true)
            {
                espMre.WaitOne();
                Player Local = GetLocal();
                List<Player>Players = GetPlayers(Local);
                ESP espForm = AssaultCubeClient.ESP.Instance;

                foreach (Player p in Players)
                {
                    float[] m = IntoTheMatrix();
                    p.Bottom = WorldToScreen(m, (float)p.X, (float)p.Y, (float)p.Z, espForm.Width, espForm.Height, false);
                    p.Top = WorldToScreen(m, (float)p.X, (float)p.Y, (float)p.Z, espForm.Width, espForm.Height, true);
                }
 
                espForm.passPlayers(Local, Players);
                espForm.RefreshMyPanel();
                Thread.Sleep(10);
            }
        }

        float[] IntoTheMatrix()
        {
           
            //byte weil genauer
            byte[] buffer = new byte[4 * 16];
            float[] ViewMatrix = new float[16];

            var bytes = Mem.ReadBytes(Offsets.ViewMatrix, (long)buffer.Length);
           
            for (int i = 0; i < ViewMatrix.Length; i++)
            {
                ViewMatrix[i] = BitConverter.ToSingle(bytes, (i * 4));
            }

            return ViewMatrix;
        }

        //Von 3D zu 2D
        System.Drawing.Point WorldToScreen(float[] mtx, float x, float y, float z, int width, int height, bool head)
        { 
           if(head)
            {
                z += 58;
            }
            var twoD = new System.Drawing.Point();

            /*View Matrix Array
             0    1    2    3
             4    5    6    7
             8    9   10   11
            12   13   14   15
            */

            float screenW = (mtx[12] * x) + (mtx[13] * y) + (mtx[14] * z) + mtx[15];

            //Spieler ist sichtbar auf meinem Bildschirm
            if(screenW > 0.001f)
            {
                float screenX = (mtx[0] * x) + (mtx[1] * y) + (mtx[2] * z) + mtx[3];
                float screenY = (mtx[4] * x) + (mtx[5] * y) + (mtx[6] * z) + mtx[7];

                float camX = width / 2f;
                float camY = height / 2f;

                float X = camX + (camX * screenX / screenW);
                float Y = camY - (camY * screenY / screenW);

                twoD.X = (int)X;
                twoD.Y = (int)Y;

                return twoD;
            }
            //nicht sichtbar
            else
            {
                return new System.Drawing.Point(-99, -99);
            }
            
        }

        private void ESP_Checked(object sender, RoutedEventArgs e)
        {
            if(Mem.GetProcIdFromName("ac_client") <= 0)
            {
                return;
            }
            ESP ESPForm = AssaultCubeClient.ESP.Instance;
            ESPForm.Show();
            if(espThread == null)
            {
                espThread = new Thread(ESP);

                if (!espThread.IsAlive)
                {

                    espThread.Start();
                }
            }
          
            espMre.Set();
        }

        private void ESP_Checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            ESP ESPForm = AssaultCubeClient.ESP.Instance;
            ESPForm.Close();
            AssaultCubeClient.ESP.Instance = null;
            espMre.Reset();
            
        }
    }

}
