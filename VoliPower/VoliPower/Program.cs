using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;



namespace VoliPower
{
    class Program
    {
        private const string ChampionName = "Volibear";
        public static Obj_AI_Hero Player;
        private static Menu _menu;
        private static Orbwalking.Orbwalker _orbwalker;
        private static Spell _q, _w, _e, _r;
        static Items.Item _botrk, _cutlass;

        public static HpBarIndicator Hpi = new HpBarIndicator();

        //credits Kurisu
        private static readonly int[] SmitePurple = { 3713, 3726, 3725, 3726, 3723 };
        private static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719 };
        private static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714 };
        private static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707 };

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnLoad;
        }

        static void Game_OnLoad(EventArgs args)
        {
            #region main
            {
                Player = ObjectManager.Player;

                if (Player.ChampionName != ChampionName)
                {
                    ShowNotification("You aren't a TeddyBear !", 3000);
                    return;
                }

                _q = new Spell(SpellSlot.Q, 600, TargetSelector.DamageType.Physical);
                _w = new Spell(SpellSlot.W, 405, TargetSelector.DamageType.Physical);
                _e = new Spell(SpellSlot.E, 400, TargetSelector.DamageType.Magical);
                _r = new Spell(SpellSlot.R, 125, TargetSelector.DamageType.Magical);

                _botrk = new Items.Item(3153, 450f);
                _cutlass = new Items.Item(3144, 450f);

            }
            #endregion

            #region content menu

            _menu = new Menu("Teddy Bear - ThunderBuddy", "teddy.bear", true);
            var orbwalkerMenu = new Menu(("Orbwalker"), "teddy.bear.orbwalker");
            _orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            _menu.AddSubMenu(orbwalkerMenu);

            Menu targetSelector = new Menu("Target Selector", "teddy.bear.ts");
            TargetSelector.AddToMenu(targetSelector);
            _menu.AddSubMenu(targetSelector);


            var comboMenu = new Menu("Combo", "teddy.bear.combo");
            {
                comboMenu.AddItem(new MenuItem("teddy.bear.combo.useq", "Use Q").SetValue(true));
                comboMenu.AddItem(new MenuItem("teddy.bear.combo.usew", "Use W").SetValue(true));
                comboMenu.AddItem(new MenuItem("teddy.bear.combo.usee", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("teddy.bear.combo.user", "Use R").SetValue(true));
            }
            _menu.AddSubMenu(comboMenu);


            var laneclearing = new Menu("Lane clear", "teddy.bear.laneclearing");
            {
                laneclearing.AddItem(new MenuItem("teddy.bear.laneclearing.useQ", "Use Q").SetValue(true));
                laneclearing.AddItem(new MenuItem("teddy.bear.laneclearing.useW", "Use W").SetValue(true));
                laneclearing.AddItem(new MenuItem("teddy.bear.laneclearing.useE", "Use E").SetValue(true));

            }
            _menu.AddSubMenu(laneclearing);


            var fleeMenu = new Menu("Flee", "teddy.bear.flee");
            {
                fleeMenu.AddItem(new MenuItem("teddy.bear.flee.fleekey", "FLEEEEEEEEEE!").SetValue(new KeyBind('A', KeyBindType.Press)));
                fleeMenu.AddItem(new MenuItem("teddy.bear.flee.useQ", "Use Q").SetValue(true));
                fleeMenu.AddItem(new MenuItem("teddy.bear.flee.useE", "Use E").SetValue(true));

            }
            _menu.AddSubMenu(fleeMenu);


            var misc = new Menu("Misc", "teddy.bear.misc");
            {
                misc.AddItem(new MenuItem("teddy.bear.misc.skW", "safe kill with W").SetValue(true));
                misc.AddItem(new MenuItem("teddy.bear.misc.packets", "use packets").SetValue(true));
            }
            _menu.AddSubMenu(misc);


            var drawingMenu = new Menu("Drawing", "teddy.bear.drawing");
            {
                drawingMenu.AddItem(new MenuItem("DrawQ", "Draw Q range").SetValue(new Circle(true, Color.Aqua, _q.Range)));
                drawingMenu.AddItem(new MenuItem("DrawW", "Draw W range").SetValue(new Circle(true, Color.SpringGreen, _w.Range)));
                drawingMenu.AddItem(new MenuItem("DrawE", "Draw E range").SetValue(new Circle(true, Color.SlateBlue, _e.Range)));
                drawingMenu.AddItem(new MenuItem("DrawR", "Draw R range").SetValue(new Circle(true, Color.Red, _r.Range)));
                drawingMenu.AddItem(new MenuItem("DrawHP", "Draw HP Indicator").SetValue(true));
            }
            _menu.AddSubMenu(drawingMenu);

            _menu.AddToMainMenu();

            #endregion

            Interrupter2.OnInterruptableTarget += Interrupter_OnPossibleToInterrupt;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += OnEndScene;
            ShowNotification("TeddyBear - Loaded", 3000);

        }

        static void Interrupter_OnPossibleToInterrupt(Obj_AI_Base unit, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (args.DangerLevel >= Interrupter2.DangerLevel.High && unit.Distance(Player.Position) <= _q.Range)
            {
                _q.Cast(unit);
            }
        }

        private static void OnEndScene(EventArgs args)
        {
            if (_menu.Item("DrawHP").GetValue<bool>())
            {
                foreach (var enemy in
                    ObjectManager.Get<Obj_AI_Hero>().Where(ene => !ene.IsDead && ene.IsEnemy && ene.IsVisible))
                {
                    Hpi.unit = enemy;
                    Hpi.DrawDmg(CalcDamage(enemy), Color.Green);
                }
            }
        }

        private static int CalcDamage(Obj_AI_Base target)
        {
            var aa = Player.GetAutoAttackDamage(target, true) * (1 + Player.Crit);
            var damage = aa;

            if (_r.IsReady()) // rdamage
            {
                damage += _r.GetDamage(target);
            }

            if (_q.IsReady()) // qdamage
            {

                damage += _q.GetDamage(target);
            }

            if (_e.IsReady()) // edamage
            {

                damage += _e.GetDamage(target);
            }

            return (int)damage;
        }

        public static void ShowNotification(string message, int duration = -1, bool dispose = true)
        {
            Notifications.AddNotification(new Notification(message, duration, dispose));
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var menuItem1 = _menu.Item("DrawQ").GetValue<Circle>();
            var menuItem2 = _menu.Item("DrawE").GetValue<Circle>();
            var menuItem3 = _menu.Item("DrawW").GetValue<Circle>();
            var menuItem4 = _menu.Item("DrawR").GetValue<Circle>();

            if (menuItem1.Active && _q.IsReady()) Render.Circle.DrawCircle(Player.Position, _q.Range, Color.SpringGreen);
            if (menuItem2.Active && _e.IsReady()) Render.Circle.DrawCircle(Player.Position, _e.Range, Color.Crimson);
            if (menuItem3.Active && _w.IsReady()) Render.Circle.DrawCircle(Player.Position, _w.Range, Color.Aqua);
            if (menuItem4.Active && _r.IsReady()) Render.Circle.DrawCircle(Player.Position, _r.Range, Color.Firebrick);
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (_menu.Item("teddy.bear.flee.fleekey").GetValue<KeyBind>().Active)
            {
                Flee();
            }


            switch (_orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Laneclear();
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    break;
            }
        }

        private static void Flee()
        {
            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos, false);
            _q.Cast();
        }

        private static void Laneclear() // jungle clear ^^
        {
            bool vQ = _q.IsReady() && _menu.Item("teddy.bear.laneclearing.useQ").GetValue<bool>();
            bool vW = _w.IsReady() && _menu.Item("teddy.bear.laneclearing.useW").GetValue<bool>();
            bool vE = _e.IsReady() && _menu.Item("teddy.bear.laneclearing.useE").GetValue<bool>();

            var minionBase = MinionManager.GetMinions(_e.Range);
            var jungleBase = MinionManager.GetMinions(_w.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            #region Q-Cast Jungle
            if (vQ)
            {
                foreach (var junglemob in jungleBase.Where(x => x.HealthPercent >= 25) )
                {
                    _q.Cast(junglemob);
                }
            }
            #endregion

            #region W-Cast Jungle
            if (vW)
            {
                foreach (Obj_AI_Minion junglemob in ObjectManager.Get<Obj_AI_Minion>().Where(
                    x =>
                        x.Name.Contains("SRU_Blue1.1.1") || x.Name.Contains("SRU_Blue7.1.1") ||
                        x.Name.Contains("SRU_Red4.1.1") || x.Name.Contains("SRU_Red10.1.1") || 
                        x.Name.Contains("SRU_Dragon6.1.1") || x.Name.Contains("SRU_Baron12.1.1") 
                        && x.Health < _w.GetDamage(x)))
                {
                    if (junglemob.Health < _w.GetDamage(junglemob))
                            _w.CastOnUnit(junglemob);
                }
            }
            #endregion

            #region E-Cast Jungle
            if (vE)
            {
                if (jungleBase.Count >= 2)
                {
                    _e.Cast();
                }

                if(minionBase.Count >= 3)
                {
                    _e.Cast();
                }

            }
            #endregion
        } 

        private static void Combo()
        {
            bool vQ = _q.IsReady() && _menu.Item("teddy.bear.combo.useq").GetValue<bool>();
            bool vW = _w.IsReady() && _menu.Item("teddy.bear.combo.usew").GetValue<bool>();
            bool vE = _e.IsReady() && _menu.Item("teddy.bear.combo.usee").GetValue<bool>();
            bool vR = _r.IsReady() && _menu.Item("teddy.bear.combo.user").GetValue<bool>();
            bool useskW = _menu.Item("teddy.bear.misc.skW").GetValue<bool>();

            Obj_AI_Hero tsQ = TargetSelector.GetTarget(_q.Range, TargetSelector.DamageType.Magical);
            Obj_AI_Hero tsR = TargetSelector.GetTarget(_r.Range, TargetSelector.DamageType.Magical);

            #region Q-Cast
            if (vQ)
            {
                if (tsQ.Distance(Player.Position) >= 2500 && tsQ.Direction == Player.Direction &&  tsQ.MoveSpeed > Player.MoveSpeed &&
                    tsQ.MoveSpeed < Player.MoveSpeed*1.3)
                {
                    _q.Cast(tsQ);
                }
                if (tsQ.GetEnemiesInRange(100).Any(enemies => enemies.IsEnemy && !enemies.IsDead) && tsQ.IsValidTarget())
                {
                    _q.Cast(tsQ);
                }
                else if (Player.CountAlliesInRange(500) >= 1 && tsQ.IsValidTarget())
                {
                    _q.Cast(tsQ);
                }
                else if (tsQ.IsValidTarget())
                {
                    _q.Cast(tsQ);
                }
            }
            #endregion

            #region W-Cast
            if (vW && useskW)
            {
                if (tsQ.IsValidTarget(_w.Range) && tsQ.Health < _w.GetDamage(tsQ))
                {
                    _w.CastOnUnit(tsQ);
                }
            }
            else if (vW)
            {
                if (tsQ.IsValidTarget(_w.Range))
                {
                    _w.CastOnUnit(tsQ);
                }
            }
            #endregion

            #region E-Cast
            if (vE)
            {
                if (tsQ.IsValidTarget(_e.Range) && tsQ.Distance(Player.Position) <= _w.Range)
                {
                    _e.Cast();
                }
            }
            #endregion

            #region R-Cast
            if (vR)
            {
                if (tsR.IsValidTarget(Player.AttackRange) && tsR.HealthPercent > 25)
                {
                    _r.Cast();
                }
            }
             #endregion

        }
    }
}