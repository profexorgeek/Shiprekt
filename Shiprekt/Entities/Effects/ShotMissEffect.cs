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
        public bool IsDirtMiss { get; set; } = false;
        const string dirtMissChainName = "SmokeParticles";
        const string WaterMissChainName = "WakeParticle2";
        float lifeSecondsRemaining;
        EasyEmitter shotMissEmitter;

        private void CustomInitialize()
        {
            lifeSecondsRemaining = EffectDurationSeconds;
            shotMissEmitter = EasyEmitter.BuildExplosion(GlobalContent.EffectChains[WaterMissChainName], EmitterPower.Tiny, 6f, 7f, 3f, 16f);
            shotMissEmitter.AttachTo(this, false);

            // override some core emitter settings for ripples
            shotMissEmitter.NumberPerEmission = 10;
            shotMissEmitter.EmissionSettings.Alpha = 0.25f;
            shotMissEmitter.EmissionSettings.RotationZVelocity = 0;
            shotMissEmitter.EmissionSettings.RotationZVelocityRange = 0;
        }

        private void CustomActivity()
        {
            shotMissEmitter.TimedEmit();
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

            shotMissEmitter.Emit();
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
