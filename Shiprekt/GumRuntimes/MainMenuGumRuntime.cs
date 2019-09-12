using System;
using System.Collections.Generic;
using System.Linq;

namespace Shiprekt.GumRuntimes
{
    public partial class MainMenuGumRuntime
    {
        partial void CustomInitialize () 
        {
        }

        public void UnjoinAll()
        {
            foreach(var joinedPlayer in JoinedPlayerContainer.Children)
            {
                joinedPlayer.CurrentJoinedCategoryState =
                    JoinableShipAndStatusRuntime.JoinedCategory.NotJoined;
            }
        }

        public void JoinWith(GumRuntimes.ShipFrontRuntime.SailDesign sailDesign)
        {
            var whichToJoinWith = JoinedPlayerContainer.Children
                .First(item => item.CurrentJoinedCategoryState == JoinableShipAndStatusRuntime.JoinedCategory.NotJoined);

            if(whichToJoinWith != null)
            {
                whichToJoinWith.CurrentJoinedCategoryState = JoinableShipAndStatusRuntime.JoinedCategory.Joined;
                whichToJoinWith.SailDesignState = sailDesign;
                whichToJoinWith.CurrentWinOrNormalState = JoinableShipAndStatusRuntime.WinOrNormal.NoStats;
            }
        }

        internal void UnjoinWith(ShipFrontRuntime.SailDesign sailDesign)
        {
            var whichToUnjoin = JoinedPlayerContainer.Children
                .FirstOrDefault(item => item.SailDesignState == sailDesign);

            if(whichToUnjoin != null)
            {
                whichToUnjoin.CurrentJoinedCategoryState = JoinableShipAndStatusRuntime.JoinedCategory.NotJoined;
            }
        }
    }
}
