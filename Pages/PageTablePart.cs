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

            //Генерування коду 
            {
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
            }

            hPaned.Pack1(vBox, false, false);
        }

        #region Присвоєння / зчитування значень віджетів

        public async void SetValue()
        {
            FillTabularParts();

            entryName.Text = TablePart.Name;

            if (IsNew)
                entryTable.Text = await Configuration.GetNewUnigueTableName(Program.Kernel);
            else
                entryTable.Text = TablePart.Table;

            textViewDesc.Buffer.Text = TablePart.Desc;
        }

        void FillTabularParts()
        {
            foreach (ConfigurationField field in TablePart.Fields.Values)
                listBoxFields.Add(new Label(field.Name) { Name = field.Name, Halign = Align.Start });

            listBoxFields.ShowAll();
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

        #region Генерування коду

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

        #endregion
    }
}