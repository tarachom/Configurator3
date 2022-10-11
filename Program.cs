using Gtk;
using AccountingSoftware;

namespace Configurator
{
    class Program
    {
        public static Kernel? Kernel { get; set; }

        public static void Main()
        {
            Application.Init();
            new FormConfigurationSelection();
            Application.Run();
        }
    }
}