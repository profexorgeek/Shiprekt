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



namespace Shiprekt.Screens
{
	public partial class GameScreen
	{

		void CustomInitialize()
		{
            DummyShip.InitializeRacingInput(InputManager.Xbox360GamePads[0]);
            DummyShip.TeamIndex = 1;

            FlatRedBallServices.Game.IsMouseVisible = true;

            OffsetTilemapLayers();
		}

        private void OffsetTilemapLayers()
        {
            foreach(var layer in Map.MapLayers)
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


		}

		void CustomDestroy()
		{


		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
