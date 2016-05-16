namespace Valvrave_Sharp.Evade
{
    #region

    using LeagueSharp;

    #endregion
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Menu;
    internal enum SpellValidTargets
    {
        AllyMinions,

        EnemyMinions,

        AllyWards,

        EnemyWards,

        AllyChampions,

        EnemyChampions
    }

    internal class EvadeSpellData
    {
        #region Fields

        internal string CheckBuffName = "";

        internal string CheckSpellName = "";

        internal int Delay;

        internal bool ExtraDelay;

        internal bool FixedRange;

        internal bool IsBlink;

        internal bool IsDash;

        internal bool IsInvulnerability;

        internal bool IsMovementSpeedBuff;

        internal bool IsShield;

        internal bool IsSpellShield;

        internal MoveSpeedAmount MoveSpeedTotalAmount;

        internal string Name;

        internal float Range;

        internal bool SelfCast;

        internal SpellSlot Slot;

        internal int Speed;

        internal bool UnderTower;

        internal SpellValidTargets[] ValidTargets;

        private int dangerLevel;

        public static bool getCheckBoxItem(string item)
        {
            return Config.evadeMenu[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return Config.evadeMenu[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(string item)
        {
            return Config.evadeMenu[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(string item)
        {
            return Config.evadeMenu[item].Cast<ComboBox>().CurrentValue;
        }

        #endregion

        #region Delegates

        internal delegate float MoveSpeedAmount();

        #endregion

        #region Public Properties

        internal int DangerLevel
        {
            get
            {
                if (Config.evadeMenu[this.Name + "DangerLevel"] == null)
                {
                    return this.dangerLevel;
                }
                 return getSliderItem(this.Name + "DangerLevel");
            }
            set
            {
                this.dangerLevel = value;
            }
        }

        internal bool Enable => getCheckBoxItem(this.Name + "Enabled");

        internal bool IsReady
            =>
                (this.CheckSpellName == ""
                 || Program.Player.Spellbook.GetSpell(this.Slot).SData.Name.ToLower() == this.CheckSpellName)
                && Program.Player.Spellbook.CanUseSpell(this.Slot) == SpellState.Ready;

        public bool IsTargetted => this.ValidTargets != null;

        #endregion
    }

    internal class DashData : EvadeSpellData
    {
        #region Constructors and Destructors

        public DashData(
            string name,
            SpellSlot slot,
            float range,
            bool fixedRange,
            int delay,
            int speed,
            int dangerLevel)
        {
            this.Name = name;
            this.Range = range;
            this.Slot = slot;
            this.FixedRange = fixedRange;
            this.Delay = delay;
            this.Speed = speed;
            this.DangerLevel = dangerLevel;
            this.IsDash = true;
        }

        #endregion
    }

    internal class BlinkData : EvadeSpellData
    {
        #region Constructors and Destructors

        public BlinkData(string name, SpellSlot slot, float range, int delay, int dangerLevel)
        {
            this.Name = name;
            this.Range = range;
            this.Slot = slot;
            this.Delay = delay;
            this.DangerLevel = dangerLevel;
            this.IsBlink = true;
        }

        #endregion
    }

    internal class InvulnerabilityData : EvadeSpellData
    {
        #region Constructors and Destructors

        public InvulnerabilityData(string name, SpellSlot slot, int delay, int dangerLevel)
        {
            this.Name = name;
            this.Slot = slot;
            this.Delay = delay;
            this.DangerLevel = dangerLevel;
            this.IsInvulnerability = true;
        }

        #endregion
    }

    internal class ShieldData : EvadeSpellData
    {
        #region Constructors and Destructors

        public ShieldData(string name, SpellSlot slot, int delay, int dangerLevel, bool isSpellShield = false)
        {
            this.Name = name;
            this.Slot = slot;
            this.Delay = delay;
            this.DangerLevel = dangerLevel;
            this.IsSpellShield = isSpellShield;
            this.IsShield = !this.IsSpellShield;
        }

        #endregion
    }

    internal class MoveBuffData : EvadeSpellData
    {
        #region Constructors and Destructors

        public MoveBuffData(string name, SpellSlot slot, int delay, int dangerLevel, MoveSpeedAmount amount)
        {
            this.Name = name;
            this.Slot = slot;
            this.Delay = delay;
            this.DangerLevel = dangerLevel;
            this.MoveSpeedTotalAmount = amount;
            this.IsMovementSpeedBuff = true;
        }

        #endregion
    }
}