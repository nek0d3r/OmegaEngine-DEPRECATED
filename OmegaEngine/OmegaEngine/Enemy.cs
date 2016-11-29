using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OmegaEngine
{
    public class Enemy
    {
        public Texture2D sprites;
        public Globals.SpriteFace spriteFace;

        int count;

        public Vector2 position;
        public bool moving;
        public int step;
        public int delay;
        public bool aggro;
        public int aggroDelay;
        public int aggroRadius;

        public Stopwatch timer = new Stopwatch();
        public Random rand = new Random();

        public List<Globals.battleEnemyTypes> types = new List<Globals.battleEnemyTypes>();

        public Enemy(string spriteMap, Vector2 pos, Globals.SpriteFace sprFace, Globals.battleEnemyTypes type)
        {
            sprites = Globals.Content.Load<Texture2D>("Assets/" + spriteMap);
            spriteFace = sprFace;

            position = pos;

            moving = false;
            timer.Restart();
            count = 0;
            delay = 500;
            aggro = false;
            aggroDelay = 100;
            aggroRadius = 5;

            types.Add(type);
        }

        public void addEnemy(Globals.battleEnemyTypes type)
        {
            types.Add(type);
        }

        public void enemySpriteMap(string spriteMap)
        {
            sprites = Globals.Content.Load<Texture2D>("Assets/" + spriteMap);
        }

        public void Update()
        {
            if (!moving)
            {
                if (!Globals.pause && !Globals.sleep)
                {
                    int direction = 5;
                    if (!aggro && timer.ElapsedMilliseconds >= delay)
                    {
                        if (Math.Round(Math.Abs(Globals.position.X - position.X) + Math.Abs(Globals.position.Y - position.Y)) <= aggroRadius)
                            aggro = true;
                        direction = rand.Next(5);
                    }
                    else if (aggro && timer.ElapsedMilliseconds >= aggroDelay)
                    {
                        if (Math.Round(Math.Abs(Globals.position.X - position.X)) >= Math.Round(Math.Abs(Globals.position.Y - position.Y)))
                        {
                            if (Globals.position.X > position.X)
                                direction = 3;
                            else
                                direction = 2;
                        }
                        else if (Math.Round(Math.Abs(Globals.position.Y - position.Y)) > Math.Round(Math.Abs(Globals.position.X - position.X)))
                        {
                            if (Globals.position.Y > position.Y)
                                direction = 1;
                            else
                                direction = 0;
                        }

                        if (Math.Round(Globals.position.X) == Math.Round(position.X) && Math.Round(Globals.position.Y) == Math.Round(position.Y))
                        {
                            direction = 5;
                        }
                    }
                    if ((!aggro && timer.ElapsedMilliseconds >= delay) || (aggro && timer.ElapsedMilliseconds >= aggroDelay))
                    {
                        switch (direction)
                        {
                            case 0:                     // Up
                                spriteFace = Globals.SpriteFace.Up;
                                if (Math.Round(position.Y) > 0 && Globals.permissions[Convert.ToInt32(Math.Round(position.Y) - 1)][Convert.ToInt32(Math.Round(position.X))] == Globals.Permissions.Open && Globals.overlayPerms[Convert.ToInt32(Math.Round(position.Y) - 1)][Convert.ToInt32(Math.Round(position.X))] == Globals.Permissions.Open)
                                {
                                    step = Globals.spriteSrcUp.Count;
                                    moving = true;
                                    timer.Start();
                                }
                                break;
                            case 1:                     // Down
                                spriteFace = Globals.SpriteFace.Down;
                                if (Math.Round(position.Y) < Globals.mapBounds.Height / Globals.bitWidth - 1 && Globals.permissions[Convert.ToInt32(Math.Round(position.Y) + 1)][Convert.ToInt32(Math.Round(position.X))] == Globals.Permissions.Open && Globals.overlayPerms[Convert.ToInt32(Math.Round(position.Y) + 1)][Convert.ToInt32(Math.Round(position.X))] == Globals.Permissions.Open)
                                {
                                    step = Globals.spriteSrcDown.Count;
                                    moving = true;
                                    timer.Start();
                                }
                                break;
                            case 2:                     // Left
                                spriteFace = Globals.SpriteFace.Left;
                                if (Math.Round(position.X) > 0 && Globals.permissions[Convert.ToInt32(Math.Round(position.Y))][Convert.ToInt32(Math.Round(position.X) - 1)] == Globals.Permissions.Open && Globals.overlayPerms[Convert.ToInt32(Math.Round(position.Y))][Convert.ToInt32(Math.Round(position.X) - 1)] == Globals.Permissions.Open)
                                {
                                    step = Globals.spriteSrcLeft.Count;
                                    moving = true;
                                    timer.Start();
                                }
                                break;
                            case 3:                     // Right
                                spriteFace = Globals.SpriteFace.Right;
                                if (Math.Round(position.X) < Globals.mapBounds.Width / Globals.bitWidth - 1 && Globals.permissions[Convert.ToInt32(Math.Round(position.Y))][Convert.ToInt32(Math.Round(position.X) + 1)] == Globals.Permissions.Open && Globals.overlayPerms[Convert.ToInt32(Math.Round(position.Y))][Convert.ToInt32(Math.Round(position.X) + 1)] == Globals.Permissions.Open)
                                {
                                    step = Globals.spriteSrcRight.Count;
                                    moving = true;
                                    timer.Start();
                                }
                                break;
                            default:
                                timer.Restart();
                                break;
                        }
                    }
                }
            }
            else if (timer.ElapsedMilliseconds >= Globals.STEP_DELAY)
            {
                if (spriteFace == Globals.SpriteFace.Up)
                {
                    position.Y -= (float)1 / Globals.spriteSrcUp.Count;
                }
                if (spriteFace == Globals.SpriteFace.Down)
                {
                    position.Y += (float)1 / Globals.spriteSrcDown.Count;
                }
                if (spriteFace == Globals.SpriteFace.Left)
                {
                    position.X -= (float)1 / Globals.spriteSrcLeft.Count;
                }
                if (spriteFace == Globals.SpriteFace.Right)
                {
                    position.X += (float)1 / Globals.spriteSrcRight.Count;
                }

                count++;
                step--;
                timer.Restart();
                // When movement is done
                if (step == 0)
                {
                    moving = false;
                    count = 0;
                    timer.Restart();
                }
            }
        }

        public Vector2 getFrameSrc()
        {
            if (spriteFace == Globals.SpriteFace.Down)
            {
                return Globals.spriteSrcDown[count];
            }
            else if (spriteFace == Globals.SpriteFace.Up)
            {
                return Globals.spriteSrcUp[count];
            }
            else if (spriteFace == Globals.SpriteFace.Left)
            {
                return Globals.spriteSrcLeft[count];
            }
            else
            {
                return Globals.spriteSrcRight[count];
            }
        }
    }
}