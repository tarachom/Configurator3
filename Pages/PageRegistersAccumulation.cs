/*
Copyright (C) 2019-2022 TARAKHOMYN YURIY IVANOVYCH
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
    class PageRegistersAccumulation : VBox
    {
        Configuration? Conf
        {
            get
            {
                return Program.Kernel?.Conf;
            }
        }

        public ConfigurationRegistersAccumulation ConfRegister { get; set; } = new ConfigurationRegistersAccumulation();
        public FormConfigurator? GeneralForm { get; set; }
        public bool IsNew { get; set; } = true;

        #region Fields

        ListBox listBoxAllowDocumentSpend = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxDimensionFields = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxResourcesFields = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxPropertyFields = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxTableParts = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxQueryBlock = new ListBox() { SelectionMode = SelectionMode.Single };
        Entry entryName = new Entry() { WidthRequest = 500 };
        Entry entryTable = new Entry() { WidthRequest = 500 };
        TextView textViewDesc = new TextView();
        ComboBoxText comboBoxTypeReg = new ComboBoxText();

        #endregion

        public PageRegistersAccumulation() : base()
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

        void CreatePack1(HPaned hPaned)
        {
            VBox vBox = new VBox();

            //Назва
            HBox hBoxName = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxName, false, false, 5);

            hBoxName.PackStart(new Label("Назва:"), false, false, 5);
            hBoxName.PackStart(entryName, false, false, 5);

            //Таблиця
            HBox hBoxTable = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxTable, false, false, 5);

            hBoxTable.PackStart(new Label("Таблиця:"), false, false, 5);
            hBoxTable.PackStart(entryTable, false, false, 5);

            //Тип
            HBox hBoxTypeReg = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxTypeReg, false, false, 5);

            comboBoxTypeReg.Append(TypeRegistersAccumulation.Residues.ToString(), "Залишки");
            comboBoxTypeReg.Append(TypeRegistersAccumulation.Turnover.ToString(), "Обороти");

            hBoxTypeReg.PackStart(new Label("Вид регістру:"), false, false, 5);
            hBoxTypeReg.PackStart(comboBoxTypeReg, false, false, 5);

            //Опис
            HBox hBoxDesc = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxDesc, false, false, 5);

            hBoxDesc.PackStart(new Label("Опис:") { Valign = Align.Start }, false, false, 5);

            ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 500, HeightRequest = 100 };
            scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollTextView.Add(textViewDesc);

            hBoxDesc.PackStart(scrollTextView, false, false, 5);

            //Заголовок списку документів
            Expander expanderAllowDocumentSpend = new Expander("Документи які використовують цей регістер");
            vBox.PackStart(expanderAllowDocumentSpend, false, false, 5);

            //Список документів
            HBox hBoxAllowDocumentSpend = new HBox() { Halign = Align.End };
            expanderAllowDocumentSpend.Add(hBoxAllowDocumentSpend);

            ScrolledWindow scrollAllowDocumentSpend = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollAllowDocumentSpend.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollAllowDocumentSpend.SetSizeRequest(500, 200);

            scrollAllowDocumentSpend.Add(listBoxAllowDocumentSpend);
            hBoxAllowDocumentSpend.PackStart(scrollAllowDocumentSpend, true, true, 5);

            //Separator
            vBox.PackStart(new Separator(Orientation.Horizontal), false, false, 5);

            //VirtualTable
            HBox hBoxButtonCreateVirtualTable = new HBox();
            vBox.PackStart(hBoxButtonCreateVirtualTable, false, false, 5);

            Button bCreateVirtualTable = new Button("Створити");
            bCreateVirtualTable.Clicked += OnCreateVirtualTableClick;

            hBoxButtonCreateVirtualTable.PackStart(bCreateVirtualTable, false, false, 5);

            //Табличні частини
            CreateTablePartList(vBox);

            //Запити
            CreateQueryList(vBox);

            hPaned.Pack1(vBox, false, false);
        }

        void CreatePack2(HPaned hPaned)
        {
            VBox vBox = new VBox();

            //Поля
            CreateDimensionFieldList(vBox);

            CreateResourcesFieldList(vBox);

            CreatePropertyFieldList(vBox);

            hPaned.Pack2(vBox, true, false);
        }

        #region Fields

        void CreateDimensionFieldList(VBox vBoxContainer)
        {
            VBox vBox = new VBox();

            HBox hBox = new HBox();
            hBox.PackStart(new Label("Виміри:"), false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(Stock.New) { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnDimensionFieldsAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(Stock.Copy) { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnDimensionFieldsCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(Stock.Refresh) { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnDimensionFieldsRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(Stock.Clear) { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnDimensionFieldsRemoveClick;
            toolbar.Add(buttonDelete);

            HBox hBoxScroll = new HBox();
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 150);

            listBoxDimensionFields.ButtonPressEvent += OnDimensionFieldsButtonPress;

            scrollList.Add(listBoxDimensionFields);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            vBoxContainer.PackStart(vBox, false, false, 0);
        }

        void CreateResourcesFieldList(VBox vBoxContainer)
        {
            VBox vBox = new VBox();

            HBox hBox = new HBox();
            hBox.PackStart(new Label("Ресурси:"), false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(Stock.New) { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnResourcesFieldsAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(Stock.Copy) { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnResourcesFieldsCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(Stock.Refresh) { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnResourcesFieldsRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(Stock.Clear) { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnResourcesFieldsRemoveClick;
            toolbar.Add(buttonDelete);

            HBox hBoxScroll = new HBox();
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 150);

            listBoxResourcesFields.ButtonPressEvent += OnResourcesFieldsButtonPress;

            scrollList.Add(listBoxResourcesFields);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            vBoxContainer.PackStart(vBox, false, false, 0);
        }

        void CreatePropertyFieldList(VBox vBoxContainer)
        {
            VBox vBox = new VBox();

            HBox hBox = new HBox();
            hBox.PackStart(new Label("Поля:"), false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(Stock.New) { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnPropertyFieldsAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(Stock.Copy) { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnPropertyFieldsCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(Stock.Refresh) { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnPropertyFieldsRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(Stock.Clear) { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnPropertyFieldsRemoveClick;
            toolbar.Add(buttonDelete);

            HBox hBoxScroll = new HBox();
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 150);

            listBoxPropertyFields.ButtonPressEvent += OnPropertyFieldsButtonPress;

            scrollList.Add(listBoxPropertyFields);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            vBoxContainer.PackStart(vBox, false, false, 0);
        }

        void CreateTablePartList(VBox vBoxContainer)
        {
            VBox vBox = new VBox();

            HBox hBox = new HBox();
            hBox.PackStart(new Label("Віртуальні таблиці:"), false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(Stock.New) { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnTabularPartsAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(Stock.Copy) { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnTabularPartsCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(Stock.Refresh) { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnTabularPartsRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(Stock.Clear) { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnTabularPartsRemoveClick;
            toolbar.Add(buttonDelete);

            HBox hBoxScroll = new HBox();
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 150);

            listBoxTableParts.ButtonPressEvent += OnTabularPartsButtonPress;

            scrollList.Add(listBoxTableParts);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            vBoxContainer.PackStart(vBox, false, false, 0);
        }

        void CreateQueryList(VBox vBoxContainer)
        {
            VBox vBox = new VBox();

            HBox hBox = new HBox();
            hBox.PackStart(new Label("Блоки запитів:"), false, false, 5);
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
            scrollList.SetSizeRequest(0, 150);

            listBoxQueryBlock.ButtonPressEvent += OnQueryListButtonPress;

            scrollList.Add(listBoxQueryBlock);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            vBoxContainer.PackStart(vBox, false, false, 0);
        }

        #endregion

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            FillAllowDocumentSpend();
            FillDimensionFields();
            FillResourcesFields();
            FillPropertyFields();
            FillTabularParts();
            FillQueryBlockList();

            entryName.Text = ConfRegister.Name;

            if (IsNew)
                entryTable.Text = Configuration.GetNewUnigueTableName(Program.Kernel!);
            else
                entryTable.Text = ConfRegister.Table;

            comboBoxTypeReg.ActiveId = ConfRegister.TypeRegistersAccumulation.ToString();

            if (comboBoxTypeReg.Active == -1)
                comboBoxTypeReg.Active = 0;

            textViewDesc.Buffer.Text = ConfRegister.Desc;
        }

        void FillAllowDocumentSpend()
        {
            foreach (string field in ConfRegister.AllowDocumentSpend)
                listBoxAllowDocumentSpend.Add(new Label(field) { Name = field, Halign = Align.Start });
        }

        void FillDimensionFields()
        {
            foreach (ConfigurationObjectField field in ConfRegister.DimensionFields.Values)
                listBoxDimensionFields.Add(new Label(field.Name) { Name = field.Name, Halign = Align.Start });
        }

        void FillResourcesFields()
        {
            foreach (ConfigurationObjectField field in ConfRegister.ResourcesFields.Values)
                listBoxResourcesFields.Add(new Label(field.Name) { Name = field.Name, Halign = Align.Start });
        }

        void FillPropertyFields()
        {
            foreach (ConfigurationObjectField field in ConfRegister.PropertyFields.Values)
                listBoxPropertyFields.Add(new Label(field.Name) { Name = field.Name, Halign = Align.Start });
        }

        void FillTabularParts()
        {
            foreach (ConfigurationObjectTablePart tablePart in ConfRegister.TabularParts.Values)
                listBoxTableParts.Add(new Label(tablePart.Name) { Name = tablePart.Name, Halign = Align.Start });
        }

        void FillQueryBlockList()
        {
            foreach (ConfigurationObjectQueryBlock queryBlock in ConfRegister.QueryBlockList.Values)
                listBoxQueryBlock.Add(new Label(queryBlock.Name) { Name = queryBlock.Name, Halign = Align.Start });
        }

        void GetValue()
        {
            ConfRegister.Name = entryName.Text;
            ConfRegister.Table = entryTable.Text;
            ConfRegister.TypeRegistersAccumulation = Enum.Parse<TypeRegistersAccumulation>(comboBoxTypeReg.ActiveId);
            ConfRegister.Desc = textViewDesc.Buffer.Text;
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
                if (Conf!.RegistersAccumulation.ContainsKey(entryName.Text))
                {
                    Message.Error(GeneralForm, $"Назва регістру не унікальна");
                    return;
                }
            }
            else
            {
                if (ConfRegister.Name != entryName.Text)
                {
                    if (Conf!.RegistersAccumulation.ContainsKey(entryName.Text))
                    {
                        Message.Error(GeneralForm, $"Назва регістру не унікальна");
                        return;
                    }
                }

                Conf!.RegistersAccumulation.Remove(ConfRegister.Name);
            }

            GetValue();

            Conf!.AppendRegistersAccumulation(ConfRegister);

            IsNew = false;

            GeneralForm?.LoadTreeAsync();
            GeneralForm?.RenameCurrentPageNotebook($"Регістер накопичення: {ConfRegister.Name}");
        }

        #region Query

        void CreateQueryBlock_Залишки(ConfigurationObjectTablePart TablePart)
        {
            ConfigurationObjectQueryBlock queryBlock = new ConfigurationObjectQueryBlock("Залишки");
            ConfRegister.AppendQueryBlockList(queryBlock);

            string regName = $"{ConfRegister.Name}";
            string tablePartName = $"{ConfRegister.Name}_{TablePart.Name}_TablePart";

            queryBlock.Query.Add("DELETE", @$"

DELETE FROM {{{tablePartName}.TABLE}}
WHERE date_trunc('day', Рег_{regName}.period::timestamp) = @ПеріодДеньВідбір

");

            string query = "\n";

            query += @$"
INSERT INTO {{{tablePartName}.TABLE}} 
(
    uid,";

            int counter = 0;

            foreach (ConfigurationObjectField field in TablePart.Fields.Values)
                query += @$"
    {{{tablePartName}.{field.Name}}}" + (++counter < TablePart.Fields.Count ? "," : "");

            query += @$"
)
SELECT
    uuid_generate_v4(),
    date_trunc('day', Рег_{regName}.period::timestamp) AS Період,";

            counter = 0;

            //Виміри
            foreach (ConfigurationObjectField field in ConfRegister.DimensionFields.Values)
                query += @$"
    Рег_{regName}.{{{regName}_Const.{field.Name}}} AS {field.Name},";

            //Ресурси
            foreach (ConfigurationObjectField field in ConfRegister.ResourcesFields.Values)
                query += @$"
    SUM(CASE WHEN Рег_{regName}.income = true THEN 
        Рег_{regName}.{{{regName}_Const.{field.Name}}} ELSE 
        -Рег_{regName}.{{{regName}_Const.{field.Name}}} END) AS {field.Name}" +
        (++counter < ConfRegister.ResourcesFields.Count ? "," : "");

            query += @$"

FROM
    {{{regName}_Const.TABLE}} AS Рег_{regName}

WHERE
    date_trunc('day', Рег_{regName}.period::timestamp) = @ПеріодДеньВідбір

GROUP BY
    Період";

            //Виміри
            foreach (ConfigurationObjectField field in ConfRegister.DimensionFields.Values)
                query += $", {field.Name}";

            query += @$"

HAVING
";

            counter = 0;

            //Ресурси
            foreach (ConfigurationObjectField field in ConfRegister.ResourcesFields.Values)
                query += @$"
    SUM(CASE WHEN Рег_{regName}.income = true THEN 
        Рег_{regName}.{{{regName}_Const.{field.Name}}} ELSE 
        -Рег_{regName}.{{{regName}_Const.{field.Name}}} END) != 0 " +
        (++counter < ConfRegister.ResourcesFields.Count ? "\n OR \n" : "");

            query += "\n\n\n\n\n";

            queryBlock.Query.Add("SELECT", query);
        }

        #endregion

        #region VirtualTable

        void CreateVirtualTable_Залишки()
        {
            ConfigurationObjectTablePart TablePart = CreateVirtualTable_Table("Залишки");

            CreateVirtualTable_Field(TablePart, new ConfigurationObjectField("Період", "", "date", "", "", false, true));

            //Виміри
            foreach (ConfigurationObjectField field in ConfRegister.DimensionFields.Values)
                CreateVirtualTable_Field(TablePart, field);

            //Ресурси
            foreach (ConfigurationObjectField field in ConfRegister.ResourcesFields.Values)
                CreateVirtualTable_Field(TablePart, field);

            CreateQueryBlock_Залишки(TablePart);
        }

        void CreateVirtualTable_Обороти()
        {
            ConfigurationObjectTablePart TablePart = CreateVirtualTable_Table("Обороти");

            //Виміри
            foreach (ConfigurationObjectField field in ConfRegister.DimensionFields.Values)
                CreateVirtualTable_Field(TablePart, field);

            //Ресурси
            foreach (ConfigurationObjectField field in ConfRegister.ResourcesFields.Values)
            {
                CreateVirtualTable_Field(TablePart, field, "Прихід");
                CreateVirtualTable_Field(TablePart, field, "Розхід");
            }

            ConfigurationObjectQueryBlock queryBlock = new ConfigurationObjectQueryBlock("Обороти");
            ConfRegister.AppendQueryBlockList(queryBlock);

            queryBlock.Query.Add("DELETE", @$"DELETE FROM {TablePart.Table}");
            queryBlock.Query.Add("SELECT", @$"SELECT * FROM {ConfRegister.Table}");
        }

        void CreateVirtualTable_ЗалишкиТаОбороти()
        {
            ConfigurationObjectTablePart TablePart = CreateVirtualTable_Table("ЗалишкиТаОбороти");

            CreateVirtualTable_Field(TablePart, new ConfigurationObjectField("Період", "", "date", "", "", false, true));

            //Виміри
            foreach (ConfigurationObjectField field in ConfRegister.DimensionFields.Values)
                CreateVirtualTable_Field(TablePart, field);

            //Ресурси
            foreach (ConfigurationObjectField field in ConfRegister.ResourcesFields.Values)
            {
                CreateVirtualTable_Field(TablePart, field, "ПочатковийЗалишок");
                CreateVirtualTable_Field(TablePart, field, "Прихід");
                CreateVirtualTable_Field(TablePart, field, "Розхід");
                CreateVirtualTable_Field(TablePart, field, "КінцевийЗалишок");
            }

            ConfigurationObjectQueryBlock queryBlock = new ConfigurationObjectQueryBlock("ЗалишкиТаОбороти");
            ConfRegister.AppendQueryBlockList(queryBlock);

            queryBlock.Query.Add("DELETE", @$"DELETE FROM {TablePart.Table}");
            queryBlock.Query.Add("SELECT", @$"SELECT * FROM {ConfRegister.Table}");
        }

        ConfigurationObjectTablePart CreateVirtualTable_Table(string tableName)
        {
            if (!ConfRegister.TabularParts.ContainsKey(tableName))
            {
                string table = Configuration.GetNewUnigueTableName(Program.Kernel!);
                ConfRegister.AppendTablePart(new ConfigurationObjectTablePart(tableName, table, "Віртуальна таблиця"));
            }

            return ConfRegister.TabularParts[tableName];
        }

        void CreateVirtualTable_Field(ConfigurationObjectTablePart TablePart, ConfigurationObjectField field, string prefixName = "")
        {
            if (!TablePart.Fields.ContainsKey(field.Name + prefixName))
            {
                string fieldColumnName = Configuration.GetNewUnigueColumnName(Program.Kernel!, TablePart.Table, TablePart.Fields);
                TablePart.AppendField(new ConfigurationObjectField(field.Name + prefixName, fieldColumnName, field.Type, field.Pointer, field.Desc, false, field.IsIndex));
            }
        }

        void OnCreateVirtualTableClick(object? sender, EventArgs args)
        {
            ConfRegister.QueryBlockList.Clear();

            switch (ConfRegister.TypeRegistersAccumulation)
            {
                case TypeRegistersAccumulation.Residues: /* Залишки */
                    {
                        CreateVirtualTable_Залишки();
                        CreateVirtualTable_Обороти();
                        CreateVirtualTable_ЗалишкиТаОбороти();

                        break;
                    }
                case TypeRegistersAccumulation.Turnover: /* Обороти */
                    {
                        CreateVirtualTable_Обороти();
                        break;
                    }
            }

            TabularPartsRefreshList();
            QueryListRefreshList();
        }

        #endregion

        #region Dimension Fields

        void OnDimensionFieldsButtonPress(object? sender, ButtonPressEventArgs args)
        {
            if (args.Event.Type == Gdk.EventType.DoubleButtonPress)
            {
                ListBoxRow[] selectedRows = listBoxDimensionFields.SelectedRows;

                if (selectedRows.Length != 0)
                {
                    ListBoxRow curRow = selectedRows[0];

                    if (ConfRegister.DimensionFields.ContainsKey(curRow.Child.Name))
                        GeneralForm?.CreateNotebookPage($"Поле: {curRow.Child.Name}", () =>
                        {
                            PageField page = new PageField()
                            {
                                Fields = ConfRegister.DimensionFields,
                                Field = ConfRegister.DimensionFields[curRow.Child.Name],
                                IsNew = false,
                                GeneralForm = GeneralForm,
                                CallBack_RefreshList = DimensionFieldsRefreshList
                            };

                            page.SetValue();

                            return page;
                        });
                }
            }
        }

        void OnDimensionFieldsAddClick(object? sender, EventArgs args)
        {
            GeneralForm?.CreateNotebookPage("Поле *", () =>
            {
                PageField page = new PageField()
                {
                    Fields = ConfRegister.DimensionFields,
                    IsNew = true,
                    GeneralForm = GeneralForm,
                    CallBack_RefreshList = DimensionFieldsRefreshList
                };

                page.SetValue();

                return page;
            });
        }

        void OnDimensionFieldsCopyClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxDimensionFields.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (ConfRegister.DimensionFields.ContainsKey(row.Child.Name))
                    {
                        ConfigurationObjectField newField = ConfRegister.DimensionFields[row.Child.Name].Copy();
                        newField.Name += GenerateName.GetNewName();

                        ConfRegister.AppendDimensionField(newField);
                    }
                }

                DimensionFieldsRefreshList();

                GeneralForm?.LoadTreeAsync();
            }
        }

        void OnDimensionFieldsRefreshClick(object? sender, EventArgs args)
        {
            foreach (Widget item in listBoxDimensionFields.Children)
                listBoxDimensionFields.Remove(item);

            FillDimensionFields();

            listBoxDimensionFields.ShowAll();
        }

        void OnDimensionFieldsRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxDimensionFields.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (ConfRegister.DimensionFields.ContainsKey(row.Child.Name))
                        ConfRegister.DimensionFields.Remove(row.Child.Name);
                }

                DimensionFieldsRefreshList();

                GeneralForm?.LoadTreeAsync();
            }
        }

        void DimensionFieldsRefreshList()
        {
            OnDimensionFieldsRefreshClick(null, new EventArgs());
        }

        #endregion

        #region Resources Fields

        void OnResourcesFieldsButtonPress(object? sender, ButtonPressEventArgs args)
        {
            if (args.Event.Type == Gdk.EventType.DoubleButtonPress)
            {
                ListBoxRow[] selectedRows = listBoxResourcesFields.SelectedRows;

                if (selectedRows.Length != 0)
                {
                    ListBoxRow curRow = selectedRows[0];

                    if (ConfRegister.ResourcesFields.ContainsKey(curRow.Child.Name))
                        GeneralForm?.CreateNotebookPage($"Поле: {curRow.Child.Name}", () =>
                        {
                            PageField page = new PageField()
                            {
                                Fields = ConfRegister.ResourcesFields,
                                Field = ConfRegister.ResourcesFields[curRow.Child.Name],
                                IsNew = false,
                                GeneralForm = GeneralForm,
                                CallBack_RefreshList = ResourcesFieldsRefreshList
                            };

                            page.SetValue();

                            return page;
                        });
                }
            }
        }

        void OnResourcesFieldsAddClick(object? sender, EventArgs args)
        {
            GeneralForm?.CreateNotebookPage("Поле *", () =>
            {
                PageField page = new PageField()
                {
                    Fields = ConfRegister.ResourcesFields,
                    IsNew = true,
                    GeneralForm = GeneralForm,
                    CallBack_RefreshList = ResourcesFieldsRefreshList
                };

                page.SetValue();

                return page;
            });
        }

        void OnResourcesFieldsCopyClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxResourcesFields.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (ConfRegister.ResourcesFields.ContainsKey(row.Child.Name))
                    {
                        ConfigurationObjectField newField = ConfRegister.ResourcesFields[row.Child.Name].Copy();
                        newField.Name += GenerateName.GetNewName();

                        ConfRegister.AppendResourcesField(newField);
                    }
                }

                ResourcesFieldsRefreshList();

                GeneralForm?.LoadTreeAsync();
            }
        }

        void OnResourcesFieldsRefreshClick(object? sender, EventArgs args)
        {
            foreach (Widget item in listBoxResourcesFields.Children)
                listBoxResourcesFields.Remove(item);

            FillResourcesFields();

            listBoxResourcesFields.ShowAll();
        }

        void OnResourcesFieldsRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxResourcesFields.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (ConfRegister.ResourcesFields.ContainsKey(row.Child.Name))
                        ConfRegister.ResourcesFields.Remove(row.Child.Name);
                }

                ResourcesFieldsRefreshList();

                GeneralForm?.LoadTreeAsync();
            }
        }

        void ResourcesFieldsRefreshList()
        {
            OnResourcesFieldsRefreshClick(null, new EventArgs());
        }

        #endregion

        #region Property Fields

        void OnPropertyFieldsButtonPress(object? sender, ButtonPressEventArgs args)
        {
            if (args.Event.Type == Gdk.EventType.DoubleButtonPress)
            {
                ListBoxRow[] selectedRows = listBoxPropertyFields.SelectedRows;

                if (selectedRows.Length != 0)
                {
                    ListBoxRow curRow = selectedRows[0];

                    if (ConfRegister.PropertyFields.ContainsKey(curRow.Child.Name))
                        GeneralForm?.CreateNotebookPage($"Поле: {curRow.Child.Name}", () =>
                        {
                            PageField page = new PageField()
                            {
                                Fields = ConfRegister.PropertyFields,
                                Field = ConfRegister.PropertyFields[curRow.Child.Name],
                                IsNew = false,
                                GeneralForm = GeneralForm,
                                CallBack_RefreshList = PropertyFieldsRefreshList
                            };

                            page.SetValue();

                            return page;
                        });
                }
            }
        }

        void OnPropertyFieldsAddClick(object? sender, EventArgs args)
        {
            GeneralForm?.CreateNotebookPage("Поле *", () =>
            {
                PageField page = new PageField()
                {
                    Table = ConfRegister.Table,
                    Fields = ConfRegister.PropertyFields,
                    IsNew = true,
                    GeneralForm = GeneralForm,
                    CallBack_RefreshList = PropertyFieldsRefreshList
                };

                page.SetValue();

                return page;
            });
        }

        void OnPropertyFieldsCopyClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxPropertyFields.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (ConfRegister.PropertyFields.ContainsKey(row.Child.Name))
                    {
                        ConfigurationObjectField newField = ConfRegister.PropertyFields[row.Child.Name].Copy();
                        newField.Name += GenerateName.GetNewName();

                        ConfRegister.AppendPropertyField(newField);
                    }
                }

                PropertyFieldsRefreshList();

                GeneralForm?.LoadTreeAsync();
            }
        }

        void OnPropertyFieldsRefreshClick(object? sender, EventArgs args)
        {
            foreach (Widget item in listBoxPropertyFields.Children)
                listBoxPropertyFields.Remove(item);

            FillPropertyFields();

            listBoxPropertyFields.ShowAll();
        }

        void OnPropertyFieldsRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxPropertyFields.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (ConfRegister.PropertyFields.ContainsKey(row.Child.Name))
                        ConfRegister.PropertyFields.Remove(row.Child.Name);
                }

                PropertyFieldsRefreshList();

                GeneralForm?.LoadTreeAsync();
            }
        }

        void PropertyFieldsRefreshList()
        {
            OnPropertyFieldsRefreshClick(null, new EventArgs());
        }

        #endregion

        #region TabularParts

        void OnTabularPartsButtonPress(object? sender, ButtonPressEventArgs args)
        {
            if (args.Event.Type == Gdk.EventType.DoubleButtonPress)
            {
                ListBoxRow[] selectedRows = listBoxTableParts.SelectedRows;

                if (selectedRows.Length != 0)
                {
                    ListBoxRow curRow = selectedRows[0];

                    if (ConfRegister.TabularParts.ContainsKey(curRow.Child.Name))
                        GeneralForm?.CreateNotebookPage($"Таблична частина: {curRow.Child.Name}", () =>
                        {
                            PageTablePart page = new PageTablePart()
                            {
                                TabularParts = ConfRegister.TabularParts,
                                TablePart = ConfRegister.TabularParts[curRow.Child.Name],
                                IsNew = false,
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
                    TabularParts = ConfRegister.TabularParts,
                    IsNew = true,
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
                {
                    if (ConfRegister.TabularParts.ContainsKey(row.Child.Name))
                    {
                        ConfigurationObjectTablePart newTablePart = ConfRegister.TabularParts[row.Child.Name].Copy();
                        newTablePart.Name += GenerateName.GetNewName();
                        newTablePart.Table = Configuration.GetNewUnigueTableName(Program.Kernel!);

                        ConfRegister.AppendTablePart(newTablePart);
                    }
                }

                TabularPartsRefreshList();

                GeneralForm?.LoadTreeAsync();
            }
        }

        void OnTabularPartsRefreshClick(object? sender, EventArgs args)
        {
            foreach (Widget item in listBoxTableParts.Children)
                listBoxTableParts.Remove(item);

            FillTabularParts();

            listBoxTableParts.ShowAll();
        }

        void OnTabularPartsRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxTableParts.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (ConfRegister.TabularParts.ContainsKey(row.Child.Name))
                        ConfRegister.TabularParts.Remove(row.Child.Name);
                }

                TabularPartsRefreshList();

                GeneralForm?.LoadTreeAsync();
            }
        }

        void TabularPartsRefreshList()
        {
            OnTabularPartsRefreshClick(null, new EventArgs());
        }

        #endregion

        #region QueryList

        void OnQueryListButtonPress(object? sender, ButtonPressEventArgs args)
        {
            if (args.Event.Type == Gdk.EventType.DoubleButtonPress)
            {
                ListBoxRow[] selectedRows = listBoxQueryBlock.SelectedRows;

                if (selectedRows.Length != 0)
                {
                    ListBoxRow curRow = selectedRows[0];

                    if (ConfRegister.QueryBlockList.ContainsKey(curRow.Child.Name))
                        GeneralForm?.CreateNotebookPage($"Блок запитів: {curRow.Child.Name}", () =>
                        {
                            PageQueryBlock page = new PageQueryBlock()
                            {
                                QueryBlockList = ConfRegister.QueryBlockList,
                                QueryBlock = ConfRegister.QueryBlockList[curRow.Child.Name],
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
            GeneralForm?.CreateNotebookPage($"Блок запитів: *", () =>
            {
                PageQueryBlock page = new PageQueryBlock()
                {
                    QueryBlockList = ConfRegister.QueryBlockList,
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
            ListBoxRow[] selectedRows = listBoxQueryBlock.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (ConfRegister.QueryBlockList.ContainsKey(row.Child.Name))
                    {
                        ConfigurationObjectQueryBlock newQueryBlock = ConfRegister.QueryBlockList[row.Child.Name].Copy();
                        newQueryBlock.Name += GenerateName.GetNewName();

                        ConfRegister.AppendQueryBlockList(newQueryBlock);
                    }
                }

                QueryListRefreshList();
            }
        }

        void OnQueryListRefreshClick(object? sender, EventArgs args)
        {
            foreach (Widget item in listBoxQueryBlock.Children)
                listBoxQueryBlock.Remove(item);

            FillQueryBlockList();

            listBoxQueryBlock.ShowAll();
        }

        void OnQueryListRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxQueryBlock.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (ConfRegister.QueryBlockList.ContainsKey(row.Child.Name))
                        ConfRegister.QueryBlockList.Remove(row.Child.Name);
                }

                QueryListRefreshList();
            }
        }

        void QueryListRefreshList()
        {
            OnQueryListRefreshClick(null, new EventArgs());
        }

        #endregion

    }
}