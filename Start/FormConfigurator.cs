using Gtk;

using AccountingSoftware;

namespace Configurator
{
    class FormConfigurator : Window
    {
        public ConfigurationParam? OpenConfigurationParam { get; set; }
        Configuration? Conf
        {
            get
            {
                return Program.Kernel?.Conf;
            }
        }
        List<string> TreeRowExpanded;

        HPaned hPaned;
        TreeView treeConfiguration;
        TreeStore treeStore;
        Notebook topNotebook;

        #region LoadTreeConfiguration

        string GetTypeInfo(string ConfType, string Pointer)
        {
            return ConfType == "pointer" || ConfType == "enum" ? Pointer : ConfType;
        }

        void LoadConstant(TreeIter rootIter, ConfigurationConstants confConstant)
        {
            string key = $"Константи.{confConstant.Block.BlockName}/{confConstant.Name}";

            TreeIter constantIter = treeStore.AppendValues(rootIter, confConstant.Name, GetTypeInfo(confConstant.Type, confConstant.Pointer), key);

            if (confConstant.TabularParts.Count > 0)
            {
                TreeIter constantTabularPartsIter = treeStore.AppendValues(constantIter, "[ Табличні частини ]", "", key);

                foreach (KeyValuePair<string, ConfigurationObjectTablePart> ConfTablePart in confConstant.TabularParts)
                {
                    TreeIter constantTablePartIter = treeStore.AppendValues(constantTabularPartsIter, ConfTablePart.Value.Name, "", key);

                    foreach (KeyValuePair<string, ConfigurationObjectField> ConfTablePartFields in ConfTablePart.Value.Fields)
                    {
                        string typeInfo = GetTypeInfo(ConfTablePartFields.Value.Type, ConfTablePartFields.Value.Pointer);
                        treeStore.AppendValues(constantTablePartIter, ConfTablePartFields.Value.Name, typeInfo, key);
                    }

                    IsExpand(constantTablePartIter);
                }

                IsExpand(constantTabularPartsIter);
            }

            IsExpand(constantIter);
        }

        void LoadConstants(TreeIter rootIter)
        {
            foreach (KeyValuePair<string, ConfigurationConstantsBlock> ConfConstantsBlock in Conf!.ConstantsBlock)
            {
                TreeIter contantsBlockIter = treeStore.AppendValues(rootIter, ConfConstantsBlock.Value.BlockName);
                foreach (ConfigurationConstants ConfConstants in ConfConstantsBlock.Value.Constants.Values)
                {
                    LoadConstant(contantsBlockIter, ConfConstants);
                }
                IsExpand(contantsBlockIter);
            }
        }

        void LoadDirectory(TreeIter rootIter, ConfigurationDirectories confDirectory)
        {
            TreeIter directoryIter = treeStore.AppendValues(rootIter, confDirectory.Name);

            foreach (KeyValuePair<string, ConfigurationObjectField> ConfFields in confDirectory.Fields)
            {
                string info = GetTypeInfo(ConfFields.Value.Type, ConfFields.Value.Pointer);
                treeStore.AppendValues(directoryIter, ConfFields.Value.Name, info);
            }

            if (confDirectory.TabularParts.Count > 0)
            {
                TreeIter directoriTabularPartsIter = treeStore.AppendValues(directoryIter, "[ Табличні частини ]");

                foreach (KeyValuePair<string, ConfigurationObjectTablePart> ConfTablePart in confDirectory.TabularParts)
                {
                    TreeIter directoriTablePartIter = treeStore.AppendValues(directoriTabularPartsIter, ConfTablePart.Value.Name);

                    foreach (KeyValuePair<string, ConfigurationObjectField> ConfTablePartFields in ConfTablePart.Value.Fields)
                    {
                        string info = GetTypeInfo(ConfTablePartFields.Value.Type, ConfTablePartFields.Value.Pointer);
                        treeStore.AppendValues(directoriTablePartIter, ConfTablePartFields.Value.Name, info);
                    }

                    IsExpand(directoriTablePartIter);
                }

                IsExpand(directoriTabularPartsIter);
            }

            IsExpand(directoryIter);
        }

        void LoadDirectories(TreeIter rootIter)
        {
            foreach (ConfigurationDirectories ConfDirectory in Conf!.Directories.Values)
            {
                LoadDirectory(rootIter, ConfDirectory);
            }
        }

