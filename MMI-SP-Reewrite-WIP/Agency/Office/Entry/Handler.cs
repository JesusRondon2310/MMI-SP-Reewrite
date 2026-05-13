using GTA;
using MMI_SP.Agency.MainMenu;
using System;

namespace MMI_SP.Agency.Office.Entry
{
    internal class Handler
    {
        private readonly Office.Manager _office;
        private readonly CutsceneManager _cutscene;
        private readonly UI _menu;
        private bool _isExiting = false;

        public Handler(Script script)
        {
            _office = new Office.Manager();
            _cutscene = new CutsceneManager();
            _menu = new UI();
        }

        public void Enter()
        {
            _isExiting = false;
            EnterSequence.Execute(_menu, _cutscene, _office);
        }

        public void Exit()
        {
            ExitSequence.Execute(_office, _cutscene);
        }

        public void ErrorCancel(bool menu = true)
        {
            CancelSequence.Execute(_menu, _cutscene, menu);
        }

        public void OnTick()
        {
            _cutscene.UpdateHUD();
            _office.UpdateSpeechTimer();
            _menu?.Update();

            // Salir solo si ningún menú está visible
            if (_menu != null && !_menu.IsAnyMenuVisible() && !_isExiting)
            {
                // Debug para verificar qué menús están visibles
                Logger.Debug($"MainMenu visible: {_menu.IsMainMenuVisible()}, Submenu visible: {_menu.IsSubmenuVisible()}");
                _isExiting = true;
                Exit();
            }
        }

        public void CleanUp()
        {
            CleanUpSequence.Execute(_office);
        }
    }
}