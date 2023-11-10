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

namespace Configurator
{
    class PageUsersList : VBox
    {
        public FormConfigurator? GeneralForm { get; set; }

        #region Fields

        enum Columns
        {
            Image,
            UID,
            Name,
            FullName,
            DateAdd,
            DateUpdate,
            Info
        }

        ListStore Store = new ListStore(
            typeof(Gdk.Pixbuf),
            typeof(string), //UID
            typeof(string), //Name
            typeof(string), //FullName
            typeof(string), //DateAdd
            typeof(string), //DateUpdate
            typeof(string)  //Info
        );

        TreeView TreeViewGrid;

        #endregion

        public PageUsersList() : base()
        {
            HBox hBox = new HBox();

            Button bClose = new Button("Закрити");
            bClose.Clicked += (object? sender, EventArgs args) => { GeneralForm?.CloseCurrentPageNotebook(); };
            hBox.PackStart(bClose, false, false, 10);

            PackStart(hBox, false, false, 10);

            TreeViewGrid = new TreeView(Store);
            TreeViewGrid.Selection.Mode = SelectionMode.Multiple;
            TreeViewGrid.ActivateOnSingleClick = false;
            TreeViewGrid.RowActivated += OnRowActivated;

            AddColumn();

            CreateToolbar();

            HBox hBoxList = new HBox();

            ScrolledWindow scrollTree = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollTree.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollTree.Add(TreeViewGrid);

            hBoxList.PackStart(scrollTree, true, true, 5);
            PackStart(hBoxList, true, true, 0);

            HBox hBoxBottom = new HBox();
            PackStart(hBoxBottom, false, false, 2);

            ShowAll();
        }

        public void LoadRecords()
        {
            Store.Clear();

            List<Dictionary<string, object>> listRow = Program.Kernel!.DataBase.SpetialTableUsersExtendetList();

            foreach (Dictionary<string, object> record in listRow)
            {
                Store.AppendValues(
                    new Gdk.Pixbuf(AppContext.BaseDirectory + "images/doc.png"),
                    record["uid"].ToString(),
                    record["name"].ToString(),
                    record["fullname"].ToString(),
                    record["dateadd"].ToString(),
                    record["dateupdate"].ToString(),
                    record["info"]
                );
            }
        }

        void CreateToolbar()
        {
            HBox hBoxToolbar = new HBox();

            Toolbar toolbar = new Toolbar();
            hBoxToolbar.PackStart(toolbar, true, true, 5);

            ToolButton addButton = new ToolButton(Stock.Add) { TooltipText = "Додати" };
            addButton.Clicked += OnAddClick;
            toolbar.Add(addButton);

            ToolButton upButton = new ToolButton(Stock.Edit) { TooltipText = "Редагувати" };
            upButton.Clicked += OnEditClick;
            toolbar.Add(upButton);

            ToolButton deleteButton = new ToolButton(Stock.Delete) { TooltipText = "Видалити" };
            deleteButton.Clicked += OnDeleteClick;
            toolbar.Add(deleteButton);

            ToolButton refreshButton = new ToolButton(Stock.Refresh) { TooltipText = "Обновити" };
            refreshButton.Clicked += OnRefreshClick;
            toolbar.Add(refreshButton);

            PackStart(hBoxToolbar, false, false, 0);
        }

        #region TreeView

        void AddColumn()
        {
            TreeViewGrid.AppendColumn(new TreeViewColumn("", new CellRendererPixbuf(), "pixbuf", (int)Columns.Image));
            TreeViewGrid.AppendColumn(new TreeViewColumn("UID", new CellRendererText(), "text", (int)Columns.UID) { Visible = false });
            TreeViewGrid.AppendColumn(new TreeViewColumn("Логін", new CellRendererText(), "text", (int)Columns.Name));
            TreeViewGrid.AppendColumn(new TreeViewColumn("Назва", new CellRendererText(), "text", (int)Columns.FullName));
            TreeViewGrid.AppendColumn(new TreeViewColumn("Додано", new CellRendererText(), "text", (int)Columns.DateUpdate));
            TreeViewGrid.AppendColumn(new TreeViewColumn("Обновлено", new CellRendererText(), "text", (int)Columns.DateUpdate));
            TreeViewGrid.AppendColumn(new TreeViewColumn("Інфо", new CellRendererText(), "text", (int)Columns.Info));

            //Пустишка
            TreeViewGrid.AppendColumn(new TreeViewColumn());
        }

        #endregion

        #region ToolBar

        void OnAddClick(object? sender, EventArgs args)
        {
            GeneralForm?.CreateNotebookPage("Користувач *", () =>
            {
                PageUser page = new PageUser()
                {
                    IsNew = true,
                    GeneralForm = GeneralForm,
                    CallBack_RefreshList = LoadRecords
                };

                return page;
            });
        }

        void OnEditClick(object? sender, EventArgs args)
        {
            if (TreeViewGrid.Selection.CountSelectedRows() != 0)
            {
                TreeIter iter;
                if (TreeViewGrid.Model.GetIter(out iter, TreeViewGrid.Selection.GetSelectedRows()[0]))
                {
                    string uid = (string)TreeViewGrid.Model.GetValue(iter, (int)Columns.UID);
                    string fullName = (string)TreeViewGrid.Model.GetValue(iter, (int)Columns.FullName);

                    GeneralForm?.CreateNotebookPage($"Користувач: {fullName}", () =>
                    {
                        PageUser page = new PageUser()
                        {
                            IsNew = false,
                            UID = Guid.Parse(uid),
                            GeneralForm = GeneralForm,
                            CallBack_RefreshList = LoadRecords
                        };

                        page.SetValue();

                        return page;
                    });
                }
            }
        }

        void OnRowActivated(object sender, RowActivatedArgs args)
        {
            OnEditClick(sender, new EventArgs());
        }

        void OnRefreshClick(object? sender, EventArgs args)
        {
            LoadRecords();
        }

        async void OnDeleteClick(object? sender, EventArgs args)
        {
            if (TreeViewGrid.Selection.CountSelectedRows() != 0)
            {
                if (Message.Request(GeneralForm, "Видалити?") == ResponseType.Yes)
                {
                    TreePath[] selectionRows = TreeViewGrid.Selection.GetSelectedRows();

                    foreach (TreePath itemPath in selectionRows)
                    {
                        TreeIter iter;
                        TreeViewGrid.Model.GetIter(out iter, itemPath);

                        string uid = (string)TreeViewGrid.Model.GetValue(iter, 1);
                        string name = (string)TreeViewGrid.Model.GetValue(iter, (int)Columns.Name);

                        if (Program.Kernel != null && !await Program.Kernel.DataBase.SpetialTableUsersDelete(Guid.Parse(uid), name))
                            Message.Error(GeneralForm, "Не вдалось видалити користувача");
                    }

                    LoadRecords();
                }
            }
        }

        #endregion
    }
}