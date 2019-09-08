using System;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Specialized;
using FlatRedBall.Audio;
using FlatRedBall.Screens;
using Shiprekt.Entities;
using Shiprekt.Screens;
using FlatRedBall.Math.Geometry;
using Microsoft.Xna.Framework;

namespace Shiprekt.Screens
{
    public partial class GameScreen
    {
		Circle shipCollisionTestCircle;

		void OnBulletListVsShipListCollisionOccurred (Entities.Bullet bullet, Entities.Ship ship) 
        {
            if(bullet.TeamIndex != ship.TeamIndex)
            {
                bullet.CollideAgainstBounce(ship, .05f, 1, 1);

                bullet.Destroy();

                ship.TakeDamage(Bullet.DamageToDeal);
            }
        }
        void OnShipListVsShipListCollisionOccurred (Entities.Ship first, Entities.Ship second) 
        {
			//If the ship is blocked in the front, we want to apply some steering motion to that ship either toward the other ships front or rear vector, whichever is closest.
			TrySteerShipAwayFromCollision(first, second);
			TrySteerShipAwayFromCollision(second, first);
			first.CollideAgainstBounce(second, 1, 1, .3f); 
		}
		
		private bool TrySteerShipAwayFromCollision(Ship ship1, Ship other)
		{
			//Put the collision in front of the ship. 
			if (shipCollisionTestCircle == null) shipCollisionTestCircle = new Circle();
			shipCollisionTestCircle.Radius = 14;
			shipCollisionTestCircle.Position = ship1.Position;
			shipCollisionTestCircle.Position += ship1.Forward * 25;
			
			//If that ship is in front of this ship, apply steering. 
			if (shipCollisionTestCircle.CollideAgainst(other.Collision))
			{
				//RotateShip
				var shipForward = ship1.Forward;
				var otherForward = other.Forward;
				var otherBackward = -otherForward; 

				var angle1 = Math.Atan2(otherForward.Y - shipForward.Y, otherForward.X - shipForward.X);				
				var angle2 = Math.Atan2(otherBackward.Y - shipForward.Y, otherBackward.X - shipForward.X);

				double finalAngle = angle1; 
				if (Math.Abs(angle1) > Math.Abs(angle2))
				{
					finalAngle = angle2; 
				}

				var turn = -finalAngle * ship1.CarData.MaxTurnRate * TimeManager.SecondDifference;
				ship1.RotationZ += (float)turn;

				//Modify movement velocity. 
				//var awayVector = (ship1.Position - other.Position).ToVector2();
				//awayVector.Normalize();
				//awayVector *= ship1.Forward.Length() * 4;
				//ship1.Velocity += awayVector.ToVector3(); 
				//var newVelocity = ship1.Velocity;

				//var initialXSign = Math.Sign(newVelocity.X);
				//var initialYSign = Math.Sign(newVelocity.Y);

				//newVelocity += awayVector.ToVector3();
				//if (Math.Sign(newVelocity.X) != initialXSign) newVelocity.X = 0;
				//if (Math.Sign(newVelocity.Y) != initialYSign) newVelocity.Y = 0;
				//ship1.Velocity = newVelocity; 
				return true; 
			}
			return false; 
		}		
	}
}
