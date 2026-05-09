using System;

using GTA;

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

            Logger.Debug("Waiting for game to be loaded...");
            Logger.Debug("Game is loaded");

            Logger.Debug("Waiting for screen to fade...");
            while (GTA.UI.Screen.IsFadingIn)
            {
                Yield();
            }
            Logger.Debug("Screen has faded");

            
            Logger.Debug("Loading configuration values...");
            Config.Initialize();
            Logger.Debug("Configuration values loaded");

            _initialized = true;

            Tick -= Initialize;
        }


        void DebugOnTick(object sender, EventArgs e)
        {
            Ped character = Game.Player.Character;

            if (character.CurrentVehicle != null)
            {
                Debug.ShowVehicleInfo(character.CurrentVehicle);
            }
        }
    }
}
