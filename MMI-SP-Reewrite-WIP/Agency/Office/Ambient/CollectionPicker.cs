using GTA;
using System;
using System.Collections.Generic;
using static MMI_SP.Agency.ItemsManager;

namespace MMI_SP.Agency.Office.Ambient
{
    internal static class CollectionPicker
    {
        public static OfficeItemsCollection Pick()
        {
            Random rnd = new Random(Game.GameTime);
            List<OfficeItemsCollection> pool = new List<OfficeItemsCollection>();

            int hour = World.CurrentTimeOfDay.Hours;

            if (hour >= 2 && hour < 12)
            {
                pool.AddRange(ItemsManager.GetItemsCollection(CollectionType.Normal));
            }
            else if (hour >= 12 && hour < 14)
            {
                pool.AddRange(ItemsManager.GetItemsCollection(CollectionType.Midday));
                pool.AddRange(ItemsManager.GetItemsCollection(CollectionType.Normal));
            }
            else if (hour >= 14) // La condición original (>= 14 && < 0) no se cumple nunca; se asume resto del día
            {
                pool.AddRange(ItemsManager.GetItemsCollection(CollectionType.Normal));
            }
            else // hour >= 0 && hour < 2 (madrugada)
            {
                pool.AddRange(ItemsManager.GetItemsCollection(CollectionType.Normal));
                pool.AddRange(ItemsManager.GetItemsCollection(CollectionType.Night));
            }

            return pool[rnd.Next(0, pool.Count)];
        }
    }
}