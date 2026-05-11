using GTA;
using GTA.Math;
using GTA.Native;
using MMI_SP.Helpers;
using MMI_SP.Agency.MainMenu;
using System;

namespace MMI_SP.Agency.Office
{
    internal class EntryManager
    {
        internal static Vector3 OfficePlayerPos { get; } = new Vector3(120.0f, -620.50f, 206.35f);

        private readonly Script _script;
        private readonly OfficeManager _officeMgr;
        private readonly CutsceneManager _cutsceneMgr;
        private readonly UI _menu;

        public EntryManager(Script script)
        {
            _script = script;
            _menu = new UI();
            _menu.Closed(Exit);

            _officeMgr = new OfficeManager();
            _cutsceneMgr = new CutsceneManager();
        }

        /// <summary>
        /// Inicia la visita completa: menú, cinemática, teletransporte, oficina.
        /// </summary>
        public void Enter()
        {
            Logger.Debug("Reset the menu");

            try
            {
                _menu.RebuildMenu(Exit);  // Destruye el menú anterior y crea uno nuevo
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                GTA.UI.Notification.Show("MMI-SP: Error with module NativeUI!");
                ErrorCancel(false);
                return;
            }

            // Cinemática de entrada
            Logger.Debug("Entering cutscene");
            _cutsceneMgr.EnterCutscene();
            Cutscenes.EnteringAgency();

            // Teletransportar al jugador
            Logger.Debug("Teleport the player in the office");
            Game.Player.Character.Position = OfficePlayerPos;
            Game.Player.Character.IsPositionFrozen = true;

            Logger.Debug("Force load office");
            Function.Call(Hash.LOAD_SCENE, OfficePlayerPos.X, OfficePlayerPos.Y, OfficePlayerPos.Z);
            Logger.Debug("Wait until everything is loaded");

            Screen.UIHandler(1000);
            Logger.Debug("Open menu");

            try
            {
                _menu.Show();
            }
            catch (Exception e)
            {
                Logger.Error("Error: EnterAgency - " + e.Message);
                GTA.UI.Notification.Show("MMI-SP: Error with module NativeUI!");
                ErrorCancel(false);
                return;
            }

            // Crear la oficina
            Logger.Debug("Office creation");
            _officeMgr.CreateOffice();

            // Diálogos del NPC (comentados por ahora)
            // _officeMgr.Speak(...)

            Logger.Debug("_office.itemsCollection:");
            Logger.Debug("type=" + _officeMgr.CurrentCollectionType);
            Logger.Debug("count=" + _officeMgr.CurrentCollectionCount);

            _officeMgr.StartSpeechTimer();
        }

        /// <summary>
        /// Finaliza la visita.
        /// </summary>
        public void Exit()
        {
            GTA.UI.Screen.FadeOut(1000);
            Screen.UIHandler(1000);

            _officeMgr.DestroyOffice();

            Game.Player.Character.IsPositionFrozen = false;
            Game.Player.Character.Position = Reception.Position;

            Function.Call(Hash.LOAD_SCENE, Reception.Position.X, Reception.Position.Y, Reception.Position.Z);
            Screen.UIHandler(1000);

            Cutscenes.LeavingAgency();
            _cutsceneMgr.ExitCutscene();
        }

        /// <summary>
        /// Cancela la visita por error.
        /// </summary>
        public void ErrorCancel(bool menu = true)
        {
            _cutsceneMgr.CancelCutscene();
            Game.Player.Character.Position = Reception.Position;
            Game.Player.Character.IsPositionFrozen = false;
            GTA.UI.Screen.FadeIn(1000);
            World.RenderingCamera = null;

            if (menu)
            {
                _menu.Reset();
                _menu.Show();
            }
        }

        /// <summary>
        /// Tick durante la visita.
        /// </summary>
        public void OnTick()
        {
            _cutsceneMgr.UpdateHUD();
            _officeMgr.UpdateSpeechTimer();
            _menu?.Update();
        }

        /// <summary>
        /// Limpia al abortar el script.
        /// </summary>
        public void CleanUp()
        {
            _officeMgr.DestroyOffice();
        }
    }
}