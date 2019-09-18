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
    public partial class ShotMissEffect
    {
        // If we ever get more than dirt/water, this might need to be an enum system.
        public bool IsGroundHit { get; set; } = false;
        const string GroundMissChainName = "SmokeParticles";
        const string WaterMissChainName = "WakeParticle2";
        float lifeSecondsRemaining;
        EasyEmitter waterShotMissEmitter;
        EasyEmitter groundShotMissEmitter;

        private void CustomInitialize()
        {
            lifeSecondsRemaining = EffectDurationSeconds;

            waterShotMissEmitter = EasyEmitter.BuildExplosion(GlobalContent.EffectChains[WaterMissChainName], EmitterPower.Tiny, 6f, 7f, 3f, 16f);
            waterShotMissEmitter.AttachTo(this, false);

            // override some core emitter settings for ripples
            waterShotMissEmitter.NumberPerEmission = 10;
            waterShotMissEmitter.EmissionSettings.Alpha = 0.25f;
            waterShotMissEmitter.EmissionSettings.RotationZVelocity = 0;
            waterShotMissEmitter.EmissionSettings.RotationZVelocityRange = 0;

            groundShotMissEmitter = EasyEmitter.BuildExplosion(GlobalContent.EffectChains[GroundMissChainName], EmitterPower.Tiny, 6f, 7f, 3f, 16f);
            groundShotMissEmitter.AttachTo(this, false);

            // override some core emitter settings for ripples
            groundShotMissEmitter.NumberPerEmission = 10;
            groundShotMissEmitter.EmissionSettings.Alpha = 0.3f;
            groundShotMissEmitter.EmissionSettings.RotationZVelocity = 0;
            groundShotMissEmitter.EmissionSettings.RotationZVelocityRange = 0;
        }

        private void CustomActivity()
        {
            waterShotMissEmitter.TimedEmit();
            DoLifeCountdown();
        }

        private void CustomDestroy()
        {


        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }

        public void TriggerEffect(float x, float y, float rotation)
        {
            X = x;
            Y = y;
            RotationZ = rotation;

            if (IsGroundHit)
            {
                Z = 5;
                groundShotMissEmitter.Emit();
            }
            else
            {
                Z = 0;
                waterShotMissEmitter.Emit();
            }
        }


        void DoLifeCountdown()
        {
            lifeSecondsRemaining -= TimeManager.SecondDifference;

            if (lifeSecondsRemaining <= 0)
            {
                Destroy();
            }
        }

        void DoEffectManagement()
        {

        }
    }
}
