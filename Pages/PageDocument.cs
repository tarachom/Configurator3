using Gtk;

using AccountingSoftware;

namespace Configurator
{
    class PageDocument : VBox
    {
        Configuration? Conf
        {
            get
            {
                return Program.Kernel?.Conf;
            }
        }

        public ConfigurationDocuments ConfDocument { get; set; } = new ConfigurationDocuments();
        public FormConfigurator? GeneralForm { get; set; }
        public bool IsNew { get; set; } = true;

        ListBox listBoxFields = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxTableParts = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxTabularList = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxAllowRegAccum = new ListBox() { SelectionMode = SelectionMode.Single };

        Entry entryName = new Entry() { WidthRequest = 500 };
        Entry entryTable = new Entry() { WidthRequest = 500 };
        Entry entrySpend = new Entry() { WidthRequest = 500 };
        Entry entryClearSpend = new Entry() { WidthRequest = 500 };
        Entry entryBeforeSave = new Entry() { WidthRequest = 500 };
        Entry entryAfterSave = new Entry() { WidthRequest = 500 };
        Entry entryBeforeDelete = new Entry() { WidthRequest = 500 };
        TextView textViewDesc = new TextView();

        public PageDocument() : base()
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

            //Заголовок списку регістрів
            HBox hBoxAllowRegAcummInfo = new HBox() { Halign = Align.Center };
            vBox.PackStart(hBoxAllowRegAcummInfo, false, false, 5);
            hBoxAllowRegAcummInfo.PackStart(new Label("Регістри які використовує документ"), false, false, 5);

            //Робить рухи по регістрах
            HBox hBoxAllowRegAcumm = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxAllowRegAcumm, false, false, 5);

            ScrolledWindow scrollAllowList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollAllowList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollAllowList.SetSizeRequest(500, 200);

            scrollAllowList.Add(listBoxAllowRegAccum);
            hBoxAllowRegAcumm.PackStart(scrollAllowList, true, true, 5);

            //Заголовок блоку Функції
            HBox hBoxSpendInfo = new HBox() { Halign = Align.Center };
            vBox.PackStart(hBoxSpendInfo, false, false, 5);
            hBoxSpendInfo.PackStart(new Label("Функції"), false, false, 5);

