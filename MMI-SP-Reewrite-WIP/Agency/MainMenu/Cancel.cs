using NativeUI;
using MMI_SP.Agency.MainMenu.SubMenus;

namespace MMI_SP.Agency.MainMenu
{
    internal static class Cancel
    {
        private static CancelHandler _cancelHandler;
        private static UIMenu _parentMenu;
        private static MenuPool _pool;

        /// <summary>
        /// Crea el botón "Cancelar seguro" en el menú principal y su submenú asociado.
        /// </summary>
        public static void Build(UIMenu parentMenu, MenuPool pool)
        {
            _parentMenu = parentMenu;
            _pool = pool;
            _cancelHandler = new CancelHandler(parentMenu, pool, RebuildMainMenu);
            _cancelHandler.Build();
        }

        internal static void RebuildMainMenu()
        {
            ExecuteRebuild.MainMenu(_parentMenu, _pool);
        }

        internal static void Refresh()
        {
            if (_cancelHandler?.Pool == null) return;
            ExecuteRebuild.SubMenu(() => _cancelHandler.Repopulate(), _cancelHandler.Pool);
        }
    }
}