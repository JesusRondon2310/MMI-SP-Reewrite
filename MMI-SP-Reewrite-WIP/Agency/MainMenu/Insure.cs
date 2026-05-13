using System;
using GTA;
using GTA.Native;
using NativeUI;
using MMI_SP.Helpers;
using MMI_SP.Insurance;
using MMI_SP.Agency.MainMenu.Buttons;

namespace MMI_SP.Agency.MainMenu
{
    internal static class Insure
    {
        private static UIMenuItem _itemInsure;

        public static void Build(UIMenu parentMenu)
        {
            RemoveExistingItem(parentMenu);

            Vehicle veh = Game.Player.LastVehicle;
            if (veh == null || !veh.Exists()) return;

            DetermineState(out string description, out bool enabled, out string rightLabel);

            _itemInsure = Buttons.Build.MainMenuItem(
                parentMenu,
                "Asegurar vehículo",
                description,
                enabled,
                rightLabel,
                Action.OnActivated
            );
        }

        public static void Update(UIMenu parentMenu)
        {
            if (_itemInsure == null) return;

            DetermineState(out string description, out bool enabled, out string rightLabel);

            _itemInsure.Enabled = enabled;
            _itemInsure.Description = description;
            _itemInsure.SetRightLabel(rightLabel);
        }

        private static void DetermineState(out string description, out bool enabled, out string rightLabel)
        {
            Vehicle veh = Game.Player.LastVehicle;
            if (veh == null || !veh.Exists())
            {
                description = "No hay vehículo disponible";
                enabled = false;
                rightLabel = "";
                return;
            }

            int cost = Core.GetCost(veh);
            string vehName = Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, veh.Model.Hash);
            bool isInsured = Core.IsInsured(veh);
            bool isInsurable = Core.IsInsurable(veh);

            if (isInsured)
            {
                description = $"Este vehículo ya está asegurado\n{vehName}";
                enabled = false;
                rightLabel = "";
            }
            else if (!isInsurable)
            {
                description = $"No se puede asegurar este vehículo\n{vehName}";
                enabled = false;
                rightLabel = "";
            }
            else
            {
                description = $"Asegurar el último vehículo en uso\n{vehName}";
                enabled = true;
                rightLabel = cost + "$";
            }
        }

        private static void RemoveExistingItem(UIMenu parentMenu)
        {
            if (_itemInsure == null) return;
            _itemInsure.Activated -= Action.OnActivated;
            parentMenu.MenuItems.Remove(_itemInsure);
            _itemInsure = null;
        }
    }
}