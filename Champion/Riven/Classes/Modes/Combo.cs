using System;
using LeagueSharp.Common;
using EloBuddy.SDK;
using EloBuddy;

namespace NechritoRiven
{
    class Combo
    {
        private static void Game_OnUpdate(EventArgs args)
        {
            ComboLogic();
        }
        public static void ComboLogic()
        {
            {
                var catchRange = Spells._e.IsReady() ? Spells._e.Range + Program.truerange + 200 : Program.truerange + 200;
                var targetR = TargetSelector.GetTarget(250 + Program.Player.AttackRange + 70, DamageType.Physical);
                if (Spells._r.IsReady() && Spells._r.Instance.Name == Program.IsFirstR && MenuConfig.AlwaysR &&
                    targetR != null) Logic.ForceR();

                if (Spells._w.IsReady() && Logic.InWRange(targetR) && targetR != null) Spells._w.Cast();
                if (Spells._r.IsReady() && Spells._r.Instance.Name == Program.IsFirstR && Spells._w.IsReady() && targetR != null &&
                    Spells._e.IsReady() &&
                    targetR.IsValidTarget() && !targetR.IsZombie && (Dmg.IsKillableR(targetR) || MenuConfig.AlwaysR))
                {
                    if (!Logic.InWRange(targetR))
                    {
                        Spells._e.Cast(targetR.Position);
                        Logic.ForceR();
                        LeagueSharp.Common.Utility.DelayAction.Add(200, Logic.ForceW);
                        LeagueSharp.Common.Utility.DelayAction.Add(30, () => Logic.ForceCastQ(targetR));
                    }
                }

                else if (Spells._w.IsReady() && Spells._e.IsReady())
                {
                    if (targetR.IsValidTarget() && targetR != null && !targetR.IsZombie && !Logic.InWRange(targetR))
                    {
                        Spells._e.Cast(targetR.Position);
                        if (Logic.InWRange(targetR))
                            LeagueSharp.Common.Utility.DelayAction.Add(100, Logic.ForceW);
                        LeagueSharp.Common.Utility.DelayAction.Add(30, () => Logic.ForceCastQ(targetR));
                    }
                }

                else if (Spells._q.IsReady() && targetR.Distance(Program.Player.ServerPosition) > catchRange)
                {
                    if (MenuConfig.UseGap)
                    {
                        if (Utils.GameTimeTickCount - Logic._lastQ >= MenuConfig.GapTime * 10)
                        {
                            if (Spells._q.IsReady() && Utils.GameTimeTickCount - Logic.laste >= 600)
                            {
                                Spells._q.Cast(targetR.ServerPosition);
                            }
                        }
                        else if (Spells._e.IsReady())
                        {
                            if (targetR != null && (targetR.IsValidTarget() && !targetR.IsZombie && !Logic.InWRange(targetR)))
                            {
                                Spells._e.Cast(targetR.Position);
                            }
                        }
                    }
                }
            }
        }
    }
}
