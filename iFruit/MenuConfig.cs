using System;
using System.Collections.Generic;
using System.Drawing;

using NativeUI;
using GTA;
using GTA.Native;
using System.IO;

namespace MMI_SP.iFruit
{
    class MenuConfig
    {
        private static string _menuTitle = "Configuración MMI";
        public string MenuTitle { get => _menuTitle; set => _menuTitle = value; }

        private static readonly Point _offset = new Point(640, 360);

        private readonly MenuPool _menuPool;
        readonly UIMenu _mainMenu = new UIMenu(_menuTitle, "Ajustes del mod Mors Mutual Insurance", _offset);


        internal void MenuPoolProcessMenus() { _menuPool.ProcessMenus(); }

        //Main Base
        public MenuConfig()
        {
            _menuPool = new MenuPool();
            _menuPool.Add(_mainMenu);

            UIMenu submenuGeneral = AddSubMenu(_menuPool, _mainMenu, _menuTitle, "General", _offset);
            AddMenuConfigLanguage(submenuGeneral, "Language", Config.Settings.GetValue("General", "language", "default"), "Idioma");
            AddMenuConfigCheckbox(submenuGeneral, "General", "PersistentInsuredVehicles", Config.PersistentVehicles, "Vehículos asegurados persistentes");

            UIMenu submenuiFruit = AddSubMenu(_menuPool, _mainMenu, _menuTitle, "iFruit", _offset);
            AddMenuConfigList(submenuiFruit, "iFruit", "PhoneVolume", Config.iFruitVolume, "Volumen del teléfono", 0, 100, 5);
            AddMenuConfigCheckbox(submenuiFruit, "iFruit", "CaniFruitInsure", Config.CaniFruitInsure, "Asegurar desde iFruit");
            AddMenuConfigCheckbox(submenuiFruit, "iFruit", "CaniFruitCancel", Config.CaniFruitCancel, "Cancelar desde iFruit");
            AddMenuConfigCheckbox(submenuiFruit, "iFruit", "CaniFruitRecover", Config.CaniFruitRecover, "Recuperar desde iFruit");
            AddMenuConfigCheckbox(submenuiFruit, "iFruit", "CaniFruitPlate", Config.CaniFruitPlate, "Cambiar matrícula desde iFruit");

            UIMenu submenuInsurance = AddSubMenu(_menuPool, _mainMenu, _menuTitle, "Seguros", _offset);
            AddMenuConfigList(submenuInsurance, "Insurance", "InsuranceCostMultiplier", Config.InsuranceMult, GetCostMultiplierDescription("InsuranceCostMultiplier"), 0f, 100f, 0.1f);
            AddMenuConfigList(submenuInsurance, "Insurance", "RecoverCostMultiplier", Config.RecoverMult, GetCostMultiplierDescription("RecoverCostMultiplier"), 0f, 100f, 0.1f);
            AddMenuConfigList(submenuInsurance, "Insurance", "StolenCostMultiplier", Config.StolenMult, GetCostMultiplierDescription("StolenCostMultiplier"), 0f, 100f, 0.1f);

            UIMenu submenuBringVehicle = AddSubMenu(_menuPool, _mainMenu, _menuTitle, "Traer vehículo", _offset);
            AddMenuConfigList(submenuBringVehicle, "BringVehicle", "BringVehicleBasePrice", Config.BringVehicleBasePrice, "Precio base", 0, 2000, 50);
            AddMenuConfigBringVehicleInstant(submenuBringVehicle, "BringVehicle", "BringVehicleInstant", Config.BringVehicleInstant, "");
            AddMenuConfigList(submenuBringVehicle, "BringVehicle", "BringVehicleRadius", Config.BringVehicleRadius, "Radio de búsqueda", 10, 2000, 5);
            AddMenuConfigList(submenuBringVehicle, "BringVehicle", "BringVehicleTimeout", Config.BringVehicleTimeout, "Tiempo de espera", 1, 30, 1);
        }

        internal void Show()
        {
            _mainMenu.Visible = true;
            Function.Call(Hash.SET_CURSOR_POSITION, 0.5f, 0.5f);    // Cursor position centered
        }

        private void AddMenuConfigLanguage(UIMenu menu, string key, string value, string description)
        {
            bool found = false;
            int counter = 0;
            List<dynamic> languages = new List<dynamic>();

            foreach (string file in Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MMI"), "*.xml"))
            {
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.Name != "db.xml")
                { 
                    string name = fileInfo.Name.Substring(0, fileInfo.Name.Length - 4);
                    languages.Add(name);
                    if (!found)
                        if (string.Compare(value, name, true) == 0)
                            found = true;
                        else
                            counter++;
                }
            }

            UIMenuListItem listItem = new UIMenuListItem(key, languages, counter, description);
            menu.AddItem(listItem);
            menu.OnListChange += (sender, item, index) =>
            {
                if (item == listItem)
                {
                    Config.Settings.SetValue("General", key, item.Items[index].ToString());
                    Config.Settings.Save();
                }
            };
        }

