using NativeUI;
using System.IO;

namespace MMI_SP.Agency.MainMenu
{
    internal static class Interface
    {
        internal static (MenuPool pool, UIMenu mainMenu) Build()
        {
            var pool = new MenuPool();
            var mainMenu = new UIMenu("Mors Mutual", "Menú de seguros");
            pool.Add(mainMenu);

            // Banner
            if (File.Exists(Config.BannerImage))
                mainMenu.SetBannerType(Config.BannerImage);

            return (pool, mainMenu);
        }
    }
}