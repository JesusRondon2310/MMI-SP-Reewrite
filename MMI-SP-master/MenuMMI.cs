using System.Collections.Generic;

using NativeUI;
using GTA;
using GTA.Native;

using MMI_SP.Common;
using MMI_SP.iFruit;

namespace MMI_SP
{
    using T = Translator;

    class MenuMMI
    {
        private MenuPool _menuPool;
        internal void MenuPoolProcessMenus() { _menuPool.ProcessMenus(); }

        private UIMenu _mainMenu = new UIMenu("", "Menu");
        internal UIMenu Mainmenu { get => _mainMenu; }

        public bool OpenedFromiFruit { get => _openedFromiFruit; private set => _openedFromiFruit = value; }
        private bool _openedFromiFruit = false;

        // Sub menus
        UIMenuItem _itemInsure;
        UIMenu _submenuRecover;
        UIMenu _submenuStolen;
        UIMenu _submenuCancel;
        UIMenu _submenuPlate;
        UIMenu _submenuBring;

        //Main Base
        public MenuMMI()
        {
            _menuPool = new MenuPool();
            _menuPool.Add(_mainMenu);
        }

        internal void Show()
        {
            _mainMenu.Visible = true;
        }

        /// <summary>
        /// Creates the menu
        /// </summary>
        internal void Create()
        {
            //if (System.IO.File.Exists(Config.BannerImage)) _mainMenu.SetBannerType(Config.BannerImage);

            if (OpenedFromiFruit)
            {
                if (Config.CaniFruitInsure) BuildItemInsure();
                if (Config.CaniFruitCancel) CreateMenuCancel(_mainMenu);
                if (Config.CaniFruitRecover) CreateMenuRecover(_mainMenu);
                if (Config.CaniFruitStolen) CreateMenuStolen(_mainMenu);
                if (Config.CaniFruitPlate) CreateMenuPlate(_mainMenu);
                CreateMenuBring(_mainMenu);
            }
            else
            {
                BuildItemInsure();
                CreateMenuCancel(_mainMenu);
                CreateMenuRecover(_mainMenu);
                CreateMenuStolen(_mainMenu);
                CreateMenuPlate(_mainMenu);
            }

            _menuPool.RefreshIndex();
        }
        /// <summary>
        /// Remove everything in the menu and recreates it.
        /// Allow us to have dynamic menu creation.
        /// </summary>
        internal void Reset(bool iFruit = false)
        {
            OpenedFromiFruit = iFruit;
            if (_mainMenu != null) _mainMenu.MenuItems.Clear();
            Create();
        }

        /// <summary>
        /// Set the menu index to the first item.
        /// </summary>
        /// <param name="menu"></param>
        private void RefreshMenuIndex(UIMenu menu, string itemDescription)
        {
            if (menu != null)
            {
                if (menu.MenuItems.Count <= 0)
                {
                    UIMenuItem cancelContract = new UIMenuItem(T.GetString("Empty"), itemDescription) { Enabled = false };
                    menu.AddItem(cancelContract);

                    menu.CurrentSelection = 0;
                }
                else
                {
                    if (menu.CurrentSelection > menu.MenuItems.Count - 1)
                        menu.CurrentSelection = 0;
                }

                menu.UpdateScaleform();
            }
        }

