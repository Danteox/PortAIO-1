using System.Drawing;
using System.Linq;
using EloBuddy;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace FastTrundle
{
    public class FastTrundleMenu
    {
        #region Data

        public static Menu Menu { get; set; }
        public static Menu comboMenu, harassMenu, healMenu, clearMenu, miscMenu, lasthitMenu, jungleMenu, itemMenu;
        #endregion

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

        #region Methods

        public static void Initialize()
        {

            Menu = MainMenu.AddMenu("Fast Trundle", "Trundle");

            comboMenu = Menu.AddSubMenu("Combo", "Combo");
            comboMenu.Add("FastTrundle.Combo.Q", new CheckBox("Use Q"));
            comboMenu.Add("FastTrundle.Combo.W", new CheckBox("Use W"));
            comboMenu.Add("FastTrundle.Combo.E", new CheckBox("Use E"));
            comboMenu.Add("FastTrundle.Combo.R", new CheckBox("Use R"));
            comboMenu.AddSeparator();
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                comboMenu.Add("FastTrundle.R.On" + hero.CharData.BaseSkinName, new CheckBox("Use R on: " + hero.CharData.BaseSkinName));           
            }
            comboMenu.AddSeparator();
            comboMenu.Add("FastTrundle.Combo.Ignite", new CheckBox("Use Ignite"));

            harassMenu = Menu.AddSubMenu("Harass", "Harass");
            harassMenu.Add("FastTrundle.Harass.Q", new CheckBox("Use Q"));
            harassMenu.Add("FastTrundle.Harass.W", new CheckBox("Use W"));
            harassMenu.Add("FastTrundle.Harass.E", new CheckBox("Use E"));
            harassMenu.Add("FastTrundle.Harass.Mana", new Slider("Minimum mana", 25, 1, 100));


            lasthitMenu = Menu.AddSubMenu("LastHit", "Lasthit");
            lasthitMenu.Add("FastTrundle.LastHit.Q", new CheckBox("Use Q"));
            lasthitMenu.Add("FastTrundle.LastHit.Mana", new Slider("Minimum mana", 25, 1, 100));

            clearMenu = Menu.AddSubMenu("Laneclear", "Laneclear");
            clearMenu.Add("FastTrundle.LaneClear.Q", new CheckBox("Use Q"));
            clearMenu.Add("FastTrundle.LaneClear.Q.Lasthit", new CheckBox("Only lasthit with Q"));
            clearMenu.Add("FastTrundle.LaneClear.W", new CheckBox("Use W"));
            clearMenu.Add("FastTrundle.LaneClear.Mana", new Slider("Minimum mana", 25, 1, 100));

            jungleMenu = Menu.AddSubMenu("Jungleclear", "Jungleclear");
            jungleMenu.Add("FastTrundle.JungleClear.Q", new CheckBox("Use Q"));
            jungleMenu.Add("FastTrundle.JungleClear.W", new CheckBox("Use W"));
            jungleMenu.Add("FastTrundle.JungleClear.Mana", new Slider("Minimum mana", 25, 1, 100));

            itemMenu = Menu.AddSubMenu("Items", "Items");
            itemMenu.Add("FastTrundle.Items.Hydra", new CheckBox("Use Tiamat / Ravenous Hydra"));
            itemMenu.Add("FastTrundle.Items.Titanic", new CheckBox("Use Titanic Hydra"));
            itemMenu.Add("FastTrundle.Items.Youmuu", new CheckBox("Use Youmuu's Ghostblade"));
            itemMenu.Add("FastTrundle.Items.Blade", new CheckBox("Use Cutlass / BOTRK"));
            itemMenu.Add("FastTrundle.Items.Blade.MyHP", new Slider("When my HP % <", 50, 1, 100));

            miscMenu = Menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("FastTrundle.Draw.Q", new CheckBox("Draw Q"));
            miscMenu.Add("FastTrundle.Draw.W", new CheckBox("Draw W"));
            miscMenu.Add("FastTrundle.Draw.E", new CheckBox("Draw E"));
            miscMenu.Add("FastTrundle.Draw.R", new CheckBox("Draw R"));
            miscMenu.Add("FastTrundle.Draw.Pillar", new CheckBox("Draw Pillar"));
            miscMenu.Add("FastTrundle.Antigapcloser", new CheckBox("Antigapcloser"));
            miscMenu.Add("FastTrundle.Interrupter", new CheckBox("Interrupter"));

        }

        #endregion
    }
}