        void LoadDocument(TreeIter rootIter, ConfigurationDocuments confDocument)
        {
            TreeIter documentIter = treeStore.AppendValues(rootIter, confDocument.Name);

            foreach (KeyValuePair<string, ConfigurationObjectField> ConfFields in confDocument.Fields)
            {
                string info = GetTypeInfo(ConfFields.Value.Type, ConfFields.Value.Pointer);
                treeStore.AppendValues(documentIter, ConfFields.Value.Name, info);
            }

            if (confDocument.TabularParts.Count > 0)
            {
                TreeIter documentTabularPartsIter = treeStore.AppendValues(documentIter, "[ Табличні частини ]");

                foreach (KeyValuePair<string, ConfigurationObjectTablePart> ConfTablePart in confDocument.TabularParts)
                {
                    TreeIter documentTablePartIter = treeStore.AppendValues(documentTabularPartsIter, ConfTablePart.Value.Name);

                    foreach (KeyValuePair<string, ConfigurationObjectField> ConfTablePartFields in ConfTablePart.Value.Fields)
                    {
                        string info = GetTypeInfo(ConfTablePartFields.Value.Type, ConfTablePartFields.Value.Pointer);
                        treeStore.AppendValues(documentTablePartIter, ConfTablePartFields.Value.Name, info);
                    }

                    IsExpand(documentTablePartIter);
                }

                IsExpand(documentTabularPartsIter);
            }

            IsExpand(documentIter);
        }

        void LoadDocuments(TreeIter rootIter)
        {
            foreach (ConfigurationDocuments ConfDocuments in Conf!.Documents.Values)
            {
                LoadDocument(rootIter, ConfDocuments);
            }
        }

        void LoadEnum(TreeIter rootIter, ConfigurationEnums confEnum)
        {
            TreeIter enumIter = treeStore.AppendValues(rootIter, confEnum.Name);

            foreach (KeyValuePair<string, ConfigurationEnumField> ConfEnumFields in confEnum.Fields)
                treeStore.AppendValues(enumIter, ConfEnumFields.Value.Name);

            IsExpand(enumIter);
        }

        void LoadEnums(TreeIter rootIter)
        {
            foreach (ConfigurationEnums ConfEnum in Conf!.Enums.Values)
            {
                LoadEnum(rootIter, ConfEnum);
            }
        }

        void LoadRegisterInformation(TreeIter rootIter, ConfigurationRegistersInformation confRegisterInformation)
        {
            TreeIter registerInformationIter = treeStore.AppendValues(rootIter, confRegisterInformation.Name);

            TreeIter dimensionFieldsIter = treeStore.AppendValues(registerInformationIter, "Виміри");

            //Поля вимірів
            foreach (KeyValuePair<string, ConfigurationObjectField> ConfDimensionFields in confRegisterInformation.DimensionFields)
            {
                string info = GetTypeInfo(ConfDimensionFields.Value.Type, ConfDimensionFields.Value.Pointer);
                treeStore.AppendValues(dimensionFieldsIter, ConfDimensionFields.Value.Name, info);
            }

            IsExpand(dimensionFieldsIter);

            TreeIter resourcesFieldsIter = treeStore.AppendValues(registerInformationIter, "Ресурси");

            //Поля ресурсів
            foreach (KeyValuePair<string, ConfigurationObjectField> ConfResourcesFields in confRegisterInformation.ResourcesFields)
            {
                string info = GetTypeInfo(ConfResourcesFields.Value.Type, ConfResourcesFields.Value.Pointer);
                treeStore.AppendValues(resourcesFieldsIter, ConfResourcesFields.Value.Name, info);
            }

            IsExpand(resourcesFieldsIter);

            TreeIter propertyFieldsIter = treeStore.AppendValues(registerInformationIter, "Поля");

            //Поля реквізитів
            foreach (KeyValuePair<string, ConfigurationObjectField> ConfPropertyFields in confRegisterInformation.PropertyFields)
            {
                string info = GetTypeInfo(ConfPropertyFields.Value.Type, ConfPropertyFields.Value.Pointer);
                treeStore.AppendValues(propertyFieldsIter, ConfPropertyFields.Value.Name, info);
            }

            IsExpand(propertyFieldsIter);

            IsExpand(registerInformationIter);
        }

        void LoadRegistersInformation(TreeIter rootIter)
        {
            foreach (ConfigurationRegistersInformation ConfRegistersInformation in Conf!.RegistersInformation.Values)
            {
                LoadRegisterInformation(rootIter, ConfRegistersInformation);
            }
        }

