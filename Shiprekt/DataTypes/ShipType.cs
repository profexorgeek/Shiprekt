using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiprekt.DataTypes
{
    public enum ShipType
    {
        Gray,
        Green,
        Purple,
        RedStripes
    }

    public static class ShipTypeExtensions
    {
        public static IEnumerable<ShipType> AllShipTypes
        {
            get
            {
                yield return ShipType.Gray;
                yield return ShipType.Green;
                yield return ShipType.Purple;
                yield return ShipType.RedStripes;
            }
        }
        public static Entities.ShipSail.SailColor ToSailColor(this ShipType shipType)
        {
            switch(shipType)
            {
                case ShipType.Gray: return Entities.ShipSail.SailColor.Gray;
                case ShipType.Green: return Entities.ShipSail.SailColor.Green;
                case ShipType.Purple: return Entities.ShipSail.SailColor.Purple;
                case ShipType.RedStripes: return Entities.ShipSail.SailColor.RedStripe;
            }
            return null;
        }

        public static GumRuntimes.ShipFrontRuntime.SailDesign ToGum(this ShipType shipType)
        {
            switch (shipType)
            {
                case ShipType.Gray: return GumRuntimes.ShipFrontRuntime.SailDesign.Gray;
                case ShipType.Green: return GumRuntimes.ShipFrontRuntime.SailDesign.Green;
                case ShipType.Purple: return GumRuntimes.ShipFrontRuntime.SailDesign.Purple;
                case ShipType.RedStripes: return GumRuntimes.ShipFrontRuntime.SailDesign.RedStripes;
            }
            return GumRuntimes.ShipFrontRuntime.SailDesign.Gray;
        }

        public static ShipType ToShipType(this GumRuntimes.ShipFrontRuntime.SailDesign gumSailDesign)
        {
            switch (gumSailDesign)
            {
                case GumRuntimes.ShipFrontRuntime.SailDesign.Gray: return ShipType.Gray;
                case GumRuntimes.ShipFrontRuntime.SailDesign.Green: return ShipType.Green;
                case GumRuntimes.ShipFrontRuntime.SailDesign.Purple: return ShipType.Purple;
                case GumRuntimes.ShipFrontRuntime.SailDesign.RedStripes: return ShipType.RedStripes;
            }
            return ShipType.Gray;
        }
    }
}
