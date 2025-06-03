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

namespace Configurator
{
    class PageMaintenance : Box
    {
        Configuration Conf { get { return Program.Kernel.Conf; } }

        public FormConfigurator? GeneralForm { get; set; }
        CancellationTokenSource? CancellationTokenThread { get; set; }

        #region Fields

        Button bOk;
        Button bStop;
        Button bClose;

        ScrolledWindow scrollListBoxTerminal;
        TextView textTerminal;

        #endregion

        public PageMaintenance() : base(Orientation.Vertical, 0)
        {
            Box hBox = new Box(Orientation.Horizontal, 0);

            bOk = new Button("Оптимізувати");
            bOk.Clicked += OnOkClick;

            hBox.PackStart(bOk, false, false, 10);

            bStop = new Button("Зупинити") { Sensitive = false };
            bStop.Clicked += OnStopClick;

            hBox.PackStart(bStop, false, false, 10);

            bClose = new Button("Закрити");
            bClose.Clicked += (object? sender, EventArgs args) => GeneralForm?.CloseCurrentPageNotebook();

            hBox.PackStart(bClose, false, false, 10);

            PackStart(hBox, false, false, 10);

            //Terminal
            Box hBoxTerminal = new Box(Orientation.Horizontal, 0);
            PackStart(hBoxTerminal, true, true, 5);

            scrollListBoxTerminal = new ScrolledWindow();
            scrollListBoxTerminal.KineticScrolling = true;
            scrollListBoxTerminal.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollListBoxTerminal.Add(textTerminal = new TextView());

            hBoxTerminal.PackStart(scrollListBoxTerminal, true, true, 5);

            ShowAll();
        }

        void OnOkClick(object? sender, EventArgs args)
        {
            CancellationTokenThread = new CancellationTokenSource();
            Thread thread = new Thread(new ThreadStart(MaintenanceTable));
            thread.Start();
        }

        void OnStopClick(object? sender, EventArgs args)
        {
            CancellationTokenThread?.Cancel();
        }

        void ButtonSensitive(bool sensitive)
        {
            Application.Invoke
            (
                delegate
                {
                    bOk.Sensitive = sensitive;
                    bClose.Sensitive = sensitive;

                    textTerminal.Sensitive = sensitive;

                    bStop.Sensitive = !sensitive;
                }
            );
        }

        void ApendLine(string text)
        {
            Application.Invoke
            (
                delegate
                {
                    textTerminal.Buffer.InsertAtCursor(text + "\n");
                    scrollListBoxTerminal.Vadjustment.Value = scrollListBoxTerminal.Vadjustment.Upper;
                }
            );
        }

        void ClearListBoxTerminal()
        {
            Application.Invoke
            (
                delegate
                {
                    textTerminal.Buffer.Text = "";
                }
            );
        }

        async void MaintenanceTable()
        {
            ButtonSensitive(false);
            ClearListBoxTerminal();

            ApendLine("Структура бази даних");
            ConfigurationInformationSchema informationSchema = await Program.Kernel.DataBase.SelectInformationSchema();

            ApendLine("Таблиць: " + informationSchema.Tables.Count);
            ApendLine("");

            ApendLine("Обробка таблиць:");

            foreach (ConfigurationInformationSchema_Table table in informationSchema.Tables.Values)
            {
                if (CancellationTokenThread!.IsCancellationRequested)
                    break;

                ApendLine($" --> {table.TableName}");

                string query = $@"VACUUM FULL {table.TableName};";

                // 5 хв таймаут 
                await Program.Kernel.DataBase.ExecuteSQL(query, 0, 300);
            }

            ApendLine("");
            ApendLine("Готово!");

            ButtonSensitive(true);

            Thread.Sleep(1000);
            ApendLine("\n\n\n");
        }
    }
}