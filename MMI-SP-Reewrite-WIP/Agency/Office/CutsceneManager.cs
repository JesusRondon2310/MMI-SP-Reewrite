using GTA;
using GTA.Native;

namespace MMI_SP.Agency.Office
{
    internal class CutsceneManager
    {
        internal bool IsInCutscene { get; private set; }

        /// <summary>
        /// Marca el inicio de la cinemática.
        /// </summary>
        public void EnterCutscene()
        {
            IsInCutscene = true;
        }

        /// <summary>
        /// Marca el fin de la cinemática.
        /// </summary>
        public void ExitCutscene()
        {
            IsInCutscene = false;
        }

        /// <summary>
        /// Cancela la cinemática (por error).
        /// </summary>
        public void CancelCutscene()
        {
            IsInCutscene = false;
        }

        /// <summary>
        /// Oculta el HUD si está en cinemática.
        /// </summary>
        public void UpdateHUD()
        {
            if (IsInCutscene)
                Function.Call(Hash.HIDE_HUD_AND_RADAR_THIS_FRAME);
        }
    }
}