        /// <summary>
        /// Insure a vehicle by adding it to the database.
        /// </summary>
        private void BuildItemInsure()
        {
            Vehicle veh = Game.Player.LastVehicle;
            if (veh.Exists())
            {
                int cost = InsuranceManager.GetVehicleInsuranceCost(veh);

                if (!InsuranceManager.IsVehicleInsured(Utils.GetVehicleIdentifier(veh)))
                {
                    if (InsuranceManager.IsVehicleInsurable(veh))
                    {
                        _itemInsure = new UIMenuItem(T.GetString("InsureVehicle"), T.GetString("InsureVehicleDesc") + "\n" + veh.LocalizedName + ".");
                        _itemInsure.SetRightLabel(cost + "$");
                        _mainMenu.AddItem(_itemInsure);
                    }
                    else
                    {
                        _itemInsure = new UIMenuItem(T.GetString("InsureVehicle"), T.GetString("VehicleWrongType") + " " + veh.LocalizedName + ".");
                        _itemInsure.Enabled = false; // Asignación directa para evitar errores de constructor
                        _mainMenu.AddItem(_itemInsure);
                    }
                }
                else
                {
                    _itemInsure = new UIMenuItem(T.GetString("InsureVehicle"), T.GetString("VehicleAlreadyInsured") + "\n" + veh.LocalizedName + ".");
                    _itemInsure.Enabled = false;
                    _mainMenu.AddItem(_itemInsure);
                }

                _mainMenu.OnItemSelect += (sender, item, index) =>
                {
                    // Cláusula de guarda inicial
                    if (item != _itemInsure) return;

                    // Referencia local para limpiar el código
                    Vehicle lastVeh = Game.Player.LastVehicle;

                    // Validaciones rápidas (Guard Clauses)
                    if (lastVeh == null || !lastVeh.Exists()) return;

                    string vehID = Utils.GetVehicleIdentifier(lastVeh);
                    if (InsuranceManager.IsVehicleInsured(vehID)) return;
                    if (!InsuranceManager.IsVehicleInsurable(lastVeh)) return;

                    // Reemplazo de SE.Player.AddCashToPlayer
                    if (Game.Player.Money >= cost)
                    {
                        Game.Player.Money -= cost;
                        InsureVehicle(lastVeh);
                    }
                    else
                    {
                        if (OpenedFromiFruit) MMISound.Play(MMISound.SoundFamily.NoMoney);

                        // Reemplazo de UI.Notify por la nativa de notificación (0x7B5280EBA9840C72)
                        Utils.ShowNotification("CHAR_CARSITE", "Mors Mutual", T.GetString("NotifyNoMoney"), "");
                    }
                };
            }
        }
        /// <summary>
        /// Update the item's text according to the insured vehicle status.
        /// </summary>
        private void RefreshItemInsure()
        {
            if (_itemInsure != null)
            {
                Vehicle veh = Game.Player.LastVehicle;
                if (veh != null)
                {
                    if (veh.Exists())
                    {
                        if (!InsuranceManager.IsVehicleInsured(Utils.GetVehicleIdentifier(veh)))
                        {
                            if (InsuranceManager.IsVehicleInsurable(veh))
                            {
                                int cost = InsuranceManager.GetVehicleInsuranceCost(veh);
                                _itemInsure.Text = T.GetString("InsureVehicle");
                                _itemInsure.Description = T.GetString("InsureVehicleDesc") + "\n" + veh.LocalizedName + ".";
                                _itemInsure.Enabled = true;
                            }
                        }
                        else
                        {
                            _itemInsure.Description = T.GetString("VehicleAlreadyInsured") + "\n" + veh.LocalizedName + ".";
                            _itemInsure.Enabled = false;
                        }
                    }
                    else
                    {
                        _mainMenu.RemoveItemAt(0);
                    }
                }
                else
                {
                    _mainMenu.RemoveItemAt(0);
                }
            }
        }
        private void InsureVehicle(Vehicle veh)
        {
            if (OpenedFromiFruit) MMISound.Play(MMISound.SoundFamily.Okay);
            InsuranceManager.Instance.InsureVehicle(veh);
            Utils.ShowNotification("CHAR_CARSITE", "Mors Mutual", "", T.GetString("NotifyVehicleIsInsured"));

            _itemInsure.Enabled = false;

            // Updates
            RefreshMenuIndex(_submenuCancel, T.GetString("CancelInsuranceItemEmptyDesc"));
            RebuildMenuCancel();

            RefreshMenuIndex(_submenuStolen, T.GetString("StolenVehicleItemEmptyDesc"));
            RebuildMenuStolen();

            if (OpenedFromiFruit)
            {
                RefreshMenuIndex(_submenuBring, T.GetString("BringVehicleItemEmptyDesc"));
                RebuildMenuBring();
            }

            RefreshMenuIndex(_submenuPlate, T.GetString("PlateChangeItemEmptyDesc"));
            RebuildMenuPlate();
        }


