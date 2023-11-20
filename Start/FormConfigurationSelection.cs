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

        HBox hBoxContainerToolbar = new HBox(); //Для тулбара

        VBox vBoxContainerLeft; //Лівий для списку
        VBox vBoxContainerRight; //Правий для кнопок

        protected ListBox listBox; //Список
        Button buttonConfigurator; //Кнопка конфігуратор

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

            vBoxContainerLeft = new VBox() { WidthRequest = 500 };
            hBoxContainer.PackStart(vBoxContainerLeft, false, false, 0);

            vBoxContainerRight = new VBox() { WidthRequest = 100 };
            hBoxContainer.PackStart(vBoxContainerRight, false, false, 0);
            //<--

            //Список
            {
                HBox hBox = new HBox();
                vBoxContainerLeft.PackStart(hBox, false, false, 2);

                ScrolledWindow scroll = new ScrolledWindow() { ShadowType = ShadowType.In };
                scroll.SetSizeRequest(500, 280);
                scroll.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

                listBox = new ListBox { SelectionMode = SelectionMode.Single };
                listBox.ButtonPressEvent += OnListBoxDataBaseButtonPress;
                scroll.Add(listBox);

                hBox.PackStart(scroll, false, false, 4);
            }

            //Кнопка Відкрити
            if (TypeOpenForm == TypeForm.WorkingProgram)
            {
                HBox hBoxOpen = new HBox() { Halign = Align.Start };
                vBoxContainerRight.PackStart(hBoxOpen, false, false, 2);

                Button buttonOpen = new Button("Відкрити") { WidthRequest = 140 };
                buttonOpen.Clicked += OnButtonOpenClicked;

                hBoxOpen.PackStart(buttonOpen, false, false, 2);

                //Фокус для кнопки Відкрити після відкриття форми
                Shown += (object? sender, EventArgs args) => { buttonOpen.GrabFocus(); };
            }

            //Кнопка Конфігуратор
            {
                HBox hBoxConfigurator = new HBox() { Halign = Align.Start };
                vBoxContainerRight.PackStart(hBoxConfigurator, false, false, 2);

                buttonConfigurator = new Button("Конфігуратор") { WidthRequest = 140 };
                buttonConfigurator.Clicked += OnButtonOpenConfiguratorClicked;

                hBoxConfigurator.PackStart(buttonConfigurator, false, false, 2);

                if (TypeOpenForm == TypeForm.Configurator)
                    //Фокус для кнопки Конфігуратор після відкриття форми
                    Shown += (object? sender, EventArgs args) => { buttonConfigurator.GrabFocus(); };
            }

            ShowAll();

            LoadConfigurationParam();
            FillListBoxDataBase();
        }

        void CreateToolbar(VBox vBox)
        {
            Toolbar toolbar = new Toolbar();
            hBoxContainerToolbar.PackStart(toolbar, true, true, 0);

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

            vBox.PackStart(hBoxContainerToolbar, false, false, 0);
        }

        void LoadConfigurationParam()
        {
            ConfigurationParamCollection.PathToXML = System.IO.Path.Combine(AppContext.BaseDirectory, "ConfigurationParam.xml");
            ConfigurationParamCollection.LoadConfigurationParamFromXML(ConfigurationParamCollection.PathToXML);
        }

        void FillListBoxDataBase(string selectConfKey = "")
        {
            //Очищення у зворотньому напрямку
            for (int i = listBox.Children.Length - 1; i >= 0; i--)
                listBox.Remove(listBox.Children[i]);

            //Заповнення списку
            foreach (ConfigurationParam itemConfigurationParam in ConfigurationParamCollection.ListConfigurationParam!)
            {
                ListBoxRow row = new ListBoxRow() { Name = itemConfigurationParam.ConfigurationKey };
                row.Add(new Label(itemConfigurationParam.ToString()) { Halign = Align.Start });

                listBox.Add(row);

                if (!string.IsNullOrEmpty(selectConfKey))
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

            //Виділення першого елементу в списку
            if (listBox.Children.Length != 0 && listBox.SelectedRow == null)
            {
                ListBoxRow row = (ListBoxRow)listBox.Children[0];
                listBox.SelectRow(row);
            }
        }

        void CallBackUpdate(ConfigurationParam itemConfigurationParam)
        {
            ConfigurationParamCollection.UpdateConfigurationParam(itemConfigurationParam);
            ConfigurationParamCollection.SaveConfigurationParamFromXML(ConfigurationParamCollection.PathToXML);

            FillListBoxDataBase(itemConfigurationParam.ConfigurationKey);
        }

        public virtual async ValueTask Open() { await ValueTask.FromResult(true); }

        async void OnButtonOpenClicked(object? sender, EventArgs args)
        {
            if (sender == null) return;

            //Блокування кнопок і списку
            Button buttonOpen = (Button)sender;
            hBoxContainerToolbar.Sensitive = listBox.Sensitive =
            buttonConfigurator.Sensitive = buttonOpen.Sensitive = false;

            //Спінер на форму
            HBox hBoxSpinner = new HBox() { Halign = Align.Center };
            hBoxSpinner.PackStart(new Spinner() { Active = true }, false, false, 10);

            vBoxContainerRight.PackStart(hBoxSpinner, false, false, 2);
            vBoxContainerRight.ShowAll();

            await Open();

            //Видалення спінера
            vBoxContainerRight.Remove(hBoxSpinner);
            vBoxContainerRight.ShowAll();

            //Розблокування кнопок і списку
            hBoxContainerToolbar.Sensitive = listBox.Sensitive =
            buttonConfigurator.Sensitive = buttonOpen.Sensitive = true;
        }

        async void OnButtonOpenConfiguratorClicked(object? sender, EventArgs args)
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

                bool result = await Program.Kernel.Open(PathToConfXML,
                    OpenConfigurationParam.DataBaseServer,
                    OpenConfigurationParam.DataBaseLogin,
                    OpenConfigurationParam.DataBasePassword,
                    OpenConfigurationParam.DataBasePort,
                    OpenConfigurationParam.DataBaseBaseName
                );

                if (result)
                {
                    FormConfigurator сonfigurator = new FormConfigurator { OpenConfigurationParam = ConfigurationParamCollection.GetConfigurationParam(selectedRows[0].Name) };

                    сonfigurator.Show();
                    сonfigurator.SetValue();
                    сonfigurator.LoadTreeAsync();

                    Hide();
                }
                else
                    Message.Error(this, "Error: " + Program.Kernel.Exception?.Message);
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
                FormConfigurationSelectionParam configurationSelectionParam = new FormConfigurationSelectionParam
                {
                    Modal = true,
                    TransientFor = this,
                    Resizable = false,
                    OpenConfigurationParam = ConfigurationParamCollection.GetConfigurationParam(selectedRows[0].Name),
                    CallBackUpdate = CallBackUpdate
                };

                configurationSelectionParam.Show();
            }
        }
    }
}