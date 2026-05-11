using System.Collections.Generic;
using GTA;
using MMI_SP.Helpers;

namespace MMI_SP.Insurance
{
    internal class Manager
    {
        private List<string> _insuredVehicles;

        internal void LoadFrom(List<string> list) => _insuredVehicles = list;

        // 1. Validación estática: ¿Es asegurable?
        public static bool IsInsurable(Vehicle veh)
        {
            if (veh == null || !veh.IsAlive) return false;
            return !veh.Model.IsTrain &&
                   (veh.Model.IsCar || veh.Model.IsBike || veh.Model.IsQuadBike ||
                    veh.Model.IsHelicopter || veh.Model.IsPlane || veh.Model.IsBoat);
        }

        // 2. Consulta de instancia: ¿Ya está en la lista?
        internal bool IsInsured(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return false;
            return _insuredVehicles.Contains(VehicleIdentifier.Get(veh));
        }

        // 3. Acción de instancia: Añadir a la lista y guardar
        internal void Insure(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return;
            string id = VehicleIdentifier.Get(veh);
            if (!_insuredVehicles.Contains(id))
            {
                _insuredVehicles.Add(id);
                Repository.Save(_insuredVehicles);
                Logger.Debug($"Vehículo asegurado: {id}");
            }
        }
    }
}