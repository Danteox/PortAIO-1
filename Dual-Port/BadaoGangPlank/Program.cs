﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;

namespace BadaoKingdom
{
    static class Program
    {
        public static readonly List<string> SupportedChampion = new List<string>()
        {
            "Gangplank"
        };

        public static void Game_OnGameLoad()
        {
            if (!SupportedChampion.Contains(ObjectManager.Player.ChampionName))
            {
                return;
            }
            Chat.Print("<font color=\"#24ff24\">Badao </font>" + "<font color=\"#ff8d1a\">" +
                ObjectManager.Player.ChampionName + "</font>" + "<font color=\"#24ff24\"> loaded!</font>");
            BadaoChampion.BadaoGangplank.BadaoGangplank.BadaoActivate();
        }
    }
}
