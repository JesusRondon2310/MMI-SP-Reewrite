using System;
using GTA;
using MMI_SP.Agency.Office.Ambient;

namespace MMI_SP.Agency.Office
{
    internal class Manager
    {
        private Handler _office;
        private TimeSpan _officeLastCreation = new TimeSpan(0);
        private ItemsManager.OfficeItemsCollection _officeLastCollection = new ItemsManager.OfficeItemsCollection();
        private int _timerRandomSpeech;

        internal string CurrentCollectionType => _office?.ItemsCollection.Type.ToString() ?? "None";
        internal int CurrentCollectionCount => _office?.ItemsCollection.Count ?? 0;

        internal void CreateOffice()
        {
            // Reutiliza la colección si estamos en la misma franja horaria
            if (_officeLastCreation.Days == World.CurrentTimeOfDay.Days &&
                _officeLastCreation.Hours == World.CurrentTimeOfDay.Hours &&
                _officeLastCollection.Count > 0)
            {
                _office = new Handler(_officeLastCollection);
            }
            else
            {
                _office = new Handler();
                _officeLastCreation = World.CurrentTimeOfDay;
                _officeLastCollection?.DeleteItems();
                _officeLastCollection = new ItemsManager.OfficeItemsCollection(_office.ItemsCollection);
            }
        }

        internal void StartSpeechTimer()
        {
            // Intervalo aleatorio entre 10 y 20 segundos
            _timerRandomSpeech = Game.GameTime + new Random(Game.GameTime).Next(10000, 20000);
        }

        internal void UpdateSpeechTimer()
        {
            if (_office == null)
            {
                _timerRandomSpeech = 0;
                return;
            }

            if (_timerRandomSpeech == 0 || Game.GameTime >= _timerRandomSpeech)
            {
                // _office.NpcSay(DialogueManager.SpeechType.OfficeSomething);
                _timerRandomSpeech = Game.GameTime + new Random(Game.GameTime).Next(10000, 20000);
            }
        }

        internal void DestroyOffice()
        {
            _office?.CleanOffice();
            _office = null;
        }
    }
}