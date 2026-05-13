using GTA;
using GTA.Math;
using GTA.Native;

namespace MMI_SP.Agency.Office.Ambient
{
    internal static class NpcHandler
    {
        public static Ped Create()
        {
            Ped npc = World.CreatePed(Config.NpcModel, Config.NpcPos);
            if (npc != null && npc.Exists())
            {
                npc.IsPersistent = true;
                SetComponentVariations(npc);
            }
            return npc;
        }

        public static void SetAI(Ped npc)
        {
            if (npc == null || !npc.Exists()) return;

            npc.Task.PlayAnimation(
                "amb@prop_human_seat_chair@female@arms_folded@base",
                "base", 1.0f, -1, AnimationFlags.Loop);
            npc.IsPositionFrozen = true;
            npc.Position = Config.NpcPos;
            npc.Rotation = Config.NpcRot;
            npc.Task.LookAt(Config.CameraPos);
        }

        private static void SetComponentVariations(Ped npc)
        {
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, npc, 0, 0, 0, 0); // Face
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, npc, 2, 1, 0, 0); // Hair
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, npc, 3, 1, 0, 0); // Torso
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, npc, 4, 0, 1, 0); // Legs
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, npc, 6, 0, 0, 0); // Feet
            Function.Call(Hash.SET_PED_PROP_INDEX, npc, 1, 0, 0, 0);       // Glasses
        }
    }
}