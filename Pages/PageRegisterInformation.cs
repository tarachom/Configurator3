/*
Copyright (C) 2019-2023 TARAKHOMYN YURIY IVANOVYCH
All rights reserved.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

/*
Автор:    Тарахомин Юрій Іванович
Адреса:   Україна, м. Львів
Сайт:     accounting.org.ua
*/

using Gtk;

using AccountingSoftware;

namespace Configurator
{
    class PageRegisterInformation : Box
    {
        Configuration Conf { get { return Program.Kernel.Conf; } }

        public ConfigurationRegistersInformation ConfRegister { get; set; } = new ConfigurationRegistersInformation();
        public FormConfigurator? GeneralForm { get; set; }
        public bool IsNew { get; set; } = true;

        #region Fields

        ListBox listBoxDimensionFields = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxResourcesFields = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxPropertyFields = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxTabularList = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxFormsList = new ListBox() { SelectionMode = SelectionMode.Single };
        Entry entryName = new Entry() { WidthRequest = 500 };
        Entry entryFullName = new Entry() { WidthRequest = 500 };
        Entry entryTable = new Entry() { WidthRequest = 500 };
        TextView textViewDesc = new TextView() { WrapMode = WrapMode.Word };

        #endregion

        public PageRegisterInformation() : base(Orientation.Vertical, 0)
        {
            Box hBox = new Box(Orientation.Horizontal, 0);

            Button bSave = new Button("Зберегти");
            bSave.Clicked += OnSaveClick;

            hBox.PackStart(bSave, false, false, 10);

            Button bClose = new Button("Закрити");
            bClose.Clicked += (object? sender, EventArgs args) => { GeneralForm?.CloseCurrentPageNotebook(); };

            hBox.PackStart(bClose, false, false, 10);

            PackStart(hBox, false, false, 10);

            Paned hPaned = new Paned(Orientation.Horizontal) { BorderWidth = 5 };

            CreatePack1(hPaned);
            CreatePack2(hPaned);

            PackStart(hPaned, false, false, 5);

            ShowAll();
        }

        void CreatePack1(Paned hPaned)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            //Назва
            Box hBoxName = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxName, false, false, 5);

            hBoxName.PackStart(new Label("Назва:"), false, false, 5);
            hBoxName.PackStart(entryName, false, false, 5);

