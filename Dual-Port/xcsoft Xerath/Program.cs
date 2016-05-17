#region

using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy.SDK;
using EloBuddy;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

#endregion

namespace Xerath_edit
{
    internal class Program
    {
        public const string ChampionName = "Xerath";

        //Orbwalker instance

        //Spells
        public static List<Spell> SpellList = new List<Spell>();

        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;

        //Menu
        private static Menu Menu { get; set; }
        public static Menu comboMenu, harassMenu, rMenu, clearMenu, miscMenu, jungleMenu, drawMenu;

        private static AIHeroClient Player;

        private static Vector2 PingLocation;
        private static int LastPingT = 0;
        private static bool AttacksEnabled
        {
            get
            {
                if (IsCastingR)
                    return false;

                if (Q.IsCharging)
                    return false;

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                    return IsPassiveUp || (!Q.IsReady() && !W.IsReady() && !E.IsReady());

                return true;
            }
        }

        public static bool IsPassiveUp
        {
            get { return ObjectManager.Player.HasBuff("xerathascended2onhit"); }
        }

        public static bool IsCastingR
        {
            get
            {
                return ObjectManager.Player.HasBuff("XerathLocusOfPower2") ||
                       (ObjectManager.Player.LastCastedSpellName() == "XerathLocusOfPower2" &&
                        Environment.TickCount - ObjectManager.Player.LastCastedSpellT() < 500);
            }
        }

        public static class RCharge
        {
            public static int CastT;
            public static int Index;
            public static Vector3 Position;
            public static bool TapKeyPressed;
        }

        public static void Game_OnGameLoad()
        {
            Player = ObjectManager.Player;

            if (Player.ChampionName != ChampionName) return;

            //Create the spells
            Q = new Spell(SpellSlot.Q, 1550);
            W = new Spell(SpellSlot.W, 1000);
            E = new Spell(SpellSlot.E, 1150);
            R = new Spell(SpellSlot.R, 675);

            Q.SetSkillshot(0.6f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.7f, 125f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 60f, 1400f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.7f, 120f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Q.SetCharged("XerathArcanopulseChargeUp", "XerathArcanopulseChargeUp", 750, 1550, 1.5f);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            Menu = MainMenu.AddMenu("Xerath", "Xerath");

            comboMenu = Menu.AddSubMenu("Combo", "Combo");
            comboMenu.Add("UseQCombo", new CheckBox("Use Q"));
            comboMenu.Add("UseWCombo", new CheckBox("Use W"));
            comboMenu.Add("UseECombo", new CheckBox("Use E"));

            rMenu = Menu.AddSubMenu("R", "R");
            rMenu.Add("EnableRUsage", new CheckBox("Auto use charges"));
            rMenu.Add("rMode", new ComboBox("Mode", 1, "Normal", "Custom delays", "OnTap", "Hitchance high"));
            rMenu.Add("rModeKey", new KeyBind("OnTap key", false, KeyBind.BindTypes.HoldActive, 'T'));
            rMenu.AddLabel("Custom delays");
            for (int i = 1; i <= 3; i++)
            rMenu.Add("Delay", new Slider("Delay" + i , 0, 1500, 0));
            rMenu.Add("PingRKillable", new CheckBox("Ping on killable targets (only local)"));
            rMenu.Add("BlockMovement", new CheckBox("Block right click while casting R", false));
            rMenu.Add("OnlyNearMouse", new CheckBox("Focus only targets near mouse", false));
            rMenu.Add("MRadius", new Slider("Radius", 700, 1500, 300));

            harassMenu = Menu.AddSubMenu("Harass", "Harass");
            harassMenu.Add("UseQHarass", new CheckBox("Use Q"));
            harassMenu.Add("UseWHarass", new CheckBox("Use W"));
            harassMenu.Add("HarassActiveT", new KeyBind("Harass (toggle)!", false, KeyBind.BindTypes.PressToggle, 'L'));

            clearMenu = Menu.AddSubMenu("Farm", "Farm");
            clearMenu.Add("UseQFarm", new ComboBox("Use Q", 1, "Freeze", "LaneClear", "Both", "No"));
            clearMenu.Add("UseWFarm", new ComboBox("Use W", 1, "Freeze", "LaneClear", "Both", "No"));

            jungleMenu = Menu.AddSubMenu("JungleFarm", "JungleFarm");
            jungleMenu.Add("UseQJFarm", new CheckBox("Use Q"));
            jungleMenu.Add("UseWJFarm", new CheckBox("Use W"));

            miscMenu = Menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("InterruptSpells", new CheckBox("Interrupt spells"));
            miscMenu.Add("AutoEGC", new CheckBox("AutoE gapclosers"));


            drawMenu = Menu.AddSubMenu("Drawings", "Drawings");
            drawMenu.Add("QRange", new CheckBox("Q range"));
            drawMenu.Add("WRange", new CheckBox("W range"));
            drawMenu.Add("ERange", new CheckBox("E range"));
            drawMenu.Add("RRange", new CheckBox("R range"));
            drawMenu.Add("RangeM", new CheckBox("R range (minimap)"));


            //Add the events we are going to use:
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Interrupter.OnPossibleToInterrupt += Interrupter_OnPossibleToInterrupt;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            AIHeroClient.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
            Game.OnWndProc += Game_OnWndProc;
            Chat.Print(ChampionName + " Loaded!");
            Orbwalker.OnPreAttack += Orbwalking_BeforeAttack;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;

        }


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

        public static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (IsCastingR && getCheckBoxItem(rMenu, "BlockMovement") && sender.Owner.IsMe)
            {
                args.Process = false;
                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
            }
            else
            {
                args.Process = true;
                Orbwalker.DisableAttacking = false;
                Orbwalker.DisableMovement = false;
            }
        }

