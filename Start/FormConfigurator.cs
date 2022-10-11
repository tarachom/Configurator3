using Gtk;
using System;
using System.IO;

using AccountingSoftware;

namespace Configurator
{
    class FormConfigurator : Window
    {
        public ConfigurationParam? OpenConfigurationParam { get; set; }

        public Configuration? Conf
        {
            get
            {
                return Program.Kernel?.Conf;
            }
        }

        HPaned hPaned;

        TreeView treeConfiguration;
        TreeStore treeStore;

        Notebook TopNotebook;

        public FormConfigurator() : base("Конфігуратор")
        {
            SetDefaultSize(1000, 600);
            SetPosition(WindowPosition.Center);
            SetDefaultIconFromFile("configurator.ico");

            DeleteEvent += delegate { Application.Quit(); };

            hPaned = new HPaned();
            hPaned.Position = 400;

            ScrolledWindow scroll = new ScrolledWindow() { ShadowType = ShadowType.In };
            scroll.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

            treeConfiguration = new TreeView();

            TreeViewColumn languages = new TreeViewColumn();
            languages.Title = "Конфігурація";

            CellRendererText cell = new CellRendererText();
            languages.PackStart(cell, true);
            languages.AddAttribute(cell, "text", 0);

            treeStore = new TreeStore(typeof(string), typeof(string));

            LoadConf();

            treeConfiguration.AppendColumn(languages);
            treeConfiguration.Model = treeStore;

            scroll.Add(treeConfiguration);

            hPaned.Pack1(scroll, false, true);

            TopNotebook = new Notebook() { BorderWidth = 0, ShowBorder = false };
            TopNotebook.TabPos = PositionType.Top;

            CreateTopNotebookPages();
            
            hPaned.Pack2(TopNotebook, false, true);

            Add(hPaned);
            ShowAll();
        }

        void CreateTopNotebookPages()
        {
            ScrolledWindow scroll = new ScrolledWindow() { ShadowType = ShadowType.In };
            scroll.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

            int numPage = TopNotebook.AppendPage(scroll, new Label { Text = "Ok", Expand = false, Halign = Align.End });

        }

        #region LoadTreeConfiguration

        public void LoadConstant(TreeIter rootIter, ConfigurationConstants confConstant)
        {
            TreeIter contantIter = treeStore.AppendValues(rootIter, confConstant.Name);

            if (confConstant.TabularParts.Count > 0)
            {
                TreeIter constantTabularPartsIter = treeStore.AppendValues(contantIter, "Табличні частини");

                foreach (KeyValuePair<string, ConfigurationObjectTablePart> ConfTablePart in confConstant.TabularParts)
                {
                    TreeIter constantTablePartIter = treeStore.AppendValues(constantTabularPartsIter, ConfTablePart.Value.Name);

                    //Поля
                    foreach (KeyValuePair<string, ConfigurationObjectField> ConfTablePartFields in ConfTablePart.Value.Fields)
                    {
                        string info = (ConfTablePartFields.Value.Type == "pointer" || ConfTablePartFields.Value.Type == "enum") ?
                            " -> " + ConfTablePartFields.Value.Pointer : "";

                        TreeIter fieldIter = treeStore.AppendValues(constantTablePartIter, ConfTablePartFields.Value.Name + info);
                    }
                }
            }
        }

        public void LoadConstants(TreeIter rootIter)
        {
            Console.WriteLine(Conf!.ConstantsBlock.Count);

            foreach (KeyValuePair<string, ConfigurationConstantsBlock> ConfConstantsBlock in Conf!.ConstantsBlock)
            {
                TreeIter contantsBlockIter = treeStore.AppendValues(rootIter, ConfConstantsBlock.Value.BlockName);

                Console.WriteLine(ConfConstantsBlock.Value.BlockName);

                foreach (ConfigurationConstants ConfConstants in ConfConstantsBlock.Value.Constants.Values)
                {
                    LoadConstant(contantsBlockIter, ConfConstants);
                }
            }
        }

