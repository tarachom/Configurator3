/*
Copyright (C) 2019-2025 TARAKHOMYN YURIY IVANOVYCH
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
using InterfaceGtkLib;

namespace Configurator
{
    public class FormConfigurator : Window
    {
        readonly object loked = new();
        public ConfigurationParam? OpenConfigurationParam { get; set; }
        Configuration Conf { get { return Program.Kernel.Conf; } }

        // Список відритих віток
        List<string> TreeRowExpanded = [];

        // Поточний рядок для позиціонування при перегрузці
        TreePath? SelectionCurrentPath;

        #region Fields

        Paned hPaned;
        TreeView treeConfiguration;
        TreeStore treeStore;
        Notebook topNotebook;
        Statusbar statusBar;

        #endregion

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

                foreach (KeyValuePair<string, ConfigurationTablePart> ConfTablePart in confConstant.TabularParts)
                {
                    string keyTablePart = $"{key}/{ConfTablePart.Value.Name}";

                    TreeIter constantTablePartIter = treeStore.AppendValues(constantTabularPartsIter, ConfTablePart.Value.Name, "", keyTablePart);

                    foreach (KeyValuePair<string, ConfigurationField> ConfTablePartFields in ConfTablePart.Value.Fields)
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
            foreach (KeyValuePair<string, ConfigurationConstantsBlock> ConfConstantsBlock in Conf.ConstantsBlock)
            {
                string key = $"БлокКонстант.{ConfConstantsBlock.Value.BlockName}";

                TreeIter contantsBlockIter = treeStore.AppendValues(rootIter, ConfConstantsBlock.Value.BlockName, "", key);

                foreach (ConfigurationConstants ConfConstants in ConfConstantsBlock.Value.Constants.Values)
                    LoadConstant(contantsBlockIter, ConfConstants);

                IsExpand(contantsBlockIter);
            }
        }

        void LoadDirectory(TreeIter rootIter, ConfigurationDirectories confDirectory)
        {
            string key = $"Довідники.{confDirectory.Name}";

            TreeIter directoryIter = treeStore.AppendValues(rootIter, confDirectory.Name, "", key);

            foreach (KeyValuePair<string, ConfigurationField> ConfFields in confDirectory.Fields)
            {
                string info = GetTypeInfo(ConfFields.Value.Type, ConfFields.Value.Pointer);
                string keyField = $"{key}:{ConfFields.Value.Name}";

                treeStore.AppendValues(directoryIter, ConfFields.Value.Name, info, keyField);
            }

            if (confDirectory.TabularParts.Count > 0)
            {
                TreeIter directoriTabularPartsIter = treeStore.AppendValues(directoryIter, "[ Табличні частини ]");

                foreach (KeyValuePair<string, ConfigurationTablePart> ConfTablePart in confDirectory.TabularParts)
                {
                    string keyTablePart = $"{key}/{ConfTablePart.Value.Name}";

                    TreeIter directoriTablePartIter = treeStore.AppendValues(directoriTabularPartsIter, ConfTablePart.Value.Name, "", keyTablePart);

                    foreach (KeyValuePair<string, ConfigurationField> ConfTablePartFields in ConfTablePart.Value.Fields)
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
            foreach (ConfigurationDirectories ConfDirectory in Conf.Directories.Values)
                LoadDirectory(rootIter, ConfDirectory);
        }

        void LoadDocument(TreeIter rootIter, ConfigurationDocuments confDocument)
        {
            string key = $"Документи.{confDocument.Name}";

            TreeIter documentIter = treeStore.AppendValues(rootIter, confDocument.Name, "", key);

            foreach (KeyValuePair<string, ConfigurationField> ConfFields in confDocument.Fields)
            {
                string info = GetTypeInfo(ConfFields.Value.Type, ConfFields.Value.Pointer);
                string keyField = $"{key}:{ConfFields.Value.Name}";

                treeStore.AppendValues(documentIter, ConfFields.Value.Name, info, keyField);
            }

            if (confDocument.TabularParts.Count > 0)
            {
                TreeIter documentTabularPartsIter = treeStore.AppendValues(documentIter, "[ Табличні частини ]");

                foreach (KeyValuePair<string, ConfigurationTablePart> ConfTablePart in confDocument.TabularParts)
                {
                    string keyTablePart = $"{key}/{ConfTablePart.Value.Name}";

                    TreeIter documentTablePartIter = treeStore.AppendValues(documentTabularPartsIter, ConfTablePart.Value.Name, "", keyTablePart);

                    foreach (KeyValuePair<string, ConfigurationField> ConfTablePartFields in ConfTablePart.Value.Fields)
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
            foreach (ConfigurationDocuments ConfDocuments in Conf.Documents.Values)
                LoadDocument(rootIter, ConfDocuments);
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
            foreach (ConfigurationEnums ConfEnum in Conf.Enums.Values)
                LoadEnum(rootIter, ConfEnum);
        }

        void LoadJournal(TreeIter rootIter, ConfigurationJournals confJournal)
        {
            string key = $"Журнал.{confJournal.Name}";

            TreeIter journalIter = treeStore.AppendValues(rootIter, confJournal.Name, "", key);

            foreach (KeyValuePair<string, ConfigurationJournalField> ConfEnumFields in confJournal.Fields)
            {
                string keyField = $"{key}:{ConfEnumFields.Value.Name}";

                treeStore.AppendValues(journalIter, ConfEnumFields.Value.Name, "", keyField);
            }

            IsExpand(journalIter);
        }

        void LoadJournals(TreeIter rootIter)
        {
            foreach (ConfigurationJournals ConfJournal in Conf.Journals.Values)
                LoadJournal(rootIter, ConfJournal);
        }

        void LoadRegisterInformation(TreeIter rootIter, ConfigurationRegistersInformation confRegisterInformation)
        {
            string key = $"РегістриІнформації.{confRegisterInformation.Name}";

            TreeIter registerInformationIter = treeStore.AppendValues(rootIter, confRegisterInformation.Name, "", key);

            TreeIter dimensionFieldsIter = treeStore.AppendValues(registerInformationIter, "Виміри");

            //Поля вимірів
            foreach (KeyValuePair<string, ConfigurationField> ConfDimensionFields in confRegisterInformation.DimensionFields)
            {
                string info = GetTypeInfo(ConfDimensionFields.Value.Type, ConfDimensionFields.Value.Pointer);
                string keyField = $"{key}/Dimension:{ConfDimensionFields.Value.Name}";

                treeStore.AppendValues(dimensionFieldsIter, ConfDimensionFields.Value.Name, info, keyField);
            }

            IsExpand(dimensionFieldsIter);

            TreeIter resourcesFieldsIter = treeStore.AppendValues(registerInformationIter, "Ресурси");

            //Поля ресурсів
            foreach (KeyValuePair<string, ConfigurationField> ConfResourcesFields in confRegisterInformation.ResourcesFields)
            {
                string info = GetTypeInfo(ConfResourcesFields.Value.Type, ConfResourcesFields.Value.Pointer);
                string keyField = $"{key}/Resources:{ConfResourcesFields.Value.Name}";

                treeStore.AppendValues(resourcesFieldsIter, ConfResourcesFields.Value.Name, info, keyField);
            }

            IsExpand(resourcesFieldsIter);

            TreeIter propertyFieldsIter = treeStore.AppendValues(registerInformationIter, "Поля");

            //Поля реквізитів
            foreach (KeyValuePair<string, ConfigurationField> ConfPropertyFields in confRegisterInformation.PropertyFields)
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
            foreach (ConfigurationRegistersInformation ConfRegistersInformation in Conf.RegistersInformation.Values)
                LoadRegisterInformation(rootIter, ConfRegistersInformation);
        }

        void LoadRegisterAccumulation(TreeIter rootIter, ConfigurationRegistersAccumulation confRegisterAccumulation)
        {
            string key = $"РегістриНакопичення.{confRegisterAccumulation.Name}";

            TreeIter registerAccumulationIter = treeStore.AppendValues(rootIter, confRegisterAccumulation.Name, "", key);

            TreeIter dimensionFieldsIter = treeStore.AppendValues(registerAccumulationIter, "Виміри");

            //Поля вимірів
            foreach (KeyValuePair<string, ConfigurationField> ConfDimensionFields in confRegisterAccumulation.DimensionFields)
            {
                string info = GetTypeInfo(ConfDimensionFields.Value.Type, ConfDimensionFields.Value.Pointer);
                string keyField = $"{key}/Dimension:{ConfDimensionFields.Value.Name}";

                treeStore.AppendValues(dimensionFieldsIter, ConfDimensionFields.Value.Name, info, keyField);
            }

            IsExpand(dimensionFieldsIter);

            TreeIter resourcesFieldsIter = treeStore.AppendValues(registerAccumulationIter, "Ресурси");

            //Поля ресурсів
            foreach (KeyValuePair<string, ConfigurationField> ConfResourcesFields in confRegisterAccumulation.ResourcesFields)
            {
                string info = GetTypeInfo(ConfResourcesFields.Value.Type, ConfResourcesFields.Value.Pointer);
                string keyField = $"{key}/Resources:{ConfResourcesFields.Value.Name}";

                treeStore.AppendValues(resourcesFieldsIter, ConfResourcesFields.Value.Name, info, keyField);
            }

            IsExpand(resourcesFieldsIter);

            TreeIter propertyFieldsIter = treeStore.AppendValues(registerAccumulationIter, "Поля");

            //Поля реквізитів
            foreach (KeyValuePair<string, ConfigurationField> ConfPropertyFields in confRegisterAccumulation.PropertyFields)
            {
                string info = GetTypeInfo(ConfPropertyFields.Value.Type, ConfPropertyFields.Value.Pointer);
                string keyField = $"{key}/Property:{ConfPropertyFields.Value.Name}";

                treeStore.AppendValues(propertyFieldsIter, ConfPropertyFields.Value.Name, info, keyField);
            }

            IsExpand(propertyFieldsIter);

            if (confRegisterAccumulation.TabularParts.Count > 0)
            {
                TreeIter registerAccumulationTabularPartsIter = treeStore.AppendValues(registerAccumulationIter, "[ Таблиці для розрахунків ]");

                foreach (KeyValuePair<string, ConfigurationTablePart> ConfTablePart in confRegisterAccumulation.TabularParts)
                {
                    string keyTablePart = $"{key}/{ConfTablePart.Value.Name}";

                    TreeIter registerAccumulationTablePartIter = treeStore.AppendValues(registerAccumulationTabularPartsIter, ConfTablePart.Value.Name, "", keyTablePart);

                    foreach (KeyValuePair<string, ConfigurationField> ConfTablePartFields in ConfTablePart.Value.Fields)
                    {
                        string info = GetTypeInfo(ConfTablePartFields.Value.Type, ConfTablePartFields.Value.Pointer);
                        string keyField = $"{keyTablePart}/{ConfTablePartFields.Value.Name}";

                        treeStore.AppendValues(registerAccumulationTablePartIter, ConfTablePartFields.Value.Name, info, keyField);
                    }

                    IsExpand(registerAccumulationTablePartIter);
                }

                IsExpand(registerAccumulationTabularPartsIter);
            }

            IsExpand(registerAccumulationIter);
        }

        void LoadRegistersAccumulation(TreeIter rootIter)
        {
            foreach (ConfigurationRegistersAccumulation ConfRegistersAccumulation in Conf.RegistersAccumulation.Values)
                LoadRegisterAccumulation(rootIter, ConfRegistersAccumulation);
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

                        TreeIter journalsIter = treeStore.AppendValues("Журнали");
                        LoadJournals(journalsIter);
                        IsExpand(journalsIter);

                        TreeIter registersInformationIter = treeStore.AppendValues("Регістри відомостей");
                        LoadRegistersInformation(registersInformationIter);
                        IsExpand(registersInformationIter);

                        TreeIter registersAccumulationIter = treeStore.AppendValues("Регістри накопичення");
                        LoadRegistersAccumulation(registersAccumulationIter);
                        IsExpand(registersAccumulationIter);

                        if (SelectionCurrentPath != null)
                            treeConfiguration.SetCursor(SelectionCurrentPath, treeConfiguration.Columns[0], false);
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

        public Dictionary<string, ConfigurationField> GetConstantsAllFields()
        {
            Dictionary<string, ConfigurationField> ConstantsAllFields = [];

            foreach (ConfigurationConstantsBlock block in Conf.ConstantsBlock.Values)
                foreach (ConfigurationConstants constants in block.Constants.Values)
                {
                    string fullName = block.BlockName + "." + constants.Name;
                    ConstantsAllFields.Add(fullName, new ConfigurationField(fullName, fullName, constants.NameInTable, constants.Type, constants.Pointer, constants.Desc));
                }

            return ConstantsAllFields;
        }

        public void SetValue()
        {
            if (OpenConfigurationParam != null)
            {
                statusBar.Halign = Align.Start;
                statusBar.Add(new Label($"Конфігурація: {Conf.Name} ") { UseUnderline = false });
                statusBar.Add(new Separator(Orientation.Vertical));
                statusBar.Add(new Label($" Сервер: {OpenConfigurationParam.DataBaseServer} ") { UseUnderline = false });
                statusBar.Add(new Separator(Orientation.Vertical));
                statusBar.Add(new Label($" База даних: {OpenConfigurationParam.DataBaseBaseName} ") { UseUnderline = false });
                statusBar.ShowAll();
            }

            //Стартова
            {
                PageHome page = new PageHome();
                CreateNotebookPage("Стартова", () => page);
            }
        }

        #endregion

        public FormConfigurator() : base("Конфігуратор")
        {
            SetDefaultSize(1200, 800);
            SetPosition(WindowPosition.Center);

            string ico_file_name = AppContext.BaseDirectory + "images/configurator.ico";

            if (File.Exists(ico_file_name))
                SetDefaultIconFromFile(ico_file_name);

            DeleteEvent += delegate { Application.Quit(); };

            Box vbox = new Box(Orientation.Vertical, 0);
            Add(vbox);

            //MenuBar
            vbox.PackStart(CreateMenuBar(), false, false, 2);

            //Toolbar
            vbox.PackStart(CreateToolbar(), false, false, 0);

            hPaned = new Paned(Orientation.Horizontal) { Position = 200 };

            ScrolledWindow scrollTree = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollTree.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

            treeConfiguration = new TreeView() { BorderWidth = 0, EnableTreeLines = false };
            treeConfiguration.RowActivated += OnRowActivated;
            treeConfiguration.RowExpanded += OnRowExpanded;
            treeConfiguration.RowCollapsed += OnRowCollapsed;

            scrollTree.Add(treeConfiguration);
            treeStore = AddTreeColumn();

            hPaned.Pack1(scrollTree, false, true);

            topNotebook = new Notebook
            {
                Scrollable = true,
                EnablePopup = true,
                BorderWidth = 0,
                ShowBorder = false,
                TabPos = PositionType.Top
            };

            hPaned.Pack2(topNotebook, false, true);

            vbox.PackStart(hPaned, true, true, 0);

            statusBar = new Statusbar();
            vbox.PackStart(statusBar, false, false, 0);

            ShowAll();
        }

        Box CreateMenuBar()
        {
            MenuBar mb = new MenuBar();

            //1
            Menu СonfMenu = new Menu();
            MenuItem configurationItem = new MenuItem("Конфігурація") { Submenu = СonfMenu };

            MenuItem saveConfiguration = new MenuItem("Зберегти конфігурацію");
            saveConfiguration.Activated += OnSaveConfigurationClick;
            СonfMenu.Append(saveConfiguration);

            MenuItem editConfigurationInfo = new MenuItem("Параметри конфігурації");
            editConfigurationInfo.Activated += OnConfigurationInfo;
            СonfMenu.Append(editConfigurationInfo);

            MenuItem editDictTSearch = new MenuItem("Повнотектовий пошук");
            editDictTSearch.Activated += OnDictTSearch;
            СonfMenu.Append(editDictTSearch);

            mb.Append(configurationItem);

            //2
            Menu UsersMenu = new Menu();
            MenuItem usersItem = new MenuItem("Користувачі") { Submenu = UsersMenu };

            MenuItem usersList = new MenuItem("Список користувачів");
            usersList.Activated += OnUsersListClick;
            UsersMenu.Append(usersList);

            mb.Append(usersItem);

            //3
            Menu UploadAndLoadMenu = new Menu();
            MenuItem uploadAndLoadDataMenuItem = new MenuItem("Вигрузка та загрузка") { Submenu = UploadAndLoadMenu };

            MenuItem uploadConfigurationToFile = new MenuItem("Вигрузити конфігурацію в файл");
            uploadConfigurationToFile.Activated += OnUploadConfigurationToFileClick;
            UploadAndLoadMenu.Append(uploadConfigurationToFile);

            MenuItem loadConfigurationFromFile = new MenuItem("Загрузити конфігурацію з файлу");
            loadConfigurationFromFile.Activated += OnLoadConfigurationFromFileClick;
            UploadAndLoadMenu.Append(loadConfigurationFromFile);

            MenuItem uploadAndLoadData = new MenuItem("Вигрузка та загрузка даних");
            uploadAndLoadData.Activated += OnUnloadingAndLoadingData;
            UploadAndLoadMenu.Append(uploadAndLoadData);

            MenuItem maintenanceItem = new MenuItem("Оптимізація таблиць");
            maintenanceItem.Activated += OnMaintenance;
            UploadAndLoadMenu.Append(maintenanceItem);

            MenuItem shemaItem = new MenuItem("Схема бази даних");
            shemaItem.Activated += OnShema;
            UploadAndLoadMenu.Append(shemaItem);

            mb.Append(uploadAndLoadDataMenuItem);

            //4
            Menu AboutMenu = new Menu();
            MenuItem aboutMenuItem = new MenuItem("Про програму") { Submenu = AboutMenu };

            MenuItem aboutMenuInfoItem = new MenuItem("Інформація");
            aboutMenuInfoItem.Activated += OnAboutMenuInfo;
            AboutMenu.Append(aboutMenuInfoItem);

            MenuItem aboutMenuHelpItem = new MenuItem("Допомога");
            //aboutMenuInfoItem.Activated += OnUploadConfigurationToFileClick;
            AboutMenu.Append(aboutMenuHelpItem);

            mb.Append(aboutMenuItem);

            Box vbox = new Box(Orientation.Vertical, 0);
            vbox.PackStart(mb, false, false, 0);

            return vbox;
        }

        Toolbar CreateToolbar()
        {
            Toolbar toolbar = new Toolbar();

            MenuToolButton menuToolButton = new MenuToolButton(new Image(Stock.New, IconSize.Menu), "Додати") { Label = "Додати", IsImportant = true };
            menuToolButton.Clicked += (object? sender, EventArgs arg) => { ((Menu)((MenuToolButton)sender!).Menu).Popup(); };
            menuToolButton.Menu = CreateAddMenu();
            toolbar.Add(menuToolButton);

            ToolButton refreshButton = new ToolButton(new Image(Stock.Refresh, IconSize.Menu), "Обновити") { Label = "Обновити", IsImportant = true };
            refreshButton.Clicked += OnRefreshClick;
            toolbar.Add(refreshButton);

            ToolButton deleteButton = new ToolButton(new Image(Stock.Clear, IconSize.Menu), "Видалити") { Label = "Видалити", IsImportant = true };
            deleteButton.Clicked += OnDeleteClick;
            toolbar.Add(deleteButton);

            ToolButton copyButton = new ToolButton(new Image(Stock.Copy, IconSize.Menu), "Копіювати") { Label = "Копіювати", IsImportant = true };
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

            MenuItem AddJournal = new MenuItem("Журнал");
            AddJournal.Activated += OnAddJournal;
            Menu.Append(AddJournal);

            MenuItem AddRegistersInformation = new MenuItem("Регістр відомостей");
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
                PageSaveConfiguration page = new PageSaveConfiguration() { GeneralForm = this };
                page.SetValue();
                return page;
            });
        }

        void OnConfigurationInfo(object? sender, EventArgs args)
        {
            CreateNotebookPage("Параметри конфігурації", () =>
            {
                PageConfigurationInfo page = new PageConfigurationInfo() { GeneralForm = this };
                page.SetValue();
                return page;
            });
        }

        void OnDictTSearch(object? sender, EventArgs args)
        {
            CreateNotebookPage("Повнотектовий пошук", () =>
            {
                PageDictTSearch page = new PageDictTSearch() { GeneralForm = this };
                page.SetValue();
                return page;
            });
        }

        void OnUploadConfigurationToFileClick(object? sender, EventArgs args)
        {
            string folderSave = "";
            string fileConfName = "Confa_" + Conf.NameSpace + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xml";
            bool saveOk = false;

            FileChooserDialog fc = new FileChooserDialog("Виберіть каталог для вигрузки конфігурації", this,
                FileChooserAction.SelectFolder, "Закрити", ResponseType.Cancel, "Вибрати", ResponseType.Accept);

            if (fc.Run() == (int)ResponseType.Accept)
                if (!string.IsNullOrEmpty(fc.CurrentFolder))
                {
                    string fileConf = System.IO.Path.Combine(fc.CurrentFolder, fileConfName);
                    Configuration.Save(fileConf, Conf);

                    folderSave = fc.CurrentFolder;
                    saveOk = true;
                }

            fc.Dispose();
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
                if (!string.IsNullOrEmpty(fc.Filename))
                {
                    Configuration.Load(fc.Filename, out Configuration openConf);
                    openConf.PathToXmlFileConfiguration = Conf.PathToXmlFileConfiguration;

                    Program.Kernel.Conf = openConf;

                    loadOk = true;
                }

            fc.Dispose();
            fc.Destroy();

            if (loadOk)
                LoadTreeAsync();
        }

        void OnUsersListClick(object? sender, EventArgs args)
        {
            CreateNotebookPage("Список користувачів", () =>
            {
                PageUsersList page = new PageUsersList() { GeneralForm = this };
                page.LoadRecords();
                return page;
            });
        }

        void OnUnloadingAndLoadingData(object? sender, EventArgs args)
        {
            CreateNotebookPage("Вигрузка та загрузка даних", () =>
            {
                PageUnloadingAndLoadingData page = new PageUnloadingAndLoadingData() { GeneralForm = this };
                return page;
            });
        }

        void OnMaintenance(object? sender, EventArgs args)
        {
            CreateNotebookPage("Оптимізація таблиць", () =>
            {
                PageMaintenance page = new PageMaintenance() { GeneralForm = this };
                return page;
            });
        }

        void OnShema(object? sender, EventArgs args)
        {
            CreateNotebookPage("Схема бази даних", () =>
            {
                PageShema page = new PageShema() { GeneralForm = this };
                page.LoadShema();

                return page;
            });
        }

        void OnAboutMenuInfo(object? sender, EventArgs args)
        {
            AboutDialog about = new AboutDialog() { Title = "Конфігуратор" };
            about.ProgramName = "Конфігуратор";
            about.Version = "Версія 3.0";
            about.Copyright = "(c) Тарахомин Юрій Іванович";
            about.Comments = "Проектування бази даних PostgreSQL";
            about.Website = "https://accounting.org.ua";

            string logo_file_name = AppContext.BaseDirectory + "images/logo.jpg";

            if (File.Exists(logo_file_name))
                about.Logo = new Gdk.Pixbuf(logo_file_name);

            about.Run();

            about.Dispose();
            about.Destroy();
        }

        #endregion

        #region Event Tree, ToolBar

        void OnRowActivated(object sender, RowActivatedArgs args)
        {
            if (!treeConfiguration.Selection.GetSelected(out TreeIter iter) || !treeConfiguration.Model.GetIter(out iter, args.Path))
                return;

            string keyComposite = (string)treeConfiguration.Model.GetValue(iter, 2);
            if (string.IsNullOrEmpty(keyComposite) || !keyComposite.Contains('.'))
                return;

            SelectionCurrentPath = treeConfiguration.Model.GetPath(iter);

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
                                ConfConstantsBlock = Conf.ConstantsBlock[name],
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
                                            ConfConstants = Conf.ConstantsBlock[blockConst].Constants[nameConst]
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
                                            TabularParts = Conf.ConstantsBlock[blockConst].Constants[nameConst].TabularParts,
                                            TablePart = Conf.ConstantsBlock[blockConst].Constants[nameConst].TabularParts[nameTablePart],
                                            IsNew = false,
                                            Owner = new OwnerTablePart(false, "Constants", nameConst, blockConst),
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
                                            Fields = Conf.ConstantsBlock[blockConst].Constants[nameConst].TabularParts[nameTablePart].Fields,
                                            Field = Conf.ConstantsBlock[blockConst].Constants[nameConst].TabularParts[nameTablePart].Fields[nameField],
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
                                    if (directory.Contains(':'))
                                    {
                                        string[] directoryAndField = directory.Split(":");
                                        string directoryName = directoryAndField[0];
                                        string fieldName = directoryAndField[1];

                                        CreateNotebookPage($"Поле: {fieldName}", () =>
                                        {
                                            PageField page = new PageField()
                                            {
                                                Fields = Conf.Directories[directoryName].Fields,
                                                Field = Conf.Directories[directoryName].Fields[fieldName],
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
                                                ConfDirectory = Conf.Directories[directory],
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
                                            TabularParts = Conf.Directories[directory].TabularParts,
                                            TablePart = Conf.Directories[directory].TabularParts[nameTablePart],
                                            IsNew = false,
                                            Owner = new OwnerTablePart(true, "Directory", directory)
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
                                            Fields = Conf.Directories[directory].TabularParts[nameTablePart].Fields,
                                            Field = Conf.Directories[directory].TabularParts[nameTablePart].Fields[nameField],
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
                                    if (document.Contains(':'))
                                    {
                                        string[] documentAndField = document.Split(":");
                                        string documentName = documentAndField[0];
                                        string fieldName = documentAndField[1];

                                        CreateNotebookPage($"Поле: {fieldName}", () =>
                                        {
                                            PageField page = new PageField()
                                            {
                                                Fields = Conf.Documents[documentName].Fields,
                                                Field = Conf.Documents[documentName].Fields[fieldName],
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
                                                ConfDocument = Conf.Documents[document],
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
                                            TabularParts = Conf.Documents[document].TabularParts,
                                            TablePart = Conf.Documents[document].TabularParts[nameTablePart],
                                            IsNew = false,
                                            Owner = new OwnerTablePart(true, "Document", document)
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
                                            Fields = Conf.Documents[document].TabularParts[nameTablePart].Fields,
                                            Field = Conf.Documents[document].TabularParts[nameTablePart].Fields[nameField],
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
                                    CreateNotebookPage($"Регістр інформації: {register}", () =>
                                    {
                                        PageRegisterInformation page = new PageRegisterInformation()
                                        {
                                            ConfRegister = Conf.RegistersInformation[register],
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
                                        Dictionary<string, ConfigurationField> AllFields = Configuration.CombineAllFieldForRegister
                                        (
                                            Conf.RegistersInformation[register].DimensionFields.Values,
                                            Conf.RegistersInformation[register].ResourcesFields.Values,
                                            Conf.RegistersInformation[register].PropertyFields.Values
                                        );

                                        Dictionary<string, ConfigurationField> Fields;
                                        ConfigurationField Field;

                                        if (typeName == "Dimension")
                                        {
                                            Fields = Conf.RegistersInformation[register].DimensionFields;
                                            Field = Conf.RegistersInformation[register].DimensionFields[fieldName];
                                        }
                                        else if (typeName == "Resources")
                                        {
                                            Fields = Conf.RegistersInformation[register].ResourcesFields;
                                            Field = Conf.RegistersInformation[register].ResourcesFields[fieldName];
                                        }
                                        else
                                        {
                                            Fields = Conf.RegistersInformation[register].PropertyFields;
                                            Field = Conf.RegistersInformation[register].PropertyFields[fieldName];
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
                                    CreateNotebookPage($"Регістр накопичення: {register}", () =>
                                    {
                                        PageRegisterAccumulation page = new PageRegisterAccumulation()
                                        {
                                            ConfRegister = Conf.RegistersAccumulation[register],
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
                                    if (registerPath[1].Contains(':'))
                                    {
                                        string[] typeAndField = registerPath[1].Split(":");
                                        string typeName = typeAndField[0];
                                        string fieldName = typeAndField[1];

                                        CreateNotebookPage($"Поле: {fieldName}", () =>
                                        {
                                            Dictionary<string, ConfigurationField> AllFields = Configuration.CombineAllFieldForRegister
                                            (
                                                Conf.RegistersAccumulation[register].DimensionFields.Values,
                                                Conf.RegistersAccumulation[register].ResourcesFields.Values,
                                                Conf.RegistersAccumulation[register].PropertyFields.Values
                                            );

                                            Dictionary<string, ConfigurationField> Fields;
                                            ConfigurationField Field;

                                            if (typeName == "Dimension")
                                            {
                                                Fields = Conf.RegistersAccumulation[register].DimensionFields;
                                                Field = Conf.RegistersAccumulation[register].DimensionFields[fieldName];
                                            }
                                            else if (typeName == "Resources")
                                            {
                                                Fields = Conf.RegistersAccumulation[register].ResourcesFields;
                                                Field = Conf.RegistersAccumulation[register].ResourcesFields[fieldName];
                                            }
                                            else
                                            {
                                                Fields = Conf.RegistersAccumulation[register].PropertyFields;
                                                Field = Conf.RegistersAccumulation[register].PropertyFields[fieldName];
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
                                    }
                                    else
                                    {
                                        string nameTablePart = registerPath[1];

                                        CreateNotebookPage($"Таблична частина: {nameTablePart}", () =>
                                        {
                                            PageTablePart page = new PageTablePart()
                                            {
                                                GeneralForm = this,
                                                TabularParts = Conf.RegistersAccumulation[register].TabularParts,
                                                TablePart = Conf.RegistersAccumulation[register].TabularParts[nameTablePart],
                                                IsNew = false,
                                                Owner = new OwnerTablePart(false, "RegistersAccumulation", register)
                                            };

                                            page.SetValue();

                                            return page;
                                        });
                                        break;
                                    }

                                    break;
                                }
                            case 3:
                                {
                                    string nameTablePart = registerPath[1];
                                    string fieldName = registerPath[2];

                                    CreateNotebookPage($"Поле: {fieldName}", () =>
                                    {
                                        PageField page = new PageField()
                                        {
                                            Fields = Conf.RegistersAccumulation[register].TabularParts[nameTablePart].Fields,
                                            Field = Conf.RegistersAccumulation[register].TabularParts[nameTablePart].Fields[fieldName],
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
                        if (name.Contains(':'))
                        {
                            string[] enumAndField = name.Split(":");
                            string enumName = enumAndField[0];
                            string fieldName = enumAndField[1];

                            CreateNotebookPage($"Поле: {fieldName}", () =>
                            {
                                PageEnumField page = new PageEnumField()
                                {
                                    Fields = Conf.Enums[enumName].Fields,
                                    Field = Conf.Enums[enumName].Fields[fieldName],
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
                                    ConfEnum = Conf.Enums[name],
                                    IsNew = false,
                                    GeneralForm = this
                                };

                                page.SetValue();

                                return page;
                            });
                        }
                        break;
                    }
                case "Журнал":
                    {
                        if (name.Contains(':'))
                        {
                            string[] journalAndField = name.Split(":");
                            string journalName = journalAndField[0];
                            string fieldName = journalAndField[1];

                            CreateNotebookPage($"Поле: {fieldName}", () =>
                            {
                                PageJournalField page = new PageJournalField()
                                {
                                    Fields = Conf.Journals[journalName].Fields,
                                    Field = Conf.Journals[journalName].Fields[fieldName],
                                    IsNew = false,
                                    GeneralForm = this
                                };

                                page.SetValue();

                                return page;
                            });
                        }
                        else
                        {
                            CreateNotebookPage($"Журнал: {name}", () =>
                            {
                                PageJournal page = new PageJournal()
                                {
                                    ConfJournals = Conf.Journals[name],
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
            if (!treeConfiguration.Selection.GetSelected(out TreeIter iter))
                return;

            TreePath pathRemove = treeConfiguration.Model.GetPath(iter);
            string keyComposite = (string)treeConfiguration.Model.GetValue(iter, 2);

            if (string.IsNullOrEmpty(keyComposite) || !keyComposite.Contains('.'))
                return;

            bool reloadTree = false;

            string[] keySplit = keyComposite.Split(".");
            string block = keySplit[0];
            string name = keySplit[1];

            switch (block)
            {
                case "БлокКонстант":
                    {
                        if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                        {
                            Conf.ConstantsBlock.Remove(name);
                            reloadTree = true;
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
                                    if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                    {
                                        Conf.ConstantsBlock[blockConst].Constants.Remove(nameConst);
                                        reloadTree = true;
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    string nameTablePart = blockAndName[2];

                                    if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                    {
                                        Conf.ConstantsBlock[blockConst].Constants[nameConst].TabularParts.Remove(nameTablePart);
                                        reloadTree = true;
                                    }
                                    break;
                                }
                            case 4:
                                {
                                    string nameTablePart = blockAndName[2];
                                    string nameField = blockAndName[3];

                                    if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                    {
                                        Conf.ConstantsBlock[blockConst].Constants[nameConst].TabularParts[nameTablePart].Fields.Remove(nameField);
                                        reloadTree = true;
                                    }
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
                                    if (directory.Contains(':'))
                                    {
                                        string[] directoryAndField = directory.Split(":");
                                        string directoryName = directoryAndField[0];
                                        string fieldName = directoryAndField[1];

                                        if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                        {
                                            Conf.Directories[directoryName].Fields.Remove(fieldName);
                                            reloadTree = true;
                                        }
                                    }
                                    else
                                    {
                                        List<string> ListPointers = Conf.SearchForPointers("Довідники." + directory);

                                        if (ListPointers.Count == 0)
                                        {
                                            if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                            {
                                                Conf.Directories.Remove(directory);
                                                reloadTree = true;
                                            }
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
                                    {
                                        Conf.Directories[directory].TabularParts.Remove(nameTablePart);
                                        reloadTree = true;
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    string nameTablePart = directoryPath[1];
                                    string nameField = directoryPath[2];

                                    if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                    {
                                        Conf.Directories[directory].TabularParts[nameTablePart].Fields.Remove(nameField);
                                        reloadTree = true;
                                    }
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
                                    if (document.Contains(':'))
                                    {
                                        string[] documentAndField = document.Split(":");
                                        string documentName = documentAndField[0];
                                        string fieldName = documentAndField[1];

                                        if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                        {
                                            Conf.Documents[documentName].Fields.Remove(fieldName);
                                            reloadTree = true;
                                        }
                                    }
                                    else
                                    {
                                        List<string> ListPointers = Conf.SearchForPointers("Документи." + document);

                                        if (ListPointers.Count == 0)
                                        {
                                            if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                            {
                                                Conf.Documents.Remove(document);
                                                reloadTree = true;
                                            }
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
                                    {
                                        Conf.Documents[document].TabularParts.Remove(nameTablePart);
                                        reloadTree = true;
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    string nameTablePart = documentPath[1];
                                    string nameField = documentPath[2];

                                    if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                    {
                                        Conf.Documents[document].TabularParts[nameTablePart].Fields.Remove(nameField);
                                        reloadTree = true;
                                    }
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
                                    {
                                        Conf.RegistersInformation.Remove(register);
                                        reloadTree = true;
                                    }
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
                                            Conf.RegistersInformation[register].DimensionFields.Remove(fieldName);
                                        else if (typeName == "Resources")
                                            Conf.RegistersInformation[register].ResourcesFields.Remove(fieldName);
                                        else
                                            Conf.RegistersInformation[register].PropertyFields.Remove(fieldName);

                                        reloadTree = true;
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
                                    {
                                        Conf.RegistersAccumulation.Remove(register);
                                        reloadTree = true;
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    if (registerPath[1].Contains(':'))
                                    {
                                        string[] typeAndField = registerPath[1].Split(":");
                                        string typeName = typeAndField[0];
                                        string fieldName = typeAndField[1];

                                        if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                        {
                                            if (typeName == "Dimension")
                                                Conf.RegistersAccumulation[register].DimensionFields.Remove(fieldName);
                                            else if (typeName == "Resources")
                                                Conf.RegistersAccumulation[register].ResourcesFields.Remove(fieldName);
                                            else
                                                Conf.RegistersAccumulation[register].PropertyFields.Remove(fieldName);

                                            reloadTree = true;
                                        }
                                    }
                                    else
                                    {
                                        string nameTablePart = registerPath[1];

                                        if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                        {
                                            Conf.RegistersAccumulation[register].TabularParts.Remove(nameTablePart);
                                            reloadTree = true;
                                        }
                                    }

                                    break;
                                }
                            case 3:
                                {
                                    string nameTablePart = registerPath[1];
                                    string nameField = registerPath[2];

                                    if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                    {
                                        Conf.RegistersAccumulation[register].TabularParts[nameTablePart].Fields.Remove(nameField);
                                        reloadTree = true;
                                    }

                                    break;
                                }
                        }

                        break;
                    }
                case "Перелічення":
                    {
                        if (name.Contains(':'))
                        {
                            string[] enumAndField = name.Split(":");
                            string enumName = enumAndField[0];
                            string fieldName = enumAndField[1];

                            if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                            {
                                Conf.Enums[enumName].Fields.Remove(fieldName);
                                reloadTree = true;
                            }
                        }
                        else
                        {
                            List<string> ListPointers = Conf.SearchForPointersEnum("Перелічення." + name);

                            if (ListPointers.Count == 0)
                            {
                                if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                                {
                                    Conf.Enums.Remove(name);
                                    reloadTree = true;
                                }
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
                case "Журнал":
                    {
                        if (name.Contains(':'))
                        {
                            string[] journalAndField = name.Split(":");
                            string journalName = journalAndField[0];
                            string fieldName = journalAndField[1];

                            if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                            {
                                Conf.Journals[journalName].Fields.Remove(fieldName);
                                reloadTree = true;
                            }
                        }
                        else
                        {
                            if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                            {
                                Conf.Journals.Remove(name);
                                reloadTree = true;
                            }
                        }

                        break;
                    }
            }

            if (reloadTree)
            {
                if (TreeRowExpanded.Contains(pathRemove.ToString()))
                    TreeRowExpanded.Remove(pathRemove.ToString());

                LoadTreeAsync();
            }
        }

        async void OnCopyClick(object? sender, EventArgs args)
        {
            if (!treeConfiguration.Selection.GetSelected(out TreeIter iter))
                return;

            TreePath pathRemove = treeConfiguration.Model.GetPath(iter);
            string keyComposite = (string)treeConfiguration.Model.GetValue(iter, 2);

            if (string.IsNullOrEmpty(keyComposite) || !keyComposite.Contains('.'))
                return;

            string[] keySplit = keyComposite.Split(".");
            string block = keySplit[0];
            string name = keySplit[1];

            switch (block)
            {
                case "БлокКонстант":
                    {
                        ConfigurationConstantsBlock newConstantBlock = Conf.ConstantsBlock[name].Copy();
                        newConstantBlock.BlockName += GenerateName.GetNewName();

                        Dictionary<string, ConfigurationField> AllFields = GetConstantsAllFields();

                        if (!Conf.ConstantsBlock.ContainsKey(newConstantBlock.BlockName))
                        {
                            foreach (ConfigurationConstants itemConst in newConstantBlock.Constants.Values)
                            {
                                itemConst.NameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, "tab_constants", AllFields);

                                foreach (ConfigurationTablePart tablePart in itemConst.TabularParts.Values)
                                    tablePart.Table = await Configuration.GetNewUnigueTableName(Program.Kernel);
                            }

                            Conf.AppendConstantsBlock(newConstantBlock);
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
                                    ConfigurationConstants newConstant = Conf.ConstantsBlock[blockConst].Constants[nameConst].Copy();
                                    newConstant.Name += GenerateName.GetNewName();
                                    newConstant.NameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, "tab_constants", GetConstantsAllFields());

                                    if (!Conf.ConstantsBlock[blockConst].Constants.ContainsKey(newConstant.Name))
                                    {
                                        foreach (ConfigurationTablePart tablePart in newConstant.TabularParts.Values)
                                            tablePart.Table = await Configuration.GetNewUnigueTableName(Program.Kernel);

                                        Conf.ConstantsBlock[blockConst].AppendConstant(newConstant);
                                    }

                                    break;
                                }
                            case 3:
                                {
                                    string nameTablePart = blockAndName[2];
                                    ConfigurationTablePart newTablePart = Conf.ConstantsBlock[blockConst].Constants[nameConst].TabularParts[nameTablePart].Copy();
                                    newTablePart.Name += GenerateName.GetNewName();
                                    newTablePart.Table = await Configuration.GetNewUnigueTableName(Program.Kernel);

                                    if (!Conf.ConstantsBlock[blockConst].Constants[nameConst].TabularParts.ContainsKey(newTablePart.Name))
                                        Conf.ConstantsBlock[blockConst].Constants[nameConst].AppendTablePart(newTablePart);

                                    break;
                                }
                            case 4:
                                {
                                    string nameTablePart = blockAndName[2];
                                    string nameField = blockAndName[3];

                                    ConfigurationField newField = Conf.ConstantsBlock[blockConst].Constants[nameConst].TabularParts[nameTablePart].Fields[nameField].Copy();
                                    newField.Name += GenerateName.GetNewName();
                                    newField.NameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel,
                                        Conf.ConstantsBlock[blockConst].Constants[nameConst].TabularParts[nameTablePart].Table,
                                        Conf.ConstantsBlock[blockConst].Constants[nameConst].TabularParts[nameTablePart].Fields);

                                    if (!Conf.ConstantsBlock[blockConst].Constants[nameConst].TabularParts[nameTablePart].Fields.ContainsKey(newField.Name))
                                        Conf.ConstantsBlock[blockConst].Constants[nameConst].TabularParts[nameTablePart].AppendField(newField);

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
                                    if (directory.Contains(':'))
                                    {
                                        string[] directoryAndField = directory.Split(":");
                                        string directoryName = directoryAndField[0];
                                        string fieldName = directoryAndField[1];

                                        ConfigurationField newField = Conf.Directories[directoryName].Fields[fieldName].Copy();
                                        newField.Name += GenerateName.GetNewName();
                                        newField.NameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, Conf.Directories[directoryName].Table, Conf.Directories[directoryName].Fields);

                                        if (!Conf.Directories[directoryName].Fields.ContainsKey(newField.Name))
                                            Conf.Directories[directoryName].AppendField(newField);
                                    }
                                    else
                                    {
                                        ConfigurationDirectories newDirectory = Conf.Directories[directory].Copy();
                                        newDirectory.Name += GenerateName.GetNewName();
                                        newDirectory.Table = await Configuration.GetNewUnigueTableName(Program.Kernel);

                                        if (!Conf.Directories.ContainsKey(newDirectory.Name))
                                        {
                                            foreach (ConfigurationTablePart tablePart in newDirectory.TabularParts.Values)
                                                tablePart.Table = await Configuration.GetNewUnigueTableName(Program.Kernel);

                                            Conf.AppendDirectory(newDirectory);
                                        }
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    string nameTablePart = directoryPath[1];
                                    ConfigurationTablePart newTablePart = Conf.Directories[directory].TabularParts[nameTablePart].Copy();
                                    newTablePart.Name += GenerateName.GetNewName();
                                    newTablePart.Table = await Configuration.GetNewUnigueTableName(Program.Kernel);

                                    if (!Conf.Directories[directory].TabularParts.ContainsKey(newTablePart.Name))
                                        Conf.Directories[directory].AppendTablePart(newTablePart);

                                    break;
                                }
                            case 3:
                                {
                                    string nameTablePart = directoryPath[1];
                                    string nameField = directoryPath[2];

                                    ConfigurationField newField = Conf.Directories[directory].TabularParts[nameTablePart].Fields[nameField].Copy();
                                    newField.Name += GenerateName.GetNewName();
                                    newField.NameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel,
                                        Conf.Directories[directory].TabularParts[nameTablePart].Table,
                                        Conf.Directories[directory].TabularParts[nameTablePart].Fields);

                                    if (!Conf.Directories[directory].TabularParts[nameTablePart].Fields.ContainsKey(newField.Name))
                                        Conf.Directories[directory].TabularParts[nameTablePart].AppendField(newField);

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
                                    if (document.Contains(':'))
                                    {
                                        string[] documentAndField = document.Split(":");
                                        string documentName = documentAndField[0];
                                        string fieldName = documentAndField[1];

                                        ConfigurationField newField = Conf.Documents[documentName].Fields[fieldName].Copy();
                                        newField.Name += GenerateName.GetNewName();
                                        newField.NameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, Conf.Documents[documentName].Table, Conf.Documents[documentName].Fields);

                                        if (!Conf.Documents[documentName].Fields.ContainsKey(newField.Name))
                                            Conf.Documents[documentName].AppendField(newField);
                                    }
                                    else
                                    {
                                        ConfigurationDocuments newDocument = Conf.Documents[document].Copy();
                                        newDocument.Name += GenerateName.GetNewName();
                                        newDocument.Table = await Configuration.GetNewUnigueTableName(Program.Kernel);

                                        if (!Conf.Documents.ContainsKey(newDocument.Name))
                                        {
                                            foreach (ConfigurationTablePart tablePart in newDocument.TabularParts.Values)
                                                tablePart.Table = await Configuration.GetNewUnigueTableName(Program.Kernel);

                                            Conf.AppendDocument(newDocument);
                                        }
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    string nameTablePart = documentPath[1];

                                    ConfigurationTablePart newTablePart = Conf.Documents[document].TabularParts[nameTablePart].Copy();
                                    newTablePart.Name += GenerateName.GetNewName();
                                    newTablePart.Table = await Configuration.GetNewUnigueTableName(Program.Kernel);

                                    if (!Conf.Documents[document].TabularParts.ContainsKey(newTablePart.Name))
                                        Conf.Documents[document].AppendTablePart(newTablePart);

                                    break;
                                }
                            case 3:
                                {
                                    string nameTablePart = documentPath[1];
                                    string nameField = documentPath[2];

                                    ConfigurationField newField = Conf.Documents[document].TabularParts[nameTablePart].Fields[nameField].Copy();
                                    newField.Name += GenerateName.GetNewName();
                                    newField.NameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel,
                                        Conf.Documents[document].TabularParts[nameTablePart].Table,
                                        Conf.Documents[document].TabularParts[nameTablePart].Fields);

                                    if (!Conf.Documents[document].TabularParts[nameTablePart].Fields.ContainsKey(newField.Name))
                                        Conf.Documents[document].TabularParts[nameTablePart].AppendField(newField);

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
                                    ConfigurationRegistersInformation newRegInfo = Conf.RegistersInformation[register].Copy();
                                    newRegInfo.Name += GenerateName.GetNewName();
                                    newRegInfo.Table = await Configuration.GetNewUnigueTableName(Program.Kernel);

                                    if (!Conf.RegistersInformation.ContainsKey(newRegInfo.Name))
                                        Conf.AppendRegistersInformation(newRegInfo);

                                    break;
                                }
                            case 2:
                                {
                                    string[] typeAndField = registerPath[1].Split(":");
                                    string typeName = typeAndField[0];
                                    string fieldName = typeAndField[1];

                                    Dictionary<string, ConfigurationField> AllFields = Configuration.CombineAllFieldForRegister
                                    (
                                        Conf.RegistersInformation[register].DimensionFields.Values,
                                        Conf.RegistersInformation[register].ResourcesFields.Values,
                                        Conf.RegistersInformation[register].PropertyFields.Values
                                    );

                                    ConfigurationField newField;

                                    if (typeName == "Dimension")
                                        newField = Conf.RegistersInformation[register].DimensionFields[fieldName].Copy();
                                    else if (typeName == "Resources")
                                        newField = Conf.RegistersInformation[register].ResourcesFields[fieldName].Copy();
                                    else
                                        newField = Conf.RegistersInformation[register].PropertyFields[fieldName].Copy();

                                    newField.Name += GenerateName.GetNewName();
                                    newField.NameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, Conf.RegistersInformation[register].Table, AllFields);

                                    if (typeName == "Dimension")
                                    {
                                        if (!Conf.RegistersInformation[register].DimensionFields.ContainsKey(newField.Name))
                                            Conf.RegistersInformation[register].AppendDimensionField(newField);
                                    }
                                    else if (typeName == "Resources")
                                    {
                                        if (!Conf.RegistersInformation[register].ResourcesFields.ContainsKey(newField.Name))
                                            Conf.RegistersInformation[register].AppendResourcesField(newField);
                                    }
                                    else
                                    {
                                        if (!Conf.RegistersInformation[register].PropertyFields.ContainsKey(newField.Name))
                                            Conf.RegistersInformation[register].AppendPropertyField(newField);
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
                                    ConfigurationRegistersAccumulation newRegAccum = Conf.RegistersAccumulation[register].Copy();
                                    newRegAccum.Name += GenerateName.GetNewName();
                                    newRegAccum.Table = await Configuration.GetNewUnigueTableName(Program.Kernel);

                                    if (!Conf.RegistersAccumulation.ContainsKey(newRegAccum.Name))
                                    {
                                        foreach (ConfigurationTablePart tablePart in newRegAccum.TabularParts.Values)
                                            tablePart.Table = await Configuration.GetNewUnigueTableName(Program.Kernel);

                                        Conf.AppendRegistersAccumulation(newRegAccum);
                                    }

                                    break;
                                }
                            case 2:
                                {
                                    if (registerPath[1].Contains(':'))
                                    {
                                        string[] typeAndField = registerPath[1].Split(":");
                                        string typeName = typeAndField[0];
                                        string fieldName = typeAndField[1];

                                        Dictionary<string, ConfigurationField> AllFields = Configuration.CombineAllFieldForRegister
                                        (
                                            Conf.RegistersInformation[register].DimensionFields.Values,
                                            Conf.RegistersInformation[register].ResourcesFields.Values,
                                            Conf.RegistersInformation[register].PropertyFields.Values
                                        );

                                        ConfigurationField newField;

                                        if (typeName == "Dimension")
                                            newField = Conf.RegistersAccumulation[register].DimensionFields[fieldName].Copy();
                                        else if (typeName == "Resources")
                                            newField = Conf.RegistersAccumulation[register].ResourcesFields[fieldName].Copy();
                                        else
                                            newField = Conf.RegistersAccumulation[register].PropertyFields[fieldName].Copy();

                                        newField.Name += GenerateName.GetNewName();
                                        newField.NameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, Conf.RegistersAccumulation[register].Table, AllFields);

                                        if (typeName == "Dimension")
                                        {
                                            if (!Conf.RegistersAccumulation[register].DimensionFields.ContainsKey(newField.Name))
                                                Conf.RegistersAccumulation[register].AppendDimensionField(newField);
                                        }
                                        else if (typeName == "Resources")
                                        {
                                            if (!Conf.RegistersAccumulation[register].ResourcesFields.ContainsKey(newField.Name))
                                                Conf.RegistersAccumulation[register].AppendResourcesField(newField);
                                        }
                                        else
                                        {
                                            if (!Conf.RegistersAccumulation[register].PropertyFields.ContainsKey(newField.Name))
                                                Conf.RegistersAccumulation[register].AppendPropertyField(newField);
                                        }
                                    }
                                    else
                                    {
                                        string nameTablePart = registerPath[1];

                                        ConfigurationTablePart newTablePart = Conf.RegistersAccumulation[register].TabularParts[nameTablePart].Copy();
                                        newTablePart.Name += GenerateName.GetNewName();
                                        newTablePart.Table = await Configuration.GetNewUnigueTableName(Program.Kernel);

                                        if (!Conf.RegistersAccumulation[register].TabularParts.ContainsKey(newTablePart.Name))
                                            Conf.RegistersAccumulation[register].AppendTablePart(newTablePart);
                                    }

                                    break;
                                }
                            case 3:
                                {
                                    string nameTablePart = registerPath[1];
                                    string nameField = registerPath[2];

                                    ConfigurationField newField = Conf.RegistersAccumulation[register].TabularParts[nameTablePart].Fields[nameField].Copy();
                                    newField.Name += GenerateName.GetNewName();
                                    newField.NameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel,
                                        Conf.RegistersAccumulation[register].TabularParts[nameTablePart].Table,
                                        Conf.RegistersAccumulation[register].TabularParts[nameTablePart].Fields);

                                    if (!Conf.RegistersAccumulation[register].TabularParts[nameTablePart].Fields.ContainsKey(newField.Name))
                                        Conf.RegistersAccumulation[register].TabularParts[nameTablePart].AppendField(newField);

                                    break;
                                }
                        }

                        break;
                    }
                case "Перелічення":
                    {
                        if (name.Contains(':'))
                        {
                            string[] enumAndField = name.Split(":");
                            string enumName = enumAndField[0];
                            string fieldName = enumAndField[1];

                            ConfigurationEnumField newField = Conf.Enums[enumName].Fields[fieldName].Copy(++Conf.Enums[enumName].SerialNumber);
                            newField.Name += GenerateName.GetNewName();

                            if (!Conf.Enums[enumName].Fields.ContainsKey(newField.Name))
                                Conf.Enums[enumName].AppendField(newField);

                        }
                        else
                        {
                            ConfigurationEnums newEnum = Conf.Enums[name].Copy();
                            newEnum.Name += GenerateName.GetNewName();

                            if (!Conf.Enums.ContainsKey(newEnum.Name))
                                Conf.AppendEnum(newEnum);
                        }

                        break;
                    }
                case "Журнал":
                    {
                        if (name.Contains(':'))
                        {
                            string[] journalAndField = name.Split(":");
                            string journalName = journalAndField[0];
                            string fieldName = journalAndField[1];

                            ConfigurationJournalField newField = Conf.Journals[journalName].Fields[fieldName].Copy();
                            newField.Name += GenerateName.GetNewName();

                            if (!Conf.Journals[journalName].Fields.ContainsKey(newField.Name))
                                Conf.Journals[journalName].AppendField(newField);
                        }
                        else
                        {
                            ConfigurationJournals newJournal = Conf.Journals[name].Copy();
                            newJournal.Name += GenerateName.GetNewName();

                            if (!Conf.Journals.ContainsKey(newJournal.Name))
                                Conf.AppendJournal(newJournal);
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

        void OnAddJournal(object? sender, EventArgs args)
        {
            CreateNotebookPage($"Журнал: *", () =>
            {
                PageJournal page = new PageJournal()
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
            CreateNotebookPage($"Регістр відомостей: *", () =>
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
            CreateNotebookPage($"Регістр накопичення: *", () =>
            {
                PageRegisterAccumulation page = new PageRegisterAccumulation()
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