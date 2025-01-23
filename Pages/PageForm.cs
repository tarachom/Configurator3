/*
Copyright (C) 2019-2025 TARAKHOMYN YURIY IVANOVYCH
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
        public string ParentName { get; set; } = ""; //Назва власника
        public string ParentType { get; set; } = ""; //Тип власника (Довідник, Документ ...)
        public ConfigurationTriggerFunctions? TriggerFunctions;
        public ConfigurationSpendFunctions? SpendFunctions;
        public Dictionary<string, ConfigurationField> Fields = []; //Поля
        public Dictionary<string, ConfigurationTablePart> TabularParts = []; //Табличні частини
        public Dictionary<string, ConfigurationTabularList> TabularLists = []; //Табличні списки
        public string TabularList { get; set; } = ""; //Табличний список зафіксований у формі
        public Dictionary<string, ConfigurationForms> Forms { get; set; } = []; //Форми
        public ConfigurationForms Form { get; set; } = new ConfigurationForms(); //Форма
        public FormConfigurator? GeneralForm { get; set; }
        public System.Action? CallBack_RefreshList { get; set; }
        public bool IsNew { get; set; } = true;
        public ConfigurationForms.TypeForms TypeForm { get; set; } = ConfigurationForms.TypeForms.None;

        public OwnerTablePart Owner { get; set; } = new OwnerTablePart(); // Власник табличної частини
        public DirectoryOtherInfoStruct DirectoryOtherInfo { get; set; } = new DirectoryOtherInfoStruct(); // Для довідника
        public DocumentOtherInfoStruct DocumentOtherInfo { get; set; } = new DocumentOtherInfoStruct(); // Для документу
        public RegisterAccumulationOtherInfoStruct RegistersAccumulationOtherInfo { get; set; } = new RegisterAccumulationOtherInfoStruct(); // Для регістру накопичення

        #region Fields

        Label labelType = new Label();
        Entry entryName = new Entry() { WidthRequest = 150 };
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
            typeof(string), //Caption
            typeof(uint),   //Size
            typeof(uint),   //Height
            typeof(int),    //SortNum
            typeof(bool),   //MultipleSelect
            typeof(string)  //Type
        );
        enum FormElementFieldColumns
        {
            Visible,
            Name,
            Caption,
            Size,
            Height,
            SortNum,
            MultipleSelect,
            Type
        }

        #endregion

        #region FormElementTablePart / Табличні частини для форми елемента

        TreeView treeViewFormElementTablePart;
        ListStore listStoreFormElementTablePart = new ListStore(
            typeof(bool),   //Visible
            typeof(string), //Name
            typeof(string), //Caption
            typeof(uint),   //Size
            typeof(uint),   //Height
            typeof(int)     //SortNum
        );
        enum FormElementTablePartColumns
        {
            Visible,
            Name,
            Caption,
            Size,
            Height,
            SortNum
        }

        #endregion

        #region FormTablePart

        CheckButton checkButtonIncludeIconColumn = new CheckButton("Вставити поле з іконкою в табличну частину");

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

            //Size
            {
                CellRendererText cell = new CellRendererText() { Editable = true, Xalign = 1 };
                cell.Edited += (object o, EditedArgs args) =>
                {
                    if (listStoreFormElementField.GetIterFromString(out TreeIter iter, args.Path))
                    {
                        uint.TryParse(args.NewText, out uint size);
                        listStoreFormElementField.SetValue(iter, (int)FormElementFieldColumns.Size, size);
                    }
                };
                treeViewFormElementField.AppendColumn(new TreeViewColumn("Ширина", cell, "text", FormElementFieldColumns.Size) { Alignment = 1 });
            }

            //Height
            {
                CellRendererText cell = new CellRendererText() { Editable = true, Xalign = 1 };
                cell.Edited += (object o, EditedArgs args) =>
                {
                    if (listStoreFormElementField.GetIterFromString(out TreeIter iter, args.Path))
                    {
                        uint.TryParse(args.NewText, out uint height);
                        listStoreFormElementField.SetValue(iter, (int)FormElementFieldColumns.Height, height);
                    }
                };
                treeViewFormElementField.AppendColumn(new TreeViewColumn("Висота", cell, "text", FormElementFieldColumns.Height) { Alignment = 1 });
            }

            //SortNum
            {
                CellRendererText cell = new CellRendererText() { Editable = true, Xalign = 1 };
                cell.Edited += (object o, EditedArgs args) =>
                {
                    if (listStoreFormElementField.GetIterFromString(out TreeIter iter, args.Path))
                    {
                        uint.TryParse(args.NewText, out uint sortNum);
                        listStoreFormElementField.SetValue(iter, (int)FormElementFieldColumns.SortNum, sortNum);
                    }
                };
                treeViewFormElementField.AppendColumn(new TreeViewColumn("Порядок", cell, "text", FormElementFieldColumns.SortNum) { Alignment = 1 });
                listStoreFormElementField.SetSortColumnId((int)FormElementFieldColumns.SortNum, SortType.Ascending);
            }

            //MultipleSelect
            {
                CellRendererToggle cell = new CellRendererToggle();
                cell.Toggled += (object o, ToggledArgs args) =>
                {
                    if (listStoreFormElementField.GetIterFromString(out TreeIter iter, args.Path))
                    {
                        bool val = (bool)listStoreFormElementField.GetValue(iter, (int)FormElementFieldColumns.MultipleSelect);
                        listStoreFormElementField.SetValue(iter, (int)FormElementFieldColumns.MultipleSelect, !val);
                    }
                };
                treeViewFormElementField.AppendColumn(new TreeViewColumn("Підбір", cell, "active", FormElementFieldColumns.MultipleSelect));
            }

            //Type
            treeViewFormElementField.AppendColumn(new TreeViewColumn("Тип", new CellRendererText(), "text", FormElementFieldColumns.Type));

            //Пустишка
            treeViewFormElementField.AppendColumn(new TreeViewColumn());
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

            //Size
            {
                CellRendererText cell = new CellRendererText() { Editable = true, Xalign = 1 };
                cell.Edited += (object o, EditedArgs args) =>
                {
                    if (listStoreFormElementTablePart.GetIterFromString(out TreeIter iter, args.Path))
                    {
                        uint.TryParse(args.NewText, out uint size);
                        listStoreFormElementTablePart.SetValue(iter, (int)FormElementTablePartColumns.Size, size);
                    }
                };
                treeViewFormElementTablePart.AppendColumn(new TreeViewColumn("Ширина", cell, "text", FormElementTablePartColumns.Size) { Alignment = 1 });
            }

            //Height
            {
                CellRendererText cell = new CellRendererText() { Editable = true, Xalign = 1 };
                cell.Edited += (object o, EditedArgs args) =>
                {
                    if (listStoreFormElementTablePart.GetIterFromString(out TreeIter iter, args.Path))
                    {
                        uint.TryParse(args.NewText, out uint height);
                        listStoreFormElementTablePart.SetValue(iter, (int)FormElementTablePartColumns.Height, height);
                    }
                };
                treeViewFormElementTablePart.AppendColumn(new TreeViewColumn("Висота", cell, "text", FormElementTablePartColumns.Height) { Alignment = 1 });
            }

            //SortNum
            {
                CellRendererText cell = new CellRendererText() { Editable = true, Xalign = 1 };
                cell.Edited += (object o, EditedArgs args) =>
                {
                    if (listStoreFormElementTablePart.GetIterFromString(out TreeIter iter, args.Path))
                    {
                        uint.TryParse(args.NewText, out uint sortNum);
                        listStoreFormElementTablePart.SetValue(iter, (int)FormElementTablePartColumns.SortNum, sortNum);
                    }
                };
                treeViewFormElementTablePart.AppendColumn(new TreeViewColumn("Порядок", cell, "text", FormElementTablePartColumns.SortNum) { Alignment = 1 });
                listStoreFormElementTablePart.SetSortColumnId((int)FormElementTablePartColumns.SortNum, SortType.Ascending);
            }

            //Пустишка
            treeViewFormElementTablePart.AppendColumn(new TreeViewColumn());
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

                ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 168, HeightRequest = 100 };
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

        #region CreateFunc

        void CreateNotePage(string tabName, Widget pageWidget)
        {
            ScrolledWindow scroll = new ScrolledWindow();
            scroll.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scroll.Add(pageWidget);

            notebook.AppendPage(scroll, new Label(tabName));
            notebook.ShowAll();
        }

        void CreateToolbarFormElementField(Box vBox)
        {
            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton addUp = new ToolButton(new Image(Stock.GoUp, IconSize.Menu), "Up") { TooltipText = "Up" };
            addUp.Clicked += (object? sender, EventArgs args) => TreeViewFunc.SortTreeView(treeViewFormElementField, (int)FormElementFieldColumns.SortNum, false);
            toolbar.Add(addUp);

            ToolButton addDown = new ToolButton(new Image(Stock.GoDown, IconSize.Menu), "Down") { TooltipText = "Down" };
            addDown.Clicked += (object? sender, EventArgs args) => TreeViewFunc.SortTreeView(treeViewFormElementField, (int)FormElementFieldColumns.SortNum, true);
            toolbar.Add(addDown);
        }

        void CreateToolbarFormElementTablePart(Box vBox)
        {
            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton addUp = new ToolButton(new Image(Stock.GoUp, IconSize.Menu), "Up") { TooltipText = "Up" };
            addUp.Clicked += (object? sender, EventArgs args) => TreeViewFunc.SortTreeView(treeViewFormElementTablePart, (int)FormElementTablePartColumns.SortNum, false);
            toolbar.Add(addUp);

            ToolButton addDown = new ToolButton(new Image(Stock.GoDown, IconSize.Menu), "Down") { TooltipText = "Down" };
            addDown.Clicked += (object? sender, EventArgs args) => TreeViewFunc.SortTreeView(treeViewFormElementTablePart, (int)FormElementTablePartColumns.SortNum, true);
            toolbar.Add(addDown);
        }

        void CreateElementForm(bool existTablePart = true)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            //Поля
            {
                Box hBoxCaption = new Box(Orientation.Horizontal, 0);
                hBoxCaption.PackStart(new Label("Поля"), false, false, 5);
                vBox.PackStart(hBoxCaption, false, false, 5);

                CreateToolbarFormElementField(vBox);

                Box hBoxScroll = new Box(Orientation.Horizontal, 0);
                ScrolledWindow scroll = new ScrolledWindow() { ShadowType = ShadowType.In, HeightRequest = 500 };
                scroll.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

                scroll.Add(treeViewFormElementField);
                hBoxScroll.PackStart(scroll, true, true, 5);

                vBox.PackStart(hBoxScroll, false, false, 0);
            }

            //Табличні частини
            if (existTablePart)
            {
                Box hBoxCaption = new Box(Orientation.Horizontal, 0);
                hBoxCaption.PackStart(new Label("Табличні частини"), false, false, 5);
                vBox.PackStart(hBoxCaption, false, false, 5);

                CreateToolbarFormElementTablePart(vBox);

                Box hBoxScroll = new Box(Orientation.Horizontal, 0);
                ScrolledWindow scroll = new ScrolledWindow() { ShadowType = ShadowType.In, HeightRequest = 180 };
                scroll.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

                scroll.Add(treeViewFormElementTablePart);
                hBoxScroll.PackStart(scroll, true, true, 5);

                vBox.PackStart(hBoxScroll, false, false, 0);
            }

            CreateNotePage("Форма", vBox);
        }

        void CreateSettingsTablePartForm()
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            {
                Box hBox = new Box(Orientation.Horizontal, 0);
                vBox.PackStart(hBox, false, false, 10);

                hBox.PackStart(new Label("<b>Додаткові параметри для табличної частини</b>") { UseMarkup = true, UseUnderline = false }, false, false, 10);
            }

            {
                Box hBox = new Box(Orientation.Horizontal, 0);
                vBox.PackStart(hBox, false, false, 5);

                // Вставити поле з іконкою в табличну частину
                hBox.PackStart(checkButtonIncludeIconColumn, false, false, 5);
            }

            CreateNotePage("Додатково", vBox);
        }

        public void CreateListForm()
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            //Список табличних списків
            {
                Box hBox = new Box(Orientation.Horizontal, 0) { Halign = Align.Start };
                hBox.PackStart(new Label("Табличний список:"), false, false, 5);
                hBox.PackStart(сomboBoxFormListTabularList, false, false, 5);

                vBox.PackStart(hBox, false, false, 5);
            }

            CreateNotePage("Форма", vBox);
        }

        #endregion

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            string name = TypeForm switch
            {
                ConfigurationForms.TypeForms.Element => "Елемент",
                ConfigurationForms.TypeForms.List => "Список",
                ConfigurationForms.TypeForms.ListSmallSelect => "Швидкий вибір",
                ConfigurationForms.TypeForms.PointerControl => "PointerControl",
                ConfigurationForms.TypeForms.MultiplePointerControl => "MultiplePointerControl",
                ConfigurationForms.TypeForms.ListAndTree => "Список з Деревом",
                ConfigurationForms.TypeForms.TablePart => "Таблична частина",
                ConfigurationForms.TypeForms.Function => "Функції",
                ConfigurationForms.TypeForms.Triggers => "Тригери",
                ConfigurationForms.TypeForms.SpendTheDocument => "Проведення документу",
                ConfigurationForms.TypeForms.Report => "Звіт",
                _ => ""
            };

            if (IsNew)
            {
                Form.Type = TypeForm;
                entryName.Text = textViewDesc.Buffer.Text = name;
            }
            else
            {
                entryName.Text = Form.Name;
                textViewDesc.Buffer.Text = Form.Desc;
            }

            labelType.Markup = $"<b>{name}</b>";

            switch (TypeForm)
            {
                case ConfigurationForms.TypeForms.Element:
                    {
                        CreateElementForm(!(ParentType == "RegisterInformation" || ParentType == "RegisterAccumulation"));
                        break;
                    }
                case ConfigurationForms.TypeForms.TablePart:
                    {
                        checkButtonIncludeIconColumn.Active = Form.IncludeIconColumn;

                        CreateElementForm(false);
                        CreateSettingsTablePartForm();
                        break;
                    }
                case ConfigurationForms.TypeForms.Report:
                    {
                        CreateElementForm(false);
                        break;
                    }
                case ConfigurationForms.TypeForms.List:
                case ConfigurationForms.TypeForms.ListSmallSelect:
                case ConfigurationForms.TypeForms.ListAndTree:
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
                        ConfigurationForms.TypeForms.ListSmallSelect => "ListSmallSelect",
                        ConfigurationForms.TypeForms.PointerControl => "PointerControl",
                        ConfigurationForms.TypeForms.MultiplePointerControl => "MultiplePointerControl",
                        ConfigurationForms.TypeForms.ListAndTree => "ListAndTree",
                        ConfigurationForms.TypeForms.TablePart => "TablePart",
                        ConfigurationForms.TypeForms.Function => "Function",
                        ConfigurationForms.TypeForms.Triggers => "Triggers",
                        ConfigurationForms.TypeForms.SpendTheDocument => "SpendTheDocument",
                        ConfigurationForms.TypeForms.Report => "Report",
                        _ => ""
                    });
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

            if (TypeForm == ConfigurationForms.TypeForms.List ||
                TypeForm == ConfigurationForms.TypeForms.ListSmallSelect ||
                TypeForm == ConfigurationForms.TypeForms.ListAndTree)
            {
                FillFormList();
                сomboBoxFormListTabularList.ActiveId = TabularList;
                if (сomboBoxFormListTabularList.Active == -1) сomboBoxFormListTabularList.Active = 0;
            }
            else if (TypeForm == ConfigurationForms.TypeForms.Element)
            {
                FillTreeViewFormElementField();
                FillTreeViewFormElementTablePart();
            }
            else if (TypeForm == ConfigurationForms.TypeForms.TablePart)
            {
                FillTreeViewFormElementField();
            }
            else if (TypeForm == ConfigurationForms.TypeForms.Report)
            {
                FillTreeViewFormElementField();
            }
        }

        void FillFormList()
        {
            foreach (KeyValuePair<string, ConfigurationTabularList> tabularList in TabularLists)
                сomboBoxFormListTabularList.Append(tabularList.Key, tabularList.Value.Name);
        }

        void FillTreeViewFormElementField()
        {
            int counter = 0;
            foreach (ConfigurationField field in Fields.Values)
            {
                bool isExistField = Form.ElementFields.ContainsKey(field.Name);

                string caption = isExistField ?
                    (!string.IsNullOrEmpty(Form.ElementFields[field.Name].Caption) ?
                        Form.ElementFields[field.Name].Caption : field.Name) : field.Name;

                uint size = isExistField ? Form.ElementFields[field.Name].Size : 0;
                uint height = isExistField ? Form.ElementFields[field.Name].Height : 0;
                int sortNum = isExistField ? Form.ElementFields[field.Name].SortNum : IsNew ? ++counter : 100;
                bool multipleSelect = isExistField ? Form.ElementFields[field.Name].MultipleSelect : false;
                string sType = field.Type == "pointer" || field.Type == "enum" ? field.Pointer : field.Type;

                //Для нової форми видимими стають всі поля
                listStoreFormElementField.AppendValues(IsNew || isExistField, field.Name, caption, size, height, sortNum, multipleSelect, sType);
            }
        }

        void FillTreeViewFormElementTablePart()
        {
            int counter = 0;
            foreach (ConfigurationTablePart tablePart in TabularParts.Values)
            {
                bool isExistField = Form.ElementTableParts.ContainsKey(tablePart.Name);

                string caption = isExistField ?
                    (!string.IsNullOrEmpty(Form.ElementTableParts[tablePart.Name].Caption) ?
                        Form.ElementTableParts[tablePart.Name].Caption : tablePart.Name) : tablePart.Name;

                uint size = isExistField ? Form.ElementTableParts[tablePart.Name].Size : 0;
                uint height = isExistField ? Form.ElementTableParts[tablePart.Name].Height : 0;
                int sortNum = isExistField ? Form.ElementTableParts[tablePart.Name].SortNum : IsNew ? ++counter : 100;

                //Для нової форми видимими стають таб частини
                listStoreFormElementTablePart.AppendValues(IsNew || isExistField, tablePart.Name, caption, size, height, sortNum);
            }
        }

        void GetValue()
        {
            Form.Name = entryName.Text;
            Form.Desc = textViewDesc.Buffer.Text;

            if (TypeForm == ConfigurationForms.TypeForms.List ||
                TypeForm == ConfigurationForms.TypeForms.ListSmallSelect ||
                TypeForm == ConfigurationForms.TypeForms.ListAndTree)
                Form.TabularList = сomboBoxFormListTabularList.ActiveId;
            else if (TypeForm == ConfigurationForms.TypeForms.Element ||
                TypeForm == ConfigurationForms.TypeForms.TablePart ||
                TypeForm == ConfigurationForms.TypeForms.Report)
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
                                uint size = (uint)listStoreFormElementField.GetValue(iter, (int)FormElementFieldColumns.Size);
                                uint height = (uint)listStoreFormElementField.GetValue(iter, (int)FormElementFieldColumns.Height);
                                int sortNum = (int)listStoreFormElementField.GetValue(iter, (int)FormElementFieldColumns.SortNum);
                                bool multipleSelect = (bool)listStoreFormElementField.GetValue(iter, (int)FormElementFieldColumns.MultipleSelect);

                                ConfigurationField field = Fields[name];
                                Form.AppendElementField(new ConfigurationFormsElementField(field.Name, caption, size, height, sortNum, multipleSelect));
                            }
                        }
                        while (listStoreFormElementField.IterNext(ref iter));
                }

                //Табличні частини тільки для елементу
                if (TypeForm == ConfigurationForms.TypeForms.Element)
                {
                    Form.ElementTableParts.Clear();
                    if (listStoreFormElementTablePart.GetIterFirst(out TreeIter iter))
                        do
                        {
                            if ((bool)listStoreFormElementTablePart.GetValue(iter, (int)FormElementTablePartColumns.Visible))
                            {
                                string name = (string)listStoreFormElementTablePart.GetValue(iter, (int)FormElementTablePartColumns.Name);
                                string caption = (string)listStoreFormElementTablePart.GetValue(iter, (int)FormElementTablePartColumns.Caption);
                                uint size = (uint)listStoreFormElementTablePart.GetValue(iter, (int)FormElementTablePartColumns.Size);
                                uint height = (uint)listStoreFormElementTablePart.GetValue(iter, (int)FormElementTablePartColumns.Height);
                                int sortNum = (int)listStoreFormElementTablePart.GetValue(iter, (int)FormElementTablePartColumns.SortNum);

                                ConfigurationTablePart tablePart = TabularParts[name];
                                Form.AppendElementTablePart(new ConfigurationFormsElementTablePart(tablePart.Name, caption, size, height, sortNum));
                            }
                        }
                        while (listStoreFormElementTablePart.IterNext(ref iter));
                }
                else if (TypeForm == ConfigurationForms.TypeForms.TablePart)
                    Form.IncludeIconColumn = checkButtonIncludeIconColumn.Active;
            }
        }

        #endregion

        void OnSaveClick(object? sender, EventArgs args)
        {
            entryName.Text = entryName.Text.Trim();

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

        void GenerateCode(string fileName)
        {
            if (!(ParentType == "Directory" || ParentType == "Document" ||
                ParentType == "RegisterInformation" || ParentType == "RegisterAccumulation" ||
                ParentType == "TablePart"))
            {
                Message.Error(GeneralForm, "Невірно вказаний тип власника форми. Має бути Directory, Document, RegisterInformation, RegisterAccumulation або TablePart");
                return;
            }

            GetValue();

            XmlDocument xmlConfDocument = new XmlDocument();
            xmlConfDocument.AppendChild(xmlConfDocument.CreateXmlDeclaration("1.0", "utf-8", ""));

            XmlElement rootNode = xmlConfDocument.CreateElement("root");
            xmlConfDocument.AppendChild(rootNode);

            XmlElement nodeParentType = xmlConfDocument.CreateElement(ParentType);
            rootNode.AppendChild(nodeParentType);

            XmlElement nodeName = xmlConfDocument.CreateElement("Name");
            nodeName.InnerText = ParentName;
            nodeParentType.AppendChild(nodeName);

            if (ParentType == "Directory")
            {
                XmlElement nodeDirectoryAutomaticNumeration = xmlConfDocument.CreateElement("AutomaticNumeration");
                nodeDirectoryAutomaticNumeration.InnerText = DirectoryOtherInfo.AutomaticNumeration ? "1" : "0";
                nodeParentType.AppendChild(nodeDirectoryAutomaticNumeration);

                if (TriggerFunctions != null)
                    Configuration.SaveTriggerFunctions(TriggerFunctions, xmlConfDocument, nodeParentType);

                XmlElement nodeDirectoryType = xmlConfDocument.CreateElement("Type");
                nodeDirectoryType.InnerText = DirectoryOtherInfo.TypeDirectory.ToString();
                nodeParentType.AppendChild(nodeDirectoryType);

                if (DirectoryOtherInfo.TypeDirectory == ConfigurationDirectories.TypeDirectories.HierarchyInAnotherDirectory)
                {
                    string pointerFolders = DirectoryOtherInfo.PointerFolders;
                    if (!string.IsNullOrEmpty(pointerFolders) && pointerFolders.Contains('.'))
                    {
                        string[] pointer_and_type = pointerFolders.Split(".", StringSplitOptions.None);
                        if (pointer_and_type.Length == 2)
                        {
                            //Назва довідника
                            XmlElement nodeDirectoryPointerFolders = xmlConfDocument.CreateElement("PointerFolders");
                            nodeDirectoryPointerFolders.InnerText = pointer_and_type[1];
                            nodeParentType.AppendChild(nodeDirectoryPointerFolders);

                            //Пошук поля Папки
                            foreach (ConfigurationField field in Fields.Values)
                                if (field.Type == "pointer" && field.Pointer == $"Довідники.{pointer_and_type[1]}")
                                {
                                    XmlElement nodeDirectoryFolder = xmlConfDocument.CreateElement("FieldFolder");
                                    nodeDirectoryFolder.InnerText = field.Name;
                                    nodeParentType.AppendChild(nodeDirectoryFolder);

                                    break;
                                }
                        }
                    }
                }
                else if (DirectoryOtherInfo.TypeDirectory == ConfigurationDirectories.TypeDirectories.Hierarchical)
                {
                    XmlElement nodeDirectoryParentField = xmlConfDocument.CreateElement("ParentField");
                    nodeDirectoryParentField.InnerText = DirectoryOtherInfo.ParentField;
                    nodeParentType.AppendChild(nodeDirectoryParentField);
                }

                //Довідник власник
                XmlElement nodeDirectoryOwner = xmlConfDocument.CreateElement("DirectoryOwner");
                nodeDirectoryOwner.InnerText = DirectoryOtherInfo.DirectoryOwner;
                nodeParentType.AppendChild(nodeDirectoryOwner);

                //Поле вказівник на довідник власник
                XmlElement nodePointerFieldOwner = xmlConfDocument.CreateElement("PointerFieldOwner");
                nodePointerFieldOwner.InnerText = DirectoryOtherInfo.PointerFieldOwner;
                nodeParentType.AppendChild(nodePointerFieldOwner);
            }

            if (ParentType == "Document")
            {
                XmlElement nodeDocumentAutomaticNumeration = xmlConfDocument.CreateElement("AutomaticNumeration");
                nodeDocumentAutomaticNumeration.InnerText = DocumentOtherInfo.AutomaticNumeration ? "1" : "0";
                nodeParentType.AppendChild(nodeDocumentAutomaticNumeration);

                if (TriggerFunctions != null)
                    Configuration.SaveTriggerFunctions(TriggerFunctions, xmlConfDocument, nodeParentType);

                if (SpendFunctions != null)
                    Configuration.SaveSpendFunctions(SpendFunctions, xmlConfDocument, nodeParentType);

                XmlElement nodeDocumentExportXML = xmlConfDocument.CreateElement("ExportXML");
                nodeDocumentExportXML.InnerText = DocumentOtherInfo.ExportXML ? "1" : "0";
                nodeParentType.AppendChild(nodeDocumentExportXML);
            }

            if (ParentType == "RegisterAccumulation")
            {
                XmlElement nodeRegisterAccumulationType = xmlConfDocument.CreateElement("Type");
                nodeRegisterAccumulationType.InnerText = RegistersAccumulationOtherInfo.TypeReg.ToString();
                nodeParentType.AppendChild(nodeRegisterAccumulationType);
            }

            if (TypeForm == ConfigurationForms.TypeForms.List || TypeForm == ConfigurationForms.TypeForms.ListSmallSelect || TypeForm == ConfigurationForms.TypeForms.ListAndTree)
            {
                XmlElement nodeTabularList = xmlConfDocument.CreateElement("TabularList");
                nodeTabularList.InnerText = Form.TabularList;
                nodeParentType.AppendChild(nodeTabularList);
            }
            else if (TypeForm == ConfigurationForms.TypeForms.Element)
            {
                Configuration.SaveFormElementField(Conf, Fields, Form.ElementFields, xmlConfDocument, nodeParentType);
                Configuration.SaveFormElementTablePart(TabularParts, Form.ElementTableParts, xmlConfDocument, nodeParentType);
            }
            else if (TypeForm == ConfigurationForms.TypeForms.TablePart)
            {
                XmlElement nodeOwnerExist = xmlConfDocument.CreateElement("OwnerExist");
                nodeOwnerExist.InnerText = Owner.Exist ? "1" : "0";
                nodeParentType.AppendChild(nodeOwnerExist);

                XmlElement nodeOwnerType = xmlConfDocument.CreateElement("OwnerType");
                nodeOwnerType.InnerText = Owner.Type;
                nodeParentType.AppendChild(nodeOwnerType);

                XmlElement nodeOwnerName = xmlConfDocument.CreateElement("OwnerName");
                nodeOwnerName.InnerText = Owner.Name;
                nodeParentType.AppendChild(nodeOwnerName);

                XmlElement nodeOwnerBlockName = xmlConfDocument.CreateElement("OwnerBlockName");
                nodeOwnerBlockName.InnerText = Owner.BlockName;
                nodeParentType.AppendChild(nodeOwnerBlockName);

                XmlElement nodeIncludeIconColumn = xmlConfDocument.CreateElement("IncludeIconColumn");
                nodeIncludeIconColumn.InnerText = checkButtonIncludeIconColumn.Active ? "1" : "0";
                nodeParentType.AppendChild(nodeIncludeIconColumn);

                Configuration.SaveFormElementField(Conf, Fields, Form.ElementFields, xmlConfDocument, nodeParentType);
            }
            else if (TypeForm == ConfigurationForms.TypeForms.Report)
            {
                if (ParentType == "TablePart")
                {
                    XmlElement nodeOwnerName = xmlConfDocument.CreateElement("OwnerName");
                    nodeOwnerName.InnerText = Owner.Name;
                    nodeParentType.AppendChild(nodeOwnerName);

                    XmlElement nodeOwnerBlockName = xmlConfDocument.CreateElement("OwnerBlockName");
                    nodeOwnerBlockName.InnerText = Owner.BlockName;
                    nodeParentType.AppendChild(nodeOwnerBlockName);
                }

                Configuration.SaveFormElementField(Conf, Fields, Form.ElementFields, xmlConfDocument, nodeParentType);
            }
            else if (TypeForm == ConfigurationForms.TypeForms.Function)
            {
                Configuration.SaveFields(Fields, xmlConfDocument, nodeParentType, ParentType);
                Configuration.SaveTabularParts(Conf, TabularParts, xmlConfDocument, nodeParentType);
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