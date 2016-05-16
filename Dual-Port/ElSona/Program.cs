namespace ELSona
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using EloBuddy;
    using LeagueSharp.Common;
    using EloBuddy.SDK;
    using Spell = LeagueSharp.Common.Spell;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    internal class Sona 
    {
        #region Static Fields

        private static readonly Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
                                                                       {
                                                                           { Spells.Q, new Spell(SpellSlot.Q, 850) },
                                                                           { Spells.W, new Spell(SpellSlot.W, 1000) },
                                                                           { Spells.E, new Spell(SpellSlot.E, 350) },
                                                                           { Spells.R, new Spell(SpellSlot.R, 1000) }
                                                                       };

        private static SpellSlot Ignite;


        #endregion

        #region Enums

        public enum Spells
        {
            Q,

            W,

            E,

            R
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        private static Menu Menu;
        public static Menu comboMenu, harassMenu, healMenu, laneMenu, ksMenu, drawMenu, miscMenu;

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private static AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>


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


        public static void Load()
        {

            Menu = MainMenu.AddMenu("ELSona", "Sona");

            comboMenu = Menu.AddSubMenu("Combo", "Combo");
            comboMenu.Add("ElEasy.Sona.Combo.Q", new CheckBox("Use Q"));
            comboMenu.Add("ElEasy.Sona.Combo.W", new CheckBox("Use W", false));
            comboMenu.Add("ElEasy.Sona.Combo.E", new CheckBox("Use E"));
            comboMenu.Add("ElEasy.Sona.Combo.R", new CheckBox("Use R"));
            comboMenu.Add("ElEasy.Sona.Combo.Count.R", new Slider("Minimum hit by R", 2, 1, 5));
            comboMenu.Add("ElEasy.Sona.Combo.Ignite", new CheckBox("Use Ignite"));

            harassMenu = Menu.AddSubMenu("Harass", "Harass");
            harassMenu.Add("ElEasy.Sona.Harass.Q", new CheckBox("Use Q"));
            harassMenu.Add("ElEasy.Sona.Harass.Player.Mana", new Slider("Minimum Mana", 55, 1, 100));
            harassMenu.Add("ElEasy.Sona.Autoharass.Activated", new KeyBind("Autoharass", false, KeyBind.BindTypes.PressToggle, 'L'));


            healMenu = Menu.AddSubMenu("Heal", "Heal");
            healMenu.Add("ElEasy.Sona.Heal.Activated", new CheckBox("Heal"));
            healMenu.Add("ElEasy.Sona.Heal.Player.HP", new Slider("Player HP", 55, 1, 100));
            healMenu.Add("ElEasy.Sona.Heal.Ally.HP", new Slider("Ally HP", 55, 1, 100));
            healMenu.Add("ElEasy.Sona.Heal.Player.Mana", new Slider("Minimum Mana", 55, 1, 100));


            miscMenu = Menu.AddSubMenu("Miscellaneous", "Misc");
            miscMenu.Add("ElEasy.Sona.Interrupt.Activated", new CheckBox("Interrupt spells"));
            miscMenu.Add("ElEasy.SonaGapCloser.Activated", new CheckBox("Anti gapcloser"));


            drawMenu = Menu.AddSubMenu("Drawings", "Draw");
            drawMenu.Add("ElEasy.Sona.Draw.off", new CheckBox("Turn drawings off"));
            drawMenu.Add("ElEasy.Sona.Draw.Q", new CheckBox("Draw Q"));
            drawMenu.Add("ElEasy.Sona.Draw.W", new CheckBox("Draw W"));
            drawMenu.Add("ElEasy.Sona.Draw.E", new CheckBox("Draw E"));
            drawMenu.Add("ElEasy.Sona.Draw.R", new CheckBox("Draw R"));

            Console.WriteLine("Loaded Sona");
            Ignite = Player.GetSpellSlot("summonerdot");

            spells[Spells.R].SetSkillshot(0.5f, 125, 3000f, false, SkillshotType.SkillshotLine);

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }

        #endregion

        #region Methods

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (getCheckBoxItem(miscMenu, "ElEasy.SonaGapCloser.Activated") && spells[Spells.R].IsReady()
                && gapcloser.Sender.IsValidTarget(spells[Spells.R].Range))
            {
                spells[Spells.R].Cast(gapcloser.Sender);
            }
        }

        private static void AutoHarass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, DamageType.Magical);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            if (getKeyBindItem(harassMenu, "ElEasy.Sona.Autoharass.Activated")
                && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast(target);
            }
        }

        private static void HealManager()
        {
            var useHeal = getCheckBoxItem(healMenu, "ElEasy.Sona.Heal.Activated");
            var playerMana = getSliderItem(healMenu, "ElEasy.Sona.Heal.Player.Mana");
            var playerHp = getSliderItem(healMenu, "ElEasy.Sona.Heal.Player.HP");
            var allyHp = getSliderItem(healMenu, "ElEasy.Sona.Heal.Ally.HP");

            if (Player.IsRecalling() || Player.InFountain() || !useHeal
                || Player.ManaPercent < playerMana || !spells[Spells.W].IsReady())
            {
                return;
            }

            if ((Player.Health / Player.MaxHealth) * 100 <= playerHp)
            {
                spells[Spells.W].Cast();
            }

            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(h => h.IsAlly && !h.IsMe))
            {
                if ((hero.Health / hero.MaxHealth) * 100 <= allyHp && spells[Spells.W].IsInRange(hero))
                {
                    spells[Spells.W].Cast();
                }
            }
        }

        private static float IgniteDamage(AIHeroClient target)
        {
            if (Ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)Player.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
        }

        private static void Interrupter2_OnInterruptableTarget(
            AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (args.DangerLevel != Interrupter2.DangerLevel.High
                || sender.Distance(Player) > spells[Spells.R].Range)
            {
                return;
            }

            if (sender.IsValidTarget(spells[Spells.R].Range) && args.DangerLevel == Interrupter2.DangerLevel.High
                && spells[Spells.R].IsReady())
            {
                spells[Spells.R].Cast(sender.Position);
            }
        }

        private static void OnCombo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, DamageType.Magical);
            var rTarget = TargetSelector.GetTarget(spells[Spells.R].Range, DamageType.Magical);

            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            var useQ = getCheckBoxItem(comboMenu, "ElEasy.Sona.Combo.Q");
            var useW = getCheckBoxItem(comboMenu, "ElEasy.Sona.Combo.W");
            var useE = getCheckBoxItem(comboMenu, "ElEasy.Sona.Combo.E");
            var useR = getCheckBoxItem(comboMenu, "ElEasy.Sona.Combo.R");
            var useI = getCheckBoxItem(comboMenu, "ElEasy.Sona.Combo.Ignite");
            var hitByR = getSliderItem(comboMenu, "ElEasy.Sona.Combo.Count.R");

            if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast(target);
            }

            if (useE && spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast();
            }

            if (useW && spells[Spells.W].IsReady() && target.IsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast();
            }

            if (useR && spells[Spells.R].IsReady() && rTarget.IsValidTarget(spells[Spells.R].Range))
            {
                var pred = spells[Spells.R].GetPrediction(target);
                if (pred.Hitchance >= HitChance.High)
                {
                    var hits = HeroManager.Enemies.Where(x => x.Distance(target) <= spells[Spells.R].Width).ToList();
                    Console.WriteLine(hits.Count);
                    if (hits.Any(hit => hits.Count >= hitByR))
                    {
                        spells[Spells.R].Cast(pred.CastPosition);
                    }
                }
            }

            if (Player.Distance(target) <= 600 && IgniteDamage(target) >= target.Health && useI)
            {
                Player.Spellbook.CastSpell(Ignite, target);
            }
        }


         private static void OnDraw(EventArgs args)

        {
            if (getCheckBoxItem(drawMenu, "ElEasy.Sona.Draw.off"))
            {
                return;
            }

            if (getCheckBoxItem(drawMenu, "ElEasy.Sona.Combo.Q"))

                {
                    Render.Circle.DrawCircle(Player.Position, spells[Spells.Q].Range, Color.White);
                }
            

            if (getCheckBoxItem(drawMenu, "ElEasy.Sona.Draw.E"))

                {
                    Render.Circle.DrawCircle(Player.Position, spells[Spells.E].Range, Color.White);
                }
            
    
            if (getCheckBoxItem(drawMenu, "ElEasy.Sona.Draw.W"))

                {
                    Render.Circle.DrawCircle(Player.Position, spells[Spells.W].Range, Color.White);
                }
            
            if (getCheckBoxItem(drawMenu, "ElEasy.Sona.Draw.R"))

                {
                    Render.Circle.DrawCircle(Player.Position, spells[Spells.R].Range, Color.White);
                }
                            
                }
      

        private static void OnHarass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, DamageType.Magical);
            if (target == null || !target.IsValid)
            {
                return;
            }

            if (Player.ManaPercent < getSliderItem(harassMenu, "ElEasy.Sona.Harass.Player.Mana"))
            {
                return;
            }

            if (getCheckBoxItem(harassMenu, "ElEasy.Sona.Harass.Q") && spells[Spells.Q].IsReady()
                && target.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast(target);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            switch (Orbwalker.ActiveModesFlags)
            {
                case Orbwalker.ActiveModes.Combo:
                    OnCombo();
                    break;

                case Orbwalker.ActiveModes.Harass:
                    OnHarass();
                    break;
            }

            HealManager();
            AutoHarass();
        }

        #endregion
    }
}