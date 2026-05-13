using GTA;
using GTA.Native;
using MMI_SP.Helpers;

namespace MMI_SP.Agency.Office.Entry
{
    internal static class ExitSequence
    {
        public static void Execute(Office.Manager office, CutsceneManager cutscene)
        {
            GTA.UI.Screen.FadeOut(1000);
            Screen.UIHandler(1000);

            office.DestroyOffice();

            Game.Player.Character.IsPositionFrozen = false;
            Game.Player.Character.Position = Agency.Reception.Position;

            Function.Call(Hash.LOAD_SCENE, Agency.Reception.Position.X,
                          Agency.Reception.Position.Y, Agency.Reception.Position.Z);
            Screen.UIHandler(1000);

            Cutscenes.LeavingAgency();
            cutscene.Exit();
        }
    }
}