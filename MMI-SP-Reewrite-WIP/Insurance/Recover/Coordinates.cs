using GTA;
using GTA.Math;

namespace MMI_SP.Insurance.Recover
{
    internal static class Coordinates
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        // Depósito LSIA — vehículos de tierra
        private static readonly Vector3[] CarSpawns =
        {
            new Vector3(-1092.4f, -3139.5f, 13.94f),
            new Vector3(-1087.0f, -3139.5f, 13.94f),
            new Vector3(-1081.6f, -3139.5f, 13.94f),
            new Vector3(-1076.2f, -3139.5f, 13.94f),
            new Vector3(-1070.8f, -3139.5f, 13.94f),
        };
        private static readonly float[] CarHeadings = { 90f, 90f, 90f, 90f, 90f };

        // Motos — mismo depósito, zona lateral
        private static readonly Vector3[] BikeSpawns =
        {
            new Vector3(-1065.0f, -3141.0f, 13.94f),
            new Vector3(-1062.0f, -3141.0f, 13.94f),
            new Vector3(-1059.0f, -3141.0f, 13.94f),
        };
        private static readonly float[] BikeHeadings = { 90f, 90f, 90f };

        // Helicópteros — plataforma LSIA
        private static readonly Vector3[] HelicopterSpawns =
        {
            new Vector3(-1040.0f, -2996.0f, 14.0f),
            new Vector3(-1055.0f, -2996.0f, 14.0f),
        };
        private static readonly float[] HelicopterHeadings = { 180f, 180f };

        // Aviones — zona de rodaje LSIA
        private static readonly Vector3[] PlaneSpawns =
        {
            new Vector3(-1336.0f, -3044.0f, 13.94f),
            new Vector3(-1336.0f, -3058.0f, 13.94f),
        };
        private static readonly float[] PlaneHeadings = { 355f, 355f };

        // Barcos — Puerto del Sol
        private static readonly Vector3[] BoatSpawns =
        {
            new Vector3(-729.0f, -1479.0f, 0.5f),
            new Vector3(-718.0f, -1479.0f, 0.5f),
        };
        private static readonly float[] BoatHeadings = { 270f, 270f };

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        internal static void GetRecoverNode(Vehicle tempVeh, out Vector3 pos, out float heading)
        {
            if (tempVeh.Model.IsHelicopter)
            {
                pos = HelicopterSpawns[0];
                heading = HelicopterHeadings[0];
                return;
            }
            if (tempVeh.Model.IsPlane)
            {
                pos = PlaneSpawns[0];
                heading = PlaneHeadings[0];
                return;
            }
            if (tempVeh.Model.IsBoat)
            {
                pos = BoatSpawns[0];
                heading = BoatHeadings[0];
                return;
            }
            if (tempVeh.Model.IsBike || tempVeh.Model.IsQuadBike)
            {
                pos = BikeSpawns[0];
                heading = BikeHeadings[0];
                return;
            }

            // Coches y resto
            pos = CarSpawns[0];
            heading = CarHeadings[0];
        }
    }
}