            //Повна Назва
            Box hBoxFullName = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxFullName, false, false, 5);

            hBoxFullName.PackStart(new Label("Повна назва:"), false, false, 5);
            hBoxFullName.PackStart(entryFullName, false, false, 5);

            //Таблиця
            Box hBoxTable = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxTable, false, false, 5);

            hBoxTable.PackStart(new Label("Таблиця:"), false, false, 5);
            hBoxTable.PackStart(entryTable, false, false, 5);

            //Опис
            Box hBoxDesc = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxDesc, false, false, 5);

            hBoxDesc.PackStart(new Label("Опис:") { Valign = Align.Start }, false, false, 5);

            ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 500, HeightRequest = 100 };
            scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollTextView.Add(textViewDesc);

            hBoxDesc.PackStart(scrollTextView, false, false, 5);

            //Списки
            {
                Expander expanderForm = new Expander("Табличні списки");
                vBox.PackStart(expanderForm, false, false, 5);

                Box vBoxForm = new Box(Orientation.Vertical, 0);
                expanderForm.Add(vBoxForm);

                //Заголовок блоку Forms
                Box hBoxInterfaceCreateInfo = new Box(Orientation.Horizontal, 0) { Halign = Align.Center };
                vBoxForm.PackStart(hBoxInterfaceCreateInfo, false, false, 5);
                hBoxInterfaceCreateInfo.PackStart(new Label("Табличні списки"), false, false, 5);

                //Табличні списки
                CreateTabularList(vBoxForm);
            }

            //Форми
            {
                Expander expanderForm = new Expander("Форми");
                vBox.PackStart(expanderForm, false, false, 5);

                Box vBoxForm = new Box(Orientation.Vertical, 0);
                expanderForm.Add(vBoxForm);

                //Заголовок блоку Forms
                Box hBoxInterfaceCreateInfo = new Box(Orientation.Horizontal, 0) { Halign = Align.Center };
                vBoxForm.PackStart(hBoxInterfaceCreateInfo, false, false, 5);
                hBoxInterfaceCreateInfo.PackStart(new Label("Форми"), false, false, 5);

                //Форми
                CreateFormsList(vBoxForm);
            }

            hPaned.Pack1(vBox, false, false);
        }

        void CreatePack2(Paned hPaned)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            //Стандартні поля
            Expander expanderDefField = new Expander("Стандартні поля");
            vBox.PackStart(expanderDefField, false, false, 5);

            expanderDefField.Add(new Label(" <b>uid</b> \n <b>period</b> - дата та час запису \n <b>owner</b> - власник запису") { Halign = Align.Start, UseMarkup = true, UseUnderline = false, Selectable = true });

            //Поля
            CreateDimensionFieldList(vBox);

            CreateResourcesFieldList(vBox);

            CreatePropertyFieldList(vBox);

            hPaned.Pack2(vBox, true, false);
        }

        #region Fields

        void CreateDimensionFieldList(Box vBoxContainer)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            Box hBox = new Box(Orientation.Horizontal, 0);
            hBox.PackStart(new Label("Виміри:"), false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(new Image(Stock.New, IconSize.Menu), "Додати") { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnDimensionFieldsAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(new Image(Stock.Copy, IconSize.Menu), "Копіювати") { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnDimensionFieldsCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(new Image(Stock.Refresh, IconSize.Menu), "Обновити") { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnDimensionFieldsRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(new Image(Stock.Clear, IconSize.Menu), "Видалити") { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnDimensionFieldsRemoveClick;
            toolbar.Add(buttonDelete);

            Box hBoxScroll = new Box(Orientation.Horizontal, 0);
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 150);

            listBoxDimensionFields.ButtonPressEvent += OnDimensionFieldsButtonPress;

            scrollList.Add(listBoxDimensionFields);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            vBoxContainer.PackStart(vBox, false, false, 0);
        }

        void CreateResourcesFieldList(Box vBoxContainer)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            Box hBox = new Box(Orientation.Horizontal, 0);
            hBox.PackStart(new Label("Ресурси:"), false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(new Image(Stock.New, IconSize.Menu), "Додати") { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnResourcesFieldsAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(new Image(Stock.Copy, IconSize.Menu), "Копіювати") { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnResourcesFieldsCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(new Image(Stock.Refresh, IconSize.Menu), "Обновити") { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnResourcesFieldsRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(new Image(Stock.Clear, IconSize.Menu), "Видалити") { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnResourcesFieldsRemoveClick;
            toolbar.Add(buttonDelete);

            Box hBoxScroll = new Box(Orientation.Horizontal, 0);
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 150);

            listBoxResourcesFields.ButtonPressEvent += OnResourcesFieldsButtonPress;

            scrollList.Add(listBoxResourcesFields);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            vBoxContainer.PackStart(vBox, false, false, 0);
        }

        void CreatePropertyFieldList(Box vBoxContainer)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            Box hBox = new Box(Orientation.Horizontal, 0);
            hBox.PackStart(new Label("Поля:"), false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(new Image(Stock.New, IconSize.Menu), "Додати") { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnPropertyFieldsAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(new Image(Stock.Copy, IconSize.Menu), "Копіювати") { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnPropertyFieldsCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(new Image(Stock.Refresh, IconSize.Menu), "Обновити") { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnPropertyFieldsRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(new Image(Stock.Clear, IconSize.Menu), "Видалити") { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnPropertyFieldsRemoveClick;
            toolbar.Add(buttonDelete);

            Box hBoxScroll = new Box(Orientation.Horizontal, 0);
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 150);

            listBoxPropertyFields.ButtonPressEvent += OnPropertyFieldsButtonPress;

            scrollList.Add(listBoxPropertyFields);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            vBoxContainer.PackStart(vBox, false, false, 0);
        }

        void CreateTabularList(Box vBoxContainer)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(new Image(Stock.New, IconSize.Menu), "Додати") { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnTabularListAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(new Image(Stock.Copy, IconSize.Menu), "Копіювати") { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnTabularListCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(new Image(Stock.Refresh, IconSize.Menu), "Обновити") { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnTabularListRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(new Image(Stock.Clear, IconSize.Menu), "Видалити") { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnTabularListRemoveClick;
            toolbar.Add(buttonDelete);

            Box hBoxScroll = new Box(Orientation.Horizontal, 0);
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 100);

            listBoxTabularList.ButtonPressEvent += OnTabularListButtonPress;

            scrollList.Add(listBoxTabularList);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            vBoxContainer.PackStart(vBox, false, false, 0);
        }

        void CreateFormsList(Box vBoxContainer)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            Button buttonCreateForms = new Button("Створити");
            buttonCreateForms.Clicked += (object? sender, EventArgs args) =>
            {

            };

            Box hBox = new Box(Orientation.Horizontal, 0);
            hBox.PackStart(buttonCreateForms, false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            MenuToolButton buttonAdd = new MenuToolButton(new Image(Stock.New, IconSize.Menu), "Додати") { IsImportant = true, Menu = OnFormsListAddFormSubMenu() };
            buttonAdd.Clicked += (object? sender, EventArgs arg) => { ((Menu)((MenuToolButton)sender!).Menu).Popup(); };
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(new Image(Stock.Copy, IconSize.Menu), "Копіювати") { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnFormsListCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(new Image(Stock.Refresh, IconSize.Menu), "Обновити") { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnFormsListRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(new Image(Stock.Clear, IconSize.Menu), "Видалити") { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnFormsListRemoveClick;
            toolbar.Add(buttonDelete);

            Box hBoxScroll = new Box(Orientation.Horizontal, 0);
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 200);

            listBoxFormsList.ButtonPressEvent += OnFormsListButtonPress;

            scrollList.Add(listBoxFormsList);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            vBoxContainer.PackStart(vBox, false, false, 0);
        }

        #endregion

        #region Присвоєння / зчитування значень віджетів

        public async void SetValue()
        {
            FillDimensionFields();
            FillResourcesFields();
            FillPropertyFields();
            FillTabularList();
            FillFormsList();

            entryName.Text = ConfRegister.Name;
            entryFullName.Text = ConfRegister.FullName;

            if (IsNew)
                entryTable.Text = await Configuration.GetNewUnigueTableName(Program.Kernel);
            else
                entryTable.Text = ConfRegister.Table;

            textViewDesc.Buffer.Text = ConfRegister.Desc;
        }

        void FillDimensionFields()
        {
            foreach (ConfigurationField field in ConfRegister.DimensionFields.Values)
                listBoxDimensionFields.Add(new Label(field.Name) { Name = field.Name, Halign = Align.Start, UseUnderline = false });

            listBoxDimensionFields.ShowAll();
        }

        void FillResourcesFields()
        {
            foreach (ConfigurationField field in ConfRegister.ResourcesFields.Values)
                listBoxResourcesFields.Add(new Label(field.Name) { Name = field.Name, Halign = Align.Start, UseUnderline = false });

            listBoxResourcesFields.ShowAll();
        }

        void FillPropertyFields()
        {
            foreach (ConfigurationField field in ConfRegister.PropertyFields.Values)
                listBoxPropertyFields.Add(new Label(field.Name) { Name = field.Name, Halign = Align.Start, UseUnderline = false });

            listBoxPropertyFields.ShowAll();
        }

        void FillTabularList()
        {
            foreach (ConfigurationTabularList tableList in ConfRegister.TabularList.Values)
                listBoxTabularList.Add(new Label(tableList.Name) { Name = tableList.Name, Halign = Align.Start, UseUnderline = false });

            listBoxTabularList.ShowAll();
        }

        void FillFormsList()
        {
            foreach (ConfigurationForms form in ConfRegister.Forms.Values)
                listBoxFormsList.Add(new Label(form.Name) { Name = form.Name, Halign = Align.Start, UseUnderline = false });

            listBoxFormsList.ShowAll();
        }

        void GetValue()
        {
            if (string.IsNullOrEmpty(entryFullName.Text))
                entryFullName.Text = entryName.Text;

            ConfRegister.Name = entryName.Text;
            ConfRegister.FullName = entryFullName.Text;
            ConfRegister.Table = entryTable.Text;
            ConfRegister.Desc = textViewDesc.Buffer.Text;
        }

        #endregion

        void OnSaveClick(object? sender, EventArgs args)
        {
            string name = entryName.Text;
            string errorList = Configuration.ValidateConfigurationObjectName(ref name);
            entryName.Text = name;

            if (errorList.Length > 0)
            {
                Message.Error(GeneralForm, $"{errorList}");
                return;
            }

            if (IsNew)
            {
                if (Conf.RegistersInformation.ContainsKey(entryName.Text))
                {
                    Message.Error(GeneralForm, $"Назва регістру не унікальна");
                    return;
                }
            }
            else
            {
                if (ConfRegister.Name != entryName.Text)
                {
                    if (Conf.RegistersInformation.ContainsKey(entryName.Text))
                    {
                        Message.Error(GeneralForm, $"Назва регістру не унікальна");
                        return;
                    }
                }

                Conf.RegistersInformation.Remove(ConfRegister.Name);
            }

            GetValue();

            Conf.AppendRegistersInformation(ConfRegister);

            IsNew = false;

            GeneralForm?.LoadTreeAsync();
            GeneralForm?.RenameCurrentPageNotebook($"Регістр відомостей: {ConfRegister.Name}");
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

                    if (ConfRegister.DimensionFields.TryGetValue(curRow.Child.Name, out ConfigurationField? dimensionFields))
                        GeneralForm?.CreateNotebookPage($"Поле: {curRow.Child.Name}", () =>
                        {
                            Dictionary<string, ConfigurationField> AllFields = Conf.CombineAllFieldForRegister
                            (
                                ConfRegister.DimensionFields.Values,
                                ConfRegister.ResourcesFields.Values,
                                ConfRegister.PropertyFields.Values
                            );

                            PageField page = new PageField()
                            {
                                AllFields = AllFields,
                                Fields = ConfRegister.DimensionFields,
                                Field = dimensionFields,
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
                Dictionary<string, ConfigurationField> AllFields = Conf.CombineAllFieldForRegister
                (
                    ConfRegister.DimensionFields.Values,
                    ConfRegister.ResourcesFields.Values,
                    ConfRegister.PropertyFields.Values
                );

                PageField page = new PageField()
                {
                    AllFields = AllFields,
                    Fields = ConfRegister.DimensionFields,
                    IsNew = true,
                    GeneralForm = GeneralForm,
                    Table = ConfRegister.Table,
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
                    if (ConfRegister.DimensionFields.TryGetValue(row.Child.Name, out ConfigurationField? dimensionFields))
                    {
                        ConfigurationField newField = dimensionFields.Copy();
                        newField.Name += GenerateName.GetNewName();

                        ConfRegister.AppendDimensionField(newField);
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
        }

        void OnDimensionFieldsRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxDimensionFields.SelectedRows;
            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                    ConfRegister.DimensionFields.Remove(row.Child.Name);

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
                    if (ConfRegister.ResourcesFields.TryGetValue(curRow.Child.Name, out ConfigurationField? resourcesField))
                        GeneralForm?.CreateNotebookPage($"Поле: {curRow.Child.Name}", () =>
                        {
                            Dictionary<string, ConfigurationField> AllFields = Conf.CombineAllFieldForRegister
                            (
                                ConfRegister.DimensionFields.Values,
                                ConfRegister.ResourcesFields.Values,
                                ConfRegister.PropertyFields.Values
                            );

                            PageField page = new PageField()
                            {
                                AllFields = AllFields,
                                Fields = ConfRegister.ResourcesFields,
                                Field = resourcesField,
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
                Dictionary<string, ConfigurationField> AllFields = Conf.CombineAllFieldForRegister
                (
                    ConfRegister.DimensionFields.Values,
                    ConfRegister.ResourcesFields.Values,
                    ConfRegister.PropertyFields.Values
                );

                PageField page = new PageField()
                {
                    AllFields = AllFields,
                    Fields = ConfRegister.ResourcesFields,
                    IsNew = true,
                    GeneralForm = GeneralForm,
                    Table = ConfRegister.Table,
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
                    if (ConfRegister.ResourcesFields.TryGetValue(row.Child.Name, out ConfigurationField? resourcesField))
                    {
                        ConfigurationField newField = resourcesField.Copy();
                        newField.Name += GenerateName.GetNewName();

                        ConfRegister.AppendResourcesField(newField);
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
        }

        void OnResourcesFieldsRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxResourcesFields.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                    ConfRegister.ResourcesFields.Remove(row.Child.Name);

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
                    if (ConfRegister.PropertyFields.TryGetValue(curRow.Child.Name, out ConfigurationField? propertyField))
                        GeneralForm?.CreateNotebookPage($"Поле: {curRow.Child.Name}", () =>
                        {
                            Dictionary<string, ConfigurationField> AllFields = Conf.CombineAllFieldForRegister
                            (
                                ConfRegister.DimensionFields.Values,
                                ConfRegister.ResourcesFields.Values,
                                ConfRegister.PropertyFields.Values
                            );

                            PageField page = new PageField()
                            {
                                AllFields = AllFields,
                                Fields = ConfRegister.PropertyFields,
                                Field = propertyField,
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
                Dictionary<string, ConfigurationField> AllFields = Conf.CombineAllFieldForRegister
                 (
                     ConfRegister.DimensionFields.Values,
                     ConfRegister.ResourcesFields.Values,
                     ConfRegister.PropertyFields.Values
                 );

                PageField page = new PageField()
                {
                    AllFields = AllFields,
                    Fields = ConfRegister.PropertyFields,
                    IsNew = true,
                    GeneralForm = GeneralForm,
                    Table = ConfRegister.Table,
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
                    if (ConfRegister.PropertyFields.TryGetValue(row.Child.Name, out ConfigurationField? propertyFields))
                    {
                        ConfigurationField newField = propertyFields.Copy();
                        newField.Name += GenerateName.GetNewName();

                        ConfRegister.AppendPropertyField(newField);
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
        }

        void OnPropertyFieldsRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxPropertyFields.SelectedRows;
            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                    ConfRegister.PropertyFields.Remove(row.Child.Name);

                PropertyFieldsRefreshList();
                GeneralForm?.LoadTreeAsync();
            }
        }

        void PropertyFieldsRefreshList()
        {
            OnPropertyFieldsRefreshClick(null, new EventArgs());
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

                    if (ConfRegister.TabularList.TryGetValue(curRow.Child.Name, out ConfigurationTabularList? tabularList))
                        GeneralForm?.CreateNotebookPage($"Табличний список: {curRow.Child.Name}", () =>
                        {
                            Dictionary<string, ConfigurationField> AllFields = Conf.CombineAllFieldForRegister
                            (
                                ConfRegister.DimensionFields.Values,
                                ConfRegister.ResourcesFields.Values,
                                ConfRegister.PropertyFields.Values
                            );

                            PageTabularList page = new PageTabularList()
                            {
                                Fields = AllFields,
                                TabularLists = ConfRegister.TabularList,
                                TabularList = tabularList,
                                IsNew = false,
                                GeneralForm = GeneralForm,
                                CallBack_RefreshList = TabularListRefreshList,
                                ConfOwnerName = "РегістриВідомостей"
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
                Dictionary<string, ConfigurationField> AllFields = Conf.CombineAllFieldForRegister
                (
                    ConfRegister.DimensionFields.Values,
                    ConfRegister.ResourcesFields.Values,
                    ConfRegister.PropertyFields.Values
                );

                PageTabularList page = new PageTabularList()
                {
                    Fields = AllFields,
                    TabularLists = ConfRegister.TabularList,
                    IsNew = true,
                    GeneralForm = GeneralForm,
                    CallBack_RefreshList = TabularListRefreshList,
                    ConfOwnerName = "РегістриВідомостей"
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
                    if (ConfRegister.TabularList.TryGetValue(row.Child.Name, out ConfigurationTabularList? tabularList))
                    {
                        ConfigurationTabularList newTableList = tabularList.Copy();
                        newTableList.Name += GenerateName.GetNewName();

                        ConfRegister.AppendTableList(newTableList);
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
        }

        void OnTabularListRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxTabularList.SelectedRows;
            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                    ConfRegister.TabularList.Remove(row.Child.Name);

                TabularListRefreshList();
                GeneralForm?.LoadTreeAsync();
            }
        }

        void TabularListRefreshList()
        {
            OnTabularListRefreshClick(null, new EventArgs());
        }

        #endregion

        #region FormsList

        void OnFormsListButtonPress(object? sender, ButtonPressEventArgs args)
        {
            if (args.Event.Type == Gdk.EventType.DoubleButtonPress)
            {
                ListBoxRow[] selectedRows = listBoxFormsList.SelectedRows;
                if (selectedRows.Length != 0)
                {
                    ListBoxRow curRow = selectedRows[0];
                    if (ConfRegister.Forms.TryGetValue(curRow.Child.Name, out ConfigurationForms? form))
                        GeneralForm?.CreateNotebookPage($"Форма: {curRow.Child.Name}", () =>
                        {
                            Dictionary<string, ConfigurationField> AllFields = Conf.CombineAllFieldForRegister
                            (
                                ConfRegister.DimensionFields.Values,
                                ConfRegister.ResourcesFields.Values,
                                ConfRegister.PropertyFields.Values
                            );

                            PageForm page = new PageForm()
                            {
                                ParentName = ConfRegister.Name,
                                ParentType = "RegisterInformation",
                                Forms = ConfRegister.Forms,
                                Form = form,
                                TypeForm = form.Type,
                                Fields = AllFields,
                                TabularLists = ConfRegister.TabularList,
                                TabularList = form.TabularList,
                                IsNew = false,
                                GeneralForm = GeneralForm,
                                CallBack_RefreshList = FormsListRefreshList
                            };

                            page.SetValue();
                            return page;
                        });
                }
            }
        }

        Menu OnFormsListAddFormSubMenu()
        {
            //Внутрішня функція для субменю
            void OnFormsListAdd(ConfigurationForms.TypeForms typeForms)
            {
                if (string.IsNullOrEmpty(entryName.Text))
                {
                    Message.Error(GeneralForm, "Назва регістру не вказана");
                    return;
                }

                GeneralForm?.CreateNotebookPage("Форма *", () =>
                {
                    Dictionary<string, ConfigurationField> AllFields = Conf.CombineAllFieldForRegister
                    (
                        ConfRegister.DimensionFields.Values,
                        ConfRegister.ResourcesFields.Values,
                        ConfRegister.PropertyFields.Values
                    );

                    PageForm page = new PageForm()
                    {
                        ParentName = ConfRegister.Name,
                        ParentType = "RegisterInformation",
                        Forms = ConfRegister.Forms,
                        TypeForm = typeForms,
                        Fields = AllFields,
                        TabularLists = ConfRegister.TabularList,
                        IsNew = true,
                        GeneralForm = GeneralForm,
                        CallBack_RefreshList = FormsListRefreshList,
                    };

                    page.SetValue();
                    return page;
                });
            }

            Menu Menu = new Menu();

            {
                MenuItem item = new MenuItem("Елемент");
                item.Activated += (object? sender, EventArgs args) => { OnFormsListAdd(ConfigurationForms.TypeForms.Element); };
                Menu.Append(item);
            }

            {
                MenuItem item = new MenuItem("Список");
                item.Activated += (object? sender, EventArgs args) => { OnFormsListAdd(ConfigurationForms.TypeForms.List); };
                Menu.Append(item);
            }

            Menu.ShowAll();

            return Menu;
        }

        void OnFormsListCopyClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxFormsList.SelectedRows;
            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                    if (ConfRegister.Forms.TryGetValue(row.Child.Name, out ConfigurationForms? form))
                    {
                        ConfigurationForms newForms = form.Copy();
                        newForms.Name += GenerateName.GetNewName();

                        ConfRegister.AppendForms(newForms);
                    }

                FormsListRefreshList();
            }
        }

        void OnFormsListRefreshClick(object? sender, EventArgs args)
        {
            foreach (Widget item in listBoxFormsList.Children)
                listBoxFormsList.Remove(item);

            FillFormsList();
        }

        void OnFormsListRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxFormsList.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                    ConfRegister.Forms.Remove(row.Child.Name);

                FormsListRefreshList();
            }
        }

        void FormsListRefreshList()
        {
            OnFormsListRefreshClick(null, new EventArgs());
        }

        #endregion
    }
}