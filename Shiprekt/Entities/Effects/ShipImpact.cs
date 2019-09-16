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
    public partial class ShipImpact
    {
        List<Sprite> particles = new List<Sprite>();

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
			if (particles.Count > 0)
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
			for (int i = 0; i < numPartiles; i++)
			{
				var randAngleOffset = (float)FlatRedBallServices.Random.Between(-MaxAngleVarianceDeg, MaxAngleVarianceDeg);
				var curAngle = Math.Atan2(direction.Y, direction.X);
				var newAngle = curAngle + randAngleOffset;
				var newVector = new Vector2((float)Math.Cos(newAngle), (float)Math.Sin(newAngle));
				CreateShipImpactParticle(position, newVector);
			}
		}

		void DoSpriteRemoval()
		{
			for (var i = particles.Count - 1; i > -1; i--)
			{
				if (particles[i].Alpha <= 0)
				{
					var particle = particles[i];
					particles.Remove(particle);
					SpriteManager.RemoveSprite(particle);
				}
			}
		}

		Sprite CreateShipImpactParticle(Vector2 position, Vector2 movementVector)
		{

			var chains = GlobalContent.EffectChains;
			var rand = FlatRedBallServices.Random;
			var particle = SpriteManager.AddParticleSprite(GlobalContent.shiprekt);
			//var lateralRotation = (float)(RotationZ - (Math.PI / 2f));
			var magnitude = rand.Between(VelocityMin, VelocityMax);
			var movementTime = rand.Between(MinTimeToStop, MaxTimeToStop);
			particle.AnimationChains = chains;
			particle.CurrentChainName = "WoodParticles";
			particle.Animate = false;
			particle.CurrentFrameIndex = rand.Next(0, particle.CurrentChain.Count);
			particle.AlphaRate = -1f / ParticleLifeSeconds;
			particle.XVelocity = movementVector.X * magnitude;
			particle.YVelocity = movementVector.Y * magnitude;
			particle.Drag = ParticleDrag;
			// if you don't like magic constants, give this method a double overload
			// so I can use Math.PI
			particle.RotationZ = rand.Between(-3.14f, 3.14f);
			particle.X = position.X;
			particle.Y = position.Y;
			particle.Z = 3f;
			particle.TextureScale = StartScale;
			particle.Call(() => { particle.Velocity = Vector3.Zero; }).After(movementTime);
			return particle;
		}
	}
}
