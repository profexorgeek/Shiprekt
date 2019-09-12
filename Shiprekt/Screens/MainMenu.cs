﻿using System;
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

namespace Shiprekt.Screens
{
	public partial class MainMenu
	{

		void CustomInitialize()
		{
            MainMenuGum.UnjoinAll();

		}

		void CustomActivity(bool firstTimeCalled)
		{
            int minimumPlayers = 1;

            foreach(var gamePad in InputManager.Xbox360GamePads)
            {
                var pressedStart = gamePad.ButtonPushed(Xbox360GamePad.Button.Start);
                var pressedBack = gamePad.ButtonPushed(Xbox360GamePad.Button.Back);
                var pressedAnything = gamePad.AnyButtonPushed();

                if(!JoinedPlayerManager.IsJoined(gamePad) && pressedAnything)
                {
                    JoinWith(gamePad);
                }
                else if(JoinedPlayerManager.IsJoined(gamePad) && pressedBack)
                {
                    UnjoinWith(gamePad);
                }
                else if(pressedStart && JoinedPlayerManager.JoinedPlayers.Count >= minimumPlayers)
                {
                    GoToGameScreen();
                }
                
            }



            //var keyboard = InputManager.Keyboard;

            //if (!JoinedPlayerManager.IsJoined(keyboard) && keyboard.AnyKeyPushed())
            //{
            //    JoinWith(keyboard);
            //}
            //else if (JoinedPlayerManager.IsJoined(keyboard) && 
            //    keyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.Escape))
            //{
            //    UnjoinWith(keyboard);
            //}
            //else if (keyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.Enter) && 
            //    JoinedPlayerManager.JoinedPlayers.Count >= minimumPlayers)
            //{
            //    GoToGameScreen();
            //}

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
            this.MoveToScreen(typeof(Level1));
        }

        void CustomDestroy()
		{


		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