        void LoadRegisterAccumulation(TreeIter rootIter, ConfigurationRegistersAccumulation confRegisterAccumulation)
        {
            TreeIter registerAccumulationIter = treeStore.AppendValues(rootIter, confRegisterAccumulation.Name);

            TreeIter dimensionFieldsIter = treeStore.AppendValues(registerAccumulationIter, "Виміри");

            //Поля вимірів
            foreach (KeyValuePair<string, ConfigurationObjectField> ConfDimensionFields in confRegisterAccumulation.DimensionFields)
            {
                string info = GetTypeInfo(ConfDimensionFields.Value.Type, ConfDimensionFields.Value.Pointer);
                TreeIter fieldIter = treeStore.AppendValues(dimensionFieldsIter, ConfDimensionFields.Value.Name, info);
            }

            IsExpand(dimensionFieldsIter);

            TreeIter resourcesFieldsIter = treeStore.AppendValues(registerAccumulationIter, "Ресурси");

            //Поля ресурсів
            foreach (KeyValuePair<string, ConfigurationObjectField> ConfResourcesFields in confRegisterAccumulation.ResourcesFields)
            {
                string info = GetTypeInfo(ConfResourcesFields.Value.Type, ConfResourcesFields.Value.Pointer);
                TreeIter fieldIter = treeStore.AppendValues(resourcesFieldsIter, ConfResourcesFields.Value.Name, info);
            }

            IsExpand(resourcesFieldsIter);

            TreeIter propertyFieldsIter = treeStore.AppendValues(registerAccumulationIter, "Поля");

            //Поля реквізитів
            foreach (KeyValuePair<string, ConfigurationObjectField> ConfPropertyFields in confRegisterAccumulation.PropertyFields)
            {
                string info = GetTypeInfo(ConfPropertyFields.Value.Type, ConfPropertyFields.Value.Pointer);
                TreeIter fieldIter = treeStore.AppendValues(propertyFieldsIter, ConfPropertyFields.Value.Name, info);
            }

            IsExpand(propertyFieldsIter);

            IsExpand(registerAccumulationIter);
        }

        void LoadRegistersAccumulation(TreeIter rootIter)
        {
            foreach (ConfigurationRegistersAccumulation ConfRegistersAccumulation in Conf!.RegistersAccumulation.Values)
            {
                LoadRegisterAccumulation(rootIter, ConfRegistersAccumulation);
            }
        }

        void IsExpand(TreeIter iter)
        {
            TreePath path = treeConfiguration.Model.GetPath(iter);

            if (TreeRowExpanded.Contains(path.ToString()))
                treeConfiguration.ExpandToPath(path);
        }

        void LoadTree()
        {
            treeStore.Clear();

            TreeIter contantsIter = treeStore.AppendValues("Константи");
            LoadConstants(contantsIter);
            IsExpand(contantsIter);

            TreeIter directoriesIter = treeStore.AppendValues("Довідники");
            LoadDirectories(directoriesIter);
            IsExpand(directoriesIter);

            TreeIter documentsIter = treeStore.AppendValues("Документи");
            LoadDocuments(documentsIter);
            IsExpand(documentsIter);

            TreeIter enumsIter = treeStore.AppendValues("Перелічення");
            LoadEnums(enumsIter);
            IsExpand(enumsIter);

            TreeIter registersInformationIter = treeStore.AppendValues("Регістри відомостей");
            LoadRegistersInformation(registersInformationIter);
            IsExpand(registersInformationIter);

            TreeIter registersAccumulationIter = treeStore.AppendValues("Регістри накопичення");
            LoadRegistersAccumulation(registersAccumulationIter);
            IsExpand(registersAccumulationIter);
        }

        TreeStore AddTreeColumn()
        {
            treeStore = new TreeStore(typeof(string), typeof(string), typeof(string));

            treeConfiguration.AppendColumn(new TreeViewColumn("Конфігурація", new CellRendererText(), "text", 0));
            treeConfiguration.AppendColumn(new TreeViewColumn("Тип", new CellRendererText(), "text", 1));
            treeConfiguration.AppendColumn(new TreeViewColumn("Ключ", new CellRendererText(), "text", 2) { /*Visible = false*/ });
            treeConfiguration.Model = treeStore;

            return treeStore;
        }

        #endregion

