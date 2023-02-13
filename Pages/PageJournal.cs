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
    class PageJournal : VBox
    {
        Configuration? Conf
        {
            get
            {
                return Program.Kernel?.Conf;
            }
        }

        public ConfigurationJournals ConfJournals { get; set; } = new ConfigurationJournals();
        public FormConfigurator? GeneralForm { get; set; }
        public bool IsNew { get; set; } = true;

        #region Fields

        ListBox listBoxFields = new ListBox() { SelectionMode = SelectionMode.Single };
        Entry entryName = new Entry() { WidthRequest = 500 };
        TextView textViewDesc = new TextView();

        #endregion

        public PageJournal() : base()
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

        void CreatePack2(HPaned hPaned)
        {
            VBox vBox = new VBox();

            //Поля
            CreateFieldList(vBox);

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

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            FillFields();

            entryName.Text = ConfJournals.Name;
            textViewDesc.Buffer.Text = ConfJournals.Desc;
        }

        void FillFields()
        {
            foreach (ConfigurationJournalField field in ConfJournals.Fields.Values)
                listBoxFields.Add(new Label($"{field.Name}") { Name = field.Name, Halign = Align.Start });
        }

        void GetValue()
        {
            ConfJournals.Name = entryName.Text;
            ConfJournals.Desc = textViewDesc.Buffer.Text;
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
                if (Conf!.Enums.ContainsKey(entryName.Text))
                {
                    Message.Error(GeneralForm, $"Назва журналу не унікальна");
                    return;
                }
            }
            else
            {
                if (ConfJournals.Name != entryName.Text)
                {
                    if (Conf!.Journals.ContainsKey(entryName.Text))
                    {
                        Message.Error(GeneralForm, $"Назва журналу не унікальна");
                        return;
                    }
                }

                Conf!.Journals.Remove(ConfJournals.Name);
            }

            GetValue();

            Conf!.AppendJournal(ConfJournals);

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

                    if (ConfJournals.Fields.ContainsKey(curRow.Child.Name))
                        GeneralForm?.CreateNotebookPage($"Поле: {curRow.Child.Name}", () =>
                        {
                            PageJournalField page = new PageJournalField()
                            {
                                Fields = ConfJournals.Fields,
                                Field = ConfJournals.Fields[curRow.Child.Name],
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
                {
                    if (ConfJournals.Fields.ContainsKey(row.Child.Name))
                    {
                        ConfigurationJournalField newField = ConfJournals.Fields[row.Child.Name].Copy();
                        newField.Name += GenerateName.GetNewName();

                        ConfJournals.AppendField(newField);
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
                    if (ConfJournals.Fields.ContainsKey(row.Child.Name))
                        ConfJournals.Fields.Remove(row.Child.Name);
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
    }
}