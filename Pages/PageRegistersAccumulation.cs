/*
Copyright (C) 2019-2024 TARAKHOMYN YURIY IVANOVYCH
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
    class PageRegisterAccumulation : Box
    {
        Configuration Conf { get { return Program.Kernel.Conf; } }

        public ConfigurationRegistersAccumulation ConfRegister { get; set; } = new ConfigurationRegistersAccumulation();
        public FormConfigurator? GeneralForm { get; set; }
        public bool IsNew { get; set; } = true;

        #region Fields

        ListBox listBoxAllowDocumentSpend = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxDimensionFields = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxResourcesFields = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxPropertyFields = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxTabularList = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxFormsList = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxTableParts = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxQueryBlock = new ListBox() { SelectionMode = SelectionMode.Single };
        Entry entryName = new Entry() { WidthRequest = 500 };
        Entry entryFullName = new Entry() { WidthRequest = 500 };
        Entry entryTable = new Entry() { WidthRequest = 500 };
        TextView textViewDesc = new TextView() { WrapMode = WrapMode.Word };
        ComboBoxText comboBoxTypeReg = new ComboBoxText();
        CheckButton checkButtonNoSummary = new CheckButton("Без підсумків");

        #endregion

        public PageRegisterAccumulation() : base(Orientation.Vertical, 0)
        {
            Box hBox = new Box(Orientation.Horizontal, 0);

            Button bSave = new Button("Зберегти");
            bSave.Clicked += OnSaveClick;

            hBox.PackStart(bSave, false, false, 10);

            Button bClose = new Button("Закрити");
            bClose.Clicked += (object? sender, EventArgs args) => { GeneralForm?.CloseCurrentPageNotebook(); };

            hBox.PackStart(bClose, false, false, 10);

            PackStart(hBox, false, false, 10);

            Paned hPaned = new Paned(Orientation.Horizontal) { BorderWidth = 5 };

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

            //Повна Назва
            Box hBoxFullName = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxFullName, false, false, 5);

            hBoxFullName.PackStart(new Label("Повна назва:"), false, false, 5);
            hBoxFullName.PackStart(entryFullName, false, false, 5);

            //Таблиця
            Box hBoxTable = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxTable, false, false, 5);

            hBoxTable.PackStart(new Label("Таблиця:"), false, false, 5);
            hBoxTable.PackStart(entryTable, false, false, 5);

            //Тип
            Box hBoxTypeReg = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxTypeReg, false, false, 5);

            comboBoxTypeReg.Append(TypeRegistersAccumulation.Residues.ToString(), "Залишки");
            comboBoxTypeReg.Append(TypeRegistersAccumulation.Turnover.ToString(), "Обороти");

            hBoxTypeReg.PackStart(new Label("Вид регістру:"), false, false, 5);
            hBoxTypeReg.PackStart(comboBoxTypeReg, false, false, 5);

            //Опис
            Box hBoxDesc = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxDesc, false, false, 5);

            hBoxDesc.PackStart(new Label("Опис:") { Valign = Align.Start }, false, false, 5);

            ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 500, HeightRequest = 100 };
            scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollTextView.Add(textViewDesc);

            hBoxDesc.PackStart(scrollTextView, false, false, 5);

            //Заголовок списку документів
            {
                Expander expanderAllowDocumentSpend = new Expander("Документи");
                vBox.PackStart(expanderAllowDocumentSpend, false, false, 5);

                Box vBoxDocList = new Box(Orientation.Vertical, 0);
                expanderAllowDocumentSpend.Add(vBoxDocList);

                //Заголовок блоку
                Box hBoxInfo = new Box(Orientation.Horizontal, 0) { Halign = Align.Center };
                vBoxDocList.PackStart(hBoxInfo, false, false, 5);
                hBoxInfo.PackStart(new Label("Документи які використовують цей регістр"), false, false, 5);

                //Список документів
                Box hBoxAllowDocumentSpend = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
                vBoxDocList.PackStart(hBoxAllowDocumentSpend, false, false, 5);

                ScrolledWindow scrollAllowDocumentSpend = new ScrolledWindow() { ShadowType = ShadowType.In };
                scrollAllowDocumentSpend.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
                scrollAllowDocumentSpend.SetSizeRequest(500, 200);

                scrollAllowDocumentSpend.Add(listBoxAllowDocumentSpend);
                hBoxAllowDocumentSpend.PackStart(scrollAllowDocumentSpend, true, true, 5);
            }

            //Списки та форми
            {
                Expander expanderForm = new Expander("Табличні списки");
                vBox.PackStart(expanderForm, false, false, 5);

                Box vBoxForm = new Box(Orientation.Vertical, 0);
                expanderForm.Add(vBoxForm);

                //Заголовок блоку
                Box hBoxInfo = new Box(Orientation.Horizontal, 0) { Halign = Align.Center };
                vBoxForm.PackStart(hBoxInfo, false, false, 5);
                hBoxInfo.PackStart(new Label("Табличні списки"), false, false, 5);

                //Табличні списки
                CreateTabularList(vBoxForm);
            }

            //Форми
            {
                Expander expanderForm = new Expander("Форми");
                vBox.PackStart(expanderForm, false, false, 5);

                Box vBoxForm = new Box(Orientation.Vertical, 0);
                expanderForm.Add(vBoxForm);

                //Заголовок блоку Forms
                Box hBoxInterfaceCreateInfo = new Box(Orientation.Horizontal, 0) { Halign = Align.Center };
                vBoxForm.PackStart(hBoxInterfaceCreateInfo, false, false, 5);
                hBoxInterfaceCreateInfo.PackStart(new Label("Форми"), false, false, 5);

                //Форми
                CreateFormsList(vBoxForm);
            }

            //Таблиці для розрахунків
            {
                Expander expanderVirtualTable = new Expander("Таблиці для розрахунків");
                vBox.PackStart(expanderVirtualTable, false, false, 5);

                Box vBoxVirtualTable = new Box(Orientation.Vertical, 0);
                expanderVirtualTable.Add(vBoxVirtualTable);

                //Заголовок блоку
                Box hBoxInfo = new Box(Orientation.Horizontal, 0) { Halign = Align.Center };
                vBoxVirtualTable.PackStart(hBoxInfo, false, false, 5);
                hBoxInfo.PackStart(new Label("Таблиці та блоки запитів для розрахунків"), false, false, 5);

                //Створити
                Box hBoxButtonCreateVirtualTable = new Box(Orientation.Horizontal, 0);
                vBoxVirtualTable.PackStart(hBoxButtonCreateVirtualTable, false, false, 5);

                Button bCreateVirtualTable = new Button("Створити");
                bCreateVirtualTable.Clicked += OnCreateVirtualTableClick;

                hBoxButtonCreateVirtualTable.PackStart(bCreateVirtualTable, false, false, 5);
                hBoxButtonCreateVirtualTable.PackStart(checkButtonNoSummary, false, false, 5);

                //Табличні частини
                CreateTablePartList(vBoxVirtualTable);

                //Запити
                CreateQueryList(vBoxVirtualTable);
            }

            hPaned.Pack1(vBox, false, false);
        }

        void CreatePack2(Paned hPaned)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            //Стандартні поля
            Expander expanderDefField = new Expander("Стандартні поля");
            vBox.PackStart(expanderDefField, false, false, 5);

            expanderDefField.Add(new Label(" <b>uid</b> \n <b>period</b> - дата та час запису \n <b>income</b> - дохід (true), розхід (false) \n <b>owner</b> - власник запису") { Halign = Align.Start, UseMarkup = true, UseUnderline = false, Selectable = true });

            //Поля
            CreateDimensionFieldList(vBox);
            CreateResourcesFieldList(vBox);
            CreatePropertyFieldList(vBox);

            hPaned.Pack2(vBox, true, false);
        }

        #region Fields

        void CreateDimensionFieldList(Box vBoxContainer)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            Box hBox = new Box(Orientation.Horizontal, 0);
            hBox.PackStart(new Label("Виміри:"), false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(new Image(Stock.New, IconSize.Menu), "Додати") { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnDimensionFieldsAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(new Image(Stock.Copy, IconSize.Menu), "Копіювати") { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnDimensionFieldsCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(new Image(Stock.Refresh, IconSize.Menu), "Обновити") { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnDimensionFieldsRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(new Image(Stock.Clear, IconSize.Menu), "Видалити") { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnDimensionFieldsRemoveClick;
            toolbar.Add(buttonDelete);

            Box hBoxScroll = new Box(Orientation.Horizontal, 0);
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 150);

            listBoxDimensionFields.ButtonPressEvent += OnDimensionFieldsButtonPress;

            scrollList.Add(listBoxDimensionFields);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            vBoxContainer.PackStart(vBox, false, false, 0);
        }

        void CreateResourcesFieldList(Box vBoxContainer)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            Box hBox = new Box(Orientation.Horizontal, 0);
            hBox.PackStart(new Label("Ресурси:"), false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(new Image(Stock.New, IconSize.Menu), "Додати") { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnResourcesFieldsAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(new Image(Stock.Copy, IconSize.Menu), "Копіювати") { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnResourcesFieldsCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(new Image(Stock.Refresh, IconSize.Menu), "Обновити") { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnResourcesFieldsRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(new Image(Stock.Clear, IconSize.Menu), "Видалити") { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnResourcesFieldsRemoveClick;
            toolbar.Add(buttonDelete);

            Box hBoxScroll = new Box(Orientation.Horizontal, 0);
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 150);

            listBoxResourcesFields.ButtonPressEvent += OnResourcesFieldsButtonPress;

            scrollList.Add(listBoxResourcesFields);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            vBoxContainer.PackStart(vBox, false, false, 0);
        }

        void CreatePropertyFieldList(Box vBoxContainer)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            Box hBox = new Box(Orientation.Horizontal, 0);
            hBox.PackStart(new Label("Поля:"), false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(new Image(Stock.New, IconSize.Menu), "Додати") { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnPropertyFieldsAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(new Image(Stock.Copy, IconSize.Menu), "Копіювати") { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnPropertyFieldsCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(new Image(Stock.Refresh, IconSize.Menu), "Обновити") { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnPropertyFieldsRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(new Image(Stock.Clear, IconSize.Menu), "Видалити") { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnPropertyFieldsRemoveClick;
            toolbar.Add(buttonDelete);

            Box hBoxScroll = new Box(Orientation.Horizontal, 0);
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 150);

            listBoxPropertyFields.ButtonPressEvent += OnPropertyFieldsButtonPress;

            scrollList.Add(listBoxPropertyFields);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            vBoxContainer.PackStart(vBox, false, false, 0);
        }

        void CreateTabularList(Box vBoxContainer)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(new Image(Stock.New, IconSize.Menu), "Додати") { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnTabularListAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(new Image(Stock.Copy, IconSize.Menu), "Копіювати") { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnTabularListCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(new Image(Stock.Refresh, IconSize.Menu), "Обновити") { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnTabularListRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(new Image(Stock.Clear, IconSize.Menu), "Видалити") { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnTabularListRemoveClick;
            toolbar.Add(buttonDelete);

            Box hBoxScroll = new Box(Orientation.Horizontal, 0);
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 100);

            listBoxTabularList.ButtonPressEvent += OnTabularListButtonPress;

            scrollList.Add(listBoxTabularList);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            vBoxContainer.PackStart(vBox, false, false, 0);
        }

        void CreateFormsList(Box vBoxContainer)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            Button buttonCreateForms = new Button("Створити");
            buttonCreateForms.Clicked += (object? sender, EventArgs args) =>
            {

            };

            Box hBox = new Box(Orientation.Horizontal, 0);
            hBox.PackStart(buttonCreateForms, false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            MenuToolButton buttonAdd = new MenuToolButton(new Image(Stock.New, IconSize.Menu), "Додати") { IsImportant = true, Menu = OnFormsListAddFormSubMenu() };
            buttonAdd.Clicked += (object? sender, EventArgs arg) => { ((Menu)((MenuToolButton)sender!).Menu).Popup(); };
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(new Image(Stock.Copy, IconSize.Menu), "Копіювати") { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnFormsListCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(new Image(Stock.Refresh, IconSize.Menu), "Обновити") { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnFormsListRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(new Image(Stock.Clear, IconSize.Menu), "Видалити") { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnFormsListRemoveClick;
            toolbar.Add(buttonDelete);

            Box hBoxScroll = new Box(Orientation.Horizontal, 0);
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 200);

            listBoxFormsList.ButtonPressEvent += OnFormsListButtonPress;

            scrollList.Add(listBoxFormsList);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            vBoxContainer.PackStart(vBox, false, false, 0);
        }

        void CreateTablePartList(Box vBoxContainer)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            Box hBox = new Box(Orientation.Horizontal, 0);
            hBox.PackStart(new Label("Таблиці"), false, false, 5);
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
            scrollList.SetSizeRequest(0, 150);

            listBoxTableParts.ButtonPressEvent += OnTabularPartsButtonPress;

            scrollList.Add(listBoxTableParts);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            vBoxContainer.PackStart(vBox, false, false, 0);
        }

        void CreateQueryList(Box vBoxContainer)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            Box hBox = new Box(Orientation.Horizontal, 0);
            hBox.PackStart(new Label("Блоки запитів"), false, false, 5);
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

            Box hBoxScroll = new Box(Orientation.Horizontal, 0);
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

        public async void SetValue()
        {
            FillAllowDocumentSpend();
            FillDimensionFields();
            FillResourcesFields();
            FillPropertyFields();
            FillTabularList();
            FillFormsList();
            FillTabularParts();
            FillQueryBlockList();

            entryName.Text = ConfRegister.Name;
            entryFullName.Text = ConfRegister.FullName;

            if (IsNew)
                entryTable.Text = await Configuration.GetNewUnigueTableName(Program.Kernel);
            else
                entryTable.Text = ConfRegister.Table;

            comboBoxTypeReg.ActiveId = ConfRegister.TypeRegistersAccumulation.ToString();

            if (comboBoxTypeReg.Active == -1)
                comboBoxTypeReg.Active = 0;

            textViewDesc.Buffer.Text = ConfRegister.Desc;
            checkButtonNoSummary.Active = ConfRegister.NoSummary;
        }

        void FillAllowDocumentSpend()
        {
            foreach (string field in ConfRegister.AllowDocumentSpend)
                listBoxAllowDocumentSpend.Add(new Label(field) { Name = field, Halign = Align.Start, UseUnderline = false });

            listBoxAllowDocumentSpend.ShowAll();
        }

        void FillDimensionFields()
        {
            foreach (ConfigurationField field in ConfRegister.DimensionFields.Values)
                listBoxDimensionFields.Add(new Label(field.Name) { Name = field.Name, Halign = Align.Start, UseUnderline = false });

            listBoxDimensionFields.ShowAll();
        }

        void FillResourcesFields()
        {
            foreach (ConfigurationField field in ConfRegister.ResourcesFields.Values)
                listBoxResourcesFields.Add(new Label(field.Name) { Name = field.Name, Halign = Align.Start, UseUnderline = false });

            listBoxResourcesFields.ShowAll();
        }

        void FillPropertyFields()
        {
            foreach (ConfigurationField field in ConfRegister.PropertyFields.Values)
                listBoxPropertyFields.Add(new Label(field.Name) { Name = field.Name, Halign = Align.Start, UseUnderline = false });

            listBoxPropertyFields.ShowAll();
        }

        void FillTabularList()
        {
            foreach (ConfigurationTabularList tableList in ConfRegister.TabularList.Values)
                listBoxTabularList.Add(new Label(tableList.Name) { Name = tableList.Name, Halign = Align.Start, UseUnderline = false });

            listBoxTabularList.ShowAll();
        }

        void FillTabularParts()
        {
            foreach (ConfigurationTablePart tablePart in ConfRegister.TabularParts.Values)
                listBoxTableParts.Add(new Label(tablePart.Name) { Name = tablePart.Name, Halign = Align.Start, UseUnderline = false });

            listBoxTableParts.ShowAll();
        }

        void FillFormsList()
        {
            foreach (ConfigurationForms form in ConfRegister.Forms.Values)
                listBoxFormsList.Add(new Label(form.Name) { Name = form.Name, Halign = Align.Start, UseUnderline = false });

            listBoxFormsList.ShowAll();
        }

        void FillQueryBlockList()
        {
            foreach (ConfigurationQueryBlock queryBlock in ConfRegister.QueryBlockList.Values)
                listBoxQueryBlock.Add(new Label(queryBlock.Name) { Name = queryBlock.Name, Halign = Align.Start, UseUnderline = false });

            listBoxQueryBlock.ShowAll();
        }

        void GetValue()
        {
            if (string.IsNullOrEmpty(entryFullName.Text))
                entryFullName.Text = entryName.Text;

            ConfRegister.Name = entryName.Text;
            ConfRegister.FullName = entryFullName.Text;
            ConfRegister.Table = entryTable.Text;
            ConfRegister.TypeRegistersAccumulation = Enum.Parse<TypeRegistersAccumulation>(comboBoxTypeReg.ActiveId);
            ConfRegister.Desc = textViewDesc.Buffer.Text;
            ConfRegister.NoSummary = checkButtonNoSummary.Active;
        }

        RegisterAccumulationOtherInfoStruct GetRegisterAccumulationOtherInfo()
        {
            return new RegisterAccumulationOtherInfoStruct
            (
                Enum.Parse<TypeRegistersAccumulation>(comboBoxTypeReg.ActiveId)
            );
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
                if (Conf.RegistersAccumulation.ContainsKey(entryName.Text))
                {
                    Message.Error(GeneralForm, $"Назва регістру не унікальна");
                    return;
                }
            }
            else
            {
                if (ConfRegister.Name != entryName.Text)
                {
                    if (Conf.RegistersAccumulation.ContainsKey(entryName.Text))
                    {
                        Message.Error(GeneralForm, $"Назва регістру не унікальна");
                        return;
                    }
                }

                Conf.RegistersAccumulation.Remove(ConfRegister.Name);
            }

            GetValue();

            Conf.AppendRegistersAccumulation(ConfRegister);

            IsNew = false;

            GeneralForm?.LoadTreeAsync();
            GeneralForm?.RenameCurrentPageNotebook($"Регістр накопичення: {ConfRegister.Name}");
        }

        #region QueryBlock

        //
        // Залишки
        //

        void CreateQueryBlock_Залишки(ConfigurationTablePart TablePart)
        {
            string queryBlockKey = "Залишки";

            ConfigurationQueryBlock queryBlock;
            if (ConfRegister.QueryBlockList.TryGetValue(queryBlockKey, out ConfigurationQueryBlock? block))
            {
                queryBlock = block;
                queryBlock.Query.Clear();
            }
            else
                ConfRegister.AppendQueryBlockList(queryBlock = new ConfigurationQueryBlock(queryBlockKey));

            string regName = $"{ConfRegister.Name}";
            string tablePartName = $"{ConfRegister.Name}_{TablePart.Name}_TablePart";

            //
            // DELETE
            //

            queryBlock.Query.Add("DELETE", @$"
DELETE FROM 
    {{{tablePartName}.TABLE}}
WHERE 
    {{{tablePartName}.TABLE}}.{{{tablePartName}.Період}} = @ПеріодДеньВідбір
");

            //
            // INSERT
            //

            string query = @$"
INSERT INTO {{{tablePartName}.TABLE}} 
(
    uid,";

            int counter = 0;

            foreach (ConfigurationField field in TablePart.Fields.Values)
                query += @$"
    {{{tablePartName}.{field.Name}}}" + (++counter < TablePart.Fields.Count ? "," : "");

            query += @$"
)
SELECT
    uuid_generate_v4(),
    date_trunc('day', {regName}.period::timestamp) AS Період,";

            counter = 0;

            //Виміри
            foreach (ConfigurationField field in ConfRegister.DimensionFields.Values)
                query += @$"
    {regName}.{{{regName}_Const.{field.Name}}} AS {field.Name},";

            //Ресурси
            foreach (ConfigurationField field in ConfRegister.ResourcesFields.Values)
                query += @$"

    /* {field.Name} */
    SUM(CASE WHEN {regName}.income = true THEN 
        {regName}.{{{regName}_Const.{field.Name}}} ELSE 
        -{regName}.{{{regName}_Const.{field.Name}}} END) AS {field.Name}" +
        (++counter < ConfRegister.ResourcesFields.Count ? "," : "");

            query += @$"

