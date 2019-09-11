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

namespace Shiprekt.Screens
{
	public partial class MainMenu
	{

		void CustomInitialize()
		{


		}

		void CustomActivity(bool firstTimeCalled)
		{
            if(InputManager.Keyboard.AnyKeyPushed() || GuiManager.Cursor.PrimaryClick ||
                InputManager.Xbox360GamePads.Any(item => item.AnyButtonPushed()))
            {
                this.MoveToScreen(typeof(Level1));
            }

		}

		void CustomDestroy()
		{


		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