        private void AddMenuConfigBringVehicleInstant(UIMenu menu, string section, string key, bool isChecked, string description)
        {
            string textTrue = "Instantáneo";
            string textFalse = "Con retraso";

            UIMenuCheckboxItem notifyItem = new UIMenuCheckboxItem(key, isChecked, description);
            if (notifyItem.Checked)
                notifyItem.Description = textTrue;
            else
                notifyItem.Description = textFalse;

            menu.AddItem(notifyItem);
            menu.OnCheckboxChange += (sender, item, index) =>
            {
                if (item == notifyItem)
                {
                    Config.Settings.SetValue(section, key, item.Checked);
                    Config.UpdateValue(key, item.Checked);
                    Config.Settings.Save();
                    if (item.Checked)
                        item.Description = textTrue;
                    else
                        item.Description = textFalse;
                }
            };
        }

        private void AddMenuConfigList(UIMenu menu, string section, string key, float value, string description, float startValue, float stopValue, float increment)
        {
            bool found = false;
            int counter = 0;
            List<dynamic> values = new List<dynamic>();

            for (float i = startValue; i < stopValue; i += increment)
            {
                values.Add(Math.Round(i, 1, MidpointRounding.AwayFromZero));
                if (!found)
                    if (Math.Round(value, 1, MidpointRounding.AwayFromZero) == Math.Round(i, 1, MidpointRounding.AwayFromZero))
                        found = true;
                    else
                        counter++;
            }

            UIMenuListItem listItem = new UIMenuListItem(key, values, counter, description);
            menu.AddItem(listItem);
            menu.OnListChange += (sender, item, index) =>
            {
                if (item == listItem)
                {
                    Config.Settings.SetValue(section, key, ((float)item.Items[index]).ToString().ToString().Replace(",", "."));
                    Config.UpdateValue(key, (float)item.Items[index]);
                    Config.Settings.Save();
                }
            };
        }
        private void AddMenuConfigList(UIMenu menu, string section, string key, int value, string description, int startValue, int stopValue, int increment)
        {
            bool found = false;
            int counter = 0;
            List<dynamic> values = new List<dynamic>();

            for (int i = startValue; i <= stopValue; i += increment)
            {
                values.Add(i);
                if (!found)
                    if (value == i)
                        found = true;
                    else
                        counter++;
            }

            UIMenuListItem listItem = new UIMenuListItem(key, values, counter, description);
            menu.AddItem(listItem);
            menu.OnListChange += (sender, item, index) =>
            {
                if (item == listItem)
                {
                    Config.Settings.SetValue(section, key, (int)item.Items[index]);
                    Config.UpdateValue(key, (int)item.Items[index]);
                    Config.Settings.Save();
                }
            };
        }
        private void AddMenuConfigCheckbox(UIMenu menu, string section, string key, bool isChecked, string description)
        {
            UIMenuCheckboxItem notifyItem = new UIMenuCheckboxItem(key, isChecked, description);
            menu.AddItem(notifyItem);
            menu.OnCheckboxChange += (sender, item, index) =>
            {
                if (item == notifyItem)
                {
                    Config.Settings.SetValue(section, key, item.Checked);
                    Config.UpdateValue(key, item.Checked);
                    Config.Settings.Save();
                }
            };
        }

        private string GetCostMultiplierDescription(string costType)
        {
            Vehicle playerVehicle = Game.Player.LastVehicle;
            if (playerVehicle != null)
            {
                if (string.Compare(costType, "InsuranceCostMultiplier", true) == 0)
                    return "Coste del seguro" + " " + "(actual: $VehicleInsuranceCost$)";
                else if (string.Compare(costType, "RecoverCostMultiplier", true) == 0)
                    return "Coste de recuperación" + " " + "(actual: $VehicleRecoverCost$)";
            }
            else
            {
                if (string.Compare(costType, "InsuranceCostMultiplier", true) == 0)
                    return "Coste del seguro";
                else if (string.Compare(costType, "RecoverCostMultiplier", true) == 0)
                    return "Coste de recuperación";
            }
            return "";
        }

        // Workaround since NativeUI.MenuPool doesn't have an AddSubMenu function supporting menu Offset
        private UIMenu AddSubMenu(MenuPool pool, UIMenu menu, string title, string text, Point offset)
        {
            var item = new UIMenuItem(text);
            menu.AddItem(item);
            var submenu = new UIMenu(title, text, offset);
            pool.Add(submenu);
            menu.BindMenuToItem(submenu, item);
            return submenu;
        }

    }
}
