using Gtk;

using AccountingSoftware;

namespace Configurator
{
    class PageMaintenance : VBox
    {
        Configuration? Conf
        {
            get
            {
                return Program.Kernel?.Conf;
            }
        }

        public FormConfigurator? GeneralForm { get; set; }
        CancellationTokenSource? CancellationTokenThread { get; set; }

        Button bOk;
        Button bStop;
        Button bClose;

        ScrolledWindow scrollListBoxTerminal;
        TextView textTerminal;

        public PageMaintenance() : base()
        {
            new VBox();
            HBox hBox = new HBox();

            bOk = new Button("Оптимізувати");
            bOk.Clicked += OnOkClick;

            hBox.PackStart(bOk, false, false, 10);

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
            Gtk.Application.Invoke
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
            Gtk.Application.Invoke
            (
                delegate
                {
                    textTerminal.Buffer.InsertAtCursor(text + "\n");
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

        void MaintenanceTable()
        {
            ButtonSensitive(false);
            ClearListBoxTerminal();

            ApendLine("Структура бази даних");
            ConfigurationInformationSchema informationSchema = Program.Kernel!.DataBase.SelectInformationSchema();

            ApendLine("Таблиць: " + informationSchema.Tables.Count);
            ApendLine("");

            ApendLine("Обробка таблиць:");

            foreach (ConfigurationInformationSchema_Table table in informationSchema.Tables.Values)
            {
                if (CancellationTokenThread!.IsCancellationRequested)
                    break;

                ApendLine($" --> {table.TableName}");

                string query = $@"VACUUM FULL {table.TableName};";

                Program.Kernel.DataBase.ExecuteSQL(query);
            }

            ApendLine("");
            ApendLine("Готово!");

            ButtonSensitive(true);
        }
    }
}