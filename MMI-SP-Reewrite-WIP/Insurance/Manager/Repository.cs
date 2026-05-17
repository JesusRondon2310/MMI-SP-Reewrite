using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using MMI_SP.PatternMatching;

namespace MMI_SP.Insurance
{
    internal static class Repository
    {
        internal static string DatabasePath => Path.Combine(MMI_SP.Config.BaseDir, "asegurados.dat");

        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static Result<List<VehicleData>> Load()
        {
            try
            {
                var list = new List<VehicleData>();
                if (!File.Exists(DatabasePath))
                {
                    Logger.Debug("Base de datos no encontrada. Lista vacía.");
                    return new Ok<List<VehicleData>>(list);
                }

                foreach (string line in File.ReadAllLines(DatabasePath))
                {
                    VehicleData data = Deserialize(line);
                    if (data != null) list.Add(data);
                }

                Logger.Debug($"Cargados {list.Count} vehículos asegurados.");
                return new Ok<List<VehicleData>>(list);
            }
            catch (Exception ex)
            {
                return new Err<List<VehicleData>>(ex.Message);
            }
        }

        internal static Result<bool> Save(List<VehicleData> vehicles)
        {
            try
            {
                var lines = new List<string>();
                foreach (VehicleData v in vehicles)
                    lines.Add(Serialize(v));

                File.WriteAllLines(DatabasePath, lines);
                Logger.Debug("Base de datos guardada.");
                return new Ok<bool>(true);
            }
            catch (Exception ex)
            {
                return new Err<bool>(ex.Message);
            }
        }

        private static string Serialize(VehicleData v)
        {
            return string.Join("|", new[]
            {
                v.Id, v.Plate,
                v.PrimaryColor.ToString(), v.SecondaryColor.ToString(),
                v.IsDestroyed ? "1" : "0",
                v.WindowTint.ToString(), v.WheelType.ToString(), v.WheelColor.ToString(),
                v.TireSmokeR.ToString(), v.TireSmokeG.ToString(), v.TireSmokeB.ToString(),
                v.PosX.ToString(CultureInfo.InvariantCulture),
                v.PosY.ToString(CultureInfo.InvariantCulture),
                v.PosZ.ToString(CultureInfo.InvariantCulture),
                v.Heading.ToString(CultureInfo.InvariantCulture)
            });
        }

        private static VehicleData Deserialize(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return null;
            string[] p = line.Split('|');
            if (p.Length < 15) return null;

            try
            {
                return new VehicleData(
                    p[0], p[1],
                    int.Parse(p[2]), int.Parse(p[3]),
                    p[4] == "1",
                    int.Parse(p[5]), int.Parse(p[6]), int.Parse(p[7]),
                    int.Parse(p[8]), int.Parse(p[9]), int.Parse(p[10]),
                    float.Parse(p[11], CultureInfo.InvariantCulture),
                    float.Parse(p[12], CultureInfo.InvariantCulture),
                    float.Parse(p[13], CultureInfo.InvariantCulture),
                    float.Parse(p[14], CultureInfo.InvariantCulture));
            }
            catch
            {
                return null;
            }
        }
    }
}
