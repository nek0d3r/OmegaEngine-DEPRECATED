using System;
using Microsoft.Xna.Framework;

namespace OmegaEngine
{
    public class Fade
    {
        public float level;
        public float step;
        public int delay;
        public Color color;

        public Fade()
        {
            level = 0f;
            step = .01f;
            delay = 20;
            color = Color.Black;
        }

        public void stepDown()
        {
            level -= step;
        }

        public void stepUp()
        {
            level += step;
        }
    }
}

