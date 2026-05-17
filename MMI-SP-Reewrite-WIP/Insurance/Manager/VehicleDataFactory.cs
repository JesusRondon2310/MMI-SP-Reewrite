using System.Drawing;
using GTA;
using GTA.Native;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;

namespace MMI_SP.Insurance
{
    internal static class VehicleDataFactory
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static Result<VehicleData> CreateFrom(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return new Err<VehicleData>("El vehículo no existe.");

            string id = VehicleIdentifier.Get(veh);
            if (string.IsNullOrEmpty(id)) return new Err<VehicleData>("No se pudo obtener el ID del vehículo.");

#pragma warning disable CS0618
            Color smoke = veh.Mods.TireSmokeColor;
            return new Ok<VehicleData>(new VehicleData(
                id,
                veh.Mods.LicensePlate,
                (int)veh.Mods.PrimaryColor,
                (int)veh.Mods.SecondaryColor,
                false,
                (int)veh.Mods.WindowTint,
                (int)veh.Mods.WheelType,
                0,
                smoke.R, smoke.G, smoke.B,
                veh.Position.X, veh.Position.Y, veh.Position.Z,
                veh.Heading));
#pragma warning restore CS0618
        }
    }
}
