﻿using System;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Specialized;
using FlatRedBall.Audio;
using FlatRedBall.Screens;
using Shiprekt.Entities;
using Shiprekt.Entities.Effects;
using Shiprekt.Screens;
using Shiprekt.Factories;

namespace Shiprekt.Entities
{
    public partial class Ship
    {
        void OnAfterDying (Entities.Ship value) 
        {
            ShipDeathEffectFactory.CreateNew().TriggerEffect(X, Y, RotationZ);

            shipdeath01.Play();
        }

    }
}
