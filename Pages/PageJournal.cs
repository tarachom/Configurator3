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
    class PageJournal : Box
    {
        Configuration Conf { get { return Program.Kernel.Conf; } }

        public ConfigurationJournals ConfJournals { get; set; } = new ConfigurationJournals();
        public FormConfigurator? GeneralForm { get; set; }
        public bool IsNew { get; set; } = true;

        #region Fields

        ListBox listBoxFields = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxDocuments = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxTabularList = new ListBox() { SelectionMode = SelectionMode.Single };
        Entry entryName = new Entry() { WidthRequest = 500 };
        TextView textViewDesc = new TextView() { WrapMode = WrapMode.Word };

        #endregion

        public PageJournal() : base(Orientation.Vertical, 0)
        {
            Box hBox = new Box(Orientation.Horizontal, 0);

            Button bSave = new Button("Зберегти");
            bSave.Clicked += OnSaveClick;

            hBox.PackStart(bSave, false, false, 10);

            Button bClose = new Button("Закрити");
            bClose.Clicked += (object? sender, EventArgs args) => GeneralForm?.CloseCurrentPageNotebook();

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

            //Опис
            Box hBoxDesc = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxDesc, false, false, 5);

            hBoxDesc.PackStart(new Label("Опис:") { Valign = Align.Start }, false, false, 5);

            ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 500, HeightRequest = 100 };
            scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollTextView.Add(textViewDesc);

            hBoxDesc.PackStart(scrollTextView, false, false, 5);

            //Документи
            {
                Expander expanderDocuments = new Expander("Документи які входять в журнал");
                vBox.PackStart(expanderDocuments, false, false, 5);

                Box vBoxDocument = new Box(Orientation.Vertical, 0);
                expanderDocuments.Add(vBoxDocument);

                Box hBoxDocument = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
                vBoxDocument.PackStart(hBoxDocument, false, false, 5);

                ScrolledWindow scrollAllowList = new ScrolledWindow() { ShadowType = ShadowType.In };
                scrollAllowList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
                scrollAllowList.SetSizeRequest(500, 500);

                scrollAllowList.Add(listBoxDocuments);
                hBoxDocument.PackStart(scrollAllowList, true, true, 5);

                //Заповнити
                Box hBoxFillDoc = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
                vBoxDocument.PackStart(hBoxFillDoc, false, false, 5);

                Button bFillDoc = new Button("Заповнити список налаштування");
                bFillDoc.Clicked += OnFillDocumentsClick;

                hBoxFillDoc.PackStart(bFillDoc, false, false, 10);
            }

            //Списки та форми
            {
                Expander expanderForm = new Expander("Налаштування для документів") { Expanded = true };
                vBox.PackStart(expanderForm, false, false, 5);

                Box vBoxForm = new Box(Orientation.Vertical, 0);
                expanderForm.Add(vBoxForm);

                //Заголовок блоку Forms
                Box hBoxInterfaceCreateInfo = new Box(Orientation.Horizontal, 0) { Halign = Align.Center };
                vBoxForm.PackStart(hBoxInterfaceCreateInfo, false, false, 5);
                hBoxInterfaceCreateInfo.PackStart(new Label("Документи"), false, false, 5);

                //Табличні списки
                CreateTabularList(vBoxForm);
            }

            hPaned.Pack1(vBox, false, false);
        }

        void CreatePack2(Paned hPaned)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            //Поля
            CreateFieldList(vBox);

            hPaned.Pack2(vBox, true, false);
        }

        void CreateFieldList(Box vBoxContainer)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            Box hBox = new Box(Orientation.Horizontal, 0);
            hBox.PackStart(new Label("Поля:"), false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(new Image(Stock.New, IconSize.Menu), "Додати") { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnFieldsAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(new Image(Stock.Copy, IconSize.Menu), "Копіювати") { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnFieldsCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(new Image(Stock.Refresh, IconSize.Menu), "Обновити") { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnFieldsRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(new Image(Stock.Clear, IconSize.Menu), "Видалити") { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnFieldsRemoveClick;
            toolbar.Add(buttonDelete);

            Box hBoxScroll = new Box(Orientation.Horizontal, 0);
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 400);

            listBoxFields.ButtonPressEvent += OnFieldsButtonPress;

            scrollList.Add(listBoxFields);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            vBoxContainer.PackStart(vBox, false, false, 0);
        }

        void CreateTabularList(Box vBoxContainer)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonDelete = new ToolButton(new Image(Stock.Clear, IconSize.Menu), "Видалити") { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnTabularListRemoveClick;
            toolbar.Add(buttonDelete);

            Box hBoxScroll = new Box(Orientation.Horizontal, 0);
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 500);

            listBoxTabularList.ButtonPressEvent += OnTabularListButtonPress;

            scrollList.Add(listBoxTabularList);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            vBoxContainer.PackStart(vBox, false, false, 0);
        }

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            FillFields();
            FillDocuments();
            FillTabularList();

            entryName.Text = ConfJournals.Name;
            textViewDesc.Buffer.Text = ConfJournals.Desc;
        }

        void FillFields()
        {
            foreach (ConfigurationJournalField field in ConfJournals.Fields.Values)
                listBoxFields.Add(new Label($"{field.Name}" + (field.SortField ? " [Order]" : "") + (field.WherePeriod ? " [WherePeriod]" : "")) { Name = field.Name, Halign = Align.Start, UseUnderline = false });

            listBoxFields.ShowAll();
        }

        void FillDocuments()
        {
            foreach (ConfigurationDocuments doc in Conf.Documents.Values)
                listBoxDocuments.Add(
                    new CheckButton(doc.Name)
                    {
                        Name = doc.Name,
                        Active = ConfJournals.AllowDocuments.Contains(doc.Name)
                    });

            listBoxDocuments.ShowAll();
        }

        void FillTabularList()
        {
            foreach (ConfigurationTabularList tableList in ConfJournals.TabularList.Values)
                listBoxTabularList.Add(new Label(tableList.Name) { Name = tableList.Name, Halign = Align.Start, UseUnderline = false });

            listBoxTabularList.ShowAll();
        }

        void GetValue()
        {
            ConfJournals.Name = entryName.Text;
            ConfJournals.Desc = textViewDesc.Buffer.Text;

            //Доспупні документи
            ConfJournals.AllowDocuments.Clear();

            foreach (ListBoxRow item in listBoxDocuments.Children)
            {
                CheckButton cb = (CheckButton)item.Child;
                if (cb.Active)
                    ConfJournals.AllowDocuments.Add(cb.Name);
            }
        }

        #endregion

        void OnFillDocumentsClick(object? sender, EventArgs args)
        {
            //Ключі
            List<string> Keys = ConfJournals.TabularList.Keys.ToList();

            foreach (ListBoxRow item in listBoxDocuments.Children)
            {
                CheckButton cb = (CheckButton)item.Child;
                if (cb.Active)
                    if (!Keys.Contains(cb.Name))
                        ConfJournals.AppendTableList(new ConfigurationTabularList(cb.Name, ""));
            }

            TabularListRefreshList();
        }

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
                if (Conf.Enums.ContainsKey(entryName.Text))
                {
                    Message.Error(GeneralForm, $"Назва журналу не унікальна");
                    return;
                }
            }
            else
            {
                if (ConfJournals.Name != entryName.Text)
                {
                    if (Conf.Journals.ContainsKey(entryName.Text))
                    {
                        Message.Error(GeneralForm, $"Назва журналу не унікальна");
                        return;
                    }
                }

                Conf.Journals.Remove(ConfJournals.Name);
            }

            GetValue();

            Conf.AppendJournal(ConfJournals);

            IsNew = false;

            GeneralForm?.LoadTreeAsync();
            GeneralForm?.RenameCurrentPageNotebook($"Журнал: {ConfJournals.Name}");
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
                    if (ConfJournals.Fields.TryGetValue(curRow.Child.Name, out ConfigurationJournalField? field))
                        GeneralForm?.CreateNotebookPage($"Поле: {curRow.Child.Name}", () =>
                        {
                            PageJournalField page = new PageJournalField()
                            {
                                Fields = ConfJournals.Fields,
                                Field = field,
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
                PageJournalField page = new PageJournalField()
                {
                    Fields = ConfJournals.Fields,
                    Field = new ConfigurationJournalField(),
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
                    if (ConfJournals.Fields.TryGetValue(row.Child.Name, out ConfigurationJournalField? field))
                    {
                        ConfigurationJournalField newField = field.Copy();
                        newField.Name += GenerateName.GetNewName();

                        ConfJournals.AppendField(newField);
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
        }

        void OnFieldsRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxFields.SelectedRows;
            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                    ConfJournals.Fields.Remove(row.Child.Name);

                FieldsRefreshList();
                GeneralForm?.LoadTreeAsync();
            }
        }

        void FieldsRefreshList()
        {
            OnFieldsRefreshClick(null, new EventArgs());
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
                    if (ConfJournals.TabularList.TryGetValue(curRow.Child.Name, out ConfigurationTabularList? tabularList))
                        GeneralForm?.CreateNotebookPage($"Налаштування: {curRow.Child.Name}", () =>
                        {
                            PageJournalTabularList page = new PageJournalTabularList()
                            {
                                Fields = ConfJournals.Fields,
                                TabularLists = ConfJournals.TabularList,
                                TabularList = tabularList,
                                GeneralForm = GeneralForm,
                                CallBack_RefreshList = TabularListRefreshList
                            };

                            page.SetValue();
                            return page;
                        });
                }
            }
        }

        void OnTabularListRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxTabularList.SelectedRows;
            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                    ConfJournals.TabularList.Remove(row.Child.Name);

                TabularListRefreshList();
                GeneralForm?.LoadTreeAsync();
            }
        }

        void TabularListRefreshList()
        {
            foreach (Widget item in listBoxTabularList.Children)
                listBoxTabularList.Remove(item);

            FillTabularList();
        }

        #endregion

    }
}