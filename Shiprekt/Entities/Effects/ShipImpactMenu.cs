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

namespace Shiprekt.Entities.Effects
{
    public partial class ShipImpactMenu
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
            ParticleActivity();

        }



        private void CustomDestroy()
        {


        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }

        private void ParticleActivity()
        {
            foreach (var particle in woodParticles)
            {
                if(particle.Y < SeaLevel)
                {
                    particle.Velocity = Vector3.Zero;
                    particle.Acceleration = Vector3.Zero; 
                }
            }
        }

        protected override Sprite CreateShipImpactParticle(Vector2 position, Vector2 movementVector)
        {
            var chains = GlobalContent.EffectChains;
            var rand = FlatRedBallServices.Random;
            var particle = SpriteManager.AddParticleSprite(GlobalContent.shiprekt);
            woodParticles.Add(particle); 

            //var lateralRotation = (float)(RotationZ - (Math.PI / 2f));
            var magnitude = rand.Between(VelocityMin, VelocityMax);
            var movementTime = rand.Between(MinTimeToStop, MaxTimeToStop);
            particle.AnimationChains = chains;
            particle.CurrentChainName = "WoodParticle";
            particle.Animate = false;
            particle.CurrentFrameIndex = rand.Next(0, particle.CurrentChain.Count);
            particle.AlphaRate = -1f / ParticleLifeSeconds;
            particle.XVelocity = movementVector.X * magnitude;
            particle.YVelocity = movementVector.Y * magnitude;
            particle.YAcceleration = Gravity; 
           
            // if you don't like magic constants, give this method a double overload
            // so I can use Math.PI
            particle.RotationZ = rand.Between(-3.14f, 3.14f);
            particle.X = position.X;
            particle.Y = position.Y;
            particle.Z = this.Z;
            particle.TextureScale = StartScale;            
            return particle;
        }
    }
}
