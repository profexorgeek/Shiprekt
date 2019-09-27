using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Shiprekt.GumRuntimes
{
    public partial class JoinableShipAndStatusRuntime
    {
        public Vector2 GunLeftAbsolutePosition
        {
            get
            {
                return new Vector2(GunLeft.AbsoluteX, GunLeft.AbsoluteY); 
            }
        }

        public Vector2 GunRightAbsolutePosition
        {
            get
            {
                return new Vector2(GunRight.AbsoluteX, GunRight.AbsoluteY);
            }
        }
        partial void CustomInitialize () 
        {
        }
    }
}
