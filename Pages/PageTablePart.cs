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

        ListBox listBoxFields = new ListBox() { SelectionMode = SelectionMode.Single };
        Entry entryName = new Entry() { WidthRequest = 400 };
        TextView textViewDesc = new TextView();

        public PageTablePart() : base()
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

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(Stock.Add) { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnTabularPartsAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonRefresh = new ToolButton(Stock.Refresh) { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnTabularPartsRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(Stock.Delete) { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnTabularPartsRemoveClick;
            toolbar.Add(buttonDelete);

            HBox hBoxScroll = new HBox();
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 300);

            listBoxFields.ButtonPressEvent += OnTabularPartsButtonPress;

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

        void OnSaveClick(object? sender, EventArgs args)
        {
            string name = entryName.Text;
            string errorList = Configuration.ValidateConfigurationObjectName(Program.Kernel!, ref name);
            entryName.Text = name;

            if (errorList.Length > 0)
            {
                Message.ErrorMessage($"{errorList}");
                return;
            }

            if (IsNew)
            {
                if (ConfConstants.TabularParts.ContainsKey(entryName.Text))
                {
                    Message.ErrorMessage($"Назва табличної частини не унікальна");
                    return;
                }
            }
            else
            {
                if (TablePart.Name != entryName.Text)
                {
                    if (ConfConstants.TabularParts.ContainsKey(entryName.Text))
                    {
                        Message.ErrorMessage($"Назва табличної частини не унікальна");
                        return;
                    }
                }

                ConfConstants.TabularParts.Remove(TablePart.Name);
            }

            GetValue();

            ConfConstants.AppendTablePart(TablePart);

            IsNew = false;

            GeneralForm?.LoadTree();
            GeneralForm?.RenameCurrentPageNotebook($"Таблична частина: {TablePart.Name}");
        }

        void OnTabularPartsButtonPress(object? sender, ButtonPressEventArgs args)
        {
            if (args.Event.Type == Gdk.EventType.DoubleButtonPress)
            {
                ListBoxRow[] selectedRows = listBoxFields.SelectedRows;

                if (selectedRows.Length != 0)
                {
                    ListBoxRow curRow = selectedRows[0];

                    if (TablePart.Fields.ContainsKey(curRow.Child.Name))
                        GeneralForm?.CreateNotebookPage($"Поле: {curRow.Child.Name}", () =>
                        {
                            PageField page = new PageField()
                            {
                                Fields = TablePart.Fields,
                                Field = TablePart.Fields[curRow.Child.Name],
                                IsNew = false,
                                GeneralForm = GeneralForm
                            };

                            page.SetValue();

                            return page;
                        });
                }
            }
        }

        void OnTabularPartsAddClick(object? sender, EventArgs args)
        {
            GeneralForm?.CreateNotebookPage("Поле *", () =>
            {
                PageField page = new PageField()
                {
                    Fields = TablePart.Fields,
                    IsNew = true,
                    GeneralForm = GeneralForm
                };

                page.SetDefValue();

                return page;
            });
        }

        void OnTabularPartsRefreshClick(object? sender, EventArgs args)
        {
            foreach (Widget item in listBoxFields.Children)
                listBoxFields.Remove(item);

            FillTabularParts();

            listBoxFields.ShowAll();
        }

        void OnTabularPartsRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxFields.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (TablePart.Fields.ContainsKey(row.Child.Name))
                        TablePart.Fields.Remove(row.Child.Name);
                }

                OnTabularPartsRefreshClick(null, new EventArgs());

                GeneralForm?.LoadTree();
            }
        }
    }
}