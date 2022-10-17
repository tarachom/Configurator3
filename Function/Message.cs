using Gtk;

namespace Configurator
{
    class Message
    {
        public static void Info(string message)
        {
            MessageDialog md = new MessageDialog(null, DialogFlags.DestroyWithParent, MessageType.Info, ButtonsType.Ok, message);
            md.WindowPosition = WindowPosition.Center;
            md.Run();
            md.Destroy();
        }

        public static void Error(string message)
        {
            MessageDialog md = new MessageDialog(null, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.Close, message);
            md.WindowPosition = WindowPosition.Center;
            md.Run();
            md.Destroy();
        }
    }
}