using System;
using System.IO;

namespace MMI_SP
{
    internal static class Config
    {
        internal static readonly string BaseDir = AppDomain.CurrentDomain.BaseDirectory + "\\MMI";
        internal static readonly string BannerImage = BaseDir + "\\banner.png";

        // Multiplicador de precio del seguro. 1.0 = precio normal.
        public static float InsuranceMult { get; } = 1.0f;

        public static void Initialize()
        {
            if (!Directory.Exists(BaseDir))
            {
                Logger.Debug("Creando carpeta MMI...");
                Directory.CreateDirectory(BaseDir);
            }
        }
    }
}