using System;
using System.IO;
using NativeUI;
using MMI_SP.Helpers;

namespace MMI_SP.Agency.MainMenu.Buttons
{
    internal static class Build
    {
        internal static UIMenuItem MainMenuItem(
            UIMenu parentMenu,
            string text,
            string description,
            bool enabled,
            string rightLabel,
            ItemActivatedEvent onActivated)
        {
            UIMenuItem item = new UIMenuItem(text, description);
            item.Enabled = enabled;
            if (!string.IsNullOrEmpty(rightLabel))
                item.SetRightLabel(rightLabel);

            item.Activated += onActivated;
            parentMenu.AddItem(item);
            return item;
        }

        internal static UIMenu SubMenu(
            UIMenu parentMenu,
            MenuPool pool,
            string title,
            string description,
            string itemDescription,
            Action<string> onVehicleSelected,
            string emptyMessage)
        {
            // Usar el método disponible en NativeUI (como Bob74)
            UIMenu submenu = pool.AddSubMenu(parentMenu, title, description);

            // Si la descripción del ítem es diferente, la cambiamos
            if (!string.IsNullOrEmpty(itemDescription) && itemDescription != description)
            {
                UIMenuItem lastItem = parentMenu.MenuItems[parentMenu.MenuItems.Count - 1];
                lastItem.Description = itemDescription;
            }

            // Configurar banner
            if (File.Exists(Config.BannerImage))
                submenu.SetBannerType(Config.BannerImage);

            // Poblar con los vehículos
            Fill.SubMenu(submenu, onVehicleSelected, emptyMessage);
            return submenu;
        }
    }
}