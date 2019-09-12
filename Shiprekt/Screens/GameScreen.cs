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
using Shiprekt.Managers;
using Shiprekt.DataTypes;

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
            InitializeShips();
            Camera.Main.Z = 500;
            FlatRedBallServices.Game.IsMouseVisible = true;

            windDirection = Vector2.UnitX;// FlatRedBallServices.Random.RadialVector2(1, 1);

            DoInitialCloudSpawning();

            OffsetTilemapLayers();
        }

        private void InitializeShips()
        {
            int index = 0;
            foreach(var player in JoinedPlayerManager.JoinedPlayers)
            {
                var ship = ShipFactory.CreateNew();
                ship.X = 400 + 50*index;
                ship.Y = -400;
                ship.SetTeam(index);
                ship.SetSail(player.ShipType.ToSailColor());
                ship.InitializeRacingInput(player.InputDevice);
                index++;
            }
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
            var shipToFollow = ShipList[0];
            Camera.Main.X = shipToFollow.X;
            Camera.Main.Y = shipToFollow.Y;
			
            MurderLostBirds();
            DoBirdSpawning();
            UpdateShipSailsActivity();
            RemoveLostClouds();
            DoCloudSpawning();
        }

		internal void UpdateShipSailsActivity()
        {
            foreach(var ship in ShipList)
            {
                ship.ApplyWind(windDirection);
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
            const float offScreenBuffer = 10;
            for (int i = BirdList.Count - 1; i >= 0; i -= 1)
            {
                var bird = BirdList[i];
                if (bird.X > this.Map.Width + birdRadiusEstimate + offScreenBuffer || bird.Y < -this.Map.Height - birdRadiusEstimate - offScreenBuffer
                    || bird.X < 0 - birdRadiusEstimate - offScreenBuffer || bird.Y > 0 + birdRadiusEstimate + offScreenBuffer)
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

        // TODO: Get wind from game.
        static Vector2 TEMP_DEFAULT_WIND = new Vector2(30, 5);

        const float cloudRadiusEstimate = 50;
        static float SecondsToNextCloudMax = 3;
        static float SecondsToNextCloud = 0;
        void RemoveLostClouds()
        {
            const float offScreenBuffer = 10;
            for (int i = CloudList.Count - 1; i >= 0; i -= 1)
            {
                var cloud = CloudList[i];
                if (cloud.X > this.Map.Width + cloudRadiusEstimate + offScreenBuffer || cloud.Y < -this.Map.Height - cloudRadiusEstimate - offScreenBuffer
                    || cloud.X < 0 - cloudRadiusEstimate - offScreenBuffer || cloud.Y > 0 + cloudRadiusEstimate + offScreenBuffer)
                {
                    cloud.Destroy();
                }
            }
        }
        void SpawnCloud(float x, float y)
        {
            var windVelocity = TEMP_DEFAULT_WIND;
            var cloud = CloudFactory.CreateNew(x, y);
            cloud.Altitude = FlatRedBallServices.Random.Between(Cloud.CloudAltitudeMin, Cloud.CloudAltitudeMax);
            cloud.Velocity.X = windVelocity.X;
            cloud.Velocity.Y = windVelocity.Y;
            cloud.PickRandomSprite();
        }
        void DoInitialCloudSpawning()
        {
            // Spawn portion of the cloud amount initially
            for (int i = 0; i < CloudCountMax / 6; i += 1)
            {
                var x = FlatRedBallServices.Random.Between(cloudRadiusEstimate, Map.Width - cloudRadiusEstimate);
                var y = FlatRedBallServices.Random.Between(cloudRadiusEstimate, -Map.Height + cloudRadiusEstimate);
                SpawnCloud(x, y);
            }
        }
        void DoCloudSpawning()
        {
            if (CloudList.Count <= CloudCountMax)
            {
                // Limit random cloud spawning to varied timer
                SecondsToNextCloud -= TimeManager.SecondDifference;
                if (SecondsToNextCloud > 0)
                {
                    return;
                }
                SecondsToNextCloud = FlatRedBallServices.Random.Between(0, SecondsToNextCloudMax);

                var windVelocity = TEMP_DEFAULT_WIND;

                // Consider half of game screen perimiter, from 0,0 to Width,-Height.
                float spawnRangeMax = Map.Height + Map.Width;
                // Find a number along that perimter, spawn based on that number with wind determining top/right/bottom/left.
                float spawnPointInRange = FlatRedBallServices.Random.Between(0, spawnRangeMax);

                // NOTE: For primary directions, clouds will spawn outside and be culled immediately until RNGesus sees fit to put them all on the appropriate side.
                float cloudSpawnX = 0;
                float cloudSpawnY = 0;
                if (windVelocity.X >= 0 && windVelocity.Y >= 0)
                {
                    // Wind goes up/right
                    if (spawnPointInRange <= Map.Height)
                    {
                        // Spawn at point left of map
                        cloudSpawnX = -cloudRadiusEstimate;
                        cloudSpawnY = FlatRedBallServices.Random.Between(-Map.Height, 0);
                    }
                    else // (spawnPointInRange >= Map.Height)
                    {
                        // Spawn at point below map
                        cloudSpawnX = FlatRedBallServices.Random.Between(0, Map.Width);
                        cloudSpawnY = Map.Height + cloudRadiusEstimate;
                    }
                }
                else if (windVelocity.X >= 0 && windVelocity.Y <= 0)
                {
                    // Wind goes down/right
                    if (spawnPointInRange <= Map.Height)
                    {
                        // Spawn at point left of map
                        cloudSpawnX = -cloudRadiusEstimate;
                        cloudSpawnY = FlatRedBallServices.Random.Between(-Map.Height, 0);
                    }
                    else // (spawnPointInRange >= Map.Height)
                    {
                        // Spawn at point above map
                        cloudSpawnX = FlatRedBallServices.Random.Between(0, Map.Width);
                        cloudSpawnY = -cloudRadiusEstimate;
                    }
                }
                else if (windVelocity.X <= 0 && windVelocity.Y >= 0)
                {
                    // Wind goes up/left
                    if (spawnPointInRange <= Map.Height)
                    {
                        // Spawn at point right of map
                        cloudSpawnX = Map.Width + cloudRadiusEstimate;
                        cloudSpawnY = FlatRedBallServices.Random.Between(-Map.Height, 0);
                    }
                    else // (spawnPointInRange >= Map.Height)
                    {
                        // Spawn at point below map
                        cloudSpawnX = FlatRedBallServices.Random.Between(0, Map.Width);
                        cloudSpawnY = Map.Height + cloudRadiusEstimate;
                    }
                }
                else if (windVelocity.X <= 0 && windVelocity.Y <= 0)
                {
                    // Wind goes down/left
                    // Spawn from right/top
                    if (spawnPointInRange <= Map.Height)
                    {
                        // Spawn at point right of map
                        cloudSpawnX = Map.Width + cloudRadiusEstimate;
                        cloudSpawnY = FlatRedBallServices.Random.Between(-Map.Height, 0);
                    }
                    else // (spawnPointInRange >= Map.Height)
                    {
                        // Spawn at point above map
                        cloudSpawnX = FlatRedBallServices.Random.Between(0, Map.Width);
                        cloudSpawnY = -cloudRadiusEstimate;
                    }
                }
                SpawnCloud(cloudSpawnX, cloudSpawnY);
            }
        }
    }
}
