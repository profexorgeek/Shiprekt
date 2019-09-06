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
	public partial class Ship
	{

        public int TeamIndex { get; set; }

        IPressableInput shootLeftInput;
        IPressableInput shootRightInput;


        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
        private void CustomInitialize()
		{


		}

        partial void CustomInitializeTopDownInput()
        {
            if(InputDevice is Xbox360GamePad gamePad)
            {
                shootLeftInput = gamePad.LeftTrigger
                    .Or(gamePad.GetButton(Xbox360GamePad.Button.LeftShoulder));
                shootRightInput = gamePad.RightTrigger
                    .Or(gamePad.GetButton(Xbox360GamePad.Button.RightShoulder));
            }
            else if(InputDevice is Keyboard keyboard)
            {
                shootLeftInput = keyboard.GetKey(Microsoft.Xna.Framework.Input.Keys.Q)
                    .Or(InputManager.Mouse.GetButton(Mouse.MouseButtons.LeftButton));

                shootRightInput = keyboard.GetKey(Microsoft.Xna.Framework.Input.Keys.E)
                    .Or(InputManager.Mouse.GetButton(Mouse.MouseButtons.RightButton));

            }

        }

        private void CustomActivity()
		{
            DoShootingActivity();


		}

        internal void TakeDamage(int damageAmount)
        {

        }

        private void DoShootingActivity()
        {
            if(shootLeftInput.WasJustPressed)
            {
                Shoot(this.Left.ToVector2());
            }
            else if(shootRightInput.WasJustPressed)
            {
                Shoot(this.Right.ToVector2());

            }
        }

        private void Shoot(Vector2 bulletDirection)
        {
            var bullet = Factories.BulletFactory.CreateNew(this.X, this.Y);

            bullet.Velocity = (bulletDirection * Bullet.BulletSpeed).ToVector3();
            bullet.TeamIndex = this.TeamIndex;
            var bulletDuration = Bullet.BulletDistance / Bullet.BulletSpeed;

            bullet.Call(bullet.HitSurface).After(bulletDuration);
        }

        private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
