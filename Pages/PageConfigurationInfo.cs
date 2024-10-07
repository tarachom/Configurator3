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
    class PageConfigurationInfo : Box
    {
        Configuration Conf { get { return Program.Kernel.Conf; } }

        public FormConfigurator? GeneralForm { get; set; }

        #region Fields

        Entry entryName = new Entry() { WidthRequest = 500 };
        Entry entrySubtitle = new Entry() { WidthRequest = 500 };
        Entry entryNameSpaceGenerationCode = new Entry() { WidthRequest = 500 };
        Entry entryNameSpace = new Entry() { WidthRequest = 500 };
        Entry entryAutor = new Entry() { WidthRequest = 500 };
        TextView textViewDesc = new TextView() { WrapMode = WrapMode.Word };

        #endregion

        public PageConfigurationInfo() : base(Orientation.Vertical, 0)
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

            //Підзаголовок
            Box hBoxSubtitle = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxSubtitle, false, false, 5);

            hBoxSubtitle.PackStart(new Label("Підзаголовок:"), false, false, 5);
            hBoxSubtitle.PackStart(entrySubtitle, false, false, 5);

            //Простір імен
            Box hBoxNameSpaceGenerationCode = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxNameSpaceGenerationCode, false, false, 5);

            hBoxNameSpaceGenerationCode.PackStart(new Label("Простір імен згенерованого коду:"), false, false, 5);
            hBoxNameSpaceGenerationCode.PackStart(entryNameSpaceGenerationCode, false, false, 5);

            //Простір імен
            Box hBoxNameSpace = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxNameSpace, false, false, 5);

            hBoxNameSpace.PackStart(new Label("Простір імен програми:"), false, false, 5);
            hBoxNameSpace.PackStart(entryNameSpace, false, false, 5);

            //Автор
            Box hBoxAutor = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxAutor, false, false, 5);

            hBoxAutor.PackStart(new Label("Автор:"), false, false, 5);
            hBoxAutor.PackStart(entryAutor, false, false, 5);

            //Опис
            Box hBoxDesc = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxDesc, false, false, 5);

            hBoxDesc.PackStart(new Label("Опис:") { Valign = Align.Start }, false, false, 5);

            ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 500, HeightRequest = 500 };
            scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollTextView.Add(textViewDesc);

            hBoxDesc.PackStart(scrollTextView, false, false, 5);

            hPaned.Pack1(vBox, false, false);
        }

        void CreatePack2(Paned hPaned)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            Box hBox = new Box(Orientation.Horizontal, 0) { Halign = Align.Fill };
            vBox.PackStart(hBox, false, false, 5);

            hBox.PackStart(new Label("Загальна інформація про конфігурацію"), false, false, 5);

            hPaned.Pack2(vBox, false, false);
        }

        #region Присвоєння / зчитування значень віджетів

        public void SetValue()
        {
            entryName.Text = Conf.Name;
            entrySubtitle.Text = Conf.Subtitle;
            entryNameSpaceGenerationCode.Text = Conf.NameSpaceGenerationCode;
            entryNameSpace.Text = Conf.NameSpace;
            entryAutor.Text = Conf.Author;
            textViewDesc.Buffer.Text = Conf.Desc;
        }

        void GetValue()
        {
            Conf.Name = entryName.Text;
            Conf.Subtitle = entrySubtitle.Text;
            Conf.NameSpaceGenerationCode = entryNameSpaceGenerationCode.Text;
            Conf.NameSpace = entryNameSpace.Text;
            Conf.Author = entryAutor.Text;
            Conf.Desc = textViewDesc.Buffer.Text;
        }

        #endregion

        void OnSaveClick(object? sender, EventArgs args)
        {
            string name = entryNameSpace.Text;
            string errorList = Configuration.ValidateConfigurationObjectName(ref name);
            entryNameSpace.Text = name;

            if (errorList.Length > 0)
            {
                Message.Error(GeneralForm, $"{errorList}");
                return;
            }

            GetValue();
        }
    }
}