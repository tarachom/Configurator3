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
using System.Xml;

namespace Configurator
{
    class PageTablePart : VBox
    {
        Configuration Conf { get { return Program.Kernel.Conf; } }

        public Dictionary<string, ConfigurationTablePart> TabularParts { get; set; } = new Dictionary<string, ConfigurationTablePart>();
        public ConfigurationTablePart TablePart { get; set; } = new ConfigurationTablePart();
        public FormConfigurator? GeneralForm { get; set; }
        public System.Action? CallBack_RefreshList { get; set; }
        public bool IsNew { get; set; } = true;

        #region Fields

        ListBox listBoxFields = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxTabularList = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxFormsList = new ListBox() { SelectionMode = SelectionMode.Single };

        Entry entryName = new Entry() { WidthRequest = 500 };
        Entry entryTable = new Entry() { WidthRequest = 500 };
        TextView textViewDesc = new TextView() { WrapMode = WrapMode.Word };

        #endregion

        public PageTablePart() : base()
        {
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

            //Базові поля
            {
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
            }

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

            //Генерування коду 
            /*{
                Expander expanderTemplates = new Expander("Генерування коду");
                vBox.PackStart(expanderTemplates, false, false, 5);

                VBox vBoxTemplates = new VBox();
                expanderTemplates.Add(vBoxTemplates);

                //Заголовок
                HBox hBoxInfo = new HBox() { Halign = Align.Start };
                vBoxTemplates.PackStart(hBoxInfo, false, false, 5);
                hBoxInfo.PackStart(new Label("Таблична частина") { UseMarkup = true, Selectable = true }, false, false, 5);

                //Таблична частина
                HBox hBox = new HBox() { Halign = Align.Start };
                vBoxTemplates.PackStart(hBox, false, false, 5);
                {
                    Button buttonConstructorElement = new Button("Таблична частина");
                    hBox.PackStart(buttonConstructorElement, false, false, 5);
                    buttonConstructorElement.Clicked += (object? sender, EventArgs args) => { GenerateCode((Widget)sender!, "TablePart"); };
                }
            }*/

            hPaned.Pack1(vBox, false, false);
        }

        void CreatePack2(HPaned hPaned)
        {
            VBox vBox = new VBox();

            HBox hBox = new HBox();
            hBox.PackStart(new Label("Поля:"), false, false, 5);
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
            scrollList.SetSizeRequest(0, 500);

            listBoxFields.ButtonPressEvent += OnTabularPartsButtonPress;

            scrollList.Add(listBoxFields);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);
            hPaned.Pack2(vBox, true, false);
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

        #region Присвоєння / зчитування значень віджетів

        public async void SetValue()
        {
            entryName.Text = TablePart.Name;

            if (IsNew)
                entryTable.Text = await Configuration.GetNewUnigueTableName(Program.Kernel);
            else
                entryTable.Text = TablePart.Table;

            textViewDesc.Buffer.Text = TablePart.Desc;

            FillTabularParts();
            FillTabularList();
            FillFormsList();
        }

        void FillTabularParts()
        {
            foreach (ConfigurationField field in TablePart.Fields.Values)
                listBoxFields.Add(new Label(field.Name) { Name = field.Name, Halign = Align.Start });

            listBoxFields.ShowAll();
        }

        void FillTabularList()
        {
            foreach (ConfigurationTabularList tableList in TablePart.TabularList.Values)
                listBoxTabularList.Add(new Label(tableList.Name) { Name = tableList.Name, Halign = Align.Start });

            listBoxTabularList.ShowAll();
        }

        void FillFormsList()
        {
            foreach (ConfigurationForms form in TablePart.Forms.Values)
                listBoxFormsList.Add(new Label(form.Name) { Name = form.Name, Halign = Align.Start });

            listBoxFormsList.ShowAll();
        }

        void GetValue()
        {
            TablePart.Name = entryName.Text;
            TablePart.Table = entryTable.Text;
            TablePart.Desc = textViewDesc.Buffer.Text;
        }

        #endregion

        void OnSaveClick(object? sender, EventArgs args)
        {
            string name = entryName.Text;
            string errorList = Configuration.ValidateConfigurationObjectName(Program.Kernel, ref name);
            entryName.Text = name;

            if (errorList.Length > 0)
            {
                Message.Error(GeneralForm, $"{errorList}");
                return;
            }

            if (IsNew)
            {
                if (TabularParts.ContainsKey(entryName.Text))
                {
                    Message.Error(GeneralForm, $"Назва табличної частини не унікальна");
                    return;
                }
            }
            else
            {
                if (TablePart.Name != entryName.Text)
                {
                    if (TabularParts.ContainsKey(entryName.Text))
                    {
                        Message.Error(GeneralForm, $"Назва табличної частини не унікальна");
                        return;
                    }
                }

                TabularParts.Remove(TablePart.Name);
            }

            GetValue();

            TabularParts.Add(TablePart.Name, TablePart);

            IsNew = false;

            GeneralForm?.LoadTreeAsync();
            GeneralForm?.RenameCurrentPageNotebook($"Таблична частина: {TablePart.Name}");

            CallBack_RefreshList?.Invoke();
        }

        #region OnTabularParts
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
            GeneralForm?.CreateNotebookPage("Поле: *", () =>
            {
                PageField page = new PageField()
                {
                    Table = TablePart.Table,
                    Fields = TablePart.Fields,
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
            ListBoxRow[] selectedRows = listBoxFields.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (TablePart.Fields.ContainsKey(row.Child.Name))
                    {
                        ConfigurationField newField = TablePart.Fields[row.Child.Name].Copy();
                        newField.NameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, TablePart.Table, TablePart.Fields);
                        newField.Name += GenerateName.GetNewName();

                        TablePart.AppendField(newField);
                    }
                }

                OnTabularPartsRefreshClick(null, new EventArgs());

                GeneralForm?.LoadTreeAsync();
            }
        }

