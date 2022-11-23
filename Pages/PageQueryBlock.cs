using Gtk;

using AccountingSoftware;

namespace Configurator
{
    class PageQueryBlock : VBox
    {
        Configuration? Conf
        {
            get
            {
                return Program.Kernel?.Conf;
            }
        }

        public Dictionary<string, ConfigurationObjectQueryBlock> QueryBlockList { get; set; } = new Dictionary<string, ConfigurationObjectQueryBlock>();
        public ConfigurationObjectQueryBlock QueryBlock { get; set; } = new ConfigurationObjectQueryBlock();
        public FormConfigurator? GeneralForm { get; set; }
        public System.Action? CallBack_RefreshList { get; set; }
        public bool IsNew { get; set; } = true;

        #region Fields

        ListBox listBoxQuery = new ListBox() { SelectionMode = SelectionMode.Single };
        Entry entryName = new Entry() { WidthRequest = 500 };
        Entry entryTable = new Entry() { WidthRequest = 500 };
        TextView textViewDesc = new TextView();

        #endregion

        public PageQueryBlock() : base()
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

        void CreatePack2(HPaned hPaned)
        {
            VBox vBox = new VBox();

            HBox hBox = new HBox();
            hBox.PackStart(new Label("Поля:"), false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(Stock.New) { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnQueryListAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(Stock.Copy) { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnQueryListCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(Stock.Refresh) { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnQueryListRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(Stock.Clear) { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnQueryListRemoveClick;
            toolbar.Add(buttonDelete);

            HBox hBoxScroll = new HBox();
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 300);

            listBoxQuery.ButtonPressEvent += OnQueryListButtonPress;

            scrollList.Add(listBoxQuery);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);
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

            hPaned.Pack1(vBox, false, false);
        }

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            FillQueryList();

            entryName.Text = QueryBlock.Name;
        }

        void FillQueryList()
        {
            foreach (KeyValuePair<int, string> query in QueryBlock.Query)
                listBoxQuery.Add(new Label(query.Key.ToString()) { Name = query.Key.ToString(), Halign = Align.Start });
        }

        void GetValue()
        {
            QueryBlock.Name = entryName.Text;
        }

        #endregion

        void OnSaveClick(object? sender, EventArgs args)
        {
            if (IsNew)
            {
                if (QueryBlockList.ContainsKey(entryName.Text))
                {
                    Message.Error(GeneralForm, $"Назва не унікальна");
                    return;
                }
            }
            else
            {
                if (QueryBlock.Name != entryName.Text)
                {
                    if (QueryBlockList.ContainsKey(entryName.Text))
                    {
                        Message.Error(GeneralForm, $"Назва не унікальна");
                        return;
                    }
                }

                QueryBlockList.Remove(QueryBlock.Name);
            }

            GetValue();

            QueryBlockList.Add(QueryBlock.Name, QueryBlock);

            IsNew = false;

            GeneralForm?.RenameCurrentPageNotebook($"Query: {QueryBlock.Name}");

            if (CallBack_RefreshList != null)
                CallBack_RefreshList.Invoke();
        }

        void OnQueryListButtonPress(object? sender, ButtonPressEventArgs args)
        {
            if (args.Event.Type == Gdk.EventType.DoubleButtonPress)
            {
                ListBoxRow[] selectedRows = listBoxQuery.SelectedRows;

                if (selectedRows.Length != 0)
                {
                    ListBoxRow curRow = selectedRows[0];

                    // if (TablePart.Fields.ContainsKey(curRow.Child.Name))
                    //     GeneralForm?.CreateNotebookPage($"Поле: {curRow.Child.Name}", () =>
                    //     {
                    //         PageField page = new PageField()
                    //         {
                    //             Fields = TablePart.Fields,
                    //             Field = TablePart.Fields[curRow.Child.Name],
                    //             IsNew = false,
                    //             GeneralForm = GeneralForm,
                    //             CallBack_RefreshList = TabularPartsRefreshList
                    //         };

                    //         page.SetValue();

                    //         return page;
                    //     });
                }
            }
        }

        void OnQueryListAddClick(object? sender, EventArgs args)
        {
            // GeneralForm?.CreateNotebookPage("Поле: *", () =>
            // {
            //     PageField page = new PageField()
            //     {
            //         Table = TablePart.Table,
            //         Fields = TablePart.Fields,
            //         IsNew = true,
            //         GeneralForm = GeneralForm,
            //         CallBack_RefreshList = TabularPartsRefreshList
            //     };

            //     page.SetValue();

            //     return page;
            // });
        }

        void OnQueryListCopyClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxQuery.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (QueryBlockList.ContainsKey(row.Child.Name))
                    {
                        ConfigurationObjectQueryBlock newQueryBlock = QueryBlockList[row.Child.Name].Copy();
                        newQueryBlock.Name += GenerateName.GetNewName();

                        QueryBlockList.Add(newQueryBlock.Name, newQueryBlock);
                    }
                }

                QueryListRefreshList();

                GeneralForm?.LoadTreeAsync();
            }
        }

        void OnQueryListRefreshClick(object? sender, EventArgs args)
        {
            foreach (Widget item in listBoxQuery.Children)
                listBoxQuery.Remove(item);

            FillQueryList();

            listBoxQuery.ShowAll();
        }

        void OnQueryListRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxQuery.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (QueryBlockList.ContainsKey(row.Child.Name))
                        QueryBlockList.Remove(row.Child.Name);
                }

                QueryListRefreshList();
            }
        }

        void QueryListRefreshList()
        {
            OnQueryListRefreshClick(null, new EventArgs());
        }
    }
}