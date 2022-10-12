using Gtk;

using AccountingSoftware;

namespace Configurator
{
    class PageConstant : VBox
    {
        Configuration? Conf
        {
            get
            {
                return Program.Kernel?.Conf;
            }
        }



        public PageConstant() : base()
        {
            new VBox(false, 0);
            BorderWidth = 0;

            

            HBox hBox = new HBox(false, 0);

            Button b = new Button("Закрити");
            hBox.PackStart(b, false, false, 0);

            PackStart(hBox, false, false, 0);

            ShowAll();
        }
    }
}