            //Проведення
            HBox hBoxSpend = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxSpend, false, false, 5);

            hBoxSpend.PackStart(new Label("Проведення:"), false, false, 5);
            hBoxSpend.PackStart(entrySpend, false, false, 5);

            //Очищення
            HBox hBoxClearSpend = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxClearSpend, false, false, 5);

            hBoxClearSpend.PackStart(new Label("Очищення:"), false, false, 5);
            hBoxClearSpend.PackStart(entryClearSpend, false, false, 5);

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

            //Табличні списки
            CreateTabularList(vBox);

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

            ToolButton buttonAdd = new ToolButton(Stock.New) { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnFieldsAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(Stock.Copy) { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnFieldsCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(Stock.Refresh) { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnFieldsRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(Stock.Clear) { Label = "Видалити", IsImportant = true };
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

            ToolButton buttonAdd = new ToolButton(Stock.New) { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnTabularPartsAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(Stock.Copy) { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnTabularPartsCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(Stock.Refresh) { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnTabularPartsRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(Stock.Clear) { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnTabularPartsRemoveClick;
            toolbar.Add(buttonDelete);

            HBox hBoxScroll = new HBox();
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 100);

            listBoxTableParts.ButtonPressEvent += OnTabularPartsButtonPress;

            scrollList.Add(listBoxTableParts);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            vBoxContainer.PackStart(vBox, false, false, 0);
        }

        void CreateTabularList(VBox vBoxContainer)
        {
            VBox vBox = new VBox();

            HBox hBox = new HBox();
            hBox.PackStart(new Label("Табличні списки:"), false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(Stock.New) { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnTabularListAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(Stock.Copy) { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnTabularListCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(Stock.Refresh) { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnTabularListRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(Stock.Clear) { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnTabularListRemoveClick;
            toolbar.Add(buttonDelete);

            HBox hBoxScroll = new HBox();
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 100);

            listBoxTabularList.ButtonPressEvent += OnTabularListButtonPress;

            scrollList.Add(listBoxTabularList);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            vBoxContainer.PackStart(vBox, false, false, 0);
        }

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            entryName.Text = ConfDocument.Name;

            if (IsNew)
            {
                entryTable.Text = Configuration.GetNewUnigueTableName(Program.Kernel!);

                ConfDocument.AppendField(new ConfigurationObjectField("Назва", "docname", "string", "", "Назва", true));
                ConfDocument.AppendField(new ConfigurationObjectField("ДатаДок", "docdate", "datetime", "", "ДатаДок", false, true));
                ConfDocument.AppendField(new ConfigurationObjectField("НомерДок", "docnomer", "string", "", "НомерДок", false, true));

                string nameInTable_Comment = Configuration.GetNewUnigueColumnName(Program.Kernel!, entryTable.Text, ConfDocument.Fields);
                ConfDocument.AppendField(new ConfigurationObjectField("Коментар", nameInTable_Comment, "string", "", "Коментар"));
            }
            else
                entryTable.Text = ConfDocument.Table;

            textViewDesc.Buffer.Text = ConfDocument.Desc;

            entrySpend.Text = ConfDocument.SpendFunctions.Spend;
            entryClearSpend.Text = ConfDocument.SpendFunctions.ClearSpend;

            entryBeforeSave.Text = ConfDocument.TriggerFunctions.BeforeSave;
            entryAfterSave.Text = ConfDocument.TriggerFunctions.AfterSave;
            entryBeforeDelete.Text = ConfDocument.TriggerFunctions.BeforeDelete;

            FillAllowRegAccum();
            FillFields();
            FillTabularParts();
            FillTabularList();
        }

        void FillAllowRegAccum()
        {
            foreach (ConfigurationRegistersAccumulation regAccum in Conf!.RegistersAccumulation.Values)
                listBoxAllowRegAccum.Add(
                    new CheckButton(regAccum.Name)
                    {
                        Name = regAccum.Name,
                        Active = ConfDocument.AllowRegisterAccumulation.Contains(regAccum.Name)
                    });
        }

        void FillFields()
        {
            foreach (ConfigurationObjectField field in ConfDocument.Fields.Values)
                listBoxFields.Add(new Label(field.Name + (field.IsPresentation ? " [ представлення ]" : "")) { Name = field.Name, Halign = Align.Start });
        }

        void FillTabularParts()
        {
            foreach (ConfigurationObjectTablePart tablePart in ConfDocument.TabularParts.Values)
                listBoxTableParts.Add(new Label(tablePart.Name) { Name = tablePart.Name, Halign = Align.Start });
        }

        void FillTabularList()
        {
            foreach (ConfigurationTabularList tableList in ConfDocument.TabularList.Values)
                listBoxTabularList.Add(new Label(tableList.Name) { Name = tableList.Name, Halign = Align.Start });
        }

        void GetValue()
        {
            ConfDocument.Name = entryName.Text;
            ConfDocument.Table = entryTable.Text;
            ConfDocument.Desc = textViewDesc.Buffer.Text;

            ConfDocument.SpendFunctions.Spend = entrySpend.Text;
            ConfDocument.SpendFunctions.ClearSpend = entryClearSpend.Text;

            ConfDocument.TriggerFunctions.BeforeSave = entryBeforeSave.Text;
            ConfDocument.TriggerFunctions.AfterSave = entryAfterSave.Text;
            ConfDocument.TriggerFunctions.BeforeDelete = entryBeforeDelete.Text;

            //Доспупні регістри
            ConfDocument.AllowRegisterAccumulation.Clear();

            foreach (ListBoxRow item in listBoxAllowRegAccum.Children)
            {
                CheckButton cb = (CheckButton)item.Child;
                if (cb.Active)
                    ConfDocument.AllowRegisterAccumulation.Add(cb.Name);
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
                if (Conf!.Documents.ContainsKey(entryName.Text))
                {
                    Message.Error(GeneralForm, $"Назва документу не унікальна");
                    return;
                }
            }
            else
            {
                if (ConfDocument.Name != entryName.Text)
                {
                    if (Conf!.Documents.ContainsKey(entryName.Text))
                    {
                        Message.Error(GeneralForm, $"Назва документу не унікальна");
                        return;
                    }
                }

                Conf!.Documents.Remove(ConfDocument.Name);
            }

            GetValue();

            Conf!.AppendDocument(ConfDocument);

            IsNew = false;

            GeneralForm?.LoadTreeAsync();
            GeneralForm?.RenameCurrentPageNotebook($"Документ: {ConfDocument.Name}");
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

                    if (ConfDocument.Fields.ContainsKey(curRow.Child.Name))
                        GeneralForm?.CreateNotebookPage($"Поле: {curRow.Child.Name}", () =>
                        {
                            PageField page = new PageField()
                            {
                                Fields = ConfDocument.Fields,
                                Field = ConfDocument.Fields[curRow.Child.Name],
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
                    Table = ConfDocument.Table,
                    Fields = ConfDocument.Fields,
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
                    if (ConfDocument.Fields.ContainsKey(row.Child.Name))
                    {
                        ConfigurationObjectField newField = ConfDocument.Fields[row.Child.Name].Copy();
                        newField.Name += GenerateName.GetNewName();

                        ConfDocument.AppendField(newField);
                    }
                }

                FieldsRefreshList();

                GeneralForm?.LoadTreeAsync();
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
                    if (ConfDocument.Fields.ContainsKey(row.Child.Name))
                        ConfDocument.Fields.Remove(row.Child.Name);
                }

                FieldsRefreshList();

                GeneralForm?.LoadTreeAsync();
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

                    if (ConfDocument.TabularParts.ContainsKey(curRow.Child.Name))
                        GeneralForm?.CreateNotebookPage($"Таблична частина: {curRow.Child.Name}", () =>
                        {
                            PageTablePart page = new PageTablePart()
                            {
                                TabularParts = ConfDocument.TabularParts,
                                TablePart = ConfDocument.TabularParts[curRow.Child.Name],
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
                    TabularParts = ConfDocument.TabularParts,
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
                    if (ConfDocument.TabularParts.ContainsKey(row.Child.Name))
                    {
                        ConfigurationObjectTablePart newTablePart = ConfDocument.TabularParts[row.Child.Name].Copy();
                        newTablePart.Name += GenerateName.GetNewName();
                        newTablePart.Table = Configuration.GetNewUnigueTableName(Program.Kernel!);

                        ConfDocument.AppendTablePart(newTablePart);
                    }
                }

                TabularPartsRefreshList();

                GeneralForm?.LoadTreeAsync();
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
                    if (ConfDocument.TabularParts.ContainsKey(row.Child.Name))
                        ConfDocument.TabularParts.Remove(row.Child.Name);
                }

                TabularPartsRefreshList();

                GeneralForm?.LoadTreeAsync();
            }
        }

        void TabularPartsRefreshList()
        {
            OnTabularPartsRefreshClick(null, new EventArgs());
        }

        #endregion

        #region TabularList

        void OnTabularListButtonPress(object? sender, ButtonPressEventArgs args)
        {
            if (args.Event.Type == Gdk.EventType.DoubleButtonPress)
            {
                ListBoxRow[] selectedRows = listBoxTabularList.SelectedRows;

                if (selectedRows.Length != 0)
                {
                    ListBoxRow curRow = selectedRows[0];

                    if (ConfDocument.TabularList.ContainsKey(curRow.Child.Name))
                        GeneralForm?.CreateNotebookPage($"Табличний список: {curRow.Child.Name}", () =>
                        {
                            PageTabularList page = new PageTabularList()
                            {
                                Fields = ConfDocument.Fields,
                                TabularLists = ConfDocument.TabularList,
                                TabularList = ConfDocument.TabularList[curRow.Child.Name],
                                IsNew = false,
                                GeneralForm = GeneralForm,
                                CallBack_RefreshList = TabularListRefreshList
                            };

                            page.SetValue();

                            return page;
                        });
                }
            }
        }

        void OnTabularListAddClick(object? sender, EventArgs args)
        {
            GeneralForm?.CreateNotebookPage("Табличний список *", () =>
            {
                PageTabularList page = new PageTabularList()
                {
                    Fields = ConfDocument.Fields,
                    TabularLists = ConfDocument.TabularList,
                    IsNew = true,
                    GeneralForm = GeneralForm,
                    CallBack_RefreshList = TabularListRefreshList
                };

                page.SetValue();

                return page;
            });
        }

        void OnTabularListCopyClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxTabularList.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (ConfDocument.TabularList.ContainsKey(row.Child.Name))
                    {
                        ConfigurationTabularList newTableList = ConfDocument.TabularList[row.Child.Name].Copy();
                        newTableList.Name += GenerateName.GetNewName();

                        ConfDocument.AppendTableList(newTableList);
                    }
                }

                TabularListRefreshList();

                GeneralForm?.LoadTreeAsync();
            }
        }

        void OnTabularListRefreshClick(object? sender, EventArgs args)
        {
            foreach (Widget item in listBoxTabularList.Children)
                listBoxTabularList.Remove(item);

            FillTabularList();

            listBoxTabularList.ShowAll();
        }


        void OnTabularListRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxTabularList.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (ConfDocument.TabularList.ContainsKey(row.Child.Name))
                        ConfDocument.TabularList.Remove(row.Child.Name);
                }

                TabularListRefreshList();

                GeneralForm?.LoadTreeAsync();
            }
        }

        void TabularListRefreshList()
        {
            OnTabularListRefreshClick(null, new EventArgs());
        }

        #endregion

    }
}