using Gtk;

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

        Button bAnalize;
        Button bAnalizeAndCreateSQL;
        Button bExecuteSQLAndGenerateCode;
        Button bClose;

        ScrolledWindow scrollListBoxTerminal;

        TextView textTerminal;

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

            bExecuteSQLAndGenerateCode = new Button("Збереження змін. Крок 2");
            bExecuteSQLAndGenerateCode.Clicked += OnExecuteSQLAndGenerateCodeClick;
            hBox.PackStart(bExecuteSQLAndGenerateCode, false, false, 10);

            bClose = new Button("Закрити");
            bClose.Clicked += OnCloseClick;
            hBox.PackStart(bClose, false, false, 10);

            PackStart(hBox, false, false, 10);

            //Terminal
            HBox hBoxTerminal = new HBox();
            PackStart(hBoxTerminal, true, true, 5);

            scrollListBoxTerminal = new ScrolledWindow();
            scrollListBoxTerminal.KineticScrolling = true;
            scrollListBoxTerminal.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollListBoxTerminal.Add(textTerminal = new TextView());

            hBoxTerminal.PackStart(scrollListBoxTerminal, true, true, 5);

            ShowAll();
        }

        void OnCloseClick(object? sender, EventArgs args)
        {
            string fullPathToCopyXmlFileConguratifion =
                System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf!.PathToXmlFileConfiguration)!, Conf.PathToCopyXmlFileConfiguration);

            if (File.Exists(fullPathToCopyXmlFileConguratifion))
                File.Delete(fullPathToCopyXmlFileConguratifion);

            if (File.Exists(Conf!.PathToTempXmlFileConfiguration))
                File.Delete(Conf!.PathToTempXmlFileConfiguration);

            GeneralForm?.CloseCurrentPageNotebook();
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

        void ButtonSensitive(bool sensitive)
        {
            Gtk.Application.Invoke
            (
                delegate
                {
                    bAnalize.Sensitive = sensitive;
                    bAnalizeAndCreateSQL.Sensitive = sensitive;
                    bExecuteSQLAndGenerateCode.Sensitive = sensitive;
                    bClose.Sensitive = sensitive;
                }
            );
        }

        void ApendLine(string text)
        {
            Gtk.Application.Invoke
            (
                delegate
                {
                    textTerminal.Buffer.Text += text + "\n";
                    TextMark end = textTerminal.Buffer.CreateMark("end", textTerminal.Buffer.GetIterAtLine(textTerminal.Buffer.LineCount), false);
                    textTerminal.ScrollToMark(end, 0, true, 0, 0);
                    textTerminal.Buffer.DeleteMark(end);
                }
            );
        }

        void ClearListBoxTerminal()
        {
            Gtk.Application.Invoke
            (
                delegate
                {
                    textTerminal.Buffer.Text = "";
                }
            );
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
            ButtonSensitive(false);

            ClearListBoxTerminal();

            ApendLine("[ КОНФІГУРАЦІЯ ]\n");

            ApendLine("1. Створення копії файлу конфігурації");
            Conf!.PathToCopyXmlFileConfiguration = Configuration.CreateCopyConfigurationFile(Conf.PathToXmlFileConfiguration, Conf.PathToCopyXmlFileConfiguration);
            ApendLine(" --> " + Conf.PathToCopyXmlFileConfiguration + "\n");

            string fullPathToCopyXmlFileConguratifion = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, Conf.PathToCopyXmlFileConfiguration);

            Conf!.PathToTempXmlFileConfiguration = Configuration.GetTempPathToConfigurationFile(Conf.PathToXmlFileConfiguration, Conf!.PathToTempXmlFileConfiguration);

            ApendLine("2. Збереження конфігурації у тимчасовий файл");
            Configuration.Save(Conf.PathToTempXmlFileConfiguration, Conf);
            ApendLine(" --> " + Conf.PathToTempXmlFileConfiguration + "\n");

            ApendLine("3. Отримання структури бази даних");
            ConfigurationInformationSchema informationSchema = Program.Kernel!.DataBase.SelectInformationSchema();

            if (informationSchema.Tables.Count > 0)
            {
                string informationSchemaFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "InformationSchema.xml");

                Configuration.SaveInformationSchema(informationSchema, informationSchemaFile);
                ApendLine(" --> " + informationSchemaFile + "\n");

                ApendLine("4. Створення загального файлу для порівняння");

                string oneFileForComparison = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "ComparisonAllData.xml");

                Configuration.CreateOneFileForComparison(
                    informationSchemaFile,
                    Conf!.PathToTempXmlFileConfiguration,
                    fullPathToCopyXmlFileConguratifion,
                    oneFileForComparison
                );
                ApendLine(" --> " + oneFileForComparison + "\n");

                ApendLine("5. Порівняння конфігурації та бази даних");

                string comparisonFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "Comparison.xml");

                try
                {
                    Configuration.Comparison(
                        oneFileForComparison,
                        System.IO.Path.Combine(PathToXsltTemplate, "Comparison.xslt"),
                        comparisonFile
                    );
                }
                catch (Exception ex)
                {
                    ApendLine(ex.Message);
                    return;
                }

                ApendLine(" --> " + comparisonFile + "\n");

                XPathDocument xPathDoc = new XPathDocument(
                    System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "Comparison.xml")
                );
                XPathNavigator xPathDocNavigator = xPathDoc.CreateNavigator();

                XPathNodeIterator nodeDeleteDirectory = xPathDocNavigator.Select("/root/Control_Table[IsExist = 'delete']");
                int counterDelete = nodeDeleteDirectory?.Count ?? 0;
                ApendLine("Видалених: " + counterDelete + "\n");

                while (nodeDeleteDirectory!.MoveNext())
                {
                    XPathNavigator? nodeName = nodeDeleteDirectory?.Current?.SelectSingleNode("Name");
                    XPathNavigator? nodeTable = nodeDeleteDirectory?.Current?.SelectSingleNode("Table");
                    XPathNavigator? nodeType = nodeDeleteDirectory?.Current?.SelectSingleNode("Type");

                    ApendLine("Видалений " + GetNameFromType(nodeType?.Value ?? "") + ": " + nodeName?.Value + "\n");
                }

                XPathNodeIterator nodeNewDirectory = xPathDocNavigator.Select("/root/Control_Table[IsExist = 'no']");
                int counterNew = nodeNewDirectory?.Count ?? 0;
                ApendLine("Нових: " + counterNew + "\n");

                while (nodeNewDirectory!.MoveNext())
                {
                    XPathNavigator? nodeName = nodeNewDirectory?.Current?.SelectSingleNode("Name");
                    XPathNavigator? nodeType = nodeNewDirectory?.Current?.SelectSingleNode("Type");
                    ApendLine("Новий " + GetNameFromType(nodeType?.Value ?? "") + ": " + nodeName?.Value + "\n");

                    InfoTableCreateFieldCreate(nodeNewDirectory?.Current, "\t ");
                    ApendLine("");

                    XPathNodeIterator? nodeDirectoryTabularParts = nodeNewDirectory?.Current?.Select("Control_TabularParts");
                    while (nodeDirectoryTabularParts!.MoveNext())
                    {
                        XPathNavigator? nodeTabularPartsName = nodeDirectoryTabularParts?.Current?.SelectSingleNode("Name");
                        ApendLine("\t Нова таблична частина: " + nodeTabularPartsName?.Value + "\n");

                        InfoTableCreateFieldCreate(nodeDirectoryTabularParts?.Current, "\t\t ");
                    }
                }

                XPathNodeIterator nodeDirectoryExist = xPathDocNavigator.Select("/root/Control_Table[IsExist = 'yes']");
                ApendLine("Зміни:\n");

                while (nodeDirectoryExist!.MoveNext())
                {
                    bool flag = false;

                    XPathNodeIterator? nodeDirectoryDeleteField = nodeDirectoryExist.Current?.Select("Control_Field[IsExist = 'delete']");
                    if (nodeDirectoryDeleteField?.Count > 0)
                    {
                        XPathNavigator? nodeName = nodeDirectoryExist?.Current?.SelectSingleNode("Name");
                        XPathNavigator? nodeType = nodeDirectoryExist?.Current?.SelectSingleNode("Type");
                        ApendLine(GetNameFromType(nodeType!.Value) + ": " + nodeName?.Value + "\n");
                        flag = true;
                    }
                    while (nodeDirectoryDeleteField!.MoveNext())
                    {
                        XPathNavigator? nodeFieldName = nodeDirectoryDeleteField?.Current?.SelectSingleNode("Name");
                        ApendLine("\t Видалене Поле: " + nodeFieldName?.Value + "\n");
                    }

                    XPathNodeIterator? nodeDirectoryNewField = nodeDirectoryExist?.Current?.Select("Control_Field[IsExist = 'no']");
                    if (nodeDirectoryNewField?.Count > 0)
                    {
                        XPathNavigator? nodeName = nodeDirectoryExist?.Current?.SelectSingleNode("Name");
                        XPathNavigator? nodeType = nodeDirectoryExist?.Current?.SelectSingleNode("Type");
                        ApendLine(GetNameFromType(nodeType?.Value ?? "") + ": " + nodeName?.Value + "\n");
                        flag = true;
                    }
                    while (nodeDirectoryNewField!.MoveNext())
                    {
                        XPathNavigator? nodeFieldName = nodeDirectoryNewField?.Current?.SelectSingleNode("Name");
                        ApendLine("\t Нове Поле: " + nodeFieldName?.Value + "\n");
                    }

                    XPathNodeIterator? nodeDirectoryClearField = nodeDirectoryExist?.Current?.Select("Control_Field[IsExist = 'yes']/Type[Coincide = 'clear']");
                    if (nodeDirectoryClearField?.Count > 0 && flag == false)
                    {
                        XPathNavigator? nodeName = nodeDirectoryClearField?.Current?.SelectSingleNode("Name");
                        XPathNavigator? nodeType = nodeDirectoryClearField?.Current?.SelectSingleNode("Type");
                        ApendLine(GetNameFromType(nodeType?.Value ?? "") + ": " + nodeName?.Value + "\n");
                        flag = true;
                    }
                    while (nodeDirectoryClearField!.MoveNext())
                    {
                        XPathNavigator? nodeFieldName = nodeDirectoryClearField?.Current?.SelectSingleNode("../Name");
                        ApendLine("\t Поле: " + nodeFieldName?.Value + " -> змінений тип даних. Можлива втрата даних, або колонка буде скопійована!" + "\n");
                    }

                    XPathNodeIterator? nodeDirectoryExistField = nodeDirectoryExist?.Current?.Select("Control_Field[IsExist = 'yes']/Type[Coincide = 'no']");
                    if (nodeDirectoryExistField?.Count > 0 && flag == false)
                    {
                        XPathNavigator? nodeName = nodeDirectoryExistField?.Current?.SelectSingleNode("Name");
                        XPathNavigator? nodeType = nodeDirectoryExistField?.Current?.SelectSingleNode("Type");
                        ApendLine(GetNameFromType(nodeType?.Value ?? "") + ": " + nodeName?.Value + "\n");
                        flag = true;
                    }
                    while (nodeDirectoryExistField!.MoveNext())
                    {
                        XPathNavigator? nodeFieldName = nodeDirectoryExistField?.Current?.SelectSingleNode("../Name");
                        XPathNavigator? nodeDataType = nodeDirectoryExistField?.Current?.SelectSingleNode("DataType");
                        XPathNavigator? nodeUdtName = nodeDirectoryExistField?.Current?.SelectSingleNode("UdtName");
                        XPathNavigator? nodeDataTypeCreate = nodeDirectoryExistField?.Current?.SelectSingleNode("DataTypeCreate");

                        ApendLine("\t Поле: " + nodeFieldName?.Value + " -> змінений тип даних (Тип в базі: " + nodeDataType?.Value + "(" + nodeUdtName?.Value + ")" + " -> Новий тип: " + nodeDataTypeCreate?.Value + "). Можлива втрата даних, або колонка буде скопійована!" + "\n");
                    }

                    XPathNodeIterator? nodeDirectoryNewTabularParts = nodeDirectoryExist?.Current?.Select("Control_TabularParts[IsExist = 'no']");
                    if (nodeDirectoryNewTabularParts?.Count > 0)
                    {
                        if (flag == false)
                        {
                            XPathNavigator? nodeName = nodeDirectoryExist?.Current?.SelectSingleNode("Name");
                            XPathNavigator? nodeType = nodeDirectoryExist?.Current?.SelectSingleNode("Type");
                            ApendLine(GetNameFromType(nodeType?.Value ?? "") + ": " + nodeName?.Value + "\n");
                            flag = true;
                        }
                    }
                    while (nodeDirectoryNewTabularParts!.MoveNext())
                    {
                        XPathNavigator? nodeTabularPartsName = nodeDirectoryNewTabularParts?.Current?.SelectSingleNode("Name");
                        ApendLine("\t Нова таблична частина : " + nodeTabularPartsName?.Value + "\n");

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
                                ApendLine(GetNameFromType(nodeType?.Value ?? "") + ": " + nodeName?.Value + "\n");
                                flag = true;
                            }

                            if (!flagTP)
                            {
                                XPathNavigator? nodeTabularPartsName = nodeDirectoryTabularParts?.Current?.SelectSingleNode("Name");
                                ApendLine("\t Таблична частина : " + nodeTabularPartsName?.Value + "\n");
                                flagTP = true;
                            }
                        }
                        while (nodeDirectoryTabularPartsNewField!.MoveNext())
                        {
                            XPathNavigator? nodeFieldName = nodeDirectoryTabularPartsNewField?.Current?.SelectSingleNode("Name");
                            XPathNavigator? nodeConfType = nodeDirectoryTabularPartsNewField?.Current?.SelectSingleNode("FieldCreate/ConfType");

                            ApendLine("\t\t Нове Поле: " + nodeFieldName?.Value + "(Тип: " + nodeConfType?.Value + ")" + "\n");
                        }

                        XPathNodeIterator? nodeDirectoryTabularPartsField = nodeDirectoryTabularParts?.Current?.Select("Control_Field[IsExist = 'yes']/Type[Coincide = 'no']");
                        if (nodeDirectoryTabularPartsField?.Count > 0)
                        {
                            if (flag == false)
                            {
                                XPathNavigator? nodeName = nodeDirectoryExist?.Current?.SelectSingleNode("Name");
                                XPathNavigator? nodeType = nodeDirectoryExist?.Current?.SelectSingleNode("Type");
                                ApendLine(GetNameFromType(nodeType?.Value ?? "") + ": " + nodeName?.Value + "\n");
                                flag = true;
                            }

                            if (!flagTP)
                            {
                                XPathNavigator? nodeTabularPartsName = nodeDirectoryTabularParts?.Current?.SelectSingleNode("Name");
                                ApendLine("\t Таблична частина : " + nodeTabularPartsName?.Value + "\n");
                                flagTP = true;
                            }
                        }
                        while (nodeDirectoryTabularPartsField!.MoveNext())
                        {
                            XPathNavigator? nodeFieldName = nodeDirectoryTabularPartsField?.Current?.SelectSingleNode("../Name");
                            XPathNavigator? nodeDataType = nodeDirectoryTabularPartsField?.Current?.SelectSingleNode("DataType");
                            XPathNavigator? nodeDataTypeCreate = nodeDirectoryTabularPartsField?.Current?.SelectSingleNode("DataTypeCreate");

                            ApendLine("\t\t Поле: " + nodeFieldName?.Value + " -> змінений тип даних (Тип в базі: " + nodeDataType?.Value + " -> Новий тип: " + nodeDataTypeCreate?.Value + "). Можлива втрата даних, або колонка буде скопійована!" + "\n");
                        }
                    }
                }
            }
            else
            {
                ApendLine("Нова база даних");
            }

            ApendLine("\nВидалення копії файлу конфігурації");
            if (File.Exists(fullPathToCopyXmlFileConguratifion))
            {
                File.Delete(fullPathToCopyXmlFileConguratifion);
                ApendLine(" --> " + fullPathToCopyXmlFileConguratifion + "\n");
            }

            ApendLine("Видалення тимчасового файлу");
            if (File.Exists(Conf!.PathToTempXmlFileConfiguration))
            {
                File.Delete(Conf!.PathToTempXmlFileConfiguration);
                ApendLine(" --> " + Conf!.PathToTempXmlFileConfiguration + "\n");
            }

            ButtonSensitive(true);
        }

        void SaveAnalizeAndCreateSQL()
        {
            ButtonSensitive(false);

            ClearListBoxTerminal();

            ApendLine("[ АНАЛІЗ ]\n");

            string replacementColumn = "yes"; //(checkBoxReplacement.Checked ? "yes" : "no");

            ApendLine("1. Створення копії файлу конфігурації");
            Conf!.PathToCopyXmlFileConfiguration = Configuration.CreateCopyConfigurationFile(Conf.PathToXmlFileConfiguration, Conf.PathToCopyXmlFileConfiguration);
            ApendLine(" --> " + Conf.PathToCopyXmlFileConfiguration + "\n");

            string fullPathToCopyXmlFileConguratifion = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, Conf.PathToCopyXmlFileConfiguration);

            Conf.PathToTempXmlFileConfiguration = Configuration.GetTempPathToConfigurationFile(Conf.PathToXmlFileConfiguration, Conf.PathToTempXmlFileConfiguration);

            ApendLine("2. Збереження конфігурації у тимчасовий файл");
            Configuration.Save(Conf.PathToTempXmlFileConfiguration, Conf);
            ApendLine(" --> " + Conf.PathToTempXmlFileConfiguration + "\n");

            ApendLine("2. Отримання структури бази даних");
            ConfigurationInformationSchema informationSchema = Program.Kernel!.DataBase.SelectInformationSchema();
            Configuration.SaveInformationSchema(informationSchema,
                 System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "InformationSchema.xml"));

            ApendLine("3. Створення загального файлу для порівняння");
            Configuration.CreateOneFileForComparison(
                System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "InformationSchema.xml"),
                Conf.PathToTempXmlFileConfiguration,
                fullPathToCopyXmlFileConguratifion,
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
                ApendLine("");

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

                ApendLine("\n[ Команди SQL ]\n");

                XPathNodeIterator nodeSQL = xPathDocNavigator.Select("/root/sql");
                if (nodeSQL.Count == 0)
                {
                    ApendLine("Команди відсутні!");
                }
                else
                {
                    while (nodeSQL!.MoveNext())
                    {
                        ApendLine(" - " + nodeSQL?.Current?.Value);
                    }

                    ApendLine("\n Для внесення змін - натисніть \"Збереження змін. Крок 2\"\n");
                }
            }
            else
            {
                ApendLine("Нова база даних");
            }

            ButtonSensitive(true);
        }

        void ExecuteSQLAndGenerateCode()
        {
            ButtonSensitive(false);

            ClearListBoxTerminal();

            string pathToSqlCommandFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf!.PathToXmlFileConfiguration)!, "ComparisonAnalize.xml");

            if (File.Exists(pathToSqlCommandFile))
            {
                //Read SQL
                List<string> SqlList = Configuration.ListComparisonSql(pathToSqlCommandFile);

                ApendLine("[ Виконання SQL ]\n");

                if (SqlList.Count == 0)
                {
                    ApendLine("Команди відсутні!");
                }
                else
                {
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
                }

                ApendLine("\nВидалення файлу команд " + pathToSqlCommandFile + "\n");
                File.Delete(pathToSqlCommandFile);
            }

            if (File.Exists(Conf.PathToTempXmlFileConfiguration))
            {
                ApendLine("Збереження конфігурації та видалення тимчасових файлів");
                Configuration.RewriteConfigurationFileFromTempFile(
                    Conf.PathToXmlFileConfiguration,
                    Conf.PathToTempXmlFileConfiguration,
                    System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, Conf.PathToCopyXmlFileConfiguration)
                );
            }

            if (File.Exists(System.IO.Path.Combine(PathToXsltTemplate, "CodeGeneration.xslt")))
            {
                ApendLine("\n[ Генерування коду ]\n");
                Configuration.GenerationCode(Conf.PathToXmlFileConfiguration,
                    System.IO.Path.Combine(PathToXsltTemplate, "CodeGeneration.xslt"),
                    System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "CodeGeneration.cs"));
            }

            ApendLine("ГОТОВО!");

            ButtonSensitive(true);
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