        private static void Orbwalking_BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            args.Process = AttacksEnabled;
        }

        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!getCheckBoxItem(miscMenu, "AutoEGC")) return;

            if (Player.Distance(gapcloser.Sender, false) < E.Range)
            {
                E.Cast(gapcloser.Sender);
            }
        }

        static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg == (uint)WindowsMessages.WM_KEYUP)
                RCharge.TapKeyPressed = true;
        }

        static void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "XerathLocusOfPower2")
                {
                    RCharge.CastT = 0;
                    RCharge.Index = 0;
                    RCharge.Position = new Vector3();
                    RCharge.TapKeyPressed = false;
                }
                else if (args.SData.Name == "xerathlocuspulse")
                {
                    RCharge.CastT = Environment.TickCount;
                    RCharge.Index++;
                    RCharge.Position = args.End;
                    RCharge.TapKeyPressed = false;
                }
            }
        }

        private static void Interrupter_OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (!getCheckBoxItem(miscMenu, "InterruptSpells")) return;

            if (Player.Distance(unit, false) < E.Range)
            {
                E.Cast(unit);
            }
        }

        private static void Combo()
        {

            UseSpells(getCheckBoxItem(comboMenu, "UseQCombo"), getCheckBoxItem(comboMenu, "UseWCombo"),
                getCheckBoxItem(comboMenu, "UseECombo"));
        }

        private static void Harass()
        {
            UseSpells(getCheckBoxItem(harassMenu, "UseQHarass"), getCheckBoxItem(harassMenu, "UseWHarass"),
                false);
        }

        private static void UseSpells(bool useQ, bool useW, bool useE)
        {
            var qTarget = TargetSelector.GetTarget(Q.ChargedMaxRange, DamageType.Magical);
            var wTarget = TargetSelector.GetTarget(W.Range + W.Width * 0.5f, DamageType.Magical);
            var eTarget = TargetSelector.GetTarget(E.Range, DamageType.Magical);

            if (eTarget != null && useE && E.IsReady())
            {
                if (Player.Distance(eTarget, false) < E.Range * 0.4f)
                {
                    if (E.GetPrediction(eTarget).Hitchance >= HitChance.VeryHigh)
                        E.Cast(eTarget);
                }
                else if ((!useW || !W.IsReady()))
                {
                    if (E.GetPrediction(eTarget).Hitchance >= HitChance.VeryHigh)
                        E.Cast(eTarget);
                }
            }

            if (useQ && Q.IsReady())
            {
                if (Q.IsCharging)
                {
                    float distance = Player.Distance(qTarget, false) + 20;

                    if (distance > Q.ChargedMaxRange)
                        distance = Q.ChargedMaxRange;

                    if (Q.Range >= distance && Q.GetPrediction(qTarget).Hitchance >= HitChance.VeryHigh)
                        Q.Cast(qTarget, false, false);
                }
                else if (qTarget != null && (!useW || !W.IsReady() || Player.Distance(qTarget, false) > W.Range))
                {
                    Q.StartCharging();
                }
            }

            if (wTarget != null && useW && W.IsReady() && W.GetPrediction(wTarget).Hitchance >= HitChance.VeryHigh)
                W.Cast(wTarget, false, true);
        }

        private static AIHeroClient GetTargetNearMouse(float distance)
        {
            AIHeroClient bestTarget = null;
            var bestRatio = 0f;

            if (TargetSelector.SelectedTarget.IsValidTarget() && !TargetSelector.SelectedTarget.IsInvulnerable &&
                (Game.CursorPos.Distance(TargetSelector.SelectedTarget.ServerPosition) < distance && ObjectManager.Player.Distance(TargetSelector.SelectedTarget, false) < R.Range))
            {
                return TargetSelector.SelectedTarget;
            }

            foreach (var hero in ObjectManager.Get<AIHeroClient>())
            {
                if (!hero.IsValidTarget(R.Range) || !hero.IsInvulnerable || Game.CursorPos.Distance(hero.ServerPosition) > distance)
                {
                    continue;
                }

                var damage = (float)ObjectManager.Player.CalcDamage(hero, DamageType.Magical, 100);
                var ratio = damage / (1 + hero.Health) * TargetSelector.GetPriority(hero);

                if (ratio > bestRatio)
                {
                    bestRatio = ratio;
                    bestTarget = hero;
                }
            }

            return bestTarget;
        }

        private static void WhileCastingR()
        {
            if (!getCheckBoxItem(rMenu, "EnableRUsage")) return;
            var rMode = getBoxItem(rMenu, "rMode");

            var rTarget = getCheckBoxItem(rMenu, "OnlyNearMouse")  ? GetTargetNearMouse(getSliderItem(rMenu, "MRadius")) : TargetSelector.GetTarget(R.Range, DamageType.Magical);

            if (rTarget != null)
            {
                //Wait at least 0.6f if the target is going to die or if the target is to far away
                if (rTarget.Health - R.GetDamage(rTarget) < 0)
                    if (Environment.TickCount - RCharge.CastT <= 700) return;

                if ((RCharge.Index != 0 && rTarget.Distance(RCharge.Position) > 1000))
                    if (Environment.TickCount - RCharge.CastT <= Math.Min(2500, rTarget.Distance(RCharge.Position) - 1000)) return;

                switch (rMode)
                {
                    case 0://Normal
                        R.Cast(rTarget, true);
                        break;

                    case 1://Selected delays.
                        var delay = getSliderItem(rMenu, "Delay" + (RCharge.Index + 1));
                        if (Environment.TickCount - RCharge.CastT > delay)
                            R.Cast(rTarget, true);
                        break;

                    case 2://On tap
                        if (RCharge.TapKeyPressed)
                            R.Cast(rTarget, true);
                        break;

                    case 4://Hitchance Veryhigh[added by xcsoft]
                        if (R.GetPrediction(rTarget).Hitchance >= HitChance.High)
                            R.Cast(rTarget, true);
                        break;
                }
            }
        }

        private static void Farm(bool laneClear)
        {
            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.ChargedMaxRange,
                MinionTypes.All);
            var rangedMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range + W.Width + 30,
                MinionTypes.Ranged);

            var useQi = getBoxItem(clearMenu, "UseQFarm"); 
            var useWi = getBoxItem(clearMenu, "UseWFarm"); 
            var useQ = (laneClear && (useQi == 1 || useQi == 2)) || (!laneClear && (useQi == 0 || useQi == 2));
            var useW = (laneClear && (useWi == 1 || useWi == 2)) || (!laneClear && (useWi == 0 || useWi == 2));

            if (useW && W.IsReady())
            {
                var locW = W.GetCircularFarmLocation(rangedMinionsW, W.Width * 0.75f);
                if (locW.MinionsHit >= 3 && W.IsInRange(locW.Position.To3D()))
                {
                    W.Cast(locW.Position);
                    return;
                }
                else
                {
                    var locW2 = W.GetCircularFarmLocation(allMinionsQ, W.Width * 0.75f);
                    if (locW2.MinionsHit >= 1 && W.IsInRange(locW.Position.To3D()))
                    {
                        W.Cast(locW.Position);
                        return;
                    }

                }
            }

            if (useQ && Q.IsReady())
            {
                if (Q.IsCharging)
                {
                    var locQ = Q.GetLineFarmLocation(allMinionsQ);
                    if (allMinionsQ.Count == allMinionsQ.Count(m => Player.Distance(m) < Q.Range) && locQ.MinionsHit > 0 && locQ.Position.IsValid())
                        Q.Cast(locQ.Position);
                }
                else if (allMinionsQ.Count > 0)
                    Q.StartCharging();
            }
        }

        private static void JungleFarm()
        {
            var useQ = getCheckBoxItem(jungleMenu, "UseQJFarm");
            var useW = getCheckBoxItem(jungleMenu, "UseWJFarm");
            var mobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range, MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (mobs.Count > 0)
            {
                var mob = mobs[0];
                if (useW && W.IsReady())
                {
                    W.Cast(mob);
                }
                else if (useQ && Q.IsReady())
                {
                    if (!Q.IsCharging)
                        Q.StartCharging();
                    else
                        Q.Cast(mob);
                }
            }
        }

        private static void Ping(Vector2 position)
        {
            if (Environment.TickCount - LastPingT < 30 * 1000) return;
            LastPingT = Environment.TickCount;
            PingLocation = position;
            SimplePing();
            Utility.DelayAction.Add(150, SimplePing);
            Utility.DelayAction.Add(300, SimplePing);
            Utility.DelayAction.Add(400, SimplePing);
            Utility.DelayAction.Add(800, SimplePing);
        }

        private static void SimplePing()
        {
            //Packet.S2C.Ping.Encoded(new Packet.S2C.Ping.Struct(PingLocation.X, PingLocation.Y, 0, 0, Packet.PingType.Fallback)).Process();
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead) return;

            Orbwalker.DisableMovement = true;

            //Update the R range
            R.Range = 2000 + R.Level * 1200;

            if (IsCastingR)
            {
                Orbwalker.DisableMovement = false;
                WhileCastingR();
                return;
            }

            if (R.IsReady() && getCheckBoxItem(rMenu, "PingRKillable"))
            {
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(h => h.IsValidTarget() && (float)Player.GetSpellDamage(h, SpellSlot.R) * 3 > h.Health))
                {
                    Ping(enemy.Position.To2D());
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            else
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) ||
                    getKeyBindItem(harassMenu, "HarassActiveT"))
                    Harass();

                var lc = Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear);
                if (lc || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                    Farm(lc);

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                    JungleFarm();
            }
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (R.Level == 0) return;
            var menuItem = getCheckBoxItem(drawMenu, R.Slot + "RangeM");
            if (menuItem)
                Utility.DrawCircle(Player.Position, R.Range, Color.Red, 2, 30, true);
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (IsCastingR)
            {
                if (getCheckBoxItem(rMenu, "OnlyNearMouse"))
                {
                    Utility.DrawCircle(Game.CursorPos, getSliderItem(rMenu, "MRadius"), Color.White);
                }
            }

            //Draw the ranges of the spells.
            foreach (var spell in SpellList)
            {
                var menuItem = getCheckBoxItem(drawMenu, spell.Slot + "Range");
                if (menuItem && (spell.Slot != SpellSlot.R || R.Level > 0))
                    Utility.DrawCircle(Player.Position, spell.Range, Color.Red);
            }
        }
    }
}
