using System;
using System.Drawing;
using System.Security.AccessControl;
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
        #region vars

        public const int XOffset = 15;
        public const int YOffset = 35;

        public static Line Line;

        public static readonly Render.Text Text = new Render.Text(
            0, 0, "", 12, new ColorBGRA(red: 255, green: 0, blue: 0, alpha: 255), "Verdana");

        public static Font Textx;

        public static int Minionsgesamt;
        public static int Percent;

        private static Texture CdFrameTexture;
        private static Sprite Sprite;

        public static int X;
        public static int Y;

        public static bool Enabled = true;

        public static long MinCount;

        #endregion

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            //if (Game.MapId != GameMapId.CrystalScar) { return; }

            Init.PrepareMenu();

            Minionsgesamt = Game.Time < 60 ? 0 : HeroManager.Player.MinionsKilled;

            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Minion.OnCreate += Obj_AI_Minion_OnCreate;
            

        }

        private static void Obj_AI_Minion_OnCreate(GameObject sender, EventArgs args)
        {

            if (HeroManager.Player.Team == GameObjectTeam.Chaos && sender.Name.Contains("Minion_T200"))
            {
                MinCount++;

            }else if (HeroManager.Player.Team == GameObjectTeam.Order && sender.Name.Contains("Minion_T100"))
            {
                MinCount++;
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {

            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>())
            {

                int mingesamt = 0;

                if (Game.MapId == GameMapId.HowlingAbyss)
                {
                    mingesamt = (int) MinCount + Minionsgesamt;
                }
                else if (Game.MapId == GameMapId.TwistedTreeline)
                {
                    mingesamt = (int)MinCount / 2 + Minionsgesamt;
                }
                else if (Game.MapId == GameMapId.SummonersRift)
                {
                    mingesamt = (int) MinCount / 3 + Minionsgesamt;
                }

                if (hero.IsDead | !hero.IsVisible | !Init.Menuenable2.GetValue<bool>() |
                    (hero.IsAlly && !Init.Menuenable4.GetValue<bool>() && !hero.IsMe) | (hero.IsMe && !Init.Menuenable3.GetValue<bool>()))
                {
                    Text.text = "";
                    continue;
                }

                var cs = hero.MinionsKilled + hero.NeutralMinionsKilled + hero.SuperMonsterKilled;
                var pos = Drawing.WorldToScreen(hero.Position);
                pos.X -= 50 + Init.XPos.GetValue<Slider>().Value;
                pos.Y += 20 + Init.YPos.GetValue<Slider>().Value;

                if (hero.IsMe && Init.Menuenable3.GetValue<bool>())
                {

                    Text.X = (int)pos.X;
                    Text.X += 110 / 6;
                    Text.Y = (int)pos.Y;
                    Text.Color = new ColorBGRA(red: 255, green: 255, blue: 255, alpha: 255);

                    Percent = mingesamt != 0 ? (cs*100/mingesamt) : 0;

                    if (!Init.Advanced.GetValue<bool>())
                    {
                        if (!Init.Advanced_box.GetValue<bool>())
                        {
                            Line.Begin();
                            Line.Draw(new[] {new Vector2(pos.X, pos.Y - 2), new Vector2(pos.X + 50, pos.Y - 2)},
                                new ColorBGRA(255, 255, 255, 255));
                            Line.End();

                            Line.Begin();
                            Line.Draw(new[] {new Vector2(pos.X, pos.Y + 14), new Vector2(pos.X + 50, pos.Y + 14)},
                                new ColorBGRA(255, 255, 255, 255));
                            Line.End();

                            Line.Begin();
                            Line.Draw(new[] {new Vector2(pos.X, pos.Y - 2), new Vector2(pos.X, pos.Y + 14)},
                                new ColorBGRA(255, 255, 255, 255));
                            Line.End();

                            Line.Begin();
                            Line.Draw(new[] {new Vector2(pos.X + 50, pos.Y - 2), new Vector2(pos.X + 50, pos.Y + 14)},
                                new ColorBGRA(255, 255, 255, 255));
                            Line.End();
                        }

                        Text.text = Percent + " %";

                    }
                    else
                    {
                        if (!Init.Advanced_box.GetValue<bool>())
                        {
                            Line.Begin();
                            Line.Draw(new[] {new Vector2(pos.X, pos.Y - 2), new Vector2(pos.X + 100, pos.Y - 2)},
                                new ColorBGRA(255, 255, 255, 255));
                            Line.End();


                            Line.Begin();
                            Line.Draw(new[] {new Vector2(pos.X, pos.Y + 14), new Vector2(pos.X + 100, pos.Y + 14)},
                                new ColorBGRA(255, 255, 255, 255));
                            Line.End();

                            Line.Begin();
                            Line.Draw(new[] {new Vector2(pos.X, pos.Y - 2), new Vector2(pos.X, pos.Y + 14)},
                                new ColorBGRA(255, 255, 255, 255));
                            Line.End();

                            Line.Begin();
                            Line.Draw(
                                new[] {new Vector2(pos.X + 100, pos.Y - 2), new Vector2(pos.X + 100, pos.Y + 14)},
                                new ColorBGRA(255, 255, 255, 255));
                            Line.End();
                        }
                        Text.text = Percent + " %" + " |  " + cs + " / " + mingesamt;
                    }

                    Text.OnEndScene();

                    continue;

                }

                if (!Init.Advanced_box.GetValue<bool>())
                {
                    Line.Begin();
                    Line.Draw(new[] {new Vector2(pos.X, pos.Y - 2), new Vector2(pos.X + 100, pos.Y - 2)},
                        new ColorBGRA(255, 255, 255, 255));
                    Line.End();

                    Line.Begin();
                    Line.Draw(new[] {new Vector2(pos.X, pos.Y + 14), new Vector2(pos.X + 100, pos.Y + 14)},
                        new ColorBGRA(255, 255, 255, 255));
                    Line.End();

                    Line.Begin();
                    Line.Draw(new[] {new Vector2(pos.X, pos.Y - 2), new Vector2(pos.X, pos.Y + 14)},
                        new ColorBGRA(255, 255, 255, 255));
                    Line.End();

                    Line.Begin();
                    Line.Draw(new[] {new Vector2(pos.X + 100, pos.Y - 2), new Vector2(pos.X + 100, pos.Y + 14)},
                        new ColorBGRA(255, 255, 255, 255));
                    Line.End();
                }

                Text.X = (int)pos.X;
                Text.X += 110 / 6;

                Text.Y = (int)pos.Y;
                Text.Color = new ColorBGRA(red: 255, green: 255, blue: 255, alpha: 255);
                Text.text = "CS Count: " + mingesamt;
                Text.OnEndScene();
                
            }

        }

    }
}
