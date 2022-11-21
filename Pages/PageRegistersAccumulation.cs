using Gtk;

using AccountingSoftware;

namespace Configurator
{
    class PageRegistersAccumulation : VBox
    {
        Configuration? Conf
        {
            get
            {
                return Program.Kernel?.Conf;
            }
        }

        public ConfigurationRegistersAccumulation ConfRegister { get; set; } = new ConfigurationRegistersAccumulation();
        public FormConfigurator? GeneralForm { get; set; }
        public bool IsNew { get; set; } = true;

        #region Fields

        ListBox listBoxAllowDocumentSpend = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxDimensionFields = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxResourcesFields = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxPropertyFields = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxTableParts = new ListBox() { SelectionMode = SelectionMode.Single };
        Entry entryName = new Entry() { WidthRequest = 500 };
        Entry entryTable = new Entry() { WidthRequest = 500 };
        TextView textViewDesc = new TextView();
        ComboBoxText comboBoxTypeReg = new ComboBoxText();

        #endregion

        public PageRegistersAccumulation() : base()
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

            //Поля
            CreateDimensionFieldList(vBox);

            CreateResourcesFieldList(vBox);

            CreatePropertyFieldList(vBox);

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

            //Таблиця
            HBox hBoxTable = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxTable, false, false, 5);

            hBoxTable.PackStart(new Label("Таблиця:"), false, false, 5);
            hBoxTable.PackStart(entryTable, false, false, 5);

