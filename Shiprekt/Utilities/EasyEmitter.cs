using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlatRedBall;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;

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
        private static readonly Dictionary<EmitterPower, int> ExplosionSizeParticles = new Dictionary<EmitterPower, int>
        {
            {EmitterPower.Tiny, 8},
            {EmitterPower.Small, 10},
            {EmitterPower.Medium, 16},
            {EmitterPower.Large, 32},
            {EmitterPower.Huge, 64 },
            {EmitterPower.Enormous, 128}
        };

        const float DefaultDrag = 5f;
        const float VelocityRangePercent = 0.5f;
        const float VelocitySizeCoefficient = 1f;
        const float RotationVelocity = 2f;
        const float PiFloat = (float)Math.PI;

        SpriteList particles { get; set; } = new SpriteList();

        public static EasyEmitter Explosion(AnimationChain particleChain, EmitterPower size = EmitterPower.Medium, float lifeSeconds = 1f, float wedgeDegrees = 360f, float scalePerSecond = 0f, float area = 1f)
        {
            var emitter = new EasyEmitter();
            var frame1 = particleChain[0];
            var xScale = frame1.Texture.Width * (frame1.RightCoordinate - frame1.LeftCoordinate);
            var yScale = frame1.Texture.Height * (frame1.TopCoordinate - frame1.BottomCoordinate);
            var scaleVelocity = scalePerSecond + 1f;
            var radialVelocity = (float)size * VelocitySizeCoefficient * (1 - VelocityRangePercent);
            var radialRange = (float)size * VelocitySizeCoefficient * VelocityRangePercent;

            emitter.EmissionSettings = new EmissionSettings
            {
                
                Alpha = 1f,
                AlphaRate = -1f / lifeSeconds,
                AnimationChain = particleChain,
                Animate = true,
                Drag = DefaultDrag,
                ScaleX = xScale,
                ScaleY = yScale,
                ScaleXVelocity = scaleVelocity,
                ScaleYVelocity = scaleVelocity,
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
            emitter.NumberPerEmission = ExplosionSizeParticles[size];

            return emitter;
        }

        public SpriteList Emit()
        {
            particles.Clear();
            Emit(particles);

            return particles;
        }
    }
}
