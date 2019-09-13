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

namespace Shiprekt.Entities.Effects
{
	public partial class WakeEffect
	{
        EasyEmitter rippleEmitter;
        EasyEmitter trailEmitter;

		private void CustomInitialize()
		{
            CreateEmitters();
		}

		private void CustomActivity()
		{
            DoEmission();
		}

		private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }

        private void CreateEmitters()
        {
            // Create emitters using EasyEmitter
            var rippleParticle = GlobalContent.EffectChains["WakeParticle"];
            rippleEmitter = EasyEmitter.BuildContrail(rippleParticle, EmitterPower.Tiny, 3f, 0.70f, 25f);
            
            var trailParticle = GlobalContent.EffectChains["WakeParticle2"];
            trailEmitter = EasyEmitter.BuildContrail(trailParticle, EmitterPower.Tiny, 6f, 7f, 3f, 16f);


            // Override a few settings
            rippleEmitter.EmissionSettings.Alpha = 0.35f;
            rippleEmitter.RelativeX = 2f;

            trailEmitter.EmissionSettings.Alpha = 0.35f;
            trailEmitter.EmissionSettings.RotationZVelocity = 0;
            trailEmitter.EmissionSettings.RotationZVelocityRange = 0;

            // Attach and position
            rippleEmitter.AttachTo(this, false);
            rippleEmitter.RelativeZ = -1f;

            trailEmitter.AttachTo(this, false);
            trailEmitter.RelativeZ = -1.5f;
            trailEmitter.RelativeX = -16f;
            
        }

        private void DoEmission()
        {
            // Easy emitters emit based on distance, not time!
            rippleEmitter.DistanceEmit();
            trailEmitter.DistanceEmit();
        }
	}
}
