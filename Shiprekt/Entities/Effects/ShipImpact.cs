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
using FlatRedBall.Math;

namespace Shiprekt.Entities.Effects
{
    public partial class ShipImpact
    {
        protected List<Sprite> woodParticles = new List<Sprite>();
        protected List<Sprite> explosionParticles = new List<Sprite>(); 

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
            if (woodParticles.Count > 0 || explosionParticles.Count > 0)
            {
                DoSpriteRemoval();
            }
            else
            {
                Destroy();
            }
		}

		private void CustomDestroy()
        {

        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }

		public void EmitEffectParticles(Vector2 position, Vector2 direction)
		{
			var numPartiles = FlatRedBallServices.Random.Next(MinParticles, MaxParticles + 1);
            CreateExplosionParticle(position); 
			for (int i = 0; i < numPartiles; i++)
			{
				var randAngleDeg = (float)FlatRedBallServices.Random.Between(-MaxAngleVarianceDeg, MaxAngleVarianceDeg);
				var curAngleDeg = Math.Atan2(direction.Y, direction.X) * (180/Math.PI);
				var newAngleDeg = curAngleDeg + randAngleDeg;
                var newAngleRad = newAngleDeg * (Math.PI / 180); 
				var newVector = new Vector2((float)Math.Cos(newAngleRad), (float)Math.Sin(newAngleRad));
				CreateShipImpactParticle(position, newVector);
			}
		}

		void DoSpriteRemoval()
		{
			for (var i = woodParticles.Count - 1; i > -1; i--)
			{
				if (woodParticles[i].Alpha <= 0)
				{
					var particle = woodParticles[i];
					woodParticles.RemoveAt(i);
					SpriteManager.RemoveSprite(particle);
				}
			}
            for (int i = explosionParticles.Count - 1; i >= 0; i--)
            {                
                if (explosionParticles[i].JustCycled)
                {
                    var particle = explosionParticles[i]; 
                    explosionParticles.RemoveAt(i);
                    SpriteManager.RemoveSprite(particle); 
                }
            }
		}

        protected virtual Sprite CreateExplosionParticle(Vector2 position)
        {
            var particle = SpriteManager.AddParticleSprite(GlobalContent.shiprekt);
            explosionParticles.Add(particle);
            var chains = GlobalContent.EffectChains;
            particle.Position = position.ToVector3();
            particle.Z = this.Z + 1; 
            particle.AnimationChains = chains;
            particle.CurrentChainName = "Explosion";
            particle.CurrentFrameIndex = 0;
            particle.TextureScale = 1; 
            particle.Animate = true;
            
            return particle; 
        }
		protected virtual Sprite CreateShipImpactParticle(Vector2 position, Vector2 movementVector)
		{
			var particle = SpriteManager.AddParticleSprite(GlobalContent.shiprekt);
            woodParticles.Add(particle);
            var chains = GlobalContent.EffectChains;
            var rand = FlatRedBallServices.Random;
            particle.AnimationChains = chains;
            particle.CurrentChainName = "WoodParticle";
            particle.Animate = false;
            particle.CurrentFrameIndex = rand.Next(0, particle.CurrentChain.Count);

            //var lateralRotation = (float)(RotationZ - (Math.PI / 2f));
            var magnitude = rand.Between(VelocityMin, VelocityMax);
			var movementTime = rand.Between(MinTimeToStop, MaxTimeToStop);

			particle.AlphaRate = -1f / ParticleLifeSeconds;
			particle.XVelocity = movementVector.X * magnitude;
			particle.YVelocity = movementVector.Y * magnitude;
			particle.Drag = ParticleDrag;
			// if you don't like magic constants, give this method a double overload
			// so I can use Math.PI
			particle.RotationZ = rand.Between(-3.14f, 3.14f);
			particle.X = position.X;
			particle.Y = position.Y;
			particle.Z = this.Z;
			particle.TextureScale = StartScale;
			particle.Call(() => { particle.Velocity = Vector3.Zero; }).After(movementTime);
			return particle;
		}
	}
}
