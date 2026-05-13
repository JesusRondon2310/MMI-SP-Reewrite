using GTA;
using NativeUI;
using MMI_SP.Helpers;
using MMI_SP.Insurance;

namespace MMI_SP.Agency.MainMenu
{
    internal static class Action
    {
        private const string NotifyChar = "CHAR_CARSITE";
        private const string NotifyTitle = "Mors Mutual";

        internal static void OnActivated(UIMenu sender, UIMenuItem selectedItem)
        {
            if (selectedItem == null || !selectedItem.Enabled) return;

            Vehicle veh = Game.Player.LastVehicle;
            if (veh == null || !veh.Exists()) return;
            if (Core.IsInsured(veh)) return;
            if (!Core.IsInsurable(veh)) return;

            int cost = Core.GetCost(veh);
            if (Game.Player.Money < cost)
            {
                Notification.Show(NotifyChar, NotifyTitle, "No tienes suficiente dinero.", "");
                return;
            }

            Game.Player.Money -= cost;
            Core.Insure(veh);
            Cancel.Refresh();
            Notification.Show(NotifyChar, NotifyTitle, "Vehículo asegurado correctamente.", "");

            selectedItem.Enabled = false;
            selectedItem.SetRightLabel("");
        }
    }
}