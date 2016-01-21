using System;
using System.Drawing;
using System.Drawing.Printing;
using LeagueSharp;
using LeagueSharp.Common;
using _Damage = LeagueSharp.Common.Damage;
using SharpDX;
using Color = System.Drawing.Color;
using Spells = DamageIndicator.Champions;

namespace DamageIndicator
{
    class Program
    {

        #region some var's
        public static Obj_AI_Hero Player = ObjectManager.Player;
        private static Obj_AI_Hero _target;
        private static Menu _menu;
        private static Menu _calc;
        private static MenuItem drawFill;
        private static MenuItem drawLine;
        private static MenuItem CalcQ;
        private static MenuItem CalcW;
        private static MenuItem CalcE;
        private static MenuItem CalcR;
        private static MenuItem CalcAA;
        private static MenuItem CalcItems;
        private static MenuItem CalcSummoners;
        public delegate float DamageToUnitDelegate(Obj_AI_Hero hero);
        public static Color Color = Color.Lime;
        public static Color FillColor = Color.Goldenrod;
        public static bool Fill = true;
        public static bool Enabled = true;
        private static DamageToUnitDelegate _damageToUnit;
        private static readonly Render.Text Text = new Render.Text(
            0, 0, "", 11, new ColorBGRA(255, 0, 0, 255), "monospace");

        private static readonly SpellSlot IgniteSlot = ObjectManager.Player.GetSpellSlot("SummonerDot");

        private static float _percentpenetrationarmor;
        private static float _percentpenetrationmagic;
        #endregion

        static void Main(string[] args)
        {

            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;

        }

        static void Game_OnGameLoad(EventArgs args)
        {

            #region sayings

            //Game.PrintChat("<font color=\"#AF7AFF\"><b>Damage Indicator</b></font> for " + Player.ChampionName + " - Loaded");
            Notifications.AddNotification("Damage Indicator for " + Player.ChampionName, 10, true);
            #endregion

            #region Menushit
            _menu = new Menu("Damage Indicator", "menu", true);
            var Drawings = new Menu("Drawings", "Drawings");
            _menu.AddSubMenu(Drawings);

            drawFill = new MenuItem("menu.Drawings.Draw_Fill", "Draw Combo Damage (Fill)", true).SetValue(new Circle(true, Color.FromArgb(90, 255, 169, 4)));
            Drawings.AddItem(drawFill);
            drawLine = new MenuItem("menu.Drawings.Draw_Line", "Draw Combo Damage (Line)", true).SetValue(new Circle(true, Color.FromArgb(90, 255, 169, 4)));
            Drawings.AddItem(drawLine);


            var _calc = new Menu("Calculations", "Calc");
            _menu.AddSubMenu(_calc);

            CalcQ = new MenuItem("menu.Calc.calcQ", "calculate Q Damage").SetValue(true);
            _calc.AddItem(CalcQ);
            CalcW = new MenuItem("menu.Calc.calcW", "calculate W Damage").SetValue(true);
            _calc.AddItem(CalcW);
            CalcE = new MenuItem("menu.Calc.calcE", "calculate E Damage").SetValue(true);
            _calc.AddItem(CalcE);
            CalcR = new MenuItem("menu.Calc.calcR", "calculate R Damage").SetValue(true);
            _calc.AddItem(CalcR);
            CalcAA = new MenuItem("menu.Calc.calcAA", "calculate AA Damage").SetValue(new Slider(0, 1, 10));
            _calc.AddItem(CalcAA);
            CalcItems = new MenuItem("menu.Calc.calcItems", "calculate Items Damage").SetValue(true);
            _calc.AddItem(CalcItems);
            CalcSummoners = new MenuItem("menu.Calc.calcSummoners", "calculate SummonerSpell Damage").SetValue(true);
            _calc.AddItem(CalcSummoners);

            _menu.AddItem(new MenuItem("422442<ef<ef4242f", ""));
            _menu.AddItem(new MenuItem("42f<afsf", "Version: 1.4.0.0"));
            _menu.AddItem(new MenuItem("fsfqwfa", "Made By Kyon"));
            _menu.AddItem(new MenuItem("awfafaF", "Thanks for using !"));

            _menu.AddToMainMenu();

            #endregion

            Spells.Load();
            Console.WriteLine("Menu Loaded");
            Drawing.OnDraw += Drawing_OnDraw;

        }

        private static void Drawing_OnDraw(EventArgs args)
        {

            if (!Player.IsDead)
            {
                DamageIndicator.DamageToUnit = GetComboDamage;
                DamageIndicator.Enabled = true;
                DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
                DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;
                DamageIndicator.Color = drawLine.GetValue<Circle>().Color;
                DamageIndicator.ColorFill = drawLine.GetValue<Circle>().Active;
            }
            
        }

