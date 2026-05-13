using GTA;

namespace MMI_SP.Insurance
{
    public static class Calculator
    {
        public static int GetCost(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return 0;

            int baseCost = 500;
            if (veh.Model.IsHelicopter || veh.Model.IsPlane)
                baseCost += 5000;
            else if (veh.Model.IsBoat)
                baseCost += 2000;
            else if (veh.Model.IsBike || veh.Model.IsQuadBike)
                baseCost += 300;
            else if (veh.ClassType == VehicleClass.Military)
                baseCost += 10000;
            else
                baseCost += 1000;

            return (int)(baseCost * Config.InsuranceMult);
        }
    }
}