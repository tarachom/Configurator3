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
    class PageConstantBlock : VBox
    {
        Configuration? Conf
        {
            get
            {
                return Program.Kernel?.Conf;
            }
        }

        public ConfigurationConstantsBlock ConfConstantsBlock { get; set; } = new ConfigurationConstantsBlock();
        public FormConfigurator? GeneralForm { get; set; }
        public bool IsNew { get; set; } = true;

        #region Fields

        Entry entryName = new Entry() { WidthRequest = 500 };
        TextView textViewDesc = new TextView() { WrapMode = WrapMode.Word };

        #endregion

        public PageConstantBlock() : base()
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

            HPaned hPaned = new HPaned() { BorderWidth = 5, Position = 500 };

            CreatePack1(hPaned);
            CreatePack2(hPaned);

            PackStart(hPaned, false, false, 5);

            ShowAll();
        }

        void CreatePack1(HPaned hPaned)
        {
            VBox vBox = new VBox();

            //Назва
            HBox hBoxName = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxName, false, false, 5);

            hBoxName.PackStart(new Label("Назва:"), false, false, 5);
            hBoxName.PackStart(entryName, false, false, 5);

            //Опис
            HBox hBoxDesc = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxDesc, false, false, 5);

            hBoxDesc.PackStart(new Label("Опис:") { Valign = Align.Start }, false, false, 5);

            ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 500, HeightRequest = 100 };
            scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollTextView.Add(textViewDesc);

            hBoxDesc.PackStart(scrollTextView, false, false, 5);

            hPaned.Pack1(vBox, false, false);
        }

        void CreatePack2(HPaned hPaned)
        {
            VBox vBox = new VBox();

            HBox hBox = new HBox() { Halign = Align.Fill };
            vBox.PackStart(hBox, false, false, 5);

            Button bAddConst = new Button("Додати константу");
            bAddConst.Clicked += OnAddConstClick;

            hBox.PackStart(bAddConst, false, false, 10);

            hPaned.Pack2(vBox, false, false);
        }

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            entryName.Text = ConfConstantsBlock.BlockName;
            textViewDesc.Buffer.Text = ConfConstantsBlock.Desc;
        }

        void GetValue()
        {
            ConfConstantsBlock.BlockName = entryName.Text;
            ConfConstantsBlock.Desc = textViewDesc.Buffer.Text;
        }

        #endregion

        void OnSaveClick(object? sender, EventArgs args)
        {
            string name = entryName.Text;
            string errorList = Configuration.ValidateConfigurationObjectName(Program.Kernel!, ref name);
            entryName.Text = name;

            if (errorList.Length > 0)
            {
                Message.Error(GeneralForm, $"{errorList}");
                return;
            }

            if (IsNew)
            {
                if (Conf!.ConstantsBlock.ContainsKey(entryName.Text))
                {
                    Message.Error(GeneralForm, $"Назва блоку не унікальна");
                    return;
                }
            }
            else
            {
                if (ConfConstantsBlock.BlockName != entryName.Text)
                {
                    if (Conf!.ConstantsBlock.ContainsKey(entryName.Text))
                    {
                        Message.Error(GeneralForm, $"Назва блоку не унікальна");
                        return;
                    }
                }

                Conf!.ConstantsBlock.Remove(ConfConstantsBlock.BlockName);
            }

            GetValue();

            Conf!.ConstantsBlock.Add(ConfConstantsBlock.BlockName, ConfConstantsBlock);

            IsNew = false;

            GeneralForm?.LoadTreeAsync();
            GeneralForm?.RenameCurrentPageNotebook($"Блок констант: {ConfConstantsBlock.BlockName}");
        }

        void OnAddConstClick(object? sender, EventArgs args)
        {
            GeneralForm?.CreateNotebookPage("Константа *", () =>
            {
                ConfigurationConstants ConfConstants = new ConfigurationConstants();

                if (Conf!.ConstantsBlock.ContainsKey(ConfConstantsBlock.BlockName))
                    ConfConstants.Block = new ConfigurationConstantsBlock(ConfConstantsBlock.BlockName);
                else if (Conf!.ConstantsBlock.Count != 0)
                    ConfConstants.Block = Conf!.ConstantsBlock.Values.First<ConfigurationConstantsBlock>();

                PageConstant page = new PageConstant()
                {
                    ConfConstants = ConfConstants,
                    IsNew = true,
                    GeneralForm = GeneralForm
                };

                page.SetValue();

                return page;
            });
        }
    }
}