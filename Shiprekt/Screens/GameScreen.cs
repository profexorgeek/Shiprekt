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
using Keys = Microsoft.Xna.Framework.Input.Keys;
using FlatRedBall.TileEntities;
using FlatRedBall.TileCollisions;
using StateInterpolationPlugin;

namespace Shiprekt.Screens
{
    public partial class GameScreen
    {
        #region Fields/Properties

        Vector2 windDirection;

        private double SecondsLeft
        {
            get
            {
                var timePassed = TimeManager.CurrentScreenTime;

                var secondsLeft = MatchLengthInSeconds - timePassed;
                return secondsLeft;
            }
        }

        double windLastRandomized;

        #endregion

        #region Initialize

        void CustomInitialize()
        {
            TileEntityInstantiator.CreateEntitiesFrom(Map);

            InitializeShips();

            JoinedPlayerManager.ResetGameStats();

            RandomizeWind();

            // debug initialize needs to be before initializing cameras because
            // new ships may be added through debug logic.
            DebugInitialize();

            // do this after DebugInitialize so the debug ships are created too:
            PositionShipsOnSpawnPoints();

            InitializeCameras();

            DoInitialCloudSpawning();

            OffsetTilemapLayers();

            InitializeCollision();

        }

        private void PositionShipsOnSpawnPoints()
        {
            var numberOfSpawns = ShipList.Count;
            var spawnPoints = FlatRedBallServices.Random.MultipleIn(SpawnPointList, numberOfSpawns);

            for (int i = 0; i < ShipList.Count; i++)
            {
                var ship = ShipList[i];
                ship.X = spawnPoints[i].X;
                ship.Y = spawnPoints[i].Y;
            }
        }

        private void InitializeCameras()
        {
            Camera camera2 = null;
            Camera camera3 = null;
            Camera camera4 = null;

            switch(ShipList.Count)
            {
                case 1:
                    GameScreenGum.CurrentNumberOfPlayersState = 
                        GumRuntimes.GameScreenGumRuntime.NumberOfPlayers.One;
                    break;
                case 2:
                    Camera.Main.SetSplitScreenViewport(Camera.SplitScreenViewport.LeftHalf);
                    camera2 = new Camera();
                    camera2.SetSplitScreenViewport(Camera.SplitScreenViewport.RightHalf);
                    GameScreenGum.CurrentNumberOfPlayersState =
                        GumRuntimes.GameScreenGumRuntime.NumberOfPlayers.Two;
                    break;
                case 3:
                    Camera.Main.SetSplitScreenViewport(Camera.SplitScreenViewport.TopLeft);
                    camera2 = new Camera();
                    camera2.SetSplitScreenViewport(Camera.SplitScreenViewport.TopRight);
                    camera3 = new Camera();
                    camera3.SetSplitScreenViewport(Camera.SplitScreenViewport.BottomLeft);
                    GameScreenGum.CurrentNumberOfPlayersState =
                        GumRuntimes.GameScreenGumRuntime.NumberOfPlayers.Three;
                    break;
                case 4:
                    Camera.Main.SetSplitScreenViewport(Camera.SplitScreenViewport.TopLeft);
                    camera2 = new Camera();
                    camera2.SetSplitScreenViewport(Camera.SplitScreenViewport.TopRight);
                    camera3 = new Camera();
                    camera3.SetSplitScreenViewport(Camera.SplitScreenViewport.BottomLeft);
                    camera4 = new Camera();
                    camera4.SetSplitScreenViewport(Camera.SplitScreenViewport.BottomRight);
                    GameScreenGum.CurrentNumberOfPlayersState =
                        GumRuntimes.GameScreenGumRuntime.NumberOfPlayers.Four;
                    break;
            }

            Camera.Main.UsePixelCoordinates3D(0);

            if (camera2 != null)
            {
                SpriteManager.Cameras.Add(camera2);
                camera2.UsePixelCoordinates3D(0);
            }
            if (camera3 != null)
            {
                SpriteManager.Cameras.Add(camera3);
                camera3.UsePixelCoordinates3D(0);
            }
            if (camera4 != null)
            {
                SpriteManager.Cameras.Add(camera4);
                camera4.UsePixelCoordinates3D(0);
            }

            // before creating the final camera, set the min/maxes 
            for(int i = 0; i < SpriteManager.Cameras.Count; i++)
            {
                var camera = SpriteManager.Cameras[i];

                camera.SetBordersAtZ(0, -Map.Height, Map.Width, 0, 0);

                var cameraController = new CameraController();
                this.CameraControllerList.Add(cameraController);
                cameraController.TargetEntity = ShipList[i];
                cameraController.Camera = camera;
                cameraController.CurrentFollowTargetType = CameraController.FollowTargetType.Entity;
                cameraController.FollowImmediately = true;
            }

            // if there is more than one camera, then we need a final camera for UI
            if(SpriteManager.Cameras.Count > 1)
            {
                var topMostCamera = new Camera();
                topMostCamera.SetSplitScreenViewport(Camera.SplitScreenViewport.FullScreen);
                topMostCamera.BackgroundColor = Color.Transparent;
                topMostCamera.DrawsWorld = false;
                SpriteManager.Cameras.Add(topMostCamera);

                SpriteManager.RemoveLayer(HudLayer);
                topMostCamera.AddLayer(HudLayer);
            }
        }

