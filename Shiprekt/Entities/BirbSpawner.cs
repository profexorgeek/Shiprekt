using System;
using System.Collections.Generic;
using System.Text;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;
using FlatRedBall.Math.Geometry;
using Microsoft.Xna.Framework;
using Shiprekt.Factories;

namespace Shiprekt.Entities
{
    public partial class BirbSpawner
    {
        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
        private void CustomInitialize()
        {


        }

        private void CustomActivity()
        {
            ManageBirbs();

        }

        private void ManageBirbs()
        {
            for (int i = MenuBirbList.Count - 1; i > -1; i--)
            {
                var birb = MenuBirbList[i];
                if (birb.X < Camera.Main.AbsoluteLeftXEdgeAt(birb.Z) || birb.X > Camera.Main.AbsoluteRightXEdgeAt(birb.Z))
                {
                    birb.Destroy();
                    MenuBirbList.Remove(birb);
                }
            }
        }

        private void CustomDestroy()
        {


        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }

        public void SpawnBirbs(Vector3 position)
        {
            var numBirbs = FlatRedBallServices.Random.Next(MinBirbs, MaxBirbs);
            for (int i = 0; i < numBirbs; i++)
            {
                var birb = MenuBirbFactory.CreateNew(this.LayerProvidedByContainer);
                MenuBirbList.Add(birb); 
                birb.SetRandomAnimationFrame(); 

                //Position
                birb.Position = position;

                //Initial Velocity
                var randDeg = FlatRedBallServices.Random.Between(-MaxFlyVariationDeg, MaxFlyVariationDeg);
                var newAngleDeg = DefaultFlyAngleDeg + randDeg;
                var newAngleRad = newAngleDeg * (Math.PI / 180); 
                Vector3 birdVector = new Vector3((float)Math.Cos(newAngleRad), (float)Math.Sin(newAngleRad), Z) * MaxSpeed;
                birb.Velocity = birdVector;
                
                //X velocity to fly off screen. 
                var sign = FlatRedBallServices.Random.Next(0, 2) * 2 - 1;
                birb.XAcceleration = BirbXAccel * sign; 
                
                birb.Drag = FlatRedBallServices.Random.Between(MinBirbDrag, MaxBirbDrag);
            }
        }
    }
}
