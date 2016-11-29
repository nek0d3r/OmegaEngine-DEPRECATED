using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace OmegaEngine
{
    public class Events
    {
        private XmlDocument reader;
        private XmlNodeList commands;
        private XmlNode t;
        private Stack<XmlNode> tStack;
        private string line;
        private string speaker;
        private string ratio;
        private Texture2D profile;          // Texture for profile to display on message
        private Texture2D profileTo;
        private Vector2 uiSize;
        private float topLine;
        private float cutTopLine = 21.0f;
        private int textOffset;
        private int cutTextOff = 5;
        private int textLength;
        private int cutTextLen = 38;
        private bool ifTo;
        private SoundEffect dialogue;
        private SoundEffectInstance lineInst;

        private bool advance;               // Flag for testing if script line is finished (generally pressing E) to continue script
        private bool eoe;                   // Flag for end-of-event (EOE), determines to start new script or continue

        public Events()
        {
            advance = false;
            eoe = true;

            tStack = new Stack<XmlNode>();

            if (Globals.screenBounds.X / Globals.screenBounds.Y > 76.0 / 45.0)              // Average aspect ratio between 16:9 and 16:10
            {
                uiSize = new Vector2(48, 27);
                topLine = 19.0f;
                textOffset = 16;
                textLength = 27;
                ratio = "16-9";
            }
            else if (Globals.screenBounds.X / Globals.screenBounds.Y > 22.0 / 15.0)         // Average aspect ratio between 4:3 and 16:10
            {
                uiSize = new Vector2(48, 27);
                topLine = 19.7f;
                textOffset = 16;
                textLength = 27;
                ratio = "16-10";
            }
            else                                                                            // Average between 16:10 and 4:3 and under
            {
                uiSize = new Vector2(32, 24);
                topLine = 17.0f;
                textOffset = 12;
                textLength = 16;
                ratio = "4-3";
            }
        }

        public bool Parse(string script)
        {
            if (!Globals.sleep)
            {
                if (eoe)                         // Previous event ended, or none have started, read new event
                {
                    reader = new XmlDocument();
                    string filename = "Maps/" + script + ".xml";
                    if (Globals.fileOpen)
                        filename = "Saves/" + Globals.slotOpen + "/" + filename;
                    reader.Load(filename);
                    commands = reader.GetElementsByTagName("root");
                    t = commands[0].FirstChild;

                    eoe = false;
                    line = "";

                    Globals.fade = new Fade();

                    Globals.sleep = false;
                }

                if (advance)                    // Ready to advance, parse next line
                {
                    advance = false;
                    if (tStack.Count == 0 && commands[0].LastChild != t)
                    {
                        t = t.NextSibling;
                    }
                    else if (tStack.Count > 0 && tStack.Peek().LastChild != t)
                    {
                        t = t.NextSibling;
                    }
                    else if (tStack.Count > 0)
                    {
                        while (tStack.Count > 0 && tStack.Peek().LastChild == t)
                        {
                            t = tStack.Pop();
                        }
                        if (commands[0].LastChild == t)
                        {
                            eoe = true;
                            Globals.pause = false;
                            return false;
                        }
                        t = t.NextSibling;
                    }
                    else
                    {
                        eoe = true;
                        Globals.pause = false;
                        return false;
                    }

                    if (t.Name == "fadefrom")
                        Globals.fade.level = 1f;
                }
            }

            Globals.pause = false;

            if (t.Name == "if")
            {
                bool stack = true;

                for (int i = 0; i < t.Attributes.Count; i++)
                {
                    if (t.Attributes[i].Name == "has" && Globals.keyItems.ContainsKey(t.Attributes[i].Value) && !Globals.keyItems[t.Attributes[i].Value])
                        stack = false;
                    if (t.Attributes[i].Name == "flagged" && Globals.flags.ContainsKey(t.Attributes[i].Value) && !Globals.flags[t.Attributes[i].Value])
                        stack = false;
                }

                if (stack)
                {
                    tStack.Push(t);
                    t = t.FirstChild;
                }
            }
            if (t.Name == "nif")
            {
                bool stack = true;

                for (int i = 0; i < t.Attributes.Count; i++)
                {
                    if (t.Attributes[i].Name == "has" && Globals.keyItems.ContainsKey(t.Attributes[i].Value) && Globals.keyItems[t.Attributes[i].Value])
                        stack = false;
                    if (t.Attributes[i].Name == "flagged" && Globals.flags.ContainsKey(t.Attributes[i].Value) && Globals.flags[t.Attributes[i].Value])
                        stack = false;
                }

                if (stack)
                {
                    tStack.Push(t);
                    t = t.FirstChild;
                }
            }
            if (t.Name == "cutscene")
            {
                profile = Globals.Content.Load<Texture2D>("Assets/full/" + Globals.currentPlayer);
                profileTo = Globals.Content.Load<Texture2D>("Assets/full/" + Globals.currentPlayer);
                speaker = Globals.currentPlayer;
                ifTo = false;
                for (int i = 0; i < t.Attributes.Count; i++)
                {
                    if (t.Attributes[i].Name == "speaker" && t.Attributes[i].Value != "this")
                        speaker = t.Attributes[i].Value;
                    if (t.Attributes[i].Name == "from" && t.Attributes[i].Value != "this")
                        profile = Globals.Content.Load<Texture2D>("Assets/full/" + t.Attributes[i].Value);
                    if (t.Attributes[i].Name == "to")
                    {
                        ifTo = true;
                        if (t.Attributes[i].Value != "this")
                            profileTo = Globals.Content.Load<Texture2D>("Assets/full/" + t.Attributes[i].Value);
                    }
                    if (t.Attributes[i].Name == "audio")
                    {
                        dialogue = Globals.Content.Load<SoundEffect>("Audio/" + t.Attributes[i].Value);
                        lineInst = dialogue.CreateInstance();
                        lineInst.Stop();
                        lineInst.Play();
                    }
                }
                line = t.InnerText;
                Globals.pause = true;
            }
            if (t.Name == "message")
            {
                profile = Globals.Content.Load<Texture2D>("Assets/pixel");
                speaker = "";
                for (int i = 0; i < t.Attributes.Count; i++)
                {
                    if (t.Attributes[i].Name == "from")
                    {
                        if (t.Attributes[i].Value != "this")
                        {
                            profile = Globals.Content.Load<Texture2D>("Assets/message/" + t.Attributes[i].Value);
                            speaker = t.Attributes[i].Value;
                        }
                        else
                        {
                            profile = Globals.Content.Load<Texture2D>("Assets/message/" + Globals.currentPlayer);
                            speaker = Globals.currentPlayer;
                        }
                    }
                }
                line = t.InnerText;
                Globals.pause = true;
            }
            if (t.Name == "give")
            {
                if (Globals.keyItems.ContainsKey(t.InnerText))
                    Globals.keyItems[t.InnerText] = !Globals.keyItems[t.InnerText];
            }
            if (t.Name == "flag")
            {
                if (Globals.flags.ContainsKey(t.InnerText))
                    Globals.flags[t.InnerText] = !Globals.flags[t.InnerText];
            }
            if (t.Name == "warp")
            {
                Vector2 startPos = new Vector2();
                for (int i = 0; i < t.Attributes.Count; i++)
                {
                    if (t.Attributes[i].Name == "x")
                        startPos.X = Convert.ToInt32(t.Attributes[i].Value);
                    if (t.Attributes[i].Name == "y")
                        startPos.Y = Convert.ToInt32(t.Attributes[i].Value);
                }
                Map.changeMap(t.InnerText, startPos);
            }
            if (t.Name == "fadeto")
            {
                if (t.InnerText != "nil")
                    Globals.fade.delay = Convert.ToInt32(t.InnerText);
                for (int i = 0; i < t.Attributes.Count; i++)
                {
                    if (t.Attributes[i].Name == "r")
                        Globals.fade.color.R = Convert.ToByte(t.Attributes[i].Value);
                    if (t.Attributes[i].Name == "g")
                        Globals.fade.color.G = Convert.ToByte(t.Attributes[i].Value);
                    if (t.Attributes[i].Name == "b")
                        Globals.fade.color.B = Convert.ToByte(t.Attributes[i].Value);
                    if (t.Attributes[i].Name == "step")
                        Globals.fade.step = (float)double.Parse(t.Attributes[i].Value);
                }
                Globals.sleep = true;

                Globals.timer.Start();
                if (Globals.timer.ElapsedMilliseconds >= Globals.fade.delay)
                {
                    Globals.fade.stepUp();
                    Globals.timer.Restart();
                    if (Globals.fade.level >= 1.0f)
                    {
                        Globals.fade.level = 1.0f;
                        Globals.sleep = false;
                        Globals.timer.Reset();
                    }
                }
            }
            if (t.Name == "fadefrom")
            {
                if (t.InnerText != "nil")
                    Globals.fade.delay = Convert.ToInt32(t.InnerText);
                for (int i = 0; i < t.Attributes.Count; i++)
                {
                    if (t.Attributes[i].Name == "r")
                        Globals.fade.color.R = Convert.ToByte(t.Attributes[i].Value);
                    if (t.Attributes[i].Name == "g")
                        Globals.fade.color.G = Convert.ToByte(t.Attributes[i].Value);
                    if (t.Attributes[i].Name == "b")
                        Globals.fade.color.B = Convert.ToByte(t.Attributes[i].Value);
                    if (t.Attributes[i].Name == "step")
                        Globals.fade.step = (float)double.Parse(t.Attributes[i].Value);
                }
                Globals.sleep = true;

                Globals.timer.Start();
                if (Globals.timer.ElapsedMilliseconds >= Globals.fade.delay)
                {
                    Globals.fade.stepDown();
                    Globals.timer.Restart();
                    if (Globals.fade.level <= 0f)
                    {
                        Globals.fade.level = 0f;
                        Globals.sleep = false;
                        Globals.timer.Reset();
                    }
                }
            }
            if (t.Name == "shove")
            {
                if (t.InnerText == "up")
                {
                    Globals.spriteFace = Globals.SpriteFace.Up;
                    Globals.step = Globals.spriteSrcUp.Count;
                    Globals.moving = true;
                    Globals.timer.Start();
                }
                if (t.InnerText == "down")
                {
                    Globals.spriteFace = Globals.SpriteFace.Down;
                    Globals.step = Globals.spriteSrcDown.Count;
                    Globals.moving = true;
                    Globals.timer.Start();
                }
                if (t.InnerText == "left")
                {
                    Globals.spriteFace = Globals.SpriteFace.Left;
                    Globals.step = Globals.spriteSrcLeft.Count;
                    Globals.moving = true;
                    Globals.timer.Start();
                }
                if (t.InnerText == "right")
                {
                    Globals.spriteFace = Globals.SpriteFace.Right;
                    Globals.step = Globals.spriteSrcRight.Count;
                    Globals.moving = true;
                    Globals.timer.Start();
                }
            }
            if (t.Name == "retile")
            {
                string mapName = Globals.currentMap;
                Vector2 pos = Vector2.Zero;
                Vector2 newset = Vector2.Zero;
                bool overlay = false;
                for (int i = 0; i < t.Attributes.Count; i++)
                {
                    if (t.Attributes[i].Name == "x")
                        pos.X = Convert.ToInt32(t.Attributes[i].Value);
                    if (t.Attributes[i].Name == "y")
                        pos.Y = Convert.ToInt32(t.Attributes[i].Value);
                    if (t.Attributes[i].Name == "overlay" && t.Attributes[i].Value == "yes")
                        overlay = true;
                    if (t.Attributes[i].Name == "newx")
                        newset.X = Convert.ToInt32(t.Attributes[i].Value);
                    if (t.Attributes[i].Name == "newy")
                        newset.Y = Convert.ToInt32(t.Attributes[i].Value);
                }
                if (t.InnerText != "this")
                    mapName = t.InnerText;

                Map.modMapTile(mapName, pos, newset, overlay);

                if (mapName == Globals.currentMap && !overlay)
                    Globals.map[Convert.ToInt32(pos.Y)][Convert.ToInt32(pos.X)] = newset;
                else if (mapName == Globals.currentMap && overlay)
                    Globals.overlayMap[Convert.ToInt32(pos.Y)][Convert.ToInt32(pos.X)] = newset;
            }
            if (t.Name == "reperm")
            {
                string mapName = Globals.currentMap;
                Vector2 pos = Vector2.Zero;
                Globals.Permissions newperm = Globals.Permissions.Open;
                bool overlay = false;
                for (int i = 0; i < t.Attributes.Count; i++)
                {
                    if (t.Attributes[i].Name == "x")
                        pos.X = Convert.ToInt32(t.Attributes[i].Value);
                    if (t.Attributes[i].Name == "y")
                        pos.Y = Convert.ToInt32(t.Attributes[i].Value);
                    if (t.Attributes[i].Name == "overlay" && t.Attributes[i].Value == "yes")
                        overlay = true;
                    if (t.Attributes[i].Name == "perm")
                        newperm = (Globals.Permissions)Enum.Parse(typeof(Globals.Permissions), t.Attributes[i].Value);
                }
                if (t.InnerText != "this")
                    mapName = t.InnerText;

                Map.modMapPerm(mapName, pos, newperm, overlay);

                if (mapName == Globals.currentMap && !overlay)
                    Globals.permissions[Convert.ToInt32(pos.Y)][Convert.ToInt32(pos.X)] = newperm;
                else if (mapName == Globals.currentMap && overlay)
                    Globals.overlayPerms[Convert.ToInt32(pos.Y)][Convert.ToInt32(pos.X)] = newperm;
            }
            if (t.Name == "reevent")
            {
                string mapName = Globals.currentMap;
                Vector2 pos = Vector2.Zero;
                string thrower = "None";
                string eventscript = "nil";
                for (int i = 0; i < t.Attributes.Count; i++)
                {
                    if (t.Attributes[i].Name == "x")
                        pos.X = Convert.ToInt32(t.Attributes[i].Value);
                    if (t.Attributes[i].Name == "y")
                        pos.Y = Convert.ToInt32(t.Attributes[i].Value);
                    if (t.Attributes[i].Name == "throw")
                        thrower = t.Attributes[i].Value;
                    if (t.Attributes[i].Name == "script")
                        eventscript = t.Attributes[i].Value;
                }
                if (t.InnerText != "this")
                    mapName = t.InnerText;

                Map.modMapEvent(mapName, pos, thrower, eventscript);

                if (mapName == Globals.currentMap)
                {
                    Globals.eventThrows[Convert.ToInt32(pos.Y)][Convert.ToInt32(pos.X)] = (Globals.EventThrow)Enum.Parse(typeof(Globals.EventThrow), thrower);
                    Globals.events[Convert.ToInt32(pos.Y)][Convert.ToInt32(pos.X)] = eventscript;
                }
            }
            if (t.Name == "reenemy")
            {
                string mapName = Globals.currentMap;
                Vector2 pos = Vector2.Zero;
                string src = "Paul";
                Globals.SpriteFace facing = Globals.SpriteFace.Down;
                List<Globals.battleEnemyTypes> types = new List<Globals.battleEnemyTypes>();
                for (int i = 0; i < t.Attributes.Count; i++)
                {
                    if (t.Attributes[i].Name == "x")
                        pos.X = Convert.ToInt32(t.Attributes[i].Value);
                    if (t.Attributes[i].Name == "y")
                        pos.Y = Convert.ToInt32(t.Attributes[i].Value);
                    if (t.Attributes[i].Name == "src")
                        src = t.Attributes[i].Value;
                    if (t.Attributes[i].Name == "facing")
                        facing = (Globals.SpriteFace)Enum.Parse(typeof(Globals.SpriteFace), t.Attributes[i].Value);
                    if (t.Attributes[i].Name == "map" && t.Attributes[i].Value != "this")
                        mapName = t.Attributes[i].Value;
                }
                for (int i = 0; i < t.ChildNodes.Count; i++)
                {
                    types.Add((Globals.battleEnemyTypes)Enum.Parse(typeof(Globals.battleEnemyTypes), t.ChildNodes[i].InnerText));
                }

                Map.modMapEnemy(mapName, pos, src, facing, types);
            }
            if (t.Name == "remenemy")
            {
                string mapName = Globals.currentMap;
                Vector2 pos = Vector2.Zero;
                for (int i = 0; i < t.Attributes.Count; i++)
                {
                    if (t.Attributes[i].Name == "x")
                        pos.X = Convert.ToInt32(t.Attributes[i].Value);
                    if (t.Attributes[i].Name == "y")
                        pos.Y = Convert.ToInt32(t.Attributes[i].Value);
                }
                if (t.InnerText != "this")
                    mapName = t.InnerText;

                Map.modMapEnemy(mapName, pos);
            }

            advance = true;
            return true;
        }

        public void Draw()
        {
            Globals.spriteBatch.Draw(Globals.Content.Load<Texture2D>("Assets/pixel"), new Rectangle(0, 0, Convert.ToInt32(Globals.screenBounds.X), Convert.ToInt32(Globals.screenBounds.Y)), Globals.fade.color * Globals.fade.level);
            if (t.Name == "cutscene")
            {
                SpriteFont font = Globals.Content.Load<SpriteFont>("Fonts/font");
                string[] lines = { "", "", "" };
                string[] words = line.Split(' ');
                int lineAt = 0;
                foreach (string i in words)
                {
                    if (font.MeasureString(lines[lineAt] + i + " ").X <= Globals.screenBounds.X / uiSize.X * cutTextLen)
                    {
                        lines[lineAt] += i + " ";
                    }
                    else if (lineAt < 2)
                    {
                        lineAt++;
                        lines[lineAt] += i + " ";
                    }
                }
                Globals.spriteBatch.Draw(profile, new Rectangle(Convert.ToInt32(Globals.screenBounds.X / uiSize.X * 8), Convert.ToInt32(Globals.screenBounds.Y / uiSize.Y * 1), Convert.ToInt32((profile.Width * Globals.screenBounds.Y / uiSize.Y * 17) / profile.Height), Convert.ToInt32(Globals.screenBounds.Y / uiSize.Y * 17)), Color.White);
                if (ifTo)
                    Globals.spriteBatch.Draw(profileTo, new Rectangle(Convert.ToInt32(Globals.screenBounds.X / uiSize.X * 28), Convert.ToInt32(Globals.screenBounds.Y / uiSize.Y * 1), Convert.ToInt32((profileTo.Width * Globals.screenBounds.Y / uiSize.Y * 17) / profileTo.Height), Convert.ToInt32(Globals.screenBounds.Y / uiSize.Y * 17)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);

                Globals.spriteBatch.Draw(Globals.Content.Load<Texture2D>("Assets/" + ratio + "/uicutscene"), new Rectangle(0, 0, Convert.ToInt32(Globals.screenBounds.X), Convert.ToInt32(Globals.screenBounds.Y)), Color.White);
                Globals.spriteBatch.DrawString(font, speaker, new Vector2(Globals.screenBounds.X / uiSize.X * 1 + ((Globals.screenBounds.X / uiSize.X * 11 - font.MeasureString(speaker).X) / 2), Globals.screenBounds.Y / uiSize.Y * 17.1f), Color.Black);
                Globals.spriteBatch.DrawString(font, lines[0], new Vector2(Globals.screenBounds.X / uiSize.X * cutTextOff, (Globals.screenBounds.Y / uiSize.Y) * cutTopLine + ((Globals.screenBounds.Y / uiSize.Y) - font.MeasureString(lines[0]).Y) / 2), Color.White);
                Globals.spriteBatch.DrawString(font, lines[1], new Vector2(Globals.screenBounds.X / uiSize.X * cutTextOff, (Globals.screenBounds.Y / uiSize.Y) * (cutTopLine + 1) + ((Globals.screenBounds.Y / uiSize.Y) - font.MeasureString(lines[0]).Y) / 2), Color.White);
                Globals.spriteBatch.DrawString(font, lines[2], new Vector2(Globals.screenBounds.X / uiSize.X * cutTextOff, (Globals.screenBounds.Y / uiSize.Y) * (cutTopLine + 2) + ((Globals.screenBounds.Y / uiSize.Y) - font.MeasureString(lines[0]).Y) / 2), Color.White);
            }
            if (t.Name == "message")
            {
                SpriteFont font = Globals.Content.Load<SpriteFont>("Fonts/font");
                string[] lines = { "", "", "", "", "" };
                string[] words = line.Split(' ');
                int lineAt = 0;
                foreach (string i in words)
                {
                    if (font.MeasureString(lines[lineAt] + i + " ").X <= Globals.screenBounds.X / uiSize.X * textLength)
                    {
                        lines[lineAt] += i + " ";
                    }
                    else if (lineAt < 4)
                    {
                        lineAt++;
                        lines[lineAt] += i + " ";
                    }
                }
                Globals.spriteBatch.Draw(Globals.Content.Load<Texture2D>("Assets/" + ratio + "/uidialog"), new Rectangle(0, 0, Convert.ToInt32(Globals.screenBounds.X), Convert.ToInt32(Globals.screenBounds.Y)), Color.White);
                Globals.spriteBatch.DrawString(font, lines[0], new Vector2(Globals.screenBounds.X / uiSize.X * textOffset, (Globals.screenBounds.Y / uiSize.Y) * topLine + ((Globals.screenBounds.Y / uiSize.Y) - font.MeasureString(lines[0]).Y) / 2), Color.White);
                Globals.spriteBatch.DrawString(font, lines[1], new Vector2(Globals.screenBounds.X / uiSize.X * textOffset, (Globals.screenBounds.Y / uiSize.Y) * (topLine + 1) + ((Globals.screenBounds.Y / uiSize.Y) - font.MeasureString(lines[0]).Y) / 2), Color.White);
                Globals.spriteBatch.DrawString(font, lines[2], new Vector2(Globals.screenBounds.X / uiSize.X * textOffset, (Globals.screenBounds.Y / uiSize.Y) * (topLine + 2) + ((Globals.screenBounds.Y / uiSize.Y) - font.MeasureString(lines[0]).Y) / 2), Color.White);
                Globals.spriteBatch.DrawString(font, lines[3], new Vector2(Globals.screenBounds.X / uiSize.X * textOffset, (Globals.screenBounds.Y / uiSize.Y) * (topLine + 3) + ((Globals.screenBounds.Y / uiSize.Y) - font.MeasureString(lines[0]).Y) / 2), Color.White);
                Globals.spriteBatch.DrawString(font, lines[4], new Vector2(Globals.screenBounds.X / uiSize.X * textOffset, (Globals.screenBounds.Y / uiSize.Y) * (topLine + 4) + ((Globals.screenBounds.Y / uiSize.Y) - font.MeasureString(lines[0]).Y) / 2), Color.White);

                if (ratio == "16-9")
                {
                    if (speaker != "")
                    {
                        Globals.spriteBatch.DrawString(font, speaker, new Vector2(Globals.screenBounds.X / uiSize.X * 1 + ((Globals.screenBounds.X / uiSize.X * 11 - font.MeasureString(speaker).X) / 2), Globals.screenBounds.Y / uiSize.Y * 24.1f), Color.Black);
                        Globals.spriteBatch.Draw(profile, new Rectangle(Convert.ToInt32(Globals.screenBounds.X / uiSize.X), Convert.ToInt32(Globals.screenBounds.Y / uiSize.Y * 15), Convert.ToInt32(Globals.screenBounds.X / uiSize.X * 11), Convert.ToInt32(Globals.screenBounds.Y / uiSize.Y * 7)), Color.White);
                    }
                    else
                        Globals.spriteBatch.Draw(profile, new Rectangle(Convert.ToInt32(Globals.screenBounds.X / uiSize.X), Convert.ToInt32(Globals.screenBounds.Y / uiSize.Y * 15), Convert.ToInt32(Globals.screenBounds.X / uiSize.X * 11), Convert.ToInt32(Globals.screenBounds.Y / uiSize.Y * 7)), new Color(57, 57, 57));
                }
                else if (ratio == "16-10")
                {
                    Globals.spriteBatch.Draw(profile, new Rectangle(Convert.ToInt32(Globals.screenBounds.X / uiSize.X), Convert.ToInt32(Globals.screenBounds.Y / uiSize.Y * 18), Convert.ToInt32(Globals.screenBounds.X / uiSize.X * 11), Convert.ToInt32(Globals.screenBounds.Y / uiSize.Y * 7)), Color.White);
                }
                else
                {
                    Globals.spriteBatch.Draw(profile, new Rectangle(Convert.ToInt32(Globals.screenBounds.X / uiSize.X), Convert.ToInt32(Globals.screenBounds.Y / uiSize.Y * 14), Convert.ToInt32(Globals.screenBounds.X / uiSize.X * 7), Convert.ToInt32(Globals.screenBounds.Y / uiSize.Y * 4)), Color.White);
                }
            }
        }
    }
}
