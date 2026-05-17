using System.Collections.Generic;
using GTA;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;

namespace MMI_SP.Insurance
{
    internal class Insurer
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private List<VehicleData> _insuredVehicles;

        internal void LoadFrom(List<VehicleData> list) => _insuredVehicles = list;

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        public static bool IsInsurable(Vehicle veh)
        {
            if (veh == null || !veh.IsAlive) return false;
            return !veh.Model.IsTrain &&
                   (veh.Model.IsCar || veh.Model.IsBike || veh.Model.IsQuadBike ||
                    veh.Model.IsHelicopter || veh.Model.IsPlane || veh.Model.IsBoat);
        }

        internal bool IsInsured(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return false;
            string id = VehicleIdentifier.Get(veh);
            return _insuredVehicles.Exists(v => v.Id == id);
        }

        internal Result<bool> Insure(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return new Err<bool>("El vehículo no existe.");

            return VehicleDataFactory.CreateFrom(veh).AndThen<bool>(data =>
            {
                if (_insuredVehicles.Exists(v => v.Id == data.Id))
                    return new Err<bool>("El vehículo ya está asegurado.");

                _insuredVehicles.Add(data);
                Logger.Debug($"Vehículo asegurado: {data.Id}");
                return Repository.Save(_insuredVehicles);
            });
        }

        internal List<VehicleData> GetActiveList()
            => _insuredVehicles.FindAll(v => !v.IsDestroyed);

        internal List<VehicleData> GetDestroyedList()
            => _insuredVehicles.FindAll(v => v.IsDestroyed);

        internal List<string> GetInsuredIds()
        {
            var ids = new List<string>();
            foreach (VehicleData v in _insuredVehicles) ids.Add(v.Id);
            return ids;
        }

        internal Result<bool> Cancel(string vehicleId)
        {
            VehicleData target = _insuredVehicles.Find(v => v.Id == vehicleId);
            if (target == null)
                return new Err<bool>("Vehículo no encontrado en la lista de asegurados.");

            _insuredVehicles.Remove(target);
            Logger.Debug($"Vehículo cancelado: {vehicleId}");
            return Repository.Save(_insuredVehicles);
        }

        internal Result<bool> MarkAsDestroyed(string vehicleId)
        {
            VehicleData target = _insuredVehicles.Find(v => v.Id == vehicleId);
            if (target == null) return new Err<bool>("Vehículo no encontrado.");

            _insuredVehicles.Remove(target);
            _insuredVehicles.Add(target.WithIsDestroyed(true));
            Logger.Debug($"Vehículo marcado destruido: {vehicleId}");
            return Repository.Save(_insuredVehicles);
        }

        internal Result<bool> RecoverVehicle(string vehicleId)
        {
            VehicleData target = _insuredVehicles.Find(v => v.Id == vehicleId);
            if (target == null) return new Err<bool>("Vehículo no encontrado.");
            if (!target.IsDestroyed) return new Err<bool>("El vehículo no está destruido.");

            _insuredVehicles.Remove(target);
            _insuredVehicles.Add(target.WithIsDestroyed(false));
            Logger.Debug($"Vehículo recuperado: {vehicleId}");
            return Repository.Save(_insuredVehicles);
        }

        // Preserva IsDestroyed del registro existente al actualizar posición/colores.
        internal Result<bool> UpdateVehicleData(Vehicle veh)
        {
            string id = VehicleIdentifier.Get(veh);
            VehicleData existing = _insuredVehicles.Find(v => v.Id == id);
            if (existing == null) return new Err<bool>("Vehículo no encontrado para actualizar.");

            return VehicleDataFactory.CreateFrom(veh).AndThen<bool>(updated =>
            {
                VehicleData preserved = new VehicleData(
                    updated.Id, updated.Plate,
                    updated.PrimaryColor, updated.SecondaryColor, existing.IsDestroyed,
                    updated.WindowTint, updated.WheelType, updated.WheelColor,
                    updated.TireSmokeR, updated.TireSmokeG, updated.TireSmokeB,
                    updated.PosX, updated.PosY, updated.PosZ, updated.Heading);

                _insuredVehicles.Remove(existing);
                _insuredVehicles.Add(preserved);
                return Repository.Save(_insuredVehicles);
            });
        }

        internal VehicleData FindById(string vehicleId)
            => _insuredVehicles.Find(v => v.Id == vehicleId);
    }
}
