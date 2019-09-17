using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlatRedBall.Input;
using Shiprekt.DataTypes;

namespace Shiprekt.Managers
{
    public static class JoinedPlayerManager
    {
        #region Fields/Properties

        static List<JoinedPlayer> joinedPlayers = new List<JoinedPlayer>();

        public static ICollection<JoinedPlayer> JoinedPlayers => joinedPlayers;

        #endregion

        public static bool IsJoined(IInputDevice inputDevice)
        {
            return joinedPlayers.Any(item => item.InputDevice == inputDevice);
        }

        public static JoinedPlayer Join(IInputDevice device)
        {
            if(joinedPlayers.Count == 4)
            {
                // cannot join
                return null;
            }
            else
            {
                var alreadyJoinedTypes = joinedPlayers.Select(item => item.ShipType);

                var availableShipTypes = ShipTypeExtensions.AllShipTypes
                    .Except(alreadyJoinedTypes);

                var shipType = availableShipTypes.First();

                var player = new JoinedPlayer();
                player.InputDevice = device;
                player.ShipType = shipType;

                joinedPlayers.Add(player);

                return player;
            }
        }

        internal static void ResetGameStats()
        {
            foreach(var player in JoinedPlayers)
            {
                player.LastGameKills = 0;
                player.LastGameDeaths = 0;
            }
        }

        public static JoinedPlayer DropPlayer(IInputDevice device)
        {
            var player = joinedPlayers.FirstOrDefault(item => item.InputDevice == device);

            if(player != null)
            {
                joinedPlayers.Remove(player);
            }

            return player;
        }

        public static void AwardKill(IInputDevice killer)
        {
            joinedPlayers.First(item => item.InputDevice == killer).LastGameKills++;
        }

        public static void RecordDeath(IInputDevice playerWhoDied)
        {
            joinedPlayers.First(item => item.InputDevice == playerWhoDied).LastGameDeaths++;
        }
    }
}