        void OnTabularPartsRefreshClick(object? sender, EventArgs args)
        {
            foreach (Widget item in listBoxFields.Children)
                listBoxFields.Remove(item);

            FillTabularParts();
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
                    if (TablePart.TabularList.TryGetValue(curRow.Child.Name, out ConfigurationTabularList? tableList))
                        GeneralForm?.CreateNotebookPage($"Табличний список: {curRow.Child.Name}", () =>
                        {
                            PageTabularList page = new PageTabularList()
                            {
                                Fields = TablePart.Fields,
                                TabularLists = TablePart.TabularList,
                                TabularList = tableList,
                                IsNew = false,
                                GeneralForm = GeneralForm,
                                CallBack_RefreshList = TabularListRefreshList,
                                ConfOwnerName = "Довідники"
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
                    Fields = TablePart.Fields,
                    TabularLists = TablePart.TabularList,
                    IsNew = true,
                    GeneralForm = GeneralForm,
                    CallBack_RefreshList = TabularListRefreshList,
                    ConfOwnerName = "Довідники"
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
                    if (TablePart.TabularList.TryGetValue(row.Child.Name, out ConfigurationTabularList? tableList))
                    {
                        ConfigurationTabularList newTableList = tableList.Copy();
                        newTableList.Name += GenerateName.GetNewName();

                        TablePart.AppendTableList(newTableList);
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
                    TablePart.TabularList.Remove(row.Child.Name);

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

                    if (TablePart.Forms.TryGetValue(curRow.Child.Name, out ConfigurationForms? form))
                        GeneralForm?.CreateNotebookPage($"Форма: {curRow.Child.Name}", () =>
                        {
                            PageForm page = new PageForm()
                            {
                                ParentName = TablePart.Name,
                                ParentType = "TablePart",
                                Forms = TablePart.Forms,
                                Form = TablePart.Forms[curRow.Child.Name],
                                TypeForm = TablePart.Forms[curRow.Child.Name].Type,
                                Fields = TablePart.Fields,
                                TabularLists = TablePart.TabularList,
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
                    Message.Error(GeneralForm, "Назва довідника не вказана");
                    return;
                }

                GeneralForm?.CreateNotebookPage("Форма *", () =>
                {
                    PageForm page = new PageForm()
                    {
                        ParentName = TablePart.Name,
                        ParentType = "TablePart",
                        Forms = TablePart.Forms,
                        TypeForm = typeForms,
                        Fields = TablePart.Fields,
                        TabularLists = TablePart.TabularList,
                        IsNew = true,
                        GeneralForm = GeneralForm,
                        CallBack_RefreshList = FormsListRefreshList
                    };

                    page.SetValue();

                    return page;
                });
            }

            Menu Menu = new Menu();

            {
                MenuItem item = new MenuItem("Таблична частина");
                item.Activated += (object? sender, EventArgs args) => { OnFormsListAdd(ConfigurationForms.TypeForms.TablePart); };
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
                {
                    if (TablePart.Forms.ContainsKey(row.Child.Name))
                    {
                        ConfigurationForms newForms = TablePart.Forms[row.Child.Name].Copy();
                        newForms.Name += GenerateName.GetNewName();

                        TablePart.AppendForms(newForms);
                    }
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
                    if (TablePart.Forms.ContainsKey(row.Child.Name))
                        TablePart.Forms.Remove(row.Child.Name);

                FormsListRefreshList();
            }
        }

        void FormsListRefreshList()
        {
            OnFormsListRefreshClick(null, new EventArgs());
        }

        #endregion

        /* #region Генерування коду

        void GenerateCode(Widget relative_to, string fileName)
        {
            if (string.IsNullOrEmpty(entryName.Text))
            {
                Message.Error(GeneralForm, "Назва табличної частини не вказана");
                return;
            }

            if (!TabularParts.ContainsKey(entryName.Text))
            {
                Message.Error(GeneralForm, "Таблична частина не збережена в колекцію, потрібно спочатку зберегти");
                return;
            }

            XmlDocument xmlConfDocument = new XmlDocument();
            xmlConfDocument.AppendChild(xmlConfDocument.CreateXmlDeclaration("1.0", "utf-8", ""));

            XmlElement rootNode = xmlConfDocument.CreateElement("root");
            xmlConfDocument.AppendChild(rootNode);

            XmlElement nodeTablePart = xmlConfDocument.CreateElement("TablePart");
            rootNode.AppendChild(nodeTablePart);

            XmlElement nodeTablePartName = xmlConfDocument.CreateElement("Name");
            nodeTablePartName.InnerText = TablePart.Name;
            nodeTablePart.AppendChild(nodeTablePartName);

            Configuration.SaveFields(TablePart.Fields, xmlConfDocument, nodeTablePart, "TablePart");

            TextView textViewCode = new TextView();

            ScrolledWindow scrollCode = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 600, HeightRequest = 300 };
            scrollCode.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollCode.Add(textViewCode);

            Popover popover = new Popover(relative_to) { BorderWidth = 5 };
            popover.Add(scrollCode);
            popover.ShowAll();

            textViewCode.Buffer.Text = Configuration.Transform
            (
                xmlConfDocument,
                System.IO.Path.Combine(AppContext.BaseDirectory, "xslt/ConstructorTablePart.xslt"),
                new Dictionary<string, object> { { "File", fileName } }
            );

            textViewCode.Buffer.SelectRange(textViewCode.Buffer.StartIter, textViewCode.Buffer.EndIter);
            textViewCode.GrabFocus();
        }

        #endregion */
    }
}