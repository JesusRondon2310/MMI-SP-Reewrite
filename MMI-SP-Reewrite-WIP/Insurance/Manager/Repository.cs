using System.Collections.Generic;
using System.IO;

namespace MMI_SP.Insurance
{
    internal static class Repository
    {
        internal static string DatabasePath => Path.Combine(Config.BaseDir, "asegurados.txt");

        internal static List<string> Load()
        {
            if (File.Exists(DatabasePath))
            {
                var list = new List<string>(File.ReadAllLines(DatabasePath));
                Logger.Debug($"Se cargaron {list.Count} vehículos asegurados.");
                return list;
            }
            Logger.Debug("No se encontró base de datos de asegurados. Se comienza con lista vacía.");
            return new List<string>();
        }

        internal static void Save(List<string> insuredVehicles)
        {
            File.WriteAllLines(DatabasePath, insuredVehicles);
            Logger.Debug("Base de datos de asegurados guardada.");
        }
    }
}