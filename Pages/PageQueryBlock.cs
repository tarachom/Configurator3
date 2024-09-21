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
    class PageQueryBlock : Box
    {
        Configuration Conf { get { return Program.Kernel.Conf; } }

        public Dictionary<string, ConfigurationQueryBlock> QueryBlockList { get; set; } = new Dictionary<string, ConfigurationQueryBlock>();
        public ConfigurationQueryBlock QueryBlock { get; set; } = new ConfigurationQueryBlock();
        public FormConfigurator? GeneralForm { get; set; }
        public System.Action? CallBack_RefreshList { get; set; }
        public bool IsNew { get; set; } = true;

        #region Fields

        ListBox listBoxQuery = new ListBox() { SelectionMode = SelectionMode.Single };
        Entry entryName = new Entry() { WidthRequest = 800 };
        CheckButton finalCalculation = new CheckButton("Для розрахунку підсумків (запити виконуються один раз, після всіх розрахунків, вкінці)");

        #endregion

        public PageQueryBlock() : base(Orientation.Vertical, 0)
        {
            Box hBox = new Box(Orientation.Horizontal, 0);

            Button bSave = new Button("Зберегти");
            bSave.Clicked += OnSaveClick;

            hBox.PackStart(bSave, false, false, 10);

            Button bClose = new Button("Закрити");
            bClose.Clicked += (object? sender, EventArgs args) => { GeneralForm?.CloseCurrentPageNotebook(); };

            hBox.PackStart(bClose, false, false, 10);

            PackStart(hBox, false, false, 10);

            Paned hPaned = new Paned(Orientation.Horizontal) { BorderWidth = 5, Position = 800 };

            CreatePack1(hPaned);
            CreatePack2(hPaned);

            PackStart(hPaned, false, false, 5);

            ShowAll();
        }

        void CreatePack1(Paned hPaned)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            //Назва
            Box hBoxName = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxName, false, false, 5);

            hBoxName.PackStart(new Label("Назва:"), false, false, 5);
            hBoxName.PackStart(entryName, false, false, 5);

            //finalCalculation
            Box hBoxFinalCalculation = new Box(Orientation.Horizontal, 0) { Halign = Align.Start };
            vBox.PackStart(hBoxFinalCalculation, false, false, 5);

            hBoxFinalCalculation.PackStart(finalCalculation, false, false, 5);

            //Список
            Box hBox = new Box(Orientation.Horizontal, 0);
            hBox.PackStart(new Label("Поля:"), false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(new Image(Stock.New, IconSize.Menu), "Додати") { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnQueryListAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(new Image(Stock.Copy, IconSize.Menu), "Копіювати") { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnQueryListCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(new Image(Stock.Refresh, IconSize.Menu), "Обновити") { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnQueryListRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(new Image(Stock.Clear, IconSize.Menu), "Видалити") { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnQueryListRemoveClick;
            toolbar.Add(buttonDelete);

            ToolButton buttonGoUp = new ToolButton(new Image(Stock.GoUp, IconSize.Menu), "Вверх") { Label = "Вверх", IsImportant = true };
            buttonGoUp.Clicked += OnQueryListGoUpClick;
            toolbar.Add(buttonGoUp);

            ToolButton buttonGoDown = new ToolButton(new Image(Stock.GoDown, IconSize.Menu), "Вниз") { Label = "Вниз", IsImportant = true };
            buttonGoDown.Clicked += OnQueryListGoDownClick;
            toolbar.Add(buttonGoDown);

            Box hBoxScroll = new Box(Orientation.Horizontal, 0);
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 300);

            listBoxQuery.ButtonPressEvent += OnQueryListButtonPress;

            scrollList.Add(listBoxQuery);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            hPaned.Pack1(vBox, false, false);
        }

        void CreatePack2(Paned hPaned)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            Expander expanderHelp = new Expander("Довідка") { vBox };

            Box hBox = new Box(Orientation.Horizontal, 0) { Halign = Align.Fill };
            vBox.PackStart(hBox, false, false, 5);

            hBox.PackStart(new Label("help"), false, false, 5);
            hPaned.Pack2(expanderHelp, false, false);
        }

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            FillQueryList();

            entryName.Text = QueryBlock.Name;
            finalCalculation.Active = QueryBlock.FinalCalculation;
        }

        void FillQueryList()
        {
            foreach (KeyValuePair<string, string> query in QueryBlock.Query)
                listBoxQuery.Add(new Label(query.Key) { Name = query.Key.ToString(), Halign = Align.Start, UseUnderline = false });

            listBoxQuery.ShowAll();
        }

        void GetValue()
        {
            QueryBlock.Name = entryName.Text;
            QueryBlock.FinalCalculation = finalCalculation.Active;
        }

        #endregion

        void OnSaveClick(object? sender, EventArgs args)
        {
            if (string.IsNullOrEmpty(entryName.Text))
            {
                Message.Error(GeneralForm, $"Назва не задана");
                return;
            }

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

            GeneralForm?.RenameCurrentPageNotebook($"Блок запитів: {QueryBlock.Name}");
            CallBack_RefreshList?.Invoke();
        }

        #region QueryList

        void OnQueryListButtonPress(object? sender, ButtonPressEventArgs args)
        {
            if (args.Event.Type == Gdk.EventType.DoubleButtonPress)
            {
                ListBoxRow[] selectedRows = listBoxQuery.SelectedRows;
                if (selectedRows.Length != 0)
                {
                    ListBoxRow curRow = selectedRows[0];
                    if (QueryBlock.Query.ContainsKey(curRow.Child.Name))
                        GeneralForm?.CreateNotebookPage($"Запит: {curRow.Child.Name}", () =>
                        {
                            PageQuery page = new PageQuery()
                            {
                                QueryBlock = QueryBlock,
                                Key = curRow.Child.Name,
                                IsNew = false,
                                GeneralForm = GeneralForm,
                                CallBack_RefreshList = QueryListRefreshList
                            };

                            page.SetValue();
                            return page;
                        });
                }
            }
        }

        void OnQueryListAddClick(object? sender, EventArgs args)
        {
            GeneralForm?.CreateNotebookPage($"Запит: *", () =>
            {
                PageQuery page = new PageQuery()
                {
                    QueryBlock = QueryBlock,
                    IsNew = true,
                    GeneralForm = GeneralForm,
                    CallBack_RefreshList = QueryListRefreshList
                };

                page.SetValue();
                return page;
            });
        }

        void OnQueryListCopyClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxQuery.SelectedRows;
            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (QueryBlock.Query.TryGetValue(row.Child.Name, out string? value))
                    {
                        string query = value;
                        string newKey = row.Child.Name + GenerateName.GetNewName();

                        QueryBlock.Query.Add(newKey, query);
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
        }

        void OnQueryListRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxQuery.SelectedRows;
            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                    QueryBlock.Query.Remove(row.Child.Name);

                QueryListRefreshList();
            }
        }

        void OnQueryListGoUpClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxQuery.SelectedRows;
            if (selectedRows.Length != 0)
            {
                ListBoxRow curRow = selectedRows[0];
                int index = curRow.Index;

                if (curRow.Index > 0)
                {
                    List<KeyValuePair<string, string>> list = QueryBlock.Query.ToList<KeyValuePair<string, string>>();
                    KeyValuePair<string, string> itemIndex = list[index];
                    list.RemoveAt(index);
                    list.Insert(index - 1, itemIndex);

                    QueryBlock.Query.Clear();

                    foreach (KeyValuePair<string, string> item in list)
                        QueryBlock.Query.Add(item.Key, item.Value);

                    QueryListRefreshList();
                }
            }
        }

        void OnQueryListGoDownClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxQuery.SelectedRows;
            if (selectedRows.Length != 0)
            {
                ListBoxRow curRow = selectedRows[0];
                int index = curRow.Index;

                if (curRow.Index < listBoxQuery.Children.Length - 1)
                {
                    List<KeyValuePair<string, string>> list = QueryBlock.Query.ToList<KeyValuePair<string, string>>();
                    KeyValuePair<string, string> itemIndex = list[index];
                    list.RemoveAt(index);
                    list.Insert(index + 1, itemIndex);

                    QueryBlock.Query.Clear();

                    foreach (KeyValuePair<string, string> item in list)
                        QueryBlock.Query.Add(item.Key, item.Value);

                    QueryListRefreshList();
                }
            }
        }

        void QueryListRefreshList()
        {
            OnQueryListRefreshClick(null, new EventArgs());
        }

        #endregion
    }
}