FROM
    {{{regName}_Const.TABLE}} AS {regName}

WHERE
    date_trunc('day', {regName}.period::timestamp) = @ПеріодДеньВідбір

GROUP BY
    Період";

            //Виміри
            foreach (ConfigurationField field in ConfRegister.DimensionFields.Values)
                query += $", {field.Name}";

            query += @$"

HAVING";

            counter = 0;

            //Ресурси
            foreach (ConfigurationField field in ConfRegister.ResourcesFields.Values)
                query += @$"

    /* {field.Name} */
    SUM(CASE WHEN {regName}.income = true THEN 
        {regName}.{{{regName}_Const.{field.Name}}} ELSE 
        -{regName}.{{{regName}_Const.{field.Name}}} END) != 0 " +
        (++counter < ConfRegister.ResourcesFields.Count ? "\n OR \n" : "");

            query += "\n\n\n\n\n";

            queryBlock.Query.Add("SELECT", query);
        }

        void CreateQueryBlock_ЗалишкиТаОбороти(ConfigurationTablePart TablePart)
        {
            string queryBlockKey = "ЗалишкиТаОбороти";

            ConfigurationQueryBlock queryBlock;
            if (ConfRegister.QueryBlockList.TryGetValue(queryBlockKey, out ConfigurationQueryBlock? block))
            {
                queryBlock = block;
                queryBlock.Query.Clear();
            }
            else
                ConfRegister.AppendQueryBlockList(queryBlock = new ConfigurationQueryBlock(queryBlockKey));

            string regName = $"{ConfRegister.Name}";
            string tablePartName = $"{ConfRegister.Name}_{TablePart.Name}_TablePart";

            //
            // DELETE
            //

            queryBlock.Query.Add("DELETE", @$"
DELETE FROM 
    {{{tablePartName}.TABLE}}
WHERE 
    {{{tablePartName}.TABLE}}.{{{tablePartName}.Період}} = @ПеріодДеньВідбір
");

            //
            // INSERT
            //

            string query = @$"
INSERT INTO {{{tablePartName}.TABLE}} 
(
    uid,";

            int counter = 0;

            foreach (ConfigurationField field in TablePart.Fields.Values)
                query += @$"
    {{{tablePartName}.{field.Name}}}" + (++counter < TablePart.Fields.Count ? "," : "");

            query += @$"
)
SELECT
    uuid_generate_v4(),
    date_trunc('day', {regName}.period::timestamp) AS Період,";

            counter = 0;

            //Виміри
            foreach (ConfigurationField field in ConfRegister.DimensionFields.Values)
                query += @$"
    {regName}.{{{regName}_Const.{field.Name}}} AS {field.Name},";

            //Ресурси
            foreach (ConfigurationField field in ConfRegister.ResourcesFields.Values)
                query += @$"

    /* {field.Name} */
    SUM(CASE WHEN {regName}.income = true THEN 
        {regName}.{{{regName}_Const.{field.Name}}} ELSE 0 END) AS {field.Name}Прихід,
    SUM(CASE WHEN {regName}.income = false THEN 
        {regName}.{{{regName}_Const.{field.Name}}} ELSE 0 END) AS {field.Name}Розхід,
    SUM(CASE WHEN {regName}.income = true THEN 
        {regName}.{{{regName}_Const.{field.Name}}} ELSE 
        -{regName}.{{{regName}_Const.{field.Name}}} END) AS {field.Name}Залишок" +
        (++counter < ConfRegister.ResourcesFields.Count ? "," : "");

            query += @$"

FROM
    {{{regName}_Const.TABLE}} AS {regName}

WHERE
    date_trunc('day', {regName}.period::timestamp) = @ПеріодДеньВідбір

GROUP BY
    Період";

            //Виміри
            foreach (ConfigurationField field in ConfRegister.DimensionFields.Values)
                query += $", {field.Name}";

            query += @$"

HAVING";

            counter = 0;

            //Ресурси
            foreach (ConfigurationField field in ConfRegister.ResourcesFields.Values)
                query += @$"

    /* {field.Name} */
    SUM(CASE WHEN {regName}.income = true THEN 
        {regName}.{{{regName}_Const.{field.Name}}} ELSE 0 END) != 0 OR 
    SUM(CASE WHEN {regName}.income = false THEN 
        {regName}.{{{regName}_Const.{field.Name}}} ELSE 0 END) != 0 OR
    SUM(CASE WHEN {regName}.income = true THEN 
        {regName}.{{{regName}_Const.{field.Name}}} ELSE 
        -{regName}.{{{regName}_Const.{field.Name}}} END) != 0 " +
        (++counter < ConfRegister.ResourcesFields.Count ? "\n OR \n" : "");

            query += "\n\n\n\n\n";

            queryBlock.Query.Add("SELECT", query);
        }

        //
        // Обороти
        //

        void CreateQueryBlock_Обороти(ConfigurationTablePart TablePart)
        {
            string queryBlockKey = "Обороти";

            ConfigurationQueryBlock queryBlock;
            if (ConfRegister.QueryBlockList.TryGetValue(queryBlockKey, out ConfigurationQueryBlock? block))
            {
                queryBlock = block;
                queryBlock.Query.Clear();
            }
            else
                ConfRegister.AppendQueryBlockList(queryBlock = new ConfigurationQueryBlock(queryBlockKey));

            string regName = $"{ConfRegister.Name}";
            string tablePartName = $"{ConfRegister.Name}_{TablePart.Name}_TablePart";

            //
            // DELETE
            //

            queryBlock.Query.Add("DELETE", @$"
DELETE FROM 
    {{{tablePartName}.TABLE}}
WHERE 
    {{{tablePartName}.TABLE}}.{{{tablePartName}.Період}} = @ПеріодДеньВідбір
");

            //
            // INSERT
            //

            string query = @$"
INSERT INTO {{{tablePartName}.TABLE}} 
(
    uid,";

            int counter = 0;

            foreach (ConfigurationField field in TablePart.Fields.Values)
                query += @$"
    {{{tablePartName}.{field.Name}}}" + (++counter < TablePart.Fields.Count ? "," : "");

            query += @$"
)
SELECT
    uuid_generate_v4(),
    date_trunc('day', {regName}.period::timestamp) AS Період,";

            counter = 0;

            //Виміри
            foreach (ConfigurationField field in ConfRegister.DimensionFields.Values)
                query += @$"
    {regName}.{{{regName}_Const.{field.Name}}} AS {field.Name}" +
        (++counter < ConfRegister.DimensionFields.Count || ConfRegister.ResourcesFields.Count != 0 || ConfRegister.PropertyFields.Count != 0 ? "," : "");

            query += "\n";
            counter = 0;

            //Ресурси
            foreach (ConfigurationField field in ConfRegister.ResourcesFields.Values)
                query += @$"
    SUM({regName}.{{{regName}_Const.{field.Name}}}) AS {field.Name}" +
        (++counter < ConfRegister.ResourcesFields.Count || ConfRegister.PropertyFields.Count != 0 ? "," : "");

            query += "\n";
            counter = 0;

            //Реквізити
            foreach (ConfigurationField field in ConfRegister.PropertyFields.Values)
                query += @$"
    {regName}.{{{regName}_Const.{field.Name}}} AS {field.Name}" +
        (++counter < ConfRegister.PropertyFields.Count ? "," : "");

            query += @$"

FROM
    {{{regName}_Const.TABLE}} AS {regName}

WHERE
    date_trunc('day', {regName}.period::timestamp) = @ПеріодДеньВідбір

GROUP BY
    Період" + (ConfRegister.DimensionFields.Count != 0 || ConfRegister.PropertyFields.Count != 0 ? ", " : "");

            counter = 0;

            //Виміри
            foreach (ConfigurationField field in ConfRegister.DimensionFields.Values)
                query += $"{field.Name}" +
                (++counter < ConfRegister.DimensionFields.Count || ConfRegister.PropertyFields.Count != 0 ? ", " : "") +
                (counter % 5 == 0 ? "\n" : "");

            counter = 0;

            //Реквізити
            foreach (ConfigurationField field in ConfRegister.PropertyFields.Values)
                query += $"{field.Name}" +
                (++counter < ConfRegister.PropertyFields.Count ? ", " : "") +
                (counter % 5 == 0 ? "\n" : "");

            query += @$"

HAVING";

            counter = 0;

            //Ресурси
            foreach (ConfigurationField field in ConfRegister.ResourcesFields.Values)
                query += @$"
    SUM({regName}.{{{regName}_Const.{field.Name}}}) != 0 " +
        (++counter < ConfRegister.ResourcesFields.Count ? "OR" : "");

            query += "\n\n\n\n\n";

            queryBlock.Query.Add("SELECT", query);
        }

        //
        // Підсумки на основі залишків
        //

        void CreateQueryBlock_Підсумки(ConfigurationTablePart TablePart, ConfigurationTablePart Залишки_TablePart)
        {
            string queryBlockKey = "Підсумки";

            ConfigurationQueryBlock queryBlock;
            if (ConfRegister.QueryBlockList.TryGetValue(queryBlockKey, out ConfigurationQueryBlock? block))
            {
                queryBlock = block;
                queryBlock.FinalCalculation = true;
                queryBlock.Query.Clear();
            }
            else
                ConfRegister.AppendQueryBlockList(queryBlock = new ConfigurationQueryBlock(queryBlockKey, true));

            string regName = $"{ConfRegister.Name}";
            string tablePartName = $"{ConfRegister.Name}_{TablePart.Name}_TablePart";
            string tablePartName_Залишки = $"{ConfRegister.Name}_{Залишки_TablePart.Name}_TablePart";

            //
            // DELETE
            //

            queryBlock.Query.Add("DELETE", @$"
DELETE FROM {{{tablePartName}.TABLE}}
");

            //
            // INSERT
            //

            string query = @$"
INSERT INTO {{{tablePartName}.TABLE}} 
(
    uid,";

            int counter = 0;

            foreach (ConfigurationField field in TablePart.Fields.Values)
                query += @$"
    {{{tablePartName}.{field.Name}}}" + (++counter < TablePart.Fields.Count ? "," : "");

            query += @$"
)
SELECT
    uuid_generate_v4(),";

            counter = 0;

            //Виміри
            foreach (ConfigurationField field in ConfRegister.DimensionFields.Values)
            {
                query += @$"
    {regName}.{{{tablePartName_Залишки}.{field.Name}}} AS {field.Name},";
            }

            //Ресурси
            foreach (ConfigurationField field in ConfRegister.ResourcesFields.Values)
                query += @$"

    /* {field.Name} */
    SUM({regName}.{{{tablePartName_Залишки}.{field.Name}}}) AS {field.Name}" +
        (++counter < ConfRegister.ResourcesFields.Count ? "," : "");

            query += @$"

FROM
    {{{tablePartName_Залишки}.TABLE}} AS {regName}

GROUP BY
";
            counter = 0;

            //Виміри
            foreach (ConfigurationField field in ConfRegister.DimensionFields.Values)
                query += $"{field.Name}" +
                (++counter < ConfRegister.DimensionFields.Count ? ", " : "");

            query += @$"

HAVING";

            counter = 0;

            //Ресурси
            foreach (ConfigurationField field in ConfRegister.ResourcesFields.Values)
                query += @$"

    /* {field.Name} */
    SUM({regName}.{{{tablePartName_Залишки}.{field.Name}}}) != 0 " +
        (++counter < ConfRegister.ResourcesFields.Count ? " OR " : "");

            query += "\n\n\n\n\n";

            queryBlock.Query.Add("SELECT", query);
        }

        #endregion

        #region VirtualTable

        //
        // Залишки
        //

        async ValueTask<ConfigurationTablePart> CreateVirtualTable_Залишки()
        {
            ConfigurationTablePart TablePart = await CreateVirtualTable_Table("Залишки");

            int index = 0;
            CreateVirtualTable_Field(TablePart, new ConfigurationField("Період", "", "date", "", "", false, true));

            //Виміри
            foreach (ConfigurationField field in ConfRegister.DimensionFields.Values)
                CreateVirtualTable_Field(TablePart, field, "", ++index);

            //Ресурси
            foreach (ConfigurationField field in ConfRegister.ResourcesFields.Values)
                CreateVirtualTable_Field(TablePart, field, "", ++index);

            CreateQueryBlock_Залишки(TablePart);

            return TablePart;
        }

        async ValueTask CreateVirtualTable_ЗалишкиТаОбороти()
        {
            ConfigurationTablePart TablePart = await CreateVirtualTable_Table("ЗалишкиТаОбороти");

            int index = 0;
            CreateVirtualTable_Field(TablePart, new ConfigurationField("Період", "", "date", "", "", false, true));

            //Виміри
            foreach (ConfigurationField field in ConfRegister.DimensionFields.Values)
                CreateVirtualTable_Field(TablePart, field, "", ++index);

            //Ресурси
            foreach (ConfigurationField field in ConfRegister.ResourcesFields.Values)
            {
                CreateVirtualTable_Field(TablePart, field, "Прихід", ++index);
                CreateVirtualTable_Field(TablePart, field, "Розхід", ++index);
                CreateVirtualTable_Field(TablePart, field, "Залишок", ++index);
            }

            CreateQueryBlock_ЗалишкиТаОбороти(TablePart);
        }

        //
        // Обороти
        //

        async ValueTask CreateVirtualTable_Обороти()
        {
            ConfigurationTablePart TablePart = await CreateVirtualTable_Table("Обороти");

            int index = 0;
            CreateVirtualTable_Field(TablePart, new ConfigurationField("Період", "", "date", "", "", false, true));

            //Виміри
            foreach (ConfigurationField field in ConfRegister.DimensionFields.Values)
                CreateVirtualTable_Field(TablePart, field, "", ++index);

            //Ресурси
            foreach (ConfigurationField field in ConfRegister.ResourcesFields.Values)
                CreateVirtualTable_Field(TablePart, field, "", ++index);

            //Реквізити
            foreach (ConfigurationField field in ConfRegister.PropertyFields.Values)
                CreateVirtualTable_Field(TablePart, field, "", ++index);

            CreateQueryBlock_Обороти(TablePart);
        }

        //
        // Підсумки
        //

        async ValueTask CreateVirtualTable_Підсумки(ConfigurationTablePart Залишки_TablePart)
        {
            ConfigurationTablePart TablePart = await CreateVirtualTable_Table("Підсумки");

            int index = -1;

            //Виміри
            foreach (ConfigurationField field in ConfRegister.DimensionFields.Values)
                CreateVirtualTable_Field(TablePart, field, "", ++index);

            //Ресурси
            foreach (ConfigurationField field in ConfRegister.ResourcesFields.Values)
                CreateVirtualTable_Field(TablePart, field, "", ++index);

            CreateQueryBlock_Підсумки(TablePart, Залишки_TablePart);
        }

        async ValueTask<ConfigurationTablePart> CreateVirtualTable_Table(string tableName)
        {
            if (!ConfRegister.TabularParts.ContainsKey(tableName))
            {
                string table = await Configuration.GetNewUnigueTableName(Program.Kernel);
                ConfRegister.AppendTablePart(new ConfigurationTablePart(tableName, table, "Віртуальна таблиця"));
            }

            return ConfRegister.TabularParts[tableName];
        }

        void CreateVirtualTable_Field(ConfigurationTablePart TablePart, ConfigurationField field, string prefixName = "", int index = -1)
        {
            string fieldAndPrefix = field.Name + prefixName;

            if (!TablePart.Fields.ContainsKey(fieldAndPrefix))
            {
                string fieldColumnName = Configuration.GetNewUnigueColumnName(Program.Kernel, TablePart.Table, TablePart.Fields);

                if (index > -1)
                {
                    List<KeyValuePair<string, ConfigurationField>> listFields = TablePart.Fields.ToList();
                    TablePart.Fields.Clear();

                    listFields.Insert(index, new KeyValuePair<string, ConfigurationField>(fieldAndPrefix,
                        new ConfigurationField(fieldAndPrefix, fieldColumnName, field.Type, field.Pointer, field.Desc, false, field.IsIndex)));

                    foreach (KeyValuePair<string, ConfigurationField> itemField in listFields)
                        TablePart.AppendField(itemField.Value);
                }
                else
                    TablePart.AppendField(new ConfigurationField(fieldAndPrefix, fieldColumnName, field.Type, field.Pointer, field.Desc, false, field.IsIndex));
            }
        }

        async void OnCreateVirtualTableClick(object? sender, EventArgs args)
        {
            if (string.IsNullOrEmpty(entryName.Text))
            {
                Message.Error(GeneralForm, "Назва регістру не вказана");
                return;
            }

            if (Conf != null)
            {
                if (!Conf.RegistersAccumulation.ContainsKey(entryName.Text))
                {
                    Message.Error(GeneralForm, "Регістр не збережений в колекцію, потрібно спочатку Зберегти");
                    return;
                }

                if (ConfRegister.DimensionFields.Count == 0)
                {
                    Message.Error(GeneralForm, "Відсутні поля вимірів");
                    return;
                }

                if (ConfRegister.ResourcesFields.Count == 0)
                {
                    Message.Error(GeneralForm, "Відсутні поля ресурсів");
                    return;
                }

                switch (ConfRegister.TypeRegistersAccumulation)
                {
                    case TypeRegistersAccumulation.Residues: /* Залишки */
                        {
                            ConfigurationTablePart Залишки_TablePart = await CreateVirtualTable_Залишки();
                            await CreateVirtualTable_ЗалишкиТаОбороти();

                            if (!checkButtonNoSummary.Active)
                                await CreateVirtualTable_Підсумки(Залишки_TablePart);

                            break;
                        }
                    case TypeRegistersAccumulation.Turnover: /* Обороти */
                        {
                            await CreateVirtualTable_Обороти();
                            break;
                        }
                }

                TabularPartsRefreshList();
                QueryListRefreshList();

                GeneralForm?.LoadTreeAsync();
            }
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

                    if (ConfRegister.DimensionFields.TryGetValue(curRow.Child.Name, out ConfigurationField? field))
                        GeneralForm?.CreateNotebookPage($"Поле: {curRow.Child.Name}", () =>
                        {
                            Dictionary<string, ConfigurationField> AllFields = Conf.CombineAllFieldForRegister
                            (
                                ConfRegister.DimensionFields.Values,
                                ConfRegister.ResourcesFields.Values,
                                ConfRegister.PropertyFields.Values
                            );

                            PageField page = new PageField()
                            {
                                AllFields = AllFields,
                                Fields = ConfRegister.DimensionFields,
                                Field = field,
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
                Dictionary<string, ConfigurationField> AllFields = Conf.CombineAllFieldForRegister
                (
                    ConfRegister.DimensionFields.Values,
                    ConfRegister.ResourcesFields.Values,
                    ConfRegister.PropertyFields.Values
                );

                PageField page = new PageField()
                {
                    AllFields = AllFields,
                    Fields = ConfRegister.DimensionFields,
                    IsNew = true,
                    GeneralForm = GeneralForm,
                    Table = ConfRegister.Table,
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
                    if (ConfRegister.DimensionFields.TryGetValue(row.Child.Name, out ConfigurationField? field))
                    {
                        ConfigurationField newField = field.Copy();
                        newField.Name += GenerateName.GetNewName();

                        ConfRegister.AppendDimensionField(newField);
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
        }

        void OnDimensionFieldsRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxDimensionFields.SelectedRows;
            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                    ConfRegister.DimensionFields.Remove(row.Child.Name);

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
                    if (ConfRegister.ResourcesFields.TryGetValue(curRow.Child.Name, out ConfigurationField? field))
                        GeneralForm?.CreateNotebookPage($"Поле: {curRow.Child.Name}", () =>
                        {
                            Dictionary<string, ConfigurationField> AllFields = Conf.CombineAllFieldForRegister
                            (
                                ConfRegister.DimensionFields.Values,
                                ConfRegister.ResourcesFields.Values,
                                ConfRegister.PropertyFields.Values
                            );

                            PageField page = new PageField()
                            {
                                AllFields = AllFields,
                                Fields = ConfRegister.ResourcesFields,
                                Field = field,
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
                Dictionary<string, ConfigurationField> AllFields = Conf.CombineAllFieldForRegister
                (
                    ConfRegister.DimensionFields.Values,
                    ConfRegister.ResourcesFields.Values,
                    ConfRegister.PropertyFields.Values
                );

                PageField page = new PageField()
                {
                    AllFields = AllFields,
                    Fields = ConfRegister.ResourcesFields,
                    IsNew = true,
                    GeneralForm = GeneralForm,
                    Table = ConfRegister.Table,
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
                    if (ConfRegister.ResourcesFields.TryGetValue(row.Child.Name, out ConfigurationField? field))
                    {
                        ConfigurationField newField = field.Copy();
                        newField.Name += GenerateName.GetNewName();

                        ConfRegister.AppendResourcesField(newField);
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
        }

        void OnResourcesFieldsRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxResourcesFields.SelectedRows;
            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                    ConfRegister.ResourcesFields.Remove(row.Child.Name);

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
                    if (ConfRegister.PropertyFields.TryGetValue(curRow.Child.Name, out ConfigurationField? field))
                        GeneralForm?.CreateNotebookPage($"Поле: {curRow.Child.Name}", () =>
                        {
                            Dictionary<string, ConfigurationField> AllFields = Conf.CombineAllFieldForRegister
                            (
                                ConfRegister.DimensionFields.Values,
                                ConfRegister.ResourcesFields.Values,
                                ConfRegister.PropertyFields.Values
                            );

                            PageField page = new PageField()
                            {
                                AllFields = AllFields,
                                Fields = ConfRegister.PropertyFields,
                                Field = field,
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
                Dictionary<string, ConfigurationField> AllFields = Conf.CombineAllFieldForRegister
                (
                    ConfRegister.DimensionFields.Values,
                    ConfRegister.ResourcesFields.Values,
                    ConfRegister.PropertyFields.Values
                );

                PageField page = new PageField()
                {
                    AllFields = AllFields,
                    Fields = ConfRegister.PropertyFields,
                    IsNew = true,
                    GeneralForm = GeneralForm,
                    Table = ConfRegister.Table,
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
                    if (ConfRegister.PropertyFields.TryGetValue(row.Child.Name, out ConfigurationField? field))
                    {
                        ConfigurationField newField = field.Copy();
                        newField.Name += GenerateName.GetNewName();

                        ConfRegister.AppendPropertyField(newField);
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
        }

        void OnPropertyFieldsRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxPropertyFields.SelectedRows;
            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                    ConfRegister.PropertyFields.Remove(row.Child.Name);

                PropertyFieldsRefreshList();
                GeneralForm?.LoadTreeAsync();
            }
        }

        void PropertyFieldsRefreshList()
        {
            OnPropertyFieldsRefreshClick(null, new EventArgs());
        }

        #endregion

        #region TabularList

        void OnTabularListButtonPress(object? sender, ButtonPressEventArgs args)
        {
            if (args.Event.Type == Gdk.EventType.DoubleButtonPress)
            {
                ListBoxRow[] selectedRows = listBoxTabularList.SelectedRows;
                if (selectedRows.Length != 0)
                {
                    ListBoxRow curRow = selectedRows[0];
                    if (ConfRegister.TabularList.TryGetValue(curRow.Child.Name, out ConfigurationTabularList? tabularList))
                        GeneralForm?.CreateNotebookPage($"Табличний список: {curRow.Child.Name}", () =>
                        {
                            Dictionary<string, ConfigurationField> AllFields = Conf.CombineAllFieldForRegister
                            (
                                ConfRegister.DimensionFields.Values,
                                ConfRegister.ResourcesFields.Values,
                                ConfRegister.PropertyFields.Values
                            );

                            PageTabularList page = new PageTabularList()
                            {
                                Fields = AllFields,
                                TabularLists = ConfRegister.TabularList,
                                TabularList = tabularList,
                                IsNew = false,
                                GeneralForm = GeneralForm,
                                CallBack_RefreshList = TabularListRefreshList,
                                ConfOwnerName = "РегістриНакопичення"
                            };

                            page.SetValue();
                            return page;
                        });
                }
            }
        }

        void OnTabularListAddClick(object? sender, EventArgs args)
        {
            GeneralForm?.CreateNotebookPage("Табличний список *", () =>
            {
                Dictionary<string, ConfigurationField> AllFields = Conf.CombineAllFieldForRegister
                (
                    ConfRegister.DimensionFields.Values,
                    ConfRegister.ResourcesFields.Values,
                    ConfRegister.PropertyFields.Values
                );

                PageTabularList page = new PageTabularList()
                {
                    Fields = AllFields,
                    TabularLists = ConfRegister.TabularList,
                    IsNew = true,
                    GeneralForm = GeneralForm,
                    CallBack_RefreshList = TabularListRefreshList,
                    ConfOwnerName = "РегістриНакопичення"
                };

                page.SetValue();
                return page;
            });
        }

        void OnTabularListCopyClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxTabularList.SelectedRows;
            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                    if (ConfRegister.TabularList.TryGetValue(row.Child.Name, out ConfigurationTabularList? tabularList))
                    {
                        ConfigurationTabularList newTableList = tabularList.Copy();
                        newTableList.Name += GenerateName.GetNewName();

                        ConfRegister.AppendTableList(newTableList);
                    }

                TabularListRefreshList();
                GeneralForm?.LoadTreeAsync();
            }
        }

        void OnTabularListRefreshClick(object? sender, EventArgs args)
        {
            foreach (Widget item in listBoxTabularList.Children)
                listBoxTabularList.Remove(item);

            FillTabularList();
        }

        void OnTabularListRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxTabularList.SelectedRows;
            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                    ConfRegister.TabularList.Remove(row.Child.Name);

                TabularListRefreshList();
                GeneralForm?.LoadTreeAsync();
            }
        }

        void TabularListRefreshList()
        {
            OnTabularListRefreshClick(null, new EventArgs());
        }

        #endregion

        #region FormsList

        void OnFormsListButtonPress(object? sender, ButtonPressEventArgs args)
        {
            if (args.Event.Type == Gdk.EventType.DoubleButtonPress)
            {
                ListBoxRow[] selectedRows = listBoxFormsList.SelectedRows;
                if (selectedRows.Length != 0)
                {
                    ListBoxRow curRow = selectedRows[0];
                    if (ConfRegister.Forms.TryGetValue(curRow.Child.Name, out ConfigurationForms? form))
                        GeneralForm?.CreateNotebookPage($"Форма: {curRow.Child.Name}", () =>
                        {
                            Dictionary<string, ConfigurationField> AllFields = Conf.CombineAllFieldForRegister
                            (
                                ConfRegister.DimensionFields.Values,
                                ConfRegister.ResourcesFields.Values,
                                ConfRegister.PropertyFields.Values
                            );

                            PageForm page = new PageForm()
                            {
                                ParentName = ConfRegister.Name,
                                ParentType = "RegisterAccumulation",
                                Forms = ConfRegister.Forms,
                                Form = form,
                                TypeForm = form.Type,
                                Fields = AllFields,
                                TabularLists = ConfRegister.TabularList,
                                TabularList = form.TabularList,
                                IsNew = false,
                                GeneralForm = GeneralForm,
                                CallBack_RefreshList = FormsListRefreshList,
                                RegistersAccumulationOtherInfo = GetRegisterAccumulationOtherInfo()
                            };

                            page.SetValue();
                            return page;
                        });
                }
            }
        }

        Menu OnFormsListAddFormSubMenu()
        {
            //Внутрішня функція для субменю
            void OnFormsListAdd(ConfigurationForms.TypeForms typeForms)
            {
                if (string.IsNullOrEmpty(entryName.Text))
                {
                    Message.Error(GeneralForm, "Назва регістру не вказана");
                    return;
                }

                GeneralForm?.CreateNotebookPage("Форма *", () =>
                {
                    Dictionary<string, ConfigurationField> AllFields = Conf.CombineAllFieldForRegister
                    (
                        ConfRegister.DimensionFields.Values,
                        ConfRegister.ResourcesFields.Values,
                        ConfRegister.PropertyFields.Values
                    );

                    PageForm page = new PageForm()
                    {
                        ParentName = ConfRegister.Name,
                        ParentType = "RegisterAccumulation",
                        Forms = ConfRegister.Forms,
                        TypeForm = typeForms,
                        Fields = AllFields,
                        TabularLists = ConfRegister.TabularList,
                        IsNew = true,
                        GeneralForm = GeneralForm,
                        CallBack_RefreshList = FormsListRefreshList,
                        RegistersAccumulationOtherInfo = GetRegisterAccumulationOtherInfo()
                    };

                    page.SetValue();
                    return page;
                });
            }

            Menu Menu = new Menu();

            {
                MenuItem item = new MenuItem("Список");
                item.Activated += (object? sender, EventArgs args) => { OnFormsListAdd(ConfigurationForms.TypeForms.List); };
                Menu.Append(item);
            }

            {
                MenuItem item = new MenuItem("Звіт");
                item.Activated += (object? sender, EventArgs args) => { OnFormsListAdd(ConfigurationForms.TypeForms.Report); };
                Menu.Append(item);
            }

            Menu.ShowAll();

            return Menu;
        }

        void OnFormsListCopyClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxFormsList.SelectedRows;
            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                    if (ConfRegister.Forms.TryGetValue(row.Child.Name, out ConfigurationForms? form))
                    {
                        ConfigurationForms newForms = form.Copy();
                        newForms.Name += GenerateName.GetNewName();

                        ConfRegister.AppendForms(newForms);
                    }

                FormsListRefreshList();
            }
        }

        void OnFormsListRefreshClick(object? sender, EventArgs args)
        {
            foreach (Widget item in listBoxFormsList.Children)
                listBoxFormsList.Remove(item);

            FillFormsList();
        }

        void OnFormsListRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxFormsList.SelectedRows;
            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                    ConfRegister.Forms.Remove(row.Child.Name);

                FormsListRefreshList();
            }
        }

        void FormsListRefreshList()
        {
            OnFormsListRefreshClick(null, new EventArgs());
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
                    if (ConfRegister.TabularParts.TryGetValue(curRow.Child.Name, out ConfigurationTablePart? tablePart))
                        GeneralForm?.CreateNotebookPage($"Таблична частина: {curRow.Child.Name}", () =>
                        {
                            PageTablePart page = new PageTablePart()
                            {
                                TabularParts = ConfRegister.TabularParts,
                                TablePart = tablePart,
                                IsNew = false,
                                Owner = new OwnerTablePart(false, "RegistersAccumulation", ConfRegister.Name),
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
                    Owner = new OwnerTablePart(false, "RegistersAccumulation", ConfRegister.Name),
                    GeneralForm = GeneralForm,
                    CallBack_RefreshList = TabularPartsRefreshList
                };

                page.SetValue();
                return page;
            });
        }

        async void OnTabularPartsCopyClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxTableParts.SelectedRows;
            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                    if (ConfRegister.TabularParts.TryGetValue(row.Child.Name, out ConfigurationTablePart? tablePart))
                    {
                        ConfigurationTablePart newTablePart = tablePart.Copy();
                        newTablePart.Name += GenerateName.GetNewName();
                        newTablePart.Table = await Configuration.GetNewUnigueTableName(Program.Kernel);

                        ConfRegister.AppendTablePart(newTablePart);
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
        }

        void OnTabularPartsRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxTableParts.SelectedRows;
            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                    ConfRegister.TabularParts.Remove(row.Child.Name);

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
                    if (ConfRegister.QueryBlockList.TryGetValue(curRow.Child.Name, out ConfigurationQueryBlock? block))
                        GeneralForm?.CreateNotebookPage($"Блок запитів: {curRow.Child.Name}", () =>
                        {
                            PageQueryBlock page = new PageQueryBlock()
                            {
                                QueryBlockList = ConfRegister.QueryBlockList,
                                QueryBlock = block,
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
                    if (ConfRegister.QueryBlockList.TryGetValue(row.Child.Name, out ConfigurationQueryBlock? block))
                    {
                        ConfigurationQueryBlock newQueryBlock = block.Copy();
                        newQueryBlock.Name += GenerateName.GetNewName();

                        ConfRegister.AppendQueryBlockList(newQueryBlock);
                    }

                QueryListRefreshList();
            }
        }

        void OnQueryListRefreshClick(object? sender, EventArgs args)
        {
            foreach (Widget item in listBoxQueryBlock.Children)
                listBoxQueryBlock.Remove(item);

            FillQueryBlockList();
        }

        void OnQueryListRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxQueryBlock.SelectedRows;
            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                    ConfRegister.QueryBlockList.Remove(row.Child.Name);

                QueryListRefreshList();
            }
        }

        void QueryListRefreshList()
        {
            OnQueryListRefreshClick(null, new EventArgs());
        }

        #endregion
    }

    /// <summary>
    /// Структура для додаткової інформації про регістр
    /// </summary>
    struct RegisterAccumulationOtherInfoStruct(TypeRegistersAccumulation typeRegistersAccumulation = TypeRegistersAccumulation.Residues)
    {
        /// <summary>
        /// Тип регістру
        /// </summary>
        public TypeRegistersAccumulation TypeReg = typeRegistersAccumulation;
    }
}