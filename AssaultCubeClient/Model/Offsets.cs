using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssaultCubeClient.Model
{

    class Offsets
    {
        public static string PlayerBase = "ac_client.exe+0x00195404";
        public static string EntityList = "ac_client.exe+0x18AC04";
        public static string PlayerCount = "ac_client.exe+0x18AC0C";
        public static string Health = ",0xEC";
        public static string Ammo = ",0x140";
        public static string GrenadeAmmo = ",0x144";
        public static string SecAmmo = ",0x12C";
        public static string Y = ",0x2C";
        public static string X = ",0x28";
        public static string Z = ",0x30";
        public static string ViewY = ",0x38";
        public static string ViewX = ",0x34";
    }
}
