using MMI_SP.Agency.MainMenu.SubMenus;
using NativeUI;
using System;

namespace MMI_SP.Agency.MainMenu
{
    internal class UI
    {
        private MenuPool _pool;
        private UIMenu _mainMenu;

        internal UI()
        {
            BuildAndSubscribe();
        }

        internal void RebuildMenu()
        {
                BuildAndSubscribe();
        }

        private void BuildAndSubscribe()
        {
            // Creación directa del pool y menú principal (Bob74 original)
            _pool = new MenuPool();
            _mainMenu = new UIMenu("", "");
            _pool.Add(_mainMenu);

            // Poblar los botones (Insure, Cancel, etc.)
            ExecuteRebuild.MainMenu(_mainMenu, _pool);
        }

        internal bool IsAnyMenuVisible()
        {
            if (_mainMenu != null && _mainMenu.Visible) return true;

            // Verificar si CancelHandler tiene submenú y está visible
            if (CancelHandler.Instance != null && CancelHandler.Instance.Submenu != null &&
                CancelHandler.Instance.Submenu.Visible) return true;

            return false;
        }

        internal void Show() => _mainMenu.Visible = true;
        internal void Hide() => _mainMenu.Visible = false;
        internal void Update() => _pool.ProcessMenus();

        internal bool IsMainMenuVisible() => _mainMenu != null && _mainMenu.Visible;
        internal bool IsSubmenuVisible() => CancelHandler.Instance?.Submenu?.Visible ?? false;
    }
}