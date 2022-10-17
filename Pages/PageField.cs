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

        public Dictionary<string, ConfigurationObjectField> Fields = new Dictionary<string, ConfigurationObjectField>();
        public Dictionary<string, ConfigurationObjectField>? AllFields; //Для регістрів
        public ConfigurationObjectField Field { get; set; } = new ConfigurationObjectField();
        public FormConfigurator? GeneralForm { get; set; }
        public System.Action? CallBack_RefreshList { get; set; }
        public bool IsNew { get; set; } = true;

        Entry entryName = new Entry() { WidthRequest = 400 };
        TextView textViewDesc = new TextView();
        ComboBoxText comboBoxType = new ComboBoxText();
        ComboBoxText comboBoxPointer = new ComboBoxText();
        ComboBoxText comboBoxEnum = new ComboBoxText();
        CheckButton checkButtonIndex = new CheckButton("Індексувати");
        CheckButton checkButtonPresentation = new CheckButton("Використовувати для представлення");

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

            CreatePack();

            ShowAll();
        }

        void CreatePack()
        {
            VBox vBox = new VBox();

            //Назва
            HBox hBoxName = new HBox();
            vBox.PackStart(hBoxName, false, false, 5);

            hBoxName.PackStart(new Label("Назва:"), false, false, 5);
            hBoxName.PackStart(entryName, false, false, 5);

            //Тип
            HBox hBoxType = new HBox();
            vBox.PackStart(hBoxType, false, false, 5);

            foreach (var fieldType in FieldType.DefaultList())
                comboBoxType.Append(fieldType.ConfTypeName, fieldType.ViewTypeName);

            comboBoxType.Changed += OnComboBoxTypeChanged;

            hBoxType.PackStart(new Label("Тип:"), false, false, 5);
            hBoxType.PackStart(comboBoxType, false, false, 5);

            //Pointer
            HBox hBoxPointer = new HBox();
            vBox.PackStart(hBoxPointer, false, false, 5);

            foreach (ConfigurationDirectories item in Conf!.Directories.Values)
                comboBoxPointer.Append($"Довідники.{item.Name}", $"Довідники.{item.Name}");

            foreach (ConfigurationDocuments item in Conf!.Documents.Values)
                comboBoxPointer.Append($"Документи.{item.Name}", $"Документи.{item.Name}");

            hBoxPointer.PackStart(new Label("Вказівник:"), false, false, 5);
            hBoxPointer.PackStart(comboBoxPointer, false, false, 5);

            //Enum
            HBox hBoxEnum = new HBox();
            vBox.PackStart(hBoxEnum, false, false, 5);

            foreach (ConfigurationEnums item in Conf!.Enums.Values)
                comboBoxEnum.Append($"Перелічення.{item.Name}", $"Перелічення.{item.Name}");

            hBoxEnum.PackStart(new Label("Перелічення:"), false, false, 5);
            hBoxEnum.PackStart(comboBoxEnum, false, false, 5);

            //Опис
            HBox hBoxDesc = new HBox();
            vBox.PackStart(hBoxDesc, false, false, 5);

            hBoxDesc.PackStart(new Label("Опис:") { Valign = Align.Start }, false, false, 5);

            ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollTextView.SetSizeRequest(400, 100);
            scrollTextView.Add(textViewDesc);

            hBoxDesc.PackStart(scrollTextView, false, false, 5);

            //Індексувати
            vBox.PackStart(checkButtonIndex, false, false, 5);

            //Індексувати
            vBox.PackStart(checkButtonPresentation, false, false, 5);

            PackStart(vBox, false, false, 5);
        }

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            entryName.Text = Field.Name;
            textViewDesc.Buffer.Text = Field.Desc;

            comboBoxType.ActiveId = Field.Type;

            if (comboBoxType.ActiveId == "pointer")
                comboBoxPointer.ActiveId = Field.Pointer;

            if (comboBoxType.ActiveId == "enum")
                comboBoxEnum.ActiveId = Field.Pointer;

            checkButtonIndex.Active = Field.IsIndex;
            checkButtonPresentation.Active = Field.IsPresentation;

            OnComboBoxTypeChanged(comboBoxType, new EventArgs());
        }

        public void SetDefValue()
        {
            comboBoxType.Active = 0;

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
                Message.Error($"{errorList}");
                return;
            }

            if (IsNew)
            {
                if (AllFields != null)
                {
                    if (AllFields.ContainsKey(entryName.Text))
                    {
                        Message.Error($"Назва поля не унікальна");
                        return;
                    }
                }
                else if (Fields.ContainsKey(entryName.Text))
                {
                    Message.Error($"Назва поля не унікальна");
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
                            Message.Error($"Назва поля не унікальна");
                            return;
                        }
                    }
                    else if (Fields.ContainsKey(entryName.Text))
                    {
                        Message.Error($"Назва поля не унікальна");
                        return;
                    }
                }

                Fields.Remove(Field.Name);
            }

            GetValue();

            if (Field.Type == "pointer" || Field.Type == "enum")
                if (String.IsNullOrEmpty(Field.Pointer))
                {
                    Message.Error($"Потрібно деталізувати тип для [ pointer ] або [ enum ]\nВиберіть із списку тип для деталізації");
                    return;
                }

            Fields.Add(Field.Name, Field);

            IsNew = false;

            GeneralForm?.LoadTree();
            GeneralForm?.RenameCurrentPageNotebook($"Поле: {Field.Name}");

            if (CallBack_RefreshList != null)
                CallBack_RefreshList.Invoke();
        }
    }
}