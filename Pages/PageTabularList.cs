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

namespace Configurator
{
    class PageTabularList : Box
    {
        //Configuration Conf { get { return Program.Kernel.Conf; } }

        public Dictionary<string, ConfigurationField> Fields = [];
        public Dictionary<string, ConfigurationTabularList> TabularLists { get; set; } = [];
        public ConfigurationTabularList TabularList { get; set; } = new ConfigurationTabularList();
        public FormConfigurator? GeneralForm { get; set; }
        public System.Action? CallBack_RefreshList { get; set; }
        public bool IsNew { get; set; } = true;

        public string ConfOwnerName { get; set; } = ""; /* Документи, Довідники  !!! Видалити */

        Entry entryName = new Entry() { WidthRequest = 150 };
        TextView textViewDesc = new TextView() { WrapMode = WrapMode.Word };

        #region Fields

        enum Columns
        {
            Visible,
            Name,
            Caption,
            Size,
            SortNum,
            SortField,
            SortDirection,
            FilterField,
            Type
        }

        ListStore listStore = new ListStore(
            typeof(bool),   //Visible
            typeof(string), //Name
            typeof(string), //Caption
            typeof(uint),   //Size
            typeof(int),    //SortNum
            typeof(bool),   //SortField
            typeof(bool),   //SortDirection
            typeof(bool),   //FilterField
            typeof(string)  //Type
        );

        TreeView treeViewFields;

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

        string[] AdditionalTypeList = ["string"]; //, "integer", "numeric", "boolean", "image"];

        TreeView treeViewAdditional;

        #endregion

        public PageTabularList() : base(Orientation.Vertical, 0)
        {
            Box hBox = new Box(Orientation.Horizontal, 0);

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
            treeViewAdditional.ButtonPressEvent += OnTreeViewAdditionalPressEvent;
            AddColumnTreeViewAdditional();

            Paned hPaned = new Paned(Orientation.Horizontal) { BorderWidth = 5 };

            CreatePack1(hPaned);
            CreatePack2(hPaned);

            PackStart(hPaned, false, false, 5);

            ShowAll();
        }

        void CreatePack2(Paned hPaned)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            //Поля
            {
                Box hBoxCaption = new Box(Orientation.Horizontal, 0);
                hBoxCaption.PackStart(new Label("Поля:"), false, false, 5);
                vBox.PackStart(hBoxCaption, false, false, 5);

                CreateToolbarField(vBox);

                Box hBoxScroll = new Box(Orientation.Horizontal, 0);
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
                Box hBoxCaption = new Box(Orientation.Horizontal, 0);
                hBoxCaption.PackStart(new Label("Додаткові поля:"), false, false, 5);
                vBox.PackStart(hBoxCaption, false, false, 5);

                CreateToolbarAdditionalField(vBox);

                //Tree
                Box hBoxScroll = new Box(Orientation.Horizontal, 0);
                ScrolledWindow scroll = new ScrolledWindow() { ShadowType = ShadowType.In };
                scroll.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
                scroll.SetSizeRequest(0, 400);

                scroll.Add(treeViewAdditional);
                hBoxScroll.PackStart(scroll, true, true, 5);

                vBox.PackStart(hBoxScroll, false, false, 0);
            }

            hPaned.Pack2(vBox, true, false);
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

            ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 168, HeightRequest = 100 };
            scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollTextView.Add(textViewDesc);

            hBoxDesc.PackStart(scrollTextView, false, false, 5);

