using GTA;

namespace MMI_SP.Helpers
{
    internal static class VehicleIdentifier
    {
        /// <summary>
        /// Devuelve el identificador único del vehículo (hashModelo_matrícula).
        /// </summary>
        public static string Get(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return string.Empty;
            return $"{veh.Model.Hash}_{veh.Mods.LicensePlate}";
        }
    }
}