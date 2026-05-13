using System;
using System.Collections.Generic;
using GTA;
using GTA.Native;
using NativeUI;
using MMI_SP.Insurance;

namespace MMI_SP.Agency.MainMenu.Buttons
{
    internal static class Fill
    {
        internal static void SubMenu(UIMenu submenu, Action<string> onVehicleSelected, string emptyMessage)
        {
            if (submenu == null) return;
            submenu.Clear();

            List<string> insuredIds = Core.GetInsuredList();
            if (insuredIds.Count == 0)
            {
                UIMenuItem emptyItem = new UIMenuItem("Vacío", emptyMessage);
                emptyItem.Enabled = false;
                submenu.AddItem(emptyItem);
                return;
            }

            foreach (string vehId in insuredIds)
            {
                string modelName = GetModelNameFromId(vehId);
                string plate = GetPlateFromId(vehId);

                UIMenuItem item = new UIMenuItem(modelName, $"Matrícula: {plate}");
                item.Activated += (sender, selectedItem) => onVehicleSelected(vehId);
                submenu.AddItem(item);
            }
        }

        private static string GetModelNameFromId(string vehicleId)
        {
            string[] parts = vehicleId.Split('_');
            if (parts.Length >= 1 && int.TryParse(parts[0], out int modelHash))
                return Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, modelHash);
            return "Desconocido";
        }

        private static string GetPlateFromId(string vehicleId)
        {
            string[] parts = vehicleId.Split('_');
            return parts.Length >= 2 ? parts[1] : "";
        }
    }
}