using GTA;
using GTA.Native;

namespace MMI_SP.Agency.Office
{
    internal class CutsceneManager
    {
        internal bool IsInCutscene { get; private set; }

        internal void Enter()
        {
            IsInCutscene = true;
        }

        internal void Exit()
        {
            IsInCutscene = false;
        }

        internal void UpdateHUD()
        {
            if (IsInCutscene)
                Function.Call(Hash.HIDE_HUD_AND_RADAR_THIS_FRAME);
        }
    }
}