        /// <summary>
        /// Cancel a contract by removing the vehicle from the database.
        /// </summary>
        /// <param name="menu"></param>
        private void CreateMenuCancel(UIMenu menu)
        {
            _submenuCancel = _menuPool.AddSubMenu(menu, T.GetString("CancelInsurance"), T.GetString("CancelInsuranceDesc"));
            if (System.IO.File.Exists(Config.BannerImage)) _submenuCancel.SetBannerType(Config.BannerImage);
            RebuildMenuCancel();
        }
        private void RebuildMenuCancel()
        {
            _submenuCancel.Clear();

            // Obtenemos el nombre del personaje actual (Franklin, Michael o Trevor) de forma nativa
            string currentCharacter = Game.Player.Character.Model.Hash.ToString();

            // Llamadas estáticas al InsuranceManager (sin .Instance)
            List<string> vehicleList = InsuranceManager.GetInsuredVehicles(currentCharacter, false);
            vehicleList.AddRange(InsuranceManager.GetInsuredVehicles(currentCharacter, true));


            if (vehicleList.Count > 0)
            {
                foreach (string vehID in vehicleList)
                {
                    UIMenuItem cancelContract = new UIMenuItem(InsuranceManager.Instance.GetVehicleModelName(vehID), T.GetString("CancelInsuranceItemDesc"));
                    cancelContract.SetRightLabel(InsuranceManager.Instance.GetVehicleLicensePlate(vehID));
                    _submenuCancel.AddItem(cancelContract);

                    _submenuCancel.OnItemSelect += (sender, item, index) =>
                    {
                        if (item == cancelContract)
                        {
                            if (OpenedFromiFruit) MMISound.Play(MMISound.SoundFamily.Okay);
                            InsuranceManager.Instance.CancelVehicle(vehID);
                            Utils.ShowNotification("CHAR_CARSITE", "Mors Mutual", "", T.GetString("NotifyCanceled"));
                            cancelContract.Enabled = false;

                            _submenuCancel.RemoveItemAt(index);

                            // Updates
                            RefreshMenuIndex(_submenuCancel, T.GetString("CancelInsuranceItemEmptyDesc"));

                            RefreshItemInsure();

                            RefreshMenuIndex(_submenuRecover, T.GetString("RecoverVehicleItemEmptyDesc"));
                            RebuildMenuRecover();

                            RefreshMenuIndex(_submenuStolen, T.GetString("StolenVehicleItemEmptyDesc"));
                            RebuildMenuStolen();

                            RefreshMenuIndex(_submenuPlate, T.GetString("PlateChangeItemEmptyDesc"));
                            RebuildMenuPlate();

                            if (OpenedFromiFruit)
                            {
                                RefreshMenuIndex(_submenuBring, T.GetString("BringVehicleItemEmptyDesc"));
                                RebuildMenuBring();
                            }
                        }
                    };
                }
            }
            else
            {
                UIMenuItem cancelContract = new UIMenuItem(T.GetString("Empty"), T.GetString("CancelInsuranceItemEmptyDesc")) { Enabled = false };
                _submenuCancel.AddItem(cancelContract);
            }
        }


        /// <summary>
        /// Recover a detroyed vehicle.
        /// </summary>
        /// <param name="menu"></param>
        private void CreateMenuRecover(UIMenu menu)
        {
            _submenuRecover = _menuPool.AddSubMenu(menu, T.GetString("RecoverVehicle"), T.GetString("RecoverVehicleDesc"));
            if (System.IO.File.Exists(Config.BannerImage)) _submenuRecover.SetBannerType(Config.BannerImage);
            RebuildMenuRecover();
        }
        private void RebuildMenuRecover()
        {
            _submenuRecover.Clear();

            // 1. Obtenemos el identificador del personaje de forma nativa (v3.7.0)
            string currentCharacter = Game.Player.Character.Model.Hash.ToString();

            // 2. Llamada al InsuranceManager (Ajusta .Instance según si lo hiciste static o no)
            List<string> deadVehicleList = InsuranceManager.GetInsuredVehicles(currentCharacter, true);
            if (deadVehicleList.Count > 0)
            {
                foreach (string vehID in deadVehicleList)
                {
                    int cost = InsuranceManager.Instance.GetVehicleInsuranceCost(vehID, InsuranceManager.Multiplier.Recover);
                    UIMenuItem recoverVehicle = new UIMenuItem(InsuranceManager.Instance.GetVehicleFriendlyName(vehID, false), T.GetString("NotifyDeliverVehicle"));
                    recoverVehicle.SetRightLabel(cost + "$");
                    _submenuRecover.AddItem(recoverVehicle);

                    _submenuRecover.OnItemSelect += (sender, item, index) =>
                    {
                        if (item == recoverVehicle)
                        {
                            if (Game.Player.Money >= cost)
                            {
                                if (OpenedFromiFruit) MMISound.Play(MMISound.SoundFamily.Okay);
                                InsuranceManager.Instance.RecoverVehicle(vehID);
                                // Reemplazo de UI.Notify por la notificación con icono de Mors Mutual
                                Utils.ShowNotification("CHAR_CARSITE", "Mors Mutual", "", T.GetString("NotifyDeliverVehicle"));
                                recoverVehicle.Enabled = false;

                                _submenuRecover.RemoveItemAt(index);

                                // Updates
                                RefreshMenuIndex(_submenuRecover, T.GetString("RecoverVehicleItemEmptyDesc"));
                                RebuildMenuStolen();

                                if (OpenedFromiFruit) RebuildMenuBring();
                            }
                            else
                            {
                                if (OpenedFromiFruit) MMISound.Play(MMISound.SoundFamily.NoMoney);
                                // Reemplazo final de UI.Notify para falta de fondos
                                Utils.ShowNotification("CHAR_CARSITE", "Mors Mutual", "", T.GetString("NotifyNoMoney"));
                            }
                        }
                    };
                }
            }
            else
            {
                UIMenuItem recoverVehicle = new UIMenuItem(T.GetString("Empty"), T.GetString("RecoverVehicleItemEmptyDesc")) { Enabled = false };
                _submenuRecover.AddItem(recoverVehicle);
            }
        }


