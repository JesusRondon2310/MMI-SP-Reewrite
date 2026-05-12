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

        public Handler(Script script)
        {
            _office = new Office.Manager();
            _cutscene = new CutsceneManager();
            _menu = new UI(Exit);
        }

        public void Enter()
        {
            EnterSequence.Execute(_menu, _cutscene, _office, Exit);
        }

        public void Exit()
        {
            ExitSequence.Execute(_office, _cutscene);
        }

        public void ErrorCancel(bool menu = true)
        {
            CancelSequence.Execute(_menu, _cutscene, menu ? (Action)Exit : null, menu);
        }

        public void OnTick()
        {
            _cutscene.UpdateHUD();
            _office.UpdateSpeechTimer();
            _menu?.Update();
        }

        public void CleanUp()
        {
            CleanUpSequence.Execute(_office);
        }
    }
}