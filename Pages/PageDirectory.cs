using Gtk;

using AccountingSoftware;

namespace Configurator
{
    class PageDirectory : VBox
    {
        Configuration? Conf
        {
            get
            {
                return Program.Kernel?.Conf;
            }
        }

        public ConfigurationDirectories ConfDirectory { get; set; } = new ConfigurationDirectories();
        public FormConfigurator? GeneralForm { get; set; }
        public bool IsNew { get; set; } = true;

        ListBox listBoxFields = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxTableParts = new ListBox() { SelectionMode = SelectionMode.Single };
        Entry entryName = new Entry() { WidthRequest = 500 };
        Entry entryTable = new Entry() { WidthRequest = 500 };
        Entry entryBeforeSave = new Entry() { WidthRequest = 500 };
        Entry entryAfterSave = new Entry() { WidthRequest = 500 };
        Entry entryBeforeDelete = new Entry() { WidthRequest = 500 };
        TextView textViewDesc = new TextView();

        public PageDirectory() : base()
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

        void CreatePack1(HPaned hPaned)
        {
            VBox vBox = new VBox();

            //Назва
            HBox hBoxName = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxName, false, false, 5);

            hBoxName.PackStart(new Label("Назва:"), false, false, 5);
            hBoxName.PackStart(entryName, false, false, 5);

            //Таблиця
            HBox hBoxTable = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxTable, false, false, 5);

            hBoxTable.PackStart(new Label("Таблиця:"), false, false, 5);
            hBoxTable.PackStart(entryTable, false, false, 5);

