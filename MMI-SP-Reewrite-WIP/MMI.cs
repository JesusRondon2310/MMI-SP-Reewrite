using System;
using GTA;
using MMI_SP.PatternMatching;
using MMI_SP.Helpers;

namespace MMI_SP
{
    internal class MMI : Script
    {
        public static bool IsDebug = false;
        private static bool _initialized = false;
        public static bool IsInitialized => _initialized;

        public MMI()
        {
#if DEBUG
            IsDebug = true;
#endif
            Tick += Initialize;
        }

        private void Initialize(object sender, EventArgs e)
        {
            Logger.ResetLogFile();

            Logger.Debug("Waiting for game to be loaded...");
            Logger.Debug("Game is loaded");

            Logger.Debug("Waiting for screen to fade...");
            while (GTA.UI.Screen.IsFadingIn)
                Yield();

            Logger.Debug("Screen has faded");

            Logger.Debug("Loading configuration values...");
            Config.Initialize();
            Logger.Debug("Configuration values loaded");

            Logger.Debug("Initializing InsuranceManager...");
            var result = Insurance.Core.Initialize();

            switch (result)
            {
                case Ok<bool> _:
                    Logger.Debug("InsuranceManager initialized successfully");
                    _initialized = true;
                    break;

                case Err<bool> err:
                    Logger.Error($"InsuranceManager initialization failed: {err.FullMessage}");
                    Notification.Show("CHAR_CARSITE", "Mors Mutual", "Error initializing MMI‑SP. Check the log.", "");
                    break;
            }

            Tick -= Initialize;
        }
    }
}