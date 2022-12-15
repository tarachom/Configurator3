using Gtk;

using AccountingSoftware;

namespace Configurator
{
    class PageHome : VBox
    {
        public PageHome() : base()
        {
            new VBox();
            HBox hBox = new HBox();

            PackStart(hBox, false, false, 10);

            ShowAll();
        }
    }
}