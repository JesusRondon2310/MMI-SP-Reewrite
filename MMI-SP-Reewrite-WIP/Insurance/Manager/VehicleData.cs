namespace MMI_SP.Insurance
{
    internal class VehicleData
    {
        // ==========================================
        // BLOQUE: Datos
        // ==========================================
        internal string Id { get; }
        internal string Plate { get; }
        internal int PrimaryColor { get; }
        internal int SecondaryColor { get; }
        internal bool IsDestroyed { get; }
        internal int WindowTint { get; }
        internal int WheelType { get; }
        internal int WheelColor { get; }
        internal int TireSmokeR { get; }
        internal int TireSmokeG { get; }
        internal int TireSmokeB { get; }
        internal float PosX { get; }
        internal float PosY { get; }
        internal float PosZ { get; }
        internal float Heading { get; }

        internal VehicleData(
            string id, string plate,
            int primaryColor, int secondaryColor, bool isDestroyed,
            int windowTint, int wheelType, int wheelColor,
            int tireSmokeR, int tireSmokeG, int tireSmokeB,
            float posX, float posY, float posZ, float heading)
        {
            Id = id;
            Plate = plate;
            PrimaryColor = primaryColor;
            SecondaryColor = secondaryColor;
            IsDestroyed = isDestroyed;
            WindowTint = windowTint;
            WheelType = wheelType;
            WheelColor = wheelColor;
            TireSmokeR = tireSmokeR;
            TireSmokeG = tireSmokeG;
            TireSmokeB = tireSmokeB;
            PosX = posX;
            PosY = posY;
            PosZ = posZ;
            Heading = heading;
        }

        internal VehicleData WithIsDestroyed(bool isDestroyed)
        {
            return new VehicleData(
                Id, Plate, PrimaryColor, SecondaryColor, isDestroyed,
                WindowTint, WheelType, WheelColor,
                TireSmokeR, TireSmokeG, TireSmokeB,
                PosX, PosY, PosZ, Heading);
        }

        internal int GetModelHash()
        {
            string[] parts = Id.Split('_');
            if (parts.Length >= 1 && int.TryParse(parts[0], out int hash)) return hash;
            return 0;
        }
    }
}
