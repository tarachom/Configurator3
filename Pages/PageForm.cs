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
        Configuration Conf { get { return Program.Kernel.Conf; } }

        public Dictionary<string, ConfigurationForms> Forms { get; set; } = new Dictionary<string, ConfigurationForms>();
        public ConfigurationForms Form { get; set; } = new ConfigurationForms();
        public FormConfigurator? GeneralForm { get; set; }
        public System.Action? CallBack_RefreshList { get; set; }
        public bool IsNew { get; set; } = true;

        #region Fields

        Entry entryName = new Entry() { WidthRequest = 300 };
        TextView textViewDesc = new TextView() { WrapMode = WrapMode.Word };
        ComboBoxText comboBoxTypeForm = new ComboBoxText();
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

                ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 300, HeightRequest = 100 };
                scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
                scrollTextView.Add(textViewDesc);

                hBoxDesc.PackStart(scrollTextView, false, false, 5);
            }

            //Тип форми
            {
                HBox hBoxType = new HBox() { Halign = Align.End };
                vBox.PackStart(hBoxType, false, false, 5);

                comboBoxTypeForm.Append(ConfigurationForms.TypeForms.None.ToString(), "Неоприділений");
                comboBoxTypeForm.Append(ConfigurationForms.TypeForms.List.ToString(), "Список");
                comboBoxTypeForm.Append(ConfigurationForms.TypeForms.Element.ToString(), "Елемент");

                hBoxType.PackStart(new Label("Тип форми:"), false, false, 5);
                hBoxType.PackStart(comboBoxTypeForm, false, false, 5);
            }

            hPaned.Pack1(vBox, false, false);
        }

        void CreatePack2(HPaned hPaned)
        {
            VBox vBox = new VBox();

            HBox hBox = new HBox() { Halign = Align.Fill };
            vBox.PackStart(hBox, true, true, 5);

            sourceViewCode.Buffer.Language = new LanguageManager().GetLanguage("c-sharp");

            ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In, HeightRequest = 600 };
            scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollTextView.Add(sourceViewCode);

            hBox.PackStart(scrollTextView, true, true, 5);

            hPaned.Pack2(vBox, true, false);
        }

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            entryName.Text = Form.Name;
            textViewDesc.Buffer.Text = Form.Desc;
            comboBoxTypeForm.ActiveId = Form.Type.ToString();
        }

        void GetValue()
        {
            Form.Name = entryName.Text;
            Form.Desc = textViewDesc.Buffer.Text;
            Form.Type = Enum.Parse<ConfigurationForms.TypeForms>(comboBoxTypeForm.ActiveId);
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