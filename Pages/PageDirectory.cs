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
    class PageDirectory : VBox
    {
        Configuration? Conf
        {
            get
            {
                return Program.Kernel?.Conf;
            }
        }

        public ConfigurationDirectories ConfDirectory { get; set; } = new ConfigurationDirectories();
        public FormConfigurator? GeneralForm { get; set; }
        public bool IsNew { get; set; } = true;

        #region Fields

        ListBox listBoxFields = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxTableParts = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxTabularList = new ListBox() { SelectionMode = SelectionMode.Single };

        Entry entryName = new Entry() { WidthRequest = 500 };
        Entry entryFullName = new Entry() { WidthRequest = 500 };
        Entry entryTable = new Entry() { WidthRequest = 500 };
        Entry entryNew = new Entry() { WidthRequest = 500 };
        Entry entryCopying = new Entry() { WidthRequest = 500 };
        Entry entryBeforeSave = new Entry() { WidthRequest = 500 };
        Entry entryAfterSave = new Entry() { WidthRequest = 500 };
        Entry entrySetDeletionLabel = new Entry() { WidthRequest = 500 };
        Entry entryBeforeDelete = new Entry() { WidthRequest = 500 };
        TextView textViewDesc = new TextView() { WrapMode = WrapMode.Word };
        CheckButton checkButtonAutoNum = new CheckButton("Автоматична нумерація");

        #endregion

        public PageDirectory() : base()
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

            //Базові поля
            {
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
            }

            //Автоматична нумерація
            {
                Expander expanderAutoNum = new Expander("Автоматична нумерація");
                vBox.PackStart(expanderAutoNum, false, false, 5);

                VBox vBoxAutoNum = new VBox();
                expanderAutoNum.Add(vBoxAutoNum);

                //Заголовок
                HBox hBoxAutoNumInfo = new HBox() { Halign = Align.Start };
                vBoxAutoNum.PackStart(hBoxAutoNumInfo, false, false, 5);
                hBoxAutoNumInfo.PackStart(new Label(
                    "Для автоматичної нумерації використовується константа в блоці <b>НумераціяДовідників</b>. " +
                    "Назва константи - це назва довідника, тому рекомендується спочатку записати довідник, " +
                    "а тоді вже включати автоматичну нумерацію.")
                { Wrap = true, UseMarkup = true }, false, false, 5);

                //Кнопка
                HBox hBoxAutoNum = new HBox() { Halign = Align.Start };
                vBoxAutoNum.PackStart(hBoxAutoNum, false, false, 5);
                hBoxAutoNum.PackStart(checkButtonAutoNum, false, false, 5);

                Button buttonAddConstAutoNum = new Button("Створити константу");
                hBoxAutoNum.PackStart(buttonAddConstAutoNum, false, false, 5);

                buttonAddConstAutoNum.Clicked += (object? sender, EventArgs args) =>
                {
                    if (String.IsNullOrEmpty(entryName.Text))
                    {
                        Message.Error(GeneralForm, "Назва довідника не вказана");
                        return;
                    }

                    if (Conf != null)
                    {
                        if (!Conf.Directories.ContainsKey(entryName.Text))
                        {
                            Message.Error(GeneralForm, "Довідник не збережений в колекцію, потрібно спочатку Зберегти");
                            return;
                        }

                        if (!Conf.ConstantsBlock.ContainsKey("НумераціяДовідників"))
                            Conf.AppendConstantsBlock(new ConfigurationConstantsBlock("НумераціяДовідників", "Нумерація довідників"));

                        ConfigurationConstantsBlock blockAutoNum = Conf.ConstantsBlock["НумераціяДовідників"];

                        //Назва поля в таблиці
                        string nameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel!, SpecialTables.Constants, GeneralForm!.GetConstantsAllFields());

                        if (!blockAutoNum.Constants.ContainsKey(entryName.Text))
                            blockAutoNum.AppendConstant(new ConfigurationConstants(entryName.Text, nameInTable, "integer", blockAutoNum));

                        checkButtonAutoNum.Active = true;
                        GeneralForm?.LoadTreeAsync();
                    }
                };
            }

            //Функції та тригери
            {
                Expander expanderTriger = new Expander("Тригери");
                vBox.PackStart(expanderTriger, false, false, 5);

                VBox vBoxTriger = new VBox();
                expanderTriger.Add(vBoxTriger);

                //Заголовок блоку Тригери
                HBox hBoxTrigerInfo = new HBox() { Halign = Align.Center };
                vBoxTriger.PackStart(hBoxTrigerInfo, false, false, 5);
                hBoxTrigerInfo.PackStart(new Label("Тригери"), false, false, 5);

                //Новий
                HBox hBoxTrigerNew = new HBox() { Halign = Align.End };
                vBoxTriger.PackStart(hBoxTrigerNew, false, false, 5);

                hBoxTrigerNew.PackStart(new Label("Новий:"), false, false, 5);
                hBoxTrigerNew.PackStart(entryNew, false, false, 5);

                //Копіювання
                HBox hBoxTrigerCopying = new HBox() { Halign = Align.End };
                vBoxTriger.PackStart(hBoxTrigerCopying, false, false, 5);

                hBoxTrigerCopying.PackStart(new Label("Копіювання:"), false, false, 5);
                hBoxTrigerCopying.PackStart(entryCopying, false, false, 5);

                //Перед записом
                HBox hBoxTrigerBeforeSave = new HBox() { Halign = Align.End };
                vBoxTriger.PackStart(hBoxTrigerBeforeSave, false, false, 5);

                hBoxTrigerBeforeSave.PackStart(new Label("Перед записом:"), false, false, 5);
                hBoxTrigerBeforeSave.PackStart(entryBeforeSave, false, false, 5);

                //Після запису
                HBox hBoxTrigerAfterSave = new HBox() { Halign = Align.End };
                vBoxTriger.PackStart(hBoxTrigerAfterSave, false, false, 5);

                hBoxTrigerAfterSave.PackStart(new Label("Після запису:"), false, false, 5);
                hBoxTrigerAfterSave.PackStart(entryAfterSave, false, false, 5);

                //Перед встановлення мітки на виделення
                HBox hBoxTrigerSetDeletionLabel = new HBox() { Halign = Align.End };
                vBoxTriger.PackStart(hBoxTrigerSetDeletionLabel, false, false, 5);

                hBoxTrigerSetDeletionLabel.PackStart(new Label("Встановлення мітки:"), false, false, 5);
                hBoxTrigerSetDeletionLabel.PackStart(entrySetDeletionLabel, false, false, 5);

                //Перед видаленням
                HBox hBoxTrigerBeforeDelete = new HBox() { Halign = Align.End };
                vBoxTriger.PackStart(hBoxTrigerBeforeDelete, false, false, 5);

                hBoxTrigerBeforeDelete.PackStart(new Label("Перед видаленням:"), false, false, 5);
                hBoxTrigerBeforeDelete.PackStart(entryBeforeDelete, false, false, 5);

                //
                // Конструктор для генерування класу тригерів
                //

                TextView textViewCode = new TextView();

                HBox hBoxTrigerConstructor = new HBox() { Halign = Align.Center };
                vBoxTriger.PackStart(hBoxTrigerConstructor, false, false, 5);

                Button buttonConstructor = new Button("Конструктор");
                buttonConstructor.Clicked += (object? sender, EventArgs args) =>
                {
                    entryNew.Text = entryName.Text + "_Triggers.New";
                    entryCopying.Text = entryName.Text + "_Triggers.Copying";
                    entryBeforeSave.Text = entryName.Text + "_Triggers.BeforeSave";
                    entryAfterSave.Text = entryName.Text + "_Triggers.AfterSave";
                    entrySetDeletionLabel.Text = entryName.Text + "_Triggers.SetDeletionLabel";
                    entryBeforeDelete.Text = entryName.Text + "_Triggers.BeforeDelete";

                    string AutoNumCode = checkButtonAutoNum.Active ?
                        $"ДовідникОбєкт.Код = (++НумераціяДовідників.{entryName.Text}_Const).ToString(\"D6\");" : "";

                    string CopyingCode = "ДовідникОбєкт.Назва += \" - Копія\";";

                    textViewCode.Buffer.Text = @$"
class {entryName.Text}_Triggers
{{
    public static void New({entryName.Text}_Objest ДовідникОбєкт)
    {{
        {AutoNumCode}
    }}

    public static void Copying({entryName.Text}_Objest ДовідникОбєкт, {entryName.Text}_Objest Основа)
    {{
        {CopyingCode}
    }}

    public static void BeforeSave({entryName.Text}_Objest ДовідникОбєкт)
    {{
        
    }}

    public static void AfterSave({entryName.Text}_Objest ДовідникОбєкт)
    {{
        
    }}

    public static void SetDeletionLabel({entryName.Text}_Objest ДовідникОбєкт, bool label)
    {{
        
    }}

    public static void BeforeDelete({entryName.Text}_Objest ДовідникОбєкт)
    {{
        
    }}
}}
";

                    textViewCode.Buffer.SelectRange(textViewCode.Buffer.StartIter, textViewCode.Buffer.EndIter);
                    textViewCode.GrabFocus();
                };

                hBoxTrigerConstructor.PackStart(buttonConstructor, false, false, 5);

                //Code C#

                HBox hBoxTrigerCode = new HBox() { Halign = Align.End };
                vBoxTriger.PackStart(hBoxTrigerCode, false, false, 5);

                ScrolledWindow scrollCode = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 650, HeightRequest = 200 };
                scrollCode.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
                scrollCode.Add(textViewCode);

                hBoxTrigerCode.PackStart(scrollCode, false, false, 5);
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

            //Шаблони
            {
                Expander expanderTemplates = new Expander("Готові шаблони");
                vBox.PackStart(expanderTemplates, false, false, 5);

                VBox vBoxTemplates = new VBox();
                expanderTemplates.Add(vBoxTemplates);

                //Заголовок
                HBox hBoxFolderInfo = new HBox() { Halign = Align.Start };
                vBoxTemplates.PackStart(hBoxFolderInfo, false, false, 5);
                hBoxFolderInfo.PackStart(new Label(
                    "<u>1. Довідник для ієрархії папок</u>\n\n" +
                    "Будуть створені поля <b>Назва</b> та <b>Родич</b>\n" +
                    "Додатково можна створити поле <b>Власник</b> якщо довідник підпорядкований іншому довіднику.")
                { Wrap = true, UseMarkup = true, Selectable = true }, false, false, 5);

                //Кнопка
                HBox hBoxFolder = new HBox() { Halign = Align.Start };
                vBoxTemplates.PackStart(hBoxFolder, false, false, 5);

                Button buttonConstructorFolder = new Button("Створити поля");
                hBoxFolder.PackStart(buttonConstructorFolder, false, false, 5);

                buttonConstructorFolder.Clicked += (object? sender, EventArgs args) =>
                {
                    if (String.IsNullOrEmpty(entryName.Text))
                    {
                        Message.Error(GeneralForm, "Назва довідника не вказана");
                        return;
                    }

                    if (!Conf!.Directories.ContainsKey(entryName.Text))
                    {
                        Message.Error(GeneralForm, "Довідник не збережений в колекцію, потрібно спочатку зберегти");
                        return;
                    }

                    if (!ConfDirectory.Fields.ContainsKey("Назва"))
                    {
                        string nameInTable_Name = Configuration.GetNewUnigueColumnName(Program.Kernel!, entryTable.Text, ConfDirectory.Fields);
                        ConfDirectory.AppendField(new ConfigurationObjectField("Назва", nameInTable_Name, "string", "", "Назва", true, true));
                    }

                    if (!ConfDirectory.Fields.ContainsKey("Родич"))
                    {
                        string nameInTable_Parent = Configuration.GetNewUnigueColumnName(Program.Kernel!, entryTable.Text, ConfDirectory.Fields);
                        ConfDirectory.AppendField(new ConfigurationObjectField("Родич", nameInTable_Parent, "pointer", "Довідники." + ConfDirectory.Name, "Родич", false, true));
                    }

                    FieldsRefreshList();
                    GeneralForm?.LoadTreeAsync();
                };
            }

            //Генерування коду 
            {
                TextView textViewCode = new TextView();

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

                    Button buttonConstructorList = new Button("Список");
                    hBoxElement.PackStart(buttonConstructorList, false, false, 5);

                    Button buttonConstructorListAndTree = new Button("Список з деревом");
                    hBoxElement.PackStart(buttonConstructorListAndTree, false, false, 5);

                    Button buttonConstructorListSmallSelect = new Button("Список швидкий вибір");
                    hBoxElement.PackStart(buttonConstructorListSmallSelect, false, false, 5);

                    buttonConstructorElement.Clicked += (object? sender, EventArgs args) => { GenerateCode("Element", textViewCode, true, true); };
                    buttonConstructorList.Clicked += (object? sender, EventArgs args) => { GenerateCode("List", textViewCode); };
                    buttonConstructorListAndTree.Clicked += (object? sender, EventArgs args) => { GenerateCode("ListAndTree", textViewCode, false, true); };
                    buttonConstructorListSmallSelect.Clicked += (object? sender, EventArgs args) => { GenerateCode("ListSmallSelect", textViewCode); };
                }

                //Заголовок для дерева
                HBox hBoxTreeInfo = new HBox() { Halign = Align.Start };
                vBoxTemplates.PackStart(hBoxTreeInfo, false, false, 5);
                hBoxTreeInfo.PackStart(new Label("Для дерева") { UseMarkup = true, Selectable = true }, false, false, 5);

                //Дерево
                HBox hBoxTree = new HBox() { Halign = Align.Start };
                vBoxTemplates.PackStart(hBoxTree, false, false, 5);
                {
                    Button buttonConstructorTree = new Button("Дерево");
                    hBoxTree.PackStart(buttonConstructorTree, false, false, 5);

                    Button buttonConstructorTreeSmallSelect = new Button("Дерево швидкий вибір");
                    hBoxTree.PackStart(buttonConstructorTreeSmallSelect, false, false, 5);

                    buttonConstructorTree.Clicked += (object? sender, EventArgs args) => { GenerateCode("Tree", textViewCode); };
                    buttonConstructorTreeSmallSelect.Clicked += (object? sender, EventArgs args) => { GenerateCode("TreeSmallSelect", textViewCode); };
                }

                //Заголовок PointerControl
                HBox hBoxPointerControlInfo = new HBox() { Halign = Align.Start };
                vBoxTemplates.PackStart(hBoxPointerControlInfo, false, false, 5);
                hBoxPointerControlInfo.PackStart(new Label("Елемент вибору") { UseMarkup = true, Selectable = true }, false, false, 5);

                //PointerControl
                HBox hBoxPointerControl = new HBox() { Halign = Align.Start };
                vBoxTemplates.PackStart(hBoxPointerControl, false, false, 5);
                {
                    Button buttonConstructorPointerControl = new Button("PointerControl");
                    hBoxPointerControl.PackStart(buttonConstructorPointerControl, false, false, 5);

                    buttonConstructorPointerControl.Clicked += (object? sender, EventArgs args) => { GenerateCode("PointerControl", textViewCode); };
                }

                //Code C#

                HBox hBoxCode = new HBox() { Halign = Align.End };
                vBoxTemplates.PackStart(hBoxCode, false, false, 5);

                ScrolledWindow scrollCode = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 650, HeightRequest = 300 };
                scrollCode.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
                scrollCode.Add(textViewCode);

                hBoxCode.PackStart(scrollCode, false, false, 5);
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

            HBox hBox = new HBox();
            hBox.PackStart(new Label("Табличні списки:"), false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

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

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            entryName.Text = ConfDirectory.Name;
            entryFullName.Text = ConfDirectory.FullName;

            if (IsNew)
            {
                entryTable.Text = Configuration.GetNewUnigueTableName(Program.Kernel!);

                string nameInTable_Code = Configuration.GetNewUnigueColumnName(Program.Kernel!, entryTable.Text, ConfDirectory.Fields);
                ConfDirectory.AppendField(new ConfigurationObjectField("Код", nameInTable_Code, "string", "", "Код", false, true));

                string nameInTable_Name = Configuration.GetNewUnigueColumnName(Program.Kernel!, entryTable.Text, ConfDirectory.Fields);
                ConfDirectory.AppendField(new ConfigurationObjectField("Назва", nameInTable_Name, "string", "", "Назва", true, true));

                ConfDirectory.AppendTableList(new ConfigurationTabularList("Записи", ""));
                ConfDirectory.AppendTableList(new ConfigurationTabularList("ЗаписиШвидкийВибір", ""));
            }
            else
                entryTable.Text = ConfDirectory.Table;

            textViewDesc.Buffer.Text = ConfDirectory.Desc;

            entryNew.Text = ConfDirectory.TriggerFunctions.New;
            entryCopying.Text = ConfDirectory.TriggerFunctions.Copying;
            entryBeforeSave.Text = ConfDirectory.TriggerFunctions.BeforeSave;
            entryAfterSave.Text = ConfDirectory.TriggerFunctions.AfterSave;
            entrySetDeletionLabel.Text = ConfDirectory.TriggerFunctions.SetDeletionLabel;
            entryBeforeDelete.Text = ConfDirectory.TriggerFunctions.BeforeDelete;

            checkButtonAutoNum.Active = ConfDirectory.AutomaticNumeration;

            FillFields();
            FillTabularParts();
            FillTabularList();
        }

        void FillFields()
        {
            foreach (ConfigurationObjectField field in ConfDirectory.Fields.Values)
                listBoxFields.Add(new Label(field.Name + (field.IsPresentation ? " [ представлення ]" : "")) { Name = field.Name, Halign = Align.Start });
        }

        void FillTabularParts()
        {
            foreach (ConfigurationObjectTablePart tablePart in ConfDirectory.TabularParts.Values)
                listBoxTableParts.Add(new Label(tablePart.Name) { Name = tablePart.Name, Halign = Align.Start });
        }

        void FillTabularList()
        {
            foreach (ConfigurationTabularList tableList in ConfDirectory.TabularList.Values)
                listBoxTabularList.Add(new Label(tableList.Name) { Name = tableList.Name, Halign = Align.Start });
        }

        void GetValue()
        {
            ConfDirectory.Name = entryName.Text;
            ConfDirectory.FullName = entryFullName.Text;
            ConfDirectory.Table = entryTable.Text;
            ConfDirectory.Desc = textViewDesc.Buffer.Text;

            ConfDirectory.TriggerFunctions.New = entryNew.Text;
            ConfDirectory.TriggerFunctions.Copying = entryCopying.Text;
            ConfDirectory.TriggerFunctions.BeforeSave = entryBeforeSave.Text;
            ConfDirectory.TriggerFunctions.AfterSave = entryAfterSave.Text;
            ConfDirectory.TriggerFunctions.SetDeletionLabel = entrySetDeletionLabel.Text;
            ConfDirectory.TriggerFunctions.BeforeDelete = entryBeforeDelete.Text;

            ConfDirectory.AutomaticNumeration = checkButtonAutoNum.Active;
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
                if (Conf!.Directories.ContainsKey(entryName.Text))
                {
                    Message.Error(GeneralForm, $"Назва довідника не унікальна");
                    return;
                }
            }
            else
            {
                if (ConfDirectory.Name != entryName.Text)
                {
                    if (Conf!.Directories.ContainsKey(entryName.Text))
                    {
                        Message.Error(GeneralForm, $"Назва довідника не унікальна");
                        return;
                    }
                }

                Conf!.Directories.Remove(ConfDirectory.Name);
            }

            GetValue();

            Conf!.AppendDirectory(ConfDirectory);

            IsNew = false;

            GeneralForm?.LoadTreeAsync();
            GeneralForm?.RenameCurrentPageNotebook($"Довідник: {ConfDirectory.Name}");
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

                    if (ConfDirectory.Fields.ContainsKey(curRow.Child.Name))
                        GeneralForm?.CreateNotebookPage($"Поле: {curRow.Child.Name}", () =>
                        {
                            PageField page = new PageField()
                            {
                                Fields = ConfDirectory.Fields,
                                Field = ConfDirectory.Fields[curRow.Child.Name],
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
                    Table = ConfDirectory.Table,
                    Fields = ConfDirectory.Fields,
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
                    if (ConfDirectory.Fields.ContainsKey(row.Child.Name))
                    {
                        ConfigurationObjectField newField = ConfDirectory.Fields[row.Child.Name].Copy();
                        newField.Name += GenerateName.GetNewName();
                        newField.NameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel!, ConfDirectory.Table, ConfDirectory.Fields);

                        ConfDirectory.AppendField(newField);
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

            listBoxFields.ShowAll();
        }

        void OnFieldsRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxFields.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (ConfDirectory.Fields.ContainsKey(row.Child.Name))
                        ConfDirectory.Fields.Remove(row.Child.Name);
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

                    if (ConfDirectory.TabularParts.ContainsKey(curRow.Child.Name))
                        GeneralForm?.CreateNotebookPage($"Таблична частина: {curRow.Child.Name}", () =>
                        {
                            PageTablePart page = new PageTablePart()
                            {
                                TabularParts = ConfDirectory.TabularParts,
                                TablePart = ConfDirectory.TabularParts[curRow.Child.Name],
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
                    TabularParts = ConfDirectory.TabularParts,
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
                    if (ConfDirectory.TabularParts.ContainsKey(row.Child.Name))
                    {
                        ConfigurationObjectTablePart newTablePart = ConfDirectory.TabularParts[row.Child.Name].Copy();
                        newTablePart.Name += GenerateName.GetNewName();
                        newTablePart.Table = Configuration.GetNewUnigueTableName(Program.Kernel!);

                        ConfDirectory.AppendTablePart(newTablePart);
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
                    if (ConfDirectory.TabularParts.ContainsKey(row.Child.Name))
                        ConfDirectory.TabularParts.Remove(row.Child.Name);
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

                    if (ConfDirectory.TabularList.ContainsKey(curRow.Child.Name))
                        GeneralForm?.CreateNotebookPage($"Табличний список: {curRow.Child.Name}", () =>
                        {
                            PageTabularList page = new PageTabularList()
                            {
                                Fields = ConfDirectory.Fields,
                                TabularLists = ConfDirectory.TabularList,
                                TabularList = ConfDirectory.TabularList[curRow.Child.Name],
                                IsNew = false,
                                GeneralForm = GeneralForm,
                                CallBack_RefreshList = TabularListRefreshList,
                                ConfOwnerName = "Довідники"
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
                    Fields = ConfDirectory.Fields,
                    TabularLists = ConfDirectory.TabularList,
                    IsNew = true,
                    GeneralForm = GeneralForm,
                    CallBack_RefreshList = TabularListRefreshList,
                    ConfOwnerName = "Довідники"
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
                    if (ConfDirectory.TabularList.ContainsKey(row.Child.Name))
                    {
                        ConfigurationTabularList newTableList = ConfDirectory.TabularList[row.Child.Name].Copy();
                        newTableList.Name += GenerateName.GetNewName();

                        ConfDirectory.AppendTableList(newTableList);
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

            listBoxTabularList.ShowAll();
        }

        void OnTabularListRemoveClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxTabularList.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (ConfDirectory.TabularList.ContainsKey(row.Child.Name))
                        ConfDirectory.TabularList.Remove(row.Child.Name);
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

        /// <summary>
        /// Функція для точкового генерування коду
        /// </summary>
        /// <param name="fileName">Назва файлу</param>
        /// <param name="textViewCode">Поле куди помістити згенерований код</param>
        /// <param name="includeFields">Вкласти інформацію про поля</param>
        /// <param name="includeTabularParts">Вкласти інформацію про табличні частини</param>
        void GenerateCode(string fileName, TextView textViewCode, bool includeFields = false, bool includeTabularParts = false)
        {
            if (String.IsNullOrEmpty(entryName.Text))
            {
                Message.Error(GeneralForm, "Назва довідника не вказана");
                return;
            }

            if (!Conf!.Directories.ContainsKey(entryName.Text))
            {
                Message.Error(GeneralForm, "Довідник не збережений в колекцію, потрібно спочатку зберегти");
                return;
            }

            XmlDocument xmlConfDocument = new XmlDocument();
            xmlConfDocument.AppendChild(xmlConfDocument.CreateXmlDeclaration("1.0", "utf-8", ""));

            XmlElement rootNode = xmlConfDocument.CreateElement("root");
            xmlConfDocument.AppendChild(rootNode);

            XmlElement nodeDirectory = xmlConfDocument.CreateElement("Directory");
            rootNode.AppendChild(nodeDirectory);

            XmlElement nodeDirectoryName = xmlConfDocument.CreateElement("Name");
            nodeDirectoryName.InnerText = ConfDirectory.Name;
            nodeDirectory.AppendChild(nodeDirectoryName);

            if (includeFields)
                Configuration.SaveFields(ConfDirectory.Fields, xmlConfDocument, nodeDirectory, "Directory");

            if (includeTabularParts)
                Configuration.SaveTabularParts(ConfDirectory.TabularParts, xmlConfDocument, nodeDirectory);

            Dictionary<string, object> arguments = new Dictionary<string, object>();
            arguments.Add("File", fileName);

            textViewCode.Buffer.Text = Configuration.Transform
            (
                xmlConfDocument,
                System.IO.Path.Combine(AppContext.BaseDirectory, "xslt/ConstructorDirectory.xslt"),
                arguments
            );

            textViewCode.Buffer.SelectRange(textViewCode.Buffer.StartIter, textViewCode.Buffer.EndIter);
            textViewCode.GrabFocus();
        }

        #endregion
    }
}