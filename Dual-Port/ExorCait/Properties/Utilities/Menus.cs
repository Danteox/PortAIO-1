using LeagueSharp.Common;

namespace ExorAIO.Champions.Caitlyn
{
    using System.Drawing;
    using ExorAIO.Utilities;
    using Color = SharpDX.Color;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Menu;    /// <summary>
                                ///     The menu class.
                                /// </summary>
    class Menus
    {
        /// <summary>
        ///     Initializes the menus.
        /// </summary>
        /// 

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        public static void Initialize()
        {
            Variables.QMenu = Variables.Menu.AddSubMenu("Use Q to:", "qmenu");
            Variables.QMenu.Add("qspell.auto", new CheckBox("Logical"));
            Variables.QMenu.Add("qspell.ks", new CheckBox("KillSteal"));
            Variables.QMenu.Add("qspell.farm", new CheckBox("Clear"));
            Variables.QMenu.Add("qspell.mana", new Slider("Clear: Mana >= x%", 50));

            Variables.WMenu = Variables.Menu.AddSubMenu("Use W to:", "wmenu");
            Variables.WMenu.Add("wspell.auto", new CheckBox("Logical"));
            Variables.WMenu.Add("wspell.gp", new CheckBox("Anti-Gapcloser"));

            Variables.EMenu = Variables.Menu.AddSubMenu("Use E to:", "emenu");
            Variables.EMenu.Add("espell.combo", new CheckBox("Combo"));
            Variables.EMenu.Add("espell.gp", new CheckBox("Anti-Gapcloser"));

            Variables.RMenu = Variables.Menu.AddSubMenu("Use R to:", "rmenu");
            Variables.RMenu.Add("rspell.ks", new CheckBox("KillSteal"));

            Variables.DrawingsMenu = Variables.Menu.AddSubMenu("Drawings", "drawingsmenu");
            Variables.DrawingsMenu.Add("drawings.q", new CheckBox("Q Range", false));
            Variables.DrawingsMenu.Add("drawings.w", new CheckBox("W Range", false));
            Variables.DrawingsMenu.Add("drawings.e", new CheckBox("E Range", false));
            Variables.DrawingsMenu.Add("drawings.r", new CheckBox("R Range", false));
        }
    }
}