        private void DebugInitialize()
        {

        }

        private void InitializeShips()
        {
            ShipFactory.RemoveList(this.DeadShipList);


            if(JoinedPlayerManager.JoinedPlayers.Count == 0)
            {
                var player = new JoinedPlayer();
                player.InputDevice = InputManager.Keyboard;
                player.ShipType = ShipType.Gray;

                JoinedPlayerManager.JoinedPlayers.Add(player);
            }

#if DEBUG
            if (DebuggingVariables.CreateExtraShips)
            {
                var player = new JoinedPlayer();
                player.InputDevice = InputManager.Xbox360GamePads[2];
                player.ShipType = ShipType.RedStripes;

                JoinedPlayerManager.JoinedPlayers.Add(player);
            }
#endif

            int index = 0;
            foreach(var player in JoinedPlayerManager.JoinedPlayers)
            {
                var ship = ShipFactory.CreateNew();
                ship.RotationZ = MathHelper.ToRadians(90);
                ship.SetTeam(index);
                ship.SetSail(player.ShipType.ToSailColor());
                ship.InitializeRacingInput(player.InputDevice);
                ship.AfterDying += ReactToShipDying;
                ship.BulletHit += ReactToBulletHit;
                index++;
            }
        }

        private void ReactToBulletHit(Bullet bullet)
        {
            var hitGround = GroundCollision.CollideAgainst(bullet);
            var shotMissEffect = ShotMissEffectFactory.CreateNew();
            shotMissEffect.IsGroundHit = hitGround;
            shotMissEffect.TriggerEffect(bullet.X, bullet.Y, bullet.RotationZ);
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

        private void InitializeCollision()
        {
            var decorCollision = Map.Collisions.FirstOrDefault(item => item.Name == "Decor");
            if(decorCollision != null)
            {
                foreach(var poly in decorCollision.Polygons)
                {
                    GroundCollision.Polygons.Add(poly);
                }

                if(GroundCollision.SortAxis == FlatRedBall.Math.Axis.X)
                {
                    GroundCollision.Polygons.SortXInsertionAscending();
                }
                else
                {
                    GroundCollision.Polygons.SortYInsertionAscending();
                }
            }


            GroundCollision.MergeRectangles();

            // We need to do custom logic before/after so we disable it and do manual collisions:
            ShipListVsGroundCollision.IsActive = false;
        }
        #endregion

        #region Activity

        void CustomActivity(bool firstTimeCalled)
        {
            DoCollisionActivity();

            DoUiActivity();

            DoWindChangeActivity();

            MurderLostBirds();

            DoBirdSpawning();

            UpdateShipSailsActivity();

            RemoveLostClouds();

            DoCloudSpawning();

            DoEndGameActivity();

            DoPauseUnpauseActivity();

            if (DebuggingVariables.EnableDebugKeyInput)
            {
                DoDebugInput();
            }
        }

        private void DoPauseUnpauseActivity()
        {
            if(this.IsPaused)
            {
                if(ShipList.Any(item => item.InputDevice.DefaultPauseInput.WasJustPressed))
                {
                    UnpauseThisScreen();
                }
            }
            else
            {
                if (ShipList.Any(item => item.InputDevice.DefaultPauseInput.WasJustPressed))
                {
                    PauseThisScreen();
                }
            }
        }

        private void DoWindChangeActivity()
        {
            if(PauseAdjustedSecondsSince(windLastRandomized) > TimeBetweenWindDirectionChange)
            {
                RandomizeWind();
            }
        }

        private void RandomizeWind()
        {
            windDirection = FlatRedBallServices.Random.RadialVector2(1, 1);
            windLastRandomized = PauseAdjustedCurrentTime;

            System.Diagnostics.Debug.WriteLine($"Changed wind to {windDirection} at {PauseAdjustedCurrentTime.ToString("0.00")}");
        }

        private void DoCollisionActivity()
        {
            foreach(var ship in ShipList)
            {
                ship.RecordBeforeCollisionState();
            }

            ShipListVsGroundCollision.DoCollisions();

            foreach (var ship in ShipList)
            {
                ship.RecordAfterCollisionState();
            }
        }

        private void DoUiActivity()
        {
            var secondsLeft = SecondsLeft;

            var secondsRoundedUp = System.Math.Ceiling(secondsLeft);

            int minutesLeft = (int)(secondsRoundedUp / 60);
            int remainder = (int)(secondsRoundedUp) % 60;

            var timeDisplay = $"{minutesLeft}:{remainder.ToString("00")}";

            GameScreenGum.TextInstance.Text = timeDisplay;
        }

        private void ReactToShipDying(Ship ship)
        {
            // to prevent collisions, etc
            ShipList.Remove(ship);
            DeadShipList.Add(ship);

            ship.Visible = false;

            var randomSpawnPoint = FlatRedBallServices.Random.In(SpawnPointList);

            var controller = CameraControllerList.First(item => item.TargetEntity == ship);
            controller.FollowImmediately = false;

            float timeToWatchSinkingShip = 1;
            float timeToTweenToNewPosition = 2;
            float timeToLookAtNewPosition = .5f;

            this.Call(() =>
            {
                controller.Tween("X", to: randomSpawnPoint.X, 
                    during: timeToTweenToNewPosition, 
                    interpolation: FlatRedBall.Glue.StateInterpolation.InterpolationType.Quadratic, 
                    easing: FlatRedBall.Glue.StateInterpolation.Easing.InOut);

                controller.Tween("Y", to: randomSpawnPoint.Y, 
                    during: timeToTweenToNewPosition,
                    interpolation: FlatRedBall.Glue.StateInterpolation.InterpolationType.Quadratic,
                    easing: FlatRedBall.Glue.StateInterpolation.Easing.InOut);

            })
            .After(timeToWatchSinkingShip);

            this.Call(() =>
            {
                ship.ResetHealth();
                ship.X = randomSpawnPoint.X;
                ship.Y = randomSpawnPoint.Y;

                DeadShipList.Remove(ship);
                ShipList.Add(ship);
                controller.FollowImmediately = true;


                ship.Visible = true;
            })
            .After(timeToWatchSinkingShip + timeToTweenToNewPosition + timeToLookAtNewPosition);
        }

        internal void UpdateShipSailsActivity()
        {
            foreach(var ship in ShipList)
            {
                ship.ApplyWind(windDirection);
            }
        }

        private void DoEndGameActivity()
        {
            if(SecondsLeft < 0)
            {
                EndGame();
            }
        }

        private void EndGame()
        {
            MoveToScreen(typeof(MainMenu));
        }
        #endregion

        void CustomDestroy()
        {
            while(SpriteManager.Cameras.Count > 1)
            {
                SpriteManager.Cameras.RemoveAt(SpriteManager.Cameras.Count - 1);
            }
            Camera.Main.SetSplitScreenViewport(Camera.SplitScreenViewport.FullScreen);
            Camera.Main.ClearBorders();

        }

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

        #region Bird Logic

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

        #endregion

        #region Cloud Logic

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
            var windVelocity = windDirection * WindMagnitude;
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

                // Consider half of game screen perimiter, from 0,0 to Width,-Height.
                float spawnRangeMax = Map.Height + Map.Width;
                // Find a number along that perimter, spawn based on that number with wind determining top/right/bottom/left.
                float spawnPointInRange = FlatRedBallServices.Random.Between(0, spawnRangeMax);

                // NOTE: For primary directions, clouds will spawn outside and be culled immediately until RNGesus sees fit to put them all on the appropriate side.
                float cloudSpawnX = 0;
                float cloudSpawnY = 0;
                if (windDirection.X >= 0 && windDirection.Y >= 0)
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
                else if (windDirection.X >= 0 && windDirection.Y <= 0)
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
                else if (windDirection.X <= 0 && windDirection.Y >= 0)
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
                else if (windDirection.X <= 0 && windDirection.Y <= 0)
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

        #endregion

        void DoDebugInput()
        {
            var kb = FlatRedBall.Input.InputManager.Keyboard;
            if(kb.KeyDown(Keys.LeftControl) || kb.KeyDown(Keys.RightControl))
            {
                // CTRL + F - Force kill all ships
                if(kb.KeyReleased(Keys.F))
                {
                    for(var i = 0; i < ShipList.Count; i++)
                    {
                        ShipList[i].Die();
                    }
                }
            }
        }

        public static void MoveToRandomLevel()
        {
            var derivedTypes = typeof(GameScreen).Assembly
                .GetTypes()
                .Where(item => item.BaseType == typeof(GameScreen))
                .ToArray();

            var randomType = FlatRedBallServices.Random.In(derivedTypes);

            FlatRedBall.Screens.ScreenManager.CurrentScreen.MoveToScreen(randomType);
        }
    }
}
