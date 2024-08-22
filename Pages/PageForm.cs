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
using GtkSource;

using System.Xml;

namespace Configurator
{
    class PageForm : Box
    {
        Configuration Conf { get { return Program.Kernel.Conf; } }
        public string ParentName { get; set; } = "";
        public string ParentType { get; set; } = "";
        public Dictionary<string, ConfigurationField> Fields = [];
        public Dictionary<string, ConfigurationTablePart> TabularParts = [];
        public Dictionary<string, ConfigurationTabularList> TabularLists = [];
        public string TabularList { get; set; } = "";
        public Dictionary<string, ConfigurationForms> Forms { get; set; } = [];
        public ConfigurationForms Form { get; set; } = new ConfigurationForms();
        public FormConfigurator? GeneralForm { get; set; }
        public System.Action? CallBack_RefreshList { get; set; }
        public bool IsNew { get; set; } = true;
        public ConfigurationForms.TypeForms TypeForm { get; set; } = ConfigurationForms.TypeForms.None;

        #region Fields

        Label labelType = new Label();
        Entry entryName = new Entry() { WidthRequest = 250 };
        TextView textViewDesc = new TextView() { WrapMode = WrapMode.Word };
        Notebook notebook = new Notebook()
        {
            Scrollable = true,
            EnablePopup = true,
            BorderWidth = 0,
            ShowBorder = false,
            TabPos = PositionType.Top
        };
        SourceView sourceViewCode = new SourceView() { ShowLineNumbers = true };

        #endregion

        #region FormList

        ComboBoxText сomboBoxFormListTabularList = new ComboBoxText();

        #endregion

        #region FormElementField / Поля для форми елемента

        TreeView treeViewFormElementField;
        ListStore listStoreFormElementField = new ListStore(
            typeof(bool),   //Visible
            typeof(string), //Name
            typeof(string)  //Caption
        );
        enum FormElementFieldColumns
        {
            Visible,
            Name,
            Caption,
        }

        #endregion

        #region FormElementTablePart / Табличні частини для форми елемента

        TreeView treeViewFormElementTablePart;
        ListStore listStoreFormElementTablePart = new ListStore(
            typeof(bool),   //Visible
            typeof(string), //Name
            typeof(string)  //Caption
        );
        enum FormElementTablePartColumns
        {
            Visible,
            Name,
            Caption,
        }

        #endregion

        public PageForm() : base(Orientation.Vertical, 0)
        {
            Box hBox = new Box(Orientation.Horizontal, 0);

            Button bSave = new Button("Зберегти");
            bSave.Clicked += OnSaveClick;

            hBox.PackStart(bSave, false, false, 10);

            Button bClose = new Button("Закрити");
            bClose.Clicked += (object? sender, EventArgs args) => { GeneralForm?.CloseCurrentPageNotebook(); };

            hBox.PackStart(bClose, false, false, 10);

            PackStart(hBox, false, false, 10);

            treeViewFormElementField = new TreeView(listStoreFormElementField);
            AddColumnTreeViewFormElementField();

            treeViewFormElementTablePart = new TreeView(listStoreFormElementTablePart);
            AddColumnTreeViewFormElementTablePart();

            Paned hPaned = new Paned(Orientation.Horizontal) { BorderWidth = 5 };

            CreatePack1(hPaned);
            CreatePack2(hPaned);

            PackStart(hPaned, true, true, 5);

            ShowAll();
        }

        #region TreeView

        void AddColumnTreeViewFormElementField()
        {
            //Visible
            {
                CellRendererToggle cell = new CellRendererToggle();
                cell.Toggled += (object o, ToggledArgs args) =>
                {
                    if (listStoreFormElementField.GetIterFromString(out TreeIter iter, args.Path))
                    {
                        bool val = (bool)listStoreFormElementField.GetValue(iter, (int)FormElementFieldColumns.Visible);
                        listStoreFormElementField.SetValue(iter, (int)FormElementFieldColumns.Visible, !val);
                    }
                };
                treeViewFormElementField.AppendColumn(new TreeViewColumn("", cell, "active", FormElementFieldColumns.Visible));
            }

            //Name
            treeViewFormElementField.AppendColumn(new TreeViewColumn("Назва", new CellRendererText(), "text", FormElementFieldColumns.Name));

            //Caption
            {
                CellRendererText cell = new CellRendererText() { Editable = true };
                cell.Edited += (object o, EditedArgs args) =>
                {
                    if (listStoreFormElementField.GetIterFromString(out TreeIter iter, args.Path))
                        listStoreFormElementField.SetValue(iter, (int)FormElementFieldColumns.Caption, args.NewText);
                };
                treeViewFormElementField.AppendColumn(new TreeViewColumn("Заголовок", cell, "text", FormElementFieldColumns.Caption));
            }
        }

