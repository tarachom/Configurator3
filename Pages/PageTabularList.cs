using Gtk;

using AccountingSoftware;

namespace Configurator
{
    class PageTabularList : VBox
    {
        Configuration? Conf
        {
            get
            {
                return Program.Kernel?.Conf;
            }
        }

        public Dictionary<string, ConfigurationObjectField> Fields = new Dictionary<string, ConfigurationObjectField>();
        public Dictionary<string, ConfigurationTabularList> TabularLists { get; set; } = new Dictionary<string, ConfigurationTabularList>();
        public ConfigurationTabularList TabularList { get; set; } = new ConfigurationTabularList();
        public FormConfigurator? GeneralForm { get; set; }
        public System.Action? CallBack_RefreshList { get; set; }
        public bool IsNew { get; set; } = true;

        ListBox listBoxFields = new ListBox() { SelectionMode = SelectionMode.Single };
        Entry entryName = new Entry() { WidthRequest = 500 };
        TextView textViewDesc = new TextView();

        public PageTabularList() : base()
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
            HBox hBoxName = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxName, false, false, 5);

            hBoxName.PackStart(new Label("Назва:"), false, false, 5);
            hBoxName.PackStart(entryName, false, false, 5);

            //Опис
            HBox hBoxDesc = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxDesc, false, false, 5);

            hBoxDesc.PackStart(new Label("Опис:") { Valign = Align.Start }, false, false, 5);

            ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 500, HeightRequest = 100 };
            scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollTextView.Add(textViewDesc);

            hBoxDesc.PackStart(scrollTextView, false, false, 5);

            hPaned.Pack1(vBox, false, false);
        }

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            FillTabularParts();

            entryName.Text = TabularList.Name;
            textViewDesc.Buffer.Text = TabularList.Desc;
        }

        void FillTabularParts()
        {
            foreach (ConfigurationObjectField field in Fields.Values)
                listBoxFields.Add(
                    new CheckButton(field.Name)
                    {
                        Name = field.Name,
                        Active = TabularList.Fields.ContainsKey(field.Name)
                    });
        }

        void GetValue()
        {
            TabularList.Name = entryName.Text;
            TabularList.Desc = textViewDesc.Buffer.Text;

            //Доспупні поля
            TabularList.Fields.Clear();

            foreach (ListBoxRow item in listBoxFields.Children)
            {
                CheckButton cb = (CheckButton)item.Child;
                if (cb.Active)
                {
                    ConfigurationObjectField field = Fields[cb.Name];
                    TabularList.AppendField(new ConfigurationTabularListField(field.Name));
                }
            }
        }

        #endregion

        void OnSaveClick(object? sender, EventArgs args)
        {
            string name = entryName.Text;
            string errorList = Configuration.ValidateConfigurationObjectName(Program.Kernel!, ref name);
            entryName.Text = name;

            if (errorList.Length > 0)
            {
                Message.Error(GeneralForm, $"{errorList}");
                return;
            }

            if (IsNew)
            {
                if (TabularLists.ContainsKey(entryName.Text))
                {
                    Message.Error(GeneralForm, $"Назва табличного списку не унікальна");
                    return;
                }
            }
            else
            {
                if (TabularList.Name != entryName.Text)
                {
                    if (TabularLists.ContainsKey(entryName.Text))
                    {
                        Message.Error(GeneralForm, $"Назва табличного списку не унікальна");
                        return;
                    }
                }

                TabularLists.Remove(TabularList.Name);
            }

            GetValue();

            TabularLists.Add(TabularList.Name, TabularList);

            IsNew = false;

            //GeneralForm?.LoadTreeAsync();
            GeneralForm?.RenameCurrentPageNotebook($"Табличний список: {TabularList.Name}");

            if (CallBack_RefreshList != null)
                CallBack_RefreshList.Invoke();
        }
    }
}