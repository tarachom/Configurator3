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

        public System.Action? CallBack_ClosePage { get; set; }

        public PageConstant() : base()
        {
            new VBox(false, 0);
            BorderWidth = 0;

            HBox hBox = new HBox(false, 0);

            Button bSave = new Button("Зберегти");
            bSave.Clicked += OnSaveClick;

            hBox.PackStart(bSave, false, false, 10);

            Button bClose = new Button("Закрити");
            bClose.Clicked += (object? sender, EventArgs args) => { if (CallBack_ClosePage != null) CallBack_ClosePage.Invoke(); };

            hBox.PackStart(bClose, false, false, 10);

            PackStart(hBox, false, false, 10);

            Separator hSeparator = new Separator(Orientation.Horizontal);
            PackStart(hSeparator, false, false, 2);



            ShowAll();
        }

        void OnSaveClick(object? sender, EventArgs args)
        {
            if (CallBack_ClosePage != null)
                CallBack_ClosePage.Invoke();
        }
    }
}