        public void LoadDirectory(TreeIter rootIter, ConfigurationDirectories confDirectory)
        {
            TreeIter directoryIter = treeStore.AppendValues(rootIter, confDirectory.Name);

            //Поля
            foreach (KeyValuePair<string, ConfigurationObjectField> ConfFields in confDirectory.Fields)
            {
                string info = (ConfFields.Value.Type == "pointer" || ConfFields.Value.Type == "enum") ?
                    " -> " + ConfFields.Value.Pointer : "";

                TreeIter fieldIter = treeStore.AppendValues(directoryIter, ConfFields.Value.Name + info);
            }

            if (confDirectory.TabularParts.Count > 0)
            {
                TreeIter directoriTabularPartsIter = treeStore.AppendValues(directoryIter, "Табличні частини");

                foreach (KeyValuePair<string, ConfigurationObjectTablePart> ConfTablePart in confDirectory.TabularParts)
                {
                    TreeIter directoriTablePartIter = treeStore.AppendValues(directoriTabularPartsIter, ConfTablePart.Value.Name);

                    //Поля
                    foreach (KeyValuePair<string, ConfigurationObjectField> ConfTablePartFields in ConfTablePart.Value.Fields)
                    {
                        string info = (ConfTablePartFields.Value.Type == "pointer" || ConfTablePartFields.Value.Type == "enum") ?
                            " -> " + ConfTablePartFields.Value.Pointer : "";

                        TreeIter fieldIter = treeStore.AppendValues(directoriTablePartIter, ConfTablePartFields.Value.Name + info);
                    }
                }
            }
        }

        public void LoadDirectories(TreeIter rootIter)
        {
            foreach (ConfigurationDirectories ConfDirectory in Conf!.Directories.Values)
            {
                LoadDirectory(rootIter, ConfDirectory);
            }
        }

        public void LoadDocument(TreeIter rootIter, ConfigurationDocuments confDocument)
        {
            TreeIter documentIter = treeStore.AppendValues(rootIter, confDocument.Name);

            //Поля
            foreach (KeyValuePair<string, ConfigurationObjectField> ConfFields in confDocument.Fields)
            {
                string info = (ConfFields.Value.Type == "pointer" || ConfFields.Value.Type == "enum") ?
                        " -> " + ConfFields.Value.Pointer : "";

                TreeIter fieldIter = treeStore.AppendValues(documentIter, ConfFields.Value.Name + info);
            }

            if (confDocument.TabularParts.Count > 0)
            {
                TreeIter documentTabularPartsIter = treeStore.AppendValues(documentIter, "Табличні частини");

                foreach (KeyValuePair<string, ConfigurationObjectTablePart> ConfTablePart in confDocument.TabularParts)
                {
                    TreeIter documentTablePartIter = treeStore.AppendValues(documentTabularPartsIter, ConfTablePart.Value.Name);

                    //Поля
                    foreach (KeyValuePair<string, ConfigurationObjectField> ConfTablePartFields in ConfTablePart.Value.Fields)
                    {
                        string info = (ConfTablePartFields.Value.Type == "pointer" || ConfTablePartFields.Value.Type == "enum") ?
                            " -> " + ConfTablePartFields.Value.Pointer : "";

                        TreeIter fieldIter = treeStore.AppendValues(documentTablePartIter, ConfTablePartFields.Value.Name + info);
                    }
                }
            }
        }

        public void LoadDocuments(TreeIter rootIter)
        {
            foreach (ConfigurationDocuments ConfDocuments in Conf!.Documents.Values)
            {
                LoadDocument(rootIter, ConfDocuments);
            }
        }

        public void LoadEnum(TreeIter rootIter, ConfigurationEnums confEnum)
        {
            TreeIter enumIter = treeStore.AppendValues(rootIter, confEnum.Name);

            //Поля
            foreach (KeyValuePair<string, ConfigurationEnumField> ConfEnumFields in confEnum.Fields)
            {
                TreeIter enumFieldIter = treeStore.AppendValues(rootIter, ConfEnumFields.Value.Name);
            }
        }

        public void LoadEnums(TreeIter rootIter)
        {
            foreach (ConfigurationEnums ConfEnum in Conf!.Enums.Values)
            {
                LoadEnum(rootIter, ConfEnum);
            }
        }

