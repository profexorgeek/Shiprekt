using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlatRedBall;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;
using Microsoft.Xna.Framework;

namespace Shiprekt.Utilities
{
    // This is intended to be a self-contained file
    // that makes it easy to add common particle effects
    // to monthly games until a more robust system is
    // created for FlatRedBall

    public enum EmitterPower
    {
        Tiny = 16,
        Small = 32,
        Medium = 64,
        Large = 128,
        Huge = 256,
        Enormous = 512
    }

    public class EasyEmitter : Emitter
    {
        // maps EmitterPower to a qty of particles
        private static readonly Dictionary<EmitterPower, int> ExplosionParticles = new Dictionary<EmitterPower, int>
        {
            {EmitterPower.Tiny, 8},
            {EmitterPower.Small, 10},
            {EmitterPower.Medium, 16},
            {EmitterPower.Large, 32},
            {EmitterPower.Huge, 64 },
            {EmitterPower.Enormous, 128}
        };

        // maps EmitterPower to a distance threshold to emit particles
        private static readonly Dictionary<EmitterPower, int> ContrailParticles = new Dictionary<EmitterPower, int>
        {
            {EmitterPower.Tiny, 1},
            {EmitterPower.Small, 2},
            {EmitterPower.Medium, 4},
            {EmitterPower.Large, 8},
            {EmitterPower.Huge, 12 },
            {EmitterPower.Enormous, 16}
        };

        const float DefaultDrag = 5f;
        const float VelocityRangePercent = 0.5f;
        const float VelocitySizeCoefficient = 1f;
        const float RotationVelocity = 2f;
        const float PiFloat = (float)Math.PI;

        SpriteList particles { get; set; } = new SpriteList();
        Vector3 lastEmissionPosition;
        EmitterPower emitterPower = EmitterPower.Medium;
        float emissionDistance = 0;

        static Vector2 GetTextureScale(AnimationChain particleChain)
        {
            var frame1 = particleChain[0];
            return new Vector2
            {
                X = frame1.Texture.Width * (frame1.RightCoordinate - frame1.LeftCoordinate) / 2f,
                Y = frame1.Texture.Height * (frame1.BottomCoordinate - frame1.TopCoordinate) / 2f
            };
        }

        public static EasyEmitter BuildExplosion(
            AnimationChain particleChain,               // the particle to use in this effect, it'll be animated
            EmitterPower power = EmitterPower.Medium,   // the strength of this effect
            float lifeSeconds = 1f,                     // how long particles stick around
            float wedgeDegrees = 360f,                  // the explosion wedge in degrees
            float scalePerSecond = 0f,                  // how quickly the particle scales
            float area = 1f)                            // the area size to emit particles
        {
            var emitter = new EasyEmitter();
            emitter.emitterPower = power;
            var scale = GetTextureScale(particleChain);
            var radialVelocity = (float)power * VelocitySizeCoefficient * (1 - VelocityRangePercent);
            var radialRange = (float)power * VelocitySizeCoefficient * VelocityRangePercent;

            emitter.EmissionSettings = new EmissionSettings
            {
                
                Alpha = 1f,
                AlphaRate = -1f / lifeSeconds,
                AnimationChain = particleChain,
                Animate = true,
                Drag = DefaultDrag,
                ScaleX = scale.X,
                ScaleY = scale.Y,
                ScaleXVelocity = scalePerSecond,
                ScaleYVelocity = scalePerSecond,
                RotationZ = -PiFloat,
                RotationZRange = PiFloat * 2f,
                RotationZVelocity = -RotationVelocity,
                RotationZVelocityRange = RotationVelocity * 2f,
                RadialVelocity = radialVelocity,
                RadialVelocityRange = radialRange,
                VelocityRangeType = RangeType.Wedge,
                WedgeAngle = 0,
                WedgeSpread = wedgeDegrees * (PiFloat / 180f)
            };

            emitter.TimedEmission = false;
            emitter.SecondFrequency = 0.1f;
            emitter.RemovalEvent = Emitter.RemovalEventType.Alpha0;
            emitter.AreaEmission = AreaEmissionType.Rectangle;
            emitter.ScaleX = area / 2f;
            emitter.ScaleY = area / 2f;
            emitter.NumberPerEmission = ExplosionParticles[power];

            return emitter;
        }

        public static EasyEmitter BuildContrail(
            AnimationChain particleChain,               // the particle to use in this effect, it'll be animated
            EmitterPower power = EmitterPower.Medium,   // the strength of this effect
            float distance = 8f,                        // the distance between each emit
            float lifeSeconds = 3f,                     // how long particles stick around
            float scalePerSecond = 2f,                  // how quickly the particle scales
            float area = 1f)                            // the area size to emit particles
        {
            var emitter = new EasyEmitter();
            emitter.emitterPower = power;
            emitter.emissionDistance = distance;
            var scale = GetTextureScale(particleChain);
            emitter.EmissionSettings = new EmissionSettings
            {

                Alpha = 1f,
                AlphaRate = -1f / lifeSeconds,
                AnimationChain = particleChain,
                Animate = true,
                Drag = DefaultDrag,
                ScaleX = scale.X,
                ScaleY = scale.Y,
                ScaleXRange = scale.X * 0.45f,
                ScaleYRange = scale.Y * 0.45f,
                ScaleXVelocity = scalePerSecond,
                ScaleYVelocity = scalePerSecond,
                RotationZ = -PiFloat,
                RotationZRange = PiFloat * 2f,
                RotationZVelocity = -0.5f,
                RotationZVelocityRange = 1f,
                VelocityRangeType = RangeType.Radial
            };

            emitter.TimedEmission = false;
            emitter.RemovalEvent = Emitter.RemovalEventType.Alpha0;
            emitter.AreaEmission = AreaEmissionType.Rectangle;
            emitter.ScaleX = area / 2f;
            emitter.ScaleY = area / 2f;
            emitter.NumberPerEmission = ContrailParticles[power];

            return emitter;
        }       

        public SpriteList Emit()
        {
            particles.Clear();
            Emit(particles);

            return particles;
        }

        public void DistanceEmit()
        {
            var currentPos = Parent?.Position ?? Vector3.Zero;
            var vectorDelta = (currentPos - lastEmissionPosition).Length();
            var elapsedDistance = Math.Abs(vectorDelta);

            if(elapsedDistance >= emissionDistance)
            {
                this.Emit();
                lastEmissionPosition = currentPos;
            }
        }
    }
}
