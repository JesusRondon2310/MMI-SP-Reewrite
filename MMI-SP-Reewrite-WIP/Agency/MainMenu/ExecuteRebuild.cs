using MMI_SP.Agency.MainMenu.SubMenus;
using NativeUI;
using System.Windows.Forms;

namespace MMI_SP.Agency.MainMenu
{
    internal static class ExecuteRebuild
    {
        internal delegate void PopulateSubMenu();

        internal static void MainMenu(UIMenu mainMenu, MenuPool pool)
        {
            mainMenu.Clear();

            if (System.IO.File.Exists(Config.BannerImage))
                mainMenu.SetBannerType(Config.BannerImage);

            Insure.Build(mainMenu);
            Cancel.Build(mainMenu, pool);
            Recover.Build(mainMenu, pool);
            pool.RefreshIndex();
        }
        
        internal static void SubMenu(PopulateSubMenu submenu, MenuPool pool)
        {
            submenu();
            pool.RefreshIndex();
        }
    }
}