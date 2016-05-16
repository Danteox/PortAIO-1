using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Menu;
using Spell = LeagueSharp.Common.Spell;
using EloBuddy.SDK.Menu.Values;

namespace NoobJaxReloaded
{
    class Program
    {
        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }
        private static readonly AIHeroClient[] AllEnemy = HeroManager.Enemies.ToArray();

        private static Spell Q, W, E, R;

        private static Items.Item tiamat, hydra, cutlass, botrk, hextech;
        private static bool IsEUsed => Player.HasBuff("JaxCounterStrike");

        private static bool IsWUsed => Player.HasBuff("JaxEmpowerTwo");

        private static Menu Menu;
        public static Menu comboMenu, harassMenu, jungleMenu, laneMenu, ksMenu, drawMenu, miscMenu;


        /// <summary>
        /// Game Loaded Method
        /// </summary>
        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Jax") // check if the current champion is Jax
                return; // stop programm

            // Set Spells
            Q = new Spell(SpellSlot.Q, 700);
            W = new Spell(SpellSlot.W, Orbwalking.GetRealAutoAttackRange(Player));
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);

            // Create Items
            hydra = new Items.Item(3074, 185);
            tiamat = new Items.Item(3077, 185);
            cutlass = new Items.Item(3144, 450);
            botrk = new Items.Item(3153, 450);
            hextech = new Items.Item(3146, 700);


            Menu = MainMenu.AddMenu("Noob Jax", "Jax");

            comboMenu = Menu.AddSubMenu("Combo", "Combo");
            comboMenu.Add("useQ", new CheckBox("Use Q"));
            comboMenu.Add("useQ2", new CheckBox("Use Q when enemy is in AA Range"));
            comboMenu.Add("useW", new CheckBox("Use W"));
            comboMenu.Add("useR", new CheckBox("Use R"));
            comboMenu.AddLabel("E options");
            comboMenu.Add("useE", new CheckBox("Use E"));
            comboMenu.Add("useE2", new CheckBox("Use second E"));

            harassMenu = Menu.AddSubMenu("Harass", "Harass");
            harassMenu.Add("harassW", new CheckBox("Use W to Cancel AA to Harass"));

            laneMenu = Menu.AddSubMenu("Laneclear", "Laneclear");
            laneMenu.Add("laneclearW", new CheckBox("Use W to LaneClear"));

            jungleMenu = Menu.AddSubMenu("Jungleclear", "Jungleclear");
            jungleMenu.Add("jungleclearQ", new CheckBox("Use Q to JungleClear"));
            jungleMenu.Add("jungleclearW", new CheckBox("Use W to JungleClear"));
            jungleMenu.Add("jungleclearE", new CheckBox("Use E to JungleClear"));

            ksMenu = Menu.AddSubMenu("Killsteal", "Killsteal");
            ksMenu.Add("Killsteal", new CheckBox("Killsteal with Q"));

            drawMenu = Menu.AddSubMenu("Drawings", "Drawings");
            drawMenu.Add("drawsetQ", new CheckBox("Draw set Q range"));
            drawMenu.Add("drawAa", new CheckBox("Draw Autoattack range"));

            miscMenu = Menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("usejump", new CheckBox("Use Wardjump"));
            miscMenu.Add("jumpkey", new KeyBind("Wardjump Key", false, KeyBind.BindTypes.HoldActive, 'T'));

            OnDoCast();
            Orbwalker.OnAttack += OnAa;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;

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

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling())
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (getCheckBoxItem(ksMenu, "Killsteal"))
            {
                Killsteal();
            }
            if (getKeyBindItem(miscMenu, "jumpkey")  && getCheckBoxItem(miscMenu, "usejump"))
            {
                WardJump();
            }

        }

        private static void Combo(bool anyTarget = false)
        {
            AIHeroClient target = TargetSelector.GetTarget(700, DamageType.Magical);

            // IITEMS
            if (target != null && Player.Distance(target) <= botrk.Range)
            {
                botrk.Cast(target);
            }
            if (target != null && Player.Distance(target) <= cutlass.Range)
            {
                cutlass.Cast(target);
            }
            if (target != null && Player.Distance(target) <= hextech.Range)
            {
                hextech.Cast(target);
            }


            // ACTUAL COMBO
            if (target != null && !target.IsZombie)
            {
                if (Q.IsReady() && getCheckBoxItem(comboMenu, "useQ"))
                {
                    if ((Player.Distance(target.Position) > Orbwalking.GetRealAutoAttackRange(Player)) || getCheckBoxItem(comboMenu, "useQ2"))
                    {
                        Q.CastOnUnit(target);
                    }
                }
                if (E.IsReady() && (getCheckBoxItem(comboMenu, "useE")))
                {
                    if ((!IsEUsed && Q.IsReady() && target.IsValidTarget(Q.Range)) || (!IsEUsed && Player.Distance(target.Position) < 250))
                    {
                        E.Cast();
                    }
                    if (getCheckBoxItem(comboMenu, "useE2") && IsEUsed && (Player.Distance(target.Position) < 180))
                    {
                        E.Cast();
                    }
                    /*if (anyTarget)
                    {
                        List<Obj_AI_Hero> enemies = Player.Position.GetEnemiesInRange(180);
                        if (enemies.Count >= 3)
                        {
                            E.Cast();
                            return;
                        }
                        if (enemies.Count == 1)
                        {
                            target = enemies.ElementAt(0);
                        }
                    }*/
                }
                if (target.HealthPercent > 20)
                {
                    if ((getCheckBoxItem(comboMenu, "useR") && Q.IsReady() && R.IsReady()) ||
                        (getCheckBoxItem(comboMenu, "useR") && R.IsReady() && !Q.IsReady() &&
                         Player.Distance(target.Position) < 300))
                        R.Cast();
                }
            }
        }

        private static void OnDoCast()
        {
            Obj_AI_Base.OnSpellCast += (sender, args) =>
            {
                //if (!sender.IsMe || !Orbwalking.IsAutoAttack((args.SData.Name))) return;
                if (sender.IsMe && args.SData.IsAutoAttack())
                {
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                    {
                        if (getCheckBoxItem(comboMenu, "useW") && W.IsReady()) W.Cast();
                    }

                    // Jungleclear 
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                    {
                        if (args.Target is Obj_AI_Minion)
                        {
                            var allJungleMinions = MinionManager.GetMinions(Q.Range, MinionTypes.All,
                        MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                            if (allJungleMinions.Count != 0)
                            {
                                if (getCheckBoxItem(jungleMenu, "jungleclearQ") && Q.IsReady())
                                {
                                    foreach (var minion in allJungleMinions)
                                    {
                                        if (minion.IsValidTarget())
                                        {
                                            Q.CastOnUnit(minion);
                                        }
                                    }
                                }
                                if (getCheckBoxItem(jungleMenu, "jungleclearW") && W.IsReady())
                                {
                                    foreach (var minion in allJungleMinions)
                                    {
                                        if (minion.IsValidTarget())
                                        {
                                            W.Cast(minion);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Laneclear
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                    {
                        if (args.Target is Obj_AI_Minion)
                        {
                            var allLaneMinions = MinionManager.GetMinions(Q.Range);
                            //Lane
                            if (getCheckBoxItem(laneMenu, "laneclearW")  && W.IsReady())
                            {
                                foreach (var minion in allLaneMinions)
                                {
                                    if (minion.IsValidTarget())
                                    {
                                        W.Cast(minion);
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.Target != null && args.Target.IsMe && args.SData.IsAutoAttack() && getCheckBoxItem(jungleMenu, "jungleclearE") && E.IsReady() && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) && sender.Team == GameObjectTeam.Neutral)
            {
                E.Cast();
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "drawsetQ"))
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range,
                    Color.Tan);
            }
            if (getCheckBoxItem(drawMenu, "drawAa"))
            {
                Render.Circle.DrawCircle(Player.Position, Orbwalking.GetRealAutoAttackRange(Player),
                    Color.Blue);
            }
        }
        static int CanKill(AIHeroClient target, bool useq)
        {
            double damage = 0;
            if (!useq)
                return 0;
            if (Q.IsReady())
            {
                damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q);
            }
            if (damage >= target.Health)
            {
                return 1;
            }
            return damage >= target.Health ? 2 : 0;

        }
        private static void Killsteal()
        {
            foreach (AIHeroClient enemy in AllEnemy)
            {
                if (enemy == null || enemy.HasBuffOfType(BuffType.Invulnerability))
                    return;

                if (CanKill(enemy, getCheckBoxItem(comboMenu, "useQ")) == 1 && enemy.IsValidTarget(390))
                {
                    Q.Cast(enemy);
                    return;
                }
            }
        }

        public static void OnAa(AttackableUnit target, EventArgs args)
        {
            AIHeroClient y = TargetSelector.GetTarget(185, DamageType.Physical);
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (hydra.IsOwned() && Player.Distance(y) < hydra.Range && hydra.IsReady() && !W.IsReady()
                    && !IsWUsed)
                    hydra.Cast();
                if (tiamat.IsOwned() && Player.Distance(y) < tiamat.Range && tiamat.IsReady() && !W.IsReady()
                    && !IsWUsed)
                    tiamat.Cast();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                if (hydra.IsOwned() && Player.Distance(y) < hydra.Range && hydra.IsReady() && !W.IsReady()
                        && !IsWUsed)
                    hydra.Cast();
                if (tiamat.IsOwned() && Player.Distance(y) < tiamat.Range && tiamat.IsReady() && !W.IsReady()
                        && !IsWUsed)
                    tiamat.Cast();
            }
        }

        public static void WardJump()
        {
           EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (!Q.IsReady())
            {
                return;
            }
            Vector3 wardJumpPosition = (Player.Position.Distance(Game.CursorPos) < 600) ? Game.CursorPos : Player.Position.LSExtend(Game.CursorPos, 600);
            var lstGameObjects = ObjectManager.Get<Obj_AI_Base>().ToArray();
            Obj_AI_Base entityToWardJump = lstGameObjects.FirstOrDefault(obj =>
                obj.Position.Distance(wardJumpPosition) < 150
                && (obj is Obj_AI_Minion || obj is AIHeroClient)
                && !obj.IsMe && !obj.IsDead
                && obj.Position.Distance(Player.Position) < Q.Range);

            if (entityToWardJump != null)
            {
                Q.Cast(entityToWardJump);
            }
            else
            {
                int wardId = GetWardItem();


                if (wardId != -1 && !wardJumpPosition.IsWall())
                {
                    PutWard(wardJumpPosition.LSTo2D(), (ItemId)wardId);
                    lstGameObjects = ObjectManager.Get<Obj_AI_Base>().ToArray();
                    Q.Cast(
                        lstGameObjects.FirstOrDefault(obj =>
                        obj.Position.Distance(wardJumpPosition) < 150 &&
                        obj is Obj_AI_Minion && obj.Position.Distance(Player.Position) < Q.Range));
                }
            }
        }
        public static int GetWardItem()
        {
            int[] wardItems = { 3340, 3350, 3205, 3207, 2049, 2045, 2044, 3361, 3154, 3362, 3160, 2043 };
            foreach (var id in wardItems.Where(id => Items.HasItem(id) && Items.CanUseItem(id)))
                return id;
            return -1;
        }
        public static void PutWard(Vector2 pos, ItemId warditem)
        {

            foreach (var slot in Player.InventoryItems.Where(slot => slot.Id == warditem))
            {
                ObjectManager.Player.Spellbook.CastSpell(slot.SpellSlot, pos.To3D());
                return;
            }
        }
    }
}

