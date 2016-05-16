using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;

namespace PortAIO.Utility
{
    class Loader
    {

        public static bool useActivator { get { return Miscc["activator"].Cast<CheckBox>().CurrentValue; } }
        public static bool cheat { get { return Miscc["cheat"].Cast<CheckBox>().CurrentValue; } }
        public static bool banwards { get { return Miscc["banwards"].Cast<CheckBox>().CurrentValue; } }
        public static bool useTracker { get { return Miscc["tracker"].Cast<CheckBox>().CurrentValue; } }
        public static bool useRecall { get { return Miscc["recall"].Cast<CheckBox>().CurrentValue; } }
        public static bool useSkin { get { return Miscc["skin"].Cast<CheckBox>().CurrentValue; } }
        public static bool champOnly { get { return Miscc["champ"].Cast<CheckBox>().CurrentValue; } }
        public static bool utilOnly { get { return Miscc["util"].Cast<CheckBox>().CurrentValue; } }
        public static bool evade { get { return Miscc["evade"].Cast<CheckBox>().CurrentValue; } }
        public static bool godTracker { get { return Miscc["godTracker"].Cast<CheckBox>().CurrentValue; } }
        public static bool ping { get { return Miscc["ping"].Cast<CheckBox>().CurrentValue; } }
        public static bool human { get { return Miscc["human"].Cast<CheckBox>().CurrentValue; } }
        public static int soraka { get { return Miscc["soraka"].Cast<ComboBox>().CurrentValue; } }
        public static int kogmaw { get { return Miscc["kogmaw"].Cast<ComboBox>().CurrentValue; } }
        public static int lux { get { return Miscc["lux"].Cast<ComboBox>().CurrentValue; } }
        public static int leesin { get { return Miscc["leesin"].Cast<ComboBox>().CurrentValue; } }
        public static bool bubba { get { return Miscc["bubba"].Cast<CheckBox>().CurrentValue; } }
        public static int kalista { get { return Miscc["kalista"].Cast<ComboBox>().CurrentValue; } }
        public static bool gank { get { return Miscc["gank"].Cast<CheckBox>().CurrentValue; } }
        public static int diana { get { return Miscc["diana"].Cast<ComboBox>().CurrentValue; } }
        public static int draven { get { return Miscc["draven"].Cast<ComboBox>().CurrentValue; } }
        public static int cait { get { return Miscc["cait"].Cast<ComboBox>().CurrentValue; } }
        public static bool intro { get { return Miscc["intro"].Cast<CheckBox>().CurrentValue; } }
        public static int twitch { get { return Miscc["twitch"].Cast<ComboBox>().CurrentValue; } }
        public static int nidalee { get { return Miscc["nidalee"].Cast<ComboBox>().CurrentValue; } }
        public static int morgana { get { return Miscc["morgana"].Cast<ComboBox>().CurrentValue; } }
        public static int lucian { get { return Miscc["lucian"].Cast<ComboBox>().CurrentValue; } }
        public static int ashe { get { return Miscc["ashe"].Cast<ComboBox>().CurrentValue; } }
        public static int vayne { get { return Miscc["vayne"].Cast<ComboBox>().CurrentValue; } }
        public static int quinn { get { return Miscc["quinn"].Cast<ComboBox>().CurrentValue; } }
        public static int kayle { get { return Miscc["kayle"].Cast<ComboBox>().CurrentValue; } }
        public static int jayce { get { return Miscc["jayce"].Cast<ComboBox>().CurrentValue; } }
        public static int yasuo { get { return Miscc["yasuo"].Cast<ComboBox>().CurrentValue; } }
        public static int katarina { get { return Miscc["katarina"].Cast<ComboBox>().CurrentValue; } }
        public static int xerath { get { return Miscc["xerath"].Cast<ComboBox>().CurrentValue; } }
        public static int gragas { get { return Miscc["gragas"].Cast<ComboBox>().CurrentValue; } }
        public static int ezreal { get { return Miscc["ezreal"].Cast<ComboBox>().CurrentValue; } }
        public static int brand { get { return Miscc["brand"].Cast<ComboBox>().CurrentValue; } }
        public static int blitzcrank { get { return Miscc["blitzcrank"].Cast<ComboBox>().CurrentValue; } }
        public static int corki { get { return Miscc["corki"].Cast<ComboBox>().CurrentValue; } }
        public static int darius { get { return Miscc["darius"].Cast<ComboBox>().CurrentValue; } }
        public static int evelynn { get { return Miscc["evelynn"].Cast<ComboBox>().CurrentValue; } }
        public static int jhin { get { return Miscc["jhin"].Cast<ComboBox>().CurrentValue; } }
        public static int kindred { get { return Miscc["kindred"].Cast<ComboBox>().CurrentValue; } }
        public static int sion { get { return Miscc["sion"].Cast<ComboBox>().CurrentValue; } }
        public static int jax { get { return Miscc["jax"].Cast<ComboBox>().CurrentValue; } }
        public static int sona { get { return Miscc["sona"].Cast<ComboBox>().CurrentValue; } }
        public static int ryze { get { return Miscc["ryze"].Cast<ComboBox>().CurrentValue; } }


