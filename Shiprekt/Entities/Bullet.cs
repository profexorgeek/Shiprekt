﻿using System;
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
            InitializeArc();
        }

        private void CustomActivity()
        {
            CannonballSpriteInstance.RelativeYVelocity = Math.Max(-100, CannonballSpriteInstance.RelativeYVelocity);
        }

        internal void HitSurface()
        {
            Instructions.Clear();

            var randomInt = FlatRedBallServices.Random.Next(2) + 1;
            var file = (SoundEffect)GetFile($"cannonballsink0{randomInt}");

            file.Play();

            // broadcast this so that a collision can occur at screen level
            Destroy();
        }


        private void CustomDestroy()
        {


        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }

        private void InitializeArc()
        {
            CannonballSpriteInstance.RelativeYVelocity = 100;
            var time = BulletDistance / BulletSpeed;
            CannonballSpriteInstance.RelativeYAcceleration = -(CannonballSpriteInstance.RelativeYVelocity / time) * 2;
        }

    }
}
