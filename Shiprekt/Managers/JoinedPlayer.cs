using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlatRedBall.Input;

namespace Shiprekt.Managers
{
    public class JoinedPlayer
    {
        public DataTypes.ShipType ShipType { get; set; }
        public IInputDevice InputDevice { get; set; }
        public int LastGameDeaths { get; set;}
        public int LastGameKills { get; set; }
    }
}
