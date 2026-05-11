using GTA.UI;

namespace MMI_SP.Helpers
{
    internal static class Notification
    {
        public static void Show(string icon, string title, string message, string description)
        {
            string fullMessage = $"{icon} {title}: {message}";
            GTA.UI.Notification.PostTicker(fullMessage, isImportant: false);
        }
    }
}