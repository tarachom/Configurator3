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
    class PageQuery : Box
    {
        Configuration Conf { get { return Program.Kernel.Conf; } }

        public ConfigurationQueryBlock QueryBlock { get; set; } = new ConfigurationQueryBlock();
        public string Key { get; set; } = "";
        public FormConfigurator? GeneralForm { get; set; }
        public System.Action? CallBack_RefreshList { get; set; }
        public bool IsNew { get; set; } = true;

        #region Fields

        Entry entryKey = new Entry() { WidthRequest = 800 };
        SourceView sourceViewCode = new SourceView() { ShowLineNumbers = true };

        #endregion

        public PageQuery() : base(Orientation.Vertical, 0)
        {
            Box hBox = new Box(Orientation.Horizontal, 0);

            Button bSave = new Button("Зберегти");
            bSave.Clicked += OnSaveClick;

            hBox.PackStart(bSave, false, false, 10);

            Button bClose = new Button("Закрити");
            bClose.Clicked += (object? sender, EventArgs args) => { GeneralForm?.CloseCurrentPageNotebook(); };

            hBox.PackStart(bClose, false, false, 10);

            PackStart(hBox, false, false, 10);

            Paned hPaned = new Paned(Orientation.Horizontal) { BorderWidth = 5, Position = 800 };

            CreatePack1(hPaned);
            CreatePack2(hPaned);

            PackStart(hPaned, false, false, 5);

            ShowAll();
        }

        void CreatePack1(Paned hPaned)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            //Ключ
            Box hBoxKey = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxKey, false, false, 5);

            hBoxKey.PackStart(new Label("Ключ:"), false, false, 5);
            hBoxKey.PackStart(entryKey, false, false, 5);

            //Query
            Box hBoxQuery = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxQuery, false, false, 5);

            hBoxQuery.PackStart(new Label("SQL:") { Valign = Align.Start }, false, false, 5);

            sourceViewCode.Buffer.Language = new LanguageManager().GetLanguage("sql");

            ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 1000, HeightRequest = 700 };
            scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollTextView.Add(sourceViewCode);

            hBoxQuery.PackStart(scrollTextView, false, false, 5);
            hPaned.Pack1(vBox, false, false);
        }

        void CreatePack2(Paned hPaned)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            Expander expanderHelp = new Expander("Довідка") { vBox };

            Box hBox = new Box(Orientation.Horizontal, 0) { Halign = Align.Fill };
            vBox.PackStart(hBox, false, false, 5);

            hBox.PackStart(new Label("help"), false, false, 5);

            hPaned.Pack2(expanderHelp, false, false);
        }

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            if (!IsNew)
                sourceViewCode.Buffer.Text = QueryBlock.Query[Key];

            entryKey.Text = Key;
        }

        void GetValue()
        {
            Key = entryKey.Text;
        }

        #endregion

        void OnSaveClick(object? sender, EventArgs args)
        {
            entryKey.Text = entryKey.Text.Trim();

            if (string.IsNullOrEmpty(entryKey.Text))
            {
                Message.Error(GeneralForm, $"Назва не задана");
                return;
            }

            if (IsNew)
            {
                if (QueryBlock.Query.ContainsKey(entryKey.Text))
                {
                    Message.Error(GeneralForm, $"Назва не унікальна");
                    return;
                }
            }
            else
            {
                if (Key != entryKey.Text)
                {
                    if (QueryBlock.Query.ContainsKey(entryKey.Text))
                    {
                        Message.Error(GeneralForm, $"Назва не унікальна");
                        return;
                    }
                }

                QueryBlock.Query.Remove(Key);
            }

            GetValue();

            QueryBlock.Query.Add(Key, sourceViewCode.Buffer.Text);

            IsNew = false;

            GeneralForm?.RenameCurrentPageNotebook($"Запит: {Key}");
            CallBack_RefreshList?.Invoke();
        }
    }
}