        private static float GetComboDamage(Obj_AI_Base enemy)
        {
            // do nothing is there no enemy
            if (enemy == null)
                return 0;

            // reset values
            double damage = 0d;

            var calcQ_ = _menu.Item("menu.Calc.calcQ").GetValue<bool>();
            var calcW_ = _menu.Item("menu.Calc.calcW").GetValue<bool>();
            var calcE_ = _menu.Item("menu.Calc.calcE").GetValue<bool>();
            var calcR_ = _menu.Item("menu.Calc.calcR").GetValue<bool>();
            var calcItems_ = _menu.Item("menu.Calc.calcItems").GetValue<bool>();
            var calcSummoners_ = _menu.Item("menu.Calc.calcSummoners").GetValue<bool>();
            var AA = _menu.Item("menu.Calc.calcAA").GetValue<Slider>().Value;

            //Console.WriteLine(Spells.Q.DamageType + " // " + Spells.W.DamageType + " // " + Spells.E.DamageType + " // " + Spells.R.DamageType);

            // Check Q Spell and Calc
            #region Q Spell Calc
            if (Spells.Q.IsReady() && calcQ_)
            {
                if (Spells.Q.DamageType == TargetSelector.DamageType.Magical)
                {
                    damage +=
                        Player.CalcDamage(enemy, _Damage.DamageType.Magical, Player.GetSpellDamage(enemy, SpellSlot.Q)) * _percentpenetrationmagic;
                }
                else if (Spells.Q.DamageType == TargetSelector.DamageType.Physical)
                {
                    damage += 
                        Player.CalcDamage(enemy, _Damage.DamageType.Physical, Player.GetSpellDamage(enemy, SpellSlot.Q)) * _percentpenetrationarmor;
                }
                else if (Spells.Q.DamageType == TargetSelector.DamageType.True)
                {
                    damage += 
                        Player.CalcDamage(enemy, _Damage.DamageType.True, Player.GetSpellDamage(enemy, SpellSlot.Q));
                }
            }
            #endregion

            // Check W Spell and Calc
            #region W Spell Calc
            if (Spells.W.IsReady() && calcW_)
            {
                if (Spells.W.DamageType == TargetSelector.DamageType.Magical)
                {
                    damage += 
                        Player.CalcDamage(enemy, _Damage.DamageType.Magical, Player.GetSpellDamage(enemy, SpellSlot.W)) * _percentpenetrationmagic;
                }
                else if (Spells.W.DamageType == TargetSelector.DamageType.Physical)
                {
                    damage += 
                        Player.CalcDamage(enemy, _Damage.DamageType.Physical, Player.GetSpellDamage(enemy, SpellSlot.W)) * _percentpenetrationarmor;
                }
                else if (Spells.W.DamageType == TargetSelector.DamageType.True)
                {
                    damage += Player.CalcDamage(enemy, _Damage.DamageType.True, Player.GetSpellDamage(enemy, SpellSlot.W));
                }
            }
            #endregion

            // Check E Spell and Calc
            #region E Spell Calc
            if (Spells.E.IsReady() && calcE_)
            {
                if (Spells.E.DamageType == TargetSelector.DamageType.Magical)
                {
                    damage += Player.CalcDamage(enemy, _Damage.DamageType.Magical, Player.GetSpellDamage(enemy, SpellSlot.E)) * _percentpenetrationmagic;
                }
                else if (Spells.E.DamageType == TargetSelector.DamageType.Physical)
                {
                    damage += Player.CalcDamage(enemy, _Damage.DamageType.Physical, Player.GetSpellDamage(enemy, SpellSlot.E)) * _percentpenetrationarmor;
                }
                else if (Spells.E.DamageType == TargetSelector.DamageType.True)
                {
                    damage += Player.CalcDamage(enemy, _Damage.DamageType.True, Player.GetSpellDamage(enemy, SpellSlot.E));
                }
            }
            #endregion

            // Check R Spell and Calc
            #region R Spell Calc
            if (Spells.R.IsReady() && calcR_)
            {
                if (Spells.R.DamageType == TargetSelector.DamageType.Magical)
                {
                    damage += Player.CalcDamage(enemy, _Damage.DamageType.Magical, Player.GetSpellDamage(enemy, SpellSlot.R)) * _percentpenetrationmagic;
                }
                else if (Spells.R.DamageType == TargetSelector.DamageType.Physical)
                {
                    damage += Player.CalcDamage(enemy, _Damage.DamageType.Physical, Player.GetSpellDamage(enemy, SpellSlot.R)) * _percentpenetrationarmor;
                }
                else if (Spells.R.DamageType == TargetSelector.DamageType.True)
                {
                    damage += Player.CalcDamage(enemy, _Damage.DamageType.True, Player.GetSpellDamage(enemy, SpellSlot.R));
                }
            }
            #endregion

            damage += Player.GetAutoAttackDamage(enemy, true) * AA; // no need for Autoattack Damage 

            if (calcItems_)
            damage = ActiveItems.CalcDamage(enemy, damage); // active Items thanks xSlaice :)

            if (Ignite_Ready() && calcSummoners_)
                damage += ObjectManager.Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);

            return (float)damage;

        }

        private static bool Ignite_Ready()
        {
            if (IgniteSlot != SpellSlot.Unknown && ObjectManager.Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
                return true;
            return false;
        }
    }
}
