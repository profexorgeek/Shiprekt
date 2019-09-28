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
using FlatRedBall.Gui;
using Shiprekt.Managers;
using Shiprekt.DataTypes;
using Microsoft.Xna.Framework;
using Shiprekt.Factories;
using FlatRedBall.Debugging;
using Shiprekt.Entities;
using RenderingLibrary;
using Camera = FlatRedBall.Camera;
using FlatRedBall.Math;
using Shiprekt.GumRuntimes;

namespace Shiprekt.Screens
{
    public partial class MainMenu
    {
        #region Initialize

        void CustomInitialize()
        {
            UpdateUiToReflectJoinedPlayers();
        }

        private void UpdateUiToReflectJoinedPlayers()
        {
            // To wipe any state that may come from Gum layout:
            MainMenuGum.UnjoinAll();

            if (JoinedPlayerManager.JoinedPlayers.Any())
            {
                // Max crashes if the list is empty
                var best = JoinedPlayerManager.JoinedPlayers.Max(
                    item => item.LastGameKills - item.LastGameDeaths);

                foreach (var player in JoinedPlayerManager.JoinedPlayers)
                {
                    var frame = MainMenuGum.JoinWith(player.ShipType.ToGum());

                    if (player.LastGameKills - player.LastGameDeaths == best)
                    {
                        frame.CurrentWinOrNormalState =
                            GumRuntimes.JoinableShipAndStatusRuntime.WinOrNormal.Winner;
                    }
                    else
                    {
                        frame.CurrentWinOrNormalState =
                            GumRuntimes.JoinableShipAndStatusRuntime.WinOrNormal.Normal;
                    }

                    frame.KillsText = $"Kills: {player.LastGameKills}";
                    frame.DeathsText = $"Deaths: {player.LastGameDeaths}";
                }
            }
        }

        #endregion

        #region Activity

        void CustomActivity(bool firstTimeCalled)
        {
            if (InputManager.Keyboard.KeyDown(Microsoft.Xna.Framework.Input.Keys.NumPad1))
            {
                Camera.Main.X--; 
            }
            else if (InputManager.Keyboard.KeyDown(Microsoft.Xna.Framework.Input.Keys.NumPad2))
            {
                Camera.Main.X++; 
            }
            JoinActivity();
            ShipFiringActivity();
            BulletCollisionActivity();
            BulletActivity();
        }

        private void JoinActivity()
        {
            int minimumPlayers = 1;

            foreach (var gamePad in InputManager.Xbox360GamePads)
            {
                var pressedStart = gamePad.ButtonPushed(Xbox360GamePad.Button.Start);
                var pressedBack = gamePad.ButtonPushed(Xbox360GamePad.Button.Back);
                var pressedAnything = gamePad.AnyButtonPushed();

                if (!JoinedPlayerManager.IsJoined(gamePad) && pressedAnything)
                {
                    JoinWith(gamePad);
                }
                else if (JoinedPlayerManager.IsJoined(gamePad) && pressedBack)
                {
                    UnjoinWith(gamePad);
                }
                else if (pressedStart && JoinedPlayerManager.JoinedPlayers.Count >= minimumPlayers)
                {
                    GoToGameScreen();
                }

            }

            var keyboard = InputManager.Keyboard;

            if (!JoinedPlayerManager.IsJoined(keyboard) && keyboard.AnyKeyPushed())
            {
                JoinWith(keyboard);
            }
            else if (JoinedPlayerManager.IsJoined(keyboard) &&
                keyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                UnjoinWith(keyboard);
            }
            else if (keyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.Enter) &&
                JoinedPlayerManager.JoinedPlayers.Count >= minimumPlayers)
            {
                GoToGameScreen();
            }

        }
        private void BulletActivity()
        {
            foreach(var bullet in BulletList)
            {
                if (bullet.Y < SeaLevel)
                {
                    bullet.Destroy(); 
                }
            }
        }

