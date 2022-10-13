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
        public System.Action? CallBack_RelodTree { get; set; }
        public bool IsNew { get; set; } = true;

        ListBox listBoxTableParts = new ListBox() { SelectionMode = SelectionMode.Single };
        Entry entryName = new Entry() { WidthRequest = 300 };
        TextView textViewDesc = new TextView();
        ComboBoxText comboBoxBlock = new ComboBoxText();
        ComboBoxText comboBoxType = new ComboBoxText();
        ComboBoxText comboBoxPointer = new ComboBoxText();
        ComboBoxText comboBoxEnum = new ComboBoxText();

        public PageConstant() : base()
        {
            new VBox(false, 0);
            BorderWidth = 0;

            HBox hBox = new HBox(false, 0);

            Button bSave = new Button("Зберегти");
            bSave.Clicked += OnSaveClick;

            hBox.PackStart(bSave, false, false, 10);

            Button bClose = new Button("Закрити");
            bClose.Clicked += (object? sender, EventArgs args) => { if (CallBack_ClosePage != null) CallBack_ClosePage.Invoke(); };

            hBox.PackStart(bClose, false, false, 10);

            PackStart(hBox, false, false, 10);

            HPaned hPaned = new HPaned();
            hPaned.BorderWidth = 7;
            hPaned.Position = 200;

            CreatePack1(hPaned);
            CreatePack2(hPaned);

            PackStart(hPaned, false, false, 5);

            ShowAll();
        }

        void CreatePack1(HPaned hPaned)
        {
            VBox vBox = new VBox(false, 0);
            vBox.PackStart(new Label("Табличні частини:") { Halign = Align.Start }, false, false, 5);

            ScrolledWindow scrollList = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollList.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollList.SetSizeRequest(0, 300);

            scrollList.Add(listBoxTableParts);

            vBox.PackStart(scrollList, false, false, 0);
            hPaned.Pack1(vBox, true, true);
        }

        void CreatePack2(HPaned hPaned)
        {
            int offset = 110;

            VBox vBox = new VBox(false, 0);

            //Назва
            Fixed fixName = new Fixed();
            vBox.PackStart(fixName, false, false, 5);

            fixName.Put(new Label("Назва:"), 5, 5);
            fixName.Put(entryName, offset, 0);

            //Блок
            foreach (ConfigurationConstantsBlock block in Conf!.ConstantsBlock.Values)
                comboBoxBlock.Append(block.BlockName, block.BlockName);

            Fixed fixBlock = new Fixed();
            vBox.PackStart(fixBlock, false, false, 5);

            fixBlock.Put(new Label("Блок:"), 5, 5);
            fixBlock.Put(comboBoxBlock, offset, 0);

            //Тип
            foreach (var fieldType in FieldType.DefaultList())
                comboBoxType.Append(fieldType.ConfTypeName, fieldType.ViewTypeName);

            comboBoxType.Changed += OnComboBoxTypeChanged;

            Fixed fixType = new Fixed();
            vBox.PackStart(fixType, false, false, 5);

            fixType.Put(new Label("Тип:"), 5, 5);
            fixType.Put(comboBoxType, offset, 0);

            //Pointer
            foreach (ConfigurationDirectories item in Conf!.Directories.Values)
                comboBoxPointer.Append($"Довідники.{item.Name}", $"Довідники.{item.Name}");

            foreach (ConfigurationDocuments item in Conf!.Documents.Values)
                comboBoxPointer.Append($"Документи.{item.Name}", $"Документи.{item.Name}");

            Fixed fixPointer = new Fixed();
            vBox.PackStart(fixPointer, false, false, 5);

            fixPointer.Put(new Label("Вказівник:"), 5, 5);
            fixPointer.Put(comboBoxPointer, offset, 0);

            //Enum
            foreach (ConfigurationEnums item in Conf!.Enums.Values)
                comboBoxEnum.Append($"Перелічення.{item.Name}", $"Перелічення.{item.Name}");

            Fixed fixEnum = new Fixed();
            vBox.PackStart(fixEnum, false, false, 5);

            fixEnum.Put(new Label("Перелічення:"), 5, 5);
            fixEnum.Put(comboBoxEnum, offset, 0);

            //Опис
            Fixed fixDesc = new Fixed();
            vBox.PackStart(fixDesc, false, false, 5);

            fixDesc.Put(new Label("Опис:"), 5, 5);

            ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollTextView.SetSizeRequest(400, 100);
            scrollTextView.Add(textViewDesc);

            fixDesc.Put(scrollTextView, offset, 5);

            hPaned.Pack2(vBox, false, false);
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
        }
    }
}