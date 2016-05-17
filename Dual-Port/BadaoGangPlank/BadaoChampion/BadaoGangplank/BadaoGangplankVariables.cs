using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Menu.Values;

namespace BadaoKingdom.BadaoChampion.BadaoGangplank
{
    public static class BadaoGangplankVariables
    {
        public static bool ComboE1 { get { return BadaoGangplankConfig.comboMenu["ComboE1"].Cast<CheckBox>().CurrentValue; } }
        public static bool HarassQ { get { return BadaoGangplankConfig.harassMenu["HarassQ"].Cast<CheckBox>().CurrentValue; } }
        public static bool JungleQ { get { return BadaoGangplankConfig.jungleMenu["jungleQ"].Cast<CheckBox>().CurrentValue; } }
        public static bool LaneQ { get { return BadaoGangplankConfig.clearMenu["LaneQ"].Cast<CheckBox>().CurrentValue; } }
        public static bool AutoWLowHealth { get { return BadaoGangplankConfig.miscMenu["AutoWLowHealth"].Cast<CheckBox>().CurrentValue; } }
        public static int AutoWLowHealthValue { get { return BadaoGangplankConfig.miscMenu["AutoWLowHealthValue"].Cast<Slider>().CurrentValue; } }
        public static bool AutoWCC { get { return BadaoGangplankConfig.miscMenu["AutoWCC"].Cast<CheckBox>().CurrentValue; } }

    }
}
