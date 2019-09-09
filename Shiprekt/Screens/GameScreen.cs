using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Localization;

using Microsoft.Xna.Framework;

using Shiprekt.Factories;
using Shiprekt.Entities;

namespace Shiprekt.Screens
{
    public partial class GameScreen
    {
        #region Fields/Properties

        Vector2 windDirection;

        #endregion


        #region Initialize

        void CustomInitialize()
        {
   //         DummyShip.InitializeRacingInput(InputManager.Xbox360GamePads[0]);
			//DummyShip.SetTeam(1);
			//DummyShip.AllowedToDrive = false;
			Ship1.SetTeam(2);
			Camera.Main.Z = 500; 
            FlatRedBallServices.Game.IsMouseVisible = true;

            windDirection = FlatRedBallServices.Random.RadialVector2(1, 1);

            OffsetTilemapLayers();
        }

		internal void OffsetTilemapLayers()
		{
			foreach (var layer in Map.MapLayers)
			{
				var property = layer.Properties.FirstOrDefault(item => item.Name == "PositionZ");
				var floatValue = layer.RelativeZ;

				if (string.IsNullOrEmpty(property.Name) == false)
				{
					float.TryParse((string)property.Value, out floatValue);
				}

				layer.RelativeZ = floatValue;
			}
		}
        #endregion

        #region Activity

        void CustomActivity(bool firstTimeCalled)
        {
            Camera.Main.X = Ship1.X;
            Camera.Main.Y = Ship1.Y;
			
            MurderLostBirds();
            DoBirdSpawning();
            UpdateShipSailsActivity();
        }

		internal void UpdateShipSailsActivity()
        {
            foreach(var ship in ShipList)
            {
                ///Placeholder wind until Victor implements it. 
                ship.
                    
                    ApplyWind(windDirection);
            }
        }
        #endregion

        void CustomDestroy()
        {


        }

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

        const float birdRadiusEstimate = 20;
		internal void MurderLostBirds()
        {
            for (int i = BirdList.Count - 1; i >= 0; i -= 1)
            {
                var bird = BirdList[i];
                if (bird.X + birdRadiusEstimate > this.Map.Width || bird.Y + birdRadiusEstimate < -this.Map.Height
                    || bird.X - birdRadiusEstimate < 0 || bird.Y - birdRadiusEstimate > 0)
                {
                    bird.Destroy();
                }
            }
        }

		internal void DoBirdSpawning()
        {
            if (BirdList.Count <= BirdCountMax)
            {
                var x = FlatRedBallServices.Random.Between(birdRadiusEstimate, Map.Width - birdRadiusEstimate);
                var y = FlatRedBallServices.Random.Between(birdRadiusEstimate, -Map.Height + birdRadiusEstimate);
                var altitude = FlatRedBallServices.Random.Between(Bird.MinBirdAltitude, Bird.MaxBirdAltitude);
                var bird = BirdFactory.CreateNew(x, y);
                bird.Altitude = altitude;
            }
        }


	}
}
