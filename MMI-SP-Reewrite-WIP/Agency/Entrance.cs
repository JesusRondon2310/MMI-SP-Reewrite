using System;
using GTA;
using GTA.Native;
using GTA.Math;
using MMI_SP.Agency.Office;

namespace MMI_SP.Agency
{
    class Reception : Script
    {
        private Blip _agencyBlip;
        private static Vector3 _position = new Vector3(-825.7242f, -261.2752f, 37.0000f);
        public static Vector3 Position => _position;

        private EntryManager _state;

        public Reception()
        {
            Tick += Initialize;
        }

        void Initialize(object sender, EventArgs e)
        {
            _agencyBlip = CreateBlip();
            _state = new EntryManager(this);

            if (Game.Player.Character.Position.DistanceTo(EntryManager.OfficePlayerPos) <= 2.0f)
                _state.ErrorCancel(false);

            Tick -= Initialize;
            Aborted += OnAborted;
            Tick += OnTick;
        }

        void OnTick(object sender, EventArgs e)
        {
            _state?.OnTick();          // maneja cutscene, NPC, menú
            DisplayAgencyThisFrame();  // detección de la puerta
        }

        private void OnAborted(object sender, EventArgs e)
        {
            _state?.CleanUp();
            if (_agencyBlip.Exists()) _agencyBlip.Delete();
        }

        private void DisplayAgencyThisFrame()
        {
            if (Game.Player.Character.Position.DistanceTo(_position) >= 4.0) return;
            if (Game.Player.Character.IsInVehicle()) return;

            if (Game.Player.Wanted.WantedLevel > 0)
            {
                GTA.UI.Screen.ShowHelpTextThisFrame("No puedes entrar con nivel de búsqueda.");
                return;
            }

            GTA.UI.Screen.ShowHelpTextThisFrame("Presiona ~y~E~w~ para entrar a la oficina de Mors Mutual.");
            if (!Game.IsControlJustReleased(Control.Context)) return;

            try
            {
                _state.Enter();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                GTA.UI.Notification.PostTicker("MMI-SP: Error al crear la oficina.", false);
                _state.ErrorCancel(true);
            }
        }

        private Blip CreateBlip()
        {
            Blip blip = World.CreateBlip(_position);
            blip.Sprite = BlipSprite.Michael;
            blip.Color = (BlipColor)6;
            blip.Name = "Mors Mutual Insurance";
            blip.IsShortRange = true;
            return blip;
        }
    }
}