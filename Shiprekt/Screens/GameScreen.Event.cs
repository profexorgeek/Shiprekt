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

namespace Shiprekt.Screens
{
    public partial class GameScreen
    {
        void OnBulletListVsShipListCollisionOccurred (Entities.Bullet bullet, Entities.Ship ship) 
        {
            if(bullet.TeamIndex != ship.TeamIndex)
            {
                bullet.CollideAgainstBounce(ship, .1f, 1, 1);

                bullet.Destroy();

                ship.TakeDamage(Bullet.DamageToDeal);
            }
        }

    }
}
