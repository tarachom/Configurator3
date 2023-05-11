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
    class PageField : VBox
    {
        Configuration? Conf
        {
            get
            {
                return Program.Kernel?.Conf;
            }
        }

        public string Table { get; set; } = "";
        public Dictionary<string, ConfigurationObjectField> Fields = new Dictionary<string, ConfigurationObjectField>();
        public Dictionary<string, ConfigurationObjectField>? AllFields; //Для регістрів
        public ConfigurationObjectField Field { get; set; } = new ConfigurationObjectField();
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
        CheckButton checkButtonMultiline = new CheckButton("Багатострічкове поле (тільки для типу string)");

        #endregion

        public PageField() : base()
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

            HPaned hPaned = new HPaned() { BorderWidth = 5, Position = 500 };

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

            //Поле
            HBox hBoxColumn = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxColumn, false, false, 5);

            hBoxColumn.PackStart(new Label("В таблиці:"), false, false, 5);
            hBoxColumn.PackStart(entryColumn, false, false, 5);

            //Тип
            HBox hBoxType = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxType, false, false, 5);

            foreach (var fieldType in FieldType.DefaultList())
                comboBoxType.Append(fieldType.ConfTypeName, fieldType.ViewTypeName);

            comboBoxType.Changed += OnComboBoxTypeChanged;

            hBoxType.PackStart(new Label("Тип:"), false, false, 5);
            hBoxType.PackStart(comboBoxType, false, false, 5);

            //Pointer
            HBox hBoxPointer = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxPointer, false, false, 5);

            foreach (ConfigurationDirectories item in Conf!.Directories.Values)
                comboBoxPointer.Append($"Довідники.{item.Name}", $"Довідники.{item.Name}");

            foreach (ConfigurationDocuments item in Conf!.Documents.Values)
                comboBoxPointer.Append($"Документи.{item.Name}", $"Документи.{item.Name}");

            hBoxPointer.PackStart(new Label("Вказівник:"), false, false, 5);
            hBoxPointer.PackStart(comboBoxPointer, false, false, 5);

            //Enum
            HBox hBoxEnum = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxEnum, false, false, 5);

            foreach (ConfigurationEnums item in Conf!.Enums.Values)
                comboBoxEnum.Append($"Перелічення.{item.Name}", $"Перелічення.{item.Name}");

            hBoxEnum.PackStart(new Label("Перелічення:"), false, false, 5);
            hBoxEnum.PackStart(comboBoxEnum, false, false, 5);

            //Опис
            HBox hBoxDesc = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxDesc, false, false, 5);

            hBoxDesc.PackStart(new Label("Опис:") { Valign = Align.Start }, false, false, 5);

            ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 500, HeightRequest = 100 };
            scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollTextView.Add(textViewDesc);

            hBoxDesc.PackStart(scrollTextView, false, false, 5);

            //Індексувати
            vBox.PackStart(checkButtonIndex, false, false, 5);

            //Індексувати
            vBox.PackStart(checkButtonPresentation, false, false, 5);

            //Повнотекстовий пошук
            vBox.PackStart(checkButtonFullTextSearch, false, false, 5);

            //Багатострічкове поле
            vBox.PackStart(checkButtonMultiline, false, false, 5);

            hPaned.Pack1(vBox, false, false);
        }

        void CreatePack2(HPaned hPaned)
        {
            VBox vBox = new VBox();

            Expander expanderHelp = new Expander("Довідка");
            expanderHelp.Add(vBox);

            List<string> helpList = new List<string>();
            helpList.Add("Типи даних та їх відповідники в C#:");
            helpList.Add("integer -> int");
            helpList.Add("numeric -> decimal");
            helpList.Add("boolean -> bool");
            helpList.Add("date -> DateTime");
            helpList.Add("datetime -> DateTime");
            helpList.Add("time -> TimeSpan");
            helpList.Add("enum -> enum");
            helpList.Add("pointer -> Довідник.Номенклатура()");
            helpList.Add("composite-pointer -> UuidAndText()");
            helpList.Add("any-pointer -> Guid()");
            helpList.Add("byte -> byte[]");
            helpList.Add("string[] -> string[]");
            helpList.Add("integer[] -> int[]");
            helpList.Add("numeric[] -> decimal[]");

            foreach (var item in helpList)
            {
                HBox hBox = new HBox() { Halign = Align.Fill };
                vBox.PackStart(hBox, false, false, 5);
                hBox.PackStart(new Label(item), false, false, 5);
            }

            hPaned.Pack2(expanderHelp, false, false);
        }

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            entryName.Text = Field.Name;

            if (IsNew)
                entryColumn.Text = Configuration.GetNewUnigueColumnName(Program.Kernel!, Table, AllFields ?? Fields);
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
            checkButtonMultiline.Active = Field.Multiline;

            OnComboBoxTypeChanged(comboBoxType, new EventArgs());
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
            Field.Multiline = checkButtonMultiline.Active;

            Field.NameInTable = entryColumn.Text;
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
            string errorList = Configuration.ValidateConfigurationObjectName(Program.Kernel!, ref name);
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

            if (Field.Type == "pointer" || Field.Type == "enum")
                if (String.IsNullOrEmpty(Field.Pointer))
                {
                    Message.Error(GeneralForm, $"Потрібно деталізувати тип для [ pointer ] або [ enum ]\nВиберіть із списку тип для деталізації");
                    return;
                }

            Fields.Add(Field.Name, Field);

            IsNew = false;

            GeneralForm?.LoadTreeAsync();
            GeneralForm?.RenameCurrentPageNotebook($"Поле: {Field.Name}");

            if (CallBack_RefreshList != null)
                CallBack_RefreshList.Invoke();
        }
    }
}