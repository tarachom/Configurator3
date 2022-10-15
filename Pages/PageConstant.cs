using Gtk;

using AccountingSoftware;

namespace Configurator
{
    class PageConstant : VBox
    {
        Configuration? Conf
        {
            get
            {
                return Program.Kernel?.Conf;
            }
        }

        public ConfigurationConstants ConfConstants { get; set; } = new ConfigurationConstants();
        public System.Action? CallBack_ClosePage { get; set; }
        public System.Action<string>? CallBack_RenamePage { get; set; }
        public System.Action? CallBack_RelodTree { get; set; }
        public bool IsNew { get; set; } = true;

        ListBox listBoxTableParts = new ListBox() { SelectionMode = SelectionMode.Single };
        Entry entryName = new Entry() { WidthRequest = 400 };
        TextView textViewDesc = new TextView();
        ComboBoxText comboBoxBlock = new ComboBoxText();
        ComboBoxText comboBoxType = new ComboBoxText();
        ComboBoxText comboBoxPointer = new ComboBoxText();
        ComboBoxText comboBoxEnum = new ComboBoxText();

        public PageConstant() : base()
        {
            new VBox();
            HBox hBox = new HBox();

            Button bSave = new Button("Зберегти");
            bSave.Clicked += OnSaveClick;

            hBox.PackStart(bSave, false, false, 10);

            Button bClose = new Button("Закрити");
            bClose.Clicked += (object? sender, EventArgs args) => { if (CallBack_ClosePage != null) CallBack_ClosePage.Invoke(); };

            hBox.PackStart(bClose, false, false, 10);

            PackStart(hBox, false, false, 10);

            HPaned hPaned = new HPaned() { BorderWidth = 5 };

            CreatePack1(hPaned);
            CreatePack2(hPaned);

            PackStart(hPaned, false, false, 5);

            ShowAll();
        }

        void CreatePack2(HPaned hPaned)
        {
            VBox vBox = new VBox();

            HBox hBox = new HBox();
            hBox.PackStart(new Label("Табличні частини:") { Halign = Align.Start }, false, false, 5);
            vBox.PackStart(hBox, false, false, 5);

            Toolbar toolbar = new Toolbar();
            vBox.PackStart(toolbar, false, false, 0);

            ToolButton buttonAdd = new ToolButton(Stock.Add) { Label = "Додати", IsImportant = true };
            //refreshButton.Clicked += OnRefreshClick;
            toolbar.Add(buttonAdd);

            ToolButton buttonRefresh = new ToolButton(Stock.Refresh) { Label = "Обновити", IsImportant = true };
            //refreshButton.Clicked += OnRefreshClick;
            toolbar.Add(buttonRefresh);

            ToolButton buttonDelete = new ToolButton(Stock.Delete) { Label = "Видалити", IsImportant = true };
            //deleteButton.Clicked += OnDeleteClick;
            toolbar.Add(buttonDelete);

            HBox hBoxScroll = new HBox();
            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 300);

            scrollList.Add(listBoxTableParts);
            hBoxScroll.PackStart(scrollList, true, true, 5);

            vBox.PackStart(hBoxScroll, false, false, 0);
            hPaned.Pack2(vBox, true, false);
        }

        void CreatePack1(HPaned hPaned)
        {
            VBox vBox = new VBox();

            //Назва
            HBox hBoxName = new HBox();
            vBox.PackStart(hBoxName, false, false, 5);

            hBoxName.PackStart(new Label("Назва:"), false, false, 5);
            hBoxName.PackStart(entryName, false, false, 5);

            //Блок
            HBox hBoxBlock = new HBox();
            vBox.PackStart(hBoxBlock, false, false, 5);

            foreach (ConfigurationConstantsBlock block in Conf!.ConstantsBlock.Values)
                comboBoxBlock.Append(block.BlockName, block.BlockName);

            hBoxBlock.PackStart(new Label("Блок:"), false, false, 5);
            hBoxBlock.PackStart(comboBoxBlock, false, false, 5);

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

            hPaned.Pack1(vBox, false, false);
        }

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            foreach (ConfigurationObjectTablePart tablePart in ConfConstants!.TabularParts.Values)
                listBoxTableParts.Add(new Label(tablePart.Name) { Halign = Align.Start });

            entryName.Text = ConfConstants?.Name;
            textViewDesc.Buffer.Text = ConfConstants?.Desc;

            comboBoxBlock.ActiveId = ConfConstants?.Block.BlockName;
            comboBoxType.ActiveId = ConfConstants?.Type;

            if (comboBoxType.ActiveId == "pointer")
                comboBoxPointer.ActiveId = ConfConstants?.Pointer;

            if (comboBoxType.ActiveId == "enum")
                comboBoxEnum.ActiveId = ConfConstants?.Pointer;

            OnComboBoxTypeChanged(comboBoxType, new EventArgs());
        }

        public void SetDefValue()
        {
            comboBoxBlock.Active = 0;
            comboBoxType.Active = 0;

            OnComboBoxTypeChanged(comboBoxType, new EventArgs());
        }

        void GetValue()
        {
            ConfConstants.Name = entryName.Text;
            ConfConstants.Desc = textViewDesc.Buffer.Text;
            ConfConstants.Block = Conf!.ConstantsBlock[comboBoxBlock.ActiveId];
            ConfConstants.Type = comboBoxType.ActiveId;

            if (ConfConstants!.Type == "pointer")
                ConfConstants.Pointer = comboBoxPointer.ActiveId;

            if (ConfConstants!.Type == "enum")
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
            string errorList = Configuration.ValidateConfigurationObjectName(Program.Kernel!, ref name);
            entryName.Text = name;

            if (errorList.Length > 0)
            {
                Message.ErrorMessage($"{errorList}");
                return;
            }

            if (IsNew)
            {
                if (Conf!.ConstantsBlock[comboBoxBlock.ActiveId].Constants.ContainsKey(entryName.Text))
                {
                    Message.ErrorMessage($"Назва константи не унікальна в межах блоку {comboBoxBlock.ActiveId}");
                    return;
                }
            }
            else
            {
                if (ConfConstants.Name != entryName.Text || ConfConstants.Block.BlockName != comboBoxBlock.ActiveId)
                {
                    if (Conf!.ConstantsBlock[comboBoxBlock.ActiveId].Constants.ContainsKey(entryName.Text))
                    {
                        Message.ErrorMessage($"Назва константи не унікальна в межах блоку {comboBoxBlock.ActiveId}");
                        return;
                    }
                }

                Conf!.ConstantsBlock[ConfConstants.Block.BlockName].Constants.Remove(ConfConstants.Name);
            }

            GetValue();

            if (ConfConstants.Type == "pointer" || ConfConstants.Type == "enum")
                if (String.IsNullOrEmpty(ConfConstants.Pointer))
                {
                    Message.ErrorMessage($"Потрібно деталізувати тип для [ pointer ] або [ enum ]\nВиберіть із списку тип для деталізації");
                    return;
                }

            Conf!.AppendConstants(ConfConstants.Block.BlockName, ConfConstants);

            IsNew = false;

            if (CallBack_RelodTree != null)
                CallBack_RelodTree.Invoke();

            if (CallBack_RenamePage != null)
                CallBack_RenamePage.Invoke($"Константа: {ConfConstants.Name}");
        }
    }
}