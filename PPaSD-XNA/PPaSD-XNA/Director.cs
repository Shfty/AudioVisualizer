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
    struct Command
    {
        public int milliseconds;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }

    class Director
    {
        List<KeyValuePair<GameObject, List<Command>>> cast;

        public Director()
        {
            cast = new List<KeyValuePair<GameObject, List<Command>>>();
        }

        public void AddObject(ref GameObject gameObject, List<Command> commandList)
        {
            cast.Add(new KeyValuePair<GameObject,List<Command>>(gameObject, commandList));
            //Console.WriteLine("Adding Object: " + gameObject.Position);
        }

        public void RemoveObject(ref GameObject gameObject)
        {
            foreach (KeyValuePair<GameObject, List<Command>> member in cast)
            {
                if (member.Key == gameObject)
                {
                    cast.Remove(member);
                }
            }
        }

        public void Play()
        {
            foreach (KeyValuePair<GameObject, List<Command>> member in cast)
            {
                member.Key.CommandIndex = 0;
                member.Key.CommandStartMs = System.Environment.TickCount;
                member.Key.Playing = true;
            }
        }

        public void Update()
        {
            foreach (KeyValuePair<GameObject, List<Command>> member in cast)
            {
                if (member.Key.Playing == true)
                {
                    int progressMs = System.Environment.TickCount - member.Key.CommandStartMs;
                    float progress = (float)progressMs / member.Value[member.Key.CommandIndex].milliseconds;

                    member.Key.Position = Vector3.SmoothStep(member.Key.Position, member.Value[member.Key.CommandIndex].position, progress);
                    member.Key.Rotation = Quaternion.Slerp(member.Key.Rotation, member.Value[member.Key.CommandIndex].rotation, progress);
                    member.Key.Scale = Vector3.SmoothStep(member.Key.Scale, member.Value[member.Key.CommandIndex].scale, progress);

                    //Console.WriteLine("Lerp-ing Object: " + member.Key.Position);

                    if (progressMs >= member.Value[member.Key.CommandIndex].milliseconds)
                    {
                        //TODO: Advance through animation list if queued
                        if (member.Key.CommandIndex == member.Value.Count() - 1)
                        {
                            if (member.Key.Loop)
                            {
                                member.Key.CommandIndex = 0;
                                member.Key.CommandStartMs = System.Environment.TickCount;
                            }
                            else
                            {
                                member.Key.Playing = false;
                            }
                        }
                        else
                        {
                            member.Key.CommandIndex++;
                            member.Key.CommandStartMs = System.Environment.TickCount;
                        }
                    }
                }
            }
        }

        public List<GameObject> Objects()
        {
            List<GameObject> list = new List<GameObject>();

            foreach (KeyValuePair<GameObject, List<Command>> member in cast)
            {
                list.Add(member.Key);
            }

            return list;
        }
    }
}
