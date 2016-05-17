using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace BadaoKingdom.BadaoChampion.BadaoGangplank
{
    public static class BadaoGangplankConfig
    {
        public static Menu Menu { get; set; }
        public static Menu comboMenu, harassMenu, healMenu, clearMenu, miscMenu, lasthitMenu, jungleMenu;
        public static void BadaoActivate()
        {

            // spells init
            BadaoMainVariables.Q = new Spell(SpellSlot.Q, 625);
            BadaoMainVariables.W = new Spell(SpellSlot.W);
            BadaoMainVariables.E = new Spell(SpellSlot.E,1000);
            BadaoMainVariables.R = new Spell(SpellSlot.R);

            // main menu

            Menu = MainMenu.AddMenu("Gangplank", "Gangplank");

            comboMenu = Menu.AddSubMenu("Combo", "Combo");
            comboMenu.Add("ComboE1", new CheckBox("Place 1st Barrel"));

            harassMenu = Menu.AddSubMenu("Harass", "Harass");
            harassMenu.Add("HarassQ", new CheckBox("Q"));

            clearMenu = Menu.AddSubMenu("LaneClear", "LaneClear");
            clearMenu.Add("LaneQ", new CheckBox("Use Q last hit"));

            jungleMenu = Menu.AddSubMenu("JungleClear", "JungleClear");
            jungleMenu.Add("jungleQ", new CheckBox("Use Q last hit"));

            miscMenu = Menu.AddSubMenu("Auto", "Auto");
            miscMenu.Add("AutoWLowHealth", new CheckBox("W when low health"));
            miscMenu.Add("AutoWLowHealthValue", new Slider("% HP to W", 20, 1, 100));
            miscMenu.Add("AutoWCC", new CheckBox("W anti CC"));

        }
    }
}