        private void ShipFiringActivity()
        {
            foreach (var gamePad in InputManager.Xbox360GamePads)
            {
                var boomLeft = gamePad.ButtonPushed(Xbox360GamePad.Button.LeftTrigger);
                var boomRight = gamePad.ButtonPushed(Xbox360GamePad.Button.RightTrigger);

                if (boomLeft || boomRight)
                {
                    var player = JoinedPlayerManager.GetPlayer(gamePad);
                    if (player != null)
                    {
                        Shoot(boomLeft, player);
                        PlayShotSound();
                    }
                }

            }
            var keyboard = InputManager.Keyboard;
            var betterPlayer = JoinedPlayerManager.GetPlayer(keyboard);
            if (betterPlayer != null)
            {
                var boomLeft = keyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.Q) || InputManager.Mouse.ButtonPushed(Mouse.MouseButtons.LeftButton);
                var boomRight = keyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.E) || InputManager.Mouse.ButtonPushed(Mouse.MouseButtons.RightButton);
                if (boomLeft || boomRight)
                {
                    Shoot(boomLeft, betterPlayer);
                    PlayShotSound();
                }
            }
        }

        void Shoot(bool left, JoinedPlayer player)
        {
            var ship = MainMenuGum.JoinedPlayerContainer.Children
            .FirstOrDefault(item => item.SailDesignState == player.ShipType.ToGum());            

            var bulletVelocity = 600f;
            var z = 20; 
            ContainerRuntime runtime; 
            
            if (left)
                runtime = ship.GetGunLeft;
            else
                runtime = ship.GetGunRight;

            var screenX = runtime.GetAbsoluteX() * (CameraSetup.Data.ScaleGum / 100) * (CameraSetup.Data.Scale / 100);
            var screenY = runtime.GetAbsoluteY() * (CameraSetup.Data.ScaleGum / 100) * (CameraSetup.Data.Scale / 100); 

            var bullet = BulletFactory.CreateNew(LayerInstance);
            bullet.Position = new Vector3()
            {
                X = Camera.Main.WorldXAt(screenX, z),
                Y = Camera.Main.WorldYAt(screenY, z),
                Z = z,
            };
            bullet.YVelocity = 100;
            bullet.YAcceleration = -600;
            bullet.TeamIndex = (int)ship.SailDesignState.Value;
            if (left)
            {
                bullet.XVelocity = -bulletVelocity;
                ship.StopAnimations();
                ship.RockRightAnimation.Play();
            }
            else
            {
                bullet.XVelocity = bulletVelocity;
                ship.StopAnimations();
                ship.RockLeftAnimation.Play();
            }
        }


        private void PlayShotSound()
        {
            switch (FlatRedBallServices.Random.Next(3))
            {
                case 0: cannon01.Play(); break;
                case 1: cannon02.Play(); break;
                case 2: cannon03.Play(); break;
            }
        }

        private void JoinWith(IInputDevice gamePad)
        {
            var newPlayer = JoinedPlayerManager.Join(gamePad);
            if (newPlayer != null)
            {
                MainMenuGum.JoinWith(newPlayer.ShipType.ToGum());
            }
        }

        private void UnjoinWith(IInputDevice gamePad)
        {
            var unjoined = JoinedPlayerManager.DropPlayer(gamePad);

            if (unjoined != null)
            {
                MainMenuGum.UnjoinWith(unjoined.ShipType.ToGum());
            }
        }

        private void GoToGameScreen()
        {
            //this.MoveToScreen(typeof(Level1));

            GameScreen.MoveToRandomLevel();
        }

        private void BulletCollisionActivity()
        {
            var z = 20;
            foreach (var bullet in BulletList)
            {
                for (int i = 0; i < MainMenuGum.JoinedPlayerContainer.Children.Count(); i++)
                {
                    var ship = MainMenuGum.JoinedPlayerContainer.Children.ElementAt(i);

                    //Ship ships that aren't joined. 
                    if (ship.CurrentJoinedCategoryState == JoinableShipAndStatusRuntime.JoinedCategory.NotJoined) continue; 

                    //Don't check collision if the bullet is on the same team as the ship. 
                    if (bullet.TeamIndex == (int)ship.SailDesignState) continue;
                    var shipSprite = ship.ShipFront;
                    var scaleFactor = (CameraSetup.Data.Scale / 100) * (CameraSetup.Data.ScaleGum / 100); 
                    var left = Camera.Main.WorldXAt(shipSprite.GetAbsoluteLeft() * scaleFactor, z);
                    var right = Camera.Main.WorldXAt(shipSprite.GetAbsoluteRight() * scaleFactor, z);
                    var top = Camera.Main.WorldYAt(shipSprite.GetAbsoluteTop() * scaleFactor, z);
                    var bottom = Camera.Main.WorldYAt(shipSprite.GetAbsoluteBottom() * scaleFactor, z);
                    if (bullet.X >= left && bullet.X <= right && bullet.Y <= top && bullet.Y >= bottom)
                    {
                        var impact = ShipImpactMenuFactory.CreateNew(LayerInstance);
                        impact.Position = bullet.Position;
                        impact.Z = 20; 
                        impact.EmitEffectParticles(bullet.Position.ToVector2(), -bullet.Velocity.Normalized().ToVector2());

                        var diceRoll = FlatRedBallServices.Random.Next(0, 101); 
                        if (diceRoll < ChanceOfBirbSpawn)
                        {
                            BirbSpawnerInstance.SpawnBirbs(bullet.Position);
                        }
                        
                        bullet.Destroy();
                        break;
                    }
                }
            }
        }

        #endregion

        void CustomDestroy()
        {

        }

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

    }
}
