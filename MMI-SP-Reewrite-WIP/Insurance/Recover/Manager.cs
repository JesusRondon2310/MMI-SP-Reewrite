using System.Drawing;
using GTA;
using GTA.Math;
using GTA.Native;
using MMI_SP.PatternMatching;

namespace MMI_SP.Insurance.Recover
{
    internal static class Manager
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static Result<bool> RecoverVehicle(string vehicleId, Insurer insurer)
        {
            VehicleData data = insurer.FindById(vehicleId);
            if (data == null) return new Err<bool>("Vehículo no encontrado en la póliza.");
            if (!data.IsDestroyed) return new Err<bool>("El vehículo no ha sido destruido.");

            int modelHash = data.GetModelHash();
            if (modelHash == 0) return new Err<bool>("Hash de modelo inválido.");

            return insurer.RecoverVehicle(vehicleId)
                .AndThen<bool>(_ => SpawnVehicle(data, modelHash));
        }

        private static Result<bool> SpawnVehicle(VehicleData data, int modelHash)
        {
            Model model = new Model(modelHash);
            model.Request(3000);
            if (!model.IsLoaded)
            {
                model.MarkAsNoLongerNeeded();
                return new Err<bool>("No se pudo cargar el modelo del vehículo.");
            }

            // Necesitamos un vehículo temporal para determinar coordenadas por tipo
            Vehicle probe = World.CreateVehicle(model, new Vector3(0f, 0f, -100f));
            if (probe == null || !probe.Exists())
            {
                model.MarkAsNoLongerNeeded();
                return new Err<bool>("No se pudo crear el vehículo sonda.");
            }

            Coordinates.GetRecoverNode(probe, out Vector3 spawnPos, out float heading);
            probe.Delete();

            Vehicle veh = World.CreateVehicle(model, spawnPos, heading);
            model.MarkAsNoLongerNeeded();

            if (veh == null || !veh.Exists())
                return new Err<bool>("No se pudo crear el vehículo en el depósito.");

            ApplyData(veh, data);
            Function.Call(Hash.SET_ENTITY_AS_MISSION_ENTITY, veh.Handle, true, true);

            Blip blip = veh.AddBlip();
            blip.Sprite = BlipSprite.PersonalVehicleCar;
            blip.Color = BlipColor.Yellow;
            blip.Name = "Vehículo recuperado";

            Logger.Debug($"Vehículo recuperado y depositado: {data.Id}");
            return new Ok<bool>(true);
        }

        private static void ApplyData(Vehicle veh, VehicleData data)
        {
            veh.Mods.LicensePlate = data.Plate;
#pragma warning disable CS0618
            veh.Mods.PrimaryColor = (VehicleColor)data.PrimaryColor;
            veh.Mods.SecondaryColor = (VehicleColor)data.SecondaryColor;
            veh.Mods.WindowTint = (VehicleWindowTint)data.WindowTint;
            veh.Mods.WheelType = (VehicleWheelType)data.WheelType;
#pragma warning restore CS0618
            veh.Mods.TireSmokeColor = Color.FromArgb(data.TireSmokeR, data.TireSmokeG, data.TireSmokeB);
        }
    }
}