        /// <summary>
        /// Recover a "stolen" vehicle (vehicle that vanished).
        /// </summary>
        /// <param name="menu"></param>
        private void CreateMenuStolen(UIMenu menu)
        {
            _submenuStolen = _menuPool.AddSubMenu(menu, T.GetString("StolenVehicle"), T.GetString("StolenVehicleDesc"));
            if (System.IO.File.Exists(Config.BannerImage)) _submenuStolen.SetBannerType(Config.BannerImage);
            RebuildMenuStolen();
        }
        private void RebuildMenuStolen()
        {
            _submenuStolen.Clear();

            // 1. Reemplazo de SE: Obtenemos el ID del personaje mediante el Hash del modelo
            string currentCharacter = Game.Player.Character.Model.Hash.ToString();

            // 2. Llamada al Manager: (Quita el .Instance si ya hiciste el método static)
            List<string> aliveVehicleList = InsuranceManager.GetInsuredVehicles(currentCharacter, false);

            if (aliveVehicleList.Count > 0)
            {
                foreach (string vehID in aliveVehicleList)
                {
                    int cost = InsuranceManager.Instance.GetVehicleInsuranceCost(vehID, InsuranceManager.Multiplier.Stolen);
                    UIMenuItem stolenVehicle = new UIMenuItem(InsuranceManager.Instance.GetVehicleFriendlyName(vehID, false), T.GetString("NotifyDeliverVehicle"));
                    stolenVehicle.SetRightLabel(cost + "$");
                    _submenuStolen.AddItem(stolenVehicle);

                    _submenuStolen.OnItemSelect += (sender, item, index) =>
                    {
                        if (item == stolenVehicle)
                        {
                            if (Game.Player.Money >= cost)
                            {
                                if (OpenedFromiFruit) MMISound.Play(MMISound.SoundFamily.Okay);

                                // Remove the vehicle from the world to avoid vehicle duplication
                                foreach (Vehicle veh in World.GetAllVehicles())
                                {
                                    if (Utils.GetVehicleIdentifier(veh) == vehID)
                                    {
                                        if (veh.AttachedBlip != null) veh.AttachedBlip.Delete();
                                        veh.Delete();
                                    }
                                }

                                InsuranceManager.Instance.RecoverVehicle(vehID);

                                // Reemplazo de UI.Notify por la notificación oficial de Mors Mutual (4 parámetros)
                                Utils.ShowNotification("CHAR_CARSITE", "Mors Mutual", "", T.GetString("NotifyDeliverVehicle"));
                                stolenVehicle.Enabled = false;

                                _submenuStolen.RemoveItemAt(index);

                                // Updates
                                RefreshMenuIndex(_submenuStolen, T.GetString("StolenVehicleItemEmptyDesc"));

                                if (OpenedFromiFruit)
                                {
                                    RefreshMenuIndex(_submenuBring, T.GetString("BringVehicleItemEmptyDesc"));
                                    RebuildMenuBring();
                                }
                            }
                            else
                            {
                                if (OpenedFromiFruit) MMISound.Play(MMISound.SoundFamily.NoMoney);
                                // Reemplazo final de UI.Notify por la notificación avanzada (4 parámetros)
                                Utils.ShowNotification("CHAR_CARSITE", "Mors Mutual", "", T.GetString("NotifyNoMoney"));
                            }
                        }
                    };
                }
            }
            else
            {
                UIMenuItem stolenVehicle = new UIMenuItem(T.GetString("Empty"), T.GetString("StolenVehicleItemEmptyDesc")) { Enabled = false };
                _submenuStolen.AddItem(stolenVehicle);
            }
        }


