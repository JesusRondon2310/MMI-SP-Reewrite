using GTA;

namespace MMI_SP.Agency.Office.Ambient
{
    internal static class CleanUp
    {
        public static void Execute()
        {
            World.RenderingCamera = null;

            if (Builder.CurrentCamera != null)
            {
                Builder.CurrentCamera.IsActive = false;
                Builder.CurrentCamera.Delete();
            }

            World.Weather = Builder.CurrentWeather;

            if (Builder.CurrentNpc != null && Builder.CurrentNpc.Exists())
            {
                Builder.CurrentNpc.Task.ClearAllImmediately();
                Builder.CurrentNpc.Delete();
            }
        }
    }
}