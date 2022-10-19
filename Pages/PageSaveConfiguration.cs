using Gtk;
using System.IO;
using System.Xml.XPath;

using AccountingSoftware;

namespace Configurator
{
    class PageSaveConfiguration : VBox
    {
        Configuration? Conf
        {
            get
            {
                return Program.Kernel?.Conf;
            }
        }

        public FormConfigurator? GeneralForm { get; set; }

        string PathToXsltTemplate = AppContext.BaseDirectory;

        //TextView textViewDesc = new TextView();

        Button bAnalize;
        Button bAnalizeAndCreateSQL;
        Button bExecuteSQLAndGenerateCode;
        Button bClose;

        ScrolledWindow scrollListBoxTerminal;
        ListBox listBoxTerminal;

        public PageSaveConfiguration() : base()
        {
            new VBox();
            HBox hBox = new HBox();

            bAnalize = new Button("Аналіз змін");
            bAnalize.Clicked += OnAnalizeClick;
            hBox.PackStart(bAnalize, false, false, 10);

            bAnalizeAndCreateSQL = new Button("Збереження змін. Крок 1");
            bAnalizeAndCreateSQL.Clicked += OnAnalizeAndCreateSQLClick;
            hBox.PackStart(bAnalizeAndCreateSQL, false, false, 10);

            bExecuteSQLAndGenerateCode = new Button("Виконання команд та генерація коду. Крок 2");
            bExecuteSQLAndGenerateCode.Clicked += OnExecuteSQLAndGenerateCodeClick;
            hBox.PackStart(bExecuteSQLAndGenerateCode, false, false, 10);

            bClose = new Button("Закрити");
            bClose.Clicked += (object? sender, EventArgs args) => { GeneralForm?.CloseCurrentPageNotebook(); };
            hBox.PackStart(bClose, false, false, 10);

            PackStart(hBox, false, false, 10);

            //Termainal
            HBox hBoxTerminal = new HBox();
            PackStart(hBoxTerminal, true, true, 5);

            scrollListBoxTerminal = new ScrolledWindow();
            scrollListBoxTerminal.KineticScrolling = true;
            scrollListBoxTerminal.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollListBoxTerminal.Add(listBoxTerminal = new ListBox() { SelectionMode = SelectionMode.None });

            hBoxTerminal.PackStart(scrollListBoxTerminal, true, true, 5);

            ShowAll();
        }

        void OnAnalizeClick(object? sender, EventArgs args)
        {
            Thread thread = new Thread(new ThreadStart(SaveAndAnalize));
            thread.Start();
        }

        void OnAnalizeAndCreateSQLClick(object? sender, EventArgs args)
        {
            Thread thread = new Thread(new ThreadStart(SaveAnalizeAndCreateSQL));
            thread.Start();
        }

        void OnExecuteSQLAndGenerateCodeClick(object? sender, EventArgs args)
        {
            Thread thread = new Thread(new ThreadStart(ExecuteSQLAndGenerateCode));
            thread.Start();
        }

        void CallBack1(bool sensitive)
        {
            bAnalize.Sensitive = sensitive;
            bAnalizeAndCreateSQL.Sensitive = sensitive;
            bClose.Sensitive = sensitive;
        }

        void ApendLine(string text)
        {
            listBoxTerminal.Add(new Label(text) { Halign = Align.Start });
            listBoxTerminal.ShowAll();

            //scrollListBoxTerminal.Vadjustment.ChangeValue();
            //scrollListBoxTerminal.Vadjustment.Value = scrollListBoxTerminal.Vadjustment.PageSize;
        }

        void ClearListBoxTerminal()
        {
            foreach (var item in listBoxTerminal.Children)
                listBoxTerminal.Remove(item);
        }