        /// <summary>
        /// Allow us to edit the license plate number of our vehicles.
        /// </summary>
        /// <param name="menu"></param>
        private void CreateMenuPlate(UIMenu menu)
        {
            _submenuPlate = _menuPool.AddSubMenu(menu, T.GetString("PlateChange"), T.GetString("PlateChangeDesc"));
            if (System.IO.File.Exists(Config.BannerImage)) _submenuPlate.SetBannerType(Config.BannerImage);
            RebuildMenuPlate();
        }
        private void RebuildMenuPlate()
        {
            int price = 1000;

            _submenuPlate.Clear();

            // 1. Obtenemos el ID del personaje de forma nativa mediante el Hash del modelo
            string currentCharacter = Game.Player.Character.Model.Hash.ToString();

            // 2. Llamada al InsuranceManager (Sin .Instance porque ya lo pusimos como static)
            List<string> vehicleList = InsuranceManager.GetInsuredVehicles(currentCharacter, false);
            vehicleList.AddRange(InsuranceManager.GetInsuredVehicles(currentCharacter, true));

            if (vehicleList.Count > 0)
            {
                foreach (string vehID in vehicleList)
                {
                    UIMenuItem changePlate = new UIMenuItem(InsuranceManager.Instance.GetVehicleFriendlyName(vehID, false));
                    changePlate.SetRightLabel(price.ToString() + "$");
                    _submenuPlate.AddItem(changePlate);

                    _submenuPlate.OnItemSelect += (sender, item, index) =>
                    {
                        if (item == changePlate)
                        {
                            if (OpenedFromiFruit) MMISound.Play(MMISound.SoundFamily.Okay);
                            string oldVehID = vehID;
                            string oldPlate = InsuranceManager.Instance.GetVehicleLicensePlate(vehID);
                            // En SHVDN v3, GetUserInput requiere el tipo de ventana, el título por defecto y la longitud
                            string newPlate = Game.GetUserInput(WindowTitle.EnterMessage60, oldPlate, 8);
                            newPlate = newPlate.PadRight(8);
                            newPlate = newPlate.ToUpperInvariant();

                            if (!string.IsNullOrEmpty(newPlate) && newPlate.Length <= 8)
                            {
                                if (newPlate != oldPlate && newPlate != "")
                                {
                                    if (Game.Player.Money >= price)
                                    {
                                        string newVehID = InsuranceManager.Instance.ChangeVehicleLicensePlate(vehID, newPlate);

                                        // Refresh item text
                                        item.Text = InsuranceManager.Instance.GetVehicleFriendlyName(newVehID, false);

                                        // Update in game vehicle
                                        for (int i = InsuranceObserver.InsuredVehList.Count - 1; i >= 0; i--)
                                        {
                                            if (Utils.GetVehicleIdentifier(InsuranceObserver.InsuredVehList[i]) == vehID)
                                            {
                                                // Update the plate on the in game's vehicles
                                                // En SHVDN v3, la propiedad correcta es LicensePlate dentro de Mods
                                                InsuranceObserver.InsuredVehList[i].Mods.LicensePlate = newPlate;

                                                // Remove the previous vehicle identifiers from the list
                                                InsuranceObserver.InsuredVehList.RemoveAt(i);
                                            }
                                        }

                                        // Update BlipsToRemove dictionnary
                                        if (InsuranceObserver.BlipsToRemove.ContainsKey(oldVehID))
                                        {
                                            Blip vehBlip = InsuranceObserver.BlipsToRemove[oldVehID];
                                            InsuranceObserver.BlipsToRemove.Remove(oldVehID);
                                            InsuranceObserver.BlipsToRemove.Add(newVehID, vehBlip);
                                        }

                                        // Combinamos las matrículas en el mensaje y usamos los 4 parámetros requeridos
                                        string plateMessage = T.GetString("NotifyPlateChanged") + "\n" + "[" + oldPlate + "] -> [" + newPlate + "]";

                                        Utils.ShowNotification("CHAR_CARSITE", "Mors Mutual", "", plateMessage);
                                        item.Enabled = false;

                                        // Updates
                                        // Need to wait for the vehicle to be detected by the InsuranceObserver's timer
                                        BigMessageThread.MessageInstance.ShowSimpleShard(T.GetString("PlateChangeUpdateDB"), T.GetString("PlateChangeUpdateDBDesc"), InsuranceObserver.Instance.DelayDetectInsuredVehicles + 1000);
                                        Script.Wait(InsuranceObserver.Instance.DelayDetectInsuredVehicles + 1000);

                                        RefreshItemInsure();

                                        RebuildMenuCancel();
                                        RebuildMenuRecover();
                                        RebuildMenuStolen();
                                        if (OpenedFromiFruit) RebuildMenuBring();
                                        RebuildMenuPlate();
                                    }
                                    else
                                    {
                                        if (OpenedFromiFruit) MMISound.Play(MMISound.SoundFamily.NoMoney);
                                        // Reemplazo final de UI.Notify por la notificación avanzada (4 parámetros)
                                        Utils.ShowNotification("CHAR_CARSITE", "Mors Mutual", "", T.GetString("NotifyNoMoney"));
                                    }
                                }
                            }
                            else
                            {
                                // Reemplazo de UI.Notify por la notificación de error de Mors Mutual
                                Utils.ShowNotification("CHAR_CARSITE", "Mors Mutual", "", T.GetString("NotifyWrongPlate"));

                            }
                        }
                    };
                }
            }
            else
            {
                UIMenuItem changePlate = new UIMenuItem(T.GetString("Empty"), T.GetString("PlateChangeItemEmptyDesc")) { Enabled = false };
                _submenuPlate.AddItem(changePlate);
            }
        }