        void AddColumnTreeViewFormElementTablePart()
        {
            //Visible
            {
                CellRendererToggle cell = new CellRendererToggle();
                cell.Toggled += (object o, ToggledArgs args) =>
                {
                    if (listStoreFormElementTablePart.GetIterFromString(out TreeIter iter, args.Path))
                    {
                        bool val = (bool)listStoreFormElementTablePart.GetValue(iter, (int)FormElementTablePartColumns.Visible);
                        listStoreFormElementTablePart.SetValue(iter, (int)FormElementTablePartColumns.Visible, !val);
                    }
                };
                treeViewFormElementTablePart.AppendColumn(new TreeViewColumn("", cell, "active", FormElementTablePartColumns.Visible));
            }

            //Name
            treeViewFormElementTablePart.AppendColumn(new TreeViewColumn("Назва", new CellRendererText(), "text", FormElementTablePartColumns.Name));

            //Caption
            {
                CellRendererText cell = new CellRendererText() { Editable = true };
                cell.Edited += (object o, EditedArgs args) =>
                {
                    if (listStoreFormElementTablePart.GetIterFromString(out TreeIter iter, args.Path))
                        listStoreFormElementTablePart.SetValue(iter, (int)FormElementTablePartColumns.Caption, args.NewText);
                };
                treeViewFormElementTablePart.AppendColumn(new TreeViewColumn("Заголовок", cell, "text", FormElementTablePartColumns.Caption));
            }
        }

        #endregion

        void CreatePack1(Paned hPaned)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            //Базові поля
            {
                //Тип форми
                Box hBoxType = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
                vBox.PackStart(hBoxType, false, false, 5);

                hBoxType.PackStart(new Label("Тип форми:"), false, false, 5);
                hBoxType.PackStart(labelType, false, false, 5);

                //Назва
                Box hBoxName = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
                vBox.PackStart(hBoxName, false, false, 5);

                hBoxName.PackStart(new Label("Назва:"), false, false, 5);
                hBoxName.PackStart(entryName, false, false, 5);

                //Опис
                Box hBoxDesc = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
                vBox.PackStart(hBoxDesc, false, false, 5);

                hBoxDesc.PackStart(new Label("Опис:") { Valign = Align.Start }, false, false, 5);

                ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 250, HeightRequest = 100 };
                scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
                scrollTextView.Add(textViewDesc);

                hBoxDesc.PackStart(scrollTextView, false, false, 5);
            }

