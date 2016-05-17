﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using LeagueSharp.Common;
using SebbyLib;
using Color = System.Drawing.Color;
using EloBuddy.SDK;
using Spell = LeagueSharp.Common.Spell;
using Prediction = LeagueSharp.Common.Prediction;
using Utility = LeagueSharp.Common.Utility;

namespace BadaoKingdom.BadaoChampion.BadaoGangplank
{
    public static class BadaoGangplankCombo
    {
        public static int LastCondition;
        public static int Estack { get{ return BadaoMainVariables.E.Instance.Ammo; } }
        public static AIHeroClient  Player { get { return ObjectManager.Player; } }
        public static void BadaoActivate ()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                return;
            if (Environment.TickCount - LastCondition >= 100 + Game.Ping)
            {
                foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget()))
                {
                    var pred = Prediction.GetPrediction(hero, 0.5f).UnitPosition;
                    if (BadaoMainVariables.Q.IsReady() && BadaoMainVariables.E.IsReady())
                    {
                        foreach (var barrel in BadaoGangplankBarrels.QableBarrels(350))
                        {
                            var nbarrels = BadaoGangplankBarrels.ChainedBarrels(barrel);
                            if (nbarrels.Any(x => x.Bottle.LSDistance(pred) <= 660 /*+ hero.BoundingRadius*/)
                                && !nbarrels.Any(x => x.Bottle.LSDistance(pred) <= 330 /*+ hero.BoundingRadius*/))
                            {
                                var mbarrels = nbarrels.FirstOrDefault(x => x.Bottle.LSDistance(pred) <= 660);
                                var pos = mbarrels.Bottle.Position.Extend(pred, 660);
                                if (Player.Distance(pos) < BadaoMainVariables.E.Range)
                                {
                                    Orbwalker.DisableAttacking = false;
                                    Orbwalker.DisableMovement = false;
                                    Utility.DelayAction.Add(100 + Game.Ping, () =>
                                    {
                                        Orbwalker.DisableAttacking = true;
                                        Orbwalker.DisableMovement = true;
                                    });
                                    BadaoMainVariables.E.Cast(pos);
                                    LastCondition = Environment.TickCount;
                                    return;
                                }
                            }
                        }
                    }
                }
                foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget()))
                {
                    var pred = Prediction.GetPrediction(hero, 0.5f).UnitPosition;
                    if (SebbyLib.Orbwalking.CanAttack() && BadaoMainVariables.E.IsReady())
                    {
                        foreach (var barrel in BadaoGangplankBarrels.AttackableBarrels(350))
                        {
                            var nbarrels = BadaoGangplankBarrels.ChainedBarrels(barrel);
                            if (nbarrels.Any(x => x.Bottle.Distance(pred) <= 660 /*+ hero.BoundingRadius*/)
                                && !nbarrels.Any(x => x.Bottle.Distance(pred) <= 330 /*+ hero.BoundingRadius*/))
                            {
                                Orbwalker.DisableAttacking = false;
                                Orbwalker.DisableMovement = false;

                                Utility.DelayAction.Add(100 + Game.Ping, () =>
                                {
                                    Orbwalker.DisableAttacking = true;
                                    Orbwalker.DisableMovement = true;
                                });
                                var mbarrels = nbarrels.FirstOrDefault(x => x.Bottle.Distance(pred) <= 660);
                                var pos = mbarrels.Bottle.Position.Extend(pred, 660);
                                BadaoMainVariables.E.Cast(pos);
                                LastCondition = Environment.TickCount;
                                return;
                            }
                        }
                    }
                }
                foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget()))
                {
                    var pred = Prediction.GetPrediction(hero, 0.5f).UnitPosition;
                    if (BadaoMainVariables.Q.IsReady())
                    {
                        foreach (var barrel in BadaoGangplankBarrels.QableBarrels())
                        {
                            var nbarrels = BadaoGangplankBarrels.ChainedBarrels(barrel);
                            if (nbarrels.Any(x => x.Bottle.Distance(pred) <= 330 /*+ hero.BoundingRadius*/))
                            {
                                Orbwalker.DisableAttacking = false;
                                Orbwalker.DisableMovement = false;
                                Utility.DelayAction.Add(100 + Game.Ping, () =>
                                {
                                    Orbwalker.DisableAttacking = true;
                                    Orbwalker.DisableMovement = true;
                                });
                                if (BadaoMainVariables.Q.Cast(barrel.Bottle) == Spell.CastStates.SuccessfullyCasted)
                                {
                                    LastCondition = Environment.TickCount;
                                    return;
                                }
                            }
                        }
                    }
                }

                foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget()))
                {
                    var pred = Prediction.GetPrediction(hero, 0.5f).UnitPosition;
                    if (SebbyLib.Orbwalking.CanAttack())
                    {
                        foreach (var barrel in BadaoGangplankBarrels.AttackableBarrels())
                        {
                            var nbarrels = BadaoGangplankBarrels.ChainedBarrels(barrel);
                            if (nbarrels.Any(x => x.Bottle.Distance(pred) <= 330 /*+ hero.BoundingRadius*/))
                            {
                                Orbwalker.DisableAttacking = false;
                                Orbwalker.DisableMovement = false;
                                Utility.DelayAction.Add(100 + Game.Ping, () =>
                                {
                                    Orbwalker.DisableAttacking = true;
                                    Orbwalker.DisableMovement = true;
                                });
                                if (EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, barrel.Bottle))
                                {
                                    LastCondition = Environment.TickCount;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            if (Estack >= 2 && BadaoMainVariables.E.IsReady() && BadaoGangplankVariables.ComboE1)
            {
                var target = TargetSelector.GetTarget(BadaoMainVariables.E.Range, DamageType.Physical);
                if( target.BadaoIsValidTarget())
                {
                    var pred = Prediction.GetPrediction(target, 0.5f).UnitPosition;
                    if (!BadaoGangplankBarrels.Barrels.Any(x => x.Bottle.Distance(pred) <= 660 /*+ target.BoundingRadius*/))
                        BadaoMainVariables.E.Cast(pred);
                }
            }
            if (BadaoMainVariables.Q.IsReady())
            {
                var target = TargetSelector.GetTarget(BadaoMainVariables.Q.Range, DamageType.Physical);
                if (target.BadaoIsValidTarget())
                {
                    bool useQ = true;
                    foreach (var barrel in BadaoGangplankBarrels.DelayedBarrels(1000))
                    {
                        var nbarrels = BadaoGangplankBarrels.ChainedBarrels(barrel);
                        if (BadaoMainVariables.E.IsReady()
                            && nbarrels.Any(x => x.Bottle.Distance(target.Position) <= 660 + target.BoundingRadius)
                            && !nbarrels.Any(x => x.Bottle.Distance(target.Position) <= 330 + target.BoundingRadius))
                        {
                            useQ = false;
                        }
                        else if (nbarrels.Any(x => x.Bottle.Distance(target.Position) <= 330 + target.BoundingRadius))
                        {
                            useQ = false;
                        }
                    }
                    if (useQ)
                    {
                        BadaoMainVariables.Q.Cast(target);
                    }
                }
            }
        }
    }
}