        /// <summary>
        /// Bring the vehicle to the player
        /// </summary>
        /// <param name="menu"></param>
        private void CreateMenuBring(UIMenu menu)
        {
            _submenuBring = _menuPool.AddSubMenu(menu, T.GetString("BringVehicle"), T.GetString("BringVehicleDesc"));
            if (System.IO.File.Exists(Config.BannerImage)) _submenuBring.SetBannerType(Config.BannerImage);
            RebuildMenuBring();
        }
        private void RebuildMenuBring()
        {
            _submenuBring.Clear();

            if (InsuranceObserver.GetBringableVehicles().Count > 0)
            {
                foreach (Vehicle veh in InsuranceObserver.GetBringableVehicles())
                {
                    string vehID = Utils.GetVehicleIdentifier(veh);

                    // 1. Obtenemos el ID del personaje actual de forma nativa (v3)
                    string currentCharacter = Game.Player.Character.Model.Hash.ToString();

                    // 2. Comparamos con el dueño registrado (usando el método estático del Manager)
                    if (currentCharacter == InsuranceManager.GetVehicleOwner(vehID))
                    {
                        int cost = (int)((Game.Player.Character.Position.DistanceTo(veh.Position) / 1000) * Config.BringVehicleBasePrice);
                        UIMenuItem bringVehicle = new UIMenuItem(InsuranceManager.Instance.GetVehicleFriendlyName(vehID, false), T.GetString("BringVehicleDesc"));
                        bringVehicle.SetRightLabel(cost + "$");
                        _submenuBring.AddItem(bringVehicle);

                        _submenuBring.OnItemSelect += (sender, item, index) =>
                        {
                            if (item == bringVehicle)
                            {
                                if (Game.Player.Money >= cost)
                                {
                                    if (OpenedFromiFruit) MMISound.Play(MMISound.SoundFamily.Okay);
                                    InsuranceObserver.Instance.BringVehicleToPlayer(veh, cost, Config.BringVehicleInstant);
                                    bringVehicle.Enabled = false;
                                    // Reemplazo de UI.Notify por la notificación oficial de Mors Mutual
                                    Utils.ShowNotification("CHAR_CARSITE", "Mors Mutual", "", T.GetString("NotifyBringVehicle"));

                                    _submenuBring.RemoveItemAt(index);

                                    // Updates
                                    RefreshMenuIndex(_submenuBring, T.GetString("BringVehicleItemEmptyDesc"));
                                }
                                else
                                {
                                    if (OpenedFromiFruit) MMISound.Play(MMISound.SoundFamily.NoMoney);
                                    // Reemplazo final de UI.Notify para falta de fondos en "Bring Vehicle"
                                    Utils.ShowNotification("CHAR_CARSITE", "Mors Mutual", "", T.GetString("NotifyNoMoney"));
                                }
                            }
                        };
                    }
                }
            }
            else
            {
                UIMenuItem bringVehicle = new UIMenuItem(T.GetString("Empty"), T.GetString("BringVehicleItemEmptyDesc")) { Enabled = false };
                _submenuBring.AddItem(bringVehicle);
            }
        }
    }
}
