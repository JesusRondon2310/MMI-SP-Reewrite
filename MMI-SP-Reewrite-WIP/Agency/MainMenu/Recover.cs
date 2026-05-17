using NativeUI;
using MMI_SP.Agency.MainMenu.SubMenus;

namespace MMI_SP.Agency.MainMenu
{
    internal static class Recover
    {
        // ==========================================
        // BLOQUE: Datos
        // ==========================================
        private static RecoverHandler _recoverHandler;
        private static UIMenu _parentMenu;
        private static MenuPool _pool;

        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        public static void Build(UIMenu parentMenu, MenuPool pool)
        {
            _parentMenu = parentMenu;
            _pool = pool;
            _recoverHandler = new RecoverHandler(parentMenu, pool);
            _recoverHandler.Build();
        }

        internal static void Refresh()
        {
            if (_recoverHandler?.Pool == null) return;
            ExecuteRebuild.SubMenu(() => _recoverHandler.Repopulate(), _recoverHandler.Pool);
        }
    }
}
