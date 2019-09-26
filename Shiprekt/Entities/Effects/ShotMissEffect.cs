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
        const string GroundMissChainName = "CannonballMissDirt";
        const string WaterMissWakeChainName = "CannonballMissWater";
        const string WaterMissSprayChainName = "WakeParticle2";
        float lifeSecondsRemaining;
        EasyEmitter waterShotMissSprayEmitter;
        EasyEmitter waterShotMissWakeEmitter;
        EasyEmitter groundShotMissEmitter;

        private void CustomInitialize()
        {
            lifeSecondsRemaining = EffectDurationSeconds;

            waterShotMissWakeEmitter = EasyEmitter.BuildExplosion(GlobalContent.EffectChains[WaterMissWakeChainName], EmitterPower.Tiny, 6f, 7f, 3f, 16f);
            waterShotMissWakeEmitter.AttachTo(this, false);

            // override some core emitter settings for spray
            waterShotMissWakeEmitter.NumberPerEmission = 10;
            waterShotMissWakeEmitter.EmissionSettings.Alpha = 0.25f;
            waterShotMissWakeEmitter.EmissionSettings.RotationZVelocity = 0;
            waterShotMissWakeEmitter.EmissionSettings.RotationZVelocityRange = 0;

            waterShotMissSprayEmitter = EasyEmitter.BuildExplosion(GlobalContent.EffectChains[WaterMissSprayChainName], EmitterPower.Tiny, 6f, 7f, 3f, 16f);
            waterShotMissSprayEmitter.AttachTo(this, false);

            // override some core emitter settings for ripples
            waterShotMissSprayEmitter.NumberPerEmission = 1;
            waterShotMissSprayEmitter.EmissionSettings.Alpha = 0.25f;
            waterShotMissSprayEmitter.EmissionSettings.RotationZVelocity = 0;
            waterShotMissSprayEmitter.EmissionSettings.RotationZVelocityRange = 0;

            groundShotMissEmitter = EasyEmitter.BuildExplosion(GlobalContent.EffectChains[GroundMissChainName], EmitterPower.Tiny, 6f, 7f, 3f, 16f);
            groundShotMissEmitter.AttachTo(this, false);

            // override some core emitter settings for "poof"
            groundShotMissEmitter.NumberPerEmission = 10;
            groundShotMissEmitter.EmissionSettings.Alpha = 0.3f;
            groundShotMissEmitter.EmissionSettings.RotationZVelocity = 0;
            groundShotMissEmitter.EmissionSettings.RotationZVelocityRange = 0;
        }

        private void CustomActivity()
        {
            waterShotMissSprayEmitter.TimedEmit();
            waterShotMissWakeEmitter.TimedEmit();
            groundShotMissEmitter.TimedEmit();
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
                waterShotMissSprayEmitter.Emit();
                waterShotMissWakeEmitter.Emit();
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
