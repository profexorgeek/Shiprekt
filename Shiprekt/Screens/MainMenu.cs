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

            if(JoinedPlayerManager.JoinedPlayers.Any())
            {
                // Max crashes if the list is empty
                var best = JoinedPlayerManager.JoinedPlayers.Max(
                    item => item.LastGameKills - item.LastGameDeaths);

                foreach(var player in JoinedPlayerManager.JoinedPlayers)
                {
                    var frame = MainMenuGum.JoinWith(player.ShipType.ToGum());

                    if(player.LastGameKills - player.LastGameDeaths == best)
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
            JoinActivity();
            ShipFiringActivity();
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

            var bullet = BulletFactory.CreateNew();
            var worldPos = new Vector3();
            var z = 20;
            var bulletVelocity = 600f;

            Vector2 gumPos = new Vector2();
            if (left)
                gumPos = ship.GunLeftAbsolutePosition;
            else
                gumPos = ship.GunRightAbsolutePosition; 

            worldPos.X = Camera.Main.WorldXAt(gumPos.X, z) + ship.GetAbsoluteWidth() / 2;
            worldPos.Y = Camera.Main.WorldYAt(gumPos.Y, z) - ship.GetAbsoluteHeight() / 2;
            worldPos.Z = z;

            bullet.Position = worldPos;
            bullet.YVelocity = 600;
            bullet.YAcceleration = -600;
            if (left) bullet.XVelocity = -bulletVelocity;
            else bullet.XVelocity = bulletVelocity;
            bullet.Call(bullet.Destroy).After(3);             
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
            if(newPlayer != null)
            {
                MainMenuGum.JoinWith(newPlayer.ShipType.ToGum());
            }
        }

        private void UnjoinWith(IInputDevice gamePad)
        {
            var unjoined = JoinedPlayerManager.DropPlayer(gamePad);

            if(unjoined != null)
            {
                MainMenuGum.UnjoinWith(unjoined.ShipType.ToGum());
            }
        }

        private void GoToGameScreen()
        {
            //this.MoveToScreen(typeof(Level1));

            GameScreen.MoveToRandomLevel();
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
