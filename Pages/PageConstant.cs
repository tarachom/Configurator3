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
        public ConfigurationConstantsBlock? ConfConstantsBlock { get; set; }
        public ConfigurationConstants ConfConstants { get; set; } = new ConfigurationConstants();
        public System.Action? CallBack_ClosePage { get; set; }
        //public System.Action<TreePath?>? CallBack_ReloadTree { get; set; }
        public TreePath? TreePathExpand { get; set; }
        public bool IsNew { get; set; } = true;

        ListBox listBoxTableParts = new ListBox() { SelectionMode = SelectionMode.Single };
        Entry entryName = new Entry() { WidthRequest = 300 };
        TextView textViewDesc = new TextView();

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

        public void SetValue()
        {
            foreach (ConfigurationObjectTablePart tablePart in ConfConstants!.TabularParts.Values)
                listBoxTableParts.Add(new Label(tablePart.Name) { Halign = Align.Start });

            entryName.Text = ConfConstants?.Name;
            textViewDesc.Buffer.Text = ConfConstants?.Desc;
        }

        void GetValue()
        {
            ConfConstants.Name = entryName.Text;
            ConfConstants.Desc = textViewDesc.Buffer.Text;
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
            VBox vBox = new VBox(false, 0);

            Fixed fixName = new Fixed();
            vBox.PackStart(fixName, false, false, 5);

            fixName.Put(new Label("Назва:"), 5, 5);
            fixName.Put(entryName, 60, 0);

            Fixed fixDesc = new Fixed();
            vBox.PackStart(fixDesc, false, false, 5);

            fixDesc.Put(new Label("Опис:"), 5, 5);

            ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollTextView.SetSizeRequest(300, 100);

            scrollTextView.Add(textViewDesc);

            fixDesc.Put(scrollTextView, 60, 5);
            textViewDesc.SetSizeRequest(300, 50);
            hPaned.Pack2(vBox, false, false);
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
                if (ConfConstantsBlock!.Constants.ContainsKey(entryName.Text))
                {
                    Message.ErrorMessage("Назва константи не унікальна");
                    return;
                }

                GetValue();

                Conf?.AppendConstants(ConfConstantsBlock!.BlockName, ConfConstants);
            }
            else
            {
                if (ConfConstants.Name != entryName.Text)
                {
                    if (ConfConstantsBlock!.Constants.ContainsKey(entryName.Text))
                    {
                        Message.ErrorMessage("Назва константи не унікальна");
                        return;
                    }

                    Conf?.ConstantsBlock.Remove(ConfConstants.Name);

                    GetValue();

                    Conf?.AppendConstants(ConfConstantsBlock!.BlockName, ConfConstants);
                }
            }
        }
    }
}