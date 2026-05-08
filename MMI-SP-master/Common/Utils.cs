using System;
using System.Text;

using GTA;
using GTA.Native;
using GTA.Math;

namespace MMI_SP.Common
{
    internal static class Utils
    {
        //internal static void ShowNotification(string picture, string title, string subtitle, string message)
        //{
        //    // Usamos los IDs directos del juego para que no dependa de si el nombre del Hash cambió
        //    Function.Call((Hash)0x20271A3007447974, "STRING"); // SET_NOTIFICATION_TEXT_ENTRY
        //    Function.Call((Hash)0x61F95635F640693A, message);    // ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME

        //    // SET_NOTIFICATION_MESSAGE
        //    Function.Call((Hash)0x1CCD9A3735907256, picture, picture, true, 4, title, subtitle);

        //    // DRAW_NOTIFICATION
        //    Function.Call((Hash)0x2ED7843F8F8FC622, false, true);
        //}

        internal static void ShowNotification(string picture, string title, string subtitle, string message)
        {
            // Solo el mensaje básico para asegurar que el mod funcione
            GTA.UI.Notification.Show(message);
        }

        internal static string ToHexString(string str)
        {
            var sb = new StringBuilder();

            var bytes = Encoding.Unicode.GetBytes(str);
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString(); // returns: "48656C6C6F20776F726C64" for "Hello world"
        }

        internal static string FromHexString(string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return Encoding.Unicode.GetString(bytes); // returns: "Hello world" for "48656C6C6F20776F726C64"
        }

        /// <summary>
        /// Return the unique identifier of the vehicle.
        /// </summary>
        /// <param name="veh"></param>
        /// <returns></returns>
        internal static string GetVehicleIdentifier(Vehicle veh)
        {
            string vehIdentifier = Game.Player.Name + veh.Model.Hash.ToString() + veh.Mods.LicensePlate;
            vehIdentifier = vehIdentifier.Replace(" ", "_");
            return vehIdentifier;
        }

        internal static EntityPosition GetVehicleSpawnLocation(Vector3 position)
        {
            for (int index = 0; index < 22; ++index)
            {
                OutputArgument outUnk = new OutputArgument();
                OutputArgument outPosition = new OutputArgument();
                OutputArgument outHeading = new OutputArgument();

                Function.Call(Hash.GET_NTH_CLOSEST_VEHICLE_NODE_WITH_HEADING, position.X, position.Y, position.Z, index, outPosition, outHeading, outUnk, 9, 3.0, 2.5);
                Vector3 newPos = outPosition.GetResult<Vector3>();
                float newHeading = outHeading.GetResult<float>();

                if (!Function.Call<bool>(Hash.IS_POINT_OBSCURED_BY_A_MISSION_ENTITY, newPos.X, newPos.Y, newPos.Z, 5.0f, 5.0f, 5.0f, 0))
                {
                    return new EntityPosition(newPos, newHeading);
                }
            }

            return new EntityPosition(position, 0f);
        }

    }
}