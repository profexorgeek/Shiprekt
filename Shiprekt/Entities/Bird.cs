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

namespace Shiprekt.Entities
{
    public partial class Bird
    {
        public bool Animating
        {
            get => BirdSprite.Animate;
            set
            {
                BirdSprite.Animate = value;
                BirdShadowSprite.Animate = value;
            }
        }
        public float Altitude
        {
            get => BirdSprite.RelativeZ;
            set
            {
                BirdSprite.RelativeZ = value;
            }
        }
        float SecondsToNextStateChange;
        float CurrentFlightSpeed;

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
            DoFlightStateManagement();
            Velocity.X = (float)Math.Cos(RotationZ) * CurrentFlightSpeed;
            Velocity.Y = (float)Math.Sin(RotationZ) * CurrentFlightSpeed;
        }

        private void CustomDestroy()
        {


        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }

        void DoFlightStateManagement()
        {
            if (SecondsToNextStateChange <= 0)
            {
                if (Animating)
                {
                    Animating = false;
                    BirdSprite.CurrentFrameIndex = 1; // Open wings (glide)
                    BirdShadowSprite.CurrentFrameIndex = 1;
                }
                else
                {
                    Animating = true;
                }
                RotationZVelocity = FlatRedBallServices.Random.Between(-MaxTurnRate, MaxTurnRate);
                CurrentFlightSpeed = FlatRedBallServices.Random.Between(MinFlightSpeed, MaxFlightSpeed);
                SecondsToNextStateChange = FlatRedBallServices.Random.Between(0, MaxSecondsToFlightStateChange);
            }
            SecondsToNextStateChange -= TimeManager.SecondDifference;
        }
    }
}
