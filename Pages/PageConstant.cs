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

            HPaned hPaned = new HPaned();
            hPaned.BorderWidth = 10;
            hPaned.Position = 200;

            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Never, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 300);

            ListBox listBox = new ListBox();
            listBox.SelectionMode = SelectionMode.Single;
            scrollList.Add(listBox);

            hPaned.Pack1(scrollList, true, true);

            VBox vBoxConstant = new VBox(false, 0);

            HBox hBoxConstant = new HBox(false, 0);
            vBoxConstant.PackStart(hBoxConstant, false, false, 0);

            Button bSave2 = new Button("Зберегти");
            hBoxConstant.PackStart(bSave2, false, false, 5);

            hPaned.Pack2(vBoxConstant, false, false);

            PackStart(hPaned, false, false, 5);

            ShowAll();
        }

        void OnSaveClick(object? sender, EventArgs args)
        {
            if (CallBack_ClosePage != null)
                CallBack_ClosePage.Invoke();
        }
    }
}