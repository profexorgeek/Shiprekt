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
    public partial class FireSmokeEmitter
    {
        double lastEmitTime;
        SpriteList emittedSprites = new SpriteList();

        Color darkColor = new Color(1f, 1f, 1f);
        Color yellowColor = new Color(1f, 1f, 0.5f);
        Color redColor = new Color(1f, 0.5f, 0.5f);

        float particleLife = 5;

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
            if(IsEmitting && TimeManager.CurrentScreenSecondsSince(lastEmitTime) > EmissionFrequency)
            {
                lastEmitTime = TimeManager.CurrentScreenTime;

                EmitSmoke();
            }

            var currentTime = TimeManager.CurrentTime;

            for(int i = 0; i < emittedSprites.Count; i++)
            {
                var sprite = emittedSprites[i];

                var timeAlive = currentTime - sprite.TimeCreated;

                const float fadeoutTime = 3;

                if(timeAlive < 1)
                {
                    // yellow -> red
                    var interpolationValue = (float)(timeAlive * 1);
                    sprite.Red = MathHelper.Lerp(yellowColor.R / 255.0f, redColor.R / 255.0f, interpolationValue);
                    sprite.Green = MathHelper.Lerp(yellowColor.G / 255.0f, redColor.G / 255.0f, interpolationValue);
                    sprite.Blue = MathHelper.Lerp(yellowColor.B / 255.0f, redColor.B / 255.0f, interpolationValue);
                }
                else if (timeAlive < 2)
                {
                    // red -> gray
                    var interpolationValue = (float)((timeAlive - 1) * 1);

                    sprite.Red = MathHelper.Lerp(redColor.R / 255.0f, darkColor.R / 255.0f, interpolationValue);
                    sprite.Green = MathHelper.Lerp(redColor.G / 255.0f, darkColor.G / 255.0f, interpolationValue);
                    sprite.Blue = MathHelper.Lerp(redColor.B / 255.0f, darkColor.B / 255.0f, interpolationValue);
                }
                else if(timeAlive > particleLife - fadeoutTime)
                {
                    var lifeLeft = (float)(particleLife - timeAlive) / fadeoutTime;

                    sprite.Alpha = MathHelper.Lerp(0, 1, lifeLeft);
                }
            }

        }

        private void EmitSmoke()
        {
            var sprite = SpriteManager.AddParticleSprite(SmokeParticles[0].Texture);
            sprite.AnimationChains = EffectChains;
            sprite.CurrentChainName = SmokeParticles.Name;
            sprite.Animate = false;
            sprite.CurrentFrameIndex = FlatRedBallServices.Random.Next(SmokeParticles.Count);

            sprite.TimeCreated = TimeManager.CurrentTime;
            sprite.TextureScale = 1;
            sprite.UpdateToCurrentAnimationFrame();
            sprite.TextureScale = -1;
            sprite.ScaleXVelocity = 1.25f;
            sprite.ScaleYVelocity = 1.25f;
            sprite.X = this.X;
            sprite.Y = this.Y;
            sprite.Z = this.Z;
            sprite.ZVelocity = 50;
            sprite.ColorOperation = FlatRedBall.Graphics.ColorOperation.Modulate;
            sprite.Drag = 0;
            sprite.Detach();
            sprite.RotationZ = FlatRedBallServices.Random.Between(-3.14f, 3.14f);
            sprite.RotationZVelocity = FlatRedBallServices.Random.Between(-0.9f, 0.9f);

            this.emittedSprites.Add(sprite);

            this.Call(() => SpriteManager.RemoveSprite(sprite)).After(particleLife);
        }

        private void CustomDestroy()
        {


        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
    }
}
