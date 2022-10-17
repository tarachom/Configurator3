using Gtk;

using AccountingSoftware;

namespace Configurator
{
    class PageConstantBlock : VBox
    {
        Configuration? Conf
        {
            get
            {
                return Program.Kernel?.Conf;
            }
        }

        public ConfigurationConstantsBlock ConfConstantsBlock { get; set; } = new ConfigurationConstantsBlock();
        public FormConfigurator? GeneralForm { get; set; }
        public bool IsNew { get; set; } = true;

        Entry entryName = new Entry() { WidthRequest = 400 };
        TextView textViewDesc = new TextView();

        public PageConstantBlock() : base()
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

            //Опис
            HBox hBoxDesc = new HBox();
            vBox.PackStart(hBoxDesc, false, false, 5);

            hBoxDesc.PackStart(new Label("Опис:") { Valign = Align.Start }, false, false, 5);

            ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollTextView.SetSizeRequest(400, 100);
            scrollTextView.Add(textViewDesc);

            hBoxDesc.PackStart(scrollTextView, false, false, 5);

            PackStart(vBox, false, false, 5);
        }

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            entryName.Text = ConfConstantsBlock.BlockName;
            textViewDesc.Buffer.Text = ConfConstantsBlock.Desc;
        }

        void GetValue()
        {
            ConfConstantsBlock.BlockName = entryName.Text;
            ConfConstantsBlock.Desc = textViewDesc.Buffer.Text;
        }

        #endregion

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
                if (Conf!.ConstantsBlock.ContainsKey(entryName.Text))
                {
                    Message.Error($"Назва блоку не унікальна");
                    return;
                }
            }
            else
            {
                if (ConfConstantsBlock.BlockName != entryName.Text)
                {
                    if (Conf!.ConstantsBlock.ContainsKey(entryName.Text))
                    {
                        Message.Error($"Назва блоку не унікальна");
                        return;
                    }
                }

                Conf!.ConstantsBlock.Remove(ConfConstantsBlock.BlockName);
            }

            GetValue();

            Conf!.ConstantsBlock.Add(ConfConstantsBlock.BlockName, ConfConstantsBlock);

            IsNew = false;

            GeneralForm?.LoadTree();
            GeneralForm?.RenameCurrentPageNotebook($"Блок констант: {ConfConstantsBlock.BlockName}");
        }
    }
}