using Gtk;

namespace Configurator
{
    class Message
    {
        public static void ErrorMessage(string message)
        {
            MessageDialog md = new MessageDialog(null, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.Close, message);
            md.Run();
            md.Destroy();
        }
    }
}