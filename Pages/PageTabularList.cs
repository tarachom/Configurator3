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
        Configuration? Conf
        {
            get
            {
                return Program.Kernel?.Conf;
            }
        }

        public Dictionary<string, ConfigurationObjectField> Fields = new Dictionary<string, ConfigurationObjectField>();
        public Dictionary<string, ConfigurationTabularList> TabularLists { get; set; } = new Dictionary<string, ConfigurationTabularList>();
        public ConfigurationTabularList TabularList { get; set; } = new ConfigurationTabularList();
        public FormConfigurator? GeneralForm { get; set; }
        public System.Action? CallBack_RefreshList { get; set; }
        public bool IsNew { get; set; } = true;

        #region Fields

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

            HPaned hPaned = new HPaned() { BorderWidth = 5 };

            CreatePack1(hPaned);
            CreatePack2(hPaned);

            PackStart(hPaned, false, false, 5);

            ShowAll();
        }

        void CreatePack2(HPaned hPaned)
        {
            VBox vBox = new VBox();

            HBox hBoxTreeView = new HBox();
            hBoxTreeView.PackStart(new Label("Поля:"), false, false, 5);
            vBox.PackStart(hBoxTreeView, false, false, 5);

            HBox hBoxScrollTreeView = new HBox();
            ScrolledWindow scrollTreeView = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollTreeView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollTreeView.SetSizeRequest(0, 500);

            scrollTreeView.Add(treeViewFields);
            hBoxScrollTreeView.PackStart(scrollTreeView, true, true, 5);

            vBox.PackStart(hBoxScrollTreeView, false, false, 0);
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
            CellRendererToggle visibleField = new CellRendererToggle();
            visibleField.Toggled += EditedVisible;
            treeViewFields.AppendColumn(new TreeViewColumn("", visibleField, "active", 0));

            treeViewFields.AppendColumn(new TreeViewColumn("Назва", new CellRendererText(), "text", 1));

            CellRendererText textCaption = new CellRendererText() { Editable = true };
            textCaption.Edited += EditedCaption;
            treeViewFields.AppendColumn(new TreeViewColumn("Заголовок", textCaption, "text", 2));

            CellRendererText textSize = new CellRendererText() { Editable = true };
            textSize.Edited += EditedSize;
            treeViewFields.AppendColumn(new TreeViewColumn("Розмір", textSize, "text", 3));

            CellRendererText textSortNum = new CellRendererText() { Editable = true };
            textSortNum.Edited += EditedSortNum;

            treeViewFields.AppendColumn(new TreeViewColumn("Порядок", textSortNum, "text", 4));
            listStore.SetSortColumnId(4, SortType.Ascending);

            CellRendererToggle sortField = new CellRendererToggle();
            sortField.Toggled += EditedSortField;
            treeViewFields.AppendColumn(new TreeViewColumn("Сортувати", sortField, "active", 5));

            treeViewFields.AppendColumn(new TreeViewColumn("Тип", new CellRendererText(), "text", 6));
        }

        private void EditedVisible(object o, ToggledArgs args)
        {
            Gtk.TreeIter iter;
            if (listStore.GetIterFromString(out iter, args.Path))
            {
                bool val = (bool)listStore.GetValue(iter, 0);
                listStore.SetValue(iter, 0, !val);
            }
        }

        private void EditedSortField(object o, ToggledArgs args)
        {
            Gtk.TreeIter iter;
            if (listStore.GetIterFromString(out iter, args.Path))
            {
                bool val = (bool)listStore.GetValue(iter, 5);
                listStore.SetValue(iter, 5, !val);
            }
        }

        private void EditedCaption(object o, EditedArgs args)
        {
            Gtk.TreeIter iter;
            if (listStore.GetIterFromString(out iter, args.Path))
                listStore.SetValue(iter, 2, args.NewText);
        }

        private void EditedSize(object o, EditedArgs args)
        {
            Gtk.TreeIter iter;
            if (listStore.GetIterFromString(out iter, args.Path))
            {
                uint size;
                uint.TryParse(args.NewText, out size);

                listStore.SetValue(iter, 3, size);
            }
        }

        private void EditedSortNum(object o, EditedArgs args)
        {
            Gtk.TreeIter iter;
            if (listStore.GetIterFromString(out iter, args.Path))
            {
                uint sortNum;
                uint.TryParse(args.NewText, out sortNum);

                listStore.SetValue(iter, 4, sortNum);
            }
        }

        #endregion

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            FillTreeView();

            if (IsNew)
                TabularList.Name = "Записи";

            entryName.Text = TabularList.Name;
            checkButtonIsTree.Active = TabularList.IsTree;
            textViewDesc.Buffer.Text = TabularList.Desc;
        }

        void FillTreeView()
        {
            foreach (ConfigurationObjectField field in Fields.Values)
            {
                bool isExistField = TabularList.Fields.ContainsKey(field.Name);

                string caption = isExistField ?
                    (!String.IsNullOrEmpty(TabularList.Fields[field.Name].Caption) ?
                        TabularList.Fields[field.Name].Caption : field.Name) : field.Name;

                uint size = isExistField ? TabularList.Fields[field.Name].Size : 0;
                int sortNum = isExistField ? TabularList.Fields[field.Name].SortNum : 100;
                bool sortField = isExistField ? TabularList.Fields[field.Name].SortField : false;
                string sType = field.Type == "pointer" || field.Type == "enum" ? field.Pointer : field.Type;

                listStore.AppendValues(isExistField, field.Name, caption, size, sortNum, sortField, sType);
            }
        }

        void GetValue()
        {
            TabularList.Name = entryName.Text;
            TabularList.IsTree = checkButtonIsTree.Active;
            TabularList.Desc = textViewDesc.Buffer.Text;

            //Доспупні поля
            TabularList.Fields.Clear();

            TreeIter iter;
            if (listStore.GetIterFirst(out iter))
                do
                {
                    if ((bool)listStore.GetValue(iter, 0))
                    {
                        string name = (string)listStore.GetValue(iter, 1);
                        string caption = (string)listStore.GetValue(iter, 2);
                        uint size = (uint)listStore.GetValue(iter, 3);
                        int sortNum = (int)listStore.GetValue(iter, 4);
                        bool sortField = (bool)listStore.GetValue(iter, 5);

                        ConfigurationObjectField field = Fields[name];
                        TabularList.AppendField(new ConfigurationTabularListField(field.Name, caption, size, sortNum, sortField));
                    }
                }
                while (listStore.IterNext(ref iter));
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