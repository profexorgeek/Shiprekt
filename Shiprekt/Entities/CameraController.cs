﻿using System;
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
    public partial class CameraController
    {
        public enum FollowTargetType
        {
            Entity,
            Position
        }

        public FollowTargetType CurrentFollowTargetType
        {
            get; set;
        }

        public Vector2 TargetPosition { get; set; }

        public PositionedObject TargetEntity { get; set; }

        public bool FollowImmediately { get; set; }

        public Camera Camera { get; set; }

        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
        private void CustomInitialize()
        {


        }

        private void CustomActivity()
        {
            if(FollowImmediately)
            {
                switch(CurrentFollowTargetType)
                {
                    case FollowTargetType.Entity:
                        X = TargetEntity.X;
                        Y = TargetEntity.Y;
                        break;
                    case FollowTargetType.Position:
                        X = TargetPosition.X;
                        Y = TargetPosition.Y;
                        break;
                }
            }

            Camera.X = X;
            Camera.Y = Y;

        }

        private void CustomDestroy()
        {


        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
    }
}