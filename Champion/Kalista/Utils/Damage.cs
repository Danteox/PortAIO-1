using EloBuddy;
using LeagueSharp.Common;
using EloBuddy.SDK;
using iKalistaReborn.Utils;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;

namespace iKalistaReborn.Utils
{
    public static class Damages
    {
        public static readonly EloBuddy.SDK.Damage.DamageSourceBoundle QDamage = new EloBuddy.SDK.Damage.DamageSourceBoundle();

        private static readonly float[] RawRendDamage = { 20, 30, 40, 50, 60 };
        private static readonly float[] RawRendDamageMultiplier = { 0.6f, 0.6f, 0.6f, 0.6f, 0.6f };
        private static readonly float[] RawRendDamagePerSpear = { 10, 14, 19, 25, 32 };
        private static readonly float[] RawRendDamagePerSpearMultiplier = { 0.2f, 0.225f, 0.25f, 0.275f, 0.3f };

        static Damages()
        {
            QDamage.Add(new EloBuddy.SDK.Damage.DamageSource(SpellSlot.Q, DamageType.Physical)
            {
                Damages = new float[] { 10, 70, 130, 190, 250 }
            });
            QDamage.Add(new EloBuddy.SDK.Damage.BonusDamageSource(SpellSlot.Q, DamageType.Physical)
            {
                DamagePercentages = new float[] { 1, 1, 1, 1, 1 }
            });
        }

        public static bool IsRendKillable(Obj_AI_Base target, float? damage = null)
        {
            // Validate unit
            if (target == null || !target.IsValidTarget() || !target.HasRendBuff())
            {
                return false;
            }

            // Take into account all kinds of shields
            var totalHealth = target.TotalShieldHealth();

            var hero = target as AIHeroClient;
            if (hero != null)
            {
                // Validate that target has no undying buff or spellshield
                if (hero.HasUndyingBuff() || hero.HasSpellShield())
                {
                    return false;
                }

                // Take into account Blitzcranks passive
                if (hero.ChampionName == "Blitzcrank" && !target.HasBuff("BlitzcrankManaBarrierCD") && !target.HasBuff("ManaBarrier"))
                {
                    totalHealth += target.Mana / 2;
                }
            }

            return (damage ?? GetRendDamage(target)) > totalHealth;
        }

        public static float GetRendDamage(AIHeroClient target)
        {
            return GetRendDamage(target, -1);
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }


        public static float GetRendDamage(Obj_AI_Base target, int customStacks = -1, BuffInstance rendBuff = null)
        {
            // Calculate the damage and return
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, GetRawRendDamage(target, customStacks, rendBuff) - getSliderItem(Kalista.miscMenu, "com.ikalista.misc.reduceE")) *
                   (Player.Instance.HasBuff("SummonerExhaustSlow") ? 0.6f : 1); // Take into account Exhaust, migh just add that to the SDK
        }

      public static float GetActualDamage(Obj_AI_Base target)
    {
        if (!SpellManager.Spell[SpellSlot.E].IsReady() || !target.HasRendBuff()) return 0f;

        var damage = GetRendDamage(target);

        if (target.Name.Contains("Baron"))
        {
            // Buff Name: barontarget or barondebuff
            // Baron's Gaze: Baron Nashor takes 50% reduced damage from champions he's damaged in the last 15 seconds. 
            damage = EloBuddy.Player.Instance.HasBuff("barontarget")
                ? damage * 0.5f
                : damage;
        }

        else if (target.Name.Contains("Dragon"))
        {
            // DragonSlayer: Reduces damage dealt by 7% per a stack
            damage = EloBuddy.Player.Instance.HasBuff("s5test_dragonslayerbuff")
                ? damage * (1 - (.07f * EloBuddy.Player.Instance.GetBuffCount("s5test_dragonslayerbuff")))
                : damage;
        }

        if (EloBuddy.Player.Instance.HasBuff("summonerexhaust"))
        {
            damage = damage * 0.6f;
        }

        if (target.HasBuff("FerociousHowl"))
        {
            damage = damage * 0.7f;
        }

        return damage;
    }

    
        public static float GetRawRendDamage(Obj_AI_Base target, int customStacks = -1, BuffInstance rendBuff = null)
        {
            rendBuff = rendBuff ?? target.GetRendBuff();
            var stacks = (customStacks > -1 ? customStacks : rendBuff != null ? rendBuff.Count : 0) - 1;
            if (stacks > -1)
            {
                var index = SpellManager.Spell[SpellSlot.E].Level - 1;
                return RawRendDamage[index] + stacks * RawRendDamagePerSpear[index] +
                       Player.Instance.TotalAttackDamage * (RawRendDamageMultiplier[index] + stacks * RawRendDamagePerSpearMultiplier[index]);
            }

            return 0;
        }
    }
}
