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

namespace Shiprekt.Entities
{
    public partial class Bullet
    {
        public int TeamIndex { get; set; }
        

        public Ship Owner { get; set; }

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

        internal void HitSurface()
        {
            Instructions.Clear();

            ShotMissEffectFactory.CreateNew().TriggerEffect(X, Y, RotationZ);

            // broadcast this so that a collision can occur at screen level
            Destroy();
        }
    }
}
