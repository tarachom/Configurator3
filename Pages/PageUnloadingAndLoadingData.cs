using Gtk;

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

        Button bUnloading;
        Button bLoading;
        Button bClose;

        ScrolledWindow scrollListBoxTerminal;
        TextView textTerminal;

        public PageUnloadingAndLoadingData() : base()
        {
            new VBox();
            HBox hBox = new HBox();

            bUnloading = new Button("Unloading");
            bUnloading.Clicked += OnUnloadingClick;
            hBox.PackStart(bUnloading, false, false, 10);

            bLoading = new Button("Loading");
            bLoading.Clicked += OnLoadingClick;
            hBox.PackStart(bLoading, false, false, 10);

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

        void OnUnloadingClick(object? sender, EventArgs args)
        {

        }

        void OnLoadingClick(object? sender, EventArgs args)
        {

        }
    }
}