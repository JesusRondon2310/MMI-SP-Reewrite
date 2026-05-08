using GTA;
using GTA.Native;
using LemonUI;
using LemonUI.Elements;
using LemonUI.Menus;
using MMI_SP.Common;
using MMI_SP.iFruit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

namespace MMI_SP
{
    class MenuMMI
    {
        private ObjectPool _menuPool;
        internal void MenuPoolProcessMenus() { _menuPool.Process(); }

        private NativeMenu _mainMenu = new NativeMenu("Mors Mutual", "Menú de seguros");
        internal NativeMenu Mainmenu { get => _mainMenu; }

        public bool OpenedFromiFruit { get => _openedFromiFruit; private set => _openedFromiFruit = value; }
        private bool _openedFromiFruit = false;

        // --- Submenús y opciones del menú principal --- \\
        // Botón "Asegurar vehículo"
        NativeItem _itemInsure;

        // Submenús (cada uno contiene opciones específicas)
        NativeMenu _submenuRecover;   // Recuperar vehículos destruidos
        NativeMenu _submenuCancel;    // Cancelar un seguro
        NativeMenu _submenuPlate;     // Cambiar matrícula
        NativeMenu _submenuBring;     // Traer vehículo al jugador

        // Constructor: crea el gestor de menús y registra el menú principal
        public MenuMMI()
        {
            _menuPool = new ObjectPool();
            _menuPool.Add(_mainMenu);
        }

        // Muestra el menú principal
        internal void Show()
        {
            _mainMenu.Visible = true;
        }
        // --- Submenús y opciones del menú principal --- \\

        /// <summary>
        /// Creates the menu
        /// </summary>
        /// <summary>
        /// Construye el menú principal y sus submenús según el contexto (teléfono u oficina).
        /// </summary>
        internal void Create()
        {
            // Banner desactivado (se deja comentado)
            // if (System.IO.File.Exists(Config.BannerImage)) _mainMenu.SetBannerType(Config.BannerImage);

            if (OpenedFromiFruit) {
                // Abierto desde el teléfono: solo las opciones habilitadas en configuración
                if (Config.CaniFruitInsure) BuildItemInsure();
                if (Config.CaniFruitCancel) CreateMenuCancel(_mainMenu);
                if (Config.CaniFruitRecover) CreateMenuRecover(_mainMenu);
                if (Config.CaniFruitPlate) CreateMenuPlate(_mainMenu);
                //CreateMenuBring(_mainMenu);
            } else {
                // Abierto desde la oficina: todas las opciones excepto traer vehículo
                BuildItemInsure();
                CreateMenuCancel(_mainMenu);
                CreateMenuRecover(_mainMenu);
                CreateMenuPlate(_mainMenu);
            }
        }

        /// <summary>
        /// Elimina todo el contenido del menú y lo reconstruye.
        /// Permite refrescar el menú dinámicamente (por ejemplo, al cambiar de vehículo).
        /// </summary>
        internal void Reset(bool iFruit = false)
        {
            OpenedFromiFruit = iFruit;
            if (_mainMenu != null)
                _mainMenu.Clear();   // Con LemonUI será _mainMenu.Clear()
            Create();
        }

