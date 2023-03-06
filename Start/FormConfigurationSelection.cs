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
    public class FormConfigurationSelection : Window
    {
        /// <summary>
        /// Варіант запуску форми
        /// </summary>
        public enum TypeForm
        {
            Configurator,
            WorkingProgram
        }

        public virtual TypeForm TypeOpenForm { get; } = TypeForm.Configurator;

        protected ListBox listBox;

        public FormConfigurationSelection() : base("Вибір бази даних")
        {
            SetPosition(WindowPosition.Center);
            Resizable = false;
            BorderWidth = 4;

            if (File.Exists(Program.IcoFileName)) SetDefaultIconFromFile(Program.IcoFileName);

            DeleteEvent += delegate { Application.Quit(); };

            VBox vBox = new VBox();
            Add(vBox);

            CreateToolbar(vBox);

            //Containers -->
            HBox hBoxContainer = new HBox();
            vBox.PackStart(hBoxContainer, false, false, 0);

            VBox vBoxContainerLeft = new VBox() { WidthRequest = 500 };
            hBoxContainer.PackStart(vBoxContainerLeft, false, false, 0);

            VBox vBoxContainerRight = new VBox() { WidthRequest = 100 };
            hBoxContainer.PackStart(vBoxContainerRight, false, false, 0);
            //<--

            //Список
            {
                HBox hBox = new HBox();
                vBoxContainerLeft.PackStart(hBox, false, false, 2);

                ScrolledWindow scroll = new ScrolledWindow();
                scroll.SetSizeRequest(500, 280);
                scroll.ShadowType = ShadowType.In;
                scroll.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

                listBox = new ListBox();
                listBox.SelectionMode = SelectionMode.Single;
                listBox.ButtonPressEvent += OnListBoxDataBaseButtonPress;
                scroll.Add(listBox);

                hBox.PackStart(scroll, false, false, 4);
            }

            //Кнопки
            if (TypeOpenForm == TypeForm.WorkingProgram)
            {
                HBox hBoxOpen = new HBox() { Halign = Align.Start };
                vBoxContainerRight.PackStart(hBoxOpen, false, false, 2);

                Button buttonOpen = new Button("Відкрити") { WidthRequest = 140 };
                buttonOpen.Clicked += OnButtonOpenClicked;

                hBoxOpen.PackStart(buttonOpen, false, false, 2);

                Shown += (object? sender, EventArgs args) =>
                {
                    buttonOpen.GrabFocus();
                };

                HBox hBoxSeparator = new HBox();
                vBoxContainerRight.PackStart(hBoxSeparator, false, false, 4);
            }

            HBox hBoxConfigurator = new HBox() { Halign = Align.Start };
            vBoxContainerRight.PackStart(hBoxConfigurator, false, false, 2);

            Button buttonConfigurator = new Button("Конфігуратор") { WidthRequest = 140 };
            buttonConfigurator.Clicked += OnButtonOpenConfiguratorClicked;

            hBoxConfigurator.PackStart(buttonConfigurator, false, false, 2);

            if (TypeOpenForm == TypeForm.Configurator)
                Shown += (object? sender, EventArgs args) =>
                {
                    buttonConfigurator.GrabFocus();
                };

            ShowAll();

            LoadConfigurationParam();
            FillListBoxDataBase();
        }

        void CreateToolbar(VBox vBox)
        {
            HBox hBoxToolbar = new HBox();

            Toolbar toolbar = new Toolbar();
            hBoxToolbar.PackStart(toolbar, true, true, 0);

            ToolButton addButton = new ToolButton(Stock.Add) { TooltipText = "Додати" };
            addButton.Clicked += OnButtonAddClicked;
            toolbar.Add(addButton);

            ToolButton upButton = new ToolButton(Stock.Edit) { TooltipText = "Редагувати" };
            upButton.Clicked += OnButtonEditClicked;
            toolbar.Add(upButton);

            ToolButton refreshButton = new ToolButton(Stock.Copy) { TooltipText = "Копіювати" };
            refreshButton.Clicked += OnButtonCopyClicked;
            toolbar.Add(refreshButton);

            ToolButton deleteButton = new ToolButton(Stock.Delete) { TooltipText = "Видалити" };
            deleteButton.Clicked += OnButtonDeleteClicked;
            toolbar.Add(deleteButton);

            vBox.PackStart(hBoxToolbar, false, false, 0);
        }

        void LoadConfigurationParam()
        {
            ConfigurationParamCollection.PathToXML = System.IO.Path.Combine(AppContext.BaseDirectory, "ConfigurationParam.xml");
            ConfigurationParamCollection.LoadConfigurationParamFromXML(ConfigurationParamCollection.PathToXML);
        }

        void FillListBoxDataBase(string selectConfKey = "")
        {
            foreach (Widget child in listBox.Children)
                listBox.Remove(child);

            foreach (ConfigurationParam itemConfigurationParam in ConfigurationParamCollection.ListConfigurationParam!)
            {
                ListBoxRow row = new ListBoxRow();
                row.Name = itemConfigurationParam.ConfigurationKey;

                Label itemLabel = new Label(itemConfigurationParam.ToString());
                itemLabel.Halign = Align.Start;

                row.Add(itemLabel);
                listBox.Add(row);

                if (!String.IsNullOrEmpty(selectConfKey))
                {
                    if (itemConfigurationParam.ConfigurationKey == selectConfKey)
                        listBox.SelectRow(row);
                }
                else
                {
                    if (itemConfigurationParam.Select)
                        listBox.SelectRow(row);
                }
            }

            listBox.ShowAll();

            if (listBox.Children.Length != 0 && listBox.SelectedRow == null)
            {
                ListBoxRow row = (ListBoxRow)listBox.Children[0];
                listBox.SelectRow(row);
            }

            //scrolledWindowListBox.Vadjustment.Value = scrolledWindowListBox.Vadjustment.Upper;
        }

        void CallBackUpdate(ConfigurationParam itemConfigurationParam)
        {
            ConfigurationParamCollection.UpdateConfigurationParam(itemConfigurationParam);
            ConfigurationParamCollection.SaveConfigurationParamFromXML(ConfigurationParamCollection.PathToXML);

            FillListBoxDataBase(itemConfigurationParam.ConfigurationKey);
        }

        public virtual void Open() { }

        void OnButtonOpenClicked(object? sender, EventArgs args)
        {
            Open();
        }

        void OnButtonOpenConfiguratorClicked(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBox.SelectedRows;

            if (selectedRows.Length != 0)
            {
                ConfigurationParam? OpenConfigurationParam = ConfigurationParamCollection.GetConfigurationParam(selectedRows[0].Name);

                if (OpenConfigurationParam == null)
                    return;

                ConfigurationParamCollection.SelectConfigurationParam(selectedRows[0].Name);
                ConfigurationParamCollection.SaveConfigurationParamFromXML(ConfigurationParamCollection.PathToXML);

                string PathToConfXML = System.IO.Path.Combine(AppContext.BaseDirectory, "Confa.xml");

                Program.Kernel = new Kernel();

                Exception exception;
                bool flagOpen = Program.Kernel.Open(PathToConfXML,
                    OpenConfigurationParam.DataBaseServer,
                    OpenConfigurationParam.DataBaseLogin,
                    OpenConfigurationParam.DataBasePassword,
                    OpenConfigurationParam.DataBasePort,
                    OpenConfigurationParam.DataBaseBaseName,
                    out exception);

                if (flagOpen)
                {
                    FormConfigurator сonfigurator = new FormConfigurator();
                    сonfigurator.OpenConfigurationParam = ConfigurationParamCollection.GetConfigurationParam(selectedRows[0].Name);
                    сonfigurator.Show();

                    сonfigurator.SetValue();

                    сonfigurator.LoadTreeAsync();

                    Hide();
                }
                else
                    Message.Error(this, "Error: " + exception.Message);
            }
        }

        void OnListBoxDataBaseButtonPress(object? sender, ButtonPressEventArgs args)
        {
            if (args.Event.Type == Gdk.EventType.DoubleButtonPress)
                OnButtonEditClicked(null, new EventArgs());
        }

        void OnButtonAddClicked(object? sender, EventArgs args)
        {
            ConfigurationParam itemConfigurationParam = ConfigurationParam.New();
            ConfigurationParamCollection.ListConfigurationParam?.Add(itemConfigurationParam);

            ConfigurationParamCollection.SaveConfigurationParamFromXML(ConfigurationParamCollection.PathToXML);
            FillListBoxDataBase(itemConfigurationParam.ConfigurationKey);
        }

        void OnButtonCopyClicked(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBox.SelectedRows;

            if (selectedRows.Length != 0)
            {
                ConfigurationParam? itemConfigurationParam = ConfigurationParamCollection.GetConfigurationParam(selectedRows[0].Name);
                if (itemConfigurationParam != null)
                {
                    ConfigurationParam copyConfigurationParam = itemConfigurationParam.Clone();
                    ConfigurationParamCollection.ListConfigurationParam?.Add(copyConfigurationParam);

                    ConfigurationParamCollection.SaveConfigurationParamFromXML(ConfigurationParamCollection.PathToXML);
                    FillListBoxDataBase(itemConfigurationParam.ConfigurationKey);
                }
            }
        }

        void OnButtonDeleteClicked(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBox.SelectedRows;

            if (selectedRows.Length != 0)
            {
                if (Message.Request(this, "Видалити?") == ResponseType.Yes)
                    if (ConfigurationParamCollection.RemoveConfigurationParam(selectedRows[0].Name))
                    {
                        ConfigurationParamCollection.SaveConfigurationParamFromXML(ConfigurationParamCollection.PathToXML);
                        FillListBoxDataBase();
                    }
            }
        }
        
        void OnButtonEditClicked(object? sender, EventArgs args)
        {
            ListBoxRow[] selectedRows = listBox.SelectedRows;

            if (selectedRows.Length != 0)
            {
                FormConfigurationSelectionParam configurationSelectionParam = new FormConfigurationSelectionParam();
                configurationSelectionParam.Modal = true;
                configurationSelectionParam.TransientFor = this;
                configurationSelectionParam.Resizable = false;
                configurationSelectionParam.OpenConfigurationParam = ConfigurationParamCollection.GetConfigurationParam(selectedRows[0].Name);
                configurationSelectionParam.CallBackUpdate = CallBackUpdate;
                configurationSelectionParam.Show();
            }
        }
    }
}