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

            // Dialog d = new Dialog("test", null, DialogFlags.Modal, false);
            // d.SetSizeRequest(100, 100);
            // d.SetPosition(WindowPosition.CenterAlways);
            // d.Show();

            //InfoBar ib = new InfoBar();
            //ib.AddActionWidget(new Label("Text"), 0);
            //ib.AddButton("Ok", 0);
            //ib.ShowCloseButton = true;
            //ib.MessageType = MessageType.Error;

            //Calendar cl = new Calendar();


            //hBox.PackStart(ib, false, false, 0);

            //hBox.PackStart(cl, false, false, 0);



            Button bClose = new Button("Закрити");
            bClose.Clicked += (object? sender, EventArgs args) => { };

            hBox.PackStart(bClose, false, false, 10);

            PackStart(hBox, false, false, 10);

            ShowAll();
        }
    }
}