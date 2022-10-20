using Gtk;

using AccountingSoftware;

namespace Configurator
{
    class FormConfigurator : Window
    {
        readonly object loked = new Object();
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
        Statusbar statusBar;

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
                TreeIter constantTabularPartsIter = treeStore.AppendValues(constantIter, "[ Табличні частини ]");

                foreach (KeyValuePair<string, ConfigurationObjectTablePart> ConfTablePart in confConstant.TabularParts)
                {
                    string keyTablePart = $"{key}/{ConfTablePart.Value.Name}";

                    TreeIter constantTablePartIter = treeStore.AppendValues(constantTabularPartsIter, ConfTablePart.Value.Name, "", keyTablePart);

                    foreach (KeyValuePair<string, ConfigurationObjectField> ConfTablePartFields in ConfTablePart.Value.Fields)
                    {
                        string typeInfo = GetTypeInfo(ConfTablePartFields.Value.Type, ConfTablePartFields.Value.Pointer);
                        string keyField = $"{keyTablePart}/{ConfTablePartFields.Value.Name}";

                        treeStore.AppendValues(constantTablePartIter, ConfTablePartFields.Value.Name, typeInfo, keyField);
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
                string key = $"БлокКонстант.{ConfConstantsBlock.Value.BlockName}";

                TreeIter contantsBlockIter = treeStore.AppendValues(rootIter, ConfConstantsBlock.Value.BlockName, "", key);

                foreach (ConfigurationConstants ConfConstants in ConfConstantsBlock.Value.Constants.Values)
                {
                    LoadConstant(contantsBlockIter, ConfConstants);
                }
                IsExpand(contantsBlockIter);
            }
        }

