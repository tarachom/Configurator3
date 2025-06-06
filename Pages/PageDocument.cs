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
using GtkSource;

namespace Configurator
{
    class PageDocument : Box
    {
        Configuration Conf { get { return Program.Kernel.Conf; } }

        public ConfigurationDocuments ConfDocument { get; set; } = new ConfigurationDocuments();
        public FormConfigurator? GeneralForm { get; set; }
        public bool IsNew { get; set; } = true;

        #region Fields

        ListBox listBoxFields = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxTableParts = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxTabularList = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxAllowRegAccum = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxFormsList = new ListBox() { SelectionMode = SelectionMode.Single };

        Entry entryName = new Entry() { WidthRequest = 500 };
        Entry entryFullName = new Entry() { WidthRequest = 500 };
        Entry entryTable = new Entry() { WidthRequest = 500 };

        #region Функції проведення і очищення

        Entry entrySpend = new Entry() { WidthRequest = 400 };
        Entry entryClearSpend = new Entry() { WidthRequest = 400 };

        Switch switchSpend = new Switch();
        Switch switchClearSpend = new Switch();

        #endregion

        #region Trigers

        Entry entryNew = new Entry() { WidthRequest = 400 };
        Entry entryCopying = new Entry() { WidthRequest = 400 };
        Entry entryBeforeSave = new Entry() { WidthRequest = 400 };
        Entry entryAfterSave = new Entry() { WidthRequest = 400 };
        Entry entrySetDeletionLabel = new Entry() { WidthRequest = 400 };
        Entry entryBeforeDelete = new Entry() { WidthRequest = 400 };

        Switch switchNew = new Switch();
        Switch switchCopying = new Switch();
        Switch switchBeforeSave = new Switch();
        Switch switchAfterSave = new Switch();
        Switch switchSetDeletionLabel = new Switch();
        Switch switchBeforeDelete = new Switch();

        #endregion

        TextView textViewDesc = new TextView() { WrapMode = WrapMode.Word };
        CheckButton checkButtonAutoNum = new CheckButton("Автоматична нумерація");
        CheckButton checkButtonVersionsHistory = new CheckButton("Зберігати історію зміни даних");
        CheckButton checkButtonExportXml = new CheckButton("Формат XML");

        #endregion

        public PageDocument() : base(Orientation.Vertical, 0)
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

        void CreatePack1(Paned hPaned)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            //Базові поля
            {
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

                //Опис
                Box hBoxDesc = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
                vBox.PackStart(hBoxDesc, false, false, 5);

                hBoxDesc.PackStart(new Label("Опис:") { Valign = Align.Start }, false, false, 5);

                ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 500, HeightRequest = 100 };
                scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
                scrollTextView.Add(textViewDesc);

                hBoxDesc.PackStart(scrollTextView, false, false, 5);
            }

