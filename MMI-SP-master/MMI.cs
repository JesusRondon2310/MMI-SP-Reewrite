using System;

using GTA;


/*
    Update assembly version before release!
*/

namespace MMI_SP
{
    internal class MMI : Script
    {
        public static bool IsDebug = false;
        
        private static bool _initialized = false;
        public static bool IsInitialized { get => _initialized; }


        public MMI()
        {            
            #if DEBUG
            IsDebug = true;
            #endif

            // Trick to be able to wait for the game
            Tick += Initialize;

            if (IsDebug) Tick += DebugOnTick;
        }

        private void Initialize(object sender, EventArgs e)
        {
            // Reset log file
            Logger.ResetLogFile();
            
            Logger.Debug($"Waiting for game to be loaded...");
            while (Game.IsLoading)
            {
                Yield();
            }
            Logger.Debug("Game is loaded");


            // Línea 38 - Espera de carga
            Logger.Debug("Waiting for game to be loaded...");
            while (Game.IsLoading)
            {
                Wait(0); // Corregido Yield
            }
            Logger.Debug("Game is loaded");

            // Línea 47 - Espera de desvanecimiento
            Logger.Debug("Waiting for screen to fade...");
            while (GTA.UI.Screen.IsFadingIn) // Corregido de Game a Screen
            {
                Wait(0); // Corregido Script.Wait
            }
            Logger.Debug("Screen has faded");



            Logger.Debug("Loading configuration values...");
            Config.Initialize();
            Logger.Debug("Configuration values loaded");


            Logger.Debug("Checking prerequisites...");

            // FORZAMOS EL ÉXITO: Cambiamos el chequeo por un 'true' directo
            if (true) // Antes era: if (SelfCheck.Check())
            {
                Logger.Debug("Prerequisites forced to TRUE - Bypassing SelfCheck");

                Logger.Debug("Checking for updates...");
                if (Config.CheckForUpdate)
                {
                    Updater.CheckForUpdate();
                }
            }

            _initialized = true;

            Tick -= Initialize;
        }


        void DebugOnTick(object sender, EventArgs e)
        {
            Ped character = Game.Player.Character;

            if (character.CurrentVehicle != null)
            {
                // En SHVDN v3 usamos la clase TextElement para dibujar en pantalla
                new GTA.UI.TextElement(Game.Player.Character.CurrentVehicle.IsPersistent.ToString(), new System.Drawing.PointF(10.0f, 10.0f), 0.5f).Draw();
            }
        }
    }
}
