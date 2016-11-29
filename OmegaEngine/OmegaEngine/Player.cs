using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OmegaEngine
{
    public class Player
    {
        Texture2D sprites;

        int count;

        // Flag for ongoing events
        bool eventRunning = false;
        string tempEvent;

        // No overloaded methods yet, but initialize with the resolution bounds
        public Player(string spriteMap)
        {
            Globals.currentPlayer = spriteMap;
            sprites = Globals.Content.Load<Texture2D>("Assets/" + Globals.currentPlayer);

            Globals.scripts = new Events();

            Globals.position = Globals.startPoint;
            Globals.offset = new Vector2(Globals.map[0].Count / 2 - Globals.startPoint.X, Globals.map.Count / 2 - Globals.startPoint.Y);
            if (Globals.map[0].Count % 2 == 0)
                Globals.offset.X -= .5f;
            if (Globals.map.Count % 2 == 0)
                Globals.offset.Y -= .5f;

            Globals.moving = false;
            Globals.timer.Reset();
            count = 0;
        }

        public static void updatePos()
        {
            Globals.position = Globals.startPoint;
            Globals.offset = new Vector2(Globals.map[0].Count / 2 - Globals.startPoint.X, Globals.map.Count / 2 - Globals.startPoint.Y);
            if (Globals.map[0].Count % 2 == 0)
                Globals.offset.X -= .5f;
            if (Globals.map.Count % 2 == 0)
                Globals.offset.Y -= .5f;
        }

        public void playerSpriteMap(string spriteMap)
        {
            Globals.currentPlayer = spriteMap;
            sprites = Globals.Content.Load<Texture2D>("Assets/" + Globals.currentPlayer);
        }

        public void Update()
        {
            List<int> remove = new List<int>();
            for (int i = 0; i < Globals.enemies.Count; i++)
            {
                Globals.enemies[i].Update();
                if (Math.Round(Globals.position.X) == Math.Round(Globals.enemies[i].position.X) && Math.Round(Globals.position.Y) == Math.Round(Globals.enemies[i].position.Y))
                {
                    remove.Insert(0, i);
                    Globals.isBattling = true;
                    /***************************************
					 * IMPORTANT: INITIATE BATTLE HERE!!!
					 * ************************************/
                    tempEvent = "firstenemy";
                    eventRunning = Globals.scripts.Parse(tempEvent);
                }
            }
            foreach (int i in remove)
                Globals.enemies.RemoveAt(i);
            // Gets input only if done moving
            if (!Globals.moving)
            {
                Globals.keyPress = Keyboard.GetState();
                Globals.gamePad = GamePad.GetState(PlayerIndex.One);

                /********************************************************   DEBUG CONTROLS  *************************************************************/
                if (Globals.keyPress.IsKeyDown(Keys.OemTilde) && Globals.prevKeyPress.IsKeyUp(Keys.OemTilde))
                    System.Windows.Forms.MessageBox.Show("Player position: (" + Math.Round(Globals.position.X) + ", " + Math.Round(Globals.position.Y) + ")\nWindow bounds: (" + Globals.windowBounds.X + ", " + Globals.windowBounds.Y + ")\nPermission: " + Globals.permissions[Convert.ToInt32(Math.Round(Globals.position.Y))][Convert.ToInt32(Math.Round(Globals.position.X))].ToString() + "\nScript: " + Globals.events[Convert.ToInt32(Math.Round(Globals.position.Y))][Convert.ToInt32(Math.Round(Globals.position.X))].ToString());
                /****************************************************************************************************************************************/

                if (!Globals.pause && !Globals.sleep)
                {
                    /************************************************************
                     * Gets input and initializes timer. To avoid dominating conditional input, movement only occurs when one direction is input and all others are released.
                     * **********************************************************/
                    if (eventRunning)
                    {
                        eventRunning = Globals.scripts.Parse(tempEvent);
                    }
                    else if (Globals.eventThrows[Convert.ToInt32(Math.Round(Globals.position.Y))][Convert.ToInt32(Math.Round(Globals.position.X))] == Globals.EventThrow.Stand
                        && Globals.events[Convert.ToInt32(Math.Round(Globals.position.Y))][Convert.ToInt32(Math.Round(Globals.position.X))] != "")
                    {
                        tempEvent = Globals.events[Convert.ToInt32(Math.Round(Globals.position.Y))][Convert.ToInt32(Math.Round(Globals.position.X))];
                        eventRunning = Globals.scripts.Parse(tempEvent);
                    }
                    else if ((Globals.keyPress.IsKeyDown(Keys.E)
                        && Globals.prevKeyPress.IsKeyUp(Keys.E))
                        || (Globals.gamePad.IsButtonDown(Buttons.A)
                        && Globals.prevGamePad.IsButtonUp(Buttons.A)))
                    {
                        tempEvent = "";
                        switch (Globals.spriteFace)
                        {
                            case Globals.SpriteFace.Up:
                                if (Convert.ToInt32(Math.Round(Globals.position.Y) - 1) != -1 && Globals.eventThrows[Convert.ToInt32(Math.Round(Globals.position.Y) - 1)][Convert.ToInt32(Math.Round(Globals.position.X))] == Globals.EventThrow.Interact && Globals.events[Convert.ToInt32(Math.Round(Globals.position.Y) - 1)][Convert.ToInt32(Math.Round(Globals.position.X))] != "")
                                    tempEvent = Globals.events[Convert.ToInt32(Math.Round(Globals.position.Y - 1))][Convert.ToInt32(Math.Round(Globals.position.X))];
                                break;
                            case Globals.SpriteFace.Down:
                                if (Convert.ToInt32(Math.Round(Globals.position.Y) + 1) != Globals.events.Count && Globals.eventThrows[Convert.ToInt32(Math.Round(Globals.position.Y) + 1)][Convert.ToInt32(Math.Round(Globals.position.X))] == Globals.EventThrow.Interact && Globals.events[Convert.ToInt32(Math.Round(Globals.position.Y) + 1)][Convert.ToInt32(Math.Round(Globals.position.X))] != "")
                                    tempEvent = Globals.events[Convert.ToInt32(Math.Round(Globals.position.Y + 1))][Convert.ToInt32(Math.Round(Globals.position.X))];
                                break;
                            case Globals.SpriteFace.Left:
                                if (Convert.ToInt32(Math.Round(Globals.position.X) - 1) != -1 && Globals.eventThrows[Convert.ToInt32(Math.Round(Globals.position.Y))][Convert.ToInt32(Math.Round(Globals.position.X) - 1)] == Globals.EventThrow.Interact && Globals.events[Convert.ToInt32(Math.Round(Globals.position.Y))][Convert.ToInt32(Math.Round(Globals.position.X) - 1)] != "")
                                    tempEvent = Globals.events[Convert.ToInt32(Math.Round(Globals.position.Y))][Convert.ToInt32(Math.Round(Globals.position.X - 1))];
                                break;
                            case Globals.SpriteFace.Right:
                                if (Convert.ToInt32(Math.Round(Globals.position.X) + 1) != Globals.events[0].Count && Globals.eventThrows[Convert.ToInt32(Math.Round(Globals.position.Y))][Convert.ToInt32(Math.Round(Globals.position.X) + 1)] == Globals.EventThrow.Interact && Globals.events[Convert.ToInt32(Math.Round(Globals.position.Y))][Convert.ToInt32(Math.Round(Globals.position.X) + 1)] != "")
                                    tempEvent = Globals.events[Convert.ToInt32(Math.Round(Globals.position.Y))][Convert.ToInt32(Math.Round(Globals.position.X + 1))];
                                break;
                        }
                        if (tempEvent != "")
                        {
                            eventRunning = Globals.scripts.Parse(tempEvent);
                        }
                    }
                    else if ((Globals.keyPress.IsKeyDown(Keys.W)
                        && Globals.keyPress.IsKeyUp(Keys.S)
                        && Globals.keyPress.IsKeyUp(Keys.A)
                        && Globals.keyPress.IsKeyUp(Keys.D))
                        || (Globals.gamePad.DPad.Up == ButtonState.Pressed
                        && Globals.gamePad.DPad.Down == ButtonState.Released
                        && Globals.gamePad.DPad.Left == ButtonState.Released
                        && Globals.gamePad.DPad.Right == ButtonState.Released))
                    {
                        Globals.spriteFace = Globals.SpriteFace.Up;
                        if (Math.Round(Globals.position.Y) > 0
                            && Globals.permissions[Convert.ToInt32(Math.Round(Globals.position.Y) - 1)][Convert.ToInt32(Math.Round(Globals.position.X))] == Globals.Permissions.Open
                            && Globals.overlayPerms[Convert.ToInt32(Math.Round(Globals.position.Y) - 1)][Convert.ToInt32(Math.Round(Globals.position.X))] == Globals.Permissions.Open)
                        {
                            Globals.step = Globals.spriteSrcUp.Count;
                            Globals.moving = true;
                            Globals.timer.Start();
                        }
                        if (Math.Round(Globals.position.Y) > 0
                            && Globals.eventThrows[Convert.ToInt32(Math.Round(Globals.position.Y) - 1)][Convert.ToInt32(Math.Round(Globals.position.X))] == Globals.EventThrow.Push
                            && Globals.events[Convert.ToInt32(Math.Round(Globals.position.Y) - 1)][Convert.ToInt32(Math.Round(Globals.position.X))] != "")
                        {
                            tempEvent = Globals.events[Convert.ToInt32(Math.Round(Globals.position.Y) - 1)][Convert.ToInt32(Math.Round(Globals.position.X))];
                            eventRunning = Globals.scripts.Parse(tempEvent);
                        }
                    }
                    else if ((Globals.keyPress.IsKeyDown(Keys.S)
                        && Globals.keyPress.IsKeyUp(Keys.W)
                        && Globals.keyPress.IsKeyUp(Keys.A)
                        && Globals.keyPress.IsKeyUp(Keys.D))
                        || (Globals.gamePad.DPad.Down == ButtonState.Pressed
                        && Globals.gamePad.DPad.Up == ButtonState.Released
                        && Globals.gamePad.DPad.Left == ButtonState.Released
                        && Globals.gamePad.DPad.Right == ButtonState.Released))
                    {
                        Globals.spriteFace = Globals.SpriteFace.Down;
                        if (Math.Round(Globals.position.Y) < Globals.mapBounds.Height / Globals.bitWidth - 1
                            && Globals.permissions[Convert.ToInt32(Math.Round(Globals.position.Y) + 1)][Convert.ToInt32(Math.Round(Globals.position.X))] == Globals.Permissions.Open
                            && Globals.overlayPerms[Convert.ToInt32(Math.Round(Globals.position.Y) + 1)][Convert.ToInt32(Math.Round(Globals.position.X))] == Globals.Permissions.Open)
                        {
                            Globals.step = Globals.spriteSrcDown.Count;
                            Globals.moving = true;
                            Globals.timer.Start();
                        }
                        if (Math.Round(Globals.position.Y) < Globals.mapBounds.Height / Globals.bitWidth - 1
                            && Globals.eventThrows[Convert.ToInt32(Math.Round(Globals.position.Y) + 1)][Convert.ToInt32(Math.Round(Globals.position.X))] == Globals.EventThrow.Push
                            && Globals.events[Convert.ToInt32(Math.Round(Globals.position.Y) + 1)][Convert.ToInt32(Math.Round(Globals.position.X))] != "")
                        {
                            tempEvent = Globals.events[Convert.ToInt32(Math.Round(Globals.position.Y) + 1)][Convert.ToInt32(Math.Round(Globals.position.X))];
                            eventRunning = Globals.scripts.Parse(tempEvent);
                        }
                    }
                    else if ((Globals.keyPress.IsKeyDown(Keys.A)
                        && Globals.keyPress.IsKeyUp(Keys.W)
                        && Globals.keyPress.IsKeyUp(Keys.S)
                        && Globals.keyPress.IsKeyUp(Keys.D))
                        || (Globals.gamePad.DPad.Left == ButtonState.Pressed
                        && Globals.gamePad.DPad.Up == ButtonState.Released
                        && Globals.gamePad.DPad.Down == ButtonState.Released
                        && Globals.gamePad.DPad.Right == ButtonState.Released))
                    {
                        Globals.spriteFace = Globals.SpriteFace.Left;
                        if (Math.Round(Globals.position.X) > 0
                            && Globals.permissions[Convert.ToInt32(Math.Round(Globals.position.Y))][Convert.ToInt32(Math.Round(Globals.position.X) - 1)] == Globals.Permissions.Open
                            && Globals.overlayPerms[Convert.ToInt32(Math.Round(Globals.position.Y))][Convert.ToInt32(Math.Round(Globals.position.X) - 1)] == Globals.Permissions.Open)
                        {
                            Globals.step = Globals.spriteSrcLeft.Count;
                            Globals.moving = true;
                            Globals.timer.Start();
                        }
                        if (Math.Round(Globals.position.X) > 0
                            && Globals.eventThrows[Convert.ToInt32(Math.Round(Globals.position.Y))][Convert.ToInt32(Math.Round(Globals.position.X) - 1)] == Globals.EventThrow.Push
                            && Globals.events[Convert.ToInt32(Math.Round(Globals.position.Y))][Convert.ToInt32(Math.Round(Globals.position.X) - 1)] != "")
                        {
                            tempEvent = Globals.events[Convert.ToInt32(Math.Round(Globals.position.Y))][Convert.ToInt32(Math.Round(Globals.position.X) - 1)];
                            eventRunning = Globals.scripts.Parse(tempEvent);
                        }
                    }
                    else if ((Globals.keyPress.IsKeyDown(Keys.D)
                        && Globals.keyPress.IsKeyUp(Keys.W)
                        && Globals.keyPress.IsKeyUp(Keys.S)
                        && Globals.keyPress.IsKeyUp(Keys.A))
                        || (Globals.gamePad.DPad.Right == ButtonState.Pressed
                        && Globals.gamePad.DPad.Up == ButtonState.Released
                        && Globals.gamePad.DPad.Down == ButtonState.Released
                        && Globals.gamePad.DPad.Left == ButtonState.Released))
                    {
                        Globals.spriteFace = Globals.SpriteFace.Right;
                        if (Math.Round(Globals.position.X) < Globals.mapBounds.Width / Globals.bitWidth - 1
                            && Globals.permissions[Convert.ToInt32(Math.Round(Globals.position.Y))][Convert.ToInt32(Math.Round(Globals.position.X) + 1)] == Globals.Permissions.Open
                            && Globals.overlayPerms[Convert.ToInt32(Math.Round(Globals.position.Y))][Convert.ToInt32(Math.Round(Globals.position.X) + 1)] == Globals.Permissions.Open)
                        {
                            Globals.step = Globals.spriteSrcRight.Count;
                            Globals.moving = true;
                            Globals.timer.Start();
                        }
                        if (Math.Round(Globals.position.X) < Globals.mapBounds.Width / Globals.bitWidth - 1
                            && Globals.eventThrows[Convert.ToInt32(Math.Round(Globals.position.Y))][Convert.ToInt32(Math.Round(Globals.position.X) + 1)] == Globals.EventThrow.Push
                            && Globals.events[Convert.ToInt32(Math.Round(Globals.position.Y))][Convert.ToInt32(Math.Round(Globals.position.X) + 1)] != "")
                        {
                            tempEvent = Globals.events[Convert.ToInt32(Math.Round(Globals.position.Y))][Convert.ToInt32(Math.Round(Globals.position.X) + 1)];
                            eventRunning = Globals.scripts.Parse(tempEvent);
                        }
                    }
                }
                else if (!Globals.sleep)
                {
                    if ((Globals.keyPress.IsKeyDown(Keys.E) && Globals.prevKeyPress.IsKeyUp(Keys.E)) || (Globals.gamePad.IsButtonDown(Buttons.A) && Globals.prevGamePad.IsButtonUp(Buttons.A)))
                        eventRunning = Globals.scripts.Parse(tempEvent);
                }
                else
                {
                    eventRunning = Globals.scripts.Parse(tempEvent);
                }

                Globals.prevKeyPress = Globals.keyPress;
                Globals.prevGamePad = Globals.gamePad;
            }
            // Moves player, but only if the timer reaches delay time
            else if (Globals.timer.ElapsedMilliseconds >= Globals.STEP_DELAY)
            {
                if (Globals.spriteFace == Globals.SpriteFace.Up)
                {
                    Globals.position.Y -= (float)1 / Globals.spriteSrcUp.Count;
                    Globals.offset.Y += (float)1 / Globals.spriteSrcUp.Count;
                }
                if (Globals.spriteFace == Globals.SpriteFace.Down)
                {
                    Globals.position.Y += (float)1 / Globals.spriteSrcDown.Count;
                    Globals.offset.Y -= (float)1 / Globals.spriteSrcUp.Count;
                }
                if (Globals.spriteFace == Globals.SpriteFace.Left)
                {
                    Globals.position.X -= (float)1 / Globals.spriteSrcLeft.Count;
                    Globals.offset.X += (float)1 / Globals.spriteSrcLeft.Count;
                }
                if (Globals.spriteFace == Globals.SpriteFace.Right)
                {
                    Globals.position.X += (float)1 / Globals.spriteSrcRight.Count;
                    Globals.offset.X -= (float)1 / Globals.spriteSrcRight.Count;
                }

                count++;
                Globals.step--;
                Globals.timer.Restart();
                // When movement is done
                if (Globals.step == 0)
                {
                    Globals.moving = false;
                    count = 0;
                    Globals.timer.Reset();
                }
            }
        }

        // Returns source frame to draw from in specific animation points
        public Vector2 getFrameSrc()
        {
            if (Globals.spriteFace == Globals.SpriteFace.Down)
            {
                return Globals.spriteSrcDown[count];
            }
            else if (Globals.spriteFace == Globals.SpriteFace.Up)
            {
                return Globals.spriteSrcUp[count];
            }
            else if (Globals.spriteFace == Globals.SpriteFace.Left)
            {
                return Globals.spriteSrcLeft[count];
            }
            else
            {
                return Globals.spriteSrcRight[count];
            }
        }

        public void Draw()
        {
            Globals.spriteBatch.Draw(sprites, new Vector2(Globals.screenBounds.X / 2 - (Globals.bitWidth / 2), Globals.screenBounds.Y / 2 - (Globals.bitWidth / 2)), new Rectangle((int)getFrameSrc().X * Globals.bitWidth, (int)getFrameSrc().Y * Globals.bitWidth, Globals.bitWidth, Globals.bitWidth), Color.White);
        }

        public void DrawEvents()
        {
            if (eventRunning)
            {
                Globals.scripts.Draw();
            }
        }
    }
}
