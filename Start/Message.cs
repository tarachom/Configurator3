using Gtk;

namespace Configurator
{
    class Message
    {
        public static void InfoMessage(string message)
        {
            MessageDialog md = new MessageDialog(null, DialogFlags.DestroyWithParent, MessageType.Info, ButtonsType.Ok, message);
            md.WindowPosition = WindowPosition.Center;
            md.Run();
            md.Destroy();
        }

        public static void ErrorMessage(string message)
        {
            MessageDialog md = new MessageDialog(null, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.Close, message);
            md.WindowPosition = WindowPosition.Center;
            md.Run();
            md.Destroy();
        }
    }
}