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

namespace Shiprekt.Entities.Effects
{
    public partial class CannonballEffect
    {
        float timeToNextEmit;
        List<Sprite> particles = new List<Sprite>();
        float timeToGenerateSprites;

        public float EffectStrength { get; set; } = 1f;

        private void CustomInitialize()
        {
            timeToGenerateSprites = EmissionTotalDuration;

        }

        private void CustomActivity()
        {
            timeToGenerateSprites -= TimeManager.SecondDifference;
            if (timeToGenerateSprites >= 0)
            {
                DoEmission();
            }
            DoSpriteRemoval();
        }

        private void CustomDestroy()
        {


        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }

        void DoEmission()
        {
            timeToNextEmit -= TimeManager.SecondDifference;

            if (timeToNextEmit <= 0)
            {
                for (var i = 0; i < EmissionCount; i++)
                {
                    particles.Add(CreateWakeParticle());
                }
                timeToNextEmit = EmissionFreqSeconds;
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

        Sprite CreateWakeParticle()
        {
            var chains = GlobalContent.EffectChains;
            var rand = FlatRedBallServices.Random;
            var particle = SpriteManager.AddParticleSprite(GlobalContent.shiprekt);
            var lateralRotation = (float)(RotationZ - (Math.PI / 2f));
            var magnitude = rand.Between(-LateralVelocityMax, LateralVelocityMax);

            particle.AnimationChains = chains;
            particle.CurrentChainName = "SmokeParticles";
            particle.Animate = false;
            particle.CurrentFrameIndex = rand.Next(0, particle.CurrentChain.Count);
            particle.AlphaRate = -1f / ParticleLifeSeconds;
            particle.XVelocity = (float)Math.Cos(lateralRotation) * magnitude;
            particle.YVelocity = (float)Math.Sin(lateralRotation) * magnitude;
            particle.Drag = 0f;
            // if you don't like magic constants, give this method a double overload
            // so I can use Math.PI
            particle.RotationZ = rand.Between(-3.14f, 3.14f);
            //particle.RotationZVelocity = rand.Between(-RotationVelocityMax, 0) * Math.Sign(particle.RotationZ);
            particle.ScaleXVelocity = ScaleVelocity;
            particle.ScaleYVelocity = ScaleVelocity;
            particle.X = this.X;
            particle.Y = this.Y;
            particle.Z = 2f;
            particle.TextureScale = StartScale;

            return particle;
        }
    }
}
