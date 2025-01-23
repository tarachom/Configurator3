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
    class PageJournalField : Box
    {
        Configuration Conf { get { return Program.Kernel.Conf; } }

        public Dictionary<string, ConfigurationJournalField> Fields = new Dictionary<string, ConfigurationJournalField>();
        public ConfigurationJournalField Field { get; set; } = new ConfigurationJournalField();
        public FormConfigurator? GeneralForm { get; set; }
        public System.Action? CallBack_RefreshList { get; set; }
        public bool IsNew { get; set; } = true;

        #region Fields

        Entry entryName = new Entry() { WidthRequest = 500 };
        TextView textViewDesc = new TextView() { WrapMode = WrapMode.Word };
        Entry entryType = new Entry();
        CheckButton checkButtonSort = new CheckButton("Сортувати");
        CheckButton checkButtonWherePeriod = new CheckButton("Відбір по періоду");

        #endregion

        public PageJournalField() : base(Orientation.Vertical, 0)
        {
            Box hBox = new Box(Orientation.Horizontal, 0);

            Button bSave = new Button("Зберегти");
            bSave.Clicked += OnSaveClick;

            hBox.PackStart(bSave, false, false, 10);

            Button bClose = new Button("Закрити");
            bClose.Clicked += (object? sender, EventArgs args) => GeneralForm?.CloseCurrentPageNotebook();

            hBox.PackStart(bClose, false, false, 10);

            PackStart(hBox, false, false, 10);

            Paned hPaned = new Paned(Orientation.Horizontal) { BorderWidth = 5, Position = 500 };

            CreatePack1(hPaned);
            CreatePack2(hPaned);

            PackStart(hPaned, false, false, 5);

            ShowAll();
        }

        void CreatePack1(Paned hPaned)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            //Назва
            Box hBoxName = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxName, false, false, 5);

            hBoxName.PackStart(new Label("Назва:"), false, false, 5);
            hBoxName.PackStart(entryName, false, false, 5);

            //Опис
            Box hBoxDesc = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxDesc, false, false, 5);

            hBoxDesc.PackStart(new Label("Опис:") { Valign = Align.Start }, false, false, 5);

            ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 500, HeightRequest = 100 };
            scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollTextView.Add(textViewDesc);

            hBoxDesc.PackStart(scrollTextView, false, false, 5);

            //Тип
            Box hBoxType = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxType, false, false, 5);

            hBoxType.PackStart(new Label("SQL Тип:"), false, false, 5);
            hBoxType.PackStart(entryType, false, false, 5);

            //Сортувати
            Box hBoxOrder = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxOrder, false, false, 5);

            hBoxOrder.PackStart(checkButtonSort, false, false, 5);
            hBoxOrder.PackStart(checkButtonWherePeriod, false, false, 5);

            hPaned.Pack1(vBox, false, false);
        }

        void CreatePack2(Paned hPaned)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            Expander expanderHelp = new Expander("Довідка") { vBox };

            Box hBox = new Box(Orientation.Horizontal, 0) { Halign = Align.Fill };
            vBox.PackStart(hBox, false, false, 5);

            hBox.PackStart(new Label("Поле журналу"), false, false, 5);

            hPaned.Pack2(expanderHelp, false, false);
        }

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            entryName.Text = Field.Name;
            textViewDesc.Buffer.Text = Field.Desc;
            entryType.Text = Field.Type;
            checkButtonSort.Active = Field.SortField;
            checkButtonWherePeriod.Active = Field.WherePeriod;
        }

        void GetValue()
        {
            Field.Name = entryName.Text;
            Field.Desc = textViewDesc.Buffer.Text;
            Field.Type = entryType.Text;
            Field.SortField = checkButtonSort.Active;
            Field.WherePeriod = checkButtonWherePeriod.Active;
        }

        #endregion

        void OnSaveClick(object? sender, EventArgs args)
        {
            string name = entryName.Text;
            string errorList = Configuration.ValidateConfigurationObjectName(ref name);
            entryName.Text = name;

            if (errorList.Length > 0)
            {
                Message.Error(GeneralForm, $"{errorList}");
                return;
            }

            if (IsNew)
            {
                if (Fields.ContainsKey(entryName.Text))
                {
                    Message.Error(GeneralForm, $"Назва поля не унікальна");
                    return;
                }
            }
            else
            {
                if (Field.Name != entryName.Text)
                {
                    if (Fields.ContainsKey(entryName.Text))
                    {
                        Message.Error(GeneralForm, $"Назва поля не унікальна");
                        return;
                    }
                }

                Fields.Remove(Field.Name);
            }

            GetValue();

            Fields.Add(Field.Name, Field);

            IsNew = false;

            GeneralForm?.LoadTreeAsync();
            GeneralForm?.RenameCurrentPageNotebook($"Поле: {Field.Name}");

            CallBack_RefreshList?.Invoke();
        }
    }
}