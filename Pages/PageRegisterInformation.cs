using Gtk;

using AccountingSoftware;

namespace Configurator
{
    class PageRegisterInformation : VBox
    {
        Configuration? Conf
        {
            get
            {
                return Program.Kernel?.Conf;
            }
        }

        public ConfigurationRegistersInformation ConfRegister { get; set; } = new ConfigurationRegistersInformation();
        public FormConfigurator? GeneralForm { get; set; }
        public bool IsNew { get; set; } = true;

        ListBox listBoxDimensionFields = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxResourcesFields = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxPropertyFields = new ListBox() { SelectionMode = SelectionMode.Single };
        Entry entryName = new Entry() { WidthRequest = 400 };
        TextView textViewDesc = new TextView();

        public PageRegisterInformation() : base()
        {
            new VBox();
            HBox hBox = new HBox();

            Button bSave = new Button("Зберегти");
            bSave.Clicked += OnSaveClick;

            hBox.PackStart(bSave, false, false, 10);

            Button bClose = new Button("Закрити");
            bClose.Clicked += (object? sender, EventArgs args) => { GeneralForm?.CloseCurrentPageNotebook(); };

            hBox.PackStart(bClose, false, false, 10);

            Button bCopy = new Button("Копіювати");
            bCopy.Clicked += OnCopyClick;

            hBox.PackStart(bCopy, false, false, 10);

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

        void CreateDimensionFieldList(VBox vBoxContainer)
        {
            VBox vBox = new VBox();

            HBox hBox = new HBox();
            hBox.PackStart(new Label("Виміри:"), false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(Stock.Add) { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnDimensionFieldsAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(Stock.Copy) { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnDimensionFieldsCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(Stock.Refresh) { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnDimensionFieldsRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(Stock.Delete) { Label = "Видалити", IsImportant = true };
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

            ToolButton buttonAdd = new ToolButton(Stock.Add) { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnResourcesFieldsAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(Stock.Copy) { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnResourcesFieldsCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(Stock.Refresh) { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnResourcesFieldsRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(Stock.Delete) { Label = "Видалити", IsImportant = true };
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

            ToolButton buttonAdd = new ToolButton(Stock.Add) { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnPropertyFieldsAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(Stock.Copy) { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnPropertyFieldsCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(Stock.Refresh) { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnPropertyFieldsRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(Stock.Delete) { Label = "Видалити", IsImportant = true };
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

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            FillDimensionFields();
            FillResourcesFields();
            FillPropertyFields();

            entryName.Text = ConfRegister.Name;
            textViewDesc.Buffer.Text = ConfRegister.Desc;
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

        void GetValue()
        {
            ConfRegister.Name = entryName.Text;
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
                Message.Error($"{errorList}");
                return;
            }

            if (IsNew)
            {
                if (Conf!.RegistersInformation.ContainsKey(entryName.Text))
                {
                    Message.Error($"Назва регістру не унікальна");
                    return;
                }
            }
            else
            {
                if (ConfRegister.Name != entryName.Text)
                {
                    if (Conf!.RegistersInformation.ContainsKey(entryName.Text))
                    {
                        Message.Error($"Назва регістру не унікальна");
                        return;
                    }
                }

                Conf!.RegistersInformation.Remove(ConfRegister.Name);
            }

            GetValue();

            Conf!.AppendRegistersInformation(ConfRegister);

            IsNew = false;

            GeneralForm?.LoadTree();
            GeneralForm?.RenameCurrentPageNotebook($"Регістр: {ConfRegister.Name}");
        }

        void OnCopyClick(object? sender, EventArgs args)
        {
            string newName = ConfRegister.Name + "_copy";

            GeneralForm?.CreateNotebookPage($"Регістр: {newName}", () =>
            {
                ConfigurationRegistersInformation newRegister = ConfRegister.Copy();
                newRegister.Name = newName;

                PageRegisterInformation page = new PageRegisterInformation()
                {
                    IsNew = true,
                    GeneralForm = GeneralForm,
                    ConfRegister = newRegister
                };

                page.SetValue();

                return page;
            });
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

                page.SetDefValue();

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
                        string newName = "";

                        for (int i = 1; i < 99; i++)
                        {
                            newName = row.Child.Name + i.ToString();

                            if (!ConfRegister.DimensionFields.ContainsKey(newName))
                                break;
                        }

                        if (String.IsNullOrEmpty(newName))
                            newName = row.Child.Name + new Random(int.MaxValue).ToString();

                        ConfigurationObjectField newField = ConfRegister.DimensionFields[row.Child.Name].Copy();
                        newField.Name = newName;

                        ConfRegister.AppendDimensionField(newField);
                    }
                }

                DimensionFieldsRefreshList();

                GeneralForm?.LoadTree();
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

                GeneralForm?.LoadTree();
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

                page.SetDefValue();

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
                        string newName = "";

                        for (int i = 1; i < 99; i++)
                        {
                            newName = row.Child.Name + i.ToString();

                            if (!ConfRegister.ResourcesFields.ContainsKey(newName))
                                break;
                        }

                        if (String.IsNullOrEmpty(newName))
                            newName = row.Child.Name + new Random(int.MaxValue).ToString();

                        ConfigurationObjectField newField = ConfRegister.ResourcesFields[row.Child.Name].Copy();
                        newField.Name = newName;

                        ConfRegister.AppendResourcesField(newField);
                    }
                }

                ResourcesFieldsRefreshList();

                GeneralForm?.LoadTree();
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

                GeneralForm?.LoadTree();
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
                    Fields = ConfRegister.PropertyFields,
                    IsNew = true,
                    GeneralForm = GeneralForm,
                    CallBack_RefreshList = PropertyFieldsRefreshList
                };

                page.SetDefValue();

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
                        string newName = "";

                        for (int i = 1; i < 99; i++)
                        {
                            newName = row.Child.Name + i.ToString();

                            if (!ConfRegister.PropertyFields.ContainsKey(newName))
                                break;
                        }

                        if (String.IsNullOrEmpty(newName))
                            newName = row.Child.Name + new Random(int.MaxValue).ToString();

                        ConfigurationObjectField newField = ConfRegister.PropertyFields[row.Child.Name].Copy();
                        newField.Name = newName;

                        ConfRegister.AppendPropertyField(newField);
                    }
                }

                PropertyFieldsRefreshList();

                GeneralForm?.LoadTree();
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

                GeneralForm?.LoadTree();
            }
        }

        void PropertyFieldsRefreshList()
        {
            OnPropertyFieldsRefreshClick(null, new EventArgs());
        }

        #endregion
    }
}