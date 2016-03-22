using System;
using System.Drawing;
using CS_Counter.Properties;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using Font = SharpDX.Direct3D9.Font;

namespace CS_Counter
{
    class CsCounter
    {

        private static readonly Render.Text Text = new Render.Text(
            0, 0, "", 12, new ColorBGRA(red: 255, green: 0, blue: 0, alpha: 255), "Verdana");

        private const int XOffset = 15;
        private const int YOffset = 35;

        private static Line _line;

        private static Menu _menu2;
        private static MenuItem _menuenable2;
        private static MenuItem _menuenable3;
        private static MenuItem _menuenable4;

        private static MenuItem _xPos;
        private static MenuItem _yPos;

        private static MenuItem _advanced;

        private static TimeSpan _minionspawn;
        public static int Countminionwave;
        private static TimeSpan _timeplus;
        private static int _minionsgesamt;
        public static int Waveone;
        public static int Wavetwo;
        public static int Wavethree;
        private static int _percent;

        public static Texture CdFrameTexture;
        public static Sprite Sprite;
        public static Font Textx;
        public static int X;
        public static int Y;

        public static bool Enabled = true;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {

            Notifications.AddNotification("CS Counter loaded.", 100);

            _menu2 = new Menu("CS Counter", "menu2", true);

            _line = new Line(Drawing.Direct3DDevice);

            Textx = new Font(
                    Drawing.Direct3DDevice,
                    new FontDescription
                    {
                        FaceName = "Calibri",
                        Height = 13,
                        OutputPrecision = FontPrecision.Default,
                        Quality = FontQuality.Default
                    });

            _menuenable2 = new MenuItem("menu.drawings.enable2", "CS Count").SetValue(true);
            _menu2.AddItem(_menuenable2);
            _menuenable3 = new MenuItem("menu.drawings.enable3", "My CS Count").SetValue(true);
            _menu2.AddItem(_menuenable3);
            _menuenable4 = new MenuItem("menu.drawings.enable4", "Allies CS Count").SetValue(true);
            _menu2.AddItem(_menuenable4);
            _advanced = new MenuItem("menu.drawings.advanced", "Advanced Farminfo (ME)").SetValue(false);
            _menu2.AddItem(_advanced);
            _xPos = new MenuItem("menu.Calc.calc5", "X - Position").SetValue(new Slider(0, -100));
            _menu2.AddItem(_xPos);
            _yPos = new MenuItem("menu.Calc.calc6", "Y - Position").SetValue(new Slider(0, -100));
            _menu2.AddItem(_yPos);


            _menu2.AddToMainMenu();

            _minionspawn = new TimeSpan(0, 0, 1, 30);
            Countminionwave = 3;
            _timeplus = new TimeSpan(0, 0, 0, 30);
            Waveone = 0;
            Wavetwo = 1;
            Wavethree = 2;
            _minionsgesamt = 0;
            Drawing.OnDraw += Drawing_OnDraw;


        }

        private static void Drawing_OnDraw(EventArgs args)
        {

            var x = new TimeSpan(0, 0, 0, 18);
            var var = TimeSpan.FromSeconds(Game.Time) - x;

           

            if (!(var <= _minionspawn))
            {
                var wave = (((int)var.TotalSeconds - (int)_minionspawn.TotalSeconds) / (int)_timeplus.TotalSeconds) + 1;
                var extraminion = Math.Floor((double)wave / 3);
                _minionsgesamt = wave * 6 + (int)extraminion;
            }

            GetCsEnemy();

            
        }