            //Тип
            HBox hBoxTypeReg = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxTypeReg, false, false, 5);

            comboBoxTypeReg.Append(TypeRegistersAccumulation.Residues.ToString(), "Залишки");
            comboBoxTypeReg.Append(TypeRegistersAccumulation.Turnover.ToString(), "Обороти");

            hBoxTypeReg.PackStart(new Label("Вид регістру:"), false, false, 5);
            hBoxTypeReg.PackStart(comboBoxTypeReg, false, false, 5);

            //Опис
            HBox hBoxDesc = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxDesc, false, false, 5);

            hBoxDesc.PackStart(new Label("Опис:") { Valign = Align.Start }, false, false, 5);

            ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 500, HeightRequest = 100 };
            scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollTextView.Add(textViewDesc);

            hBoxDesc.PackStart(scrollTextView, false, false, 5);

            //Заголовок списку документів
            HBox hBoxAllowDocumentSpendCaption = new HBox() { Halign = Align.Center };
            vBox.PackStart(hBoxAllowDocumentSpendCaption, false, false, 5);
            hBoxAllowDocumentSpendCaption.PackStart(new Label("Документи які використовують цей регістер"), false, false, 5);

            //Список документів
            HBox hBoxAllowDocumentSpend = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxAllowDocumentSpend, false, false, 5);

            ScrolledWindow scrollAllowDocumentSpend = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollAllowDocumentSpend.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollAllowDocumentSpend.SetSizeRequest(500, 200);

            scrollAllowDocumentSpend.Add(listBoxAllowDocumentSpend);
            hBoxAllowDocumentSpend.PackStart(scrollAllowDocumentSpend, true, true, 5);

            //Табличні частини
            CreateTablePartList(vBox);

            hPaned.Pack1(vBox, false, false);
        }

        #region Fields

        void CreateDimensionFieldList(VBox vBoxContainer)
        {
            VBox vBox = new VBox();

            HBox hBox = new HBox();
            hBox.PackStart(new Label("Виміри:"), false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(Stock.New) { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnDimensionFieldsAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(Stock.Copy) { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnDimensionFieldsCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(Stock.Refresh) { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnDimensionFieldsRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(Stock.Clear) { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnDimensionFieldsRemoveClick;
            toolbar.Add(buttonDelete);

            HBox hBoxScroll = new HBox();
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 150);

            listBoxDimensionFields.ButtonPressEvent += OnDimensionFieldsButtonPress;

            scrollList.Add(listBoxDimensionFields);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            vBoxContainer.PackStart(vBox, false, false, 0);
        }

        void CreateResourcesFieldList(VBox vBoxContainer)
        {
            VBox vBox = new VBox();

            HBox hBox = new HBox();
            hBox.PackStart(new Label("Ресурси:"), false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(Stock.New) { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnResourcesFieldsAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(Stock.Copy) { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnResourcesFieldsCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(Stock.Refresh) { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnResourcesFieldsRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(Stock.Clear) { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnResourcesFieldsRemoveClick;
            toolbar.Add(buttonDelete);

            HBox hBoxScroll = new HBox();
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 150);

            listBoxResourcesFields.ButtonPressEvent += OnResourcesFieldsButtonPress;

            scrollList.Add(listBoxResourcesFields);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            vBoxContainer.PackStart(vBox, false, false, 0);
        }

        void CreatePropertyFieldList(VBox vBoxContainer)
        {
            VBox vBox = new VBox();

            HBox hBox = new HBox();
            hBox.PackStart(new Label("Поля:"), false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(Stock.New) { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnPropertyFieldsAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(Stock.Copy) { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnPropertyFieldsCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(Stock.Refresh) { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnPropertyFieldsRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(Stock.Clear) { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnPropertyFieldsRemoveClick;
            toolbar.Add(buttonDelete);

            HBox hBoxScroll = new HBox();
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 150);

            listBoxPropertyFields.ButtonPressEvent += OnPropertyFieldsButtonPress;

            scrollList.Add(listBoxPropertyFields);
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

        #endregion

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            FillAllowDocumentSpend();
            FillDimensionFields();
            FillResourcesFields();
            FillPropertyFields();
            FillTabularParts();

            entryName.Text = ConfRegister.Name;

            if (IsNew)
                entryTable.Text = Configuration.GetNewUnigueTableName(Program.Kernel!);
            else
                entryTable.Text = ConfRegister.Table;

            comboBoxTypeReg.ActiveId = ConfRegister.TypeRegistersAccumulation.ToString();

            if (comboBoxTypeReg.Active == -1)
                comboBoxTypeReg.Active = 0;

            textViewDesc.Buffer.Text = ConfRegister.Desc;
        }

        void FillAllowDocumentSpend()
        {
            foreach (string field in ConfRegister.AllowDocumentSpend)
                listBoxAllowDocumentSpend.Add(new Label(field) { Name = field, Halign = Align.Start });
        }

        void FillDimensionFields()
        {
            foreach (ConfigurationObjectField field in ConfRegister.DimensionFields.Values)
                listBoxDimensionFields.Add(new Label(field.Name) { Name = field.Name, Halign = Align.Start });
        }

        void FillResourcesFields()
        {
            foreach (ConfigurationObjectField field in ConfRegister.ResourcesFields.Values)
                listBoxResourcesFields.Add(new Label(field.Name) { Name = field.Name, Halign = Align.Start });
        }

        void FillPropertyFields()
        {
            foreach (ConfigurationObjectField field in ConfRegister.PropertyFields.Values)
                listBoxPropertyFields.Add(new Label(field.Name) { Name = field.Name, Halign = Align.Start });
        }

        void FillTabularParts()
        {
            foreach (ConfigurationObjectTablePart tablePart in ConfRegister.TabularParts.Values)
                listBoxTableParts.Add(new Label(tablePart.Name) { Name = tablePart.Name, Halign = Align.Start });
        }

        void GetValue()
        {
            ConfRegister.Name = entryName.Text;
            ConfRegister.Table = entryTable.Text;
            ConfRegister.TypeRegistersAccumulation = Enum.Parse<TypeRegistersAccumulation>(comboBoxTypeReg.ActiveId);
            ConfRegister.Desc = textViewDesc.Buffer.Text;
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
                if (Conf!.RegistersAccumulation.ContainsKey(entryName.Text))
                {
                    Message.Error(GeneralForm, $"Назва регістру не унікальна");
                    return;
                }
            }
            else
            {
                if (ConfRegister.Name != entryName.Text)
                {
                    if (Conf!.RegistersAccumulation.ContainsKey(entryName.Text))
                    {
                        Message.Error(GeneralForm, $"Назва регістру не унікальна");
                        return;
                    }
                }

                Conf!.RegistersAccumulation.Remove(ConfRegister.Name);
            }

            GetValue();

            Conf!.AppendRegistersAccumulation(ConfRegister);

            IsNew = false;

            GeneralForm?.LoadTreeAsync();
            GeneralForm?.RenameCurrentPageNotebook($"Регістер накопичення: {ConfRegister.Name}");
        }

        #region Dimension Fields

        void OnDimensionFieldsButtonPress(object? sender, ButtonPressEventArgs args)
        {
            if (args.Event.Type == Gdk.EventType.DoubleButtonPress)
            {
                ListBoxRow[] selectedRows = listBoxDimensionFields.SelectedRows;

                if (selectedRows.Length != 0)
                {
                    ListBoxRow curRow = selectedRows[0];

                    if (ConfRegister.DimensionFields.ContainsKey(curRow.Child.Name))
                        GeneralForm?.CreateNotebookPage($"Поле: {curRow.Child.Name}", () =>
                        {
                            PageField page = new PageField()
                            {
                                Fields = ConfRegister.DimensionFields,
                                Field = ConfRegister.DimensionFields[curRow.Child.Name],
                                IsNew = false,
                                GeneralForm = GeneralForm,
                                CallBack_RefreshList = DimensionFieldsRefreshList
                            };

                            page.SetValue();

                            return page;
                        });
                }
            }
        }

        void OnDimensionFieldsAddClick(object? sender, EventArgs args)
        {
            GeneralForm?.CreateNotebookPage("Поле *", () =>
            {
                PageField page = new PageField()
                {
                    Fields = ConfRegister.DimensionFields,
                    IsNew = true,
                    GeneralForm = GeneralForm,
                    CallBack_RefreshList = DimensionFieldsRefreshList
                };

                page.SetValue();

                return page;
            });
        }

        void OnDimensionFieldsCopyClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxDimensionFields.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (ConfRegister.DimensionFields.ContainsKey(row.Child.Name))
                    {
                        ConfigurationObjectField newField = ConfRegister.DimensionFields[row.Child.Name].Copy();
                        newField.Name += GenerateName.GetNewName();

                        ConfRegister.AppendDimensionField(newField);
                    }
                }

                DimensionFieldsRefreshList();

                GeneralForm?.LoadTreeAsync();
            }
        }

        void OnDimensionFieldsRefreshClick(object? sender, EventArgs args)
        {
            foreach (Widget item in listBoxDimensionFields.Children)
                listBoxDimensionFields.Remove(item);

            FillDimensionFields();

            listBoxDimensionFields.ShowAll();
        }

        void OnDimensionFieldsRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxDimensionFields.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (ConfRegister.DimensionFields.ContainsKey(row.Child.Name))
                        ConfRegister.DimensionFields.Remove(row.Child.Name);
                }

                DimensionFieldsRefreshList();

                GeneralForm?.LoadTreeAsync();
            }
        }

        void DimensionFieldsRefreshList()
        {
            OnDimensionFieldsRefreshClick(null, new EventArgs());
        }

        #endregion

        #region Resources Fields

        void OnResourcesFieldsButtonPress(object? sender, ButtonPressEventArgs args)
        {
            if (args.Event.Type == Gdk.EventType.DoubleButtonPress)
            {
                ListBoxRow[] selectedRows = listBoxResourcesFields.SelectedRows;

                if (selectedRows.Length != 0)
                {
                    ListBoxRow curRow = selectedRows[0];

                    if (ConfRegister.ResourcesFields.ContainsKey(curRow.Child.Name))
                        GeneralForm?.CreateNotebookPage($"Поле: {curRow.Child.Name}", () =>
                        {
                            PageField page = new PageField()
                            {
                                Fields = ConfRegister.ResourcesFields,
                                Field = ConfRegister.ResourcesFields[curRow.Child.Name],
                                IsNew = false,
                                GeneralForm = GeneralForm,
                                CallBack_RefreshList = ResourcesFieldsRefreshList
                            };

                            page.SetValue();

                            return page;
                        });
                }
            }
        }

        void OnResourcesFieldsAddClick(object? sender, EventArgs args)
        {
            GeneralForm?.CreateNotebookPage("Поле *", () =>
            {
                PageField page = new PageField()
                {
                    Fields = ConfRegister.ResourcesFields,
                    IsNew = true,
                    GeneralForm = GeneralForm,
                    CallBack_RefreshList = ResourcesFieldsRefreshList
                };

                page.SetValue();

                return page;
            });
        }

        void OnResourcesFieldsCopyClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxResourcesFields.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (ConfRegister.ResourcesFields.ContainsKey(row.Child.Name))
                    {
                        ConfigurationObjectField newField = ConfRegister.ResourcesFields[row.Child.Name].Copy();
                        newField.Name += GenerateName.GetNewName();

                        ConfRegister.AppendResourcesField(newField);
                    }
                }

                ResourcesFieldsRefreshList();

                GeneralForm?.LoadTreeAsync();
            }
        }

        void OnResourcesFieldsRefreshClick(object? sender, EventArgs args)
        {
            foreach (Widget item in listBoxResourcesFields.Children)
                listBoxResourcesFields.Remove(item);

            FillResourcesFields();

            listBoxResourcesFields.ShowAll();
        }

        void OnResourcesFieldsRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxResourcesFields.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (ConfRegister.ResourcesFields.ContainsKey(row.Child.Name))
                        ConfRegister.ResourcesFields.Remove(row.Child.Name);
                }

                ResourcesFieldsRefreshList();

                GeneralForm?.LoadTreeAsync();
            }
        }

        void ResourcesFieldsRefreshList()
        {
            OnResourcesFieldsRefreshClick(null, new EventArgs());
        }

        #endregion

        #region Property Fields

        void OnPropertyFieldsButtonPress(object? sender, ButtonPressEventArgs args)
        {
            if (args.Event.Type == Gdk.EventType.DoubleButtonPress)
            {
                ListBoxRow[] selectedRows = listBoxPropertyFields.SelectedRows;

                if (selectedRows.Length != 0)
                {
                    ListBoxRow curRow = selectedRows[0];

                    if (ConfRegister.PropertyFields.ContainsKey(curRow.Child.Name))
                        GeneralForm?.CreateNotebookPage($"Поле: {curRow.Child.Name}", () =>
                        {
                            PageField page = new PageField()
                            {
                                Fields = ConfRegister.PropertyFields,
                                Field = ConfRegister.PropertyFields[curRow.Child.Name],
                                IsNew = false,
                                GeneralForm = GeneralForm,
                                CallBack_RefreshList = PropertyFieldsRefreshList
                            };

                            page.SetValue();

                            return page;
                        });
                }
            }
        }

        void OnPropertyFieldsAddClick(object? sender, EventArgs args)
        {
            GeneralForm?.CreateNotebookPage("Поле *", () =>
            {
                PageField page = new PageField()
                {
                    Table = ConfRegister.Table,
                    Fields = ConfRegister.PropertyFields,
                    IsNew = true,
                    GeneralForm = GeneralForm,
                    CallBack_RefreshList = PropertyFieldsRefreshList
                };

                page.SetValue();

                return page;
            });
        }

        void OnPropertyFieldsCopyClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxPropertyFields.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (ConfRegister.PropertyFields.ContainsKey(row.Child.Name))
                    {
                        ConfigurationObjectField newField = ConfRegister.PropertyFields[row.Child.Name].Copy();
                        newField.Name += GenerateName.GetNewName();

                        ConfRegister.AppendPropertyField(newField);
                    }
                }

                PropertyFieldsRefreshList();

                GeneralForm?.LoadTreeAsync();
            }
        }

        void OnPropertyFieldsRefreshClick(object? sender, EventArgs args)
        {
            foreach (Widget item in listBoxPropertyFields.Children)
                listBoxPropertyFields.Remove(item);

            FillPropertyFields();

            listBoxPropertyFields.ShowAll();
        }

        void OnPropertyFieldsRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxPropertyFields.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (ConfRegister.PropertyFields.ContainsKey(row.Child.Name))
                        ConfRegister.PropertyFields.Remove(row.Child.Name);
                }

                PropertyFieldsRefreshList();

                GeneralForm?.LoadTreeAsync();
            }
        }

        void PropertyFieldsRefreshList()
        {
            OnPropertyFieldsRefreshClick(null, new EventArgs());
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

                    if (ConfRegister.TabularParts.ContainsKey(curRow.Child.Name))
                        GeneralForm?.CreateNotebookPage($"Таблична частина: {curRow.Child.Name}", () =>
                        {
                            PageTablePart page = new PageTablePart()
                            {
                                TabularParts = ConfRegister.TabularParts,
                                TablePart = ConfRegister.TabularParts[curRow.Child.Name],
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
                    TabularParts = ConfRegister.TabularParts,
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
                    if (ConfRegister.TabularParts.ContainsKey(row.Child.Name))
                    {
                        ConfigurationObjectTablePart newTablePart = ConfRegister.TabularParts[row.Child.Name].Copy();
                        newTablePart.Name += GenerateName.GetNewName();
                        newTablePart.Table = Configuration.GetNewUnigueTableName(Program.Kernel!);

                        ConfRegister.AppendTablePart(newTablePart);
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
                    if (ConfRegister.TabularParts.ContainsKey(row.Child.Name))
                        ConfRegister.TabularParts.Remove(row.Child.Name);
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
    }
}