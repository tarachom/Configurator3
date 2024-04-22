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
using GtkSource;

namespace Configurator
{
    class PageForm : VBox
    {
        //Configuration Conf { get { return Program.Kernel.Conf; } }

        public Dictionary<string, ConfigurationField> Fields = [];
        public Dictionary<string, ConfigurationForms> Forms { get; set; } = new Dictionary<string, ConfigurationForms>();
        public ConfigurationForms Form { get; set; } = new ConfigurationForms();
        public FormConfigurator? GeneralForm { get; set; }
        public System.Action? CallBack_RefreshList { get; set; }
        public bool IsNew { get; set; } = true;
        public ConfigurationForms.TypeForms TypeForm { get; set; } = ConfigurationForms.TypeForms.None;

        #region Fields

        Entry entryName = new Entry() { WidthRequest = 250 };
        TextView textViewDesc = new TextView() { WrapMode = WrapMode.Word };
        Notebook notebook = new Notebook()
        {
            Scrollable = true,
            EnablePopup = true,
            BorderWidth = 0,
            ShowBorder = false,
            TabPos = PositionType.Top,
            HeightRequest = 600
        };
        SourceView sourceViewCode = new SourceView() { ShowLineNumbers = true };

        #endregion

        public PageForm() : base()
        {
            new VBox();
            HBox hBox = new HBox();

            Button bSave = new Button("Зберегти");
            bSave.Clicked += OnSaveClick;

            hBox.PackStart(bSave, false, false, 10);

            Button bClose = new Button("Закрити");
            bClose.Clicked += (object? sender, EventArgs args) => { GeneralForm?.CloseCurrentPageNotebook(); };

            hBox.PackStart(bClose, false, false, 10);

            PackStart(hBox, false, false, 10);

            HPaned hPaned = new HPaned() { BorderWidth = 5 };

            CreatePack1(hPaned);
            CreatePack2(hPaned);

            PackStart(hPaned, false, false, 5);

            ShowAll();
        }

        void CreatePack1(HPaned hPaned)
        {
            VBox vBox = new VBox();

            //Базові поля
            {
                //Назва
                HBox hBoxName = new HBox() { Halign = Align.End };
                vBox.PackStart(hBoxName, false, false, 5);

                hBoxName.PackStart(new Label("Назва:"), false, false, 5);
                hBoxName.PackStart(entryName, false, false, 5);

                //Опис
                HBox hBoxDesc = new HBox() { Halign = Align.End };
                vBox.PackStart(hBoxDesc, false, false, 5);

                hBoxDesc.PackStart(new Label("Опис:") { Valign = Align.Start }, false, false, 5);

                ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 250, HeightRequest = 100 };
                scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
                scrollTextView.Add(textViewDesc);

                hBoxDesc.PackStart(scrollTextView, false, false, 5);
            }

            hPaned.Pack1(vBox, false, false);
        }

        void CreatePack2(HPaned hPaned)
        {
            VBox vBox = new VBox();

            HBox hBox = new HBox() { Halign = Align.Fill };
            hBox.PackStart(notebook, true, true, 5);
            vBox.PackStart(hBox, true, true, 0);

            hPaned.Pack2(vBox, true, false);
        }

        public void CreateNotePage(string tabName, Widget pageWidget)
        {
            ScrolledWindow scroll = new ScrolledWindow() { ShadowType = ShadowType.In };
            scroll.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scroll.Add(pageWidget);

            notebook.AppendPage(scroll, new Label(tabName));
            notebook.ShowAll();
        }

        public void CreateElementForm()
        {
            VBox vBox = new VBox();

            foreach (ConfigurationField field in Fields.Values)
            {
                HBox hBox = new HBox() { Halign = Align.Start };
                hBox.PackStart(new Label(field.Name), false, false, 5);
                vBox.PackStart(hBox, false, false, 5);
            }

            CreateNotePage("Форма", vBox);
        }

        public void CreateListForm()
        {
            VBox vBox = new VBox();
            HBox hBox = new HBox() { Halign = Align.Start };

            vBox.PackStart(hBox, true, true, 0);

            CreateNotePage("Форма", vBox);
        }

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            if (IsNew)
                Form.Type = TypeForm;

            entryName.Text = Form.Name;
            textViewDesc.Buffer.Text = Form.Desc;

            switch (TypeForm)
            {
                case ConfigurationForms.TypeForms.Element:
                    {
                        CreateElementForm();
                        break;
                    }
                case ConfigurationForms.TypeForms.List:
                    {
                        CreateListForm();
                        break;
                    }
            }

            sourceViewCode.Buffer.Language = new LanguageManager().GetLanguage("c-sharp");
            CreateNotePage("Код", sourceViewCode);
        }

        void GetValue()
        {
            Form.Name = entryName.Text;
            Form.Desc = textViewDesc.Buffer.Text;
        }

        #endregion

        void OnSaveClick(object? sender, EventArgs args)
        {
            string name = entryName.Text;
            string errorList = Configuration.ValidateConfigurationObjectName(Program.Kernel, ref name);
            entryName.Text = name;

            if (errorList.Length > 0)
            {
                Message.Error(GeneralForm, $"{errorList}");
                return;
            }

            if (IsNew)
            {
                if (Forms.ContainsKey(entryName.Text))
                {
                    Message.Error(GeneralForm, $"Назва форми не унікальна");
                    return;
                }
            }
            else
            {
                if (Form.Name != entryName.Text)
                {
                    if (Forms.ContainsKey(entryName.Text))
                    {
                        Message.Error(GeneralForm, $"Назва форми не унікальна");
                        return;
                    }
                }

                Forms.Remove(Form.Name);
            }

            GetValue();

            Forms.Add(Form.Name, Form);

            IsNew = false;

            GeneralForm?.RenameCurrentPageNotebook($"Форма: {Form.Name}");

            if (CallBack_RefreshList != null)
                CallBack_RefreshList.Invoke();
        }
    }
}