using GTA;
using GTA.Native;
using NativeUI;
using MMI_SP.Insurance;
using System;

namespace MMI_SP.Agency.MainMenu
{
    internal static class Menu
    {
        public static UIMenuItem _itemInsure;

        public static void InsureButtonBuild(UIMenu parentMenu)
        {
            Vehicle veh = Game.Player.LastVehicle;
            if (veh == null || !veh.Exists()) return;

            if (_itemInsure != null)
            {
                _itemInsure.Activated -= Buttons.Insure;
                parentMenu.MenuItems.Remove(_itemInsure);
            }

            int cost = Core.GetCost(veh);
            string vehName = Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, veh.Model.Hash);
            bool isInsured = Core.IsInsured(veh);
            bool isInsurable = Core.IsInsurable(veh);

            string title = "Asegurar vehículo";
            string description;
            bool enabled = true;

            if (isInsured)
            {
                description = $"Este vehículo ya está asegurado\n{vehName}";
                enabled = false;
            }
            else if (!isInsurable)
            {
                description = $"No se puede asegurar este vehículo\n{vehName}";
                enabled = false;
            }
            else
            {
                description = $"Asegurar el último vehículo en uso\n{vehName}";
                enabled = true;
            }

            _itemInsure = new UIMenuItem(title, description);
            _itemInsure.Enabled = enabled;
            if (enabled)
                _itemInsure.SetRightLabel(cost + "$");

            _itemInsure.Activated += Buttons.Insure;
            parentMenu.AddItem(_itemInsure);
        }
    }
}