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
    class PageConstant : Box
    {
        Configuration Conf { get { return Program.Kernel.Conf; } }

        public ConfigurationConstants ConfConstants { get; set; } = new ConfigurationConstants();
        public FormConfigurator? GeneralForm { get; set; }
        public bool IsNew { get; set; } = true;

        #region Fields

        ListBox listBoxTableParts = new ListBox() { SelectionMode = SelectionMode.Single };
        Entry entryName = new Entry() { WidthRequest = 500 };
        Entry entryColumn = new Entry() { WidthRequest = 500 };
        TextView textViewDesc = new TextView() { WrapMode = WrapMode.Word };
        ComboBoxText comboBoxBlock = new ComboBoxText();
        ComboBoxText comboBoxType = new ComboBoxText();
        ComboBoxText comboBoxPointer = new ComboBoxText();
        ComboBoxText comboBoxEnum = new ComboBoxText();

        #endregion

        public PageConstant() : base(Orientation.Vertical, 0)
        {
            Box hBox = new Box(Orientation.Horizontal, 0);

            Button bSave = new Button("Зберегти");
            bSave.Clicked += OnSaveClick;

            hBox.PackStart(bSave, false, false, 10);

            Button bClose = new Button("Закрити");
            bClose.Clicked += (object? sender, EventArgs args) => GeneralForm?.CloseCurrentPageNotebook();

            hBox.PackStart(bClose, false, false, 10);

            PackStart(hBox, false, false, 10);

            Paned hPaned = new Paned(Orientation.Horizontal) { BorderWidth = 5 };

            CreatePack1(hPaned);
            CreatePack2(hPaned);

            PackStart(hPaned, false, false, 5);

            ShowAll();
        }

        void CreatePack2(Paned hPaned)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            Box hBox = new Box(Orientation.Horizontal, 0);
            hBox.PackStart(new Label("Табличні частини:"), false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(new Image(Stock.New, IconSize.Menu), "Додати") { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnTabularPartsAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(new Image(Stock.Copy, IconSize.Menu), "Копіювати") { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnTabularPartsCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(new Image(Stock.Refresh, IconSize.Menu), "Обновити") { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnTabularPartsRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(new Image(Stock.Clear, IconSize.Menu), "Видалити") { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnTabularPartsRemoveClick;
            toolbar.Add(buttonDelete);

            Box hBoxScroll = new Box(Orientation.Horizontal, 0);
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 250);

            listBoxTableParts.ButtonPressEvent += OnTabularPartsButtonPress;

            scrollList.Add(listBoxTableParts);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);
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

            //Поле
            Box hBoxColumn = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxColumn, false, false, 5);

            hBoxColumn.PackStart(new Label("Поле в таблиці:"), false, false, 5);
            hBoxColumn.PackStart(entryColumn, false, false, 5);

            //Блок
            Box hBoxBlock = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxBlock, false, false, 5);

            foreach (ConfigurationConstantsBlock block in Conf.ConstantsBlock.Values)
                comboBoxBlock.Append(block.BlockName, block.BlockName);

            hBoxBlock.PackStart(new Label("Блок:"), false, false, 5);
            hBoxBlock.PackStart(comboBoxBlock, false, false, 5);

            //Тип
            Box hBoxType = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxType, false, false, 5);

            foreach (var fieldType in FieldType.DefaultList())
                comboBoxType.Append(fieldType.ConfTypeName, fieldType.ViewTypeName);

            comboBoxType.Changed += OnComboBoxTypeChanged;

            hBoxType.PackStart(new Label("Тип:"), false, false, 5);
            hBoxType.PackStart(comboBoxType, false, false, 5);

            //Pointer
            Box hBoxPointer = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxPointer, false, false, 5);

            foreach (ConfigurationDirectories item in Conf.Directories.Values)
                comboBoxPointer.Append($"Довідники.{item.Name}", $"Довідники.{item.Name}");

            foreach (ConfigurationDocuments item in Conf.Documents.Values)
                comboBoxPointer.Append($"Документи.{item.Name}", $"Документи.{item.Name}");

            hBoxPointer.PackStart(new Label("Вказівник:"), false, false, 5);
            hBoxPointer.PackStart(comboBoxPointer, false, false, 5);

            //Enum
            Box hBoxEnum = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxEnum, false, false, 5);

            foreach (ConfigurationEnums item in Conf.Enums.Values)
                comboBoxEnum.Append($"Перелічення.{item.Name}", $"Перелічення.{item.Name}");

            hBoxEnum.PackStart(new Label("Перелічення:"), false, false, 5);
            hBoxEnum.PackStart(comboBoxEnum, false, false, 5);

            //Опис
            Box hBoxDesc = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxDesc, false, false, 5);

            hBoxDesc.PackStart(new Label("Опис:") { Valign = Align.Start }, false, false, 5);

            ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 500, HeightRequest = 100 };
            scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollTextView.Add(textViewDesc);

            hBoxDesc.PackStart(scrollTextView, false, false, 5);

            hPaned.Pack1(vBox, false, false);
        }

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            FillTabularParts();

            entryName.Text = ConfConstants.Name;

            if (IsNew)
                entryColumn.Text = Configuration.GetNewUnigueColumnName(Program.Kernel, SpecialTables.Constants, GeneralForm!.GetConstantsAllFields());
            else
                entryColumn.Text = ConfConstants.NameInTable;

            textViewDesc.Buffer.Text = ConfConstants.Desc;

            comboBoxBlock.ActiveId = ConfConstants.Block.BlockName;
            comboBoxType.ActiveId = ConfConstants.Type;

            if (comboBoxBlock.Active == -1)
                comboBoxBlock.Active = 0;

            if (comboBoxType.Active == -1)
                comboBoxType.Active = 0;

            if (comboBoxType.ActiveId == "pointer")
                comboBoxPointer.ActiveId = ConfConstants.Pointer;

            if (comboBoxType.ActiveId == "enum")
                comboBoxEnum.ActiveId = ConfConstants.Pointer;

            OnComboBoxTypeChanged(comboBoxType, new EventArgs());
        }

        void FillTabularParts()
        {
            foreach (ConfigurationTablePart tablePart in ConfConstants.TabularParts.Values)
                listBoxTableParts.Add(new Label(tablePart.Name) { Name = tablePart.Name, Halign = Align.Start, UseUnderline = false });

            listBoxTableParts.ShowAll();
        }

        void GetValue()
        {
            ConfConstants.Name = entryName.Text;
            ConfConstants.NameInTable = entryColumn.Text;
            ConfConstants.Desc = textViewDesc.Buffer.Text;
            ConfConstants.Block = Conf.ConstantsBlock[comboBoxBlock.ActiveId];
            ConfConstants.Type = comboBoxType.ActiveId;

            if (ConfConstants.Type == "pointer")
                ConfConstants.Pointer = comboBoxPointer.ActiveId;

            if (ConfConstants.Type == "enum")
                ConfConstants.Pointer = comboBoxEnum.ActiveId;
        }

        #endregion

        void OnComboBoxTypeChanged(object? sender, EventArgs args)
        {
            comboBoxPointer.Sensitive = comboBoxType.ActiveId == "pointer";
            comboBoxEnum.Sensitive = comboBoxType.ActiveId == "enum";
        }

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

            if (comboBoxBlock.Active == -1)
            {
                Message.Error(GeneralForm, $"Не вибраний блок");
                return;
            }

            if (!Conf.ConstantsBlock.ContainsKey(comboBoxBlock.ActiveId))
            {
                Message.Error(GeneralForm, $"Відсутній блок {comboBoxBlock.ActiveId}");
                return;
            }

            if (IsNew)
            {
                if (Conf.ConstantsBlock[comboBoxBlock.ActiveId].Constants.ContainsKey(entryName.Text))
                {
                    Message.Error(GeneralForm, $"Назва константи не унікальна в межах блоку {comboBoxBlock.ActiveId}");
                    return;
                }
            }
            else
            {
                if (ConfConstants.Name != entryName.Text || ConfConstants.Block.BlockName != comboBoxBlock.ActiveId)
                {
                    if (Conf.ConstantsBlock[comboBoxBlock.ActiveId].Constants.ContainsKey(entryName.Text))
                    {
                        Message.Error(GeneralForm, $"Назва константи не унікальна в межах блоку {comboBoxBlock.ActiveId}");
                        return;
                    }
                }

                Conf.ConstantsBlock[ConfConstants.Block.BlockName].Constants.Remove(ConfConstants.Name);
            }

            GetValue();

            if (comboBoxType.Active == -1)
            {
                Message.Error(GeneralForm, $"Не вибраний тип даних");
                return;
            }

            if (ConfConstants.Type == "pointer" || ConfConstants.Type == "enum")
                if (String.IsNullOrEmpty(ConfConstants.Pointer))
                {
                    Message.Error(GeneralForm, $"Потрібно деталізувати тип для [ pointer ] або [ enum ]\nВиберіть із списку тип для деталізації");
                    return;
                }

            Conf.AppendConstants(ConfConstants.Block.BlockName, ConfConstants);

            IsNew = false;

            GeneralForm?.LoadTreeAsync();
            GeneralForm?.RenameCurrentPageNotebook($"Константа: {ConfConstants.Name}");
        }

        void OnTabularPartsButtonPress(object? sender, ButtonPressEventArgs args)
        {
            if (args.Event.Type == Gdk.EventType.DoubleButtonPress)
            {
                ListBoxRow[] selectedRows = listBoxTableParts.SelectedRows;
                if (selectedRows.Length != 0)
                {
                    ListBoxRow curRow = selectedRows[0];

                    if (ConfConstants.TabularParts.TryGetValue(curRow.Child.Name, out ConfigurationTablePart? tablePart))
                        GeneralForm?.CreateNotebookPage($"Таблична частина: {curRow.Child.Name}", () =>
                        {
                            PageTablePart page = new PageTablePart()
                            {
                                TabularParts = ConfConstants.TabularParts,
                                TablePart = tablePart,
                                IsNew = false,
                                Owner = new OwnerTablePart(false, "Constants", ConfConstants.Name, ConfConstants.Block.BlockName),
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
            GeneralForm?.CreateNotebookPage("Таблична частина *", () =>
            {
                PageTablePart page = new PageTablePart()
                {
                    TabularParts = ConfConstants.TabularParts,
                    IsNew = true,
                    Owner = new OwnerTablePart(false, "Constants", ConfConstants.Name, ConfConstants.Block.BlockName),
                    GeneralForm = GeneralForm,
                    CallBack_RefreshList = TabularPartsRefreshList
                };

                page.SetValue();
                return page;
            });
        }

        void OnTabularPartsCopyClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxTableParts.SelectedRows;
            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                    if (ConfConstants.TabularParts.TryGetValue(row.Child.Name, out ConfigurationTablePart? tablePart))
                    {
                        ConfigurationTablePart newTablePart = tablePart.Copy();
                        newTablePart.Name += GenerateName.GetNewName();

                        ConfConstants.AppendTablePart(newTablePart);
                    }

                OnTabularPartsRefreshClick(null, new EventArgs());
                GeneralForm?.LoadTreeAsync();
            }
        }

        void OnTabularPartsRefreshClick(object? sender, EventArgs args)
        {
            foreach (Widget item in listBoxTableParts.Children)
                listBoxTableParts.Remove(item);

            FillTabularParts();
        }

        void OnTabularPartsRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxTableParts.SelectedRows;
            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                    ConfConstants.TabularParts.Remove(row.Child.Name);

                OnTabularPartsRefreshClick(null, new EventArgs());
                GeneralForm?.LoadTreeAsync();
            }
        }

        void TabularPartsRefreshList()
        {
            OnTabularPartsRefreshClick(null, new EventArgs());
        }
    }
}