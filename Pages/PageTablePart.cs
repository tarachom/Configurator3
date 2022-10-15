using Gtk;

using AccountingSoftware;

namespace Configurator
{
    class PageTablePart : VBox
    {
        Configuration? Conf
        {
            get
            {
                return Program.Kernel?.Conf;
            }
        }

        public ConfigurationConstants ConfConstants { get; set; } = new ConfigurationConstants();
        public ConfigurationObjectTablePart TablePart { get; set; } = new ConfigurationObjectTablePart();
        public FormConfigurator? GeneralForm { get; set; }
        public bool IsNew { get; set; } = true;

        ListBox listBoxFields = new ListBox() { SelectionMode = SelectionMode.Multiple };
        Entry entryName = new Entry() { WidthRequest = 400 };
        TextView textViewDesc = new TextView();

        public PageTablePart() : base()
        {
            new VBox();
            HBox hBox = new HBox();

            Button bSave = new Button("Зберегти");
            //bSave.Clicked += OnSaveClick;

            hBox.PackStart(bSave, false, false, 10);

            Button bClose = new Button("Закрити");
            bClose.Clicked += (object? sender, EventArgs args) => { GeneralForm?.CloseCurrentPageNotebook(); };

            hBox.PackStart(bClose, false, false, 10);

            PackStart(hBox, false, false, 10);

            HPaned hPaned = new HPaned() { BorderWidth = 5 };

            CreatePack1(hPaned);
            CreatePack2(hPaned);

            PackStart(hPaned, false, false, 5);

            ShowAll();
        }

        void CreatePack2(HPaned hPaned)
        {
            VBox vBox = new VBox();

            HBox hBox = new HBox();
            hBox.PackStart(new Label("Поля:"), false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(Stock.Add) { Label = "Додати", IsImportant = true };
            //refreshButton.Clicked += OnRefreshClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonRefresh = new ToolButton(Stock.Refresh) { Label = "Обновити", IsImportant = true };
            //buttonRefresh.Clicked += OnTabularPartsRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(Stock.Delete) { Label = "Видалити", IsImportant = true };
            //buttonDelete.Clicked += OnTabularPartsRemoveClick;
            toolbar.Add(buttonDelete);

            HBox hBoxScroll = new HBox();
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 300);

            scrollList.Add(listBoxFields);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);
            hPaned.Pack2(vBox, true, false);
        }

        void CreatePack1(HPaned hPaned)
        {
            VBox vBox = new VBox();

            //Назва
            HBox hBoxName = new HBox();
            vBox.PackStart(hBoxName, false, false, 5);

            hBoxName.PackStart(new Label("Назва:"), false, false, 5);
            hBoxName.PackStart(entryName, false, false, 5);

            //Опис
            HBox hBoxDesc = new HBox();
            vBox.PackStart(hBoxDesc, false, false, 5);

            hBoxDesc.PackStart(new Label("Опис:") { Valign = Align.Start }, false, false, 5);

            ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollTextView.SetSizeRequest(400, 100);
            scrollTextView.Add(textViewDesc);

            hBoxDesc.PackStart(scrollTextView, false, false, 5);

            hPaned.Pack1(vBox, false, false);
        }

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            FillTabularParts();

            entryName.Text = TablePart.Name;
            textViewDesc.Buffer.Text = TablePart.Desc;
        }

        void FillTabularParts()
        {
            foreach (ConfigurationObjectField field in TablePart.Fields.Values)
                listBoxFields.Add(new Label(field.Name) { Name = field.Name, Halign = Align.Start });
        }

        void GetValue()
        {
            TablePart.Name = entryName.Text;
            TablePart.Desc = textViewDesc.Buffer.Text;
        }

        #endregion

    }
}