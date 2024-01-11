using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace PPaSD_XNA
{
    class Animator
    {
        public enum EasingCurve
        {
            LINEAR = 0,
            CUBIC
        }

        public delegate void onAnimationUpdate(float value);
        public delegate void onAnimationFinish();

        public float Start { get; set; }
        public float End { get; set; }
        public float Value { get; set; }
        public int Duration { get; set; }
        public EasingCurve Curve { get; set; }
        public bool Loop { get; set; }

        onAnimationUpdate updateCallback;
        onAnimationFinish finishCallback;

        bool playing = false;
        int startTime = 0;

        public Animator(float start, float end, int duration)
        {
            Start = start;
            End = end;
            Value = start;
            Duration = duration;
            Curve = EasingCurve.CUBIC;
            Loop = false;
            this.updateCallback = null;
            this.finishCallback = null;
        }

        public Animator(float start, float end, int duration, EasingCurve curve)
        {
            Start = start;
            End = end;
            Value = start;
            Duration = duration;
            Curve = curve;
            Loop = false;
            this.updateCallback = null;
            this.finishCallback = null;
        }

        public Animator(float start, float end, int duration, EasingCurve curve, bool loop)
        {
            Start = start;
            End = end;
            Value = start;
            Duration = duration;
            Curve = curve;
            Loop = loop;
            this.updateCallback = null;
            this.finishCallback = null;
        }

        public Animator(float start, float end, int duration, EasingCurve curve, bool loop, onAnimationUpdate updateCallback, onAnimationFinish finishCallback)
        {
            Start = start;
            End = end;
            Value = start;
            Duration = duration;
            Curve = curve;
            Loop = loop;
            this.updateCallback = updateCallback;
            this.finishCallback = finishCallback;
        }

        public void Play()
        {
            this.playing = true;
            this.startTime = System.Environment.TickCount;
        }

        public void Stop()
        {
            this.playing = false;
        }

        public void Update()
        {
            if (playing)
            {
                float progress = (System.Environment.TickCount - this.startTime) / (float)Duration;

                if (progress >= 1.0f)
                {
                    if (Loop)
                    {
                        this.startTime = System.Environment.TickCount;
                        if (this.updateCallback != null)
                        {
                            updateCallback(End);
                        }
                    }
                    else
                    {
                        this.Stop();
                        if (this.finishCallback != null)
                        {
                            finishCallback();
                        }
                    }
                }

                if (Curve == EasingCurve.CUBIC)
                {
                    Value = MathHelper.SmoothStep(Start, End, progress);
                }
                else //Fall back to linear
                {
                    Value = MathHelper.Lerp(Start, End, progress);
                }
            }

            if (this.updateCallback != null)
            {
                updateCallback(Value);
            }
        }
    }
}
