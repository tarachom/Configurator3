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

using AccountingSoftware;

namespace Configurator
{
    class PageShema : VBox
    {
        readonly object loked = new Object();
        public FormConfigurator? GeneralForm { get; set; }

        TreeStore? Store;
        TreeView? treeView;

        public PageShema() : base()
        {
            new VBox();
            HBox hBox = new HBox();

            Button bClose = new Button("Закрити");
            bClose.Clicked += (object? sender, EventArgs args) => { GeneralForm?.CloseCurrentPageNotebook(); };

            hBox.PackStart(bClose, false, false, 10);

            Button bShema = new Button("Завантажити схему");
            bShema.Clicked += OnShema;

            hBox.PackStart(bShema, false, false, 10);

            PackStart(hBox, false, false, 10);

            //Shema
            HBox hBoxShema = new HBox();
            PackStart(hBoxShema, true, true, 5);

            AddColumn();

            ScrolledWindow scroll = new ScrolledWindow() { ShadowType = ShadowType.In };
            scroll.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scroll.Add(treeView);

            hBoxShema.PackStart(scroll, true, true, 10);

            ShowAll();
        }

        void AddColumn()
        {
            Store = new TreeStore(typeof(string), typeof(string), typeof(string));
            treeView = new TreeView(Store);

            treeView.AppendColumn(new TreeViewColumn("Таблиці / стовпчики", new CellRendererText(), "text", 0));
            treeView.AppendColumn(new TreeViewColumn("Тип даних", new CellRendererText(), "text", 1));
            treeView.AppendColumn(new TreeViewColumn("Тип даних", new CellRendererText(), "text", 2));
        }

        public void LoadShema()
        {
            Thread thread = new Thread(new ThreadStart(LoadShemaAsync));
            thread.Start();
        }

        async void LoadShemaAsync()
        {
            //Структура бази даних
            ConfigurationInformationSchema informationSchema = await Program.Kernel.DataBase.SelectInformationSchema();

            lock (loked)
            {
                Gtk.Application.Invoke(
                    delegate
                    {
                        Store!.Clear();

                        TreeIter rootIter = Store.AppendValues(" Схема ");

                        foreach (ConfigurationInformationSchema_Table table in informationSchema.Tables.Values)
                        {
                            TreeIter IterTable = Store.AppendValues(rootIter, table.TableName);

                            foreach (ConfigurationInformationSchema_Column column in table.Columns.Values)
                                Store.AppendValues(IterTable, column.ColumnName, column.DataType, column.UdtName);

                            if (table.Indexes.Count != 0)
                            {
                                TreeIter IterIndex = Store.AppendValues(IterTable, "[ Індекси ] ");
                                foreach (ConfigurationInformationSchema_Index index in table.Indexes.Values)
                                    Store.AppendValues(IterIndex, index.IndexName);
                            }
                        }

                        treeView!.ExpandToPath(treeView.Model.GetPath(rootIter));
                    }
                );
            }
        }

        void OnShema(object? sender, EventArgs args)
        {
            LoadShema();
        }
    }
}