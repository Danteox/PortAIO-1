using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;
using System.Drawing;
using ItemData = LeagueSharp.Common.Data.ItemData;
using Spell = LeagueSharp.Common.Spell;
using EloBuddy.SDK.Notifications;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;

namespace FastTrundle
{
    internal enum Spells
    {
        Q,
        W,
        E,
        R
    }

    internal static class Trundle
    {
        #region Data


        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
                                                             {
                                                                 { Spells.Q, new Spell(SpellSlot.Q, 550f) },
                                                                 { Spells.W, new Spell(SpellSlot.W, 900f) },
                                                                 { Spells.E, new Spell(SpellSlot.E, 1000f) },
                                                                 { Spells.R, new Spell(SpellSlot.R, 700f) }
                                                             };

        private static SpellSlot ignite;

        private static Vector3 pillarPosition;

        private static bool allowQAfterAA, allowItemsAfterAA;

        public static string ScriptVersion => typeof(Trundle).Assembly.GetName().Version.ToString();

        private static AIHeroClient Player => ObjectManager.Player;

        #endregion

        #region Methods

        #region Event handlers

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Trundle") return;

            spells[Spells.E].SetSkillshot(0.5f, 188f, 1600f, false, SkillshotType.SkillshotCircle);
            ignite = Player.GetSpellSlot("summonerdot");

            FastTrundleMenu.Initialize();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Obj_AI_Base.OnSpellCast += ObjAIBase_OnDoCast;
        }