        string GetNameFromType(string Type)
        {
            switch (Type)
            {
                case "Constants":
                    return "Константи";

                case "Constants.TablePart":
                    return "Константи.Таблична частина";

                case "Directory":
                    return "Довідник";

                case "Directory.TablePart":
                    return "Довідник.Таблична частина";

                case "Document":
                    return "Документ";

                case "Document.TablePart":
                    return "Документ.Таблична частина";

                case "RegisterInformation":
                    return "Регістер відомостей";

                case "RegisterInformation.TablePart":
                    return "Регістер відомостей.Таблична частина";

                case "RegisterAccumulation":
                    return "Регістер накопичення";

                case "RegisterAccumulation.TablePart":
                    return "Регістер накопичення.Таблична частина";

                default:
                    return "<Невідомий тип>";
            }
        }

        void SaveAndAnalize()
        {
            ClearListBoxTerminal();

            ApendLine("[ КОНФІГУРАЦІЯ ]");

            ApendLine("1. Створення копії файлу конфігурації");
            Conf!.PathToCopyXmlFileConfiguration = Configuration.CreateCopyConfigurationFile(Conf.PathToXmlFileConfiguration);
            ApendLine(" --> " + Conf.PathToCopyXmlFileConfiguration);

            Conf.PathToTempXmlFileConfiguration = Configuration.GetTempPathToConfigurationFile(Conf.PathToXmlFileConfiguration);

            ApendLine("2. Збереження конфігурації у тимчасовий файл");
            Configuration.Save(Conf.PathToTempXmlFileConfiguration, Conf);
            ApendLine(" --> " + Conf.PathToTempXmlFileConfiguration);

            ApendLine("3. Отримання структури бази даних");
            ConfigurationInformationSchema informationSchema = Program.Kernel!.DataBase.SelectInformationSchema();

            if (informationSchema.Tables.Count > 0)
            {
                Configuration.SaveInformationSchema(informationSchema,
                    System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "InformationSchema.xml"));

                ApendLine("4. Створення загального файлу для порівняння");

                Configuration.CreateOneFileForComparison(
                    System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "InformationSchema.xml"),
                    Conf.PathToTempXmlFileConfiguration,
                    System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, Conf.PathToCopyXmlFileConfiguration),
                    System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "ComparisonAllData.xml")
                );

                ApendLine("5. Порівняння конфігурації та бази даних");

                try
                {
                    Configuration.Comparison(
                        System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "ComparisonAllData.xml"),
                        System.IO.Path.Combine(PathToXsltTemplate, "Comparison.xslt"),
                        System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "Comparison.xml")
                    );
                }
                catch (Exception ex)
                {
                    ApendLine(ex.Message);
                    return;
                }

                XPathDocument xPathDoc = new XPathDocument(
                    System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "Comparison.xml")
                );
                XPathNavigator xPathDocNavigator = xPathDoc.CreateNavigator();

                XPathNodeIterator nodeDeleteDirectory = xPathDocNavigator.Select("/root/Control_Table[IsExist = 'delete']");
                while (nodeDeleteDirectory!.MoveNext())
                {
                    XPathNavigator? nodeName = nodeDeleteDirectory?.Current?.SelectSingleNode("Name");
                    XPathNavigator? nodeTable = nodeDeleteDirectory?.Current?.SelectSingleNode("Table");
                    XPathNavigator? nodeType = nodeDeleteDirectory?.Current?.SelectSingleNode("Type");

                    ApendLine("Видалений " + GetNameFromType(nodeType?.Value ?? "") + ": " + nodeName?.Value);
                }

                XPathNodeIterator nodeNewDirectory = xPathDocNavigator.Select("/root/Control_Table[IsExist = 'no']");
                while (nodeNewDirectory!.MoveNext())
                {
                    XPathNavigator? nodeName = nodeNewDirectory?.Current?.SelectSingleNode("Name");
                    XPathNavigator? nodeType = nodeNewDirectory?.Current?.SelectSingleNode("Type");
                    ApendLine("Новий " + GetNameFromType(nodeType?.Value ?? "") + ": " + nodeName?.Value);

                    InfoTableCreateFieldCreate(nodeNewDirectory?.Current, "\t ");
                    ApendLine("");

                    XPathNodeIterator? nodeDirectoryTabularParts = nodeNewDirectory?.Current?.Select("Control_TabularParts");
                    while (nodeDirectoryTabularParts!.MoveNext())
                    {
                        XPathNavigator? nodeTabularPartsName = nodeDirectoryTabularParts?.Current?.SelectSingleNode("Name");
                        ApendLine("\t Нова таблична частина: " + nodeTabularPartsName?.Value);

                        InfoTableCreateFieldCreate(nodeDirectoryTabularParts?.Current, "\t\t ");
                    }
                }

                XPathNodeIterator nodeDirectoryExist = xPathDocNavigator.Select("/root/Control_Table[IsExist = 'yes']");
                while (nodeDirectoryExist!.MoveNext())
                {
                    bool flag = false;

                    XPathNodeIterator? nodeDirectoryDeleteField = nodeDirectoryExist.Current?.Select("Control_Field[IsExist = 'delete']");
                    if (nodeDirectoryDeleteField?.Count > 0)
                    {
                        XPathNavigator? nodeName = nodeDirectoryExist?.Current?.SelectSingleNode("Name");
                        XPathNavigator? nodeType = nodeDirectoryExist?.Current?.SelectSingleNode("Type");
                        ApendLine(GetNameFromType(nodeType!.Value) + ": " + nodeName?.Value);
                        flag = true;
                    }
                    while (nodeDirectoryDeleteField!.MoveNext())
                    {
                        XPathNavigator? nodeFieldName = nodeDirectoryDeleteField?.Current?.SelectSingleNode("Name");
                        ApendLine("\t Видалене Поле: " + nodeFieldName?.Value);
                    }

                    XPathNodeIterator? nodeDirectoryNewField = nodeDirectoryExist?.Current?.Select("Control_Field[IsExist = 'no']");
                    if (nodeDirectoryNewField?.Count > 0)
                    {
                        XPathNavigator? nodeName = nodeDirectoryExist?.Current?.SelectSingleNode("Name");
                        XPathNavigator? nodeType = nodeDirectoryExist?.Current?.SelectSingleNode("Type");
                        ApendLine(GetNameFromType(nodeType?.Value ?? "") + ": " + nodeName?.Value);
                        flag = true;
                    }
                    while (nodeDirectoryNewField!.MoveNext())
                    {
                        XPathNavigator? nodeFieldName = nodeDirectoryNewField?.Current?.SelectSingleNode("Name");
                        ApendLine("\t Нове Поле: " + nodeFieldName?.Value);
                    }

                    XPathNodeIterator? nodeDirectoryClearField = nodeDirectoryExist?.Current?.Select("Control_Field[IsExist = 'yes']/Type[Coincide = 'clear']");
                    if (nodeDirectoryClearField?.Count > 0 && flag == false)
                    {
                        XPathNavigator? nodeName = nodeDirectoryClearField?.Current?.SelectSingleNode("Name");
                        XPathNavigator? nodeType = nodeDirectoryClearField?.Current?.SelectSingleNode("Type");
                        ApendLine(GetNameFromType(nodeType?.Value ?? "") + ": " + nodeName?.Value);
                        flag = true;
                    }
                    while (nodeDirectoryClearField!.MoveNext())
                    {
                        XPathNavigator? nodeFieldName = nodeDirectoryClearField?.Current?.SelectSingleNode("../Name");
                        ApendLine("\t Поле: " + nodeFieldName?.Value + " -> змінений тип даних. Можлива втрата даних, або колонка буде скопійована!");
                    }

                    XPathNodeIterator? nodeDirectoryExistField = nodeDirectoryExist?.Current?.Select("Control_Field[IsExist = 'yes']/Type[Coincide = 'no']");
                    if (nodeDirectoryExistField?.Count > 0 && flag == false)
                    {
                        XPathNavigator? nodeName = nodeDirectoryExistField?.Current?.SelectSingleNode("Name");
                        XPathNavigator? nodeType = nodeDirectoryExistField?.Current?.SelectSingleNode("Type");
                        ApendLine(GetNameFromType(nodeType?.Value ?? "") + ": " + nodeName?.Value);
                        flag = true;
                    }
                    while (nodeDirectoryExistField!.MoveNext())
                    {
                        XPathNavigator? nodeFieldName = nodeDirectoryExistField?.Current?.SelectSingleNode("../Name");
                        XPathNavigator? nodeDataType = nodeDirectoryExistField?.Current?.SelectSingleNode("DataType");
                        XPathNavigator? nodeUdtName = nodeDirectoryExistField?.Current?.SelectSingleNode("UdtName");
                        XPathNavigator? nodeDataTypeCreate = nodeDirectoryExistField?.Current?.SelectSingleNode("DataTypeCreate");

                        ApendLine("\t Поле: " + nodeFieldName?.Value + " -> змінений тип даних (Тип в базі: " + nodeDataType?.Value + "(" + nodeUdtName?.Value + ")" + " -> Новий тип: " + nodeDataTypeCreate?.Value + "). Можлива втрата даних, або колонка буде скопійована!");
                    }

                    XPathNodeIterator? nodeDirectoryNewTabularParts = nodeDirectoryExist?.Current?.Select("Control_TabularParts[IsExist = 'no']");
                    if (nodeDirectoryNewTabularParts?.Count > 0)
                    {
                        if (flag == false)
                        {
                            XPathNavigator? nodeName = nodeDirectoryExist?.Current?.SelectSingleNode("Name");
                            XPathNavigator? nodeType = nodeDirectoryExist?.Current?.SelectSingleNode("Type");
                            ApendLine(GetNameFromType(nodeType?.Value ?? "") + ": " + nodeName?.Value);
                            flag = true;
                        }
                    }
                    while (nodeDirectoryNewTabularParts!.MoveNext())
                    {
                        XPathNavigator? nodeTabularPartsName = nodeDirectoryNewTabularParts?.Current?.SelectSingleNode("Name");
                        ApendLine("\t Нова таблична частина : " + nodeTabularPartsName?.Value);

                        InfoTableCreateFieldCreate(nodeDirectoryNewTabularParts?.Current, "\t\t");
                    }

                    XPathNodeIterator? nodeDirectoryTabularParts = nodeDirectoryExist?.Current?.Select("Control_TabularParts[IsExist = 'yes']");
                    while (nodeDirectoryTabularParts!.MoveNext())
                    {
                        bool flagTP = false;

                        XPathNodeIterator? nodeDirectoryTabularPartsNewField = nodeDirectoryTabularParts?.Current?.Select("Control_Field[IsExist = 'no']");
                        if (nodeDirectoryTabularPartsNewField?.Count > 0)
                        {
                            if (!flag)
                            {
                                XPathNavigator? nodeName = nodeDirectoryExist?.Current?.SelectSingleNode("Name");
                                XPathNavigator? nodeType = nodeDirectoryExist?.Current?.SelectSingleNode("Type");
                                ApendLine(GetNameFromType(nodeType?.Value ?? "") + ": " + nodeName?.Value);
                                flag = true;
                            }

                            if (!flagTP)
                            {
                                XPathNavigator? nodeTabularPartsName = nodeDirectoryTabularParts?.Current?.SelectSingleNode("Name");
                                ApendLine("\t Таблична частина : " + nodeTabularPartsName?.Value);
                                flagTP = true;
                            }
                        }
                        while (nodeDirectoryTabularPartsNewField!.MoveNext())
                        {
                            XPathNavigator? nodeFieldName = nodeDirectoryTabularPartsNewField?.Current?.SelectSingleNode("Name");
                            XPathNavigator? nodeConfType = nodeDirectoryTabularPartsNewField?.Current?.SelectSingleNode("FieldCreate/ConfType");

                            ApendLine("\t\t Нове Поле: " + nodeFieldName?.Value + "(Тип: " + nodeConfType?.Value + ")");
                        }

                        XPathNodeIterator? nodeDirectoryTabularPartsField = nodeDirectoryTabularParts?.Current?.Select("Control_Field[IsExist = 'yes']/Type[Coincide = 'no']");
                        if (nodeDirectoryTabularPartsField?.Count > 0)
                        {
                            if (flag == false)
                            {
                                XPathNavigator? nodeName = nodeDirectoryExist?.Current?.SelectSingleNode("Name");
                                XPathNavigator? nodeType = nodeDirectoryExist?.Current?.SelectSingleNode("Type");
                                ApendLine(GetNameFromType(nodeType?.Value ?? "") + ": " + nodeName?.Value);
                                flag = true;
                            }

                            if (!flagTP)
                            {
                                XPathNavigator? nodeTabularPartsName = nodeDirectoryTabularParts?.Current?.SelectSingleNode("Name");
                                ApendLine("\t Таблична частина : " + nodeTabularPartsName?.Value);
                                flagTP = true;
                            }
                        }
                        while (nodeDirectoryTabularPartsField!.MoveNext())
                        {
                            XPathNavigator? nodeFieldName = nodeDirectoryTabularPartsField?.Current?.SelectSingleNode("../Name");
                            XPathNavigator? nodeDataType = nodeDirectoryTabularPartsField?.Current?.SelectSingleNode("DataType");
                            XPathNavigator? nodeDataTypeCreate = nodeDirectoryTabularPartsField?.Current?.SelectSingleNode("DataTypeCreate");

                            ApendLine("\t\t Поле: " + nodeFieldName?.Value + " -> змінений тип даних (Тип в базі: " + nodeDataType?.Value + " -> Новий тип: " + nodeDataTypeCreate?.Value + "). Можлива втрата даних, або колонка буде скопійована!");
                        }
                    }
                }
            }
            else
            {
                ApendLine("Нова база даних");
            }
        }

        void SaveAnalizeAndCreateSQL()
        {
            ClearListBoxTerminal();

            ApendLine("[ АНАЛІЗ ]");

            string replacementColumn = "yes"; //(checkBoxReplacement.Checked ? "yes" : "no");

            ApendLine("1. Створення копії файлу конфігурації");
            Conf!.PathToCopyXmlFileConfiguration = Configuration.CreateCopyConfigurationFile(Conf.PathToXmlFileConfiguration, Conf.PathToCopyXmlFileConfiguration);
            ApendLine(" --> " + Conf.PathToCopyXmlFileConfiguration);

            Conf.PathToTempXmlFileConfiguration = Configuration.GetTempPathToConfigurationFile(Conf.PathToXmlFileConfiguration, Conf.PathToTempXmlFileConfiguration);

            ApendLine("2. Збереження конфігурації у тимчасовий файл");
            Configuration.Save(Conf.PathToTempXmlFileConfiguration, Conf);
            ApendLine(" --> " + Conf.PathToTempXmlFileConfiguration);

            ApendLine("2. Отримання структури бази даних");
            ConfigurationInformationSchema informationSchema = Program.Kernel!.DataBase.SelectInformationSchema();
            Configuration.SaveInformationSchema(informationSchema,
                 System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "InformationSchema.xml"));

            ApendLine("3. Створення загального файлу для порівняння");
            Configuration.CreateOneFileForComparison(
                System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "InformationSchema.xml"),
                Conf.PathToTempXmlFileConfiguration,
                System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, Conf.PathToCopyXmlFileConfiguration),
                System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "ComparisonAllData.xml")
            );

            ApendLine("4. Порівняння конфігурації та бази даних");
            Configuration.Comparison(
                System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "ComparisonAllData.xml"),
                System.IO.Path.Combine(PathToXsltTemplate, "Comparison.xslt"),
                System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "Comparison.xml")
            );

            ApendLine("5. Створення команд SQL");
            Configuration.ComparisonAnalizeGeneration(
                System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "Comparison.xml"),
                System.IO.Path.Combine(PathToXsltTemplate, "ComparisonAnalize.xslt"),
                System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "ComparisonAnalize.xml"), replacementColumn);

            if (informationSchema.Tables.Count > 0)
            {
                XPathDocument xPathDoc = new XPathDocument(
                    System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "ComparisonAnalize.xml")
                );
                XPathNavigator xPathDocNavigator = xPathDoc.CreateNavigator();

                XPathNodeIterator nodeInfo = xPathDocNavigator.Select("/root/info");
                if (nodeInfo.Count == 0)
                {
                    ApendLine("Інформація відсутня!");
                }
                else
                    while (nodeInfo!.MoveNext())
                    {
                        ApendLine(nodeInfo?.Current?.Value ?? "");
                    }

                ApendLine("[ Команди SQL ]");

                XPathNodeIterator nodeSQL = xPathDocNavigator.Select("/root/sql");
                if (nodeSQL.Count == 0)
                {
                    ApendLine("Команди відсутні!");
                }
                else
                    while (nodeSQL!.MoveNext())
                    {
                        ApendLine(" - " + nodeSQL?.Current?.Value);
                    }
            }
            else
            {
                ApendLine("Нова база даних");
            }

            // CallBack1(true);
        }

        void ExecuteSQLAndGenerateCode()
        {
            ClearListBoxTerminal();

            //Read SQL
            List<string> SqlList = Configuration.ListComparisonSql(
               System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf!.PathToXmlFileConfiguration)!, "ComparisonAnalize.xml"));

            ApendLine("[ Виконання SQL ]");

            if (SqlList.Count == 0)
            {
                ApendLine("Команди відсутні!");
            }
            else
                //Execute
                foreach (string sqlText in SqlList)
                {
                    ApendLine(" --> " + sqlText);

                    try
                    {
                        Program.Kernel!.DataBase.ExecuteSQL(sqlText);
                    }
                    catch (Exception ex)
                    {
                        ApendLine("Помилка: " + ex.Message);
                    }
                }

            ApendLine("Збереження конфігурації та видалення тимчасових файлів");
            Configuration.RewriteConfigurationFileFromTempFile(
                Conf.PathToXmlFileConfiguration,
                Conf.PathToTempXmlFileConfiguration,
                Conf.PathToCopyXmlFileConfiguration);

            if (File.Exists(System.IO.Path.Combine(PathToXsltTemplate, "CodeGeneration.xslt")))
            {
                ApendLine("[ Генерування коду ]");
                Configuration.GenerationCode(Conf.PathToXmlFileConfiguration,
                    System.IO.Path.Combine(PathToXsltTemplate, "CodeGeneration.xslt"),
                    System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "CodeGeneration.cs"));
            }

            ApendLine("ГОТОВО!");
        }

        private void InfoTableCreateFieldCreate(XPathNavigator? xPathNavigator, string tab)
        {
            XPathNodeIterator? nodeField = xPathNavigator?.Select("TableCreate/FieldCreate");
            while (nodeField!.MoveNext())
            {
                XPathNavigator? nodeName = nodeField?.Current?.SelectSingleNode("Name");
                XPathNavigator? nodeConfType = nodeField?.Current?.SelectSingleNode("ConfType");

                ApendLine(tab + "Поле: " + nodeName?.Value + "(Тип: " + nodeConfType?.Value + ")");
            }
        }
    }
}