            //Автоматична нумерація
            {
                Expander expanderAutoNum = new Expander("Автоматична нумерація");
                vBox.PackStart(expanderAutoNum, false, false, 5);

                Box vBoxAutoNum = new Box(Orientation.Vertical, 0);
                expanderAutoNum.Add(vBoxAutoNum);

                //Прапорець
                Box hBoxAutoNum = new Box(Orientation.Horizontal, 0) { Halign = Align.Start };
                vBoxAutoNum.PackStart(hBoxAutoNum, false, false, 10);
                hBoxAutoNum.PackStart(checkButtonAutoNum, false, false, 5);

                //Заголовок
                Box hBoxAutoNumInfo = new Box(Orientation.Horizontal, 0) { Halign = Align.Start };
                vBoxAutoNum.PackStart(hBoxAutoNumInfo, false, false, 5);
                hBoxAutoNumInfo.PackStart(new Label(
                    "Для автоматичної нумерації використовується константа в блоці <b>НумераціяДокументів</b>. " +
                    "Назва константи - це назва документу.")
                { Wrap = true, UseMarkup = true }, false, false, 5);

                //Кнопка
                Box hBoxAutoNumButton = new Box(Orientation.Horizontal, 0) { Halign = Align.Start };
                vBoxAutoNum.PackStart(hBoxAutoNumButton, false, false, 5);

                Button buttonAddConstAutoNum = new Button("Створити константу");
                hBoxAutoNumButton.PackStart(buttonAddConstAutoNum, false, false, 5);

                buttonAddConstAutoNum.Clicked += (object? sender, EventArgs args) =>
                {
                    if (string.IsNullOrEmpty(entryName.Text))
                    {
                        Message.Error(GeneralForm, "Назва документу не вказана");
                        return;
                    }

                    if (Conf != null)
                    {
                        if (!Conf.Documents.ContainsKey(entryName.Text))
                        {
                            Message.Error(GeneralForm, "Документ не збережений в колекцію, потрібно спочатку Зберегти");
                            return;
                        }

                        if (!Conf.ConstantsBlock.ContainsKey("НумераціяДокументів"))
                            Conf.AppendConstantsBlock(new ConfigurationConstantsBlock("НумераціяДокументів", "Нумерація документів"));

                        ConfigurationConstantsBlock blockAutoNum = Conf.ConstantsBlock["НумераціяДокументів"];

                        //Назва поля в таблиці
                        string nameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, SpecialTables.Constants, GeneralForm!.GetConstantsAllFields());

                        if (!blockAutoNum.Constants.ContainsKey(entryName.Text))
                            blockAutoNum.AppendConstant(new ConfigurationConstants(entryName.Text, nameInTable, "integer", blockAutoNum));

                        checkButtonAutoNum.Active = true;
                        GeneralForm?.LoadTreeAsync();
                    }
                };
            }

            //Історія зміни даних
            {
                Expander expanderVersionsHistory = new Expander("Історія зміни даних");
                vBox.PackStart(expanderVersionsHistory, false, false, 5);

                Box vBoxVersionsHistory = new Box(Orientation.Vertical, 0);
                expanderVersionsHistory.Add(vBoxVersionsHistory);

                //Прапорець
                Box hBoxVersionsHistory = new Box(Orientation.Horizontal, 0) { Halign = Align.Start };
                vBoxVersionsHistory.PackStart(hBoxVersionsHistory, false, false, 10);
                hBoxVersionsHistory.PackStart(checkButtonVersionsHistory, false, false, 5);

                //Підказка
                Box hBoxVersionsHistoryInfo = new Box(Orientation.Horizontal, 0) { Halign = Align.Start };
                vBoxVersionsHistory.PackStart(hBoxVersionsHistoryInfo, false, false, 5);
                hBoxVersionsHistoryInfo.PackStart(new Label(
                    "При кожному збереженні об'єкта додатково буде зберігатися копія даних в історію змін")
                { Wrap = true, UseMarkup = true }, false, false, 5);
            }

            //Проведення та тригери
            {
                Expander expanderFuncAndTriger = new Expander("Проведення документу та тригери");
                vBox.PackStart(expanderFuncAndTriger, false, false, 5);

                Box vBoxFunc = new Box(Orientation.Vertical, 0);
                expanderFuncAndTriger.Add(vBoxFunc);

                //Проведення
                {
                    //Заголовок блоку Проведення
                    Box hBoxSpendInfo = new Box(Orientation.Horizontal, 0) { Halign = Align.Center };
                    vBoxFunc.PackStart(hBoxSpendInfo, false, false, 5);
                    hBoxSpendInfo.PackStart(new Label("Проведення документу"), false, false, 5);

                    //Проведення
                    {
                        Box hBox = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
                        vBoxFunc.PackStart(hBox, false, false, 5);

                        hBox.PackStart(new Label("Проведення:"), false, false, 5);
                        hBox.PackStart(entrySpend, false, false, 5);
                        CreateSwitch(hBox, switchSpend);
                    }

                    //Очищення
                    {
                        Box hBox = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
                        vBoxFunc.PackStart(hBox, false, false, 5);

                        hBox.PackStart(new Label("Очищення:"), false, false, 5);
                        hBox.PackStart(entryClearSpend, false, false, 5);
                        CreateSwitch(hBox, switchClearSpend);
                    }
                }

                //Тригери
                {
                    //Заголовок блоку Тригери
                    Box hBoxTrigerInfo = new Box(Orientation.Horizontal, 0) { Halign = Align.Center };
                    vBoxFunc.PackStart(hBoxTrigerInfo, false, false, 5);
                    hBoxTrigerInfo.PackStart(new Label("Тригери"), false, false, 5);

                    //Новий
                    {
                        Box hBox = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
                        vBoxFunc.PackStart(hBox, false, false, 5);

                        hBox.PackStart(new Label("Новий:"), false, false, 5);
                        hBox.PackStart(entryNew, false, false, 0);
                        CreateSwitch(hBox, switchNew);
                    }

                    //Копіювання
                    {
                        Box hBox = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
                        vBoxFunc.PackStart(hBox, false, false, 5);

                        hBox.PackStart(new Label("Копіювання:"), false, false, 5);
                        hBox.PackStart(entryCopying, false, false, 0);
                        CreateSwitch(hBox, switchCopying);
                    }

                    //Перед записом
                    {
                        Box hBox = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
                        vBoxFunc.PackStart(hBox, false, false, 5);

                        hBox.PackStart(new Label("Перед записом:"), false, false, 5);
                        hBox.PackStart(entryBeforeSave, false, false, 0);
                        CreateSwitch(hBox, switchBeforeSave);
                    }

                    //Після запису
                    {
                        Box hBox = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
                        vBoxFunc.PackStart(hBox, false, false, 5);

                        hBox.PackStart(new Label("Після запису:"), false, false, 5);
                        hBox.PackStart(entryAfterSave, false, false, 0);
                        CreateSwitch(hBox, switchAfterSave);
                    }

                    //Перед встановлення мітки на виделення
                    {
                        Box hBox = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
                        vBoxFunc.PackStart(hBox, false, false, 5);

                        hBox.PackStart(new Label("Встановлення мітки:"), false, false, 5);
                        hBox.PackStart(entrySetDeletionLabel, false, false, 0);
                        CreateSwitch(hBox, switchSetDeletionLabel);
                    }

                    //Перед видаленням
                    {
                        Box hBox = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
                        vBoxFunc.PackStart(hBox, false, false, 5);

                        hBox.PackStart(new Label("Перед видаленням:"), false, false, 5);
                        hBox.PackStart(entryBeforeDelete, false, false, 0);
                        CreateSwitch(hBox, switchBeforeDelete);
                    }
                }
            }

            //Регістри накопичення
            {
                Expander expanderRegAccum = new Expander("Регістри накопичення");
                vBox.PackStart(expanderRegAccum, false, false, 5);

                Box vBoxRegAccum = new Box(Orientation.Vertical, 0);
                expanderRegAccum.Add(vBoxRegAccum);

                //Заголовок списку регістрів
                Box hBoxAllowRegAcummInfo = new Box(Orientation.Horizontal, 0) { Halign = Align.Center };
                vBoxRegAccum.PackStart(hBoxAllowRegAcummInfo, false, false, 5);
                hBoxAllowRegAcummInfo.PackStart(new Label("Регістри накопичення які використовує документ"), false, false, 5);

                //Робить рухи по регістрах
                Box hBoxAllowRegAcumm = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
                vBoxRegAccum.PackStart(hBoxAllowRegAcumm, false, false, 5);

                ScrolledWindow scrollAllowList = new ScrolledWindow() { ShadowType = ShadowType.In };
                scrollAllowList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
                scrollAllowList.SetSizeRequest(500, 500);

                scrollAllowList.Add(listBoxAllowRegAccum);
                hBoxAllowRegAcumm.PackStart(scrollAllowList, true, true, 5);
            }

            //Експорт
            {
                Expander expanderExport = new Expander("Вигрузка у файл");
                vBox.PackStart(expanderExport, false, false, 5);

                Box vBoxExport = new Box(Orientation.Vertical, 0);
                expanderExport.Add(vBoxExport);

                //Прапорець
                Box hBoxExportXml = new Box(Orientation.Horizontal, 0) { Halign = Align.Start };
                vBoxExport.PackStart(hBoxExportXml, false, false, 10);
                hBoxExportXml.PackStart(checkButtonExportXml, false, false, 5);

                //Заголовок
                Box hBoxExportXmlInfo = new Box(Orientation.Horizontal, 0) { Halign = Align.Start };
                vBoxExport.PackStart(hBoxExportXmlInfo, false, false, 5);
                hBoxExportXmlInfo.PackStart(new Label("Для кожного поля додатково потрібно дозволити експорт") { Wrap = true, UseMarkup = true }, false, false, 5);

                //Кнопка
                Box hBoxExportButton = new Box(Orientation.Horizontal, 0) { Halign = Align.Start };
                vBoxExport.PackStart(hBoxExportButton, false, false, 5);

                Button buttonExportOn = new Button("Дозволити експорт для всіх полів");
                hBoxExportButton.PackStart(buttonExportOn, false, false, 5);

                buttonExportOn.Clicked += (object? sender, EventArgs args) =>
                {
                    if (Message.Request(GeneralForm, "Дозволити експорт для всіх полів документу та всіх полів табличних частин?") == ResponseType.Yes)
                    {
                        //Еспорт для всіх полів
                        foreach (ConfigurationField field in ConfDocument.Fields.Values)
                            field.IsExport = true;

                        //Обхід всіх табличних частин
                        foreach (ConfigurationTablePart tablePart in ConfDocument.TabularParts.Values)
                            //Еспорт для всіх полів
                            foreach (ConfigurationField field in tablePart.Fields.Values)
                                field.IsExport = true;
                    }
                };
            }

            //Списки та форми
            {
                Expander expanderForm = new Expander("Табличні списки");
                vBox.PackStart(expanderForm, false, false, 5);

                Box vBoxForm = new Box(Orientation.Vertical, 0);
                expanderForm.Add(vBoxForm);

                //Заголовок блоку Forms
                Box hBoxInterfaceCreateInfo = new Box(Orientation.Horizontal, 0) { Halign = Align.Center };
                vBoxForm.PackStart(hBoxInterfaceCreateInfo, false, false, 5);
                hBoxInterfaceCreateInfo.PackStart(new Label("Табличні списки"), false, false, 5);

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

            hPaned.Pack1(vBox, false, false);
        }

        void CreatePack2(Paned hPaned)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            //Стандартні поля
            Expander expanderDefField = new Expander("Стандартні поля");
            vBox.PackStart(expanderDefField, false, false, 5);

            expanderDefField.Add(new Label(" <b>uid</b> \n <b>deletion_label</b> - помітка на видалення \n <b>spend</b> - проведений (true) \n <b>spend_date</b> - дата та час проведення") { Halign = Align.Start, UseMarkup = true, UseUnderline = false, Selectable = true });

            //Поля
            CreateFieldList(vBox);

            //Табличні частини
            CreateTablePartList(vBox);

            hPaned.Pack2(vBox, true, false);
        }

        void CreateFieldList(Box vBoxContainer)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            Box hBox = new Box(Orientation.Horizontal, 0);
            hBox.PackStart(new Label("Поля:"), false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(new Image(Stock.New, IconSize.Menu), "Додати") { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnFieldsAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(new Image(Stock.Copy, IconSize.Menu), "Копіювати") { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnFieldsCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(new Image(Stock.Refresh, IconSize.Menu), "Обновити") { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnFieldsRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(new Image(Stock.Clear, IconSize.Menu), "Видалити") { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnFieldsRemoveClick;
            toolbar.Add(buttonDelete);

            Box hBoxScroll = new Box(Orientation.Horizontal, 0);
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 400);

            listBoxFields.ButtonPressEvent += OnFieldsButtonPress;

            scrollList.Add(listBoxFields);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            vBoxContainer.PackStart(vBox, false, false, 0);
        }

        void CreateTablePartList(Box vBoxContainer)
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
            scrollList.SetSizeRequest(0, 200);

            listBoxTableParts.ButtonPressEvent += OnTabularPartsButtonPress;

            scrollList.Add(listBoxTableParts);
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
            buttonCreateForms.Clicked += (sender, args) =>
            {
                void CreateForm(ConfigurationForms.TypeForms typeForms)
                {
                    //Перевірка чи вже є така форма
                    if (ConfDocument.Forms.Values.Any(x => x.Type == typeForms)) return;

                    PageForm page = new PageForm()
                    {
                        ParentName = ConfDocument.Name,
                        ParentType = "Document",
                        Forms = ConfDocument.Forms,
                        TypeForm = typeForms,
                        TriggerFunctions = ConfDocument.TriggerFunctions,
                        SpendFunctions = ConfDocument.SpendFunctions,
                        Fields = ConfDocument.Fields,
                        TabularParts = ConfDocument.TabularParts,
                        TabularLists = ConfDocument.TabularList,
                        IsNew = true,
                        GeneralForm = GeneralForm,
                        CallBack_RefreshList = FormsListRefreshList,
                        DocumentOtherInfo = GetDocumentOtherInfo(),
                        ModeOperation = PageForm.FormModeOperation.Automatic
                    };

                    page.SetValue();
                    page.GenerateCode();
                    page.OnSaveClick(page, new());
                }

                CreateForm(ConfigurationForms.TypeForms.Triggers);
                CreateForm(ConfigurationForms.TypeForms.SpendTheDocument);
                CreateForm(ConfigurationForms.TypeForms.Function);
                CreateForm(ConfigurationForms.TypeForms.Element);
                CreateForm(ConfigurationForms.TypeForms.List);
                CreateForm(ConfigurationForms.TypeForms.PointerControl);
            };

            Box hBox = new Box(Orientation.Horizontal, 0);
            hBox.PackStart(buttonCreateForms, false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            MenuToolButton buttonAdd = new MenuToolButton(new Image(Stock.New, IconSize.Menu), "Додати") { IsImportant = true, Menu = OnFormsListAddFormSubMenu() };
            buttonAdd.Clicked += (sender, arg) => { ((Menu)((MenuToolButton)sender!).Menu).Popup(); };
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

        void CreateSwitch(Box hBoxContainer, Switch switchWidget)
        {
            Box hBoxSwitch = new Box(Orientation.Horizontal, 0);
            hBoxSwitch.PackStart(switchWidget, false, false, 0);

            Box vBoxSwitch = new Box(Orientation.Vertical, 0) { Valign = Align.Center };
            vBoxSwitch.PackStart(hBoxSwitch, true, true, 0);

            hBoxContainer.PackEnd(vBoxSwitch, false, false, 5);
        }

        #region Присвоєння / зчитування значень віджетів

        public async void SetValue()
        {
            entryName.Text = ConfDocument.Name;
            entryFullName.Text = ConfDocument.FullName;

            if (IsNew)
            {
                entryTable.Text = await Configuration.GetNewUnigueTableName(Program.Kernel);

                //Заповнення полями
                ConfDocument.AppendField(new ConfigurationField("Назва", "Назва", "docname", "string", "", "Назва", true, true));
                ConfDocument.AppendField(new ConfigurationField("НомерДок", "Номер", "docnomer", "string", "", "Номер документу", false, true));
                ConfDocument.AppendField(new ConfigurationField("ДатаДок", "Дата", "docdate", "datetime", "", "Дата документу", false, true));

                string nameInTable_Comment = Configuration.GetNewUnigueColumnName(Program.Kernel, entryTable.Text, ConfDocument.Fields);
                ConfDocument.AppendField(new ConfigurationField("Коментар", "Коментар", nameInTable_Comment, "string", "", "Коментар"));

                string nameInTable_Basis = Configuration.GetNewUnigueColumnName(Program.Kernel, entryTable.Text, ConfDocument.Fields);
                ConfDocument.AppendField(new ConfigurationField("Основа", "Основа", nameInTable_Basis, "composite_pointer", "", "Основа"));

                //Заповнення списків
                ConfDocument.AppendTableList(new ConfigurationTabularList("Записи"));

                int sortNum = 0;

                //Заповнення полями списків (крім типів які ігноруються)
                foreach (var item in ConfDocument.Fields.Values.Where(x => { string[] typesIgnor = ["composite_pointer"]; return !typesIgnor.Contains(x.Type); }))
                {
                    string caption = item.Name switch { "ДатаДок" => "Дата", "НомерДок" => "Номер", _ => item.Name };
                    ConfDocument.TabularList["Записи"].AppendField(new ConfigurationTabularListField(item.Name, caption, 0, ++sortNum, item.Name == "ДатаДок"));
                }

                //Тригери
                ConfDocument.TriggerFunctions.NewAction = true;
                ConfDocument.TriggerFunctions.CopyingAction = true;
                ConfDocument.TriggerFunctions.BeforeSaveAction = true;
            }
            else
                entryTable.Text = ConfDocument.Table;

            textViewDesc.Buffer.Text = ConfDocument.Desc;

            #region Functions

            entrySpend.Text = ConfDocument.SpendFunctions.Spend;
            entryClearSpend.Text = ConfDocument.SpendFunctions.ClearSpend;

            switchSpend.Active = ConfDocument.SpendFunctions.SpendAction;
            switchClearSpend.Active = ConfDocument.SpendFunctions.ClearSpendAction;

            #endregion

            #region Trigers

            entryNew.Text = ConfDocument.TriggerFunctions.New;
            entryCopying.Text = ConfDocument.TriggerFunctions.Copying;
            entryBeforeSave.Text = ConfDocument.TriggerFunctions.BeforeSave;
            entryAfterSave.Text = ConfDocument.TriggerFunctions.AfterSave;
            entrySetDeletionLabel.Text = ConfDocument.TriggerFunctions.SetDeletionLabel;
            entryBeforeDelete.Text = ConfDocument.TriggerFunctions.BeforeDelete;

            switchNew.Active = ConfDocument.TriggerFunctions.NewAction;
            switchCopying.Active = ConfDocument.TriggerFunctions.CopyingAction;
            switchBeforeSave.Active = ConfDocument.TriggerFunctions.BeforeSaveAction;
            switchAfterSave.Active = ConfDocument.TriggerFunctions.AfterSaveAction;
            switchSetDeletionLabel.Active = ConfDocument.TriggerFunctions.SetDeletionLabelAction;
            switchBeforeDelete.Active = ConfDocument.TriggerFunctions.BeforeDeleteAction;

            #endregion

            checkButtonAutoNum.Active = ConfDocument.AutomaticNumeration;
            checkButtonVersionsHistory.Active = ConfDocument.VersionsHistory;
            checkButtonExportXml.Active = ConfDocument.ExportXml;

            FillAllowRegAccum();
            FillFields();
            FillTabularParts();
            FillTabularList();
            FillFormsList();
        }

        void FillAllowRegAccum()
        {
            foreach (ConfigurationRegistersAccumulation regAccum in Conf.RegistersAccumulation.Values)
                listBoxAllowRegAccum.Add(
                    new CheckButton(regAccum.Name)
                    {
                        Name = regAccum.Name,
                        Active = ConfDocument.AllowRegisterAccumulation.Contains(regAccum.Name)
                    });

            listBoxAllowRegAccum.ShowAll();
        }

        void FillFields()
        {
            foreach (ConfigurationField field in ConfDocument.Fields.Values)
                listBoxFields.Add(new Label(field.Name + (field.IsPresentation ? " [ представлення ]" : "")) { Name = field.Name, Halign = Align.Start, UseUnderline = false });

            listBoxFields.ShowAll();
        }

        void FillTabularParts()
        {
            foreach (ConfigurationTablePart tablePart in ConfDocument.TabularParts.Values)
                listBoxTableParts.Add(new Label(tablePart.Name) { Name = tablePart.Name, Halign = Align.Start, UseUnderline = false });

            listBoxTableParts.ShowAll();
        }

        void FillTabularList()
        {
            foreach (ConfigurationTabularList tableList in ConfDocument.TabularList.Values)
                listBoxTabularList.Add(new Label(tableList.Name) { Name = tableList.Name, Halign = Align.Start, UseUnderline = false });

            listBoxTabularList.ShowAll();
        }

        void FillFormsList()
        {
            foreach (ConfigurationForms form in ConfDocument.Forms.Values)
                listBoxFormsList.Add(new Label(form.Name) { Name = form.Name, Halign = Align.Start, UseUnderline = false });

            listBoxFormsList.ShowAll();
        }

        void GetValue()
        {
            //Поле з повною назвою переноситься із назви
            if (string.IsNullOrEmpty(entryFullName.Text))
                entryFullName.Text = Configuration.CreateFullName(entryName.Text);

            ConfDocument.Name = entryName.Text;
            ConfDocument.FullName = entryFullName.Text;
            ConfDocument.Table = entryTable.Text;
            ConfDocument.Desc = textViewDesc.Buffer.Text;

            #region Functions

            ConfDocument.SpendFunctions.Spend = entrySpend.Text;
            ConfDocument.SpendFunctions.ClearSpend = entryClearSpend.Text;

            ConfDocument.SpendFunctions.SpendAction = switchSpend.Active;
            ConfDocument.SpendFunctions.ClearSpendAction = switchClearSpend.Active;

            #endregion

            #region Trigers

            ConfDocument.TriggerFunctions.New = entryNew.Text;
            ConfDocument.TriggerFunctions.Copying = entryCopying.Text;
            ConfDocument.TriggerFunctions.BeforeSave = entryBeforeSave.Text;
            ConfDocument.TriggerFunctions.AfterSave = entryAfterSave.Text;
            ConfDocument.TriggerFunctions.SetDeletionLabel = entrySetDeletionLabel.Text;
            ConfDocument.TriggerFunctions.BeforeDelete = entryBeforeDelete.Text;

            ConfDocument.TriggerFunctions.NewAction = switchNew.Active;
            ConfDocument.TriggerFunctions.CopyingAction = switchCopying.Active;
            ConfDocument.TriggerFunctions.BeforeSaveAction = switchBeforeSave.Active;
            ConfDocument.TriggerFunctions.AfterSaveAction = switchAfterSave.Active;
            ConfDocument.TriggerFunctions.SetDeletionLabelAction = switchSetDeletionLabel.Active;
            ConfDocument.TriggerFunctions.BeforeDeleteAction = switchBeforeDelete.Active;

            #endregion

            ConfDocument.AutomaticNumeration = checkButtonAutoNum.Active;
            ConfDocument.VersionsHistory = checkButtonVersionsHistory.Active;
            ConfDocument.ExportXml = checkButtonExportXml.Active;

            //Доспупні регістри
            ConfDocument.AllowRegisterAccumulation.Clear();

            foreach (ListBoxRow item in listBoxAllowRegAccum.Children)
            {
                CheckButton cb = (CheckButton)item.Child;
                if (cb.Active)
                    ConfDocument.AllowRegisterAccumulation.Add(cb.Name);
            }
        }

        DocumentOtherInfoStruct GetDocumentOtherInfo()
        {
            return new DocumentOtherInfoStruct(ConfDocument.AutomaticNumeration, checkButtonExportXml.Active);
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
                if (Conf.Documents.ContainsKey(entryName.Text))
                {
                    Message.Error(GeneralForm, $"Назва документу не унікальна");
                    return;
                }
            }
            else
            {
                if (ConfDocument.Name != entryName.Text)
                {
                    if (Conf.Documents.ContainsKey(entryName.Text))
                    {
                        Message.Error(GeneralForm, $"Назва документу не унікальна");
                        return;
                    }
                }

                Conf.Documents.Remove(ConfDocument.Name);
            }

            GetValue();

            Conf.AppendDocument(ConfDocument);

            IsNew = false;

            GeneralForm?.LoadTreeAsync();
            GeneralForm?.RenameCurrentPageNotebook($"Документ: {ConfDocument.Name}");
        }

        #region Fields

        void OnFieldsButtonPress(object? sender, ButtonPressEventArgs args)
        {
            if (args.Event.Type == Gdk.EventType.DoubleButtonPress)
            {
                ListBoxRow[] selectedRows = listBoxFields.SelectedRows;
                if (selectedRows.Length != 0)
                {
                    ListBoxRow curRow = selectedRows[0];
                    if (ConfDocument.Fields.TryGetValue(curRow.Child.Name, out ConfigurationField? field))
                        GeneralForm?.CreateNotebookPage($"Поле: {curRow.Child.Name}", () =>
                        {
                            PageField page = new PageField()
                            {
                                Fields = ConfDocument.Fields,
                                Field = field,
                                IsNew = false,
                                GeneralForm = GeneralForm,
                                CallBack_RefreshList = FieldsRefreshList
                            };

                            page.SetValue();
                            return page;
                        });
                }
            }
        }

        void OnFieldsAddClick(object? sender, EventArgs args)
        {
            GeneralForm?.CreateNotebookPage("Поле *", () =>
            {
                PageField page = new PageField()
                {
                    Table = ConfDocument.Table,
                    Fields = ConfDocument.Fields,
                    IsNew = true,
                    GeneralForm = GeneralForm,
                    CallBack_RefreshList = FieldsRefreshList
                };

                page.SetValue();

                return page;
            });
        }

        void OnFieldsCopyClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxFields.SelectedRows;
            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                    if (ConfDocument.Fields.TryGetValue(row.Child.Name, out ConfigurationField? field))
                    {
                        ConfigurationField newField = field.Copy();
                        newField.NameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, ConfDocument.Table, ConfDocument.Fields);
                        newField.Name += GenerateName.GetNewName();

                        ConfDocument.AppendField(newField);
                    }

                FieldsRefreshList();
                GeneralForm?.LoadTreeAsync();
            }
        }

        void OnFieldsRefreshClick(object? sender, EventArgs args)
        {
            foreach (Widget item in listBoxFields.Children)
                listBoxFields.Remove(item);

            FillFields();
        }

        void OnFieldsRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxFields.SelectedRows;
            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                    ConfDocument.Fields.Remove(row.Child.Name);

                FieldsRefreshList();
                GeneralForm?.LoadTreeAsync();
            }
        }

        void FieldsRefreshList()
        {
            OnFieldsRefreshClick(null, new EventArgs());
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

                    if (ConfDocument.TabularParts.TryGetValue(curRow.Child.Name, out ConfigurationTablePart? tablePart))
                        GeneralForm?.CreateNotebookPage($"Таблична частина: {curRow.Child.Name}", () =>
                        {
                            PageTablePart page = new PageTablePart()
                            {
                                TabularParts = ConfDocument.TabularParts,
                                TablePart = tablePart,
                                IsNew = false,
                                Owner = new OwnerTablePart(true, "Document", ConfDocument.Name),
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
                    TabularParts = ConfDocument.TabularParts,
                    IsNew = true,
                    Owner = new OwnerTablePart(true, "Document", ConfDocument.Name),
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
                    if (ConfDocument.TabularParts.TryGetValue(row.Child.Name, out ConfigurationTablePart? tablePart))
                    {
                        ConfigurationTablePart newTablePart = tablePart.Copy();
                        newTablePart.Name += GenerateName.GetNewName();
                        newTablePart.Table = await Configuration.GetNewUnigueTableName(Program.Kernel);

                        ConfDocument.AppendTablePart(newTablePart);
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
                    ConfDocument.TabularParts.Remove(row.Child.Name);

                TabularPartsRefreshList();
                GeneralForm?.LoadTreeAsync();
            }
        }

        void TabularPartsRefreshList()
        {
            OnTabularPartsRefreshClick(null, new EventArgs());
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
                    if (ConfDocument.TabularList.TryGetValue(curRow.Child.Name, out ConfigurationTabularList? tabularList))
                        GeneralForm?.CreateNotebookPage($"Табличний список: {curRow.Child.Name}", () =>
                        {
                            PageTabularList page = new PageTabularList()
                            {
                                Fields = ConfDocument.Fields,
                                TabularLists = ConfDocument.TabularList,
                                TabularList = tabularList,
                                IsNew = false,
                                GeneralForm = GeneralForm,
                                CallBack_RefreshList = TabularListRefreshList,
                                ConfOwnerName = "Документи"
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
                PageTabularList page = new PageTabularList()
                {
                    Fields = ConfDocument.Fields,
                    TabularLists = ConfDocument.TabularList,
                    IsNew = true,
                    GeneralForm = GeneralForm,
                    CallBack_RefreshList = TabularListRefreshList,
                    ConfOwnerName = "Документи"
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
                    if (ConfDocument.TabularList.TryGetValue(row.Child.Name, out ConfigurationTabularList? tabularList))
                    {
                        ConfigurationTabularList newTableList = tabularList.Copy();
                        newTableList.Name += GenerateName.GetNewName();

                        ConfDocument.AppendTableList(newTableList);
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
                    ConfDocument.TabularList.Remove(row.Child.Name);

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
                    if (ConfDocument.Forms.TryGetValue(curRow.Child.Name, out ConfigurationForms? form))
                        GeneralForm?.CreateNotebookPage($"Форма: {curRow.Child.Name}", () =>
                        {
                            PageForm page = new PageForm()
                            {
                                ParentName = ConfDocument.Name,
                                ParentType = "Document",
                                Forms = ConfDocument.Forms,
                                Form = form,
                                TypeForm = form.Type,
                                TriggerFunctions = ConfDocument.TriggerFunctions,
                                SpendFunctions = ConfDocument.SpendFunctions,
                                Fields = ConfDocument.Fields,
                                TabularParts = ConfDocument.TabularParts,
                                TabularLists = ConfDocument.TabularList,
                                IsNew = false,
                                GeneralForm = GeneralForm,
                                CallBack_RefreshList = FormsListRefreshList,
                                DocumentOtherInfo = GetDocumentOtherInfo()
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
                    Message.Error(GeneralForm, "Назва документу не вказана");
                    return;
                }

                GeneralForm?.CreateNotebookPage("Форма *", () =>
                {
                    PageForm page = new PageForm()
                    {
                        ParentName = ConfDocument.Name,
                        ParentType = "Document",
                        Forms = ConfDocument.Forms,
                        TypeForm = typeForms,
                        TriggerFunctions = ConfDocument.TriggerFunctions,
                        SpendFunctions = ConfDocument.SpendFunctions,
                        Fields = ConfDocument.Fields,
                        TabularParts = ConfDocument.TabularParts,
                        TabularLists = ConfDocument.TabularList,
                        IsNew = true,
                        GeneralForm = GeneralForm,
                        CallBack_RefreshList = FormsListRefreshList,
                        DocumentOtherInfo = GetDocumentOtherInfo()
                    };

                    page.SetValue();
                    return page;
                });
            }

            Menu Menu = new Menu();

            {
                MenuItem item = new MenuItem("Тригери");
                item.Activated += (object? sender, EventArgs args) => { OnFormsListAdd(ConfigurationForms.TypeForms.Triggers); };
                Menu.Append(item);
            }

            {
                MenuItem item = new MenuItem("Проведення документу");
                item.Activated += (object? sender, EventArgs args) => { OnFormsListAdd(ConfigurationForms.TypeForms.SpendTheDocument); };
                Menu.Append(item);
            }

            {
                MenuItem item = new MenuItem("Функції");
                item.Activated += (object? sender, EventArgs args) => { OnFormsListAdd(ConfigurationForms.TypeForms.Function); };
                Menu.Append(item);
            }

            {
                MenuItem item = new MenuItem("Елемент");
                item.Activated += (object? sender, EventArgs args) => { OnFormsListAdd(ConfigurationForms.TypeForms.Element); };
                Menu.Append(item);
            }

            {
                MenuItem item = new MenuItem("Список");
                item.Activated += (object? sender, EventArgs args) => { OnFormsListAdd(ConfigurationForms.TypeForms.List); };
                Menu.Append(item);
            }

            {
                MenuItem item = new MenuItem("PointerControl");
                item.Activated += (object? sender, EventArgs args) => { OnFormsListAdd(ConfigurationForms.TypeForms.PointerControl); };
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
                    if (ConfDocument.Forms.TryGetValue(row.Child.Name, out ConfigurationForms? form))
                    {
                        ConfigurationForms newForms = form.Copy();
                        newForms.Name += GenerateName.GetNewName();

                        ConfDocument.AppendForms(newForms);
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
                    ConfDocument.Forms.Remove(row.Child.Name);

                FormsListRefreshList();
            }
        }

        void FormsListRefreshList()
        {
            OnFormsListRefreshClick(null, new EventArgs());
        }

        #endregion
    }

    /// <summary>
    /// Структура для додаткової інформації про документ
    /// </summary>
    struct DocumentOtherInfoStruct(bool automaticNumeration = false, bool exportXML = false)
    {
        public bool AutomaticNumeration = automaticNumeration;

        /// <summary>
        /// Тип регістру
        /// </summary>
        public bool ExportXML = exportXML;
    }
}