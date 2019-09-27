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
using Shiprekt.Utilities;
using Shiprekt.Factories;
using Microsoft.Xna.Framework.Audio;

namespace Shiprekt.Entities
{
    public enum SurfaceType
    {
        Water,
        Ground
    }

    public partial class Bullet
    {
        #region Fields/Properties
        
        public int TeamIndex { get; set; }

        public Ship Owner { get; set; }

        #endregion

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
            CannonballSpriteInstance.RelativeYVelocity = Math.Max(-100, CannonballSpriteInstance.RelativeYVelocity);
        }

        internal void HitSurface(SurfaceType surfaceType)
        {
            Instructions.Clear();

            var randomInt = FlatRedBallServices.Random.Next(2) + 1;
            SoundEffect file = null;

            if(surfaceType == SurfaceType.Water)
            {
                file = (SoundEffect)GetFile($"cannonballsink0{randomInt}");
            }
            else
            {
                // do we have ground hits sfx? Not yet, but if we do, add the logic here to pick one of the sounds
            }

            file?.Play();

            // broadcast this so that a collision can occur at screen level
            Destroy();
        }


        private void CustomDestroy()
        {


        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }

        public void InitializeArcAndShadow()
        {
            CannonballSpriteInstance.RelativeYVelocity = 100;
            var time = BulletDistance / BulletSpeed;
            CannonballSpriteInstance.RelativeYAcceleration = -(CannonballSpriteInstance.RelativeYVelocity / time) * 2;
        }

    }
}
