using System;
using GTA;
using MMI_SP.Agency.MainMenu;

namespace MMI_SP.Agency.Office.Entry
{
    internal static class CancelSequence
    {
        public static void Execute(UI menu, CutsceneManager cutscene, bool showMenu)
        {
            cutscene.Exit();
            Game.Player.Character.Position = Agency.Reception.Position;
            Game.Player.Character.IsPositionFrozen = false;
            GTA.UI.Screen.FadeIn(1000);
            World.RenderingCamera = null;

            if (showMenu)
            {
                menu.RebuildMenu();
                menu.Show();
            }
        }
    }
}