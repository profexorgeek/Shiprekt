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
    public partial class ShipDeathEffect
    {
        float lifeSecondsRemaining;
        EasyEmitter smokeMed;
        EasyEmitter smokeSmall;
        EasyEmitter ripples;

        private void CustomInitialize()
        {
            lifeSecondsRemaining = EffectDurationSeconds;
            SinkSprite.Visible = true;

            smokeMed = EasyEmitter.BuildExplosion(GlobalContent.EffectChains["ExplosionSmokeMed"], EmitterPower.Medium, 20f);
            smokeMed.AttachTo(this, false);
            smokeMed.RelativeZ = 10f;

            smokeSmall = EasyEmitter.BuildExplosion(GlobalContent.EffectChains["WoodParticle"], EmitterPower.Large, 2f, 360f, -4f, 8f);
            smokeSmall.AttachTo(this, false);
            smokeSmall.RelativeZ = 20f;

            ripples = EasyEmitter.BuildContrail(GlobalContent.EffectChains["Ripple"], EmitterPower.Tiny, 0f, 4f, 6f);
            ripples.AttachTo(this, false);
            ripples.RelativeZ = -2f;
            ripples.RelativeX = -4f;

            // override some core emitter settings for ripples
            ripples.TimedEmission = true;
            ripples.EmissionSettings.Alpha = 0.5f;
            ripples.EmissionSettings.RotationZVelocity = 0;
            ripples.EmissionSettings.RotationZVelocityRange = 0;
            ripples.EmissionSettings.ScaleY = 12f;
            ripples.SecondFrequency = 1f;
        }

        private void CustomActivity()
        {
            if(SinkSprite.JustCycled)
            {
                SinkSprite.Visible = false;
            }

            if(ExplodeSprite.JustCycled)
            {
                ExplodeSprite.Visible = false;
            }

            ripples.TimedEmit();
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

            SinkSprite.CurrentFrameIndex = 0;
            SinkSprite.Visible = true;

            ExplodeSprite.CurrentFrameIndex = 0;
            ExplodeSprite.Visible = true;

            smokeMed.Emit();
            smokeSmall.Emit();
        }


        void DoLifeCountdown()
        {
            lifeSecondsRemaining -= TimeManager.SecondDifference;

            if(lifeSecondsRemaining <= 0)
            {
                Destroy();
            }
        }

        void DoEffectManagement()
        {

        }
    }
}
