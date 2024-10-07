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
    class PageUser : Box
    {
        public FormConfigurator? GeneralForm { get; set; }
        public System.Action? CallBack_RefreshList { get; set; }
        public bool IsNew { get; set; } = true;
        public Guid UID { get; set; } = Guid.Empty;

        #region Fields

        Entry entryLogin = new Entry() { WidthRequest = 500 };
        Entry entryName = new Entry() { WidthRequest = 500 };
        Entry entryPassword = new Entry() { WidthRequest = 500 };
        TextView textViewInfo = new TextView() { WrapMode = WrapMode.Word };

        #endregion

        public PageUser() : base(Orientation.Vertical, 0)
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

            //Логін
            Box hBoxLogin = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxLogin, false, false, 5);

            hBoxLogin.PackStart(new Label("Логін:"), false, false, 5);
            hBoxLogin.PackStart(entryLogin, false, false, 5);

            //Назва
            Box hBoxName = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxName, false, false, 5);

            hBoxName.PackStart(new Label("Назва:"), false, false, 5);
            hBoxName.PackStart(entryName, false, false, 5);

            //Пароль
            Box hBoxPassword = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxPassword, false, false, 5);

            hBoxPassword.PackStart(new Label("Пароль:"), false, false, 5);
            hBoxPassword.PackStart(entryPassword, false, false, 5);

            //Опис
            Box hBoxDesc = new Box(Orientation.Horizontal, 0) { Halign = Align.End };
            vBox.PackStart(hBoxDesc, false, false, 5);

            hBoxDesc.PackStart(new Label("Опис:") { Valign = Align.Start }, false, false, 5);

            ScrolledWindow scrollTextView = new ScrolledWindow() { ShadowType = ShadowType.In, WidthRequest = 500, HeightRequest = 100 };
            scrollTextView.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrollTextView.Add(textViewInfo);

            hBoxDesc.PackStart(scrollTextView, false, false, 5);

            hPaned.Pack1(vBox, false, false);
        }

        void CreatePack2(Paned hPaned)
        {
            Box vBox = new Box(Orientation.Vertical, 0);

            hPaned.Pack2(vBox, false, false);
        }

        #region Присвоєння / зчитування значень віджетів

        public async void SetValue()
        {
            if (!IsNew)
            {
                SelectRequest_Record? recordResult = await Program.Kernel.DataBase.SpetialTableUsersExtendetUser(UID);

                if (recordResult != null)
                {
                    Dictionary<string, object> line = recordResult.ListRow[0];

                    entryLogin.Text = line["name"].ToString();
                    entryName.Text = line["fullname"].ToString();
                    textViewInfo.Buffer.Text = line["info"].ToString();
                }
            }
        }

        //Login, Name, Pass, Info
        (string Login, string Name, string Pass, string Info) GetValue()
        {
            return
            (
                entryLogin.Text,
                entryName.Text,
                entryPassword.Text,
                textViewInfo.Buffer.Text
            );
        }

        #endregion

        async void OnSaveClick(object? sender, EventArgs args)
        {
            var value = GetValue();

            string name = value.Login;
            string errorList = Configuration.ValidateConfigurationObjectName(ref name);

            if (errorList.Length > 0)
            {
                Message.Error(GeneralForm, $"{errorList}");
                return;
            }

            if (IsNew)
            {
                if (await Program.Kernel.DataBase.SpetialTableUsersIsExistUser(value.Login))
                {
                    Message.Error(GeneralForm, "Назва користувача не унікальна");
                    return;
                }

                Guid? UserUID = await Program.Kernel.DataBase.SpetialTableUsersAddOrUpdate(IsNew, null, value.Login, value.Name, value.Pass, value.Info);

                if (UserUID.HasValue)
                {
                    UID = UserUID.Value;
                    IsNew = false;
                }
                else
                {
                    Message.Error(GeneralForm, "Невдалось створити користувача");
                    return;
                }
            }
            else
            {
                if (await Program.Kernel.DataBase.SpetialTableUsersIsExistUser(value.Login, null, UID))
                {
                    Message.Error(GeneralForm, "Назва користувача не унікальна");
                    return;
                }

                await Program.Kernel.DataBase.SpetialTableUsersAddOrUpdate(IsNew, UID, value.Login, value.Name, value.Pass, value.Info);
            }

            CallBack_RefreshList?.Invoke();
            GeneralForm?.RenameCurrentPageNotebook($"Користувач: {value.Name}");
        }
    }
}