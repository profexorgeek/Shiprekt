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
using System.Collections.Generic;
using FlatRedBall.Debugging;

namespace Shiprekt.Screens
{
    public partial class GameScreen
    {
		Circle shipCollisionTestCircle;
        Dictionary<Ram, Ship> ShipsJustRammed = new Dictionary<Ram, Ship>(); 

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
            if (first.TeamIndex == second.TeamIndex) return; 

            if (first.CanRamShip(second))
            {
                DoTheRamming(first, second);  
            }
            else if (second.CanRamShip(first))
            {
                DoTheRamming(second, first); 
            }
            else
            {
                TrySteerShipAwayFromCollision(first, second);
                TrySteerShipAwayFromCollision(second, first);
                first.CollideAgainstBounce(second, 1, 1, .3f);
            }            
        }

        private bool TrySteerShipAwayFromCollision(Ship ship1, Ship other)
		{
			//Put the collision in front of the ship. 
			if (shipCollisionTestCircle == null) shipCollisionTestCircle = new Circle();
			shipCollisionTestCircle.Radius = 8;
			shipCollisionTestCircle.Position = ship1.Position;
			shipCollisionTestCircle.Position += ship1.Forward * 25;
			
			//If that ship is in front of this ship, apply steering. 
			if (shipCollisionTestCircle.CollideAgainst(other.Collision))
			{
				//RotateShip
				var shipForward = ship1.Forward;
				var otherForward = other.Forward;
				var otherBackward = -otherForward;

				var angle1 = Math.Atan2(otherForward.Y, otherForward.X) - Math.Atan2(shipForward.Y, shipForward.Y); 				
				var angle2 = Math.Atan2(otherBackward.Y, otherBackward.X) - Math.Atan2(shipForward.Y, shipForward.Y);

				double finalAngle = angle1; 
				if (Math.Abs(angle1) > Math.Abs(angle2))
				{
					finalAngle = angle2; 
				}

				//We only want to modify the turn if the angles of the two ships are at odds, making a player controlled turn hard. 
				//Otherwise, we want to enable the player to steer and not feel like they are out of control. 
				if (Math.Abs(finalAngle) > 0.78f)
				{
					var turn = -finalAngle * ship1.CarData.MaxTurnRate * TimeManager.SecondDifference;
					ship1.RotationZ += (float)turn;
				}
				
				return true; 
			}
			return false; 
		}		

        private void DoTheRamming(Ship rammer, Ship target)
        {
            var dmg = rammer.GetRamShipDmg(target);
            target.TakeDamage(dmg);

            
            rammer.MarkShipRammed(target);

            rammer.CollideAgainstBounce(target, 1, 0f, .3f);
        }
	}
}
