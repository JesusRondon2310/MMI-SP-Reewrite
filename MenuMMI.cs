using GTA;
using GTA.Native;
using MMI_SP.Common;
using MMI_SP.iFruit;
using NativeUI;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MMI_SP
{
    class MenuMMI
    {
        // Constantes para cadenas repetidas
        private const string NotifyChar = "CHAR_CARSITE";
        private const string NotifyTitle = "Mors Mutual";
        private const string NoMoneyMsg = "No tienes suficiente dinero.";
        private const string NoVehiclesMsg = "No tienes vehículos asegurados.";
        private const string EmptyItemTitle = "Vacío";

        private MenuPool _menuPool;
        private UIMenu _mainMenu;

        internal void MenuPoolProcessMenus() { _menuPool.ProcessMenus(); }

        public bool OpenedFromiFruit { get; private set; }

        // Elementos del menú
        private UIMenuItem _itemInsure;
        private UIMenu _submenuRecover;
        private UIMenu _submenuCancel;
        private UIMenu _submenuPlate;
        // private UIMenu _submenuBring;  // Descomentar cuando se reactive Bring

        public MenuMMI()
        {
            _menuPool = new MenuPool();
            _mainMenu = new UIMenu("Mors Mutual", "Menú de seguros");
            _menuPool.Add(_mainMenu);
        }

        internal void Show()
        {
            _mainMenu.Visible = true;
        }

        /// <summary>
        /// Construye el menú principal y sus submenús según el contexto (teléfono u oficina).
        /// </summary>
        internal void Create()
        {
            // Banner (descomentar si se desea usar imagen)
            // if (System.IO.File.Exists(Config.BannerImage))
            //     _mainMenu.SetBannerType(Config.BannerImage);

            if (OpenedFromiFruit)
            {
                if (Config.CaniFruitInsure) BuildItemInsure();
                if (Config.CaniFruitCancel) CreateMenuCancel(_mainMenu);
                if (Config.CaniFruitRecover) CreateMenuRecover(_mainMenu);
                if (Config.CaniFruitPlate) CreateMenuPlate(_mainMenu);
                // CreateMenuBring(_mainMenu);  // Descomentar cuando se reactive Bring
            }
            else
            {
                BuildItemInsure();
                CreateMenuCancel(_mainMenu);
                CreateMenuRecover(_mainMenu);
                CreateMenuPlate(_mainMenu);
            }
        }

        /// <summary>
        /// Elimina todo el contenido del menú y lo reconstruye.
        /// </summary>
        internal void Reset(bool iFruit = false)
        {
            OpenedFromiFruit = iFruit;
            _mainMenu.Clear();
            Create();
        }

        /// <summary>
        /// Reproduce el sonido de éxito (si se abrió desde iFruit) y muestra la notificación.
        /// </summary>
        /// <param name="message">Texto principal de la notificación.</param>
        private void PlaySuccess(string message)
        {
            if (OpenedFromiFruit)
                MMISound.Play(MMISound.SoundFamily.Okay);
            Utils.ShowNotification(NotifyChar, NotifyTitle, message, "");
        }

        /// <summary>
        /// Intenta gastar dinero. Devuelve true si el pago se realizó; false si no hay fondos.
        /// </summary>
        private bool TrySpendMoney(int amount)
        {
            if (Game.Player.Money < amount)
            {
                if (OpenedFromiFruit) MMISound.Play(MMISound.SoundFamily.NoMoney);
                Utils.ShowNotification(NotifyChar, NotifyTitle, NoMoneyMsg, "");
                return false;
            }
            Game.Player.Money -= amount;
            return true;
        }

        /// <summary>
        /// Reconstruye todos los submenús y el botón de asegurar.
        /// </summary>
        private void RefreshAllMenus()
        {
            BuildItemInsure();
            RebuildMenuCancel();
            RebuildMenuRecover();
            RebuildMenuPlate();
        }

        /// <summary>
        /// Asegura que los submenús no estén vacíos y luego refresca todo.
        /// Se usa tras asegurar un vehículo.
        /// </summary>
        private void RefreshAffectedMenusAfterInsurance()
        {
            EnsureMenuNotEmptyAndResetIndex(_submenuCancel, NoVehiclesMsg);
            if (OpenedFromiFruit)
            {
                // EnsureMenuNotEmptyAndResetIndex(_submenuBring, "...");
                // RebuildMenuBring();
            }
            EnsureMenuNotEmptyAndResetIndex(_submenuPlate, NoVehiclesMsg);

            RefreshAllMenus();
        }

        /// <summary>
        /// Se asegura de que el menú nunca esté vacío y restablece el índice seleccionado.
        /// </summary>
        private void EnsureMenuNotEmptyAndResetIndex(UIMenu menu, string itemDescription)
        {
            if (menu == null) return;

            if (menu.MenuItems.Count == 0)
            {
                UIMenuItem emptyItem = new UIMenuItem(EmptyItemTitle, itemDescription);
                emptyItem.Enabled = false;
                menu.AddItem(emptyItem);
                menu.CurrentSelection = 0;
            }
            else if (menu.CurrentSelection >= menu.MenuItems.Count)
            {
                menu.CurrentSelection = 0;
            }
        }

        /// <summary>
        /// Construye el botón "Asegurar vehículo" según el estado del último vehículo usado.
        /// </summary>
        private void BuildItemInsure()
        {
            Vehicle veh = Game.Player.LastVehicle;
            if (veh == null || !veh.Exists()) return;

            // Limpia el manejador anterior si ya existía
            if (_itemInsure != null)
            {
                _itemInsure.Activated -= OnInsureActivated;
            }

            int cost = InsuranceManager.GetVehicleInsuranceCost(veh);
            string vehName = Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, veh.Model.Hash);

            if (!InsuranceManager.IsVehicleInsured(Utils.Vehicle.GetVehicleIdentifier(veh)))
            {
                if (InsuranceManager.IsVehicleInsurable(veh))
                {
                    _itemInsure = new UIMenuItem("Asegurar vehículo", $"Coste: {cost}$\n{vehName}");
                    _itemInsure.Activated += OnInsureActivated;
                }
                else
                {
                    _itemInsure = new UIMenuItem("Asegurar vehículo", $"No se puede asegurar este vehículo\n{vehName}");
                    _itemInsure.Enabled = false;
                }
            }
            else
            {
                _itemInsure = new UIMenuItem("Asegurar vehículo", $"Este vehículo ya está asegurado\n{vehName}");
                _itemInsure.Enabled = false;
            }

            _mainMenu.AddItem(_itemInsure);
        }

        /// <summary>
        /// Manejador del evento de selección del botón "Asegurar vehículo".
        /// </summary>
        private void OnInsureActivated(UIMenu sender, UIMenuItem selectedItem)
        {
            Vehicle lastVeh = Game.Player.LastVehicle;
            if (lastVeh == null || !lastVeh.Exists()) return;

            string vehID = Utils.Vehicle.GetVehicleIdentifier(lastVeh);
            if (InsuranceManager.IsVehicleInsured(vehID)) return;
            if (!InsuranceManager.IsVehicleInsurable(lastVeh)) return;

            int cost = InsuranceManager.GetVehicleInsuranceCost(lastVeh);
            if (!TrySpendMoney(cost)) return;

            InsureVehicle(lastVeh);
        }

        /// <summary>
        /// Ejecuta el aseguramiento del vehículo y refresca los menús afectados.
        /// </summary>
        private void InsureVehicle(Vehicle veh)
        {
            if (OpenedFromiFruit)
            {
                InsuranceManager.Instance.InsureVehicle(veh);
                PlaySuccess("Vehículo asegurado correctamente.");
                _itemInsure.Enabled = false;

                RefreshAffectedMenusAfterInsurance();
            }
        }

        /// <summary>
        /// Crea un submenú estándar de MMI y lo vincula al menú padre.
        /// </summary>
        private UIMenu CreateStandardSubMenu(UIMenu parent, string title, string subtitle,
            string itemText, string itemDescription, Action rebuildAction)
        {
            UIMenu submenu = new UIMenu(title, subtitle);
            _menuPool.Add(submenu);

            UIMenuItem item = new UIMenuItem(itemText, itemDescription);
            item.Activated += (sender, selectedItem) => submenu.Visible = true;
            parent.AddItem(item);

            rebuildAction?.Invoke();
            return submenu;
        }

        // ------------------------------------------------------------------------
        // Submenú Cancelar seguro
        // ------------------------------------------------------------------------
        private void CreateMenuCancel(UIMenu menu)
        {
            _submenuCancel = CreateStandardSubMenu(menu, "Cancelar seguro",
                "Selecciona el vehículo a cancelar", "Cancelar seguro",
                "Elimina un vehículo de tu póliza", RebuildMenuCancel);
        }

        private void RebuildMenuCancel()
        {
            _submenuCancel.Clear();
            List<string> vehicleList = GetInsuredVehicleList();

            if (vehicleList.Count == 0)
            {
                AddEmptyItem(_submenuCancel, NoVehiclesMsg);
                return;
            }

            foreach (string vehID in vehicleList)
            {
                string modelName = InsuranceManager.Instance.GetVehicleModelName(vehID);
                string plate = InsuranceManager.Instance.GetVehicleLicensePlate(vehID);
                UIMenuItem cancelItem = new UIMenuItem(modelName, $"Matrícula: {plate}");
                _submenuCancel.AddItem(cancelItem);

                cancelItem.Activated += (sender, selectedItem) =>
                {
                    if (OpenedFromiFruit) InsuranceManager.Instance.CancelVehicle(vehID);
                    PlaySuccess("Seguro cancelado correctamente.");
                    RefreshAllMenus();
                };
            }
        }

        // ------------------------------------------------------------------------
        // Submenú Recuperar vehículo
        // ------------------------------------------------------------------------
        private void CreateMenuRecover(UIMenu menu)
        {
            _submenuRecover = CreateStandardSubMenu(menu, "Recuperar vehículo",
                "Selecciona un vehículo destruido para recuperarlo", "Recuperar vehículo",
                "Recupera un vehículo destruido de tu póliza", RebuildMenuRecover);
        }

        private void RebuildMenuRecover()
        {
            _submenuRecover.Clear();
            string currentCharacter = Game.Player.Character.Model.Hash.ToString();
            List<string> deadVehicleList = InsuranceManager.GetInsuredVehicles(currentCharacter, true);

            if (deadVehicleList.Count == 0)
            {
                AddEmptyItem(_submenuRecover, "No tienes vehículos destruidos que recuperar.");
                return;
            }

            foreach (string vehID in deadVehicleList)
            {
                int cost = InsuranceManager.Instance.GetVehicleInsuranceCost(vehID, InsuranceManager.Multiplier.Recover);
                string vehicleName = InsuranceManager.Instance.GetVehicleFriendlyName(vehID, false);
                UIMenuItem recoverItem = new UIMenuItem(vehicleName, $"Coste de recuperación: {cost}$");
                _submenuRecover.AddItem(recoverItem);

                recoverItem.Activated += (sender, selectedItem) =>
                {
                    if (!TrySpendMoney(cost)) return;

                    if (OpenedFromiFruit) InsuranceManager.Instance.RecoverVehicle(vehID);
                    PlaySuccess("Vehículo recuperado correctamente.");
                    RefreshAllMenus();
                };
            }
        }

        // ------------------------------------------------------------------------
        // Submenú Cambiar matrícula
        // ------------------------------------------------------------------------
        private void CreateMenuPlate(UIMenu menu)
        {
            _submenuPlate = CreateStandardSubMenu(menu, "Cambiar matrícula",
                "Edita la placa de tus vehículos asegurados", "Cambiar matrícula",
                "Modifica la matrícula de un vehículo asegurado", RefreshAllMenus);
        }

        private void RebuildMenuPlate()
        {
            _submenuPlate.Clear();
            const int plateChangeCost = 1000;
            List<string> vehicleList = GetInsuredVehicleList();

            if (vehicleList.Count == 0)
            {
                AddEmptyItem(_submenuPlate, NoVehiclesMsg);
                return;
            }

            foreach (string vehID in vehicleList)
            {
                string vehicleName = InsuranceManager.Instance.GetVehicleFriendlyName(vehID, false);
                UIMenuItem plateItem = new UIMenuItem(vehicleName, $"Coste: {plateChangeCost}$");
                _submenuPlate.AddItem(plateItem);

                plateItem.Activated += (sender, selectedItem) =>
                {
                    if (!TrySpendMoney(plateChangeCost)) return;
                    ChangePlateAction(vehID, plateChangeCost, plateItem);
                };
            }
        }

        /// <summary>
        /// Ejecuta el cambio de matrícula del vehículo con el identificador dado.
        /// </summary>
        private void ChangePlateAction(string vehID, int price, UIMenuItem item)
        {
            if (OpenedFromiFruit)
            {
                string oldPlate = InsuranceManager.Instance.GetVehicleLicensePlate(vehID);
                string newPlate = Game.GetUserInput(WindowTitle.EnterMessage60, oldPlate, 8);
                if (string.IsNullOrEmpty(newPlate)) return;

                newPlate = newPlate.Trim().ToUpperInvariant().PadRight(8);

                if (newPlate == oldPlate) return;

                // Cobrar (ya realizado en TrySpendMoney)
                string newVehID = InsuranceManager.Instance.ChangeVehicleLicensePlate(vehID, newPlate);

                // Actualizar descripción del ítem
                item.Description = InsuranceManager.Instance.GetVehicleFriendlyName(newVehID, false);

                // Actualizar vehículos en mundo con el identificador antiguo
                for (int i = InsuranceObserver.InsuredVehList.Count - 1; i >= 0; i--)
                {
                    if (Utils.Vehicle.GetVehicleIdentifier(InsuranceObserver.InsuredVehList[i]) == vehID)
                    {
                        InsuranceObserver.InsuredVehList[i].Mods.LicensePlate = newPlate;
                        InsuranceObserver.InsuredVehList.RemoveAt(i);
                    }
                }

                // Actualizar blips
                if (InsuranceObserver.BlipsToRemove.TryGetValue(vehID, out Blip vehBlip))
                {
                    InsuranceObserver.BlipsToRemove.Remove(vehID);
                    InsuranceObserver.BlipsToRemove.Add(newVehID, vehBlip);
                }

                PlaySuccess($"Placa cambiada: {oldPlate} → {newPlate}");

                // Refrescar todos los menús afectados
                RefreshAllMenus();
            }
        }

        // ------------------------------------------------------------------------
        // Métodos auxiliares
        // ------------------------------------------------------------------------
        private List<string> GetInsuredVehicleList()
        {
            string currentCharacter = Game.Player.Character.Model.Hash.ToString();
            List<string> vehicleList = InsuranceManager.GetInsuredVehicles(currentCharacter, false);
            vehicleList.AddRange(InsuranceManager.GetInsuredVehicles(currentCharacter, true));
            return vehicleList;
        }

        private void AddEmptyItem(UIMenu menu, string description)
        {
            UIMenuItem emptyItem = new UIMenuItem(EmptyItemTitle, description);
            emptyItem.Enabled = false;
            menu.AddItem(emptyItem);
        }

        /// <summary>
        /// Permite suscribirse al cierre del menú principal.
        /// </summary>
        public void OnMainMenuClosed(Action callback)
        {
            _mainMenu.OnMenuClose += (sender) => callback();
        }

        // ------------------------------------------------------------------------
        // Funcionalidad Bring (desactivada temporalmente, código migrado por si se reactiva)
        // ------------------------------------------------------------------------
        /*
        private void CreateMenuBring(UIMenu menu)
        {
            _submenuBring = CreateStandardSubMenu(menu, "Traer mi vehículo",
                "Un conductor traerá el vehículo hasta ti", "Traer mi vehículo",
                "Solicita que te acerquen un vehículo asegurado", RebuildMenuBring);
        }

        private void RebuildMenuBring()
        {
            _submenuBring.Clear();
            if (InsuranceObserver.GetBringableVehicles().Count > 0)
            {
                foreach (Vehicle veh in InsuranceObserver.GetBringableVehicles())
                {
                    string vehID = Utils.Vehicle.GetVehicleIdentifier(veh);
                    string currentCharacter = Game.Player.Character.Model.Hash.ToString();
                    if (currentCharacter != InsuranceManager.GetVehicleOwner(vehID)) continue;

                    int cost = (int)((Game.Player.Character.Position.DistanceTo(veh.Position) / 1000) * Config.BringVehicleBasePrice);
                    UIMenuItem bringItem = new UIMenuItem(InsuranceManager.Instance.GetVehicleFriendlyName(vehID, false), "Traer vehículo");
                    bringItem.SetRightLabel(cost + "$");
                    _submenuBring.AddItem(bringItem);

                    bringItem.Activated += (sender, selectedItem) =>
                    {
                        if (TrySpendMoney(cost))
                        {
                            if (OpenedFromiFruit) MMISound.Play(MMISound.SoundFamily.Okay);
                            InsuranceObserver.Instance.BringVehicleToPlayer(veh, cost, Config.BringVehicleInstant);
                            bringItem.Enabled = false;
                            Utils.ShowNotification(NotifyChar, NotifyTitle, "", "Vehículo en camino.");
                            _submenuBring.RemoveItemAt(_submenuBring.MenuItems.IndexOf(bringItem));
                            EnsureMenuNotEmptyAndResetIndex(_submenuBring, "No tienes vehículos disponibles.");
                        }
                    };
                }
            }
            else
            {
                AddEmptyItem(_submenuBring, "No tienes vehículos disponibles.");
            }
        }
        */
    }
}