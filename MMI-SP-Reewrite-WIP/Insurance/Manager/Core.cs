using GTA;
using System.Collections.Generic;
using MMI_SP.PatternMatching;

namespace MMI_SP.Insurance
{
    public static class Core
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private static Insurer _manager;

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        public static Result<bool> Initialize()
        {
            if (_manager != null) return new Ok<bool>(true);

            return Repository.Load().Match<Result<bool>>(
                onOk: (List<VehicleData> data) =>
                {
                    _manager = new Insurer();
                    _manager.LoadFrom(data);
                    Logger.Debug("Insurance inicializado.");
                    return new Ok<bool>(true);
                },
                onErr: (string error) => new Err<bool>(error)
            );
        }

        public static bool IsInsured(Vehicle veh) => _manager?.IsInsured(veh) ?? false;
        public static bool IsInsurable(Vehicle veh) => Insurer.IsInsurable(veh);
        public static int GetCost(Vehicle veh) => Calculator.GetCost(veh);
        public static List<VehicleData> GetActiveList() => _manager?.GetActiveList() ?? new List<VehicleData>();
        public static List<VehicleData> GetDestroyedList() => _manager?.GetDestroyedList() ?? new List<VehicleData>();
        public static VehicleData FindById(string vehicleId) => _manager?.FindById(vehicleId);

        public static Result<bool> Insure(in Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return new Err<bool>("No hay vehículo que asegurar.");
            if (IsInsured(veh)) return new Err<bool>("El vehículo ya está asegurado.");
            if (!IsInsurable(veh)) return new Err<bool>("Este vehículo no se puede asegurar.");

            int cost = GetCost(veh);
            if (Game.Player.Money < cost) return new Err<bool>("No tienes suficiente dinero.");

            Game.Player.Money -= cost;
            return _manager != null ? _manager.Insure(veh) : new Err<bool>("El gestor de seguros no está inicializado.");
        }

        public static Result<bool> Cancel(string vehicleId)
        {
            if (string.IsNullOrEmpty(vehicleId)) return new Err<bool>("ID de vehículo no válido.");
            if (_manager == null) return new Err<bool>("El gestor de seguros no está inicializado.");
            return _manager.Cancel(vehicleId);
        }

        public static Result<bool> RecoverVehicle(string vehicleId)
        {
            if (string.IsNullOrEmpty(vehicleId)) return new Err<bool>("ID de vehículo no válido.");
            if (_manager == null) return new Err<bool>("El gestor de seguros no está inicializado.");
            return MMI_SP.Insurance.Recover.Manager.RecoverVehicle(vehicleId, _manager);
        }

        public static Result<bool> MarkAsDestroyed(string vehicleId)
        {
            if (string.IsNullOrEmpty(vehicleId)) return new Err<bool>("ID de vehículo no válido.");
            if (_manager == null) return new Err<bool>("El gestor de seguros no está inicializado.");
            return _manager.MarkAsDestroyed(vehicleId);
        }

        public static Result<bool> UpdateVehicleData(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return new Err<bool>("El vehículo no existe.");
            if (_manager == null) return new Err<bool>("El gestor de seguros no está inicializado.");
            return _manager.UpdateVehicleData(veh);
        }
    }
}
