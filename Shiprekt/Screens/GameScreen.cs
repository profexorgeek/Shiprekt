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

        void CustomInitialize()
        {
            DummyShip.InitializeRacingInput(InputManager.Xbox360GamePads[0]);
            DummyShip.SetTeam(1);
            Ship1.SetTeam(2); 
			
            FlatRedBallServices.Game.IsMouseVisible = true;

            OffsetTilemapLayers();
        }

        private void OffsetTilemapLayers()
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

        void CustomActivity(bool firstTimeCalled)
        {
            Camera.Main.X = Ship1.X;
            Camera.Main.Y = Ship1.Y;

            MurderLostBirds();
            DoBirdSpawning();
            UpdateShipSailsActivity();
        }

        void CustomDestroy()
        {


        }

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

        const float birdRadiusEstimate = 20;
        void MurderLostBirds()
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

        void DoBirdSpawning()
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

        private void UpdateShipSailsActivity()
        {
            foreach(var ship in ShipList)
            {
                ///Placeholder wind until Victor implements it. 
                ship.ApplyWind(new Vector2(0,1));
            }
        }
    }
}
