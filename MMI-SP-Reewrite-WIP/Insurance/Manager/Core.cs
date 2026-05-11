using GTA;

namespace MMI_SP.Insurance
{
    public static class Core
    {
        private static Manager _manager;

        public static void Initialize()
        {
            if (_manager == null)
            {
                _manager = new Manager();
                _manager.LoadFrom(Repository.Load());
                Logger.Debug("Insurance inicializado.");
            }
        }

        public static bool IsInsured(Vehicle veh) => _manager?.IsInsured(veh) ?? false;
        public static bool IsInsurable(Vehicle veh) => Manager.IsInsurable(veh);
        public static int GetCost(Vehicle veh) => Calculator.GetCost(veh);
        public static void Insure(Vehicle veh) => _manager?.Insure(veh);
    }
}