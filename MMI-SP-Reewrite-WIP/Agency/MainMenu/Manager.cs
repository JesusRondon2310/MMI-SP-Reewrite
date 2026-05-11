using System;
using NativeUI;

namespace MMI_SP.Agency.MainMenu
{
    internal class UI
    {
        private MenuPool _pool;
        private UIMenu _mainMenu;
        private Action _onCloseAction;      // Acción a ejecutar al cerrar
        private bool _closeSubscribed;      // Para evitar suscripciones duplicadas

        /// <summary> Manejador fijo del evento OnMenuClose </summary>
        private void HandleOnMenuClose(UIMenu sender)
        {
            _onCloseAction?.Invoke();
        }

        internal UI()
        {
            (_pool, _mainMenu) = Interface.Build();  // Ya incluye el banner
            _pool.RefreshIndex();
            _closeSubscribed = false;
        }

        internal void SetupEvent()
        {
            _mainMenu.OnItemSelect += (sender, item, index) =>
            {
                if (item == Menu._itemInsure)
                {
                    Buttons.Insure(_mainMenu, item);
                }
            };
        }

        internal void Show() => _mainMenu.Visible = true;
        internal void Hide() => _mainMenu.Visible = false;
        internal void Update() => _pool.ProcessMenus();
        internal void Reset() => Execute.Rebuild(_mainMenu);

        /// <summary>
        /// Reconstruye el menú desde cero, eliminando el anterior y suscribiendo
        /// el manejador fijo al nuevo.
        /// </summary>
        public void RebuildMenu(Action onClose)
        {
            // 1. Desuscribir del menú anterior (si existe)
            if (_mainMenu != null && _closeSubscribed)
            {
                _mainMenu.OnMenuClose -= HandleOnMenuClose;
                _closeSubscribed = false;
            }

            // 2. Crear un nuevo menú y pool
            (_pool, _mainMenu) = Interface.Build();
            _pool.RefreshIndex();

            // 3. Reconstruir los botones
            Execute.Rebuild(_mainMenu);

            // 4. Guardar la nueva acción y suscribir el manejador fijo
            _onCloseAction = onClose;
            if (!_closeSubscribed)
            {
                _mainMenu.OnMenuClose += HandleOnMenuClose;
                _closeSubscribed = true;
            }
        }

        /// <summary>
        /// Asigna la acción que se ejecutará al cerrar el menú principal.
        /// Debe llamarse justo después de construir UI o tras RebuildMenu.
        /// </summary>
        internal void Closed(Action callback)
        {
            _onCloseAction = callback;
            if (!_closeSubscribed)
            {
                _mainMenu.OnMenuClose += HandleOnMenuClose;
                _closeSubscribed = true;
            }
        }
    }
}