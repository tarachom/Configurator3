using Gtk;
using System;
using System.IO;

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
            string key = $"Константи.{confConstant.Name}";

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
                }
            }
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
            }
        }

        void LoadDirectory(TreeIter rootIter, ConfigurationDirectories confDirectory)
        {
            TreeIter directoryIter = treeStore.AppendValues(rootIter, confDirectory.Name);

            foreach (KeyValuePair<string, ConfigurationObjectField> ConfFields in confDirectory.Fields)
            {
                string info = GetTypeInfo(ConfFields.Value.Type, ConfFields.Value.Pointer);
                TreeIter fieldIter = treeStore.AppendValues(directoryIter, ConfFields.Value.Name, info);
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
                        TreeIter fieldIter = treeStore.AppendValues(directoriTablePartIter, ConfTablePartFields.Value.Name, info);
                    }
                }
            }
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
                TreeIter fieldIter = treeStore.AppendValues(documentIter, ConfFields.Value.Name, info);
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
                        TreeIter fieldIter = treeStore.AppendValues(documentTablePartIter, ConfTablePartFields.Value.Name, info);
                    }
                }
            }
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
            {
                TreeIter enumFieldIter = treeStore.AppendValues(rootIter, ConfEnumFields.Value.Name);
            }
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

            TreeIter resourcesFieldsIter = treeStore.AppendValues(registerInformationIter, "Ресурси");

            //Поля ресурсів
            foreach (KeyValuePair<string, ConfigurationObjectField> ConfResourcesFields in confRegisterInformation.ResourcesFields)
            {
                string info = GetTypeInfo(ConfResourcesFields.Value.Type, ConfResourcesFields.Value.Pointer);
                treeStore.AppendValues(resourcesFieldsIter, ConfResourcesFields.Value.Name, info);
            }

            TreeIter propertyFieldsIter = treeStore.AppendValues(registerInformationIter, "Поля");

            //Поля реквізитів
            foreach (KeyValuePair<string, ConfigurationObjectField> ConfPropertyFields in confRegisterInformation.PropertyFields)
            {
                string info = GetTypeInfo(ConfPropertyFields.Value.Type, ConfPropertyFields.Value.Pointer);
                treeStore.AppendValues(propertyFieldsIter, ConfPropertyFields.Value.Name, info);
            }
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

            TreeIter resourcesFieldsIter = treeStore.AppendValues(registerAccumulationIter, "Ресурси");

            //Поля ресурсів
            foreach (KeyValuePair<string, ConfigurationObjectField> ConfResourcesFields in confRegisterAccumulation.ResourcesFields)
            {
                string info = GetTypeInfo(ConfResourcesFields.Value.Type, ConfResourcesFields.Value.Pointer);
                TreeIter fieldIter = treeStore.AppendValues(resourcesFieldsIter, ConfResourcesFields.Value.Name, info);
            }

            TreeIter propertyFieldsIter = treeStore.AppendValues(registerAccumulationIter, "Поля");

            //Поля реквізитів
            foreach (KeyValuePair<string, ConfigurationObjectField> ConfPropertyFields in confRegisterAccumulation.PropertyFields)
            {
                string info = GetTypeInfo(ConfPropertyFields.Value.Type, ConfPropertyFields.Value.Pointer);
                TreeIter fieldIter = treeStore.AppendValues(propertyFieldsIter, ConfPropertyFields.Value.Name, info);
            }
        }

        void LoadRegistersAccumulation(TreeIter rootIter)
        {
            foreach (ConfigurationRegistersAccumulation ConfRegistersAccumulation in Conf!.RegistersAccumulation.Values)
            {
                LoadRegisterAccumulation(rootIter, ConfRegistersAccumulation);
            }
        }

        void LoadTree()
        {
            treeStore.Clear();

            TreeIter contantsIter = treeStore.AppendValues("Константи");

            LoadConstants(contantsIter);

            TreeIter directoriesIter = treeStore.AppendValues("Довідники");

            LoadDirectories(directoriesIter);

            TreeIter documentsIter = treeStore.AppendValues("Документи");

            LoadDocuments(documentsIter);

            TreeIter enumsIter = treeStore.AppendValues("Перелічення");

            LoadEnums(enumsIter);

            TreeIter registersInformationIter = treeStore.AppendValues("Регістри відомостей");

            LoadRegistersInformation(registersInformationIter);

            TreeIter registersAccumulationIter = treeStore.AppendValues("Регістри накопичення");

            LoadRegistersAccumulation(registersAccumulationIter);
        }

        void LoadConf()
        {
            Thread thread = new Thread(new ThreadStart(LoadTree));
            thread.Start();
        }

        void AddTreeColumn()
        {
            treeStore = new TreeStore(typeof(string), typeof(string), typeof(string));

            treeConfiguration.AppendColumn(new TreeViewColumn("Конфігурація", new CellRendererText(), "text", 0));
            treeConfiguration.AppendColumn(new TreeViewColumn("Тип", new CellRendererText(), "text", 1));
            treeConfiguration.AppendColumn(new TreeViewColumn("Ключ", new CellRendererText(), "text", 2) { Visible = false });
            treeConfiguration.Model = treeStore;
        }

        #endregion

        public FormConfigurator() : base("Конфігуратор")
        {
            SetDefaultSize(1000, 600);
            SetPosition(WindowPosition.Center);
            SetDefaultIconFromFile("configurator.ico");

            DeleteEvent += delegate { Application.Quit(); };

            hPaned = new HPaned();
            hPaned.Position = 800;

            ScrolledWindow scrollTree = new ScrolledWindow() { ShadowType = ShadowType.In };
            scrollTree.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

            treeConfiguration = new TreeView();
            treeConfiguration.RowActivated += OnRowActivated;

            AddTreeColumn();

            scrollTree.Add(treeConfiguration);

            hPaned.Pack1(scrollTree, false, true);

            topNotebook = new Notebook() { BorderWidth = 0, ShowBorder = false };
            topNotebook.TabPos = PositionType.Top;

            CreateTopNotebookPages();

            hPaned.Pack2(topNotebook, false, true);

            Add(hPaned);
            ShowAll();

            LoadConf();
        }

        void CreateTopNotebookPages()
        {
            ScrolledWindow scroll = new ScrolledWindow() { ShadowType = ShadowType.In };
            scroll.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

            int numPage = topNotebook.AppendPage(scroll, new Label { Text = "Ok", Expand = false, Halign = Align.End });

        }

        void OnRowActivated(object sender, RowActivatedArgs args)
        {
            Console.WriteLine(args.Path + " - " + args.Column);

            TreeIter iter;
            treeConfiguration.Selection.GetSelected(out iter);

            if (treeConfiguration.Model.GetIter(out iter, args.Path))
            {
                string[] key = ((string)treeConfiguration.Model.GetValue(iter, 2)).Split(".");
                if (key.Length == 2)
                {
                    switch (key[0])
                    {
                        case "Константи":
                            {
                                Console.WriteLine(key[1]);
                                break;
                            }
                    }
                }
            }
        }

    }
}