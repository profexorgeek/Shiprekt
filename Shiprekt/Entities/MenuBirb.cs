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

namespace Shiprekt.Entities
{
    public partial class MenuBirb
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
            var xVel = Math.Abs(Velocity.X);
            var newVel = Math.Min(xVel, MaxXVelocity);
            Velocity.X = newVel * Math.Sign(Velocity.X);
            if (XVelocity < 0) BirbSprite.FlipHorizontal = true; 
        }

        private void CustomDestroy()
        {


        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }

        public void SetRandomAnimationFrame()
        {
            BirbSprite.CurrentFrameIndex = FlatRedBallServices.Random.Next(0, BirbSprite.CurrentChain.Count);
        }
    }
}