            //Опис
            HBox hBoxDesc = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxDesc, false, false, 5);

            hBoxDesc.PackStart(new Label("Опис:") { Valign = Align.Start }, false, false, 5);

            ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 500, HeightRequest = 100 };
            scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollTextView.Add(textViewDesc);

            hBoxDesc.PackStart(scrollTextView, false, false, 5);

            //Заголовок блоку Тригери
            HBox hBoxTrigerInfo = new HBox() { Halign = Align.Center };
            vBox.PackStart(hBoxTrigerInfo, false, false, 5);
            hBoxTrigerInfo.PackStart(new Label("Тригери"), false, false, 5);

            //Перед записом
            HBox hBoxTrigerBeforeSave = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxTrigerBeforeSave, false, false, 5);

            hBoxTrigerBeforeSave.PackStart(new Label("Перед записом:"), false, false, 5);
            hBoxTrigerBeforeSave.PackStart(entryBeforeSave, false, false, 5);

            //Після запису
            HBox hBoxTrigerAfterSave = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxTrigerAfterSave, false, false, 5);

            hBoxTrigerAfterSave.PackStart(new Label("Після запису:"), false, false, 5);
            hBoxTrigerAfterSave.PackStart(entryAfterSave, false, false, 5);

            //Перед видаленням
            HBox hBoxTrigerBeforeDelete = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxTrigerBeforeDelete, false, false, 5);

            hBoxTrigerBeforeDelete.PackStart(new Label("Перед видален.:"), false, false, 5);
            hBoxTrigerBeforeDelete.PackStart(entryBeforeDelete, false, false, 5);

            hPaned.Pack1(vBox, false, false);
        }

        void CreatePack2(HPaned hPaned)
        {
            VBox vBox = new VBox();

            //Поля
            CreateFieldList(vBox);

            //Табличні частини
            CreateTablePartList(vBox);

            hPaned.Pack2(vBox, true, false);
        }

        void CreateFieldList(VBox vBoxContainer)
        {
            VBox vBox = new VBox();

            HBox hBox = new HBox();
            hBox.PackStart(new Label("Поля:"), false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(Stock.Add) { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnFieldsAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(Stock.Copy) { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnFieldsCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(Stock.Refresh) { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnFieldsRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(Stock.Delete) { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnFieldsRemoveClick;
            toolbar.Add(buttonDelete);

            HBox hBoxScroll = new HBox();
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 400);

            listBoxFields.ButtonPressEvent += OnFieldsButtonPress;

            scrollList.Add(listBoxFields);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            vBoxContainer.PackStart(vBox, false, false, 0);
        }

        void CreateTablePartList(VBox vBoxContainer)
        {
            VBox vBox = new VBox();

            HBox hBox = new HBox();
            hBox.PackStart(new Label("Табличні частини:"), false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(Stock.Add) { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnTabularPartsAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(Stock.Copy) { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnTabularPartsCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(Stock.Refresh) { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnTabularPartsRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(Stock.Delete) { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnTabularPartsRemoveClick;
            toolbar.Add(buttonDelete);

            HBox hBoxScroll = new HBox();
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 200);

            listBoxTableParts.ButtonPressEvent += OnTabularPartsButtonPress;

            scrollList.Add(listBoxTableParts);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            vBoxContainer.PackStart(vBox, false, false, 0);
        }

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            entryName.Text = ConfDirectory.Name;

            if (IsNew)
            {
                entryTable.Text = Configuration.GetNewUnigueTableName(Program.Kernel!);

                string nameInTable_Code = Configuration.GetNewUnigueColumnName(Program.Kernel!, entryTable.Text, ConfDirectory.Fields);
                ConfDirectory.AppendField(new ConfigurationObjectField("Код", nameInTable_Code, "string", "", "Код"));

                string nameInTable_Name = Configuration.GetNewUnigueColumnName(Program.Kernel!, entryTable.Text, ConfDirectory.Fields);
                ConfDirectory.AppendField(new ConfigurationObjectField("Назва", nameInTable_Name, "string", "", "Назва"));
            }
            else
                entryTable.Text = ConfDirectory.Table;

            textViewDesc.Buffer.Text = ConfDirectory.Desc;

            entryBeforeSave.Text = ConfDirectory.TriggerFunctions.BeforeSave;
            entryAfterSave.Text = ConfDirectory.TriggerFunctions.AfterSave;
            entryBeforeDelete.Text = ConfDirectory.TriggerFunctions.BeforeDelete;

            FillFields();
            FillTabularParts();
        }

        void FillFields()
        {
            foreach (ConfigurationObjectField field in ConfDirectory.Fields.Values)
                listBoxFields.Add(new Label(field.Name) { Name = field.Name, Halign = Align.Start });
        }

        void FillTabularParts()
        {
            foreach (ConfigurationObjectTablePart tablePart in ConfDirectory.TabularParts.Values)
                listBoxTableParts.Add(new Label(tablePart.Name) { Name = tablePart.Name, Halign = Align.Start });
        }

        void GetValue()
        {
            ConfDirectory.Name = entryName.Text;
            ConfDirectory.Table = entryTable.Text;
            ConfDirectory.Desc = textViewDesc.Buffer.Text;

            ConfDirectory.TriggerFunctions.BeforeSave = entryBeforeSave.Text;
            ConfDirectory.TriggerFunctions.AfterSave = entryAfterSave.Text;
            ConfDirectory.TriggerFunctions.BeforeDelete = entryBeforeDelete.Text;
        }

        #endregion

        void OnSaveClick(object? sender, EventArgs args)
        {
            string name = entryName.Text;
            string errorList = Configuration.ValidateConfigurationObjectName(Program.Kernel!, ref name);
            entryName.Text = name;

            if (errorList.Length > 0)
            {
                Message.Error($"{errorList}");
                return;
            }

            if (IsNew)
            {
                if (Conf!.Directories.ContainsKey(entryName.Text))
                {
                    Message.Error($"Назва довідника не унікальна");
                    return;
                }
            }
            else
            {
                if (ConfDirectory.Name != entryName.Text)
                {
                    if (Conf!.Directories.ContainsKey(entryName.Text))
                    {
                        Message.Error($"Назва довідника не унікальна");
                        return;
                    }
                }

                Conf!.Directories.Remove(ConfDirectory.Name);
            }

            GetValue();

            Conf!.AppendDirectory(ConfDirectory);

            IsNew = false;

            GeneralForm?.LoadTree();
            GeneralForm?.RenameCurrentPageNotebook($"Довідник: {ConfDirectory.Name}");
        }

        #region Fields

        void OnFieldsButtonPress(object? sender, ButtonPressEventArgs args)
        {
            if (args.Event.Type == Gdk.EventType.DoubleButtonPress)
            {
                ListBoxRow[] selectedRows = listBoxFields.SelectedRows;

                if (selectedRows.Length != 0)
                {
                    ListBoxRow curRow = selectedRows[0];

                    if (ConfDirectory.Fields.ContainsKey(curRow.Child.Name))
                        GeneralForm?.CreateNotebookPage($"Поле: {curRow.Child.Name}", () =>
                        {
                            PageField page = new PageField()
                            {
                                Fields = ConfDirectory.Fields,
                                Field = ConfDirectory.Fields[curRow.Child.Name],
                                IsNew = false,
                                GeneralForm = GeneralForm,
                                CallBack_RefreshList = FieldsRefreshList
                            };

                            page.SetValue();

                            return page;
                        });
                }
            }
        }

        void OnFieldsAddClick(object? sender, EventArgs args)
        {
            GeneralForm?.CreateNotebookPage("Поле *", () =>
            {
                PageField page = new PageField()
                {
                    Table = ConfDirectory.Table,
                    Fields = ConfDirectory.Fields,
                    IsNew = true,
                    GeneralForm = GeneralForm,
                    CallBack_RefreshList = FieldsRefreshList
                };

                page.SetValue();

                return page;
            });
        }

        void OnFieldsCopyClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxFields.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (ConfDirectory.Fields.ContainsKey(row.Child.Name))
                    {
                        ConfigurationObjectField newField = ConfDirectory.Fields[row.Child.Name].Copy();
                        newField.Name += GenerateName.GetNewName();
                        newField.NameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel!, ConfDirectory.Table, ConfDirectory.Fields);

                        ConfDirectory.AppendField(newField);
                    }
                }

                FieldsRefreshList();

                GeneralForm?.LoadTree();
            }
        }

        void OnFieldsRefreshClick(object? sender, EventArgs args)
        {
            foreach (Widget item in listBoxFields.Children)
                listBoxFields.Remove(item);

            FillFields();

            listBoxFields.ShowAll();
        }

        void OnFieldsRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxFields.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (ConfDirectory.Fields.ContainsKey(row.Child.Name))
                        ConfDirectory.Fields.Remove(row.Child.Name);
                }

                FieldsRefreshList();

                GeneralForm?.LoadTree();
            }
        }

        void FieldsRefreshList()
        {
            OnFieldsRefreshClick(null, new EventArgs());
        }

        #endregion

        #region TabularParts

        void OnTabularPartsButtonPress(object? sender, ButtonPressEventArgs args)
        {
            if (args.Event.Type == Gdk.EventType.DoubleButtonPress)
            {
                ListBoxRow[] selectedRows = listBoxTableParts.SelectedRows;

                if (selectedRows.Length != 0)
                {
                    ListBoxRow curRow = selectedRows[0];

                    if (ConfDirectory.TabularParts.ContainsKey(curRow.Child.Name))
                        GeneralForm?.CreateNotebookPage($"Таблична частина: {curRow.Child.Name}", () =>
                        {
                            PageTablePart page = new PageTablePart()
                            {
                                TabularParts = ConfDirectory.TabularParts,
                                TablePart = ConfDirectory.TabularParts[curRow.Child.Name],
                                IsNew = false,
                                GeneralForm = GeneralForm,
                                CallBack_RefreshList = TabularPartsRefreshList
                            };

                            page.SetValue();

                            return page;
                        });
                }
            }
        }

        void OnTabularPartsAddClick(object? sender, EventArgs args)
        {
            GeneralForm?.CreateNotebookPage("Таблична частина *", () =>
            {
                PageTablePart page = new PageTablePart()
                {
                    TabularParts = ConfDirectory.TabularParts,
                    IsNew = true,
                    GeneralForm = GeneralForm,
                    CallBack_RefreshList = TabularPartsRefreshList
                };

                page.SetValue();

                return page;
            });
        }

        void OnTabularPartsCopyClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxTableParts.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (ConfDirectory.TabularParts.ContainsKey(row.Child.Name))
                    {
                        ConfigurationObjectTablePart newTablePart = ConfDirectory.TabularParts[row.Child.Name].Copy();
                        newTablePart.Name += GenerateName.GetNewName();
                        newTablePart.Table = Configuration.GetNewUnigueTableName(Program.Kernel!);

                        ConfDirectory.AppendTablePart(newTablePart);
                    }
                }

                TabularPartsRefreshList();

                GeneralForm?.LoadTree();
            }
        }

        void OnTabularPartsRefreshClick(object? sender, EventArgs args)
        {
            foreach (Widget item in listBoxTableParts.Children)
                listBoxTableParts.Remove(item);

            FillTabularParts();

            listBoxTableParts.ShowAll();
        }

        void OnTabularPartsRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxTableParts.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (ConfDirectory.TabularParts.ContainsKey(row.Child.Name))
                        ConfDirectory.TabularParts.Remove(row.Child.Name);
                }

                TabularPartsRefreshList();

                GeneralForm?.LoadTree();
            }
        }

        void TabularPartsRefreshList()
        {
            OnTabularPartsRefreshClick(null, new EventArgs());
        }

        #endregion
    }
}