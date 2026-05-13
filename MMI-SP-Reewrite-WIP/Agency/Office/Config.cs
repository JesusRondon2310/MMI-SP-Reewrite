using GTA.Math;

namespace MMI_SP.Agency.Office
{
    internal static class Config
    {
        internal static Vector3 CameraPos { get; } = new Vector3(116.0f, -620.50f, 206.35f);
        internal static Vector3 NpcPos { get; } = new Vector3(114.35f, -619.3748f, 204.50f);
        internal static Vector3 NpcRot { get; } = new Vector3(0.0f, 0.0f, -120.0f);
        internal static string NpcModel { get; } = "a_f_y_business_01";
        internal static Vector3 PlayerPos { get; } = new Vector3(120.0f, -620.50f, 206.35f);
    }
}