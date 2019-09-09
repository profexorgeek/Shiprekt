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
using System.Linq;
using Shiprekt.Utilities;

namespace Shiprekt.Entities
{
    public partial class Cloud
    {
        public bool Animating
        {
            get => CloudSprite.Animate;
            set
            {
                CloudSprite.Animate = value;
            }
        }
        public float Altitude
        {
            get => CloudSprite.RelativeZ;
            set
            {
                CloudSprite.RelativeZ = value;
            }
        }

        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
        private void CustomInitialize()
        {
            Velocity.X = (float)Math.Cos(RotationZ);
            Velocity.Y = (float)Math.Sin(RotationZ);
        }

        private void CustomActivity()
        {


        }

        private void CustomDestroy()
        {


        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }

        public void PickRandomSprite()
        {
            var randChainName = CloudChains.Random().Name;

            // sprite is the first frame by convention
            CloudSprite.CurrentChainName = randChainName;
            CloudSprite.CurrentFrameIndex = 0;

            // shadow is the second frame by convention
            CloudShadowSprite.CurrentChainName = randChainName;
            CloudShadowSprite.CurrentFrameIndex = 1;
        }
    }
}
