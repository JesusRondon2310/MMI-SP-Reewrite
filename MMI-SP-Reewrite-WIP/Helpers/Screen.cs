using GTA;
using GTA.Native;

namespace MMI_SP.Helpers
{
    internal static class Screen
    {
        /// <summary>
        /// Oculta el HUD y el radar durante un tiempo determinado (en milisegundos).
        /// </summary>
        public static void UIHandler(int duration)
        {
            int timer = Game.GameTime + duration;
            do
            {
                Function.Call(Hash.HIDE_HUD_AND_RADAR_THIS_FRAME);
                Script.Yield();
            } while (timer >= Game.GameTime);
        }
    }
}