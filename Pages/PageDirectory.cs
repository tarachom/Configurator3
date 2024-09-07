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
    class PageDirectory : Box
    {
        Configuration Conf { get { return Program.Kernel.Conf; } }

        public ConfigurationDirectories ConfDirectory { get; set; } = new ConfigurationDirectories();
        public FormConfigurator? GeneralForm { get; set; }
        public bool IsNew { get; set; } = true;

        #region Fields

        ListBox listBoxFields = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxTableParts = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxTabularList = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxFormsList = new ListBox() { SelectionMode = SelectionMode.Single };

        Entry entryName = new Entry() { WidthRequest = 500 };
        Entry entryFullName = new Entry() { WidthRequest = 500 };
        Entry entryTable = new Entry() { WidthRequest = 500 };

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
        ComboBoxText comboBoxTypeDir = new ComboBoxText();
        ComboBoxText comboBoxPointerFolders = new ComboBoxText();
        ComboBoxText comboBoxParentField = new ComboBoxText();

        #endregion

        public PageDirectory() : base(Orientation.Vertical, 0)
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
                {
                    Box hBoxDesc = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
                    vBox.PackStart(hBoxDesc, false, false, 5);

                    hBoxDesc.PackStart(new Label("Опис:") { Valign = Align.Start }, false, false, 5);

                    ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 500, HeightRequest = 100 };
                    scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
                    scrollTextView.Add(textViewDesc);

                    hBoxDesc.PackStart(scrollTextView, false, false, 5);
                }
            }

            //Ієрархія
            {
                Expander expanderHierarchical = new Expander("Ієрархія");
                vBox.PackStart(expanderHierarchical, false, false, 5);

                Box vBoxHierarchical = new Box(Orientation.Vertical, 0);
                expanderHierarchical.Add(vBoxHierarchical);

                //Тип довідника
                {
                    Box hBoxTypeDir = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
                    vBoxHierarchical.PackStart(hBoxTypeDir, false, false, 5);

                    comboBoxTypeDir.Append(ConfigurationDirectories.TypeDirectories.Normal.ToString(), "Звичайний");
                    comboBoxTypeDir.Append(ConfigurationDirectories.TypeDirectories.Hierarchical.ToString(), "Ієрархічний");
                    comboBoxTypeDir.Append(ConfigurationDirectories.TypeDirectories.HierarchyInAnotherDirectory.ToString(), "Ієрархія в окремому довіднику");

                    hBoxTypeDir.PackStart(new Label("Тип довідника:"), false, false, 5);
                    hBoxTypeDir.PackStart(comboBoxTypeDir, false, false, 5);
                }

                //Поле родич для Ієрархічного довідника
                {
                    Box hBoxParentField = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
                    vBoxHierarchical.PackStart(hBoxParentField, false, false, 5);

                    hBoxParentField.PackStart(new Label("Поле <b>Родич</b> для <b>Ієрархічного</b>:") { UseMarkup = true }, false, false, 2);
                    hBoxParentField.PackStart(comboBoxParentField, false, false, 5);
                }

                //Вказівник на ієрархію в окремому довіднику
                {
                    Box hBoxPointerFolders = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
                    vBoxHierarchical.PackStart(hBoxPointerFolders, false, false, 5);

                    Box hBoxCaptionAndCombobox = new Box(Orientation.Horizontal, 0) { Sensitive = false };
                    hBoxCaptionAndCombobox.PackStart(new Label("Довідник для ієрархії:"), false, false, 2);
                    hBoxCaptionAndCombobox.PackStart(comboBoxPointerFolders, false, false, 2);

                    hBoxPointerFolders.PackStart(hBoxCaptionAndCombobox, false, false, 5);

                    FillPointerFolders();
                }

                //Кнопка створення окремого довідника для ієрархії або додаткових полів
                Button buttonAdd = new Button("Створити");
                buttonAdd.Clicked += async (object? sender, EventArgs args) =>
                {
                    if (string.IsNullOrEmpty(entryName.Text))
                    {
                        Message.Error(GeneralForm, "Назва довідника не вказана");
                        return;
                    }

                    if (!Conf.Directories.ContainsKey(entryName.Text))
                    {
                        Message.Error(GeneralForm, "Довідник не збережений в колекцію, потрібно спочатку зберегти");
                        return;
                    }

                    if (comboBoxTypeDir.ActiveId == ConfigurationDirectories.TypeDirectories.HierarchyInAnotherDirectory.ToString())
                    {
                        string newConfDirectoryName = entryName.Text + "_Папки";

                        if (!Conf.Directories.ContainsKey(newConfDirectoryName))
                        {
                            ConfigurationDirectories newConfDirectory = new ConfigurationDirectories
                            {
                                Name = newConfDirectoryName,
                                FullName = entryName.Text + " Папки",
                                Desc = entryName.Text + " Папки",
                                TypeDirectory = ConfigurationDirectories.TypeDirectories.Hierarchical,
                                Table = await Configuration.GetNewUnigueTableName(Program.Kernel),
                                ParentField_Hierarchical = "Родич"
                            };

                            //Код
                            string nameInTable_Code = Configuration.GetNewUnigueColumnName(Program.Kernel, newConfDirectory.Table, newConfDirectory.Fields);
                            newConfDirectory.AppendField(new ConfigurationField("Код", nameInTable_Code, "string", "", "Код", false, true));

                            //Назва
                            string nameInTable_Name = Configuration.GetNewUnigueColumnName(Program.Kernel, newConfDirectory.Table, newConfDirectory.Fields);
                            newConfDirectory.AppendField(new ConfigurationField("Назва", nameInTable_Name, "string", "", "Назва", true, true));

                            //Родич
                            string nameInTable_Parent = Configuration.GetNewUnigueColumnName(Program.Kernel, newConfDirectory.Table, newConfDirectory.Fields);
                            newConfDirectory.AppendField(new ConfigurationField("Родич", nameInTable_Parent, "pointer", $"Довідники.{newConfDirectory.Name}", "Родич", false, true));

                            //Заповнення списків
                            newConfDirectory.AppendTableList(new ConfigurationTabularList("Записи", ""));
                            newConfDirectory.AppendTableList(new ConfigurationTabularList("ЗаписиШвидкийВибір", ""));

                            Conf.AppendDirectory(newConfDirectory);

                            //Перевантажити список довідників
                            FillPointerFolders();

                            //Встановити створений довідник як вибраний у списку
                            comboBoxPointerFolders.ActiveId = $"Довідники.{newConfDirectoryName}";

                            //Додати нове поле Папка в основний довідник
                            if (!ConfDirectory.Fields.ContainsKey("Папка"))
                            {
                                string nameInTable_Folder = Configuration.GetNewUnigueColumnName(Program.Kernel, ConfDirectory.Table, ConfDirectory.Fields);
                                ConfDirectory.AppendField(new ConfigurationField("Папка", nameInTable_Folder, "pointer", $"Довідники.{newConfDirectory.Name}", "Папка", false, true));
                            }

                            //Перевантажити список полів
                            FieldsRefreshList();

                            //Перевантажити дерево
                            GeneralForm?.LoadTreeAsync();
                        }
                        else
                        {
                            comboBoxPointerFolders.ActiveId = $"Довідники.{newConfDirectoryName}";
                        }
                    }
                    else if (comboBoxTypeDir.ActiveId == ConfigurationDirectories.TypeDirectories.Hierarchical.ToString())
                    {
                        if (!ConfDirectory.Fields.ContainsKey("Родич"))
                        {
                            // Родич
                            string nameInTable_Parent = Configuration.GetNewUnigueColumnName(Program.Kernel, ConfDirectory.Table, ConfDirectory.Fields);
                            ConfDirectory.AppendField(new ConfigurationField("Родич", nameInTable_Parent, "pointer", $"Довідники.{ConfDirectory.Name}", "Родич", false, true));
                            ConfDirectory.ParentField_Hierarchical = "Родич";

                            //Перевантажити список полів
                            FieldsRefreshList();

                            //Перевантажити дерево
                            GeneralForm?.LoadTreeAsync();
                        }
                    }
                };

                //Обробка вибору типу довідника
                comboBoxTypeDir.Changed += (object? sender, EventArgs args) =>
                {
                    //Вибір поля тільки для Hierarchical
                    comboBoxParentField.Parent.Sensitive =
                        comboBoxTypeDir.ActiveId == ConfigurationDirectories.TypeDirectories.Hierarchical.ToString();

                    //Вибір папки тільки якщо HierarchyInAnotherDirectory
                    comboBoxPointerFolders.Parent.Sensitive =
                        comboBoxTypeDir.ActiveId == ConfigurationDirectories.TypeDirectories.HierarchyInAnotherDirectory.ToString();

                    //Кнопка працює тільки Hierarchical або HierarchyInAnotherDirectory
                    buttonAdd.Sensitive =
                        comboBoxTypeDir.ActiveId == ConfigurationDirectories.TypeDirectories.Hierarchical.ToString() ||
                        comboBoxTypeDir.ActiveId == ConfigurationDirectories.TypeDirectories.HierarchyInAnotherDirectory.ToString();
                };

                //Кнопка
                {
                    Box hBoxButton = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
                    vBoxHierarchical.PackStart(hBoxButton, false, false, 5);

                    hBoxButton.PackStart(buttonAdd, false, false, 5);
                }
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

                //Підказка
                Box hBoxAutoNumInfo = new Box(Orientation.Horizontal, 0) { Halign = Align.Start };
                vBoxAutoNum.PackStart(hBoxAutoNumInfo, false, false, 5);
                hBoxAutoNumInfo.PackStart(new Label(
                    "Для автоматичної нумерації використовується константа в блоці <b>НумераціяДовідників</b>. " +
                    "Назва константи - це назва довідника.")
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
                        string nameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, SpecialTables.Constants, GeneralForm!.GetConstantsAllFields());

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

                Box vBoxTriger = new Box(Orientation.Vertical, 0);
                expanderTriger.Add(vBoxTriger);

                //Заголовок блоку Тригери
                Box hBoxTrigerInfo = new Box(Orientation.Horizontal, 0) { Halign = Align.Center };
                vBoxTriger.PackStart(hBoxTrigerInfo, false, false, 5);
                hBoxTrigerInfo.PackStart(new Label("Тригери"), false, false, 5);

                //Новий
                Box hBoxTrigerNew = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
                vBoxTriger.PackStart(hBoxTrigerNew, false, false, 5);

                hBoxTrigerNew.PackStart(new Label("Новий:"), false, false, 5);
                hBoxTrigerNew.PackStart(entryNew, false, false, 0);
                CreateSwitch(hBoxTrigerNew, switchNew);

                //Копіювання
                Box hBoxTrigerCopying = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
                vBoxTriger.PackStart(hBoxTrigerCopying, false, false, 5);

                hBoxTrigerCopying.PackStart(new Label("Копіювання:"), false, false, 5);
                hBoxTrigerCopying.PackStart(entryCopying, false, false, 0);
                CreateSwitch(hBoxTrigerCopying, switchCopying);

                //Перед записом
                Box hBoxTrigerBeforeSave = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
                vBoxTriger.PackStart(hBoxTrigerBeforeSave, false, false, 5);

                hBoxTrigerBeforeSave.PackStart(new Label("Перед записом:"), false, false, 5);
                hBoxTrigerBeforeSave.PackStart(entryBeforeSave, false, false, 0);
                CreateSwitch(hBoxTrigerBeforeSave, switchBeforeSave);

                //Після запису
                Box hBoxTrigerAfterSave = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
                vBoxTriger.PackStart(hBoxTrigerAfterSave, false, false, 5);

                hBoxTrigerAfterSave.PackStart(new Label("Після запису:"), false, false, 5);
                hBoxTrigerAfterSave.PackStart(entryAfterSave, false, false, 0);
                CreateSwitch(hBoxTrigerAfterSave, switchAfterSave);

                //Перед встановлення мітки на виделення
                Box hBoxTrigerSetDeletionLabel = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
                vBoxTriger.PackStart(hBoxTrigerSetDeletionLabel, false, false, 5);

                hBoxTrigerSetDeletionLabel.PackStart(new Label("Встановлення мітки:"), false, false, 5);
                hBoxTrigerSetDeletionLabel.PackStart(entrySetDeletionLabel, false, false, 0);
                CreateSwitch(hBoxTrigerSetDeletionLabel, switchSetDeletionLabel);

                //Перед видаленням
                Box hBoxTrigerBeforeDelete = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
                vBoxTriger.PackStart(hBoxTrigerBeforeDelete, false, false, 5);

                hBoxTrigerBeforeDelete.PackStart(new Label("Перед видаленням:"), false, false, 5);
                hBoxTrigerBeforeDelete.PackStart(entryBeforeDelete, false, false, 5);
                CreateSwitch(hBoxTrigerBeforeDelete, switchBeforeDelete);

                //
                // Конструктор для генерування класу тригерів
                //

                Box hBoxTrigerConstructor = new Box(Orientation.Horizontal, 0) { Halign = Align.Center };
                vBoxTriger.PackStart(hBoxTrigerConstructor, false, false, 5);

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

                    if (string.IsNullOrEmpty(entryNew.Text)) switchNew.Active = true;
                    if (string.IsNullOrEmpty(entryCopying.Text)) switchCopying.Active = true;

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
    public static async ValueTask New({entryName.Text}_Objest ДовідникОбєкт)
    {{
        {AutoNumCode}
        await ValueTask.FromResult(true);
    }}

    public static async ValueTask Copying({entryName.Text}_Objest ДовідникОбєкт, {entryName.Text}_Objest Основа)
    {{
        {CopyingCode}
        await ValueTask.FromResult(true);
    }}

    public static async ValueTask BeforeSave({entryName.Text}_Objest ДовідникОбєкт)
    {{
        await ValueTask.FromResult(true);
    }}

    public static async ValueTask AfterSave({entryName.Text}_Objest ДовідникОбєкт)
    {{
        await ValueTask.FromResult(true);
    }}

    public static async ValueTask SetDeletionLabel({entryName.Text}_Objest ДовідникОбєкт, bool label)
    {{
        await ValueTask.FromResult(true);
    }}

    public static async ValueTask BeforeDelete({entryName.Text}_Objest ДовідникОбєкт)
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

                Box vBoxForm = new Box(Orientation.Vertical, 0);
                expanderForm.Add(vBoxForm);

                //Заголовок блоку Forms
                Box hBoxInterfaceCreateInfo = new Box(Orientation.Horizontal, 0) { Halign = Align.Center };
                vBoxForm.PackStart(hBoxInterfaceCreateInfo, false, false, 5);
                hBoxInterfaceCreateInfo.PackStart(new Label("Табличні списки"), false, false, 5);

                //Табличні списки
                CreateTabularList(vBoxForm);
            }

            //Шаблони
            /*
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
                    if (string.IsNullOrEmpty(entryName.Text))
                    {
                        Message.Error(GeneralForm, "Назва довідника не вказана");
                        return;
                    }

                    if (!Conf.Directories.ContainsKey(entryName.Text))
                    {
                        Message.Error(GeneralForm, "Довідник не збережений в колекцію, потрібно спочатку зберегти");
                        return;
                    }

                    if (!ConfDirectory.Fields.ContainsKey("Назва"))
                    {
                        string nameInTable_Name = Configuration.GetNewUnigueColumnName(Program.Kernel, entryTable.Text, ConfDirectory.Fields);
                        ConfDirectory.AppendField(new ConfigurationField("Назва", nameInTable_Name, "string", "", "Назва", true, true));
                    }

                    if (!ConfDirectory.Fields.ContainsKey("Родич"))
                    {
                        string nameInTable_Parent = Configuration.GetNewUnigueColumnName(Program.Kernel, entryTable.Text, ConfDirectory.Fields);
                        ConfDirectory.AppendField(new ConfigurationField("Родич", nameInTable_Parent, "pointer", "Довідники." + ConfDirectory.Name, "Родич", false, true));
                    }

                    FieldsRefreshList();
                    GeneralForm?.LoadTreeAsync();
                };
            }
            */

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

            //Генерування коду 
            /*{
                Expander expanderTemplates = new Expander("Генерування коду");
                vBox.PackStart(expanderTemplates, false, false, 5);

                Box vBoxTemplates = new Box(Orientation.Vertical, 0);
                expanderTemplates.Add(vBoxTemplates);

                //Заголовок для списку
                Box hBoxElementInfo = new Box(Orientation.Horizontal, 0) { Halign = Align.Start };
                vBoxTemplates.PackStart(hBoxElementInfo, false, false, 5);
                hBoxElementInfo.PackStart(new Label("Для списку") { UseMarkup = true, Selectable = true }, false, false, 5);

                //Елемент
                Box hBoxElement = new Box(Orientation.Horizontal, 0) { Halign = Align.Start };
                vBoxTemplates.PackStart(hBoxElement, false, false, 5);
                {
                    Button buttonConstructorElement = new Button("Елемент");
                    hBoxElement.PackStart(buttonConstructorElement, false, false, 5);
                    buttonConstructorElement.Clicked += (object? sender, EventArgs args) => { GenerateCode((Widget)sender!, "Element", true, true); };

                    Button buttonConstructorList = new Button("Список");
                    hBoxElement.PackStart(buttonConstructorList, false, false, 5);
                    buttonConstructorList.Clicked += (object? sender, EventArgs args) => { GenerateCode((Widget)sender!, "List", false, true); };

                    Button buttonConstructorListAndTree = new Button("Список з деревом");
                    hBoxElement.PackStart(buttonConstructorListAndTree, false, false, 5);
                    buttonConstructorListAndTree.Clicked += (object? sender, EventArgs args) => { GenerateCode((Widget)sender!, "ListAndTree", false, true); };

                    Button buttonConstructorListSmallSelect = new Button("Швидкий вибір");
                    hBoxElement.PackStart(buttonConstructorListSmallSelect, false, false, 5);
                    buttonConstructorListSmallSelect.Clicked += (object? sender, EventArgs args) => { GenerateCode((Widget)sender!, "ListSmallSelect"); };

                    Button buttonConstructorPointerControl = new Button("PointerControl");
                    hBoxElement.PackStart(buttonConstructorPointerControl, false, false, 5);
                    buttonConstructorPointerControl.Clicked += (object? sender, EventArgs args) => { GenerateCode((Widget)sender!, "PointerControl"); };
                }

                //Заголовок для дерева
                Box hBoxTreeInfo = new Box(Orientation.Horizontal, 0) { Halign = Align.Start };
                vBoxTemplates.PackStart(hBoxTreeInfo, false, false, 5);
                hBoxTreeInfo.PackStart(new Label("Для дерева") { UseMarkup = true, Selectable = true }, false, false, 5);

                //Дерево
                Box hBoxTree = new Box(Orientation.Horizontal, 0) { Halign = Align.Start };
                vBoxTemplates.PackStart(hBoxTree, false, false, 5);
                {
                    Button buttonConstructorElement = new Button("Елемент");
                    hBoxTree.PackStart(buttonConstructorElement, false, false, 5);
                    buttonConstructorElement.Clicked += (object? sender, EventArgs args) => { GenerateCode((Widget)sender!, "ElementTree", true, true); };

                    Button buttonConstructorTree = new Button("Дерево");
                    hBoxTree.PackStart(buttonConstructorTree, false, false, 5);
                    buttonConstructorTree.Clicked += (object? sender, EventArgs args) => { GenerateCode((Widget)sender!, "Tree"); };

                    Button buttonConstructorTreeSmallSelect = new Button("Швидкий вибір");
                    hBoxTree.PackStart(buttonConstructorTreeSmallSelect, false, false, 5);
                    buttonConstructorTreeSmallSelect.Clicked += (object? sender, EventArgs args) => { GenerateCode((Widget)sender!, "TreeSmallSelect"); };

                    Button buttonConstructorPointerControl = new Button("PointerControl");
                    hBoxTree.PackStart(buttonConstructorPointerControl, false, false, 5);
                    buttonConstructorPointerControl.Clicked += (object? sender, EventArgs args) => { GenerateCode((Widget)sender!, "PointerControlTree"); };
                }
            }*/

            hPaned.Pack1(vBox, false, false);
        }

        void CreatePack2(Paned hPaned)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

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
            scrollList.SetSizeRequest(0, 100);

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
            entryName.Text = ConfDirectory.Name;
            entryFullName.Text = ConfDirectory.FullName;

            if (IsNew)
            {
                entryTable.Text = await Configuration.GetNewUnigueTableName(Program.Kernel);

                //Заповнення полями
                string nameInTable_Code = Configuration.GetNewUnigueColumnName(Program.Kernel, entryTable.Text, ConfDirectory.Fields);
                ConfDirectory.AppendField(new ConfigurationField("Код", nameInTable_Code, "string", "", "Код", false, true));

                string nameInTable_Name = Configuration.GetNewUnigueColumnName(Program.Kernel, entryTable.Text, ConfDirectory.Fields);
                ConfDirectory.AppendField(new ConfigurationField("Назва", nameInTable_Name, "string", "", "Назва", true, true));

                //Заповнення списків
                ConfDirectory.AppendTableList(new ConfigurationTabularList("Записи"));
                ConfDirectory.AppendTableList(new ConfigurationTabularList("ЗаписиШвидкийВибір"));

                int sortNum = 0;
                bool sortField;

                //Заповнення полями списків
                foreach (var item in ConfDirectory.Fields)
                {
                    ++sortNum;
                    sortField = item.Value.Name == "Назва";

                    ConfDirectory.TabularList["Записи"].AppendField(new ConfigurationTabularListField(item.Value.Name, item.Value.Name, 0, sortNum, sortField));
                    ConfDirectory.TabularList["ЗаписиШвидкийВибір"].AppendField(new ConfigurationTabularListField(item.Value.Name, item.Value.Name, 0, sortNum, sortField));
                }
            }
            else
                entryTable.Text = ConfDirectory.Table;

            textViewDesc.Buffer.Text = ConfDirectory.Desc;

            #region Trigers

            entryNew.Text = ConfDirectory.TriggerFunctions.New;
            entryCopying.Text = ConfDirectory.TriggerFunctions.Copying;
            entryBeforeSave.Text = ConfDirectory.TriggerFunctions.BeforeSave;
            entryAfterSave.Text = ConfDirectory.TriggerFunctions.AfterSave;
            entrySetDeletionLabel.Text = ConfDirectory.TriggerFunctions.SetDeletionLabel;
            entryBeforeDelete.Text = ConfDirectory.TriggerFunctions.BeforeDelete;

            switchNew.Active = ConfDirectory.TriggerFunctions.NewAction;
            switchCopying.Active = ConfDirectory.TriggerFunctions.CopyingAction;
            switchBeforeSave.Active = ConfDirectory.TriggerFunctions.BeforeSaveAction;
            switchAfterSave.Active = ConfDirectory.TriggerFunctions.AfterSaveAction;
            switchSetDeletionLabel.Active = ConfDirectory.TriggerFunctions.SetDeletionLabelAction;
            switchBeforeDelete.Active = ConfDirectory.TriggerFunctions.BeforeDeleteAction;

            #endregion

            checkButtonAutoNum.Active = ConfDirectory.AutomaticNumeration;
            comboBoxTypeDir.ActiveId = ConfDirectory.TypeDirectory.ToString();

            FillPointerFolders();
            comboBoxPointerFolders.ActiveId = ConfDirectory.PointerFolders_HierarchyInAnotherDirectory;

            FillFields();
            comboBoxParentField.ActiveId = ConfDirectory.ParentField_Hierarchical;

            FillTabularParts();
            FillTabularList();
            FillFormsList();
        }

        void FillFields()
        {
            foreach (ConfigurationField field in ConfDirectory.Fields.Values)
            {
                listBoxFields.Add(new Label(field.Name + (field.IsPresentation ? " [ представлення ]" : "")) { Name = field.Name, Halign = Align.Start });

                //Поля для ієрархії
                if (field.Type == "pointer" && field.Pointer == $"Довідники.{ConfDirectory.Name}")
                    comboBoxParentField.Append(field.Name, field.Name);
            }

            comboBoxParentField.ActiveId = ConfDirectory.ParentField_Hierarchical;

            listBoxFields.ShowAll();
        }

        void FillTabularParts()
        {
            foreach (ConfigurationTablePart tablePart in ConfDirectory.TabularParts.Values)
                listBoxTableParts.Add(new Label(tablePart.Name) { Name = tablePart.Name, Halign = Align.Start });

            listBoxTableParts.ShowAll();
        }

        void FillTabularList()
        {
            foreach (ConfigurationTabularList tableList in ConfDirectory.TabularList.Values)
                listBoxTabularList.Add(new Label(tableList.Name) { Name = tableList.Name, Halign = Align.Start });

            listBoxTabularList.ShowAll();
        }

        void FillFormsList()
        {
            foreach (ConfigurationForms form in ConfDirectory.Forms.Values)
                listBoxFormsList.Add(new Label(form.Name) { Name = form.Name, Halign = Align.Start });

            listBoxFormsList.ShowAll();
        }

        void FillPointerFolders()
        {
            comboBoxPointerFolders.RemoveAll();

            foreach (ConfigurationDirectories item in Conf.Directories.Values)
                comboBoxPointerFolders.Append($"Довідники.{item.Name}", $"Довідники.{item.Name}");

            comboBoxPointerFolders.ShowAll();
        }

        void GetValue()
        {
            //Поле з повною назвою переноситься із назви
            if (string.IsNullOrEmpty(entryFullName.Text))
                entryFullName.Text = entryName.Text;

            ConfDirectory.Name = entryName.Text;
            ConfDirectory.FullName = entryFullName.Text;
            ConfDirectory.Table = entryTable.Text;
            ConfDirectory.Desc = textViewDesc.Buffer.Text;

            #region Trigers

            ConfDirectory.TriggerFunctions.New = entryNew.Text;
            ConfDirectory.TriggerFunctions.Copying = entryCopying.Text;
            ConfDirectory.TriggerFunctions.BeforeSave = entryBeforeSave.Text;
            ConfDirectory.TriggerFunctions.AfterSave = entryAfterSave.Text;
            ConfDirectory.TriggerFunctions.SetDeletionLabel = entrySetDeletionLabel.Text;
            ConfDirectory.TriggerFunctions.BeforeDelete = entryBeforeDelete.Text;

            ConfDirectory.TriggerFunctions.NewAction = switchNew.Active;
            ConfDirectory.TriggerFunctions.CopyingAction = switchCopying.Active;
            ConfDirectory.TriggerFunctions.BeforeSaveAction = switchBeforeSave.Active;
            ConfDirectory.TriggerFunctions.AfterSaveAction = switchAfterSave.Active;
            ConfDirectory.TriggerFunctions.SetDeletionLabelAction = switchSetDeletionLabel.Active;
            ConfDirectory.TriggerFunctions.BeforeDeleteAction = switchBeforeDelete.Active;

            #endregion

            ConfDirectory.AutomaticNumeration = checkButtonAutoNum.Active;

            ConfDirectory.TypeDirectory = Enum.Parse<ConfigurationDirectories.TypeDirectories>(comboBoxTypeDir.ActiveId);
            ConfDirectory.PointerFolders_HierarchyInAnotherDirectory = (ConfDirectory.TypeDirectory == ConfigurationDirectories.TypeDirectories.HierarchyInAnotherDirectory &&
                comboBoxPointerFolders.Active != -1) ? comboBoxPointerFolders.ActiveId : "";
            ConfDirectory.ParentField_Hierarchical = (ConfDirectory.TypeDirectory == ConfigurationDirectories.TypeDirectories.Hierarchical &&
                comboBoxParentField.Active != -1) ? comboBoxParentField.ActiveId : "";
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
                if (Conf.Directories.ContainsKey(entryName.Text))
                {
                    Message.Error(GeneralForm, $"Назва довідника не унікальна");
                    return;
                }
            }
            else
            {
                if (ConfDirectory.Name != entryName.Text)
                {
                    if (Conf.Directories.ContainsKey(entryName.Text))
                    {
                        Message.Error(GeneralForm, $"Назва довідника не унікальна");
                        return;
                    }
                }

                Conf.Directories.Remove(ConfDirectory.Name);
            }

            GetValue();

            if (ConfDirectory.TypeDirectory == ConfigurationDirectories.TypeDirectories.Hierarchical && string.IsNullOrEmpty(ConfDirectory.ParentField_Hierarchical))
                Message.Error(GeneralForm, $"Потрібно вказати поле Родич для ієрархічного довідника");

            Conf.AppendDirectory(ConfDirectory);

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
                        ConfigurationField newField = ConfDirectory.Fields[row.Child.Name].Copy();
                        newField.Name += GenerateName.GetNewName();
                        newField.NameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, ConfDirectory.Table, ConfDirectory.Fields);

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

            //Поля для ієрархії
            comboBoxParentField.RemoveAll();

            FillFields();
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

        async void OnTabularPartsCopyClick(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBoxTableParts.SelectedRows;

            if (selectedRows.Length != 0)
            {
                foreach (ListBoxRow row in selectedRows)
                {
                    if (ConfDirectory.TabularParts.ContainsKey(row.Child.Name))
                    {
                        ConfigurationTablePart newTablePart = ConfDirectory.TabularParts[row.Child.Name].Copy();
                        newTablePart.Name += GenerateName.GetNewName();
                        newTablePart.Table = await Configuration.GetNewUnigueTableName(Program.Kernel);

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

        #region FormsList

        void OnFormsListButtonPress(object? sender, ButtonPressEventArgs args)
        {
            if (args.Event.Type == Gdk.EventType.DoubleButtonPress)
            {
                ListBoxRow[] selectedRows = listBoxFormsList.SelectedRows;

                if (selectedRows.Length != 0)
                {
                    ListBoxRow curRow = selectedRows[0];

                    if (ConfDirectory.Forms.TryGetValue(curRow.Child.Name, out ConfigurationForms? form))
                        GeneralForm?.CreateNotebookPage($"Форма: {curRow.Child.Name}", () =>
                        {
                            PageForm page = new PageForm()
                            {
                                ParentName = ConfDirectory.Name,
                                ParentType = "Directory",
                                Forms = ConfDirectory.Forms,
                                Form = ConfDirectory.Forms[curRow.Child.Name],
                                TypeForm = ConfDirectory.Forms[curRow.Child.Name].Type,
                                Fields = ConfDirectory.Fields,
                                TabularParts = ConfDirectory.TabularParts,
                                TabularLists = ConfDirectory.TabularList,
                                TabularList = form.TabularList,
                                IsNew = false,
                                GeneralForm = GeneralForm,
                                CallBack_RefreshList = FormsListRefreshList
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
                    Message.Error(GeneralForm, "Назва довідника не вказана");
                    return;
                }

                GeneralForm?.CreateNotebookPage("Форма *", () =>
                {
                    PageForm page = new PageForm()
                    {
                        ParentName = ConfDirectory.Name,
                        ParentType = "Directory",
                        Forms = ConfDirectory.Forms,
                        TypeForm = typeForms,
                        Fields = ConfDirectory.Fields,
                        TabularParts = ConfDirectory.TabularParts,
                        TabularLists = ConfDirectory.TabularList,
                        IsNew = true,
                        GeneralForm = GeneralForm,
                        CallBack_RefreshList = FormsListRefreshList,
                    };

                    page.SetValue();

                    return page;
                });
            }

            Menu Menu = new Menu();

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
                MenuItem item = new MenuItem("Швидкий вибір");
                item.Activated += (object? sender, EventArgs args) => { OnFormsListAdd(ConfigurationForms.TypeForms.ListSmallSelect); };
                Menu.Append(item);
            }

            {
                MenuItem item = new MenuItem("PointerControl");
                item.Activated += (object? sender, EventArgs args) => { OnFormsListAdd(ConfigurationForms.TypeForms.PointerControl); };
                Menu.Append(item);
            }

            {
                MenuItem item = new MenuItem("Список з деревом");
                item.Activated += (object? sender, EventArgs args) => { OnFormsListAdd(ConfigurationForms.TypeForms.ListAndTree); };
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
                {
                    if (ConfDirectory.Forms.ContainsKey(row.Child.Name))
                    {
                        ConfigurationForms newForms = ConfDirectory.Forms[row.Child.Name].Copy();
                        newForms.Name += GenerateName.GetNewName();

                        ConfDirectory.AppendForms(newForms);
                    }
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
                    if (ConfDirectory.Forms.ContainsKey(row.Child.Name))
                        ConfDirectory.Forms.Remove(row.Child.Name);

                FormsListRefreshList();
            }
        }

        void FormsListRefreshList()
        {
            OnFormsListRefreshClick(null, new EventArgs());
        }

        #endregion

        /*#region Генерування коду

        void GenerateCode(Widget relative_to, string fileName, bool includeFields = false, bool includeTabularParts = false)
        {
            if (string.IsNullOrEmpty(entryName.Text))
            {
                Message.Error(GeneralForm, "Назва довідника не вказана");
                return;
            }

            if (!Conf.Directories.ContainsKey(entryName.Text))
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
                System.IO.Path.Combine(AppContext.BaseDirectory, "xslt/ConstructorDirectory.xslt"),
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

        #endregion*/
    }
}