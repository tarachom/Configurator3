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
        public ConfigurationConstants? ConfConstants { get; set; }
        public System.Action? CallBack_ClosePage { get; set; }

        ListBox? listBoxTableParts;
        Entry? entryName;
        TextView? textViewDesc;

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
            hPaned.BorderWidth = 7;
            hPaned.Position = 200;

            CreatePack1(hPaned);
            CreatePack2(hPaned);

            PackStart(hPaned, false, false, 5);

            ShowAll();
        }

        public void SetValue()
        {
            foreach (ConfigurationObjectTablePart tablePart in ConfConstants!.TabularParts.Values)
                listBoxTableParts?.Add(new Label(tablePart.Name) { Halign = Align.Start });

            entryName!.Text = ConfConstants?.Name;
            textViewDesc!.Buffer.Text = ConfConstants?.Desc;
        }

        void CreatePack1(HPaned hPaned)
        {
            VBox vBox = new VBox(false, 0);
            vBox.PackStart(new Label("Табличні частини:") { Halign = Align.Start }, false, false, 5);

            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 300);

            listBoxTableParts = new ListBox();
            listBoxTableParts.SelectionMode = SelectionMode.Single;
            scrollList.Add(listBoxTableParts);

            vBox.PackStart(scrollList, false, false, 0);
            hPaned.Pack1(vBox, true, true);
        }

        void CreatePack2(HPaned hPaned)
        {
            VBox vBox = new VBox(false, 0);

            Fixed fixName = new Fixed();
            vBox.PackStart(fixName, false, false, 5);

            fixName.Put(new Label("Назва:"), 5, 5);
            fixName.Put(entryName = new Entry() { WidthRequest = 300 }, 60, 0);

            Fixed fixDesc = new Fixed();
            vBox.PackStart(fixDesc, false, false, 5);

            fixDesc.Put(new Label("Опис:"), 5, 5);
            fixDesc.Put(textViewDesc = new TextView(), 60, 5);
            textViewDesc.SetSizeRequest(300, 50);
            hPaned.Pack2(vBox, false, false);
        }

        void OnSaveClick(object? sender, EventArgs args)
        {
            if (ConfConstants != null && entryName != null)
            {
                ConfConstants.Name = entryName.Text;
                ConfConstants.Desc = textViewDesc?.Buffer.Text ?? "";
            }
        }
    }
}