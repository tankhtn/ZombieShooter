using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace ZombieShooter
{
    public class Global
    {
        public static bool isMusic = true;
        public static bool isDifficult = false;
        public static Song MenuSong;
        public static ContentManager Content = null;
        public static float Level1Time = 300;

        public static int MaxPlayerHP = 3000;
        public static int PlayerHP = 2000;
        public static int ZombieHP = 300;
        public static int SpiderHP = 100;
        public static int MonsterHP = 700;

        public static int ZombieDam = 100;
        public static int SpiderDam = 50;
        public static int MonsterDam = 200;
        public static int ShortGunDam = 200;
        public static int RocketDam = 400;

        public static int ZombieMoney = 100;
        public static int SpriderMoney = 50;
        public static int MonsterMoney = 250;
    }
}