            hPaned.Pack1(vBox, false, false);
        }

        #region TreeView

        void CreateToolbarField(Box vBox)
        {
            void SelectAll()
            {
                if (listStore.GetIterFirst(out TreeIter iter))
                    do
                        listStore.SetValue(iter, (int)Columns.Visible, true);
                    while (listStore.IterNext(ref iter));
            }

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAll = new ToolButton(new Image(Stock.SortAscending, IconSize.Menu), "Вибрати всі") { TooltipText = "Вибрати всі" };
            buttonAll.Clicked += (object? sender, EventArgs args) => SelectAll();
            toolbar.Add(buttonAll);

            ToolButton addUp = new ToolButton(new Image(Stock.GoUp, IconSize.Menu), "Up") { TooltipText = "Up" };
            addUp.Clicked += (object? sender, EventArgs args) => TreeViewFunc.SortTreeView(treeViewFields, (int)Columns.SortNum, false);
            toolbar.Add(addUp);

            ToolButton addDown = new ToolButton(new Image(Stock.GoDown, IconSize.Menu), "Down") { TooltipText = "Down" };
            addDown.Clicked += (object? sender, EventArgs args) => TreeViewFunc.SortTreeView(treeViewFields, (int)Columns.SortNum, true);
            toolbar.Add(addDown);
        }

        void CreateToolbarAdditionalField(Box vBox)
        {
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

            ToolButton addUp = new ToolButton(new Image(Stock.GoUp, IconSize.Menu), "Up") { TooltipText = "Up" };
            addUp.Clicked += (object? sender, EventArgs args) => TreeViewFunc.SortTreeView(treeViewAdditional, (int)Columns.SortNum, false);
            toolbar.Add(addUp);

            ToolButton addDown = new ToolButton(new Image(Stock.GoDown, IconSize.Menu), "Down") { TooltipText = "Down" };
            addDown.Clicked += (object? sender, EventArgs args) => TreeViewFunc.SortTreeView(treeViewAdditional, (int)Columns.SortNum, true);
            toolbar.Add(addDown);
        }

        void AddColumnTreeViewFields()
        {
            //Visible
            {
                CellRendererToggle cell = new CellRendererToggle();
                cell.Toggled += (object o, ToggledArgs args) =>
                {
                    if (listStore.GetIterFromString(out TreeIter iter, args.Path))
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
                    if (listStore.GetIterFromString(out TreeIter iter, args.Path))
                        listStore.SetValue(iter, (int)Columns.Caption, args.NewText);
                };
                treeViewFields.AppendColumn(new TreeViewColumn("Заголовок", cell, "text", Columns.Caption));
            }

            //Size
            {
                CellRendererText cell = new CellRendererText() { Editable = true, Xalign = 1 };
                cell.Edited += (object o, EditedArgs args) =>
                {
                    if (listStore.GetIterFromString(out TreeIter iter, args.Path))
                    {
                        uint.TryParse(args.NewText, out uint size);
                        listStore.SetValue(iter, (int)Columns.Size, size);
                    }
                };
                treeViewFields.AppendColumn(new TreeViewColumn("Розмір", cell, "text", Columns.Size) { Alignment = 1 });
            }

            //SortNum
            {
                CellRendererText cell = new CellRendererText() { Editable = true, Xalign = 1 };
                cell.Edited += (object o, EditedArgs args) =>
                {
                    if (listStore.GetIterFromString(out TreeIter iter, args.Path))
                    {
                        uint.TryParse(args.NewText, out uint sortNum);
                        listStore.SetValue(iter, (int)Columns.SortNum, sortNum);
                    }
                };
                treeViewFields.AppendColumn(new TreeViewColumn("Порядок", cell, "text", Columns.SortNum) { Alignment = 1 });
                listStore.SetSortColumnId((int)Columns.SortNum, SortType.Ascending);
            }

            //SortField
            {
                TreeViewColumn treeViewColumn = new TreeViewColumn() { Title = "Сортувати|Desc" };
                
                //Sort
                {
                    CellRendererToggle cell = new CellRendererToggle() { Xalign = 0.5f };
                    cell.Toggled += (object o, ToggledArgs args) =>
                    {
                        if (listStore.GetIterFromString(out TreeIter iter, args.Path))
                        {
                            bool val = (bool)listStore.GetValue(iter, (int)Columns.SortField);
                            listStore.SetValue(iter, (int)Columns.SortField, !val);
                        }
                    };

                    treeViewColumn.PackStart(cell, true);
                    treeViewColumn.AddAttribute(cell, "active", (int)Columns.SortField);
                }

                //Direction
                {
                    CellRendererToggle cell = new CellRendererToggle() { Xalign = 0.5f };
                    cell.Toggled += (object o, ToggledArgs args) =>
                    {
                        if (listStore.GetIterFromString(out TreeIter iter, args.Path))
                        {
                            bool val = (bool)listStore.GetValue(iter, (int)Columns.SortDirection);
                            listStore.SetValue(iter, (int)Columns.SortDirection, !val);
                        }
                    };

                    treeViewColumn.PackEnd(cell, true);
                    treeViewColumn.AddAttribute(cell, "active", (int)Columns.SortDirection);
                }

                treeViewFields.AppendColumn(treeViewColumn);
            }

            //FilterField
            {
                CellRendererToggle cell = new CellRendererToggle();
                cell.Toggled += (object o, ToggledArgs args) =>
                {
                    if (listStore.GetIterFromString(out TreeIter iter, args.Path))
                    {
                        bool val = (bool)listStore.GetValue(iter, (int)Columns.FilterField);
                        listStore.SetValue(iter, (int)Columns.FilterField, !val);
                    }
                };
                treeViewFields.AppendColumn(new TreeViewColumn("Фільтр", cell, "active", Columns.FilterField));
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
                    if (listStoreAdditional.GetIterFromString(out TreeIter iter, args.Path))
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
                    if (listStoreAdditional.GetIterFromString(out TreeIter iter, args.Path))
                        listStoreAdditional.SetValue(iter, (int)ColumnsAdditional.Name, args.NewText);
                };
                treeViewAdditional.AppendColumn(new TreeViewColumn("Назва", cell, "text", ColumnsAdditional.Name) { MinWidth = 150 });
            }

            //Caption
            {
                CellRendererText cell = new CellRendererText() { Editable = true };
                cell.Edited += (object o, EditedArgs args) =>
                {
                    if (listStoreAdditional.GetIterFromString(out TreeIter iter, args.Path))
                        listStoreAdditional.SetValue(iter, (int)ColumnsAdditional.Caption, args.NewText);
                };
                treeViewAdditional.AppendColumn(new TreeViewColumn("Заголовок", cell, "text", ColumnsAdditional.Caption) { MinWidth = 150 });
            }

            //Size
            {
                CellRendererText cell = new CellRendererText() { Editable = true, Xalign = 1 };
                cell.Edited += (object o, EditedArgs args) =>
                {
                    if (listStoreAdditional.GetIterFromString(out TreeIter iter, args.Path))
                    {
                        uint.TryParse(args.NewText, out uint size);
                        listStoreAdditional.SetValue(iter, (int)ColumnsAdditional.Size, size);
                    }
                };
                treeViewAdditional.AppendColumn(new TreeViewColumn("Розмір", cell, "text", ColumnsAdditional.Size) { Alignment = 1 });
            }

            //SortNum
            {
                CellRendererText cell = new CellRendererText() { Editable = true, Xalign = 1 };
                cell.Edited += (object o, EditedArgs args) =>
                {
                    if (listStoreAdditional.GetIterFromString(out TreeIter iter, args.Path))
                    {
                        uint.TryParse(args.NewText, out uint sortNum);
                        listStoreAdditional.SetValue(iter, (int)ColumnsAdditional.SortNum, sortNum);
                    }
                };
                treeViewAdditional.AppendColumn(new TreeViewColumn("Порядок", cell, "text", ColumnsAdditional.SortNum) { Alignment = 1 });
                listStoreAdditional.SetSortColumnId((int)ColumnsAdditional.SortNum, SortType.Ascending);
            }

            //Type
            {
                CellRendererCombo cell = new CellRendererCombo() { Editable = true, Model = listStoreAdditionalType, TextColumn = 0 };
                cell.Edited += (object sender, EditedArgs args) =>
                {
                    if (listStoreAdditional.GetIterFromString(out TreeIter iter, args.Path))
                        listStoreAdditional.SetValue(iter, (int)ColumnsAdditional.Type, args.NewText);
                };
                treeViewAdditional.AppendColumn(new TreeViewColumn("Тип", cell, "text", ColumnsAdditional.Type) { MinWidth = 100 });
            }

            //Value
            {
                TreeViewColumn Column = new TreeViewColumn("Значення", new CellRendererText() { SingleParagraphMode = true }, "text", ColumnsAdditional.Value) { MinWidth = 300, MaxWidth = 500 };
                Column.Data.Add("Column", ColumnsAdditional.Value);

                treeViewAdditional.AppendColumn(Column);
            }

            //Пустишка
            treeViewAdditional.AppendColumn(new TreeViewColumn());
        }

        #endregion

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            FillTreeView();
            FillTreeAdditionalView();

            if (IsNew)
                TabularList.Name = "Записи";

            entryName.Text = TabularList.Name;
            //checkButtonIsTree.Active = TabularList.IsTree;
            textViewDesc.Buffer.Text = TabularList.Desc;

            //Типи даних для додаткових полів
            foreach (string type in AdditionalTypeList)
                listStoreAdditionalType.AppendValues(type);
        }

        void FillTreeView()
        {
            int counter = 0;
            foreach (ConfigurationField field in Fields.Values)
            {
                bool isExistField = TabularList.Fields.ContainsKey(field.Name);

                string caption = isExistField ?
                    (!string.IsNullOrEmpty(TabularList.Fields[field.Name].Caption) ?
                        TabularList.Fields[field.Name].Caption : field.Name) : field.Name;

                uint size = isExistField ? TabularList.Fields[field.Name].Size : 0;
                int sortNum = isExistField ? TabularList.Fields[field.Name].SortNum : IsNew ? ++counter : 100;
                bool sortField = isExistField ? TabularList.Fields[field.Name].SortField : false;
                bool sortDirection = isExistField ? TabularList.Fields[field.Name].SortDirection : false;
                bool filterField = isExistField ? TabularList.Fields[field.Name].FilterField : false;
                string sType = field.Type == "pointer" || field.Type == "enum" ? field.Pointer : field.Type;

                listStore.AppendValues(isExistField, field.Name, caption, size, sortNum, sortField, sortDirection, filterField, sType);
            }
        }

        void FillTreeAdditionalView()
        {
            listStoreAdditional.Clear();
            foreach (ConfigurationTabularListAdditionalField field in TabularList.AdditionalFields.Values)
            {
                listStoreAdditional.AppendValues([
                    field.Visible,
                    field.Name,
                    field.Caption,
                    field.Size,
                    field.SortNum ,
                    field.Type,
                    field.Value
                ]);
            }
        }

        void GetValue()
        {
            TabularList.Name = entryName.Text;
            TabularList.Desc = textViewDesc.Buffer.Text;

            //Поля
            TabularList.Fields.Clear();
            if (listStore.GetIterFirst(out TreeIter iter))
                do
                {
                    if ((bool)listStore.GetValue(iter, (int)Columns.Visible))
                    {
                        string name = (string)listStore.GetValue(iter, (int)Columns.Name);
                        string caption = (string)listStore.GetValue(iter, (int)Columns.Caption);
                        uint size = (uint)listStore.GetValue(iter, (int)Columns.Size);
                        int sortNum = (int)listStore.GetValue(iter, (int)Columns.SortNum);
                        bool sortField = (bool)listStore.GetValue(iter, (int)Columns.SortField);
                        bool sortDirection = (bool)listStore.GetValue(iter, (int)Columns.SortDirection);
                        bool filterField = (bool)listStore.GetValue(iter, (int)Columns.FilterField);

                        ConfigurationField field = Fields[name];
                        TabularList.AppendField(new ConfigurationTabularListField(field.Name, caption, size, sortNum, sortField, sortDirection, filterField));
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
                    treeViewAdditional.Model.GetIter(out TreeIter iter, itemPath);
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
                    if (treeViewAdditional.Model.GetIter(out TreeIter iter, selectionRows[i]))
                        listStoreAdditional.Remove(ref iter);
            }
        }

        void OnTreeViewAdditionalPressEvent(object sender, ButtonPressEventArgs args)
        {
            if (args.Event.Button == 1 && args.Event.Type == Gdk.EventType.DoubleButtonPress && treeViewAdditional.Selection.CountSelectedRows() != 0)
            {
                treeViewAdditional.GetCursor(out TreePath itemPath, out TreeViewColumn treeColumn);
                if (treeColumn.Data.ContainsKey("Column"))
                {
                    treeViewAdditional.Model.GetIter(out TreeIter iter, itemPath);

                    //Швидкий вибір
                    Gdk.Rectangle rectangleCell = treeViewAdditional.GetCellArea(itemPath, treeColumn);
                    rectangleCell.Offset(0, rectangleCell.Height);
                    Popover popoverSmallSelect = new Popover(treeViewAdditional)
                    {
                        PointingTo = rectangleCell,
                        Position = PositionType.Left,
                        BorderWidth = 5
                    };

                    int rowNumber = int.Parse(itemPath.ToString());

                    if ((ColumnsAdditional)treeColumn.Data["Column"]! == ColumnsAdditional.Value)
                    {
                        Box vBox = new Box(Orientation.Vertical, 0);
                        SourceView sourceViewCode = new SourceView();
                        sourceViewCode.Buffer.Language = new LanguageManager().GetLanguage("sql");

                        //Кнопки
                        {
                            Box hBox = new Box(Orientation.Horizontal, 0);
                            vBox.PackStart(hBox, false, false, 5);

                            Button bSave = new Button("Зберегти");
                            bSave.Clicked += (object? sender, EventArgs args) =>
                            {
                                treeViewAdditional.Model.SetValue(iter, (int)ColumnsAdditional.Value, sourceViewCode.Buffer.Text);
                                popoverSmallSelect.Hide();
                            };
                            hBox.PackStart(bSave, false, false, 2);

                            Button bClose = new Button(new Image(AppContext.BaseDirectory + "images/clean.png")) { TooltipText = "Закрити" };
                            bClose.Clicked += (object? sender, EventArgs args) => { popoverSmallSelect.Hide(); };

                            hBox.PackEnd(bClose, false, false, 2);
                        }

                        //Text
                        {
                            Box hBox = new Box(Orientation.Horizontal, 0);
                            vBox.PackStart(hBox, false, false, 5);

                            sourceViewCode.Buffer.Text = (string)treeViewAdditional.Model.GetValue(iter, (int)ColumnsAdditional.Value);

                            ScrolledWindow scrollCode = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 1000, HeightRequest = 600 };
                            scrollCode.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
                            scrollCode.Add(sourceViewCode);

                            hBox.PackStart(scrollCode, false, false, 0);
                        }

                        popoverSmallSelect.Add(vBox);
                    }

                    popoverSmallSelect.ShowAll();
                }
            }
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
            CallBack_RefreshList?.Invoke();
        }
    }
}