        public static Menu Miscc;

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static List<string> RandomUltChampsList = new List<string>(new[] { "Ezreal", "Jinx", "Ashe", "Draven", "Gangplank", "Ziggs", "Lux", "Xerath" });
        public static List<string> BaseUltList = new List<string>(new[] { "Jinx", "Ashe", "Draven", "Ezreal", "Karthus"});
        public static List<string> Champion = new List<string>(new[] {
            "Soraka", // 0
            "KogMaw", // 1
            "LeeSin", // 2
            "Kalista", // 3
            "Diana", // 4
            "Caitlyn", // 5
            "Twitch", // 6
            "Nidalee", // 7
            "Lucian", // 8
            "Ashe", // 9
            "Vayne", // 10
            "Jayce", // 11
            "Yasuo", // 12
            "Katarina", // 13
            "Xerath", // 14
            "Gragas", // 15
            "Draven", // 16
            "Ezreal", // 17
            "Brand", // 18
            "Blitzcrank", //19
            "Corki", // 20
            "Darius", // 21
            "Evelynn", // 22
            "Jhin", //23
            "Kindred", // 24
            "Lux", //25
            "Morgana", //26
            "Quinn", //27
            "Kayle", //28
            "Sion", //29
            "Jax", //30
            "Sona", //31
            "Ryze", //32


        });