        private static void ObjAIBase_OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || !LeagueSharp.Common.Orbwalking.IsAutoAttack(args.SData.Name)) return;
            if (args.Target == null || !args.Target.IsValid) return;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                if (Orbwalker.LastTarget == null) return;

                if (allowQAfterAA && !(args.Target is Obj_AI_Turret || args.Target is Obj_Barracks || args.Target is Obj_BarracksDampener || args.Target is Obj_Building) && spells[Spells.Q].IsReady())
                {
                    spells[Spells.Q].Cast();
                    SebbyLib.Orbwalking.ResetAutoAttackTimer();
                    return;
                }
                if (allowItemsAfterAA && FastTrundleMenu.getCheckBoxItem(FastTrundleMenu.itemMenu, "FastTrundle.Items.Titanic") && Items.HasItem(3748) && Items.CanUseItem(3748)) // Titanic
                {
                    Items.UseItem(3748);
                    Orbwalker.ResetAutoAttack();
                    return;
                }
                if (allowItemsAfterAA && FastTrundleMenu.getCheckBoxItem(FastTrundleMenu.itemMenu, "FastTrundle.Items.Hydra") && Items.HasItem(3077) && Items.CanUseItem(3077))
                {
                    Items.UseItem(3077);
                    Orbwalker.ResetAutoAttack();
                    return;
                }
                if (allowItemsAfterAA && FastTrundleMenu.getCheckBoxItem(FastTrundleMenu.itemMenu, "FastTrundle.Items.Hydra") && Items.HasItem(3074) && Items.CanUseItem(3074))
                {
                    Items.UseItem(3074);
                    Orbwalker.ResetAutoAttack();
                    return;
                }
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.Distance(Player) > spells[Spells.E].Range || !FastTrundleMenu.getCheckBoxItem(FastTrundleMenu.miscMenu, "FastTrundle.Antigapcloser"))
            {
                return;
            }

            if (gapcloser.Sender.IsValidTarget(spells[Spells.E].Range))
            {
                if (spells[Spells.E].IsReady())
                {
                    spells[Spells.E].Cast(gapcloser.Sender);
                }
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!FastTrundleMenu.getCheckBoxItem(FastTrundleMenu.miscMenu, "FastTrundle.Interrupter"))
            {
                return;
            }

            if (args.DangerLevel != Interrupter2.DangerLevel.High || sender.Distance(Player) > spells[Spells.E].Range)
            {
                return;
            }

            if (spells[Spells.E].CanCast(sender) && args.DangerLevel >= Interrupter2.DangerLevel.High)
            {
                spells[Spells.E].Cast(sender.Position);
            }
        }

        private static void OnDraw(EventArgs args)
        {
            var newTarget = TargetSelector.GetTarget(spells[Spells.E].Range + 200, DamageType.Physical);
            var drawQ = FastTrundleMenu.getCheckBoxItem(FastTrundleMenu.miscMenu, "FastTrundle.Draw.Q");
            var drawW = FastTrundleMenu.getCheckBoxItem(FastTrundleMenu.miscMenu, "FastTrundle.Draw.W");
            var drawE = FastTrundleMenu.getCheckBoxItem(FastTrundleMenu.miscMenu, "FastTrundle.Draw.E");
            var drawR = FastTrundleMenu.getCheckBoxItem(FastTrundleMenu.miscMenu, "FastTrundle.Draw.R"); 
            var drawPillar = FastTrundleMenu.getCheckBoxItem(FastTrundleMenu.miscMenu, "FastTrundle.Draw.Pillar");


            if (drawQ)
               {                       
                Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.Q].Range, System.Drawing.Color.Blue);
               }
          
            if (drawW)      
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.W].Range, System.Drawing.Color.Blue);
                }
            
            if (drawE)

                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.E].Range, System.Drawing.Color.Blue);
                }
            

            if (drawR)

                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.R].Range, System.Drawing.Color.Blue);
                }
            

            if (drawPillar)
            {
                if (newTarget != null
                    && newTarget.IsVisible
                    && newTarget.IsValidTarget()
                    && !newTarget.IsDead
                    && Player.Distance(newTarget) < 3000)
                {
                    Drawing.DrawCircle(GetPillarPosition(newTarget), 188, System.Drawing.Color.Blue);
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            allowQAfterAA = allowItemsAfterAA = false;
            if (Player.IsDead || Player.IsRecalling() || MenuGUI.IsChatOpen) return;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LastHit();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear();
            }
        }

        #endregion

        #region Orbwalking Modes

        private static void JungleClear()
        {
            allowItemsAfterAA = true;

            var minion =
                MinionManager.GetMinions(
                    Player.ServerPosition,
                    spells[Spells.W].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (minion == null) return;

            if (Player.ManaPercent < FastTrundleMenu.getSliderItem(FastTrundleMenu.jungleMenu, "FastTrundle.JungleClear.Mana"))
                return;

            if (FastTrundleMenu.getCheckBoxItem(FastTrundleMenu.jungleMenu, "FastTrundle.JungleClear.Q") && spells[Spells.Q].IsReady()
                && minion.IsValidTarget(spells[Spells.Q].Range))
            {
                allowQAfterAA = true;
            }

            if (FastTrundleMenu.getCheckBoxItem(FastTrundleMenu.jungleMenu, "FastTrundle.JungleClear.W") && spells[Spells.W].IsReady()
                && minion.IsValidTarget(700))
            {
                spells[Spells.W].Cast(minion.Position);
            }
        }

        private static void LaneClear()
        {
            allowItemsAfterAA = true;

            var minion = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.W].Range).FirstOrDefault();
            if (minion == null) return;

            if (Player.ManaPercent < FastTrundleMenu.getSliderItem(FastTrundleMenu.clearMenu, "FastTrundle.LaneClear.Mana"))
                return;

            if (FastTrundleMenu.getCheckBoxItem(FastTrundleMenu.clearMenu, "FastTrundle.LaneClear.Q")
                && spells[Spells.Q].IsReady()
                && minion.IsValidTarget(spells[Spells.Q].Range))
            {
                if (FastTrundleMenu.getCheckBoxItem(FastTrundleMenu.clearMenu, "FastTrundle.LaneClear.Q.Lasthit"))
                {
                    if (minion.Health <= QDamage(minion)
                        && (minion.Health > Player.GetAutoAttackDamage(minion) ||
                            (!SebbyLib.Orbwalking.CanAttack()))) // don't overkill with Q unless we need AA reset to get it
                    {
                        spells[Spells.Q].Cast();
                      EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                    }
                }
                else
                    allowQAfterAA = true;
            }

            if (FastTrundleMenu.getCheckBoxItem(FastTrundleMenu.clearMenu, "FastTrundle.LaneClear.W") && spells[Spells.W].IsReady()
                && minion.IsValidTarget(700))
            {
                spells[Spells.W].Cast(minion.Position);
            }
        }

        private static void LastHit()
        {
            var minion = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.W].Range).FirstOrDefault();
            if (minion == null) return;

            if (Player.ManaPercent < FastTrundleMenu.getSliderItem(FastTrundleMenu.lasthitMenu, "FastTrundle.LastHit.Mana")) return;

            if (FastTrundleMenu.getCheckBoxItem(FastTrundleMenu.lasthitMenu, "FastTrundle.LastHit.Q")
                && spells[Spells.Q].IsReady()
                && minion.IsValidTarget(spells[Spells.Q].Range)
                && minion.Health <= QDamage(minion)
                && (minion.Health > Player.GetAutoAttackDamage(minion) ||
                    (!SebbyLib.Orbwalking.CanAttack()))) // don't overkill with Q unless we need AA reset to get it
            {
                spells[Spells.Q].Cast();
               EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
            }
        }

        private static void Combo()
        {
            allowItemsAfterAA = true;

            var target = TargetSelector.GetTarget(spells[Spells.E].Range, DamageType.Physical);
            if (target == null || !target.IsValidTarget()) return;

            if (FastTrundleMenu.getCheckBoxItem(FastTrundleMenu.comboMenu, "FastTrundle.Combo.E") && spells[Spells.E].IsReady()
                && target.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast(GetPillarPosition(target));
            }

            UseItems(target);

            if (FastTrundleMenu.getCheckBoxItem(FastTrundleMenu.comboMenu, "FastTrundle.Combo.W")
                && spells[Spells.W].IsReady()
                && target.IsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast(target.Position);
            }

            if (FastTrundleMenu.getCheckBoxItem(FastTrundleMenu.comboMenu, "FastTrundle.Combo.Q") && target.IsValidTarget(spells[Spells.Q].Range))
            {
                allowQAfterAA = true;
            }

            if (spells[Spells.R].IsReady() && FastTrundleMenu.getCheckBoxItem(FastTrundleMenu.comboMenu, "FastTrundle.Combo.R"))
            {
                foreach (var hero in ObjectManager.Get<AIHeroClient>())
                {
                    if (hero.IsEnemy)
                    {
                        var getEnemies = FastTrundleMenu.getCheckBoxItem(FastTrundleMenu.comboMenu, "FastTrundle.R.On" + hero.ChampionName);
                        if (getEnemies)
                        {
                            spells[Spells.R].Cast(hero);
                        }

                        if (getEnemies && Player.CountEnemiesInRange(1500) == 1)
                        {
                            spells[Spells.R].Cast(hero);
                        }
                    }
                }
            }

            if (Player.Distance(target) <= 600 && IgniteDamage(target) >= target.Health
                && FastTrundleMenu.getCheckBoxItem(FastTrundleMenu.comboMenu, "FastTrundle.Combo.IgniteW"))
            {
                Player.Spellbook.CastSpell(ignite, target);
            }
        }

        private static void Harass()
        {
            allowItemsAfterAA = true;

            var target = TargetSelector.GetTarget(spells[Spells.E].Range, DamageType.Physical);
            if (target == null || !target.IsValidTarget()) return;

            if (Player.ManaPercent < FastTrundleMenu.getSliderItem(FastTrundleMenu.harassMenu, "FastTrundle.Harass.Mana")) return;

            if (FastTrundleMenu.getCheckBoxItem(FastTrundleMenu.harassMenu, "FastTrundle.Harass.Q") && target.IsValidTarget(spells[Spells.Q].Range))
            {
                allowQAfterAA = true;
            }

            if (FastTrundleMenu.getCheckBoxItem(FastTrundleMenu.harassMenu, "FastTrundle.Harass.E") && spells[Spells.E].IsReady()
                && target.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast(GetPillarPosition(target));
            }

            if (FastTrundleMenu.getCheckBoxItem(FastTrundleMenu.harassMenu, "FastTrundle.Harass.W") && spells[Spells.W].IsReady()
                && target.IsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast(target.Position);
            }
        }

        #endregion

        #region Helpers

        private static void UseItems(Obj_AI_Base target)
        {
            if (FastTrundleMenu.getCheckBoxItem(FastTrundleMenu.itemMenu, "FastTrundle.Items.Blade")
                && Player.HealthPercent <= FastTrundleMenu.getSliderItem(FastTrundleMenu.itemMenu, "FastTrundle.Items.Blade.MyHP"))
            {
                if (ItemData.Blade_of_the_Ruined_King.GetItem().IsReady()
                    && ItemData.Blade_of_the_Ruined_King.Range < Player.Distance(target))
                {
                    ItemData.Blade_of_the_Ruined_King.GetItem().Cast(target);
                }

                if (ItemData.Bilgewater_Cutlass.GetItem().IsReady()
                    && ItemData.Bilgewater_Cutlass.Range < Player.Distance(target))
                {
                    ItemData.Bilgewater_Cutlass.GetItem().Cast(target);
                }
            }

            if (FastTrundleMenu.getCheckBoxItem(FastTrundleMenu.itemMenu, "FastTrundle.Items.Youmuu"))
            {
                if (ItemData.Youmuus_Ghostblade.GetItem().IsReady()
                    && LeagueSharp.Common.Orbwalking.GetRealAutoAttackRange(Player) < Player.Distance(target))
                {
                    ItemData.Youmuus_Ghostblade.GetItem().Cast();
                }
            }
        }


 

        private static float IgniteDamage(AIHeroClient target)
        {
            if (ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)Player.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
        }

        private static double QDamage(Obj_AI_Base target)
        {
            return Player.GetAutoAttackDamage(target) + spells[Spells.Q].GetDamage(target);
        }

        private static Vector3 GetPillarPosition(AIHeroClient target)
        {
            pillarPosition = Player.Position;

            return V2E(pillarPosition, target.Position, target.Distance(pillarPosition) + 230).To3D();
        }

        private static Vector2 V2E(Vector3 from, Vector3 direction, float distance)
        {
            return from.To2D() + distance * Vector3.Normalize(direction - from).To2D();
        }

        #endregion

        #endregion
    }
}