        void LoadDirectory(TreeIter rootIter, ConfigurationDirectories confDirectory)
        {
            string key = $"Довідники.{confDirectory.Name}";

            TreeIter directoryIter = treeStore.AppendValues(rootIter, confDirectory.Name, "", key);

            foreach (KeyValuePair<string, ConfigurationObjectField> ConfFields in confDirectory.Fields)
            {
                string info = GetTypeInfo(ConfFields.Value.Type, ConfFields.Value.Pointer);
                string keyField = $"{key}:{ConfFields.Value.Name}";

                treeStore.AppendValues(directoryIter, ConfFields.Value.Name, info, keyField);
            }

            if (confDirectory.TabularParts.Count > 0)
            {
                TreeIter directoriTabularPartsIter = treeStore.AppendValues(directoryIter, "[ Табличні частини ]");

                foreach (KeyValuePair<string, ConfigurationObjectTablePart> ConfTablePart in confDirectory.TabularParts)
                {
                    string keyTablePart = $"{key}/{ConfTablePart.Value.Name}";

                    TreeIter directoriTablePartIter = treeStore.AppendValues(directoriTabularPartsIter, ConfTablePart.Value.Name, "", keyTablePart);

                    foreach (KeyValuePair<string, ConfigurationObjectField> ConfTablePartFields in ConfTablePart.Value.Fields)
                    {
                        string info = GetTypeInfo(ConfTablePartFields.Value.Type, ConfTablePartFields.Value.Pointer);
                        string keyField = $"{keyTablePart}/{ConfTablePartFields.Value.Name}";

                        treeStore.AppendValues(directoriTablePartIter, ConfTablePartFields.Value.Name, info, keyField);
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
            string key = $"Документи.{confDocument.Name}";

            TreeIter documentIter = treeStore.AppendValues(rootIter, confDocument.Name, "", key);

            foreach (KeyValuePair<string, ConfigurationObjectField> ConfFields in confDocument.Fields)
            {
                string info = GetTypeInfo(ConfFields.Value.Type, ConfFields.Value.Pointer);
                string keyField = $"{key}:{ConfFields.Value.Name}";

                treeStore.AppendValues(documentIter, ConfFields.Value.Name, info, keyField);
            }

            if (confDocument.TabularParts.Count > 0)
            {
                TreeIter documentTabularPartsIter = treeStore.AppendValues(documentIter, "[ Табличні частини ]");

                foreach (KeyValuePair<string, ConfigurationObjectTablePart> ConfTablePart in confDocument.TabularParts)
                {
                    string keyTablePart = $"{key}/{ConfTablePart.Value.Name}";

                    TreeIter documentTablePartIter = treeStore.AppendValues(documentTabularPartsIter, ConfTablePart.Value.Name, "", keyTablePart);

                    foreach (KeyValuePair<string, ConfigurationObjectField> ConfTablePartFields in ConfTablePart.Value.Fields)
                    {
                        string info = GetTypeInfo(ConfTablePartFields.Value.Type, ConfTablePartFields.Value.Pointer);
                        string keyField = $"{keyTablePart}/{ConfTablePartFields.Value.Name}";

                        treeStore.AppendValues(documentTablePartIter, ConfTablePartFields.Value.Name, info, keyField);
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
            string key = $"Перелічення.{confEnum.Name}";

            TreeIter enumIter = treeStore.AppendValues(rootIter, confEnum.Name, "", key);

            foreach (KeyValuePair<string, ConfigurationEnumField> ConfEnumFields in confEnum.Fields)
            {
                string keyField = $"{key}:{ConfEnumFields.Value.Name}";

                treeStore.AppendValues(enumIter, ConfEnumFields.Value.Name, "", keyField);
            }

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
            string key = $"РегістриІнформації.{confRegisterInformation.Name}";

            TreeIter registerInformationIter = treeStore.AppendValues(rootIter, confRegisterInformation.Name, "", key);

            TreeIter dimensionFieldsIter = treeStore.AppendValues(registerInformationIter, "Виміри");

            //Поля вимірів
            foreach (KeyValuePair<string, ConfigurationObjectField> ConfDimensionFields in confRegisterInformation.DimensionFields)
            {
                string info = GetTypeInfo(ConfDimensionFields.Value.Type, ConfDimensionFields.Value.Pointer);
                string keyField = $"{key}/Dimension:{ConfDimensionFields.Value.Name}";

                treeStore.AppendValues(dimensionFieldsIter, ConfDimensionFields.Value.Name, info, keyField);
            }

            IsExpand(dimensionFieldsIter);

            TreeIter resourcesFieldsIter = treeStore.AppendValues(registerInformationIter, "Ресурси");

            //Поля ресурсів
            foreach (KeyValuePair<string, ConfigurationObjectField> ConfResourcesFields in confRegisterInformation.ResourcesFields)
            {
                string info = GetTypeInfo(ConfResourcesFields.Value.Type, ConfResourcesFields.Value.Pointer);
                string keyField = $"{key}/Resources:{ConfResourcesFields.Value.Name}";

                treeStore.AppendValues(resourcesFieldsIter, ConfResourcesFields.Value.Name, info, keyField);
            }

            IsExpand(resourcesFieldsIter);

            TreeIter propertyFieldsIter = treeStore.AppendValues(registerInformationIter, "Поля");

            //Поля реквізитів
            foreach (KeyValuePair<string, ConfigurationObjectField> ConfPropertyFields in confRegisterInformation.PropertyFields)
            {
                string info = GetTypeInfo(ConfPropertyFields.Value.Type, ConfPropertyFields.Value.Pointer);
                string keyField = $"{key}/Property:{ConfPropertyFields.Value.Name}";

                treeStore.AppendValues(propertyFieldsIter, ConfPropertyFields.Value.Name, info, keyField);
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
            string key = $"РегістриНакопичення.{confRegisterAccumulation.Name}";

            TreeIter registerAccumulationIter = treeStore.AppendValues(rootIter, confRegisterAccumulation.Name, "", key);

            TreeIter dimensionFieldsIter = treeStore.AppendValues(registerAccumulationIter, "Виміри");

            //Поля вимірів
            foreach (KeyValuePair<string, ConfigurationObjectField> ConfDimensionFields in confRegisterAccumulation.DimensionFields)
            {
                string info = GetTypeInfo(ConfDimensionFields.Value.Type, ConfDimensionFields.Value.Pointer);
                string keyField = $"{key}/Dimension:{ConfDimensionFields.Value.Name}";

                TreeIter fieldIter = treeStore.AppendValues(dimensionFieldsIter, ConfDimensionFields.Value.Name, info, keyField);
            }

            IsExpand(dimensionFieldsIter);

            TreeIter resourcesFieldsIter = treeStore.AppendValues(registerAccumulationIter, "Ресурси");

            //Поля ресурсів
            foreach (KeyValuePair<string, ConfigurationObjectField> ConfResourcesFields in confRegisterAccumulation.ResourcesFields)
            {
                string info = GetTypeInfo(ConfResourcesFields.Value.Type, ConfResourcesFields.Value.Pointer);
                string keyField = $"{key}/Resources:{ConfResourcesFields.Value.Name}";

                TreeIter fieldIter = treeStore.AppendValues(resourcesFieldsIter, ConfResourcesFields.Value.Name, info, keyField);
            }

            IsExpand(resourcesFieldsIter);

            TreeIter propertyFieldsIter = treeStore.AppendValues(registerAccumulationIter, "Поля");

            //Поля реквізитів
            foreach (KeyValuePair<string, ConfigurationObjectField> ConfPropertyFields in confRegisterAccumulation.PropertyFields)
            {
                string info = GetTypeInfo(ConfPropertyFields.Value.Type, ConfPropertyFields.Value.Pointer);
                string keyField = $"{key}/Property:{ConfPropertyFields.Value.Name}";

                TreeIter fieldIter = treeStore.AppendValues(propertyFieldsIter, ConfPropertyFields.Value.Name, info, keyField);
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
            lock (loked)
            {
                Gtk.Application.Invoke(
                    delegate
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

                        TreeIter registersInformationIter = treeStore.AppendValues("Регістри інформації");
                        LoadRegistersInformation(registersInformationIter);
                        IsExpand(registersInformationIter);

                        TreeIter registersAccumulationIter = treeStore.AppendValues("Регістри накопичення");
                        LoadRegistersAccumulation(registersAccumulationIter);
                        IsExpand(registersAccumulationIter);
                    }
                );
            }
        }

        public void LoadTreeAsync()
        {
            Thread thread = new Thread(new ThreadStart(LoadTree));
            thread.Start();
        }

        TreeStore AddTreeColumn()
        {
            treeStore = new TreeStore(typeof(string), typeof(string), typeof(string));

            treeConfiguration.AppendColumn(new TreeViewColumn("Конфігурація", new CellRendererText(), "text", 0));
            treeConfiguration.AppendColumn(new TreeViewColumn("Тип", new CellRendererText(), "text", 1));
            treeConfiguration.AppendColumn(new TreeViewColumn("Ключ", new CellRendererText(), "text", 2) { Visible = false });
            treeConfiguration.Model = treeStore;

            return treeStore;
        }

        #endregion

        #region Func

        public Dictionary<string, ConfigurationObjectField> GetConstantsAllFields()
        {
            Dictionary<string, ConfigurationObjectField> ConstantsAllFields = new Dictionary<string, ConfigurationObjectField>();
            foreach (ConfigurationConstantsBlock block in Conf!.ConstantsBlock.Values)
            {
                foreach (ConfigurationConstants constants in block.Constants.Values)
                {
                    string fullName = block.BlockName + "." + constants.Name;
                    ConstantsAllFields.Add(fullName, new ConfigurationObjectField(fullName, constants.NameInTable, constants.Type, constants.Pointer, constants.Desc));
                }
            }

            return ConstantsAllFields;
        }

        public void SetValue()
        {
            if (OpenConfigurationParam != null)
            {
                statusBar.Halign = Align.Start;
                statusBar.Add(new Label($"Конфігурація: {OpenConfigurationParam.ConfigurationName} "));
                statusBar.Add(new Separator(Orientation.Vertical));
                statusBar.Add(new Label($" Сервер: {OpenConfigurationParam.DataBaseServer} "));
                statusBar.Add(new Separator(Orientation.Vertical));
                statusBar.Add(new Label($" База даних: {OpenConfigurationParam.DataBaseBaseName} "));
                statusBar.ShowAll();
            }
        }

        #endregion

        public FormConfigurator() : base("Конфігуратор")
        {
            SetDefaultSize(1000, 600);
            SetPosition(WindowPosition.Center);
            SetDefaultIconFromFile("configurator.ico");

            DeleteEvent += delegate { Application.Quit(); };

            VBox vbox = new VBox();
            Add(vbox);

            //MenuBar
            vbox.PackStart(CreateMenuBar(), false, false, 0);

            vbox.PackStart(CreateToolbar(), false, false, 0);

            hPaned = new HPaned();
            hPaned.Position = 200;

            ScrolledWindow scrollTree = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollTree.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

            treeConfiguration = new TreeView() { BorderWidth = 0 };
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

            statusBar = new Statusbar();
            vbox.PackStart(statusBar, false, false, 0);

            ShowAll();
        }

        VBox CreateMenuBar()
        {
            MenuBar mb = new MenuBar();

            //1
            Menu СonfMenu = new Menu();
            MenuItem configurationItem = new MenuItem("Конфігурація");
            configurationItem.Submenu = СonfMenu;

            MenuItem saveConfiguration = new MenuItem("Зберегти конфігурацію");
            saveConfiguration.Activated += OnSaveConfigurationClick;
            СonfMenu.Append(saveConfiguration);

            mb.Append(configurationItem);

            //2
            Menu UploadAndLoadMenu = new Menu();
            MenuItem uploadAndLoadDataMenuItem = new MenuItem("Вигрузка та загрузка");
            uploadAndLoadDataMenuItem.Submenu = UploadAndLoadMenu;

            MenuItem uploadConfigurationToFile = new MenuItem("Вигрузити конфігурацію в файл");
            uploadConfigurationToFile.Activated += OnUploadConfigurationToFileClick;
            UploadAndLoadMenu.Append(uploadConfigurationToFile);

            MenuItem loadConfigurationFromFile = new MenuItem("Загрузити конфігурацію з файлу");
            loadConfigurationFromFile.Activated += OnLoadConfigurationFromFileClick;
            UploadAndLoadMenu.Append(loadConfigurationFromFile);

            MenuItem uploadAndLoadData = new MenuItem("Вигрузка та загрузка даних");
            uploadAndLoadData.Activated += OnUnloadingAndLoadingData;
            UploadAndLoadMenu.Append(uploadAndLoadData);

            mb.Append(uploadAndLoadDataMenuItem);

            VBox vbox = new VBox(false, 2);
            vbox.PackStart(mb, false, false, 0);

            return vbox;
        }

        Toolbar CreateToolbar()
        {
            Toolbar toolbar = new Toolbar();

            MenuToolButton menuToolButton = new MenuToolButton(Stock.New) { Label = "Додати", IsImportant = true };
            menuToolButton.Menu = CreateAddMenu();
            toolbar.Add(menuToolButton);

            ToolButton refreshButton = new ToolButton(Stock.Refresh) { Label = "Обновити", IsImportant = true };
            refreshButton.Clicked += OnRefreshClick;
            toolbar.Add(refreshButton);

            ToolButton deleteButton = new ToolButton(Stock.Clear) { Label = "Видалити", IsImportant = true };
            deleteButton.Clicked += OnDeleteClick;
            toolbar.Add(deleteButton);

            ToolButton copyButton = new ToolButton(Stock.Copy) { Label = "Копіювати", IsImportant = true };
            copyButton.Clicked += OnCopyClick;
            toolbar.Add(copyButton);

            return toolbar;
        }

        Menu CreateAddMenu()
        {
            Menu Menu = new Menu();

            MenuItem AddConstantBlock = new MenuItem("Блок констант");
            AddConstantBlock.Activated += OnAddConstantBlock;
            Menu.Append(AddConstantBlock);

            MenuItem AddConstant = new MenuItem("Константу");
            AddConstant.Activated += OnAddConstant;
            Menu.Append(AddConstant);

            MenuItem AddDirectory = new MenuItem("Довідник");
            AddDirectory.Activated += OnAddDirectory;
            Menu.Append(AddDirectory);

            MenuItem AddDocument = new MenuItem("Документ");
            AddDocument.Activated += OnAddDocument;
            Menu.Append(AddDocument);

            MenuItem AddEnum = new MenuItem("Перелічення");
            AddEnum.Activated += OnAddEnum;
            Menu.Append(AddEnum);

            MenuItem AddRegistersInformation = new MenuItem("Регістр інформації");
            AddRegistersInformation.Activated += OnAddRegisterInformation;
            Menu.Append(AddRegistersInformation);

            MenuItem AddRegistersAccumulation = new MenuItem("Регістр накопичення");
            AddRegistersAccumulation.Activated += OnAddRegisterAccumulation;
            Menu.Append(AddRegistersAccumulation);

            Menu.ShowAll();

            return Menu;
        }

        #region Notebook Page

        public void CloseCurrentPageNotebook()
        {
            topNotebook.RemovePage(topNotebook.CurrentPage);
        }

        public void RenameCurrentPageNotebook(string name)
        {
            topNotebook.SetTabLabelText(topNotebook.CurrentPageWidget, name);
        }

        public void CreateNotebookPage(string tabName, System.Func<Widget>? pageWidget)
        {
            ScrolledWindow scroll = new ScrolledWindow() { ShadowType = ShadowType.In };
            scroll.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

            int numPage = topNotebook.AppendPage(scroll, new Label { Text = tabName, Expand = false, Halign = Align.Start });

            if (pageWidget != null)
                scroll.Add((Widget)pageWidget.Invoke());

            topNotebook.ShowAll();

            topNotebook.CurrentPage = numPage;
        }

        #endregion

        #region Event MenuBar

        void OnSaveConfigurationClick(object? sender, EventArgs args)
        {
            CreateNotebookPage("Збереження конфігурації", () =>
            {
                PageSaveConfiguration page = new PageSaveConfiguration()
                {
                    GeneralForm = this
                };

                return page;
            });
        }

        void OnUploadConfigurationToFileClick(object? sender, EventArgs args)
        {
            string folderSave = "";
            string fileConfName = "Confa_" + Conf!.NameSpace + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xml";
            bool saveOk = false;

            FileChooserDialog fc = new FileChooserDialog("Виберіть каталог для вигрузки конфігурації", this,
                FileChooserAction.SelectFolder, "Закрити", ResponseType.Cancel, "Вибрати", ResponseType.Accept);

            if (fc.Run() == (int)ResponseType.Accept)
            {
                if (!String.IsNullOrEmpty(fc.CurrentFolder))
                {
                    string fileConf = System.IO.Path.Combine(fc.CurrentFolder, fileConfName);
                    Configuration.Save(fileConf, Conf!);

                    folderSave = fc.CurrentFolder;
                    saveOk = true;
                }
            }

            fc.Destroy();

            if (saveOk)
                Message.Info(this, "Конфігурацію вигружено в файл: " + fileConfName + "\n\nКаталог: " + folderSave);
        }

        void OnLoadConfigurationFromFileClick(object? sender, EventArgs args)
        {
            bool loadOk = false;

            FileChooserDialog fc = new FileChooserDialog("Виберіть файл для загрузки конфігурації", this,
                            FileChooserAction.Open, "Закрити", ResponseType.Cancel, "Вибрати", ResponseType.Accept);

            fc.Filter = new FileFilter();
            fc.Filter.AddPattern("*.xml");

            if (fc.Run() == (int)ResponseType.Accept)
            {
                if (!String.IsNullOrEmpty(fc.Filename))
                {
                    Configuration openConf;
                    Configuration.Load(fc.Filename, out openConf);

                    Program.Kernel!.Conf = openConf;

                    loadOk = true;
                }
            }

            fc.Destroy();

            if (loadOk)
                LoadTreeAsync();
        }

        void OnUnloadingAndLoadingData(object? sender, EventArgs args)
        {
            CreateNotebookPage("Вигрузка та загрузка даних", () =>
            {
                PageUnloadingAndLoadingData page = new PageUnloadingAndLoadingData()
                {
                    GeneralForm = this
                };

                return page;
            });
        }

        #endregion

        #region Event Tree, ToolBar

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
                case "БлокКонстант":
                    {
                        CreateNotebookPage($"Блок констант: {name}", () =>
                        {
                            PageConstantBlock page = new PageConstantBlock()
                            {
                                ConfConstantsBlock = Conf!.ConstantsBlock[name],
                                IsNew = false,
                                GeneralForm = this
                            };

                            page.SetValue();

                            return page;
                        });
                        break;
                    }
                case "Константи":
                    {
                        string[] blockAndName = name.Split("/");
                        string blockConst = blockAndName[0];
                        string nameConst = blockAndName[1];

                        switch (blockAndName.Length)
                        {
                            case 2:
                                {
                                    CreateNotebookPage("Константа: " + nameConst, () =>
                                    {
                                        PageConstant page = new PageConstant()
                                        {
                                            IsNew = false,
                                            GeneralForm = this,
                                            ConfConstants = Conf!.ConstantsBlock[blockConst].Constants[nameConst]
                                        };

                                        page.SetValue();

                                        return page;
                                    });
                                    break;
                                }
                            case 3:
                                {
                                    string nameTablePart = blockAndName[2];

                                    CreateNotebookPage($"Таблична частина: {nameTablePart}", () =>
                                    {
                                        PageTablePart page = new PageTablePart()
                                        {
                                            GeneralForm = this,
                                            TabularParts = Conf!.ConstantsBlock[blockConst].Constants[nameConst].TabularParts,
                                            TablePart = Conf!.ConstantsBlock[blockConst].Constants[nameConst].TabularParts[nameTablePart],
                                            IsNew = false
                                        };

                                        page.SetValue();

                                        return page;
                                    });

                                    break;
                                }
                            case 4:
                                {
                                    string nameTablePart = blockAndName[2];
                                    string nameField = blockAndName[3];

                                    CreateNotebookPage($"Поле: {nameField}", () =>
                                    {
                                        PageField page = new PageField()
                                        {
                                            Fields = Conf!.ConstantsBlock[blockConst].Constants[nameConst].TabularParts[nameTablePart].Fields,
                                            Field = Conf!.ConstantsBlock[blockConst].Constants[nameConst].TabularParts[nameTablePart].Fields[nameField],
                                            IsNew = false,
                                            GeneralForm = this
                                        };

                                        page.SetValue();

                                        return page;
                                    });

                                    break;
                                }
                        }

                        break;
                    }
                case "Довідники":
                    {
                        string[] directoryPath = name.Split("/");
                        string directory = directoryPath[0];

                        switch (directoryPath.Length)
                        {
                            case 1:
                                {
                                    if (directory.IndexOf(":") != -1)
                                    {
                                        string[] directoryAndField = directory.Split(":");
                                        string directoryName = directoryAndField[0];
                                        string fieldName = directoryAndField[1];

                                        CreateNotebookPage($"Поле: {fieldName}", () =>
                                        {
                                            PageField page = new PageField()
                                            {
                                                Fields = Conf!.Directories[directoryName].Fields,
                                                Field = Conf!.Directories[directoryName].Fields[fieldName],
                                                IsNew = false,
                                                GeneralForm = this
                                            };

                                            page.SetValue();

                                            return page;
                                        });
                                    }
                                    else
                                    {
                                        CreateNotebookPage($"Довідник: {directory}", () =>
                                        {
                                            PageDirectory page = new PageDirectory()
                                            {
                                                ConfDirectory = Conf!.Directories[directory],
                                                IsNew = false,
                                                GeneralForm = this
                                            };

                                            page.SetValue();

                                            return page;
                                        });
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    string nameTablePart = directoryPath[1];

                                    CreateNotebookPage($"Таблична частина: {nameTablePart}", () =>
                                    {
                                        PageTablePart page = new PageTablePart()
                                        {
                                            GeneralForm = this,
                                            TabularParts = Conf!.Directories[directory].TabularParts,
                                            TablePart = Conf!.Directories[directory].TabularParts[nameTablePart],
                                            IsNew = false
                                        };

                                        page.SetValue();

                                        return page;
                                    });
                                    break;
                                }
                            case 3:
                                {
                                    string nameTablePart = directoryPath[1];
                                    string nameField = directoryPath[2];

                                    CreateNotebookPage($"Поле: {nameField}", () =>
                                    {
                                        PageField page = new PageField()
                                        {
                                            Fields = Conf!.Directories[directory].TabularParts[nameTablePart].Fields,
                                            Field = Conf!.Directories[directory].TabularParts[nameTablePart].Fields[nameField],
                                            IsNew = false,
                                            GeneralForm = this
                                        };

                                        page.SetValue();

                                        return page;
                                    });

                                    break;
                                }
                        }

                        break;
                    }
                case "Документи":
                    {
                        string[] documentPath = name.Split("/");
                        string document = documentPath[0];

                        switch (documentPath.Length)
                        {
                            case 1:
                                {
                                    if (document.IndexOf(":") != -1)
                                    {
                                        string[] documentAndField = document.Split(":");
                                        string documentName = documentAndField[0];
                                        string fieldName = documentAndField[1];

                                        CreateNotebookPage($"Поле: {fieldName}", () =>
                                        {
                                            PageField page = new PageField()
                                            {
                                                Fields = Conf!.Documents[documentName].Fields,
                                                Field = Conf!.Documents[documentName].Fields[fieldName],
                                                IsNew = false,
                                                GeneralForm = this
                                            };

                                            page.SetValue();

                                            return page;
                                        });
                                    }
                                    else
                                    {
                                        CreateNotebookPage($"Документ: {document}", () =>
                                        {
                                            PageDocument page = new PageDocument()
                                            {
                                                ConfDocument = Conf!.Documents[document],
                                                IsNew = false,
                                                GeneralForm = this
                                            };

                                            page.SetValue();

                                            return page;
                                        });
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    string nameTablePart = documentPath[1];

                                    CreateNotebookPage($"Таблична частина: {nameTablePart}", () =>
                                    {
                                        PageTablePart page = new PageTablePart()
                                        {
                                            GeneralForm = this,
                                            TabularParts = Conf!.Documents[document].TabularParts,
                                            TablePart = Conf!.Documents[document].TabularParts[nameTablePart],
                                            IsNew = false
                                        };

                                        page.SetValue();

                                        return page;
                                    });
                                    break;
                                }
                            case 3:
                                {
                                    string nameTablePart = documentPath[1];
                                    string nameField = documentPath[2];

                                    CreateNotebookPage($"Поле: {nameField}", () =>
                                    {
                                        PageField page = new PageField()
                                        {
                                            Fields = Conf!.Documents[document].TabularParts[nameTablePart].Fields,
                                            Field = Conf!.Documents[document].TabularParts[nameTablePart].Fields[nameField],
                                            IsNew = false,
                                            GeneralForm = this
                                        };

                                        page.SetValue();

                                        return page;
                                    });

                                    break;
                                }
                        }

                        break;
                    }
                case "РегістриІнформації":
                    {
                        string[] registerPath = name.Split("/");
                        string register = registerPath[0];

                        switch (registerPath.Length)
                        {
                            case 1:
                                {
                                    CreateNotebookPage($"Регістер інформації: {register}", () =>
                                    {
                                        PageRegisterInformation page = new PageRegisterInformation()
                                        {
                                            ConfRegister = Conf!.RegistersInformation[register],
                                            IsNew = false,
                                            GeneralForm = this
                                        };

                                        page.SetValue();

                                        return page;
                                    });

                                    break;
                                }
                            case 2:
                                {
                                    string[] typeAndField = registerPath[1].Split(":");
                                    string typeName = typeAndField[0];
                                    string fieldName = typeAndField[1];

                                    CreateNotebookPage($"Поле: {fieldName}", () =>
                                    {
                                        Dictionary<string, ConfigurationObjectField> AllFields = new Dictionary<string, ConfigurationObjectField>();

                                        foreach (ConfigurationObjectField item in Conf!.RegistersInformation[register].DimensionFields.Values)
                                            AllFields.Add(item.Name, item);

                                        foreach (ConfigurationObjectField item in Conf!.RegistersInformation[register].ResourcesFields.Values)
                                            AllFields.Add(item.Name, item);

                                        foreach (ConfigurationObjectField item in Conf!.RegistersInformation[register].PropertyFields.Values)
                                            AllFields.Add(item.Name, item);

                                        Dictionary<string, ConfigurationObjectField> Fields;
                                        ConfigurationObjectField Field;

                                        if (typeName == "Dimension")
                                        {
                                            Fields = Conf!.RegistersInformation[register].DimensionFields;
                                            Field = Conf!.RegistersInformation[register].DimensionFields[fieldName];
                                        }
                                        else if (typeName == "Resources")
                                        {
                                            Fields = Conf!.RegistersInformation[register].ResourcesFields;
                                            Field = Conf!.RegistersInformation[register].ResourcesFields[fieldName];
                                        }
                                        else
                                        {
                                            Fields = Conf!.RegistersInformation[register].PropertyFields;
                                            Field = Conf!.RegistersInformation[register].PropertyFields[fieldName];
                                        }

                                        PageField page = new PageField()
                                        {
                                            AllFields = AllFields,
                                            Fields = Fields,
                                            Field = Field,
                                            IsNew = false,
                                            GeneralForm = this
                                        };

                                        page.SetValue();

                                        return page;
                                    });

                                    break;
                                }
                        }

                        break;
                    }
                case "РегістриНакопичення":
                    {
                        string[] registerPath = name.Split("/");
                        string register = registerPath[0];

                        switch (registerPath.Length)
                        {
                            case 1:
                                {
                                    CreateNotebookPage($"Регістер накопичення: {register}", () =>
                                    {
                                        PageRegistersAccumulation page = new PageRegistersAccumulation()
                                        {
                                            ConfRegister = Conf!.RegistersAccumulation[register],
                                            IsNew = false,
                                            GeneralForm = this
                                        };

                                        page.SetValue();

                                        return page;
                                    });

                                    break;
                                }
                            case 2:
                                {
                                    string[] typeAndField = registerPath[1].Split(":");
                                    string typeName = typeAndField[0];
                                    string fieldName = typeAndField[1];

                                    CreateNotebookPage($"Поле: {fieldName}", () =>
                                    {
                                        Dictionary<string, ConfigurationObjectField> AllFields = new Dictionary<string, ConfigurationObjectField>();

                                        foreach (ConfigurationObjectField item in Conf!.RegistersAccumulation[register].DimensionFields.Values)
                                            AllFields.Add(item.Name, item);

                                        foreach (ConfigurationObjectField item in Conf!.RegistersAccumulation[register].ResourcesFields.Values)
                                            AllFields.Add(item.Name, item);

                                        foreach (ConfigurationObjectField item in Conf!.RegistersAccumulation[register].PropertyFields.Values)
                                            AllFields.Add(item.Name, item);

                                        Dictionary<string, ConfigurationObjectField> Fields;
                                        ConfigurationObjectField Field;

                                        if (typeName == "Dimension")
                                        {
                                            Fields = Conf!.RegistersAccumulation[register].DimensionFields;
                                            Field = Conf!.RegistersAccumulation[register].DimensionFields[fieldName];
                                        }
                                        else if (typeName == "Resources")
                                        {
                                            Fields = Conf!.RegistersAccumulation[register].ResourcesFields;
                                            Field = Conf!.RegistersAccumulation[register].ResourcesFields[fieldName];
                                        }
                                        else
                                        {
                                            Fields = Conf!.RegistersAccumulation[register].PropertyFields;
                                            Field = Conf!.RegistersAccumulation[register].PropertyFields[fieldName];
                                        }

                                        PageField page = new PageField()
                                        {
                                            AllFields = AllFields,
                                            Fields = Fields,
                                            Field = Field,
                                            IsNew = false,
                                            GeneralForm = this
                                        };

                                        page.SetValue();

                                        return page;
                                    });

                                    break;
                                }
                        }

                        break;
                    }
                case "Перелічення":
                    {
                        if (name.IndexOf(":") != -1)
                        {
                            string[] enumAndField = name.Split(":");
                            string enumName = enumAndField[0];
                            string fieldName = enumAndField[1];

                            CreateNotebookPage($"Поле: {fieldName}", () =>
                            {
                                PageEnumField page = new PageEnumField()
                                {
                                    Fields = Conf!.Enums[enumName].Fields,
                                    Field = Conf!.Enums[enumName].Fields[fieldName],
                                    IsNew = false,
                                    GeneralForm = this
                                };

                                page.SetValue();

                                return page;
                            });
                        }
                        else
                        {
                            CreateNotebookPage($"Перелічення: {name}", () =>
                            {
                                PageEnum page = new PageEnum()
                                {
                                    ConfEnum = Conf!.Enums[name],
                                    IsNew = false,
                                    GeneralForm = this
                                };

                                page.SetValue();

                                return page;
                            });
                        }
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
            LoadTreeAsync();
        }

        void OnDeleteClick(object? sender, EventArgs args)
        {
            TreeIter iter;

            if (!treeConfiguration.Selection.GetSelected(out iter))
                return;

            TreePath pathRemove = treeConfiguration.Model.GetPath(iter);

            string keyComposite = (string)treeConfiguration.Model.GetValue(iter, 2);

            if (String.IsNullOrEmpty(keyComposite) || keyComposite.IndexOf(".") == -1)
                return;

            string[] keySplit = keyComposite.Split(".");
            string block = keySplit[0];
            string name = keySplit[1];

            switch (block)
            {
                case "БлокКонстант":
                    {
                        if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                            Conf!.ConstantsBlock.Remove(name);
                        break;
                    }
                case "Константи":
                    {
                        string[] blockAndName = name.Split("/");
                        string blockConst = blockAndName[0];
                        string nameConst = blockAndName[1];

                        switch (blockAndName.Length)
                        {
                            case 2:
                                {
                                    if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                        Conf!.ConstantsBlock[blockConst].Constants.Remove(nameConst);

                                    break;
                                }
                            case 3:
                                {
                                    string nameTablePart = blockAndName[2];

                                    if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                        Conf!.ConstantsBlock[blockConst].Constants[nameConst].TabularParts.Remove(nameTablePart);

                                    break;
                                }
                            case 4:
                                {
                                    string nameTablePart = blockAndName[2];
                                    string nameField = blockAndName[3];

                                    if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                        Conf!.ConstantsBlock[blockConst].Constants[nameConst].TabularParts[nameTablePart].Fields.Remove(nameField);

                                    break;
                                }
                        }

                        break;
                    }
                case "Довідники":
                    {
                        string[] directoryPath = name.Split("/");
                        string directory = directoryPath[0];

                        switch (directoryPath.Length)
                        {
                            case 1:
                                {
                                    if (directory.IndexOf(":") != -1)
                                    {
                                        string[] directoryAndField = directory.Split(":");
                                        string directoryName = directoryAndField[0];
                                        string fieldName = directoryAndField[1];
                                        Conf!.Directories[directoryName].Fields.Remove(fieldName);
                                    }
                                    else
                                    {
                                        List<string> ListPointers = Conf!.SearchForPointers("Довідники." + directory);

                                        if (ListPointers.Count == 0)
                                        {
                                            if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                                Conf!.Directories.Remove(directory);
                                        }
                                        else
                                        {
                                            string textListPointer = "Знайденно " + ListPointers.Count.ToString() + " вказівники на довідник \"" + directory + "\":\n";

                                            foreach (string item in ListPointers)
                                                textListPointer += " -> " + item + "\n";

                                            textListPointer += "\nВидалитити неможливо";

                                            Message.Error(this, textListPointer);
                                        }
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    string nameTablePart = directoryPath[1];

                                    if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                        Conf!.Directories[directory].TabularParts.Remove(nameTablePart);

                                    break;
                                }
                            case 3:
                                {
                                    string nameTablePart = directoryPath[1];
                                    string nameField = directoryPath[2];

                                    if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                        Conf!.Directories[directory].TabularParts[nameTablePart].Fields.Remove(nameField);

                                    break;
                                }
                        }

                        break;
                    }
                case "Документи":
                    {
                        string[] documentPath = name.Split("/");
                        string document = documentPath[0];

                        switch (documentPath.Length)
                        {
                            case 1:
                                {
                                    if (document.IndexOf(":") != -1)
                                    {
                                        string[] documentAndField = document.Split(":");
                                        string documentName = documentAndField[0];
                                        string fieldName = documentAndField[1];

                                        if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                            Conf!.Documents[documentName].Fields.Remove(fieldName);
                                    }
                                    else
                                    {
                                        List<string> ListPointers = Conf!.SearchForPointers("Документи." + document);

                                        if (ListPointers.Count == 0)
                                        {
                                            if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                                Conf!.Documents.Remove(document);
                                        }
                                        else
                                        {
                                            string textListPointer = "Знайденно " + ListPointers.Count.ToString() + " вказівники на документ \"" + document + "\":\n";

                                            foreach (string item in ListPointers)
                                                textListPointer += " -> " + item + "\n";

                                            textListPointer += "\nВидалитити неможливо";

                                            Message.Error(this, textListPointer);
                                        }
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    string nameTablePart = documentPath[1];

                                    if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                        Conf!.Documents[document].TabularParts.Remove(nameTablePart);

                                    break;
                                }
                            case 3:
                                {
                                    string nameTablePart = documentPath[1];
                                    string nameField = documentPath[2];

                                    if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                        Conf!.Documents[document].TabularParts[nameTablePart].Fields.Remove(nameField);

                                    break;
                                }
                        }

                        break;
                    }
                case "РегістриІнформації":
                    {
                        string[] registerPath = name.Split("/");
                        string register = registerPath[0];

                        switch (registerPath.Length)
                        {
                            case 1:
                                {
                                    if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                        Conf!.RegistersInformation.Remove(register);

                                    break;
                                }
                            case 2:
                                {
                                    string[] typeAndField = registerPath[1].Split(":");
                                    string typeName = typeAndField[0];
                                    string fieldName = typeAndField[1];

                                    if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                    {
                                        if (typeName == "Dimension")
                                            Conf!.RegistersInformation[register].DimensionFields.Remove(fieldName);
                                        else if (typeName == "Resources")
                                            Conf!.RegistersInformation[register].ResourcesFields.Remove(fieldName);
                                        else
                                            Conf!.RegistersInformation[register].PropertyFields.Remove(fieldName);
                                    }

                                    break;
                                }
                        }

                        break;
                    }
                case "РегістриНакопичення":
                    {
                        string[] registerPath = name.Split("/");
                        string register = registerPath[0];

                        switch (registerPath.Length)
                        {
                            case 1:
                                {
                                    if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                        Conf!.RegistersAccumulation.Remove(register);

                                    break;
                                }
                            case 2:
                                {
                                    string[] typeAndField = registerPath[1].Split(":");
                                    string typeName = typeAndField[0];
                                    string fieldName = typeAndField[1];

                                    if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                    {
                                        if (typeName == "Dimension")
                                            Conf!.RegistersAccumulation[register].DimensionFields.Remove(fieldName);
                                        else if (typeName == "Resources")
                                            Conf!.RegistersAccumulation[register].ResourcesFields.Remove(fieldName);
                                        else
                                            Conf!.RegistersAccumulation[register].PropertyFields.Remove(fieldName);
                                    }

                                    break;
                                }
                        }

                        break;
                    }
                case "Перелічення":
                    {
                        if (name.IndexOf(":") != -1)
                        {
                            string[] enumAndField = name.Split(":");
                            string enumName = enumAndField[0];
                            string fieldName = enumAndField[1];

                            if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                Conf!.Enums[enumName].Fields.Remove(fieldName);
                        }
                        else
                        {
                            List<string> ListPointers = Conf!.SearchForPointersEnum("Перелічення." + name);

                            if (ListPointers.Count == 0)
                            {
                                if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                    Conf!.Enums.Remove(name);
                            }
                            else
                            {
                                string textListPointer = "Знайденно " + ListPointers.Count.ToString() + " вказівники на перелічення \"" + name + "\":\n";

                                foreach (string item in ListPointers)
                                    textListPointer += " -> " + item + "\n";

                                textListPointer += "\nВидалитити неможливо";

                                Message.Error(this, textListPointer);
                            }
                        }

                        break;
                    }
            }

            if (TreeRowExpanded.Contains(pathRemove.ToString()))
                TreeRowExpanded.Remove(pathRemove.ToString());

            LoadTreeAsync();
        }

        void OnCopyClick(object? sender, EventArgs args)
        {
            TreeIter iter;

            if (!treeConfiguration.Selection.GetSelected(out iter))
                return;

            TreePath pathRemove = treeConfiguration.Model.GetPath(iter);

            string keyComposite = (string)treeConfiguration.Model.GetValue(iter, 2);

            if (String.IsNullOrEmpty(keyComposite) || keyComposite.IndexOf(".") == -1)
                return;

            string[] keySplit = keyComposite.Split(".");
            string block = keySplit[0];
            string name = keySplit[1];

            switch (block)
            {
                case "БлокКонстант":
                    {
                        ConfigurationConstantsBlock newConstantBlock = Conf!.ConstantsBlock[name].Copy();
                        newConstantBlock.BlockName += GenerateName.GetNewName();

                        Dictionary<string, ConfigurationObjectField> AllFields = GetConstantsAllFields();

                        if (!Conf!.ConstantsBlock.ContainsKey(newConstantBlock.BlockName))
                        {
                            foreach (ConfigurationConstants itemConst in newConstantBlock.Constants.Values)
                            {
                                itemConst.NameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel!, "tab_constants", AllFields);

                                foreach (ConfigurationObjectTablePart tablePart in itemConst.TabularParts.Values)
                                    tablePart.Table = Configuration.GetNewUnigueTableName(Program.Kernel!);
                            }

                            Conf!.AppendConstantsBlock(newConstantBlock);
                        }

                        break;
                    }
                case "Константи":
                    {
                        string[] blockAndName = name.Split("/");
                        string blockConst = blockAndName[0];
                        string nameConst = blockAndName[1];

                        switch (blockAndName.Length)
                        {
                            case 2:
                                {
                                    ConfigurationConstants newConstant = Conf!.ConstantsBlock[blockConst].Constants[nameConst].Copy();
                                    newConstant.Name += GenerateName.GetNewName();
                                    newConstant.NameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel!, "tab_constants", GetConstantsAllFields());

                                    if (!Conf!.ConstantsBlock[blockConst].Constants.ContainsKey(newConstant.Name))
                                    {
                                        foreach (ConfigurationObjectTablePart tablePart in newConstant.TabularParts.Values)
                                            tablePart.Table = Configuration.GetNewUnigueTableName(Program.Kernel!);

                                        Conf!.ConstantsBlock[blockConst].AppendConstant(newConstant);
                                    }

                                    break;
                                }
                            case 3:
                                {
                                    string nameTablePart = blockAndName[2];
                                    ConfigurationObjectTablePart newTablePart = Conf!.ConstantsBlock[blockConst].Constants[nameConst].TabularParts[nameTablePart].Copy();
                                    newTablePart.Name += GenerateName.GetNewName();
                                    newTablePart.Table = Configuration.GetNewUnigueTableName(Program.Kernel!);

                                    if (!Conf!.ConstantsBlock[blockConst].Constants[nameConst].TabularParts.ContainsKey(newTablePart.Name))
                                        Conf!.ConstantsBlock[blockConst].Constants[nameConst].AppendTablePart(newTablePart);

                                    break;
                                }
                            case 4:
                                {
                                    string nameTablePart = blockAndName[2];
                                    string nameField = blockAndName[3];

                                    ConfigurationObjectField newField = Conf!.ConstantsBlock[blockConst].Constants[nameConst].TabularParts[nameTablePart].Fields[nameField].Copy();
                                    newField.Name += GenerateName.GetNewName();
                                    newField.NameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel!,
                                        Conf!.ConstantsBlock[blockConst].Constants[nameConst].TabularParts[nameTablePart].Table,
                                        Conf!.ConstantsBlock[blockConst].Constants[nameConst].TabularParts[nameTablePart].Fields);

                                    if (!Conf!.ConstantsBlock[blockConst].Constants[nameConst].TabularParts[nameTablePart].Fields.ContainsKey(newField.Name))
                                        Conf!.ConstantsBlock[blockConst].Constants[nameConst].TabularParts[nameTablePart].AppendField(newField);

                                    break;
                                }
                        }

                        break;
                    }
                case "Довідники":
                    {
                        string[] directoryPath = name.Split("/");
                        string directory = directoryPath[0];

                        switch (directoryPath.Length)
                        {
                            case 1:
                                {
                                    if (directory.IndexOf(":") != -1)
                                    {
                                        string[] directoryAndField = directory.Split(":");
                                        string directoryName = directoryAndField[0];
                                        string fieldName = directoryAndField[1];

                                        ConfigurationObjectField newField = Conf!.Directories[directoryName].Fields[fieldName].Copy();
                                        newField.Name += GenerateName.GetNewName();
                                        newField.NameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel!, Conf!.Directories[directoryName].Table, Conf!.Directories[directoryName].Fields);

                                        if (!Conf!.Directories[directoryName].Fields.ContainsKey(newField.Name))
                                            Conf!.Directories[directoryName].AppendField(newField);
                                    }
                                    else
                                    {
                                        ConfigurationDirectories newDirectory = Conf!.Directories[directory].Copy();
                                        newDirectory.Name += GenerateName.GetNewName();
                                        newDirectory.Table = Configuration.GetNewUnigueTableName(Program.Kernel!);

                                        if (!Conf!.Directories.ContainsKey(newDirectory.Name))
                                        {
                                            foreach (ConfigurationObjectTablePart tablePart in newDirectory.TabularParts.Values)
                                                tablePart.Table = Configuration.GetNewUnigueTableName(Program.Kernel!);

                                            Conf!.AppendDirectory(newDirectory);
                                        }
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    string nameTablePart = directoryPath[1];
                                    ConfigurationObjectTablePart newTablePart = Conf!.Directories[directory].TabularParts[nameTablePart].Copy();
                                    newTablePart.Name += GenerateName.GetNewName();
                                    newTablePart.Table = Configuration.GetNewUnigueTableName(Program.Kernel!);

                                    if (!Conf!.Directories[directory].TabularParts.ContainsKey(newTablePart.Name))
                                        Conf!.Directories[directory].AppendTablePart(newTablePart);

                                    break;
                                }
                            case 3:
                                {
                                    string nameTablePart = directoryPath[1];
                                    string nameField = directoryPath[2];

                                    ConfigurationObjectField newField = Conf!.Directories[directory].TabularParts[nameTablePart].Fields[nameField].Copy();
                                    newField.Name += GenerateName.GetNewName();
                                    newField.NameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel!, Conf!.Directories[directory].Table, Conf!.Directories[directory].Fields);

                                    if (!Conf!.Directories[directory].TabularParts[nameTablePart].Fields.ContainsKey(newField.Name))
                                        Conf!.Directories[directory].TabularParts[nameTablePart].AppendField(newField);

                                    break;
                                }
                        }

                        break;
                    }
                case "Документи":
                    {
                        string[] documentPath = name.Split("/");
                        string document = documentPath[0];

                        switch (documentPath.Length)
                        {
                            case 1:
                                {
                                    if (document.IndexOf(":") != -1)
                                    {
                                        string[] documentAndField = document.Split(":");
                                        string documentName = documentAndField[0];
                                        string fieldName = documentAndField[1];

                                        ConfigurationObjectField newField = Conf!.Documents[documentName].Fields[fieldName].Copy();
                                        newField.Name += GenerateName.GetNewName();
                                        newField.NameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel!, Conf!.Documents[documentName].Table, Conf!.Documents[documentName].Fields);

                                        if (!Conf!.Documents[documentName].Fields.ContainsKey(newField.Name))
                                            Conf!.Documents[documentName].AppendField(newField);
                                    }
                                    else
                                    {
                                        ConfigurationDocuments newDocument = Conf!.Documents[document].Copy();
                                        newDocument.Name += GenerateName.GetNewName();
                                        newDocument.Table = Configuration.GetNewUnigueTableName(Program.Kernel!);

                                        if (!Conf!.Documents.ContainsKey(newDocument.Name))
                                        {
                                            foreach (ConfigurationObjectTablePart tablePart in newDocument.TabularParts.Values)
                                                tablePart.Table = Configuration.GetNewUnigueTableName(Program.Kernel!);

                                            Conf!.AppendDocument(newDocument);
                                        }
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    string nameTablePart = documentPath[1];

                                    ConfigurationObjectTablePart newTablePart = Conf!.Documents[document].TabularParts[nameTablePart].Copy();
                                    newTablePart.Name += GenerateName.GetNewName();
                                    newTablePart.Table = Configuration.GetNewUnigueTableName(Program.Kernel!);

                                    if (!Conf!.Documents[document].TabularParts.ContainsKey(newTablePart.Name))
                                        Conf!.Documents[document].AppendTablePart(newTablePart);

                                    break;
                                }
                            case 3:
                                {
                                    string nameTablePart = documentPath[1];
                                    string nameField = documentPath[2];

                                    ConfigurationObjectField newField = Conf!.Documents[document].TabularParts[nameTablePart].Fields[nameField].Copy();
                                    newField.Name += GenerateName.GetNewName();
                                    newField.NameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel!, Conf!.Documents[document].Table, Conf!.Documents[document].Fields);

                                    if (!Conf!.Documents[document].TabularParts[nameTablePart].Fields.ContainsKey(newField.Name))
                                        Conf!.Documents[document].TabularParts[nameTablePart].AppendField(newField);

                                    break;
                                }
                        }

                        break;
                    }
                case "РегістриІнформації":
                    {
                        string[] registerPath = name.Split("/");
                        string register = registerPath[0];

                        switch (registerPath.Length)
                        {
                            case 1:
                                {
                                    ConfigurationRegistersInformation newRegInfo = Conf!.RegistersInformation[register].Copy();
                                    newRegInfo.Name += GenerateName.GetNewName();
                                    newRegInfo.Table = Configuration.GetNewUnigueTableName(Program.Kernel!);

                                    if (!Conf!.RegistersInformation.ContainsKey(newRegInfo.Name))
                                        Conf!.AppendRegistersInformation(newRegInfo);

                                    break;
                                }
                            case 2:
                                {
                                    string[] typeAndField = registerPath[1].Split(":");
                                    string typeName = typeAndField[0];
                                    string fieldName = typeAndField[1];

                                    Dictionary<string, ConfigurationObjectField> AllFields = new Dictionary<string, ConfigurationObjectField>();

                                    foreach (ConfigurationObjectField item in Conf!.RegistersInformation[register].DimensionFields.Values)
                                        AllFields.Add(item.Name, item);

                                    foreach (ConfigurationObjectField item in Conf!.RegistersInformation[register].ResourcesFields.Values)
                                        AllFields.Add(item.Name, item);

                                    foreach (ConfigurationObjectField item in Conf!.RegistersInformation[register].PropertyFields.Values)
                                        AllFields.Add(item.Name, item);

                                    ConfigurationObjectField newField;

                                    if (typeName == "Dimension")
                                        newField = Conf!.RegistersInformation[register].DimensionFields[fieldName].Copy();
                                    else if (typeName == "Resources")
                                        newField = Conf!.RegistersInformation[register].ResourcesFields[fieldName].Copy();
                                    else
                                        newField = Conf!.RegistersInformation[register].PropertyFields[fieldName].Copy();

                                    newField.Name += GenerateName.GetNewName();
                                    newField.NameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel!, Conf!.RegistersInformation[register].Table, AllFields);

                                    if (typeName == "Dimension")
                                    {
                                        if (!Conf!.RegistersInformation[register].DimensionFields.ContainsKey(newField.Name))
                                            Conf!.RegistersInformation[register].AppendDimensionField(newField);
                                    }
                                    else if (typeName == "Resources")
                                    {
                                        if (!Conf!.RegistersInformation[register].ResourcesFields.ContainsKey(newField.Name))
                                            Conf!.RegistersInformation[register].AppendResourcesField(newField);
                                    }
                                    else
                                    {
                                        if (!Conf!.RegistersInformation[register].PropertyFields.ContainsKey(newField.Name))
                                            Conf!.RegistersInformation[register].AppendPropertyField(newField);
                                    }

                                    break;
                                }
                        }

                        break;
                    }
                case "РегістриНакопичення":
                    {
                        string[] registerPath = name.Split("/");
                        string register = registerPath[0];

                        switch (registerPath.Length)
                        {
                            case 1:
                                {
                                    ConfigurationRegistersAccumulation newRegAccum = Conf!.RegistersAccumulation[register].Copy();
                                    newRegAccum.Name += GenerateName.GetNewName();
                                    newRegAccum.Table = Configuration.GetNewUnigueTableName(Program.Kernel!);

                                    if (!Conf!.RegistersAccumulation.ContainsKey(newRegAccum.Name))
                                        Conf!.AppendRegistersAccumulation(newRegAccum);

                                    break;
                                }
                            case 2:
                                {
                                    string[] typeAndField = registerPath[1].Split(":");
                                    string typeName = typeAndField[0];
                                    string fieldName = typeAndField[1];

                                    Dictionary<string, ConfigurationObjectField> AllFields = new Dictionary<string, ConfigurationObjectField>();

                                    foreach (ConfigurationObjectField item in Conf!.RegistersAccumulation[register].DimensionFields.Values)
                                        AllFields.Add(item.Name, item);

                                    foreach (ConfigurationObjectField item in Conf!.RegistersAccumulation[register].ResourcesFields.Values)
                                        AllFields.Add(item.Name, item);

                                    foreach (ConfigurationObjectField item in Conf!.RegistersAccumulation[register].PropertyFields.Values)
                                        AllFields.Add(item.Name, item);

                                    ConfigurationObjectField newField;

                                    if (typeName == "Dimension")
                                        newField = Conf!.RegistersAccumulation[register].DimensionFields[fieldName].Copy();
                                    else if (typeName == "Resources")
                                        newField = Conf!.RegistersAccumulation[register].ResourcesFields[fieldName].Copy();
                                    else
                                        newField = Conf!.RegistersAccumulation[register].PropertyFields[fieldName].Copy();

                                    newField.Name += GenerateName.GetNewName();
                                    newField.NameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel!, Conf!.RegistersAccumulation[register].Table, AllFields);

                                    if (typeName == "Dimension")
                                    {
                                        if (!Conf!.RegistersAccumulation[register].DimensionFields.ContainsKey(newField.Name))
                                            Conf!.RegistersAccumulation[register].AppendDimensionField(newField);
                                    }
                                    else if (typeName == "Resources")
                                    {
                                        if (!Conf!.RegistersAccumulation[register].ResourcesFields.ContainsKey(newField.Name))
                                            Conf!.RegistersAccumulation[register].AppendResourcesField(newField);
                                    }
                                    else
                                    {
                                        if (!Conf!.RegistersAccumulation[register].PropertyFields.ContainsKey(newField.Name))
                                            Conf!.RegistersAccumulation[register].AppendPropertyField(newField);
                                    }

                                    break;
                                }
                        }

                        break;
                    }
                case "Перелічення":
                    {
                        if (name.IndexOf(":") != -1)
                        {
                            string[] enumAndField = name.Split(":");
                            string enumName = enumAndField[0];
                            string fieldName = enumAndField[1];

                            ConfigurationEnumField newField = Conf!.Enums[enumName].Fields[fieldName].Copy(++Conf!.Enums[enumName].SerialNumber);
                            newField.Name += GenerateName.GetNewName();

                            if (!Conf!.Enums[enumName].Fields.ContainsKey(newField.Name))
                                Conf!.Enums[enumName].AppendField(newField);

                        }
                        else
                        {
                            ConfigurationEnums newEnum = Conf!.Enums[name].Copy();
                            newEnum.Name += GenerateName.GetNewName();

                            if (!Conf!.Enums.ContainsKey(newEnum.Name))
                                Conf!.AppendEnum(newEnum);
                        }

                        break;
                    }
            }

            if (TreeRowExpanded.Contains(pathRemove.ToString()))
                TreeRowExpanded.Remove(pathRemove.ToString());

            LoadTreeAsync();
        }

        #endregion

        #region Add Menu

        void OnAddConstantBlock(object? sender, EventArgs args)
        {
            CreateNotebookPage("Блок констант: *", () =>
            {
                PageConstantBlock page = new PageConstantBlock()
                {
                    IsNew = true,
                    GeneralForm = this
                };

                return page;
            });
        }

        void OnAddConstant(object? sender, EventArgs args)
        {
            CreateNotebookPage("Константа *", () =>
            {
                PageConstant page = new PageConstant()
                {
                    IsNew = true,
                    GeneralForm = this
                };

                page.SetValue();

                return page;
            });
        }

        void OnAddDirectory(object? sender, EventArgs args)
        {
            CreateNotebookPage($"Довідник: *", () =>
            {
                PageDirectory page = new PageDirectory()
                {
                    IsNew = true,
                    GeneralForm = this
                };

                page.SetValue();

                return page;
            });
        }

        void OnAddDocument(object? sender, EventArgs args)
        {
            CreateNotebookPage($"Документ: *", () =>
            {
                PageDocument page = new PageDocument()
                {
                    IsNew = true,
                    GeneralForm = this
                };

                page.SetValue();

                return page;
            });
        }

        void OnAddEnum(object? sender, EventArgs args)
        {
            CreateNotebookPage($"Перелічення: *", () =>
            {
                PageEnum page = new PageEnum()
                {
                    IsNew = true,
                    GeneralForm = this
                };

                page.SetValue();

                return page;
            });
        }

        void OnAddRegisterInformation(object? sender, EventArgs args)
        {
            CreateNotebookPage($"Регістер інформації: *", () =>
            {
                PageRegisterInformation page = new PageRegisterInformation()
                {
                    IsNew = true,
                    GeneralForm = this
                };

                page.SetValue();

                return page;
            });
        }

        void OnAddRegisterAccumulation(object? sender, EventArgs args)
        {
            CreateNotebookPage($"Регістер накопичення: *", () =>
            {
                PageRegistersAccumulation page = new PageRegistersAccumulation()
                {
                    IsNew = true,
                    GeneralForm = this
                };

                page.SetValue();

                return page;
            });
        }

        #endregion

    }
}