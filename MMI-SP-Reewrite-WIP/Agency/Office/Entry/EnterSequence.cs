using System;
using GTA;
using GTA.Native;
using MMI_SP.Helpers;
using MMI_SP.Agency.MainMenu;

namespace MMI_SP.Agency.Office.Entry
{
    internal static class EnterSequence
    {
        public static void Execute(UI menu, CutsceneManager cutscene, Office.Manager office)
        {
            Logger.Debug("Reset the menu");
            try
            {
                menu.RebuildMenu();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                GTA.UI.Notification.Show("MMI-SP: Error with module NativeUI!");
                CancelSequence.Execute(menu, cutscene, false);
                return;
            }

            Logger.Debug("Entering cutscene");
            cutscene.Enter();
            Cutscenes.EnteringAgency();

            Logger.Debug("Teleport the player in the office");
            Game.Player.Character.Position = Config.PlayerPos;
            Game.Player.Character.IsPositionFrozen = true;

            Logger.Debug("Force load office");
            Function.Call(Hash.LOAD_SCENE, Config.PlayerPos.X, Config.PlayerPos.Y, Config.PlayerPos.Z);
            Logger.Debug("Wait until everything is loaded");

            Screen.UIHandler(1000);
            Logger.Debug("Open menu");

            try
            {
                menu.Show();
            }
            catch (Exception e)
            {
                Logger.Error("Error: EnterAgency - " + e.Message);
                GTA.UI.Notification.Show("MMI-SP: Error with module NativeUI!");
                //CancelSequence.Execute(menu, cutscene, false);
                return;
            }

            Logger.Debug("Office creation");
            office.CreateOffice();

            Logger.Debug("_office.itemsCollection:");
            Logger.Debug("type=" + office.CurrentCollectionType);
            Logger.Debug("count=" + office.CurrentCollectionCount);

            office.StartSpeechTimer();
        }
    }
}