using Gtk;

using AccountingSoftware;

namespace Configurator
{
    class PageQuery : VBox
    {
        Configuration? Conf
        {
            get
            {
                return Program.Kernel?.Conf;
            }
        }

        public ConfigurationObjectQueryBlock QueryBlock { get; set; } = new ConfigurationObjectQueryBlock();
        public string Key { get; set; } = "";
        public FormConfigurator? GeneralForm { get; set; }
        public System.Action? CallBack_RefreshList { get; set; }
        public bool IsNew { get; set; } = true;

        #region Fields

        Entry entryKey = new Entry() { WidthRequest = 800 };
        TextView textViewQuery = new TextView();

        #endregion

        public PageQuery() : base()
        {
            new VBox();
            HBox hBox = new HBox();

            Button bSave = new Button("Зберегти");
            bSave.Clicked += OnSaveClick;

            hBox.PackStart(bSave, false, false, 10);

            Button bClose = new Button("Закрити");
            bClose.Clicked += (object? sender, EventArgs args) => { GeneralForm?.CloseCurrentPageNotebook(); };

            hBox.PackStart(bClose, false, false, 10);

            PackStart(hBox, false, false, 10);

            HPaned hPaned = new HPaned() { BorderWidth = 5, Position = 500 };

            CreatePack1(hPaned);
            CreatePack2(hPaned);

            PackStart(hPaned, false, false, 5);

            ShowAll();
        }

        void CreatePack1(HPaned hPaned)
        {
            VBox vBox = new VBox();

            //Ключ
            HBox hBoxKey = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxKey, false, false, 5);

            hBoxKey.PackStart(new Label("Ключ:"), false, false, 5);
            hBoxKey.PackStart(entryKey, false, false, 5);

            //Query
            HBox hBoxQuery = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxQuery, false, false, 5);

            hBoxQuery.PackStart(new Label("Query:") { Valign = Align.Start }, false, false, 5);

            ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 800, HeightRequest = 500 };
            scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollTextView.Add(textViewQuery);

            hBoxQuery.PackStart(scrollTextView, false, false, 5);
            hPaned.Pack1(vBox, false, false);
        }

        void CreatePack2(HPaned hPaned)
        {
            VBox vBox = new VBox();

            Expander expanderHelp = new Expander("Довідка");
            expanderHelp.Add(vBox);

            HBox hBox = new HBox() { Halign = Align.Fill };
            vBox.PackStart(hBox, false, false, 5);

            hBox.PackStart(new Label("help"), false, false, 5);

            hPaned.Pack2(expanderHelp, false, false);
        }

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            if (!IsNew)
                textViewQuery.Buffer.Text = QueryBlock.Query[Key];

            entryKey.Text = Key;
        }

        void GetValue()
        {
            Key = entryKey.Text;
        }

        #endregion

        void OnSaveClick(object? sender, EventArgs args)
        {
            entryKey.Text = entryKey.Text.Trim();

            if (String.IsNullOrEmpty(entryKey.Text))
            {
                Message.Error(GeneralForm, $"Назва не задана");
                return;
            }

            if (IsNew)
            {
                if (QueryBlock.Query.ContainsKey(entryKey.Text))
                {
                    Message.Error(GeneralForm, $"Назва не унікальна");
                    return;
                }
            }
            else
            {
                if (Key != entryKey.Text)
                {
                    if (QueryBlock.Query.ContainsKey(entryKey.Text))
                    {
                        Message.Error(GeneralForm, $"Назва не унікальна");
                        return;
                    }
                }

                QueryBlock.Query.Remove(Key);
            }

            GetValue();

            QueryBlock.Query.Add(Key, textViewQuery.Buffer.Text);

            IsNew = false;

            GeneralForm?.RenameCurrentPageNotebook($"Query: {Key}");

            if (CallBack_RefreshList != null)
                CallBack_RefreshList.Invoke();
        }
    }
}