        private static void GetCsEnemy()
        {
           

            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>())
            {

                if (hero.IsDead | !hero.IsVisible | !_menuenable2.GetValue<bool>() |
                    (hero.IsAlly && !_menuenable4.GetValue<bool>() && !hero.IsMe) | (hero.IsMe && !_menuenable3.GetValue<bool>()))
                {
                    Text.text = "";
                    continue;
                }

                var cs = hero.MinionsKilled + hero.NeutralMinionsKilled + hero.SuperMonsterKilled;
                var barPos = hero.HPBarPosition;
                var pos = Drawing.WorldToScreen(hero.Position);
                pos.X -= 50 + _xPos.GetValue<Slider>().Value;
                pos.Y += 20 + _yPos.GetValue<Slider>().Value;

                if (hero.IsMe && _menuenable3.GetValue<bool>())
                {

                    Text.X = (int)pos.X;
                    Text.X += 110 / 6;
                    Text.Y = (int)pos.Y;
                    Text.Color = new ColorBGRA(red: 255, green: 255, blue: 255, alpha: 255);

                    if (cs != 0)
                    {
                        _percent = cs * 100 / _minionsgesamt;
                    }
                    else
                    {
                        _percent = 0;
                    }

                    if (!_advanced.GetValue<bool>())
                    {
                        _line.Begin();
                        _line.Draw(new[] { new Vector2(pos.X, pos.Y - 2), new Vector2(pos.X + 50, pos.Y - 2) }, new ColorBGRA(255, 255, 255, 255));
                        _line.End();

                        _line.Begin();
                        _line.Draw(new[] { new Vector2(pos.X, pos.Y + 14), new Vector2(pos.X + 50, pos.Y + 14) }, new ColorBGRA(255, 255, 255, 255));
                        _line.End();

                        _line.Begin();
                        _line.Draw(new[] { new Vector2(pos.X, pos.Y - 2), new Vector2(pos.X, pos.Y + 14) }, new ColorBGRA(255, 255, 255, 255));
                        _line.End();

                        _line.Begin();
                        _line.Draw(new[] { new Vector2(pos.X + 50, pos.Y - 2), new Vector2(pos.X + 50, pos.Y + 14) }, new ColorBGRA(255, 255, 255, 255));
                        _line.End();

                        Text.text = _percent + " %";
                    }
                    else
                    {
                        _line.Begin();
                        _line.Draw(new[] { new Vector2(pos.X, pos.Y - 2), new Vector2(pos.X + 100, pos.Y - 2) }, new ColorBGRA(255, 255, 255, 255));
                        _line.End();


                        _line.Begin();
                        _line.Draw(new[] { new Vector2(pos.X, pos.Y + 14), new Vector2(pos.X + 100, pos.Y + 14) }, new ColorBGRA(255, 255, 255, 255));
                        _line.End();

                        _line.Begin();
                        _line.Draw(new[] { new Vector2(pos.X, pos.Y - 2), new Vector2(pos.X, pos.Y + 14) }, new ColorBGRA(255, 255, 255, 255));
                        _line.End();

                        _line.Begin();
                        _line.Draw(new[] { new Vector2(pos.X + 100, pos.Y - 2), new Vector2(pos.X + 100, pos.Y + 14) }, new ColorBGRA(255, 255, 255, 255));
                        _line.End();

                        Text.text = _percent + " %" + " |  " + cs + " / " + _minionsgesamt;
                    }

                    Text.OnEndScene();
                    
                    continue;
                    
                }

                _line.Begin();
                _line.Draw(new[] { new Vector2(pos.X, pos.Y - 2), new Vector2(pos.X + 100, pos.Y - 2) }, new ColorBGRA(255, 255 , 255, 255));
                _line.End();

                _line.Begin();
                _line.Draw(new[] { new Vector2(pos.X, pos.Y + 14), new Vector2(pos.X + 100, pos.Y + 14) }, new ColorBGRA(255, 255, 255, 255));
                _line.End();

                _line.Begin();
                _line.Draw(new[] { new Vector2(pos.X, pos.Y - 2), new Vector2(pos.X, pos.Y + 14) }, new ColorBGRA(255, 255, 255, 255));
                _line.End();

                _line.Begin();
                _line.Draw(new[] { new Vector2(pos.X + 100, pos.Y - 2), new Vector2(pos.X + 100, pos.Y + 14) }, new ColorBGRA(255, 255, 255, 255));
                _line.End();

                Text.X = (int) pos.X;
                Text.X += 110/6;

                Text.Y = (int) pos.Y;
                Text.Color = new ColorBGRA(red: 255, green: 255, blue: 255, alpha: 255);
                Text.text = "CS Count: " + cs;
                Text.OnEndScene();

                
                
                
            }

            
        }
    }
}