        public FormConfigurator() : base("Конфігуратор")
        {
            SetDefaultSize(1000, 600);
            SetPosition(WindowPosition.Center);
            SetDefaultIconFromFile("configurator.ico");

            DeleteEvent += delegate { Application.Quit(); };

            VBox vbox = new VBox(false, 0);
            Add(vbox);

            Toolbar toolbar = new Toolbar();
            vbox.PackStart(toolbar, false, false, 0);

            Menu Menu = new Menu();
            MenuItem AddConstant = new MenuItem("Константу");
            AddConstant.Activated += OnAddConstant;
            Menu.Add(AddConstant);
            Menu.Add(new MenuItem("Блок констант"));
            Menu.ShowAll();

            MenuToolButton mt = new MenuToolButton(Stock.Add) { Label = "Додати", IsImportant = true };
            mt.Menu = Menu;

            //mt.Add(new Label("test"));
            toolbar.Add(mt);

            ToolButton refreshButton = new ToolButton(Stock.Refresh) { Label = "Обновити", IsImportant = true };
            refreshButton.Clicked += OnRefreshClick;
            toolbar.Add(refreshButton);

            ToolButton deleteButton = new ToolButton(Stock.Delete) { Label = "Видалити", IsImportant = true };
            //deleteButton.Clicked += OnDeleteClick;
            toolbar.Add(deleteButton);

            ToolButton copyButton = new ToolButton(Stock.Copy) { Label = "Копіювати", IsImportant = true };
            //copyButton.Clicked += OnCopyClick;
            toolbar.Add(copyButton);

            hPaned = new HPaned();
            hPaned.Position = 400;

            ScrolledWindow scrollTree = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollTree.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

            treeConfiguration = new TreeView();
            treeConfiguration.RowActivated += OnRowActivated;
            treeConfiguration.RowExpanded += OnRowExpanded;
            treeConfiguration.RowCollapsed += OnRowCollapsed;

            TreeRowExpanded = new List<string>();

            scrollTree.Add(treeConfiguration);
            treeStore = AddTreeColumn();

            hPaned.Pack1(scrollTree, false, true);

            topNotebook = new Notebook() { Scrollable = true, EnablePopup = true, BorderWidth = 0, ShowBorder = false };
            topNotebook.TabPos = PositionType.Top;

            CreateNotebookPage("Стартова", null);

            hPaned.Pack2(topNotebook, false, true);

            vbox.PackStart(hPaned, true, true, 0);
            ShowAll();

            LoadTree();
        }

        void CallBack_CloseCurrentPageNotebook()
        {
            topNotebook.RemovePage(topNotebook.CurrentPage);
        }

        void CreateNotebookPage(string tabName, System.Func<Widget>? pageWidget)
        {
            ScrolledWindow scroll = new ScrolledWindow() { ShadowType = ShadowType.In };
            scroll.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

            int numPage = topNotebook.AppendPage(scroll, new Label { Text = tabName, Expand = false, Halign = Align.Start });

            if (pageWidget != null)
                scroll.Add((Widget)pageWidget.Invoke());

            topNotebook.ShowAll();

            topNotebook.CurrentPage = numPage;
        }

        void OnRowActivated(object sender, RowActivatedArgs args)
        {
            TreeIter iter;

            if (!treeConfiguration.Selection.GetSelected(out iter) || !treeConfiguration.Model.GetIter(out iter, args.Path))
                return;

            string keyComposite = (string)treeConfiguration.Model.GetValue(iter, 2);

            if (String.IsNullOrEmpty(keyComposite) || keyComposite.IndexOf(".") == -1)
                return;

            string[] keySplit = keyComposite.Split(".");
            string block = keySplit[0];
            string name = keySplit[1];

            switch (block)
            {
                case "Константи":
                    {
                        string[] blockAndName = name.Split("/");
                        string blockConst = blockAndName[0];
                        string nameConst = blockAndName[1];

                        CreateNotebookPage("Константа: " + nameConst, () =>
                        {
                            PageConstant page = new PageConstant()
                            {
                                IsNew = false,
                                CallBack_ClosePage = CallBack_CloseCurrentPageNotebook,
                                CallBack_RelodTree = LoadTree,
                                ConfConstants = Conf!.ConstantsBlock[blockConst].Constants[nameConst]
                            };

                            page.SetValue();

                            return page;
                        });

                        break;
                    }
            }
        }

        void OnRowExpanded(object sender, RowExpandedArgs args)
        {
            if (!TreeRowExpanded.Contains(args.Path.ToString()))
                TreeRowExpanded.Add(args.Path.ToString());
        }

        void OnRowCollapsed(object sender, RowCollapsedArgs args)
        {
            if (TreeRowExpanded.Contains(args.Path.ToString()))
                TreeRowExpanded.Remove(args.Path.ToString());
        }

        void OnRefreshClick(object? sender, EventArgs args)
        {
            LoadTree();
        }

        void OnAddConstant(object? sender, EventArgs args)
        {
            CreateNotebookPage("Константа *", () =>
            {
                PageConstant page = new PageConstant()
                {
                    IsNew = true,
                    CallBack_ClosePage = CallBack_CloseCurrentPageNotebook,
                    CallBack_RelodTree = LoadTree
                };

                page.SetDefValue();

                return page;
            });
        }
    }
}