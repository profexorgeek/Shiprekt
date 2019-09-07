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
using static Shiprekt.Entities.ShipSail;

namespace Shiprekt.Entities
{
	public partial class Ship
	{
		public int TeamIndex { get; private set; }

		IPressableInput shootLeftInput;
		IPressableInput shootRightInput;
		I1DInput sailTurningInput;

		float cachedForwardAcceleration;
		float cachedBrakeStoppingAcceleration; 

		/// <summary>
		/// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
		/// This method is called when the Entity is added to managers. Entities which are instantiated but not
		/// added to managers will not have this method called.
		/// </summary>
		private void CustomInitialize()
		{
			CacheShipData();
		}

		private void CacheShipData()
		{
			cachedForwardAcceleration = CarData.ForwardAcceleration;
			cachedBrakeStoppingAcceleration = CarData.BrakeStoppingAcceleration; 
		}

		partial void CustomInitializeTopDownInput()
		{
			if (InputDevice is Xbox360GamePad gamePad)
			{
				shootLeftInput = gamePad.LeftTrigger
					.Or(gamePad.GetButton(Xbox360GamePad.Button.LeftShoulder));
				shootRightInput = gamePad.RightTrigger
					.Or(gamePad.GetButton(Xbox360GamePad.Button.RightShoulder));
				sailTurningInput = gamePad.RightStick.Horizontal;
				IPressableInput gas = gamePad.GetButton(Xbox360GamePad.Button.B);  
				Gas = new DelegateBasedPressableInput(() => !gas.IsDown, () => gas.WasJustReleased, () => gas.WasJustPressed);
			}
			else if (InputDevice is Keyboard keyboard)
			{
				shootLeftInput = keyboard.GetKey(Microsoft.Xna.Framework.Input.Keys.Q)
					.Or(InputManager.Mouse.GetButton(Mouse.MouseButtons.LeftButton));

				shootRightInput = keyboard.GetKey(Microsoft.Xna.Framework.Input.Keys.E)
					.Or(InputManager.Mouse.GetButton(Mouse.MouseButtons.RightButton));
				sailTurningInput = InputManager.Keyboard.Get1DInput(Microsoft.Xna.Framework.Input.Keys.Right, Microsoft.Xna.Framework.Input.Keys.Left);
				IPressableInput gas = InputManager.Keyboard.GetKey(Microsoft.Xna.Framework.Input.Keys.Space); 
				Gas = new DelegateBasedPressableInput(() => !gas.IsDown, () => gas.WasJustReleased, () => gas.WasJustPressed);
			}
		}

		private void CustomActivity()
		{
			DoShootingActivity();
			DoSailTurningActivity();
		}

		internal void TakeDamage(int damageAmount)
		{
			
		}

		private void DoShootingActivity()
		{
			if (shootLeftInput.WasJustPressed)
			{
				Shoot(this.Left.ToVector2());
			}
			else if (shootRightInput.WasJustPressed)
			{
				Shoot(this.Right.ToVector2());

			}
		}

		private void DoSailTurningActivity()
		{
			var turnSpeed = sailTurningInput.Value * ShipEntityValuesInstance.SailRotationSpeed;
			var left = ShipEntityValuesInstance.MaxSailRotation * ((float)Math.PI / 180);
			var right = 360 * ((float)Math.PI / 180) - ShipEntityValuesInstance.MaxSailRotation * ((float)Math.PI / 180);
			var middle = (float)Math.PI;

			var maxLeftTurningRight = ShipSailInstance.RelativeRotationZ == left && turnSpeed < 0;
			var maxRightTurningLeft = ShipSailInstance.RelativeRotationZ == right && turnSpeed > 0;
			var safeZone = ShipSailInstance.RelativeRotationZ < left || ShipSailInstance.RelativeRotationZ > right;
			if (maxLeftTurningRight || maxRightTurningLeft || safeZone)
			{
				ShipSailInstance.RelativeRotationZVelocity = turnSpeed; 
			}
			else
			{
				if (ShipSailInstance.RelativeRotationZ > left && ShipSailInstance.RelativeRotationZ < middle)
				{
					ShipSailInstance.RelativeRotationZ = left; 
				}
				else if (ShipSailInstance.RelativeRotationZ < right && ShipSailInstance.RelativeRotationZ > middle)
				{
					ShipSailInstance.RelativeRotationZ = right; 
				}
				ShipSailInstance.RelativeRotationZVelocity = 0;
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

		public void ApplyWind(Vector2 wind)
		{
			//Apply velocity based on the wind. 
			var sc = ShipSailInstance.GetAccelerationCoefficient(wind, 30);
			CarData.ForwardAcceleration = sc * cachedForwardAcceleration; 

			//Hack to account for lack of min-speed in the racecar plugin. 
			if (Velocity.Length() < ShipEntityValuesInstance.MinSpeed && Gas.IsDown)
			{
				CarData.ForwardAcceleration = cachedForwardAcceleration; 
			}
			//Hack to account for lack of drag in the racecar plugin. 
			else if (Velocity.Length() > ShipEntityValuesInstance.MinSpeed && Gas.IsDown)
			{
				Velocity -= this.Forward * ShipEntityValuesInstance.ShipDrag * TimeManager.SecondDifference;
			}

			//Change the sail visual. 
			ShipSailInstance.UpdateSailVisual(wind); 
		}

		public void SetTeam(int teamIndex)
		{
			TeamIndex = teamIndex; 
			switch(teamIndex)
			{
				case 0:
					ShipSailInstance.CurrentSailColorState = SailColor.Green;
					break;
				case 1:
					ShipSailInstance.CurrentSailColorState = SailColor.Pink;
					break;
				case 2:
					ShipSailInstance.CurrentSailColorState = SailColor.RedStripe;
					break;
				default:
					ShipSailInstance.CurrentSailColorState = SailColor.Black;
					break; 
			}
		}
	}
}
