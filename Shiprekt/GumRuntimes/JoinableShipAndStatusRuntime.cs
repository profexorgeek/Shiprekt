using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Shiprekt.GumRuntimes
{
    public partial class JoinableShipAndStatusRuntime
    {
        public ContainerRuntime GetGunLeft { get => GunLeft; }


        public ContainerRuntime GetGunRight { get => GunRight; }


        public ShipFrontRuntime ShipFront
        {
            get
            {
                return ShipFrontInstance;
            }
        }

        partial void CustomInitialize () 
        {
        }
    }
}
