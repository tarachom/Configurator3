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
using System.Xml;

namespace Configurator
{
    class PageDocument : VBox
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

        Entry entryName = new Entry() { WidthRequest = 500 };
        Entry entryFullName = new Entry() { WidthRequest = 500 };
        Entry entryTable = new Entry() { WidthRequest = 500 };
        Entry entrySpend = new Entry() { WidthRequest = 460 };
        Entry entryClearSpend = new Entry() { WidthRequest = 460 };

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

        #endregion

        public PageDocument() : base()
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

            //Повна Назва
            HBox hBoxFullName = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxFullName, false, false, 5);

            hBoxFullName.PackStart(new Label("Повна назва:"), false, false, 5);
            hBoxFullName.PackStart(entryFullName, false, false, 5);

            //Таблиця
            HBox hBoxTable = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxTable, false, false, 5);

            hBoxTable.PackStart(new Label("Таблиця:"), false, false, 5);
            hBoxTable.PackStart(entryTable, false, false, 5);

            //Опис
            HBox hBoxDesc = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxDesc, false, false, 5);

            hBoxDesc.PackStart(new Label("Опис:") { Valign = Align.Start }, false, false, 5);

            ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 500, HeightRequest = 100 };
            scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollTextView.Add(textViewDesc);

            hBoxDesc.PackStart(scrollTextView, false, false, 5);

            //Автоматична нумерація
            {
                Expander expanderAutoNum = new Expander("Автоматична нумерація");
                vBox.PackStart(expanderAutoNum, false, false, 5);

                VBox vBoxAutoNum = new VBox();
                expanderAutoNum.Add(vBoxAutoNum);

                //Прапорець
                HBox hBoxAutoNum = new HBox() { Halign = Align.Start };
                vBoxAutoNum.PackStart(hBoxAutoNum, false, false, 10);
                hBoxAutoNum.PackStart(checkButtonAutoNum, false, false, 5);

                //Заголовок
                HBox hBoxAutoNumInfo = new HBox() { Halign = Align.Start };
                vBoxAutoNum.PackStart(hBoxAutoNumInfo, false, false, 5);
                hBoxAutoNumInfo.PackStart(new Label(
                    "Для автоматичної нумерації використовується константа в блоці <b>НумераціяДокументів</b>. " +
                    "Назва константи - це назва документу.")
                { Wrap = true, UseMarkup = true }, false, false, 5);

                //Кнопка
                HBox hBoxAutoNumButton = new HBox() { Halign = Align.Start };
                vBoxAutoNum.PackStart(hBoxAutoNumButton, false, false, 5);

                Button buttonAddConstAutoNum = new Button("Створити константу");
                hBoxAutoNumButton.PackStart(buttonAddConstAutoNum, false, false, 5);

                buttonAddConstAutoNum.Clicked += (object? sender, EventArgs args) =>
                {
                    if (String.IsNullOrEmpty(entryName.Text))
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

            //Регістри накопичення
            {
                Expander expanderRegAccum = new Expander("Регістри накопичення");
                vBox.PackStart(expanderRegAccum, false, false, 5);

                VBox vBoxRegAccum = new VBox();
                expanderRegAccum.Add(vBoxRegAccum);

                //Заголовок списку регістрів
                HBox hBoxAllowRegAcummInfo = new HBox() { Halign = Align.Center };
                vBoxRegAccum.PackStart(hBoxAllowRegAcummInfo, false, false, 5);
                hBoxAllowRegAcummInfo.PackStart(new Label("Регістри накопичення які використовує документ"), false, false, 5);

                //Робить рухи по регістрах
                HBox hBoxAllowRegAcumm = new HBox() { Halign = Align.End };
                vBoxRegAccum.PackStart(hBoxAllowRegAcumm, false, false, 5);

                ScrolledWindow scrollAllowList = new ScrolledWindow() { ShadowType = ShadowType.In };
                scrollAllowList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
                scrollAllowList.SetSizeRequest(500, 500);

                scrollAllowList.Add(listBoxAllowRegAccum);
                hBoxAllowRegAcumm.PackStart(scrollAllowList, true, true, 5);
            }

            Expander expanderFuncAndTriger = new Expander("Функції та тригери");
            vBox.PackStart(expanderFuncAndTriger, false, false, 5);

            VBox vBoxFunc = new VBox();
            expanderFuncAndTriger.Add(vBoxFunc);

            //Функції
            {
                //Заголовок блоку Функції
                HBox hBoxSpendInfo = new HBox() { Halign = Align.Center };
                vBoxFunc.PackStart(hBoxSpendInfo, false, false, 5);
                hBoxSpendInfo.PackStart(new Label("Функції"), false, false, 5);

                //Проведення
                HBox hBoxSpend = new HBox() { Halign = Align.End };
                vBoxFunc.PackStart(hBoxSpend, false, false, 5);

                hBoxSpend.PackStart(new Label("Проведення:"), false, false, 5);
                hBoxSpend.PackStart(entrySpend, false, false, 5);

                //Очищення
                HBox hBoxClearSpend = new HBox() { Halign = Align.End };
                vBoxFunc.PackStart(hBoxClearSpend, false, false, 5);

                hBoxClearSpend.PackStart(new Label("Очищення:"), false, false, 5);
                hBoxClearSpend.PackStart(entryClearSpend, false, false, 5);

                //
                // Конструктор для генерування класу проведення
                //

                HBox hBoxTrigerConstructorSpend = new HBox() { Halign = Align.Center };
                vBoxFunc.PackStart(hBoxTrigerConstructorSpend, false, false, 5);

                Button buttonConstructorSpend = new Button("Конструктор");
                buttonConstructorSpend.Clicked += (object? sender, EventArgs args) =>
                {
                    TextView textViewCode = new TextView();

                    ScrolledWindow scrollCode = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 600, HeightRequest = 300 };
                    scrollCode.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
                    scrollCode.Add(textViewCode);

                    Popover popover = new Popover((Widget)sender!) { BorderWidth = 5 };
                    popover.Add(scrollCode);
                    popover.ShowAll();

                    //
                    // Code
                    //

                    entrySpend.Text = entryName.Text + "_SpendTheDocument.Spend";
                    entryClearSpend.Text = entryName.Text + "_SpendTheDocument.ClearSpend";

                    textViewCode.Buffer.Text = @$"
class {entryName.Text}_SpendTheDocument
{{
    public static async ValueTask<bool> Spend({entryName.Text}_Objest ДокументОбєкт)
    {{
        try
        {{
            // проведення документу
            // ...

            return true;
        }}
        catch (Exception ex)
        {{
            СпільніФункції.ДокументНеПроводиться(ДокументОбєкт, ДокументОбєкт.Назва, ex.Message);
            return false;
        }}
    }}

    public static async ValueTask ClearSpend({entryName.Text}_Objest ДокументОбєкт)
    {{
        await ValueTask.FromResult(true);
    }}
}}
";
                    textViewCode.Buffer.SelectRange(textViewCode.Buffer.StartIter, textViewCode.Buffer.EndIter);
                    textViewCode.GrabFocus();
                };

                hBoxTrigerConstructorSpend.PackStart(buttonConstructorSpend, false, false, 5);
            }

            //Тригери
            {
                //Заголовок блоку Тригери
                HBox hBoxTrigerInfo = new HBox() { Halign = Align.Center };
                vBoxFunc.PackStart(hBoxTrigerInfo, false, false, 5);
                hBoxTrigerInfo.PackStart(new Label("Тригери"), false, false, 5);

                //Новий
                HBox hBoxTrigerNew = new HBox() { Halign = Align.End };
                vBoxFunc.PackStart(hBoxTrigerNew, false, false, 5);

                hBoxTrigerNew.PackStart(new Label("Новий:"), false, false, 5);
                hBoxTrigerNew.PackStart(entryNew, false, false, 5);
                CreateSwitch(hBoxTrigerNew, switchNew);

                //Копіювання
                HBox hBoxTrigerCopying = new HBox() { Halign = Align.End };
                vBoxFunc.PackStart(hBoxTrigerCopying, false, false, 5);

                hBoxTrigerCopying.PackStart(new Label("Копіювання:"), false, false, 5);
                hBoxTrigerCopying.PackStart(entryCopying, false, false, 5);
                CreateSwitch(hBoxTrigerCopying, switchCopying);

                //Перед записом
                HBox hBoxTrigerBeforeSave = new HBox() { Halign = Align.End };
                vBoxFunc.PackStart(hBoxTrigerBeforeSave, false, false, 5);

                hBoxTrigerBeforeSave.PackStart(new Label("Перед записом:"), false, false, 5);
                hBoxTrigerBeforeSave.PackStart(entryBeforeSave, false, false, 5);
                CreateSwitch(hBoxTrigerBeforeSave, switchBeforeSave);

                //Після запису
                HBox hBoxTrigerAfterSave = new HBox() { Halign = Align.End };
                vBoxFunc.PackStart(hBoxTrigerAfterSave, false, false, 5);

                hBoxTrigerAfterSave.PackStart(new Label("Після запису:"), false, false, 5);
                hBoxTrigerAfterSave.PackStart(entryAfterSave, false, false, 5);
                CreateSwitch(hBoxTrigerAfterSave, switchAfterSave);

                //Перед встановлення мітки на виделення
                HBox hBoxTrigerSetDeletionLabel = new HBox() { Halign = Align.End };
                vBoxFunc.PackStart(hBoxTrigerSetDeletionLabel, false, false, 5);

                hBoxTrigerSetDeletionLabel.PackStart(new Label("Встановлення мітки:"), false, false, 5);
                hBoxTrigerSetDeletionLabel.PackStart(entrySetDeletionLabel, false, false, 5);
                CreateSwitch(hBoxTrigerSetDeletionLabel, switchSetDeletionLabel);

                //Перед видаленням
                HBox hBoxTrigerBeforeDelete = new HBox() { Halign = Align.End };
                vBoxFunc.PackStart(hBoxTrigerBeforeDelete, false, false, 5);

                hBoxTrigerBeforeDelete.PackStart(new Label("Перед видаленням:"), false, false, 5);
                hBoxTrigerBeforeDelete.PackStart(entryBeforeDelete, false, false, 5);
                CreateSwitch(hBoxTrigerBeforeDelete, switchBeforeDelete);

                //
                // Конструктор для генерування класу тригерів
                //

                HBox hBoxTrigerConstructor = new HBox() { Halign = Align.Center };
                vBoxFunc.PackStart(hBoxTrigerConstructor, false, false, 5);

                Button buttonConstructor = new Button("Конструктор");
                buttonConstructor.Clicked += (object? sender, EventArgs args) =>
                {
                    TextView textViewCode = new TextView();

                    ScrolledWindow scrollCode = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 600, HeightRequest = 300 };
                    scrollCode.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
                    scrollCode.Add(textViewCode);

                    Popover popover = new Popover((Widget)sender!) { BorderWidth = 5 };
                    popover.Add(scrollCode);
                    popover.ShowAll();

                    //
                    // Code
                    //

                    if (string.IsNullOrEmpty(entryNew.Text)) switchNew.Active = true;
                    if (string.IsNullOrEmpty(entryCopying.Text)) switchCopying.Active = true;
                    if (string.IsNullOrEmpty(entryBeforeSave.Text)) switchBeforeSave.Active = true;

                    entryNew.Text = entryName.Text + "_Triggers.New";
                    entryCopying.Text = entryName.Text + "_Triggers.Copying";
                    entryBeforeSave.Text = entryName.Text + "_Triggers.BeforeSave";
                    entryAfterSave.Text = entryName.Text + "_Triggers.AfterSave";
                    entrySetDeletionLabel.Text = entryName.Text + "_Triggers.SetDeletionLabel";
                    entryBeforeDelete.Text = entryName.Text + "_Triggers.BeforeDelete";

                    string AutoNumCode = checkButtonAutoNum.Active ?
                        $"ДокументОбєкт.НомерДок = (++НумераціяДокументів.{entryName.Text}_Const).ToString(\"D8\");" : "";

                    string NewCode = "ДокументОбєкт.ДатаДок = DateTime.Now;";

                    string CopyingCode = "ДокументОбєкт.Назва += \" - Копія\";";

                    textViewCode.Buffer.Text = @$"
class {entryName.Text}_Triggers
{{
    public static async ValueTask New({entryName.Text}_Objest ДокументОбєкт)
    {{
        {AutoNumCode}
        {NewCode}
        await ValueTask.FromResult(true);
    }}

    public static async ValueTask Copying({entryName.Text}_Objest ДокументОбєкт, {entryName.Text}_Objest Основа)
    {{
        {CopyingCode}
        await ValueTask.FromResult(true);
    }}

    public static async ValueTask BeforeSave({entryName.Text}_Objest ДокументОбєкт)
    {{
        ДокументОбєкт.Назва = $""{{{entryName.Text}_Const.FULLNAME}} №{{ДокументОбєкт.НомерДок}} від {{ДокументОбєкт.ДатаДок.ToShortDateString()}}"";
        await ValueTask.FromResult(true);
    }}

    public static async ValueTask AfterSave({entryName.Text}_Objest ДокументОбєкт)
    {{
        await ValueTask.FromResult(true);
    }}

    public static async ValueTask SetDeletionLabel({entryName.Text}_Objest ДокументОбєкт, bool label)
    {{
        await ValueTask.FromResult(true);
    }}

    public static async ValueTask BeforeDelete({entryName.Text}_Objest ДокументОбєкт)
    {{
        await ValueTask.FromResult(true);
    }}
}}
";
                    textViewCode.Buffer.SelectRange(textViewCode.Buffer.StartIter, textViewCode.Buffer.EndIter);
                    textViewCode.GrabFocus();
                };

                hBoxTrigerConstructor.PackStart(buttonConstructor, false, false, 5);
            }

            //Списки та форми
            {
                Expander expanderForm = new Expander("Табличні списки");
                vBox.PackStart(expanderForm, false, false, 5);

                VBox vBoxForm = new VBox();
                expanderForm.Add(vBoxForm);

                //Заголовок блоку Forms
                HBox hBoxInterfaceCreateInfo = new HBox() { Halign = Align.Center };
                vBoxForm.PackStart(hBoxInterfaceCreateInfo, false, false, 5);
                hBoxInterfaceCreateInfo.PackStart(new Label("Табличні списки"), false, false, 5);

                //Табличні списки
                CreateTabularList(vBoxForm);
            }

            //Генерування коду 
            {
                Expander expanderTemplates = new Expander("Генерування коду");
                vBox.PackStart(expanderTemplates, false, false, 5);

                VBox vBoxTemplates = new VBox();
                expanderTemplates.Add(vBoxTemplates);

                //Заголовок для списку
                HBox hBoxElementInfo = new HBox() { Halign = Align.Start };
                vBoxTemplates.PackStart(hBoxElementInfo, false, false, 5);
                hBoxElementInfo.PackStart(new Label("Для списку") { UseMarkup = true, Selectable = true }, false, false, 5);

                //Елемент
                HBox hBoxElement = new HBox() { Halign = Align.Start };
                vBoxTemplates.PackStart(hBoxElement, false, false, 5);
                {
                    Button buttonConstructorElement = new Button("Елемент");
                    hBoxElement.PackStart(buttonConstructorElement, false, false, 5);
                    buttonConstructorElement.Clicked += (object? sender, EventArgs args) => { GenerateCode((Widget)sender!, "Element", true, true); };

                    Button buttonConstructorList = new Button("Список");
                    hBoxElement.PackStart(buttonConstructorList, false, false, 5);
                    buttonConstructorList.Clicked += (object? sender, EventArgs args) => { GenerateCode((Widget)sender!, "List", false, true); };

                    Button buttonConstructorPointerControl = new Button("PointerControl");
                    hBoxElement.PackStart(buttonConstructorPointerControl, false, false, 5);
                    buttonConstructorPointerControl.Clicked += (object? sender, EventArgs args) => { GenerateCode((Widget)sender!, "PointerControl"); };
                }
            }

            hPaned.Pack1(vBox, false, false);
        }

        void CreatePack2(HPaned hPaned)
        {
            VBox vBox = new VBox();

            //Поля
            CreateFieldList(vBox);

            //Табличні частини
            CreateTablePartList(vBox);

            hPaned.Pack2(vBox, true, false);
        }

        void CreateFieldList(VBox vBoxContainer)
        {
            VBox vBox = new VBox();

            HBox hBox = new HBox();
            hBox.PackStart(new Label("Поля:"), false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(Stock.New) { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnFieldsAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(Stock.Copy) { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnFieldsCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(Stock.Refresh) { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnFieldsRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(Stock.Clear) { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnFieldsRemoveClick;
            toolbar.Add(buttonDelete);

            HBox hBoxScroll = new HBox();
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 400);

            listBoxFields.ButtonPressEvent += OnFieldsButtonPress;

            scrollList.Add(listBoxFields);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            vBoxContainer.PackStart(vBox, false, false, 0);
        }

        void CreateTablePartList(VBox vBoxContainer)
        {
            VBox vBox = new VBox();

            HBox hBox = new HBox();
            hBox.PackStart(new Label("Табличні частини:"), false, false, 5);
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
            scrollList.SetSizeRequest(0, 100);

            listBoxTableParts.ButtonPressEvent += OnTabularPartsButtonPress;

            scrollList.Add(listBoxTableParts);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            vBoxContainer.PackStart(vBox, false, false, 0);
        }

        void CreateTabularList(VBox vBoxContainer)
        {
            VBox vBox = new VBox();

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(Stock.New) { Label = "Додати", IsImportant = true };
            buttonAdd.Clicked += OnTabularListAddClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonCopy = new ToolButton(Stock.Copy) { Label = "Копіювати", IsImportant = true };
            buttonCopy.Clicked += OnTabularListCopyClick;
            toolbar.Add(buttonCopy);

            ToolButton buttonRefresh = new ToolButton(Stock.Refresh) { Label = "Обновити", IsImportant = true };
            buttonRefresh.Clicked += OnTabularListRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(Stock.Clear) { Label = "Видалити", IsImportant = true };
            buttonDelete.Clicked += OnTabularListRemoveClick;
            toolbar.Add(buttonDelete);

            HBox hBoxScroll = new HBox();
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 100);

            listBoxTabularList.ButtonPressEvent += OnTabularListButtonPress;

            scrollList.Add(listBoxTabularList);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);

            vBoxContainer.PackStart(vBox, false, false, 0);
        }

        void CreateSwitch(HBox hBoxContainer, Switch switchWidget)
        {
            HBox hBoxSwitch = new HBox();
            hBoxSwitch.PackStart(switchWidget, false, false, 0);

            VBox vBoxSwitch = new VBox() { Valign = Align.Center };
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
                ConfDocument.AppendField(new ConfigurationField("Назва", "docname", "string", "", "Назва", true, true));
                ConfDocument.AppendField(new ConfigurationField("ДатаДок", "docdate", "datetime", "", "ДатаДок", false, true));
                ConfDocument.AppendField(new ConfigurationField("НомерДок", "docnomer", "string", "", "НомерДок", false, true));

                string nameInTable_Comment = Configuration.GetNewUnigueColumnName(Program.Kernel, entryTable.Text, ConfDocument.Fields);
                ConfDocument.AppendField(new ConfigurationField("Коментар", nameInTable_Comment, "string", "", "Коментар"));

                //Заповнення списків
                ConfDocument.AppendTableList(new ConfigurationTabularList("Записи"));

                int sortNum = 0;
                bool sortField;
                string caption;

                //Заповнення полями списків
                foreach (var item in ConfDocument.Fields)
                {
                    ++sortNum;
                    sortField = item.Value.Name == "ДатаДок";
                    caption = item.Value.Name switch { "ДатаДок" => "Дата", "НомерДок" => "Номер", _ => item.Value.Name };
                    
                    ConfDocument.TabularList["Записи"].AppendField(new ConfigurationTabularListField(item.Value.Name, caption, 0, sortNum, sortField));
                }
            }
            else
                entryTable.Text = ConfDocument.Table;

            textViewDesc.Buffer.Text = ConfDocument.Desc;

            entrySpend.Text = ConfDocument.SpendFunctions.Spend;
            entryClearSpend.Text = ConfDocument.SpendFunctions.ClearSpend;

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

            FillAllowRegAccum();
            FillFields();
            FillTabularParts();
            FillTabularList();
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
                listBoxFields.Add(new Label(field.Name + (field.IsPresentation ? " [ представлення ]" : "")) { Name = field.Name, Halign = Align.Start });

            listBoxFields.ShowAll();
        }

        void FillTabularParts()
        {
            foreach (ConfigurationTablePart tablePart in ConfDocument.TabularParts.Values)
                listBoxTableParts.Add(new Label(tablePart.Name) { Name = tablePart.Name, Halign = Align.Start });

            listBoxTableParts.ShowAll();
        }

        void FillTabularList()
        {
            foreach (ConfigurationTabularList tableList in ConfDocument.TabularList.Values)
                listBoxTabularList.Add(new Label(tableList.Name) { Name = tableList.Name, Halign = Align.Start });

            listBoxTabularList.ShowAll();
        }

        void GetValue()
        {
            //Поле з повною назвою переноситься із назви
            if (string.IsNullOrEmpty(entryFullName.Text))
                entryFullName.Text = entryName.Text;

            ConfDocument.Name = entryName.Text;
            ConfDocument.FullName = entryFullName.Text;
            ConfDocument.Table = entryTable.Text;
            ConfDocument.Desc = textViewDesc.Buffer.Text;

            ConfDocument.SpendFunctions.Spend = entrySpend.Text;
            ConfDocument.SpendFunctions.ClearSpend = entryClearSpend.Text;

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

            //Доспупні регістри
            ConfDocument.AllowRegisterAccumulation.Clear();

            foreach (ListBoxRow item in listBoxAllowRegAccum.Children)
            {
                CheckButton cb = (CheckButton)item.Child;
                if (cb.Active)
                    ConfDocument.AllowRegisterAccumulation.Add(cb.Name);
            }
        }

        #endregion

        void OnSaveClick(object? sender, EventArgs args)
        {
            string name = entryName.Text;
            string errorList = Configuration.ValidateConfigurationObjectName(Program.Kernel, ref name);
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

                    if (ConfDocument.Fields.ContainsKey(curRow.Child.Name))
                        GeneralForm?.CreateNotebookPage($"Поле: {curRow.Child.Name}", () =>
                        {
                            PageField page = new PageField()
                            {
                                Fields = ConfDocument.Fields,
                                Field = ConfDocument.Fields[curRow.Child.Name],
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
                {
                    if (ConfDocument.Fields.ContainsKey(row.Child.Name))
                    {
                        ConfigurationField newField = ConfDocument.Fields[row.Child.Name].Copy();
                        newField.NameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, ConfDocument.Table, ConfDocument.Fields);
                        newField.Name += GenerateName.GetNewName();

                        ConfDocument.AppendField(newField);
                    }
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
                {
                    if (ConfDocument.Fields.ContainsKey(row.Child.Name))
                        ConfDocument.Fields.Remove(row.Child.Name);
                }

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

                    if (ConfDocument.TabularParts.ContainsKey(curRow.Child.Name))
                        GeneralForm?.CreateNotebookPage($"Таблична частина: {curRow.Child.Name}", () =>
                        {
                            PageTablePart page = new PageTablePart()
                            {
                                TabularParts = ConfDocument.TabularParts,
                                TablePart = ConfDocument.TabularParts[curRow.Child.Name],
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
                    TabularParts = ConfDocument.TabularParts,
                    IsNew = true,
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
                {
                    if (ConfDocument.TabularParts.ContainsKey(row.Child.Name))
                    {
                        ConfigurationTablePart newTablePart = ConfDocument.TabularParts[row.Child.Name].Copy();
                        newTablePart.Name += GenerateName.GetNewName();
                        newTablePart.Table = await Configuration.GetNewUnigueTableName(Program.Kernel);

                        ConfDocument.AppendTablePart(newTablePart);
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
        }

        void OnTabularPartsRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxTableParts.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (ConfDocument.TabularParts.ContainsKey(row.Child.Name))
                        ConfDocument.TabularParts.Remove(row.Child.Name);
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

        #region TabularList

        void OnTabularListButtonPress(object? sender, ButtonPressEventArgs args)
        {
            if (args.Event.Type == Gdk.EventType.DoubleButtonPress)
            {
                ListBoxRow[] selectedRows = listBoxTabularList.SelectedRows;

                if (selectedRows.Length != 0)
                {
                    ListBoxRow curRow = selectedRows[0];

                    if (ConfDocument.TabularList.ContainsKey(curRow.Child.Name))
                        GeneralForm?.CreateNotebookPage($"Табличний список: {curRow.Child.Name}", () =>
                        {
                            PageTabularList page = new PageTabularList()
                            {
                                Fields = ConfDocument.Fields,
                                TabularLists = ConfDocument.TabularList,
                                TabularList = ConfDocument.TabularList[curRow.Child.Name],
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
                {
                    if (ConfDocument.TabularList.ContainsKey(row.Child.Name))
                    {
                        ConfigurationTabularList newTableList = ConfDocument.TabularList[row.Child.Name].Copy();
                        newTableList.Name += GenerateName.GetNewName();

                        ConfDocument.AppendTableList(newTableList);
                    }
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
                {
                    if (ConfDocument.TabularList.ContainsKey(row.Child.Name))
                        ConfDocument.TabularList.Remove(row.Child.Name);
                }

                TabularListRefreshList();

                GeneralForm?.LoadTreeAsync();
            }
        }

        void TabularListRefreshList()
        {
            OnTabularListRefreshClick(null, new EventArgs());
        }

        #endregion

        #region Генерування коду

        void GenerateCode(Widget relative_to, string fileName, bool includeFields = false, bool includeTabularParts = false)
        {
            if (string.IsNullOrEmpty(entryName.Text))
            {
                Message.Error(GeneralForm, "Назва документу не вказана");
                return;
            }

            if (!Conf.Documents.ContainsKey(entryName.Text))
            {
                Message.Error(GeneralForm, "Документ не збережений в колекцію, потрібно спочатку зберегти");
                return;
            }

            XmlDocument xmlConfDocument = new XmlDocument();
            xmlConfDocument.AppendChild(xmlConfDocument.CreateXmlDeclaration("1.0", "utf-8", ""));

            XmlElement rootNode = xmlConfDocument.CreateElement("root");
            xmlConfDocument.AppendChild(rootNode);

            XmlElement nodeDirectory = xmlConfDocument.CreateElement("Document");
            rootNode.AppendChild(nodeDirectory);

            XmlElement nodeDirectoryName = xmlConfDocument.CreateElement("Name");
            nodeDirectoryName.InnerText = ConfDocument.Name;
            nodeDirectory.AppendChild(nodeDirectoryName);

            if (includeFields)
                Configuration.SaveFields(ConfDocument.Fields, xmlConfDocument, nodeDirectory, "Document");

            if (includeTabularParts)
                Configuration.SaveTabularParts(ConfDocument.TabularParts, xmlConfDocument, nodeDirectory);

            TextView textViewCode = new TextView();

            ScrolledWindow scrollCode = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 600, HeightRequest = 300 };
            scrollCode.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollCode.Add(textViewCode);

            Popover popover = new Popover(relative_to) { BorderWidth = 5 };
            popover.Add(scrollCode);
            popover.ShowAll();

            textViewCode.Buffer.Text = Configuration.Transform
            (
                xmlConfDocument,
                System.IO.Path.Combine(AppContext.BaseDirectory, "xslt/ConstructorDocument.xslt"),
                new Dictionary<string, object>
                {
                    { "File", fileName },
                    { "NameSpaceGenerationCode", Conf.NameSpaceGenerationCode },
                    { "NameSpace", Conf.NameSpace }
                }
            );

            textViewCode.Buffer.SelectRange(textViewCode.Buffer.StartIter, textViewCode.Buffer.EndIter);
            textViewCode.GrabFocus();
        }

        #endregion
    }
}