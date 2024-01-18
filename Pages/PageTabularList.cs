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
    class PageTabularList : VBox
    {
        Configuration Conf { get { return Program.Kernel.Conf; } }

        public Dictionary<string, ConfigurationObjectField> Fields = [];
        public Dictionary<string, ConfigurationTabularList> TabularLists { get; set; } = [];
        public ConfigurationTabularList TabularList { get; set; } = new ConfigurationTabularList();
        public FormConfigurator? GeneralForm { get; set; }
        public System.Action? CallBack_RefreshList { get; set; }
        public bool IsNew { get; set; } = true;

        public string ConfOwnerName { get; set; } = ""; /* Документи, Довідники */

        #region Fields

        enum Columns
        {
            Visible,
            Name,
            Caption,
            Size,
            SortNum,
            SortField,
            Type
        }

        ListStore listStore = new ListStore(
            typeof(bool),   //Visible
            typeof(string), //Name
            typeof(string), //Caption
            typeof(uint),   //Size
            typeof(int),    //SortNum
            typeof(bool),   //SortField
            typeof(string)  //Type
        );

        TreeView treeViewFields;

        Entry entryName = new Entry() { WidthRequest = 250 };
        CheckButton checkButtonIsTree = new CheckButton("Дерево");
        TextView textViewDesc = new TextView() { WrapMode = WrapMode.Word };

        #endregion

        #region AdditionalFields

        //Колонки для додатковх полів
        enum ColumnsAdditional
        {
            Visible,
            Name,
            Caption,
            Size,
            SortNum,
            Type,
            Value
        }

        //Додаткові поля
        ListStore listStoreAdditional = new ListStore(
            typeof(bool),   //Visible
            typeof(string), //Name
            typeof(string), //Caption
            typeof(uint),   //Size
            typeof(int),    //SortNum
            typeof(string), //Type
            typeof(string)  //Value
        );

        //Типи даних для додаткових полів
        ListStore listStoreAdditionalType = new ListStore(
            typeof(string) //Name
        );

        string[] AdditionalTypeList = ["string", "integer", "numeric", "boolean", "image"];

        TreeView treeViewAdditional;

        #endregion

        public PageTabularList() : base()
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

            treeViewFields = new TreeView(listStore);
            AddColumnTreeViewFields();

            treeViewAdditional = new TreeView(listStoreAdditional);
            treeViewAdditional.Selection.Mode = SelectionMode.Multiple;
            AddColumnTreeViewAdditional();

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
            {
                HBox hBoxCaption = new HBox();
                hBoxCaption.PackStart(new Label("Поля:"), false, false, 5);
                vBox.PackStart(hBoxCaption, false, false, 5);

                HBox hBoxScroll = new HBox();
                ScrolledWindow scroll = new ScrolledWindow() { ShadowType = ShadowType.In };
                scroll.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
                scroll.SetSizeRequest(0, 400);

                scroll.Add(treeViewFields);
                hBoxScroll.PackStart(scroll, true, true, 5);

                vBox.PackStart(hBoxScroll, false, false, 0);
            }

            //Додаткові поля
            {
                //Заголовок
                HBox hBoxCaption = new HBox();
                hBoxCaption.PackStart(new Label("Додаткові поля:"), false, false, 5);
                vBox.PackStart(hBoxCaption, false, false, 5);

                //Toolbar
                Toolbar toolbar = new Toolbar();
                vBox.PackStart(toolbar, false, false, 0);

                ToolButton buttonAdd = new ToolButton(new Image(Stock.New, IconSize.Menu), "Додати") { IsImportant = true };
                buttonAdd.Clicked += OnAdditionalFieldsAddClick;
                toolbar.Add(buttonAdd);

                ToolButton buttonCopy = new ToolButton(new Image(Stock.Copy, IconSize.Menu), "Копіювати") { IsImportant = true };
                buttonCopy.Clicked += OnAdditionalFieldsCopyClick;
                toolbar.Add(buttonCopy);

                ToolButton buttonDelete = new ToolButton(new Image(Stock.Clear, IconSize.Menu), "Видалити") { IsImportant = true };
                buttonDelete.Clicked += OnAdditionalFieldsRemoveClick;
                toolbar.Add(buttonDelete);

                //Tree
                HBox hBoxScroll = new HBox();
                ScrolledWindow scroll = new ScrolledWindow() { ShadowType = ShadowType.In };
                scroll.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
                scroll.SetSizeRequest(0, 400);

                scroll.Add(treeViewAdditional);
                hBoxScroll.PackStart(scroll, true, true, 5);

                vBox.PackStart(hBoxScroll, false, false, 0);
            }

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

            //Опис
            HBox hBoxDesc = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxDesc, false, false, 5);

            hBoxDesc.PackStart(new Label("Опис:") { Valign = Align.Start }, false, false, 5);

            ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 250, HeightRequest = 100 };
            scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollTextView.Add(textViewDesc);

            hBoxDesc.PackStart(scrollTextView, false, false, 5);

            //Дерево
            HBox hBoxTree = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxTree, false, false, 5);

            hBoxTree.PackStart(checkButtonIsTree, false, false, 5);

            hPaned.Pack1(vBox, false, false);
        }

        #region TreeView

        void AddColumnTreeViewFields()
        {
            //Visible
            {
                CellRendererToggle cell = new CellRendererToggle();
                cell.Toggled += (object o, ToggledArgs args) =>
                {
                    Gtk.TreeIter iter;
                    if (listStore.GetIterFromString(out iter, args.Path))
                    {
                        bool val = (bool)listStore.GetValue(iter, (int)Columns.Visible);
                        listStore.SetValue(iter, (int)Columns.Visible, !val);
                    }
                };
                treeViewFields.AppendColumn(new TreeViewColumn("", cell, "active", Columns.Visible));
            }

            //Name
            treeViewFields.AppendColumn(new TreeViewColumn("Назва", new CellRendererText(), "text", Columns.Name));

            //Caption
            {
                CellRendererText cell = new CellRendererText() { Editable = true };
                cell.Edited += (object o, EditedArgs args) =>
                {
                    Gtk.TreeIter iter;
                    if (listStore.GetIterFromString(out iter, args.Path))
                        listStore.SetValue(iter, (int)Columns.Caption, args.NewText);
                };
                treeViewFields.AppendColumn(new TreeViewColumn("Заголовок", cell, "text", Columns.Caption));
            }

            //Size
            {
                CellRendererText cell = new CellRendererText() { Editable = true };
                cell.Edited += (object o, EditedArgs args) =>
                {
                    Gtk.TreeIter iter;
                    if (listStore.GetIterFromString(out iter, args.Path))
                    {
                        uint size;
                        uint.TryParse(args.NewText, out size);

                        listStore.SetValue(iter, (int)Columns.Size, size);
                    }
                };
                treeViewFields.AppendColumn(new TreeViewColumn("Розмір", cell, "text", Columns.Size));
            }

            //SortNum
            {
                CellRendererText cell = new CellRendererText() { Editable = true };
                cell.Edited += (object o, EditedArgs args) =>
                {
                    Gtk.TreeIter iter;
                    if (listStore.GetIterFromString(out iter, args.Path))
                    {
                        uint sortNum;
                        uint.TryParse(args.NewText, out sortNum);

                        listStore.SetValue(iter, (int)Columns.SortNum, sortNum);
                    }
                };
                treeViewFields.AppendColumn(new TreeViewColumn("Порядок", cell, "text", Columns.SortNum));
                listStore.SetSortColumnId((int)Columns.SortNum, SortType.Ascending);
            }

            //SortField
            {
                CellRendererToggle cell = new CellRendererToggle();
                cell.Toggled += (object o, ToggledArgs args) =>
                {
                    Gtk.TreeIter iter;
                    if (listStore.GetIterFromString(out iter, args.Path))
                    {
                        bool val = (bool)listStore.GetValue(iter, (int)Columns.SortField);
                        listStore.SetValue(iter, (int)Columns.SortField, !val);
                    }
                };
                treeViewFields.AppendColumn(new TreeViewColumn("Сортувати", cell, "active", Columns.SortField));
            }

            //Type
            treeViewFields.AppendColumn(new TreeViewColumn("Тип", new CellRendererText(), "text", Columns.Type));
        }

        void AddColumnTreeViewAdditional()
        {
            //Visible
            {
                CellRendererToggle cell = new CellRendererToggle();
                cell.Toggled += (object o, ToggledArgs args) =>
                {
                    TreeIter iter;
                    if (listStoreAdditional.GetIterFromString(out iter, args.Path))
                    {
                        bool val = (bool)listStoreAdditional.GetValue(iter, (int)ColumnsAdditional.Visible);
                        listStoreAdditional.SetValue(iter, (int)ColumnsAdditional.Visible, !val);
                    }
                };
                treeViewAdditional.AppendColumn(new TreeViewColumn("", cell, "active", ColumnsAdditional.Visible));
            }

            //Name
            {
                CellRendererText cell = new CellRendererText() { Editable = true };
                cell.Edited += (object o, EditedArgs args) =>
                {
                    TreeIter iter;
                    if (listStoreAdditional.GetIterFromString(out iter, args.Path))
                        listStoreAdditional.SetValue(iter, (int)ColumnsAdditional.Name, args.NewText);
                };
                treeViewAdditional.AppendColumn(new TreeViewColumn("Назва", cell, "text", ColumnsAdditional.Name) { MinWidth = 150 });
            }

            //Caption
            {
                CellRendererText cell = new CellRendererText() { Editable = true };
                cell.Edited += (object o, EditedArgs args) =>
                {
                    TreeIter iter;
                    if (listStoreAdditional.GetIterFromString(out iter, args.Path))
                        listStoreAdditional.SetValue(iter, (int)ColumnsAdditional.Caption, args.NewText);
                };
                treeViewAdditional.AppendColumn(new TreeViewColumn("Заголовок", cell, "text", ColumnsAdditional.Caption) { MinWidth = 150 });
            }

            //Size
            {
                CellRendererText cell = new CellRendererText() { Editable = true };
                cell.Edited += (object o, EditedArgs args) =>
                {
                    TreeIter iter;
                    if (listStoreAdditional.GetIterFromString(out iter, args.Path))
                    {
                        uint size;
                        uint.TryParse(args.NewText, out size);

                        listStoreAdditional.SetValue(iter, (int)ColumnsAdditional.Size, size);
                    }
                };
                treeViewAdditional.AppendColumn(new TreeViewColumn("Розмір", cell, "text", ColumnsAdditional.Size));
            }

            //SortNum
            {
                CellRendererText cell = new CellRendererText() { Editable = true };
                cell.Edited += (object o, EditedArgs args) =>
                {
                    TreeIter iter;
                    if (listStoreAdditional.GetIterFromString(out iter, args.Path))
                    {
                        uint sortNum;
                        uint.TryParse(args.NewText, out sortNum);

                        listStoreAdditional.SetValue(iter, (int)ColumnsAdditional.SortNum, sortNum);
                    }
                };
                treeViewAdditional.AppendColumn(new TreeViewColumn("Порядок", cell, "text", ColumnsAdditional.SortNum));
                listStoreAdditional.SetSortColumnId((int)ColumnsAdditional.SortNum, SortType.Ascending);
            }

            //Type
            {
                CellRendererCombo cell = new CellRendererCombo() { Editable = true, Model = listStoreAdditionalType, TextColumn = 0 };
                cell.Edited += (object sender, EditedArgs args) =>
                {
                    TreeIter iter;
                    if (listStoreAdditional.GetIterFromString(out iter, args.Path))
                        listStoreAdditional.SetValue(iter, (int)ColumnsAdditional.Type, args.NewText);
                };
                treeViewAdditional.AppendColumn(new TreeViewColumn("Тип", cell, "text", ColumnsAdditional.Type));
            }

            //Caption
            {
                CellRendererText cell = new CellRendererText() { Editable = true };
                cell.Edited += (object o, EditedArgs args) =>
                {
                    TreeIter iter;
                    if (listStoreAdditional.GetIterFromString(out iter, args.Path))
                        listStoreAdditional.SetValue(iter, (int)ColumnsAdditional.Value, args.NewText);
                };
                treeViewAdditional.AppendColumn(new TreeViewColumn("Значення", cell, "text", ColumnsAdditional.Value) { MinWidth = 300 });
            }

            //Пустишка
            treeViewAdditional.AppendColumn(new TreeViewColumn());
        }

        #endregion

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            //Дерево доступне тільки для Довідників
            checkButtonIsTree.Sensitive = ConfOwnerName == "Довідники";

            FillTreeView();
            FillTreeAdditionalView();

            if (IsNew)
                TabularList.Name = "Записи";

            entryName.Text = TabularList.Name;
            checkButtonIsTree.Active = TabularList.IsTree;
            textViewDesc.Buffer.Text = TabularList.Desc;

            //Типи даних для додаткових полів
            foreach (string type in AdditionalTypeList)
                listStoreAdditionalType.AppendValues(type);
        }

        void FillTreeView()
        {
            foreach (ConfigurationObjectField field in Fields.Values)
            {
                bool isExistField = TabularList.Fields.ContainsKey(field.Name);

                string caption = isExistField ?
                    (!string.IsNullOrEmpty(TabularList.Fields[field.Name].Caption) ?
                        TabularList.Fields[field.Name].Caption : field.Name) : field.Name;

                uint size = isExistField ? TabularList.Fields[field.Name].Size : 0;
                int sortNum = isExistField ? TabularList.Fields[field.Name].SortNum : 100;
                bool sortField = isExistField ? TabularList.Fields[field.Name].SortField : false;
                string sType = field.Type == "pointer" || field.Type == "enum" ? field.Pointer : field.Type;

                listStore.AppendValues(isExistField, field.Name, caption, size, sortNum, sortField, sType);
            }
        }

        void FillTreeAdditionalView()
        {
            listStoreAdditional.Clear();

            foreach (ConfigurationTabularListAdditionalField field in TabularList.AdditionalFields.Values)
            {
                listStoreAdditional.AppendValues(
                    field.Visible,
                    field.Name,
                    field.Caption,
                    field.Size,
                    field.SortNum,
                    field.Type,
                    field.Value
                );
            }
        }

        void GetValue()
        {
            TabularList.Name = entryName.Text;
            TabularList.IsTree = checkButtonIsTree.Active;
            TabularList.Desc = textViewDesc.Buffer.Text;

            TreeIter iter;

            //Поля
            TabularList.Fields.Clear();
            if (listStore.GetIterFirst(out iter))
                do
                {
                    if ((bool)listStore.GetValue(iter, (int)Columns.Visible))
                    {
                        string name = (string)listStore.GetValue(iter, (int)Columns.Name);
                        string caption = (string)listStore.GetValue(iter, (int)Columns.Caption);
                        uint size = (uint)listStore.GetValue(iter, (int)Columns.Size);
                        int sortNum = (int)listStore.GetValue(iter, (int)Columns.SortNum);
                        bool sortField = (bool)listStore.GetValue(iter, (int)Columns.SortField);

                        ConfigurationObjectField field = Fields[name];
                        TabularList.AppendField(new ConfigurationTabularListField(field.Name, caption, size, sortNum, sortField));
                    }
                }
                while (listStore.IterNext(ref iter));

            //Додаткові поля
            TabularList.AdditionalFields.Clear();
            if (listStoreAdditional.GetIterFirst(out iter))
                do
                {
                    bool visible = (bool)listStoreAdditional.GetValue(iter, (int)ColumnsAdditional.Visible);
                    string name = (string)listStoreAdditional.GetValue(iter, (int)ColumnsAdditional.Name);

                    string newName = name;
                    //Перевірка унікальності імені поля і створення нового імені
                    for (int i = 1; TabularList.AdditionalFields.ContainsKey(newName) && i < 100; i++)
                        newName = name + i;

                    name = newName;

                    string caption = (string)listStoreAdditional.GetValue(iter, (int)ColumnsAdditional.Caption);
                    uint size = (uint)listStoreAdditional.GetValue(iter, (int)ColumnsAdditional.Size);
                    int sortNum = (int)listStoreAdditional.GetValue(iter, (int)ColumnsAdditional.SortNum);
                    string type = (string)listStoreAdditional.GetValue(iter, (int)ColumnsAdditional.Type);
                    if (!AdditionalTypeList.Contains(type))
                        type = "string";

                    string value = (string)listStoreAdditional.GetValue(iter, (int)ColumnsAdditional.Value);

                    TabularList.AppendAdditionalField(new ConfigurationTabularListAdditionalField(visible, name, caption, size, sortNum, type, value));
                }
                while (listStoreAdditional.IterNext(ref iter));

            FillTreeAdditionalView();
        }

        #endregion

        #region AdditionalFields

        void OnAdditionalFieldsAddClick(object? sender, EventArgs args)
        {
            listStoreAdditional.AppendValues(false, "field", "field", 0, 100, "string", "");
        }

        void OnAdditionalFieldsCopyClick(object? sender, EventArgs args)
        {
            if (treeViewAdditional.Selection.CountSelectedRows() != 0)
            {
                TreePath[] selectionRows = treeViewAdditional.Selection.GetSelectedRows();
                foreach (TreePath itemPath in selectionRows)
                {
                    TreeIter iter;
                    treeViewAdditional.Model.GetIter(out iter, itemPath);

                    TreeIter newIter = listStoreAdditional.Append();

                    for (int i = 0; i < Enum.GetValues(typeof(ColumnsAdditional)).Length; i++)
                        if (i == (int)ColumnsAdditional.SortNum)
                            listStoreAdditional.SetValue(newIter, i, 100);
                        else
                            listStoreAdditional.SetValue(newIter, i, listStoreAdditional.GetValue(iter, i));
                }
            }
        }

        void OnAdditionalFieldsRemoveClick(object? sender, EventArgs args)
        {
            if (treeViewAdditional.Selection.CountSelectedRows() != 0)
            {
                TreePath[] selectionRows = treeViewAdditional.Selection.GetSelectedRows();
                for (int i = selectionRows.Length - 1; i >= 0; i--)
                {
                    TreeIter iter;
                    if (treeViewAdditional.Model.GetIter(out iter, selectionRows[i]))
                        listStoreAdditional.Remove(ref iter);
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
                if (TabularLists.ContainsKey(entryName.Text))
                {
                    Message.Error(GeneralForm, $"Назва табличного списку не унікальна");
                    return;
                }
            }
            else
            {
                if (TabularList.Name != entryName.Text)
                {
                    if (TabularLists.ContainsKey(entryName.Text))
                    {
                        Message.Error(GeneralForm, $"Назва табличного списку не унікальна");
                        return;
                    }
                }

                TabularLists.Remove(TabularList.Name);
            }

            GetValue();

            TabularLists.Add(TabularList.Name, TabularList);

            IsNew = false;

            GeneralForm?.RenameCurrentPageNotebook($"Табличний список: {TabularList.Name}");

            if (CallBack_RefreshList != null)
                CallBack_RefreshList.Invoke();
        }
    }
}