        public void LoadRegisterInformation(TreeIter rootIter, ConfigurationRegistersInformation confRegisterInformation)
        {
            TreeIter registerInformationIter = treeStore.AppendValues(rootIter, confRegisterInformation.Name);

            TreeIter dimensionFieldsIter = treeStore.AppendValues(rootIter, "Виміри");

            //Поля вимірів
            foreach (KeyValuePair<string, ConfigurationObjectField> ConfDimensionFields in confRegisterInformation.DimensionFields)
            {
                string info = (ConfDimensionFields.Value.Type == "pointer" || ConfDimensionFields.Value.Type == "enum") ?
                    " -> " + ConfDimensionFields.Value.Pointer : "";

                TreeIter fieldIter = treeStore.AppendValues(dimensionFieldsIter, ConfDimensionFields.Value.Name + info);
            }

            TreeIter resourcesFieldsIter = treeStore.AppendValues(rootIter, "Ресурси");

            //Поля ресурсів
            foreach (KeyValuePair<string, ConfigurationObjectField> ConfResourcesFields in confRegisterInformation.ResourcesFields)
            {
                string info = (ConfResourcesFields.Value.Type == "pointer" || ConfResourcesFields.Value.Type == "enum") ?
                    " -> " + ConfResourcesFields.Value.Pointer : "";

                TreeIter fieldIter = treeStore.AppendValues(resourcesFieldsIter, ConfResourcesFields.Value.Name + info);
            }

            TreeIter propertyFieldsIter = treeStore.AppendValues(rootIter, "Поля");

            //Поля реквізитів
            foreach (KeyValuePair<string, ConfigurationObjectField> ConfPropertyFields in confRegisterInformation.PropertyFields)
            {
                string info = (ConfPropertyFields.Value.Type == "pointer" || ConfPropertyFields.Value.Type == "enum") ?
                    " -> " + ConfPropertyFields.Value.Pointer : "";

                TreeIter fieldIter = treeStore.AppendValues(propertyFieldsIter, ConfPropertyFields.Value.Name + info);
            }
        }

        public void LoadRegistersInformation(TreeIter rootIter)
        {
            foreach (ConfigurationRegistersInformation ConfRegistersInformation in Conf!.RegistersInformation.Values)
            {
                LoadRegisterInformation(rootIter, ConfRegistersInformation);
            }
        }

        public void LoadRegisterAccumulation(TreeIter rootIter, ConfigurationRegistersAccumulation confRegisterAccumulation)
        {
            TreeIter registerAccumulationIter = treeStore.AppendValues(rootIter, confRegisterAccumulation.Name);

            TreeIter dimensionFieldsIter = treeStore.AppendValues(rootIter, "Виміри");

            //Поля вимірів
            foreach (KeyValuePair<string, ConfigurationObjectField> ConfDimensionFields in confRegisterAccumulation.DimensionFields)
            {
                string info = (ConfDimensionFields.Value.Type == "pointer" || ConfDimensionFields.Value.Type == "enum") ?
                    " -> " + ConfDimensionFields.Value.Pointer : "";

                TreeIter fieldIter = treeStore.AppendValues(dimensionFieldsIter, ConfDimensionFields.Value.Name + info);
            }

            TreeIter resourcesFieldsIter = treeStore.AppendValues(rootIter, "Ресурси");

            //Поля ресурсів
            foreach (KeyValuePair<string, ConfigurationObjectField> ConfResourcesFields in confRegisterAccumulation.ResourcesFields)
            {
                string info = (ConfResourcesFields.Value.Type == "pointer" || ConfResourcesFields.Value.Type == "enum") ?
                    " -> " + ConfResourcesFields.Value.Pointer : "";

                TreeIter fieldIter = treeStore.AppendValues(resourcesFieldsIter, ConfResourcesFields.Value.Name + info);
            }

            TreeIter propertyFieldsIter = treeStore.AppendValues(rootIter, "Поля");

            //Поля реквізитів
            foreach (KeyValuePair<string, ConfigurationObjectField> ConfPropertyFields in confRegisterAccumulation.PropertyFields)
            {
                string info = (ConfPropertyFields.Value.Type == "pointer" || ConfPropertyFields.Value.Type == "enum") ?
                    " -> " + ConfPropertyFields.Value.Pointer : "";

                TreeIter fieldIter = treeStore.AppendValues(propertyFieldsIter, ConfPropertyFields.Value.Name + info);
            }
        }

        public void LoadRegistersAccumulation(TreeIter rootIter)
        {
            foreach (ConfigurationRegistersAccumulation ConfRegistersAccumulation in Conf!.RegistersAccumulation.Values)
            {
                LoadRegisterAccumulation(rootIter, ConfRegistersAccumulation);
            }
        }

        public void LoadTree()
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

        public void LoadConf()
        {
            Thread thread = new Thread(new ThreadStart(LoadTree));
            thread.Start();
        }

        #endregion

    }
}