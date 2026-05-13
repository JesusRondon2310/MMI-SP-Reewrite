using GTA;
using GTA.Math;
using GTA.Native;
using MMI_SP.Helpers;
using static MMI_SP.Agency.ItemsManager;

namespace MMI_SP.Agency.Office.Ambient
{
    internal static class Builder
    {
        private static Weather _storedWeather;
        private static Camera _camera;
        private static Ped _npc;

        internal static Weather CurrentWeather => _storedWeather;
        internal static Camera CurrentCamera => _camera;
        internal static Ped CurrentNpc => _npc;

        public static void Build(OfficeItemsCollection items)
        {
            // 1. Clima
            _storedWeather = World.Weather;
            World.Weather = Weather.Clouds;

            // 2. Cámara
            _camera = World.CreateCamera(Config.CameraPos, Vector3.Zero, GameplayCamera.FieldOfView);

            // 3. NPC
            _npc = NpcHandler.Create();
            if (items.Type == CollectionType.Night)
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, _npc, 2, 0, 2, 0);
            NpcHandler.SetAI(_npc);

            // 4. Props
            foreach (OfficeItem item in items)
            {
                Prop prop = item.Init();
                if (prop != null && prop.Exists() && _npc != null && _npc.Exists())
                    _npc.SetNoCollision(prop, true);
            }

            // 5. Cámara apunta al NPC
            if (_npc != null && _npc.Exists())
                _camera.PointAt(_npc.Bones[Bone.IKHead]);
            else
                _camera.PointAt(Config.NpcPos);

            World.RenderingCamera = _camera;
            GTA.UI.Screen.FadeIn(1000);
            Screen.UIHandler(1000);
        }
    }
}