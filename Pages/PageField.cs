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
    class PageField : Box
    {
        Configuration Conf { get { return Program.Kernel.Conf; } }

        public string Table { get; set; } = "";
        public Dictionary<string, ConfigurationField> Fields = [];
        public Dictionary<string, ConfigurationField>? AllFields; //Для регістрів
        public ConfigurationField Field { get; set; } = new ConfigurationField();
        public FormConfigurator? GeneralForm { get; set; }
        public System.Action? CallBack_RefreshList { get; set; }
        public bool IsNew { get; set; } = true;

        #region Fields

        Entry entryName = new Entry() { WidthRequest = 500 };
        Entry entryColumn = new Entry() { WidthRequest = 500 };
        TextView textViewDesc = new TextView() { WrapMode = WrapMode.Word };
        ComboBoxText comboBoxType = new ComboBoxText();
        ComboBoxText comboBoxPointer = new ComboBoxText();
        ComboBoxText comboBoxEnum = new ComboBoxText();
        CheckButton checkButtonIndex = new CheckButton("Індексувати");
        CheckButton checkButtonPresentation = new CheckButton("Використовувати для представлення");
        CheckButton checkButtonFullTextSearch = new CheckButton("Повнотекстовий пошук");
        CheckButton checkButtonSearch = new CheckButton("Пошук у списку");
        CheckButton checkButtonMultiline = new CheckButton("Багатострічкове поле");

        //Для композитного типу даних
        CheckButton checkButtonCompositePointerNotUseDirectories = new CheckButton("Не викоритовувати");
        CheckButton checkButtonCompositePointerNotUseDocuments = new CheckButton("Не викоритовувати");
        ListBox listBoxCompositePointerAllowDirectories = new ListBox() { SelectionMode = SelectionMode.Single };
        ListBox listBoxCompositePointerAllowDocuments = new ListBox() { SelectionMode = SelectionMode.Single };

        #endregion

        public PageField() : base(Orientation.Vertical, 0)
        {
            Box hBox = new Box(Orientation.Horizontal, 0);

            Button bSave = new Button("Зберегти");
            bSave.Clicked += OnSaveClick;

            hBox.PackStart(bSave, false, false, 10);

            Button bClose = new Button("Закрити");
            bClose.Clicked += (object? sender, EventArgs args) => { GeneralForm?.CloseCurrentPageNotebook(); };

            hBox.PackStart(bClose, false, false, 10);

            PackStart(hBox, false, false, 10);

            Paned hPaned = new Paned(Orientation.Horizontal) { BorderWidth = 5, Position = 500 };

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

            //Поле
            Box hBoxColumn = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxColumn, false, false, 5);

            hBoxColumn.PackStart(new Label("В таблиці:"), false, false, 5);
            hBoxColumn.PackStart(entryColumn, false, false, 5);

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

        void CreatePack2(Paned hPaned)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            //Індексувати
            vBox.PackStart(checkButtonIndex, false, false, 5);

            //Індексувати
            vBox.PackStart(checkButtonPresentation, false, false, 5);

            //Повнотекстовий пошук
            vBox.PackStart(checkButtonFullTextSearch, false, false, 5);

            //Пошук у списку
            vBox.PackStart(checkButtonSearch, false, false, 5);

            //Багатострічкове поле
            vBox.PackStart(checkButtonMultiline, false, false, 5);

            //Налаштування для композитного типу даних
            {
                Expander expanderCompositePointerAllow = new Expander("Налаштування для композитного типу даних");
                vBox.PackStart(expanderCompositePointerAllow, false, false, 5);

                Box vBoxCompositePointerAllow = new Box(Orientation.Vertical, 0);
                expanderCompositePointerAllow.Add(vBoxCompositePointerAllow);

                //Заголовок
                Box hBoxCompositePointerAllowInfo = new Box(Orientation.Horizontal, 5) { Halign = Align.Start };
                vBoxCompositePointerAllow.PackStart(hBoxCompositePointerAllowInfo, false, false, 10);
                hBoxCompositePointerAllowInfo.PackStart(new Label("<b>Довідники або документи доступні для вибору</b>") { UseMarkup = true }, false, false, 5);

                Box hBoxCompositePointerListBoxes = new Box(Orientation.Horizontal, 0);
                vBoxCompositePointerAllow.PackStart(hBoxCompositePointerListBoxes, false, false, 5);

                //Довідники
                {
                    Box vBoxListBox = new Box(Orientation.Vertical, 0);
                    hBoxCompositePointerListBoxes.PackStart(vBoxListBox, false, false, 5);

                    vBoxListBox.PackStart(new Label("Довідники"), false, false, 2);
                    vBoxListBox.PackStart(checkButtonCompositePointerNotUseDirectories, false, false, 5);

                    ScrolledWindow scrollList = new ScrolledWindow() { WidthRequest = 250, HeightRequest = 400, ShadowType = ShadowType.In };
                    scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
                    scrollList.Add(listBoxCompositePointerAllowDirectories);

                    vBoxListBox.PackStart(scrollList, false, false, 5);
                }

                //Документи
                {
                    Box vBoxListBox = new Box(Orientation.Vertical, 0);
                    hBoxCompositePointerListBoxes.PackStart(vBoxListBox, false, false, 5);

                    vBoxListBox.PackStart(new Label("Документи"), false, false, 2);
                    vBoxListBox.PackStart(checkButtonCompositePointerNotUseDocuments, false, false, 5);

                    ScrolledWindow scrollList = new ScrolledWindow() { WidthRequest = 250, HeightRequest = 400, ShadowType = ShadowType.In };
                    scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
                    scrollList.Add(listBoxCompositePointerAllowDocuments);

                    vBoxListBox.PackStart(scrollList, false, false, 5);
                }
            }

            //Довідка
            {
                Box vBoxExpander = new Box(Orientation.Vertical, 0);
                Expander expanderHelp = new Expander("Довідка") { vBoxExpander };

                List<string> helpList =
                [
                    "Типи даних та їх відповідники в C#:",
                "integer -> int",
                "numeric -> decimal",
                "boolean -> bool",
                "date -> DateTime",
                "datetime -> DateTime",
                "time -> TimeSpan",
                "enum -> enum",
                "pointer -> Довідник.Номенклатура()",
                "composite_pointer -> UuidAndText()",
                "any_pointer -> Guid()",
                "byte -> byte[]",
                "string[] -> string[]",
                "integer[] -> int[]",
                "numeric[] -> decimal[]",
            ];

                foreach (var item in helpList)
                {
                    Box hBox = new Box(Orientation.Horizontal, 0);
                    vBoxExpander.PackStart(hBox, false, false, 5);
                    hBox.PackStart(new Label(item) { UseUnderline = false }, false, false, 5);
                }

                vBox.PackStart(expanderHelp, false, false, 5);
            }

            hPaned.Pack2(vBox, false, false);
        }

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            entryName.Text = Field.Name;

            if (IsNew)
                entryColumn.Text = Configuration.GetNewUnigueColumnName(Program.Kernel, Table, AllFields ?? Fields);
            else
                entryColumn.Text = Field.NameInTable;

            textViewDesc.Buffer.Text = Field.Desc;

            comboBoxType.ActiveId = Field.Type;

            if (comboBoxType.Active == -1)
                comboBoxType.Active = 0;

            if (comboBoxType.ActiveId == "pointer")
                comboBoxPointer.ActiveId = Field.Pointer;

            if (comboBoxType.ActiveId == "enum")
                comboBoxEnum.ActiveId = Field.Pointer;

            checkButtonIndex.Active = Field.IsIndex;
            checkButtonPresentation.Active = Field.IsPresentation;
            checkButtonFullTextSearch.Active = Field.IsFullTextSearch;
            checkButtonSearch.Active = Field.IsSearch;
            checkButtonMultiline.Active = Field.Multiline;

            OnComboBoxTypeChanged(comboBoxType, new EventArgs());

            checkButtonCompositePointerNotUseDirectories.Active = Field.CompositePointerNotUseDirectories;
            checkButtonCompositePointerNotUseDocuments.Active = Field.CompositePointerNotUseDocuments;

            FillCompositePointerAllowDirectories();
            FillCompositePointerAllowDocuments();
        }

        void FillCompositePointerAllowDirectories()
        {
            foreach (KeyValuePair<string, ConfigurationDirectories> directories in Conf.Directories)
                listBoxCompositePointerAllowDirectories.Add(
                    new CheckButton(directories.Key)
                    {
                        Name = directories.Key,
                        Active = Field.CompositePointerAllowDirectories.Contains(directories.Key)
                    });

            listBoxCompositePointerAllowDirectories.ShowAll();
        }

        void FillCompositePointerAllowDocuments()
        {
            foreach (KeyValuePair<string, ConfigurationDocuments> documents in Conf.Documents)
                listBoxCompositePointerAllowDocuments.Add(
                    new CheckButton(documents.Key)
                    {
                        Name = documents.Key,
                        Active = Field.CompositePointerAllowDocuments.Contains(documents.Key)
                    });

            listBoxCompositePointerAllowDocuments.ShowAll();
        }

        void GetValue()
        {
            Field.Name = entryName.Text;
            Field.Desc = textViewDesc.Buffer.Text;
            Field.Type = comboBoxType.ActiveId;

            if (Field.Type == "pointer")
                Field.Pointer = comboBoxPointer.ActiveId;

            if (Field.Type == "enum")
                Field.Pointer = comboBoxEnum.ActiveId;

            Field.IsIndex = checkButtonIndex.Active;
            Field.IsPresentation = checkButtonPresentation.Active;
            Field.IsFullTextSearch = checkButtonFullTextSearch.Active;
            Field.IsSearch = checkButtonSearch.Active;
            Field.Multiline = checkButtonMultiline.Active;

            Field.NameInTable = entryColumn.Text;

            //Для композитного типу даних
            Field.CompositePointerAllowDirectories.Clear();
            Field.CompositePointerAllowDocuments.Clear();

            if (Field.Type == "composite_pointer")
            {
                Field.CompositePointerNotUseDirectories = checkButtonCompositePointerNotUseDirectories.Active;
                Field.CompositePointerNotUseDocuments = checkButtonCompositePointerNotUseDocuments.Active;

                foreach (ListBoxRow item in listBoxCompositePointerAllowDirectories.Children)
                {
                    CheckButton cb = (CheckButton)item.Child;
                    if (cb.Active)
                        Field.CompositePointerAllowDirectories.Add(cb.Name);
                }

                foreach (ListBoxRow item in listBoxCompositePointerAllowDocuments.Children)
                {
                    CheckButton cb = (CheckButton)item.Child;
                    if (cb.Active)
                        Field.CompositePointerAllowDocuments.Add(cb.Name);
                }
            }
        }

        #endregion

        void OnComboBoxTypeChanged(object? sender, EventArgs args)
        {
            string typeName = comboBoxType.ActiveId;

            comboBoxPointer.Parent.Sensitive = typeName == "pointer";
            comboBoxEnum.Parent.Sensitive = typeName == "enum";

            checkButtonPresentation.Sensitive =
                typeName == "string" ||
                typeName == "integer" ||
                typeName == "numeric" ||
                typeName == "boolean" ||
                typeName == "date" ||
                typeName == "datetime" ||
                typeName == "time";

            //string only
            {
                bool isString = typeName == "string";

                checkButtonFullTextSearch.Sensitive = isString;
                checkButtonMultiline.Sensitive = isString;
            }

            //composite_pointer
            {
                bool isCompositePointer = typeName == "composite_pointer";

                checkButtonCompositePointerNotUseDirectories.Sensitive = isCompositePointer;
                checkButtonCompositePointerNotUseDocuments.Sensitive = isCompositePointer;
                listBoxCompositePointerAllowDirectories.Sensitive = isCompositePointer;
                listBoxCompositePointerAllowDocuments.Sensitive = isCompositePointer;
            }
        }

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
                if (AllFields != null)
                {
                    if (AllFields.ContainsKey(entryName.Text))
                    {
                        Message.Error(GeneralForm, $"Назва поля не унікальна");
                        return;
                    }
                }
                else if (Fields.ContainsKey(entryName.Text))
                {
                    Message.Error(GeneralForm, $"Назва поля не унікальна");
                    return;
                }
            }
            else
            {
                if (Field.Name != entryName.Text)
                {
                    if (AllFields != null)
                    {
                        if (AllFields.ContainsKey(entryName.Text))
                        {
                            Message.Error(GeneralForm, $"Назва поля не унікальна");
                            return;
                        }
                    }
                    else if (Fields.ContainsKey(entryName.Text))
                    {
                        Message.Error(GeneralForm, $"Назва поля не унікальна");
                        return;
                    }
                }

                Fields.Remove(Field.Name);
            }

            GetValue();

            if ((Field.Type == "pointer" || Field.Type == "enum") && string.IsNullOrEmpty(Field.Pointer))
                Message.Error(GeneralForm, $"Потрібно деталізувати тип для [ pointer ] або [ enum ]\nВиберіть із списку тип для деталізації");

            Fields.Add(Field.Name, Field);

            IsNew = false;

            GeneralForm?.LoadTreeAsync();
            GeneralForm?.RenameCurrentPageNotebook($"Поле: {Field.Name}");

            CallBack_RefreshList?.Invoke();
        }
    }
}