        // Ajusta un submenú para que nunca aparezca vacío y tenga la selección en una posición válida.
        // Si no hay opciones, muestra un ítem deshabilitado.
        /// <param name="menu"></param>
        private void RefreshMenuIndex(NativeMenu menu, string itemDescription)
        {
            if (menu != null)
            {
                if (menu.Items.Count <= 0)
                {
                    NativeItem emptyItem = new NativeItem("Vacío", itemDescription);
                    emptyItem.Enabled = false;
                    menu.Add(emptyItem);

                    menu.SelectedIndex = 0;   // Selecciona el primer item
                } else {
                    if (menu.SelectedIndex > menu.Items.Count - 1)
                        menu.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// Construye el botón "Asegurar vehículo" según el estado del último vehículo usado.
        /// Si no es asegurable o ya está asegurado, el botón aparece deshabilitado.
        /// </summary>
        private void BuildItemInsure()
        {
            Vehicle veh = Game.Player.LastVehicle;
            if (veh == null || !veh.Exists()) return;

            // Limpia el botón anterior si ya existía
            if (_itemInsure != null)
            {
                _itemInsure.Activated -= OnInsureActivated;
            }

            int cost = InsuranceManager.GetVehicleInsuranceCost(veh);
            string vehName = Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, veh.Model.Hash);

            if (!InsuranceManager.IsVehicleInsured(Utils.GetVehicleIdentifier(veh)))
            {
                if (InsuranceManager.IsVehicleInsurable(veh))
                {
                    _itemInsure = new NativeItem("Asegurar vehículo", $"Coste: {cost}$\n{vehName}");
                    _itemInsure.Activated += OnInsureActivated;
                } else {
                    _itemInsure = new NativeItem("Asegurar vehículo", $"No se puede asegurar este vehículo\n{vehName}");
                    _itemInsure.Enabled = false;
                }
            } else {
                _itemInsure = new NativeItem("Asegurar vehículo", $"Este vehículo ya está asegurado\n{vehName}");
                _itemInsure.Enabled = false;
            }

            _mainMenu.Add(_itemInsure);
        }
        /// <summary>
        /// Construye el botón "Asegurar vehículo" según el estado del último vehículo usado.
        /// Si no es asegurable o ya está asegurado, el botón aparece deshabilitado.
        /// </summary>

        // Manejador extraído para poder desuscribirlo correctamente
        private void OnInsureActivated(object sender, EventArgs e)
        {
            Vehicle lastVeh = Game.Player.LastVehicle;
            if (lastVeh == null || !lastVeh.Exists()) return;

            string vehID = Utils.GetVehicleIdentifier(lastVeh);
            if (InsuranceManager.IsVehicleInsured(vehID)) return;
            if (!InsuranceManager.IsVehicleInsurable(lastVeh)) return;

            int cost = InsuranceManager.GetVehicleInsuranceCost(lastVeh);

            if (Game.Player.Money >= cost)
            {
                Game.Player.Money -= cost;
                InsureVehicle(lastVeh);
            } else {
                if (OpenedFromiFruit) MMISound.Play(MMISound.SoundFamily.NoMoney);
                Utils.ShowNotification("CHAR_CARSITE", "Mors Mutual", "No tienes suficiente dinero", "");
            }
        }


        /// <summary>
        /// Ejecuta el aseguramiento del vehículo: notifica al InsuranceManager,
        /// reproduce sonido, deshabilita el botón Asegurar y refresca los submenús afectados.
        /// </summary>
        private void InsureVehicle(Vehicle veh)
        {
            if (OpenedFromiFruit) MMISound.Play(MMISound.SoundFamily.Okay);
            InsuranceManager.Instance.InsureVehicle(veh);
            Utils.ShowNotification("CHAR_CARSITE", "Mors Mutual", "Vehículo asegurado correctamente.", "");

            _itemInsure.Enabled = false;

            // Refresca submenú Cancelar
            RefreshMenuIndex(_submenuCancel, "No tienes vehículos asegurados.");
            RebuildMenuCancel();

            // Si estamos en el teléfono, refresca el menú de traer vehículo
            if (OpenedFromiFruit)
            {
                RefreshMenuIndex(_submenuBring, "No tienes vehículos asegurados.");
                //RebuildMenuBring();
            }

            // Refresca submenú Cambiar matrícula
            RefreshMenuIndex(_submenuPlate, "No tienes vehículos asegurados.");
            RebuildMenuPlate();
        }


        /// <summary>
        /// Crea un submenú estándar de MMI, le asigna el banner (si existe) y añade un ítem en el menú padre.
        /// Devuelve el submenú creado para que el método que la llama pueda seguir usándolo.
        /// </summary>
        private NativeMenu CreateStandardSubMenu(NativeMenu parent, string title, string subtitle,
                                                  string itemText, string itemDescription, Action rebuildAction)
        {
            // 1. Crear submenú
            NativeMenu submenu = new NativeMenu(title, subtitle);
            _menuPool.Add(submenu);

            // Banner con texto y color (reemplaza la imagen)
            submenu.BannerText = new ScaledText(new PointF(0, 0), "~g~Mors Mutual Insurance");

            // 3. Ítem en el menú padre que abre el submenú
            NativeItem item = new NativeItem(itemText, itemDescription);
            item.Activated += (sender, e) => submenu.Visible = true;
            parent.Add(item);

            // 4. Construir el contenido del submenú (rebuild)
            rebuildAction?.Invoke();

            return submenu;
        }


        /// <summary>
        /// Cancel a contract by removing the vehicle from the database.
        /// </summary>
        /// <param name="menu"></param>
        private void CreateMenuCancel(NativeMenu menu)
        {
            _submenuCancel = CreateStandardSubMenu(menu,
                "Cancelar seguro", "Selecciona el vehículo a cancelar",
                "Cancelar seguro", "Elimina un vehículo de tu póliza",
                RebuildMenuCancel);
        }

        /// <summary>
        /// Reconstruye el submenú de cancelación con los vehículos asegurados del personaje.
        /// Si no hay vehículos, muestra un ítem deshabilitado.
        /// </summary>
        private void RebuildMenuCancel()
        {

            _submenuCancel.Clear();

            string currentCharacter = Game.Player.Character.Model.Hash.ToString();
            List<string> vehicleList = InsuranceManager.GetInsuredVehicles(currentCharacter, false);
            vehicleList.AddRange(InsuranceManager.GetInsuredVehicles(currentCharacter, true));

            if (vehicleList.Count > 0)
            {
                foreach (string vehID in vehicleList)
                {
                    string modelName = InsuranceManager.Instance.GetVehicleModelName(vehID);
                    string plate = InsuranceManager.Instance.GetVehicleLicensePlate(vehID);

                    NativeItem cancelItem = new NativeItem(modelName, $"Matrícula: {plate}");
                    _submenuCancel.Add(cancelItem);

                    cancelItem.Activated += (sender, e) => {
                        if (OpenedFromiFruit) MMISound.Play(MMISound.SoundFamily.Okay);
                        InsuranceManager.Instance.CancelVehicle(vehID);
                        Utils.ShowNotification("CHAR_CARSITE", "Mors Mutual", "Seguro cancelado correctamente.", "");

                        // Refresca todos los submenús afectados
                        RebuildMenuCancel();
                        BuildItemInsure(); // Refresca el botón de asegurar
                        RebuildMenuRecover();
                        RebuildMenuPlate();

                        //if (OpenedFromiFruit) RebuildMenuBring();
                    };
                }
            } else {
                NativeItem emptyItem = new NativeItem("Vacío", "No tienes vehículos asegurados.");
                emptyItem.Enabled = false;
                _submenuCancel.Add(emptyItem);
            }
        }

        /// <summary>
        /// Crea el submenú "Recuperar vehículo" y lo vincula al menú principal.
        /// </summary>
        private void CreateMenuRecover(NativeMenu menu)
        {
            _submenuRecover = CreateStandardSubMenu(menu,
                "Recuperar vehículo", "Selecciona un vehículo destruido para recuperarlo",
                "Recuperar vehículo", "Recupera un vehículo destruido de tu póliza",
                RebuildMenuRecover);
        }

        /// <summary>
        /// Reconstruye la lista de vehículos destruidos que pueden ser recuperados.
        /// Si no hay vehículos, muestra un ítem deshabilitado.
        /// </summary>
        private void RebuildMenuRecover()
        {
            _submenuRecover.Clear();
            string currentCharacter = Game.Player.Character.Model.Hash.ToString();
            List<string> deadVehicleList = InsuranceManager.GetInsuredVehicles(currentCharacter, true);

            // Guard clause: sin vehículos destruidos
            if (deadVehicleList.Count == 0)
            {
                NativeItem emptyItem = new NativeItem("Vacío", "No tienes vehículos destruidos que recuperar.");
                emptyItem.Enabled = false;
                _submenuRecover.Add(emptyItem);
                return;
            }

            foreach (string vehID in deadVehicleList)
            {
                int cost = InsuranceManager.Instance.GetVehicleInsuranceCost(vehID, InsuranceManager.Multiplier.Recover);
                string vehicleName = InsuranceManager.Instance.GetVehicleFriendlyName(vehID, false);

                NativeItem recoverItem = new NativeItem(vehicleName, $"Coste de recuperación: {cost}$");
                _submenuRecover.Add(recoverItem);

                recoverItem.Activated += (sender, e) =>
                {
                    // Guard clause: sin dinero suficiente
                    if (Game.Player.Money < cost)
                    {
                        if (OpenedFromiFruit) MMISound.Play(MMISound.SoundFamily.NoMoney);
                        Utils.ShowNotification("CHAR_CARSITE", "Mors Mutual", "No tienes suficiente dinero.", "");
                        return;
                    }

                    // Recuperación exitosa
                    if (OpenedFromiFruit) MMISound.Play(MMISound.SoundFamily.Okay);
                    InsuranceManager.Instance.RecoverVehicle(vehID);
                    Utils.ShowNotification("CHAR_CARSITE", "Mors Mutual", "Vehículo recuperado correctamente.", "");

                    RebuildMenuRecover();
                    //if (OpenedFromiFruit) RebuildMenuBring();
                };
            }
        }

        /// <summary>
        /// Crea el submenú "Cambiar matrícula" y lo vincula al menú principal.
        /// </summary>
        /// <param name="menu"></param>
        private void CreateMenuPlate(NativeMenu menu)
        {
            _submenuPlate = CreateStandardSubMenu(menu,
                "Cambiar matrícula", "Edita la placa de tus vehículos asegurados",
                "Cambiar matrícula", "Modifica la matrícula de un vehículo asegurado",
                RebuildMenuPlate);
        }

        /// <summary>
        /// Reconstruye el submenú "Cambiar matrícula" con los vehículos asegurados.
        /// Al seleccionar un vehículo se pide una nueva placa, se valida y se actualiza.
        /// </summary>
        private void RebuildMenuPlate()
        {
            _submenuPlate.Clear();
            int price = 1000;

            // Obtenemos el hash del personaje actual
            string currentCharacter = Game.Player.Character.Model.Hash.ToString();
            List<string> vehicleList = InsuranceManager.GetInsuredVehicles(currentCharacter, false);
            vehicleList.AddRange(InsuranceManager.GetInsuredVehicles(currentCharacter, true));

            // Guard clause: sin vehículos asegurados
            if (vehicleList.Count == 0)
            {
                NativeItem emptyItem = new NativeItem("Vacío", "No tienes vehículos asegurados.");
                emptyItem.Enabled = false;
                _submenuPlate.Add(emptyItem);
                return;
            }

            foreach (string vehID in vehicleList)
            {
                string vehicleName = InsuranceManager.Instance.GetVehicleFriendlyName(vehID, false);
                NativeItem plateItem = new NativeItem(vehicleName, $"Coste: {price}$");
                _submenuPlate.Add(plateItem);

                plateItem.Activated += (sender, e) =>
                {
                    // Guard clause: sin dinero suficiente
                    if (Game.Player.Money < price)
                    {
                        if (OpenedFromiFruit) MMISound.Play(MMISound.SoundFamily.NoMoney);
                        Utils.ShowNotification("CHAR_CARSITE", "Mors Mutual", "No tienes suficiente dinero.", "");
                        return;
                    }

                    ChangePlateAction(vehID, price, plateItem);
                };
            }
        }

        /// <summary>
        /// Ejecuta el cambio de matrícula del vehículo con el identificador dado.
        /// </summary>
        private void ChangePlateAction(string vehID, int price, NativeItem item)
        {
            if (OpenedFromiFruit) MMISound.Play(MMISound.SoundFamily.Okay);

            string oldPlate = InsuranceManager.Instance.GetVehicleLicensePlate(vehID);

            // Pedir la nueva matrícula
            string newPlate = Game.GetUserInput(WindowTitle.EnterMessage60, oldPlate, 8);
            if (string.IsNullOrEmpty(newPlate)) return;

            newPlate = newPlate.Trim().ToUpperInvariant();
            newPlate = newPlate.PadRight(8); // La nativa de GetUserInput puede devolver menos de 8

            // Validaciones
            if (string.IsNullOrWhiteSpace(newPlate) || newPlate.Length > 8)
            {
                Utils.ShowNotification("CHAR_CARSITE", "Mors Mutual", "Matrícula inválida.", "");
                return;
            }

            if (newPlate == oldPlate)
            {
                // No se cambió nada, se cierra el diálogo sin hacer nada
                return;
            }

            // Cobrar y cambiar la placa en la base de datos
            Game.Player.Money -= price;
            string newVehID = InsuranceManager.Instance.ChangeVehicleLicensePlate(vehID, newPlate);

            // Actualizar el nombre mostrado en el ítem
            item.Title = InsuranceManager.Instance.GetVehicleFriendlyName(newVehID, false);

            // Actualizar los vehículos en el mundo que tengan el identificador antiguo
            for (int i = InsuranceObserver.InsuredVehList.Count - 1; i >= 0; i--)
            {
                if (Utils.GetVehicleIdentifier(InsuranceObserver.InsuredVehList[i]) == vehID)
                {
                    InsuranceObserver.InsuredVehList[i].Mods.LicensePlate = newPlate;
                    InsuranceObserver.InsuredVehList.RemoveAt(i);
                }
            }

            // Actualizar el diccionario de blips
            if (InsuranceObserver.BlipsToRemove.TryGetValue(vehID, out Blip vehBlip))
            {
                InsuranceObserver.BlipsToRemove.Remove(vehID);
                InsuranceObserver.BlipsToRemove.Add(newVehID, vehBlip);
            }

            // Notificar al jugador
            Utils.ShowNotification("CHAR_CARSITE", "Mors Mutual",
                $"Placa cambiada: {oldPlate} → {newPlate}", "");

            // Refrescar todos los menús afectados (sin espera, ya que la DB está actualizada)
            BuildItemInsure(); // Refresca el botón de asegurar
            RebuildMenuCancel();
            RebuildMenuRecover();
            //if (OpenedFromiFruit) RebuildMenuBring();
            RebuildMenuPlate();
        }


        /*/// <summary>
        /// Bring the vehicle to the player
        /// </summary>
        /// <param name="menu"></param>
        private void CreateMenuBring(NativeMenu menu)
        {
            _submenuBring = CreateStandardSubMenu(menu,
                "Traer mi vehículo", "Un conductor traerá el vehículo hasta ti",
                "Traer mi vehículo", "Solicita que te acerquen un vehículo asegurado",
                RebuildMenuBring);
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
        }*/
    }
}
