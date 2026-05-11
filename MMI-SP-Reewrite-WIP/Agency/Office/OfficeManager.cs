using System;
using GTA;

namespace MMI_SP.Agency.Office
{
    internal class OfficeManager
    {
        private Ambient _office;
        private TimeSpan _officeLastCreation = new TimeSpan(0);
        private ItemsManager.OfficeItemsCollection _officeLastItemsCollection = new ItemsManager.OfficeItemsCollection();
        private int _timerRandomSpeech = 0;

        internal string CurrentCollectionType => _office?.itemsCollection.Type.ToString() ?? "None";
        internal int CurrentCollectionCount => _office?.itemsCollection.Count ?? 0;

        /// <summary>
        /// Crea la oficina (o reutiliza la anterior si coincide el tiempo).
        /// </summary>
        public void CreateOffice()
        {
            if (_officeLastCreation.Days == World.CurrentTimeOfDay.Days &&
                _officeLastCreation.Hours == World.CurrentTimeOfDay.Hours &&
                _officeLastItemsCollection.Count > 0)
            {
                Logger.Debug("Office creation with known items");
                _office = new Ambient(_officeLastItemsCollection);
            }
            else
            {
                Logger.Debug("Office creation with new items");
                _office = new Ambient();
                _officeLastCreation = World.CurrentTimeOfDay;
                _officeLastItemsCollection?.DeleteItems();
                _officeLastItemsCollection = new ItemsManager.OfficeItemsCollection(_office.itemsCollection);
            }
        }

        /// <summary>
        /// Destruye la oficina y libera recursos.
        /// </summary>
        public void DestroyOffice()
        {
            _office?.CleanUp();
            _office = null;
        }

        /// <summary>
        /// Inicia el temporizador de diálogos aleatorios.
        /// </summary>
        public void StartSpeechTimer()
        {
            _timerRandomSpeech = Game.GameTime + new Random(Game.GameTime).Next(10000, 20000);
        }

        /// <summary>
        /// Actualiza el temporizador y dispara diálogos si corresponde.
        /// </summary>
        public void UpdateSpeechTimer()
        {
            if (_office == null)
            {
                _timerRandomSpeech = 0;
                return;
            }

            if (_timerRandomSpeech <= Game.GameTime && _timerRandomSpeech != 0)
            {
                // _office.NpcSay(DialogueManager.SpeechType.OfficeSomething);
                _timerRandomSpeech = Game.GameTime + new Random(Game.GameTime).Next(10000, 20000);
            }
            else if (_timerRandomSpeech == 0)
            {
                _timerRandomSpeech = Game.GameTime + new Random(Game.GameTime).Next(10000, 20000);
            }
        }
    }
}