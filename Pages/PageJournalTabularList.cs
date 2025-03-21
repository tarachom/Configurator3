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

namespace Configurator
{
    class PageJournalTabularList : Box
    {
        Configuration Conf { get { return Program.Kernel.Conf; } }

        public Dictionary<string, ConfigurationJournalField> Fields = new Dictionary<string, ConfigurationJournalField>();
        public Dictionary<string, ConfigurationTabularList> TabularLists { get; set; } = new Dictionary<string, ConfigurationTabularList>();
        public ConfigurationTabularList TabularList { get; set; } = new ConfigurationTabularList();
        public FormConfigurator? GeneralForm { get; set; }
        public System.Action? CallBack_RefreshList { get; set; }

        #region Fields

        enum Columns
        {
            Name,
            DocField
        };

        ListStore listStore = new ListStore(
            typeof(string), //Name
            typeof(string)  //DocField
        );

        //Для списку полів документу
        ListStore listStoreDocFields = new ListStore(typeof(string));

        TreeView treeViewFields;
        Entry entryName = new Entry() { WidthRequest = 250, Sensitive = false };
        TextView textViewDesc = new TextView() { WrapMode = WrapMode.Word };

        #endregion

        public PageJournalTabularList() : base(Orientation.Vertical, 0)
        {
            Box hBox = new Box(Orientation.Horizontal, 0);

            Button bSave = new Button("Зберегти");
            bSave.Clicked += OnSaveClick;

            hBox.PackStart(bSave, false, false, 10);

            Button bClose = new Button("Закрити");
            bClose.Clicked += (object? sender, EventArgs args) => GeneralForm?.CloseCurrentPageNotebook();

            hBox.PackStart(bClose, false, false, 10);

            PackStart(hBox, false, false, 10);

            treeViewFields = new TreeView(listStore);
            AddColumnTreeViewFields();

            Paned hPaned = new Paned(Orientation.Horizontal) { BorderWidth = 5 };

            CreatePack1(hPaned);
            CreatePack2(hPaned);

            PackStart(hPaned, false, false, 5);

            ShowAll();
        }

        void CreatePack2(Paned hPaned)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            Box hBoxTreeView = new Box(Orientation.Horizontal, 0);
            hBoxTreeView.PackStart(new Label("Поля:"), false, false, 5);
            vBox.PackStart(hBoxTreeView, false, false, 5);

            Box hBoxScrollTreeView = new Box(Orientation.Horizontal, 0);
            ScrolledWindow scrollTreeView = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollTreeView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollTreeView.SetSizeRequest(0, 500);

            scrollTreeView.Add(treeViewFields);
            hBoxScrollTreeView.PackStart(scrollTreeView, true, true, 5);

            vBox.PackStart(hBoxScrollTreeView, false, false, 0);
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

            ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 250, HeightRequest = 100 };
            scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollTextView.Add(textViewDesc);

            hBoxDesc.PackStart(scrollTextView, false, false, 5);

            hPaned.Pack1(vBox, false, false);
        }

        #region TreeView

        void AddColumnTreeViewFields()
        {
            treeViewFields.AppendColumn(new TreeViewColumn("Назва", new CellRendererText(), "text", (int)Columns.Name));

            CellRendererCombo DocField = new CellRendererCombo() { Editable = true, Model = listStoreDocFields, TextColumn = 0 };
            DocField.Edited += DocFieldChanged;

            treeViewFields.AppendColumn(new TreeViewColumn("Поле документу", DocField, "text", (int)Columns.DocField) { MinWidth = 400 });
            treeViewFields.AppendColumn(new TreeViewColumn());
        }

        void DocFieldChanged(object sender, EditedArgs args)
        {
            if (listStore.GetIterFromString(out TreeIter iter, args.Path))
                listStore.SetValue(iter, (int)Columns.DocField, args.NewText);
        }

        #endregion


        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            FillTreeView();

            if (Conf.Documents.TryGetValue(TabularList.Name, out ConfigurationDocuments? document))
                foreach (string field in document.Fields.Keys)
                    listStoreDocFields.AppendValues(field);

            entryName.Text = TabularList.Name;
            textViewDesc.Buffer.Text = TabularList.Desc;
        }

        void FillTreeView()
        {
            //Цикл по полях журналу
            foreach (ConfigurationJournalField field in Fields.Values)
            {
                string docField = "";

                if (TabularList.Fields.TryGetValue(field.Name, out ConfigurationTabularListField? value))
                {
                    ConfigurationTabularListField tabularListField = value;
                    docField = tabularListField.DocField;
                }

                listStore.AppendValues(field.Name, docField);
            }
        }

        void GetValue()
        {
            TabularList.Name = entryName.Text;
            TabularList.Desc = textViewDesc.Buffer.Text;

            //Доспупні поля
            TabularList.Fields.Clear();

            if (listStore.GetIterFirst(out TreeIter iter))
                do
                {
                    string name = (string)listStore.GetValue(iter, (int)Columns.Name);
                    string docField = (string)listStore.GetValue(iter, (int)Columns.DocField);

                    ConfigurationJournalField field = Fields[name];
                    TabularList.AppendField(new ConfigurationTabularListField(field.Name, docField));
                }
                while (listStore.IterNext(ref iter));
        }

        #endregion

        void OnSaveClick(object? sender, EventArgs args)
        {
            GetValue();
            TabularLists[TabularList.Name] = TabularList;
        }
    }
}