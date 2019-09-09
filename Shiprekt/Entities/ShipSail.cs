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
using Shiprekt.Utilities;
using FlatRedBall.Math;

namespace Shiprekt.Entities
{
    // This enum relates wind strength to frame numbers in
    // the sail animation chains. The sail alignment with
    // the wind is calculated and the SailFullness is set
    // as the frame index
    public enum SailFullness
    {
        Empty = 0,
        Normal = 1,
        Full = 2
    }

    public partial class ShipSail
    {
        // This field represents how well the sails are aligned
        // with the wind. Use this to update the sail visual and
        // also to make decisions about the ship's current speed potential
        private SailFullness currentSailFullness;
        public SailFullness CurrentSailFullness
        {
            get
            {
                return currentSailFullness;
            }
            set
            {
                currentSailFullness = value;
                ShadowSprite.CurrentFrameIndex = (int)currentSailFullness;
                SailSprite.CurrentFrameIndex = (int)currentSailFullness;
            }
        }

		private void CustomInitialize()
		{


		}

		private void CustomActivity()
		{

		}

		private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }


		/// <summary>
		/// Updates the animation chain used for the sails based on the angle between its vector and the winds vector. 
		/// </summary>
		/// <param name="windVector"></param>
		public void UpdateSailVisual(Vector2 windVector)
		{			
			windVector.Normalize();

            // Find the difference in wind vs sail angles.
            // TODO: the sailAngle value may need to be calculated differently when the
            // sail rotates independent of its parent ship!
            var windAngle = Math.Atan2(windVector.Y - Vector3.Up.Y, windVector.X - Vector3.Up.X);
            var sailAngle = RotationZ;
            var absAngleDelta = Math.Abs(MathFunctions.AngleToAngle(sailAngle, windAngle));
            var absAngleDeltaDegrees = absAngleDelta.ToDegrees();

            // Update sail fullness based on alignment with wind direction
            CurrentSailFullness = SailFullness.Empty;
            if(absAngleDeltaDegrees < AngleVarianceDegreesCoefficient)
            {
                CurrentSailFullness = SailFullness.Full;
            }
            else if(absAngleDeltaDegrees < AngleVarianceDegreesCoefficient * 2)
            {
                CurrentSailFullness = SailFullness.Normal;
            }
		}
	}
}
