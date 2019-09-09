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

namespace Shiprekt.Entities
{
	public partial class ShipSail
	{
		private const string SAIL_CHAIN_NAME_PREFIX = "Sail";
		private const string SHADOW_CHAIN_NAME_PREFIX = "Shadow"; 

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

			//Edge case where the designer TOTALLY MESSED UP AND DIDN'T ADD ALL THE NECESSARY VALUES...JUSTIN.
			if (SailAnimationValuesInstance.AngleThreshold.Count != SailAnimationValuesInstance.SailChainName.Count || SailAnimationValuesInstance.AngleThreshold.Count != SailAnimationValuesInstance.ShadowChainName.Count)
			{
				throw new Exception("You must define AnimationChainNames for wind and shadow for each AngleThrehold in SpeedThresholdLevels.csv"); 
			}

			var thisAngle = this.RotationZ;
			var windAngle = Math.Atan2(windVector.Y - Vector3.Up.Y, windVector.X - Vector3.Up.X);
			var rad = FlatRedBall.Math.MathFunctions.AngleToAngle(thisAngle, windAngle);
			var angle = Math.Abs(rad * (180 / Math.PI)); 
			for (int i = 0; i < SailAnimationValuesInstance.AngleThreshold.Count; i++)
			{
				var threshold = SailAnimationValuesInstance.AngleThreshold[i]; 
				if (angle < threshold)
				{
					SailSpriteCurrentChainName = SailAnimationValuesInstance.SailChainName[i];
					ShadowSpriteCurrentChainName = SailAnimationValuesInstance.ShadowChainName[i];
					break; 
				}
			}	
		}
	}
}
