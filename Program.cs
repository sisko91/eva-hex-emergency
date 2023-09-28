using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using VRage.Game.GUI.TextPanel;
using VRageMath;
using VRageRender;

namespace IngameScript {
    partial class Program : MyGridProgram {
        IMyTextPanel lcd;
        List<Tile> tiles;
        bool enabled = false;

        public Program() {
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
            lcd = GridTerminalSystem.GetBlockWithName("Emergency LCD") as IMyTextPanel;

            tiles = new List<Tile>();
            float x = 0;
            float y = 0;
            bool odd = false;
            Random r = new Random();

            while (true) {
                while (true) {
                    tiles.Add(new Tile(new Vector2(x, y), r.Next(2, 15), r.Next(0, 2)));
                    if (y > 512) {
                        break;
                    }
                    
                    y += Tile.HexHeight + 5;
                }

                if (x > 512) {
                    break;
                }

                odd = !odd;
                y = odd ? Tile.HexHeight/2 + 2.5f: 0;
                x += Tile.HexWidth * .75f + 2;
            }
        }

        public void Main(string argument, UpdateType updateSource) {
            if (!enabled) {
                enabled = GridTerminalSystem.GetBlockWithName("Front") == null;
                var cframe = lcd.DrawFrame();
                cframe.Dispose();
                return;
            }

            lcd.ContentType = ContentType.TEXT_AND_IMAGE;
            lcd.ContentType = ContentType.SCRIPT;

            var frame = lcd.DrawFrame();
            foreach (var tile in tiles) {
                tile.Draw(frame);
            }

            frame.Dispose();
        }

        public class Tile {
            public Vector2 center;
            public float flickerRate;
            public float counter;
            bool off = false;

            public Color hexColor = new Color(255, 0, 0, 10);
            public static float HexHeight = 75f;
            public static float HexWidth = (float)(HexHeight * 2.0/Math.Sqrt(3));

            public Tile(Vector2 center, float flickerRate, float offset) {
                this.center = center;
                this.flickerRate = flickerRate;
                this.counter = offset;
            }

            public void Draw(MySpriteDrawFrame frame) {
                counter += 1;
                if (counter == flickerRate) {
                    counter = 0;
                    off = !off;
                    hexColor = off ? new Color(255, 0, 0, 5) : new Color(255, 0, 0, 20);
                }

                //Hex
                float triangleHeight = HexWidth / 4;
                MySprite rect = new MySprite(SpriteType.TEXTURE, "SquareSimple", center, new Vector2(HexWidth/2, HexHeight), hexColor);
                MySprite leftTriangle = new MySprite(SpriteType.TEXTURE, "Triangle", new Vector2(center.X - HexWidth * 3f/8f, center.Y),
                                                     new Vector2(HexHeight, HexWidth / 4), hexColor, rotation: -(float) Math.PI / 2);
                MySprite rightTriangle = new MySprite(SpriteType.TEXTURE, "Triangle", new Vector2(center.X + HexWidth * 3f/8f, center.Y),
                                                     new Vector2(HexHeight, HexWidth / 4), hexColor, rotation: (float)Math.PI / 2);

                frame.Add(rect);
                frame.Add(leftTriangle);
                frame.Add(rightTriangle);

                if (!off) {
                    MySprite text = new MySprite(SpriteType.TEXT, "Emergency", center - new Vector2(0, 8), color: Color.Black, rotation: .5f, fontId: "DEBUG");
                    MySprite upTriangle = new MySprite(SpriteType.TEXTURE, "Triangle", new Vector2(center.X, center.Y - HexHeight / 4), new Vector2(10, 15), Color.Black);
                    MySprite downTriangle = new MySprite(SpriteType.TEXTURE, "Triangle", new Vector2(center.X, center.Y + HexHeight / 4), new Vector2(10, 15), Color.Black, rotation: (float)Math.PI);
                    
                    frame.Add(text);
                    frame.Add(upTriangle);
                    frame.Add(downTriangle);
                }
            }
        }
    }
}