            hPaned.Pack1(vBox, false, false);
        }

        void CreatePack2(Paned hPaned)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            Box hBox = new Box(Orientation.Horizontal, 0) { Halign = Align.Fill };
            hBox.PackStart(notebook, true, true, 5);
            vBox.PackStart(hBox, true, true, 0);

            hPaned.Pack2(vBox, true, false);
        }

        public void CreateNotePage(string tabName, Widget pageWidget)
        {
            ScrolledWindow scroll = new ScrolledWindow();
            scroll.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scroll.Add(pageWidget);

            notebook.AppendPage(scroll, new Label(tabName));
            notebook.ShowAll();
        }

        public void CreateElementForm()
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            //Поля
            {
                Box hBoxCaption = new Box(Orientation.Horizontal, 0);
                hBoxCaption.PackStart(new Label("Поля"), false, false, 5);
                vBox.PackStart(hBoxCaption, false, false, 2);

                Box hBoxScroll = new Box(Orientation.Horizontal, 0);
                ScrolledWindow scroll = new ScrolledWindow() { ShadowType = ShadowType.In, HeightRequest = 500 };
                scroll.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

                scroll.Add(treeViewFormElementField);
                hBoxScroll.PackStart(scroll, true, true, 5);

                vBox.PackStart(hBoxScroll, false, false, 5);
            }

            //Табличні частини
            {
                Box hBoxCaption = new Box(Orientation.Horizontal, 0);
                hBoxCaption.PackStart(new Label("Табличні частини"), false, false, 5);
                vBox.PackStart(hBoxCaption, false, false, 2);

                Box hBoxScroll = new Box(Orientation.Horizontal, 0);
                ScrolledWindow scroll = new ScrolledWindow() { ShadowType = ShadowType.In, HeightRequest = 180 };
                scroll.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

                scroll.Add(treeViewFormElementTablePart);
                hBoxScroll.PackStart(scroll, true, true, 5);

                vBox.PackStart(hBoxScroll, false, false, 5);
            }

            CreateNotePage("Форма", vBox);
        }

        public void CreateListForm()
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            //Список табличних списків
            {
                Box hBox = new Box(Orientation.Horizontal, 0) { Halign = Align.Start };
                hBox.PackStart(new Label(), false, false, 5);
                hBox.PackStart(сomboBoxFormListTabularList, false, false, 5);

                vBox.PackStart(hBox, false, false, 5);
            }

            CreateNotePage("Форма", vBox);
        }

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            if (IsNew)
                Form.Type = TypeForm;

            labelType.Markup = "<b>" + Form.Type switch
            {
                ConfigurationForms.TypeForms.Element => "Елемент",
                ConfigurationForms.TypeForms.List => "Список",
                _ => ""
            } + "</b>";

            entryName.Text = Form.Name;
            textViewDesc.Buffer.Text = Form.Desc;

            switch (TypeForm)
            {
                case ConfigurationForms.TypeForms.Element:
                    {
                        CreateElementForm();
                        break;
                    }
                case ConfigurationForms.TypeForms.List:
                    {
                        CreateListForm();
                        break;
                    }
            }

            //Сторінка генерування коду
            {
                Box vBox = new Box(Orientation.Vertical, 0);

                Button buttonGenCode = new Button("Згенерувати код");
                buttonGenCode.Clicked += (object? sender, EventArgs args) =>
                {
                    GenerateCode(Form.Type switch
                    {
                        ConfigurationForms.TypeForms.Element => "Element",
                        ConfigurationForms.TypeForms.List => "List",
                        _ => ""
                    }, true, true);
                };

                Box hBoxButton = new Box(Orientation.Horizontal, 0);
                hBoxButton.PackStart(buttonGenCode, false, false, 5);
                vBox.PackStart(hBoxButton, false, false, 5);

                Box hBoxCode = new Box(Orientation.Horizontal, 0);

                sourceViewCode.Buffer.Language = new LanguageManager().GetLanguage("c-sharp");

                ScrolledWindow scrollCode = new ScrolledWindow() { ShadowType = ShadowType.In };
                scrollCode.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
                scrollCode.Add(sourceViewCode);
                hBoxCode.PackStart(scrollCode, true, true, 5);

                vBox.PackStart(hBoxCode, true, true, 5);

                CreateNotePage("Код", vBox);
            }

            if (TypeForm == ConfigurationForms.TypeForms.List)
            {
                FillFormList();
                сomboBoxFormListTabularList.ActiveId = TabularList;
            }
            else if (TypeForm == ConfigurationForms.TypeForms.Element)
            {
                FillTreeViewFormElementField();
                FillTreeViewFormElementTablePart();
            }
        }

        void FillFormList()
        {
            foreach (KeyValuePair<string, ConfigurationTabularList> tabularList in TabularLists)
                сomboBoxFormListTabularList.Append(tabularList.Key, tabularList.Value.Name);
        }

        void FillTreeViewFormElementField()
        {
            foreach (ConfigurationField field in Fields.Values)
            {
                bool isExistField = Form.ElementFields.ContainsKey(field.Name);

                string caption = isExistField ?
                    (!string.IsNullOrEmpty(Form.ElementFields[field.Name].Caption) ?
                        Form.ElementFields[field.Name].Caption : field.Name) : field.Name;

                //Для нової форми видимими стають всі поля
                listStoreFormElementField.AppendValues(IsNew || isExistField, field.Name, caption);
            }
        }

        void FillTreeViewFormElementTablePart()
        {
            foreach (ConfigurationTablePart tablePart in TabularParts.Values)
            {
                bool isExistField = Form.ElementTableParts.ContainsKey(tablePart.Name);

                string caption = isExistField ?
                    (!string.IsNullOrEmpty(Form.ElementTableParts[tablePart.Name].Caption) ?
                        Form.ElementTableParts[tablePart.Name].Caption : tablePart.Name) : tablePart.Name;

                //Для нової форми видимими стають таб частини
                listStoreFormElementTablePart.AppendValues(IsNew || isExistField, tablePart.Name, caption);
            }
        }

        void GetValue()
        {
            Form.Name = entryName.Text;
            Form.Desc = textViewDesc.Buffer.Text;

            if (TypeForm == ConfigurationForms.TypeForms.List)
                Form.TabularList = сomboBoxFormListTabularList.ActiveId;
            else if (TypeForm == ConfigurationForms.TypeForms.Element)
            {
                //Поля форми елементу
                {
                    Form.ElementFields.Clear();
                    if (listStoreFormElementField.GetIterFirst(out TreeIter iter))
                        do
                        {
                            if ((bool)listStoreFormElementField.GetValue(iter, (int)FormElementFieldColumns.Visible))
                            {
                                string name = (string)listStoreFormElementField.GetValue(iter, (int)FormElementFieldColumns.Name);
                                string caption = (string)listStoreFormElementField.GetValue(iter, (int)FormElementFieldColumns.Caption);

                                ConfigurationField field = Fields[name];
                                Form.AppendElementField(new ConfigurationFormsElementField(field.Name, caption));
                            }
                        }
                        while (listStoreFormElementField.IterNext(ref iter));
                }

                //Табличні частини форми елементу
                {
                    Form.ElementTableParts.Clear();
                    if (listStoreFormElementTablePart.GetIterFirst(out TreeIter iter))
                        do
                        {
                            if ((bool)listStoreFormElementTablePart.GetValue(iter, (int)FormElementTablePartColumns.Visible))
                            {
                                string name = (string)listStoreFormElementTablePart.GetValue(iter, (int)FormElementTablePartColumns.Name);
                                string caption = (string)listStoreFormElementTablePart.GetValue(iter, (int)FormElementTablePartColumns.Caption);

                                ConfigurationTablePart tablePart = TabularParts[name];
                                Form.AppendElementTablePart(new ConfigurationFormsElementTablePart(tablePart.Name, caption));
                            }
                        }
                        while (listStoreFormElementTablePart.IterNext(ref iter));
                }
            }
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
                if (Forms.ContainsKey(entryName.Text))
                {
                    Message.Error(GeneralForm, $"Назва форми не унікальна");
                    return;
                }
            }
            else
            {
                if (Form.Name != entryName.Text)
                {
                    if (Forms.ContainsKey(entryName.Text))
                    {
                        Message.Error(GeneralForm, $"Назва форми не унікальна");
                        return;
                    }
                }

                Forms.Remove(Form.Name);
            }

            GetValue();

            Forms.Add(Form.Name, Form);

            IsNew = false;

            GeneralForm?.RenameCurrentPageNotebook($"Форма: {Form.Name}");
            CallBack_RefreshList?.Invoke();
        }

        #region Генерування коду

        void GenerateCode(string fileName, bool includeFields = false, bool includeTabularParts = false)
        {
            if (!(ParentType == "Directory" || ParentType == "Document" || ParentType == "RegisterInformation" || ParentType == "RegistersAccumulation"))
            {
                Message.Error(GeneralForm, "Невірно вказаний тип власника форми. Має бути Directory, Document, RegisterInformation");
                return;
            }

            GetValue();

            XmlDocument xmlConfDocument = new XmlDocument();
            xmlConfDocument.AppendChild(xmlConfDocument.CreateXmlDeclaration("1.0", "utf-8", ""));

            XmlElement rootNode = xmlConfDocument.CreateElement("root");
            xmlConfDocument.AppendChild(rootNode);

            XmlElement nodeDirectory = xmlConfDocument.CreateElement(ParentType);
            rootNode.AppendChild(nodeDirectory);

            XmlElement nodeDirectoryName = xmlConfDocument.CreateElement("Name");
            nodeDirectoryName.InnerText = ParentName;
            nodeDirectory.AppendChild(nodeDirectoryName);

            if (TypeForm == ConfigurationForms.TypeForms.List)
            {
                XmlElement nodeTabularList = xmlConfDocument.CreateElement("TabularList");
                nodeTabularList.InnerText = Form.TabularList;
                nodeDirectory.AppendChild(nodeTabularList);
            }
            else if (TypeForm == ConfigurationForms.TypeForms.Element)
            {
                Configuration.SaveFormElementField(Form.ElementFields, xmlConfDocument, nodeDirectory);
                Configuration.SaveFormElementTablePart(Form.ElementTableParts, xmlConfDocument, nodeDirectory);

                if (includeFields)
                    Configuration.SaveFields(Fields, xmlConfDocument, nodeDirectory, ParentType);

                if (includeTabularParts)
                    Configuration.SaveTabularParts(TabularParts, xmlConfDocument, nodeDirectory);
            }

            sourceViewCode.Buffer.Text = Configuration.Transform
            (
                xmlConfDocument,
                System.IO.Path.Combine(AppContext.BaseDirectory, $"xslt/Constructor{ParentType}.xslt"),
                new Dictionary<string, object>
                {
                    { "File", fileName },
                    { "NameSpaceGenerationCode", Conf.NameSpaceGenerationCode },
                    { "NameSpace", Conf.NameSpace }
                }
            );
        }

        #endregion
    }
}