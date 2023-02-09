/*
Copyright (C) 2019-2023 TARAKHOMYN YURIY IVANOVYCH
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

using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;

using AccountingSoftware;

namespace Configurator
{
    class PageUnloadingAndLoadingData : VBox
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
        CancellationTokenSource? CancellationTokenThread { get; set; }

        #region Fields

        Button bUnloading;
        Button bLoading;
        Button bStop;
        Button bClose;

        ScrolledWindow scrollListBoxTerminal;
        TextView textTerminal;

        #endregion

        public PageUnloadingAndLoadingData() : base()
        {
            new VBox();
            HBox hBox = new HBox();

            bLoading = new Button("Вигрузка");
            bLoading.Clicked += OnLoadingClick;
            hBox.PackStart(bLoading, false, false, 10);

            bUnloading = new Button("Загрузка");
            bUnloading.Clicked += OnUnloadingClick;
            hBox.PackStart(bUnloading, false, false, 10);

            bStop = new Button("Зупинити") { Sensitive = false };
            bStop.Clicked += OnStopClick;

            hBox.PackStart(bStop, false, false, 10);

            bClose = new Button("Закрити");
            bClose.Clicked += (object? sender, EventArgs args) => { GeneralForm?.CloseCurrentPageNotebook(); };
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

        void OnStopClick(object? sender, EventArgs args)
        {
            CancellationTokenThread?.Cancel();
        }

        void OnUnloadingClick(object? sender, EventArgs args)
        {
            string fileImport = "";
            bool fileSelect = false;

            FileChooserDialog fc = new FileChooserDialog("Виберіть файл для загрузки даних", GeneralForm,
                FileChooserAction.Open, "Закрити", ResponseType.Cancel, "Вибрати", ResponseType.Accept);

            fc.Filter = new FileFilter();
            fc.Filter.AddPattern("*.xml");

            if (fc.Run() == (int)ResponseType.Accept)
            {
                if (!String.IsNullOrEmpty(fc.Filename))
                {
                    fileImport = fc.Filename;
                    fileSelect = true;
                }
            }

            fc.Dispose();
            fc.Destroy();

            if (fileSelect)
            {
                CancellationTokenThread = new CancellationTokenSource();
                Thread thread = new Thread(new ParameterizedThreadStart(ImportData));
                thread.Start(fileImport);
            }
        }

        void OnLoadingClick(object? sender, EventArgs args)
        {
            string fileName = Conf!.Name + "_Export_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xml";
            string fullPath = "";

            bool fileSelect = false;

            FileChooserDialog fc = new FileChooserDialog("Виберіть каталог для вигрузки даних", GeneralForm,
                FileChooserAction.SelectFolder, "Закрити", ResponseType.Cancel, "Вибрати", ResponseType.Accept);

            if (fc.Run() == (int)ResponseType.Accept)
            {
                if (!String.IsNullOrEmpty(fc.CurrentFolder))
                {
                    fullPath = System.IO.Path.Combine(fc.CurrentFolder, fileName);
                    fileSelect = true;
                }
            }

            fc.Dispose();
            fc.Destroy();

            if (fileSelect)
            {
                CancellationTokenThread = new CancellationTokenSource();
                Thread thread = new Thread(new ParameterizedThreadStart(ExportData));
                thread.Start(fullPath);
            }
        }

        void ButtonSensitive(bool sensitive)
        {
            Gtk.Application.Invoke
            (
                delegate
                {
                    bUnloading.Sensitive = sensitive;
                    bLoading.Sensitive = sensitive;
                    bClose.Sensitive = sensitive;

                    textTerminal.Sensitive = sensitive;

                    bStop.Sensitive = !sensitive;
                }
            );
        }

        void ApendLine(string text)
        {
            Gtk.Application.Invoke
            (
                delegate
                {
                    textTerminal.Buffer.InsertAtCursor(text + "\n");
                    scrollListBoxTerminal.Vadjustment.Value = scrollListBoxTerminal.Vadjustment.Upper;
                }
            );
        }

        void ApendInfo(string text)
        {
            Gtk.Application.Invoke
            (
                delegate
                {
                    textTerminal.Buffer.InsertAtCursor(text);
                    scrollListBoxTerminal.Vadjustment.Value = scrollListBoxTerminal.Vadjustment.Upper;
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

        #region Export

        /// <summary>
        /// Вигрузка даних
        /// </summary>
        /// <param name="fileExport">Файл вигрузки</param>
        void ExportData(object? fileExport)
        {
            if (fileExport == null)
                return;

            string fileExportPath = fileExport.ToString()!;

            ButtonSensitive(false);
            ClearListBoxTerminal();

            ApendLine("Файл вигрузки: " + fileExportPath + "\n\n");

            XmlWriterSettings? settings = new XmlWriterSettings() { Indent = true, Encoding = System.Text.Encoding.UTF8 };
            XmlWriter xmlWriter;

            try
            {
                xmlWriter = XmlWriter.Create(fileExportPath, settings);
            }
            catch (Exception ex)
            {
                ApendLine("Помилка створення файлу:\n" + ex.Message);
                ButtonSensitive(true);

                return;
            }

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("root");

            if (!CancellationTokenThread!.IsCancellationRequested)
            {
                ApendLine("КОНСТАНТИ");

                xmlWriter.WriteStartElement("Constants");
                foreach (ConfigurationConstantsBlock configurationConstantsBlock in Conf!.ConstantsBlock.Values)
                {
                    if (CancellationTokenThread.IsCancellationRequested)
                        break;

                    ApendLine(configurationConstantsBlock.BlockName);

                    foreach (ConfigurationConstants configurationConstants in configurationConstantsBlock.Constants.Values)
                    {
                        if (CancellationTokenThread.IsCancellationRequested)
                            break;

                        ApendLine(" --> Константа: " + configurationConstants.Name);

                        xmlWriter.WriteStartElement("Constant");
                        xmlWriter.WriteAttributeString("name", configurationConstants.Name);
                        xmlWriter.WriteAttributeString("col", configurationConstants.NameInTable);

                        foreach (ConfigurationObjectTablePart tablePart in configurationConstants.TabularParts.Values)
                        {
                            if (CancellationTokenThread.IsCancellationRequested)
                                break;

                            xmlWriter.WriteStartElement("TablePart");
                            xmlWriter.WriteAttributeString("name", tablePart.Name);
                            xmlWriter.WriteAttributeString("tab", tablePart.Table);

                            WriteQuerySelect(xmlWriter, $@"SELECT uid{GetAllFields(tablePart.Fields)} FROM {tablePart.Table}");

                            xmlWriter.WriteEndElement();
                        }

                        WriteQuerySelect(xmlWriter, $@"SELECT {configurationConstants.NameInTable} FROM tab_constants");

                        xmlWriter.WriteEndElement(); //Constant
                    }
                }
                xmlWriter.WriteEndElement(); //Constants
            }

            if (!CancellationTokenThread.IsCancellationRequested)
            {
                ApendLine("ДОВІДНИКИ");

                xmlWriter.WriteStartElement("Directories");
                foreach (ConfigurationDirectories configurationDirectories in Conf!.Directories.Values)
                {
                    if (CancellationTokenThread.IsCancellationRequested)
                        break;

                    ApendLine(" --> Довідник: " + configurationDirectories.Name);

                    xmlWriter.WriteStartElement("Directory");
                    xmlWriter.WriteAttributeString("name", configurationDirectories.Name);
                    xmlWriter.WriteAttributeString("tab", configurationDirectories.Table);

                    WriteQuerySelect(xmlWriter, $@"SELECT uid{GetAllFields(configurationDirectories.Fields)} FROM {configurationDirectories.Table}");

                    foreach (ConfigurationObjectTablePart tablePart in configurationDirectories.TabularParts.Values)
                    {
                        if (CancellationTokenThread.IsCancellationRequested)
                            break;

                        xmlWriter.WriteStartElement("TablePart");
                        xmlWriter.WriteAttributeString("name", tablePart.Name);
                        xmlWriter.WriteAttributeString("tab", tablePart.Table);

                        WriteQuerySelect(xmlWriter, $@"SELECT uid, owner{GetAllFields(tablePart.Fields)} FROM {tablePart.Table}");

                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteEndElement(); //Directory
                }
                xmlWriter.WriteEndElement(); //Directories
            }

            if (!CancellationTokenThread.IsCancellationRequested)
            {
                ApendLine("ДОКУМЕНТИ");

                xmlWriter.WriteStartElement("Documents");
                foreach (ConfigurationDocuments configurationDocuments in Conf!.Documents.Values)
                {
                    if (CancellationTokenThread.IsCancellationRequested)
                        break;

                    ApendLine(" --> Документ: " + configurationDocuments.Name);

                    xmlWriter.WriteStartElement("Document");
                    xmlWriter.WriteAttributeString("name", configurationDocuments.Name);
                    xmlWriter.WriteAttributeString("tab", configurationDocuments.Table);

                    WriteQuerySelect(xmlWriter, $@"SELECT uid, spend, spend_date{GetAllFields(configurationDocuments.Fields)} FROM {configurationDocuments.Table}");

                    foreach (ConfigurationObjectTablePart tablePart in configurationDocuments.TabularParts.Values)
                    {
                        if (CancellationTokenThread.IsCancellationRequested)
                            break;

                        xmlWriter.WriteStartElement("TablePart");
                        xmlWriter.WriteAttributeString("name", tablePart.Name);
                        xmlWriter.WriteAttributeString("tab", tablePart.Table);

                        WriteQuerySelect(xmlWriter, $@"SELECT uid, owner{GetAllFields(tablePart.Fields)} FROM {tablePart.Table}");

                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteEndElement(); //Document
                }
                xmlWriter.WriteEndElement(); //Documents
            }

            if (!CancellationTokenThread.IsCancellationRequested)
            {
                ApendLine("РЕГІСТРИ ІНФОРМАЦІЇ");

                xmlWriter.WriteStartElement("RegistersInformation");
                foreach (ConfigurationRegistersInformation configurationRegistersInformation in Conf!.RegistersInformation.Values)
                {
                    if (CancellationTokenThread.IsCancellationRequested)
                        break;

                    ApendLine(" --> Регістр: " + configurationRegistersInformation.Name);

                    xmlWriter.WriteStartElement("Register");
                    xmlWriter.WriteAttributeString("name", configurationRegistersInformation.Name);
                    xmlWriter.WriteAttributeString("tab", configurationRegistersInformation.Table);

                    string query_fields = GetAllFields(configurationRegistersInformation.DimensionFields) +
                        GetAllFields(configurationRegistersInformation.ResourcesFields) +
                        GetAllFields(configurationRegistersInformation.PropertyFields);

                    WriteQuerySelect(xmlWriter, $@"SELECT uid, period, owner{query_fields} FROM {configurationRegistersInformation.Table}");

                    xmlWriter.WriteEndElement(); //Register
                }
                xmlWriter.WriteEndElement(); //RegistersInformation
            }

            if (!CancellationTokenThread.IsCancellationRequested)
            {
                ApendLine("РЕГІСТРИ НАКОПИЧЕННЯ");

                xmlWriter.WriteStartElement("RegistersAccumulation");
                foreach (ConfigurationRegistersAccumulation configurationRegistersAccumulation in Conf!.RegistersAccumulation.Values)
                {
                    if (CancellationTokenThread.IsCancellationRequested)
                        break;

                    ApendLine(" --> Регістр: " + configurationRegistersAccumulation.Name);

                    xmlWriter.WriteStartElement("Register");
                    xmlWriter.WriteAttributeString("name", configurationRegistersAccumulation.Name);
                    xmlWriter.WriteAttributeString("tab", configurationRegistersAccumulation.Table);

                    string query_fields = GetAllFields(configurationRegistersAccumulation.DimensionFields) +
                        GetAllFields(configurationRegistersAccumulation.ResourcesFields) +
                        GetAllFields(configurationRegistersAccumulation.PropertyFields);

                    WriteQuerySelect(xmlWriter, $@"SELECT uid, period, income, owner{query_fields} FROM {configurationRegistersAccumulation.Table}");

                    foreach (ConfigurationObjectTablePart tablePart in configurationRegistersAccumulation.TabularParts.Values)
                    {
                        if (CancellationTokenThread.IsCancellationRequested)
                            break;

                        xmlWriter.WriteStartElement("TablePart");
                        xmlWriter.WriteAttributeString("name", tablePart.Name);
                        xmlWriter.WriteAttributeString("tab", tablePart.Table);

                        WriteQuerySelect(xmlWriter, $@"SELECT uid{GetAllFields(tablePart.Fields)} FROM {tablePart.Table}");

                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteEndElement(); //Register
                }
                xmlWriter.WriteEndElement(); //RegistersAccumulation
            }

            xmlWriter.WriteEndElement(); //root
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();

            ApendLine("");
            ApendLine("Готово!");

            ButtonSensitive(true);
        }

        /// <summary>
        /// Стрічка для SQL запиту із списком полів
        /// </summary>
        /// <param name="fields">Колекція полів</param>
        /// <returns>Список полів</returns>
        string GetAllFields(Dictionary<string, ConfigurationObjectField> fields)
        {
            string guery_fields = "";

            foreach (ConfigurationObjectField field in fields.Values)
                guery_fields += $", {field.NameInTable}";

            return guery_fields;
        }

        #region Info (додаткові відомості у файл вигрузки)

        void WriteFieldsInfo(XmlWriter xmlWriter, Dictionary<string, ConfigurationObjectField> fields)
        {
            xmlWriter.WriteStartElement("FieldInfo");
            foreach (ConfigurationObjectField field in fields.Values)
            {
                xmlWriter.WriteStartElement("Field");
                xmlWriter.WriteAttributeString("name", field.Name);
                xmlWriter.WriteAttributeString("col", field.NameInTable);
                xmlWriter.WriteAttributeString("type", field.Type);
                if (field.Type == "pointer" || field.Type == "enum")
                    xmlWriter.WriteAttributeString("pointer", field.Pointer);
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
        }

        void WriteTablePartInfo(XmlWriter xmlWriter, ConfigurationObjectTablePart tablePart)
        {
            xmlWriter.WriteStartElement("TablePart");
            xmlWriter.WriteAttributeString("name", tablePart.Name);
            xmlWriter.WriteAttributeString("tab", tablePart.Table);

            WriteFieldsInfo(xmlWriter, tablePart.Fields);

            xmlWriter.WriteEndElement();
        }

        void WriteTabularPartsInfo(XmlWriter xmlWriter, Dictionary<string, ConfigurationObjectTablePart> tabularParts)
        {
            if (tabularParts.Count > 0)
            {
                xmlWriter.WriteStartElement("TabularPartsInfo");
                foreach (ConfigurationObjectTablePart tablePart in tabularParts.Values)
                    WriteTablePartInfo(xmlWriter, tablePart);
                xmlWriter.WriteEndElement(); //TabularPartsInfo
            }
        }

        #endregion

        /// <summary>
        /// Виконання запиту та запис даних
        /// </summary>
        /// <param name="xmlWriter">ХМЛ</param>
        /// <param name="query">Запит</param>
        void WriteQuerySelect(XmlWriter xmlWriter, string query)
        {
            string[] columnsName = new string[] { };
            List<object[]> listRow = new List<object[]>();
            Dictionary<string, object> paramQuery = new Dictionary<string, object>();

            Program.Kernel?.DataBase.SelectRequest(query, paramQuery, out columnsName, out listRow);

            foreach (object[] row in listRow)
            {
                if (CancellationTokenThread!.IsCancellationRequested)
                    break;

                int counter = 0;

                xmlWriter.WriteStartElement("row");
                foreach (string column in columnsName)
                {
                    string typeName = row[counter].GetType().Name;

                    if (typeName != "DBNull")
                    {
                        xmlWriter.WriteStartElement(column);
                        xmlWriter.WriteAttributeString("type", typeName);

                        switch (typeName)
                        {
                            case "String[]":
                                {
                                    xmlWriter.WriteRaw(ArrayToXml<string>.Convert((string[])row[counter]));
                                    break;
                                }
                            case "Int32[]":
                                {
                                    xmlWriter.WriteRaw(ArrayToXml<int>.Convert((int[])row[counter]));
                                    break;
                                }
                            case "Decimal[]":
                                {
                                    xmlWriter.WriteRaw(ArrayToXml<decimal>.Convert((decimal[])row[counter]));
                                    break;
                                }
                            case "UuidAndText":
                                {
                                    xmlWriter.WriteRaw(((UuidAndText)row[counter]).ToXml());
                                    break;
                                }
                            case "Byte[]":
                                {
                                    xmlWriter.WriteRaw("");
                                    break;
                                }
                            default:
                                {
                                    xmlWriter.WriteString(row[counter].ToString());
                                    break;
                                }
                        }

                        xmlWriter.WriteEndElement();
                    }

                    counter++;
                }
                xmlWriter.WriteEndElement();
            }

            xmlWriter.Flush();
        }

        #endregion

        #region Import

        /// <summary>
        /// Загрузка даних
        /// </summary>
        /// <param name="fileImport">Файл загрузки</param>
        void ImportData(object? fileImport)
        {
            if (fileImport == null)
                return;

            string fileImportPath = fileImport.ToString()!;

            ButtonSensitive(false);
            ClearListBoxTerminal();

            ApendLine("Файл загрузки: " + fileImport.ToString() + "\n\n");

            ApendLine("Аналіз: ");

            string pathToXmlResultStepOne = "";

            if (!CancellationTokenThread!.IsCancellationRequested)
            {
                ApendLine(" --> Крок 1");
                pathToXmlResultStepOne = TransformXmlDataStepOne(fileImportPath);

                if (String.IsNullOrEmpty(pathToXmlResultStepOne) && !System.IO.File.Exists(pathToXmlResultStepOne))
                {
                    ButtonSensitive(true);
                    return;
                }
            }

            string pathToXmlResultStepSQL = "";

            if (!CancellationTokenThread.IsCancellationRequested)
            {
                ApendLine(" --> Крок 2");
                pathToXmlResultStepSQL = TransformStepOneToStepSQL(fileImportPath, pathToXmlResultStepOne);

                if (String.IsNullOrEmpty(pathToXmlResultStepSQL) && !System.IO.File.Exists(pathToXmlResultStepSQL))
                {
                    if (System.IO.File.Exists(pathToXmlResultStepOne))
                        try
                        {
                            System.IO.File.Delete(pathToXmlResultStepOne);
                        }
                        catch { }

                    ButtonSensitive(true);
                    return;
                }
            }

            if (!CancellationTokenThread.IsCancellationRequested)
            {
                ApendLine("Виконання команд: ");

                byte TransactionID = Program.Kernel!.DataBase.BeginTransaction();
                bool resultat = false;

                try
                {
                    resultat = ExecuteSqlList(pathToXmlResultStepSQL, TransactionID);
                }
                catch (Exception ex)
                {
                    ApendLine("Помилка: " + ex.Message);

                    Program.Kernel!.DataBase.RollbackTransaction(TransactionID);

                    //Очистка тмп файлів
                    {
                        if (System.IO.File.Exists(pathToXmlResultStepOne))
                            try
                            {
                                System.IO.File.Delete(pathToXmlResultStepOne);
                            }
                            catch { }

                        if (System.IO.File.Exists(pathToXmlResultStepSQL))
                            try
                            {
                                System.IO.File.Delete(pathToXmlResultStepSQL);
                            }
                            catch { }
                    }

                    ButtonSensitive(true);
                    return;
                }

                if (resultat)
                    Program.Kernel.DataBase.CommitTransaction(TransactionID);
            }

            //Видалення тимчасових файлів
            File.Delete(pathToXmlResultStepOne);
            File.Delete(pathToXmlResultStepSQL);

            ApendLine(" --> Готово!");

            ButtonSensitive(true);
        }

        /// <summary>
        /// Трансформація даних Крок 1
        /// </summary>
        /// <param name="fileImport">Файл загрузки</param>
        /// <returns>Файл трансформації</returns>
        string TransformXmlDataStepOne(string fileImport)
        {
            string pathToTemplate = System.IO.Path.Combine(PathToXsltTemplate, "UnloadingAndLoadingDataXML.xslt");
            string pathToDirFileImport = System.IO.Path.GetDirectoryName(fileImport)!;
            string pathToXmlResult = System.IO.Path.Combine(pathToDirFileImport, "stepone_" + Guid.NewGuid().ToString().Replace("-", "") + ".xml");

            XslCompiledTransform xsltCodeGnerator = new XslCompiledTransform();
            xsltCodeGnerator.Load(pathToTemplate, new XsltSettings(false, false), null);

            XsltArgumentList xsltArgumentList = new XsltArgumentList();

            FileStream fileStream;

            try
            {
                fileStream = new FileStream(pathToXmlResult, FileMode.Create);
            }
            catch (Exception ex)
            {
                ApendLine("Помилка створення файлу:\n" + ex.Message);
                return "";
            }

            xsltCodeGnerator.Transform(fileImport, xsltArgumentList, fileStream);

            fileStream.Close();

            return pathToXmlResult;
        }

        /// <summary>
        /// Трансформація даних Крок 2
        /// </summary>
        /// <param name="fileImport">Файл загрузки</param>
        /// <param name="fileStepOne">Файл першої трансформації (Крок 1)</param>
        /// <returns>Файл трансформації</returns>
        string TransformStepOneToStepSQL(string fileImport, string fileStepOne)
        {
            string pathToTemplate = System.IO.Path.Combine(PathToXsltTemplate, "UnloadingAndLoadingDataSQL.xslt");
            string pathToDirFileImport = System.IO.Path.GetDirectoryName(fileImport)!;
            string pathToXmlResult = System.IO.Path.Combine(pathToDirFileImport, "stepsql_" + Guid.NewGuid().ToString().Replace("-", "") + ".xml");

            XslCompiledTransform xsltCodeGnerator = new XslCompiledTransform();
            xsltCodeGnerator.Load(pathToTemplate, new XsltSettings(false, false), null);

            XsltArgumentList xsltArgumentList = new XsltArgumentList();

            FileStream fileStream;

            try
            {
                fileStream = new FileStream(pathToXmlResult, FileMode.Create);
            }
            catch (Exception ex)
            {
                ApendLine("Помилка створення файлу:\n" + ex.Message);
                return "";
            }

            xsltCodeGnerator.Transform(fileStepOne, xsltArgumentList, fileStream);

            fileStream.Close();

            return pathToXmlResult;
        }

        /// <summary>
        /// Виконання SQL запитів
        /// </summary>
        /// <param name="fileStepSQL">Файл з запитами</param>
        public bool ExecuteSqlList(string fileStepSQL, byte transactionID)
        {
            int counter = 0;
            int iter = 0;

            XPathDocument xPathDoc = new XPathDocument(fileStepSQL);
            XPathNavigator xPathDocNavigator = xPathDoc.CreateNavigator();

            XPathNodeIterator rowNodes = xPathDocNavigator.Select("/root/row");
            while (rowNodes.MoveNext())
            {
                if (CancellationTokenThread!.IsCancellationRequested)
                    return false;

                XPathNavigator? sqlNode = rowNodes.Current?.SelectSingleNode("sql");
                string sqlText = sqlNode?.Value ?? "";

                Dictionary<string, object> param = new Dictionary<string, object>();

                XPathNodeIterator? paramNodes = rowNodes.Current?.Select("p");
                while (paramNodes!.MoveNext())
                {
                    string paramName = paramNodes.Current?.GetAttribute("name", "") ?? "";
                    string paramType = paramNodes.Current?.GetAttribute("type", "") ?? "";

                    string paramValue = paramNodes.Current?.Value ?? "";
                    object paramObj;

                    switch (paramType)
                    {
                        case "Guid":
                            {
                                paramObj = Guid.Parse(paramValue);
                                break;
                            }
                        case "Int32":
                            {
                                paramObj = int.Parse(paramValue);
                                break;
                            }
                        case "DateTime":
                            {
                                paramObj = DateTime.Parse(paramValue);
                                break;
                            }
                        case "TimeSpan":
                            {
                                paramObj = TimeSpan.Parse(paramValue);
                                break;
                            }
                        case "Boolean":
                            {
                                paramObj = Boolean.Parse(paramValue);
                                break;
                            }
                        case "Decimal":
                            {
                                paramObj = Decimal.Parse(paramValue);
                                break;
                            }
                        case "String":
                            {
                                paramObj = paramValue;
                                break;
                            }
                        case "String[]":
                            {
                                paramObj = ArrayToXml.Convert(paramNodes.Current?.InnerXml ?? "");
                                break;
                            }
                        case "Int32[]":
                            {
                                string[] tmpValue = ArrayToXml.Convert(paramNodes.Current?.InnerXml ?? "");
                                int[] tmpIntValue = new int[tmpValue.Length];

                                for (int i = 0; i < tmpValue.Length; i++)
                                    tmpIntValue[i] = int.Parse(tmpValue[i]);

                                paramObj = tmpIntValue;
                                break;
                            }
                        case "Decimal[]":
                            {
                                string[] tmpValue = ArrayToXml.Convert(paramNodes.Current?.InnerXml ?? "");
                                decimal[] tmpDecimalValue = new decimal[tmpValue.Length];

                                for (int i = 0; i < tmpValue.Length; i++)
                                    tmpDecimalValue[i] = decimal.Parse(tmpValue[i]);

                                paramObj = tmpDecimalValue;
                                break;
                            }
                        case "UuidAndText":
                            {
                                paramObj = ArrayToXml.ConvertUuidAndText(paramNodes.Current?.InnerXml ?? "");
                                break;
                            }
                        case "Byte[]":
                            {
                                paramObj = new Byte[] { };
                                break;
                            }
                        default:
                            {
                                ApendLine("Не оприділений тип: " + paramType);
                                paramObj = paramValue;
                                break;
                            }
                    }

                    param.Add(paramName, paramObj);
                }

                int result = Program.Kernel!.DataBase.ExecuteSQL(sqlText, param, transactionID);

                if (iter > 100)
                {
                    ApendInfo(".");
                    iter = 0;

                    counter++;
                }

                if (counter > 300)
                {
                    ApendInfo("\n");
                    counter = 0;
                }

                iter++;
            }

            return true;
        }

        #endregion
    }
}