        public static void Menu()
        {
            Miscc = MainMenu.AddMenu("PortAIO Misc", "berbsicmisc");
            Miscc.Add("intro", new CheckBox("Load Intro?", true));
            Miscc.AddSeparator();
            Miscc.AddGroupLabel("Champion Changes");
            if (Champion.Contains(ObjectManager.Player.ChampionName))
            {
                if (Player.ChampionName.Equals(Champion[0]))
                {
                    Miscc.Add("soraka", new ComboBox("Use addon for Soraka : ", 0, "Sophie Soraka", "ChallengerSeries"));
                }
                if (Player.ChampionName.Equals(Champion[1]))
                {
                    Miscc.Add("kogmaw", new ComboBox("Use addon for Kog'Maw : ", 0, "Sharpshooter", "ChallengerSeries", "OKTW"));
                }
                if (Player.ChampionName.Equals(Champion[2]))
                {
                    Miscc.Add("leesin", new ComboBox("Use addon for Lee Sin : ", 0, "ValvraveSharp", "El Lee Sin : Reborn"));
                }
                if (Player.ChampionName.Equals(Champion[3]))
                {
                    Miscc.Add("kalista", new ComboBox("Use addon for Kalista : ", 0, "iKalista", "iKalista - Reborn", "ChallengerSeries"));
                }
                if (Player.ChampionName.Equals(Champion[4]))
                {
                    Miscc.Add("diana", new ComboBox("Use addon for Diana : ", 0, "ElDiana", "Nechrito Diana"));
                }
                if (Player.ChampionName.Equals(Champion[5]))
                {
                    Miscc.Add("cait", new ComboBox("Use addon for Caitlyn : ", 0, "OKTW", "ExorSeries : AIO"));
                }
                if (Player.ChampionName.Equals(Champion[6]))
                {
                    Miscc.Add("twitch", new ComboBox("Use addon for Twitch : ", 0, "OKTW", "Nechrito Twitch"));
                }
                if (Player.ChampionName.Equals(Champion[7]))
                {
                    Miscc.Add("nidalee", new ComboBox("Use addon for Nidalee : ", 0, "Kurisu", "Nechrito"));
                }
                if (Player.ChampionName.Equals(Champion[8]))
                {
                    Miscc.Add("lucian", new ComboBox("Use addon for Lucian : ", 0, "LCS Lucian", "ChallengerSeries", "iLucian"));
                }
                if (Player.ChampionName.Equals(Champion[9]))
                {
                    Miscc.Add("ashe", new ComboBox("Use addon for Ashe : ", 0, "OKTW", "ChallengerSeries"));
                }
                if (Player.ChampionName.Equals(Champion[10]))
                {
                    Miscc.Add("vayne", new ComboBox("Use addon for Vayne : ", 0, "ChallengerVayne", "VayneHunterReborn"));
                }
                if (Player.ChampionName.Equals(Champion[11]))
                {
                    Miscc.Add("jayce", new ComboBox("Use addon for Jayce : ", 0, "OKTW", "Hoe's Jayce"));
                }
                if (Player.ChampionName.Equals(Champion[12]))
                {
                    Miscc.Add("yasuo", new ComboBox("Use addon for Yasuo : ", 0, "ValvraveSharp", "YasuoPro"));
                }
                if (Player.ChampionName.Equals(Champion[13]))
                {
                    Miscc.Add("katarina", new ComboBox("Use addon for Katarina : ", 0, "Staberina", "e.Motion Katarina"));
                }
                if (Player.ChampionName.Equals(Champion[14]))
                {
                    Miscc.Add("xerath", new ComboBox("Use addon for Xerath : ", 0, "OKTW", "ElXerath"));
                }
                if (Player.ChampionName.Equals(Champion[15]))
                {
                    Miscc.Add("gragas", new ComboBox("Use addon for Gragas : ", 0, "Drunk Carry", "Nechrito"));
                }
                if (Player.ChampionName.Equals(Champion[16]))
                {
                    Miscc.Add("draven", new ComboBox("Use addon for Draven : ", 0, "Sharp Shooter/Exor", "Tyler1"));
                }
                if (Player.ChampionName.Equals(Champion[17]))
                {
                    Miscc.Add("ezreal", new ComboBox("Use addon for Ezreal : ", 0, "OKTW", "iDzEzreal"));
                }
                if (Player.ChampionName.Equals(Champion[18]))
                {
                    Miscc.Add("brand", new ComboBox("Use addon for Brand : ", 0, "TheBrand", "OKTW"));
                }
                if (Player.ChampionName.Equals(Champion[19]))
                {
                    Miscc.Add("blitzcrank", new ComboBox("Use addon for Blitzcrank : ", 0, "FreshBooster", "OKTW"));
                }
                if (Player.ChampionName.Equals(Champion[20]))
                {
                    Miscc.Add("corki", new ComboBox("Use addon for Corki : ", 0, "ElCorki", "OKTW"));
                }
                if (Player.ChampionName.Equals(Champion[21]))
                {
                    Miscc.Add("darius", new ComboBox("Use addon for Darius : ", 0, "ExoryAIO", "OKTW"));
                }
                if (Player.ChampionName.Equals(Champion[22]))
                {
                    Miscc.Add("evelynn", new ComboBox("Use addon for Evelynn : ", 0, "Evelynn#", "OKTW"));
                }
                if (Player.ChampionName.Equals(Champion[23]))
                {
                    Miscc.Add("jhin", new ComboBox("Use addon for Jhin : ", 0, "Jhin Virtuoso", "OKTW"));
                }
                if (Player.ChampionName.Equals(Champion[24]))
                {
                    Miscc.Add("kindred", new ComboBox("Use addon for Kindred : ", 0, "Kindred Yin Yang", "OKTW"));
                }
                if (Player.ChampionName.Equals(Champion[25]))
                {
                    Miscc.Add("lux", new ComboBox("Use addon for Lux : ", 0, "MoonLux", "OKTW"));
                }
                if (Player.ChampionName.Equals(Champion[26]))
                {
                    Miscc.Add("morgana", new ComboBox("Use addon for Morgana : ", 0, "Kurisu Morgana", "OKTW"));
                }
                if (Player.ChampionName.Equals(Champion[27]))
                {
                    Miscc.Add("quinn", new ComboBox("Use addon for Quinn : ", 0, "GFuel Quinn", "OKTW"));
                }
                if (Player.ChampionName.Equals(Champion[28]))
                {
                    Miscc.Add("kayle", new ComboBox("Use addon for Quinn : ", 0, "Seph Kayle", "OKTW"));
                }
                if (Player.ChampionName.Equals(Champion[29]))
                {
                    Miscc.Add("Sion", new ComboBox("Use addon for Sion : ", 0, "Underrated Sion", "Simple Sion"));
                }
                if (Player.ChampionName.Equals(Champion[30]))
                {
                    Miscc.Add("Jax", new ComboBox("Use addon for Jax : ", 0, "xQx Jax", "Noob Jax Reloaded"));
                }
                if (Player.ChampionName.Equals(Champion[31]))
                {
                    Miscc.Add("Sona", new ComboBox("Use addon for Sona : ", 0, "vSeries Sona", "ElSona"));
                }
                if (Player.ChampionName.Equals(Champion[32]))
                {
                    Miscc.Add("Ryze", new ComboBox("Use addon for Ryze : ", 0, "ExoryAIO Ryze", "ElRyze"));
                }
            }
            else
            {
                Miscc.AddLabel("This champion is not supported for these feature.");
            }
            Miscc.AddSeparator();
            Miscc.AddGroupLabel("Util Changes");
            Miscc.AddLabel("Please F5 after making any changes below >>");
            Miscc.Add("champ", new CheckBox("Champ only mode? (No utils will load)", false));
            Miscc.Add("util", new CheckBox("Util only mode? (No champs will load)", false));
            Miscc.AddSeparator();
            Miscc.Add("activator", new CheckBox("Enable ElUtilitySuite?"));
            Miscc.Add("tracker", new CheckBox("Enable NabbTracker?"));
            Miscc.Add("recall", new CheckBox("Enable Recall Tracker?"));
            Miscc.AddSeparator();
            Miscc.Add("skin", new CheckBox("Enable Skin Hack?"));
            Miscc.Add("evade", new CheckBox("Enable Evade?", false));
            Miscc.Add("godTracker", new CheckBox("Enable God Jungle Tracker?", false));
            Miscc.AddSeparator();
            Miscc.Add("ping", new CheckBox("Enable Ping Block?", false));
            Miscc.Add("human", new CheckBox("Enable Humanizer?", false));
            Miscc.Add("gank", new CheckBox("Enable GankAlerter?", false));
            Miscc.AddSeparator();
            Miscc.Add("cheat", new CheckBox("Enable TheCheater?", false));
            Miscc.Add("banwards", new CheckBox("Enable Sebby BanWars?", false));



            /*
            Miscc.Add("stream", new CheckBox("Enable StreamBuddy?", false));
            public static bool stream { get { return Miscc["stream"].Cast<CheckBox>().CurrentValue; } }
            public static bool randomUlt { get { return Miscc["randomUlt"].Cast<CheckBox>().CurrentValue; } }
            public static bool baseUlt { get { return Miscc["baseUlt"].Cast<CheckBox>().CurrentValue; } }

            if (RandomUltChampsList.Contains(ObjectManager.Player.ChampionName))
            {
                Miscc.Add("randomUlt", new CheckBox("Enable Random Ult?", false));
            }

            if (BaseUltList.Contains(ObjectManager.Player.ChampionName))
            {
                Miscc.Add("baseUlt", new CheckBox("Enable Base Ult?", false));
            }
            */
        }
    }
}
