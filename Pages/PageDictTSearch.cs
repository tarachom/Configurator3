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
    class PageDictTSearch : VBox
    {
        Configuration Conf { get { return Program.Kernel.Conf; } }

        public FormConfigurator? GeneralForm { get; set; }

        #region Fields

        ComboBoxText comboBoxTsConfig = new ComboBoxText();

        #endregion

        public PageDictTSearch() : base()
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

            HPaned hPaned = new HPaned() { BorderWidth = 5, Position = 200 };

            CreatePack1(hPaned);
            CreatePack2(hPaned);

            PackStart(hPaned, false, false, 5);

            ShowAll();
        }

        void CreatePack1(HPaned hPaned)
        {
            VBox vBox = new VBox();

            //Види словників
            HBox hBoxTsConfig = new HBox() { Halign = Align.End };
            vBox.PackStart(hBoxTsConfig, false, false, 5);

            hBoxTsConfig.PackStart(new Label("Вид словника:"), false, false, 5);
            hBoxTsConfig.PackStart(comboBoxTsConfig, false, false, 5);

            hPaned.Pack1(vBox, false, false);
        }

        void CreatePack2(HPaned hPaned)
        {
            VBox vBox = new VBox();

            //Довідка
            {
                HBox hBoxHelp = new HBox() { Halign = Align.Start };
                vBox.PackStart(hBoxHelp, false, false, 5);

                //Довідка текст
                Label labelHelp = new Label(
@"<b>ДОДАВАННЯ УКРАЇНСЬКОГО СЛОВНИКА</b>
1. <b>Відкрити репозиторій:</b> https://github.com/brown-uk/dict_uk/releases

2. <b>Завантажити архів:</b> hunspell-uk_UA_***.zip

3. <b>Завантажити файл ukrainian.stop:</b> https://github.com/brown-uk/dict_uk/blob/master/distr/postgresql/ukrainian.stop

4. <b>Скопіювати файли в директорію PostgreSql:</b>
sudo cp uk_UA.aff $(pg_config --sharedir)/tsearch_data/uk_ua.affix
sudo cp uk_UA.dic $(pg_config --sharedir)/tsearch_data/uk_ua.dict
sudo cp ukrainian.stop $(pg_config --sharedir)/tsearch_data/ukrainian.stop            
")
                {
                    Wrap = true,
                    UseMarkup = true,
                    Selectable = true
                };

                hBoxHelp.PackStart(labelHelp, false, false, 10);
            }

            //SQL
            {
                HBox hBoxSql = new HBox() { Halign = Align.Start };
                vBox.PackStart(hBoxSql, false, false, 5);

                //SQL текст
                Label labelSQL = new Label(
@"<b>SQL</b>
<b>Створення словника</b>
CREATE TEXT SEARCH DICTIONARY ukrainian_huns (TEMPLATE = ispell, DictFile = uk_UA, AffFile = uk_UA, StopWords = ukrainian);

<b>Створення словника стоп слів</b>
CREATE TEXT SEARCH DICTIONARY ukrainian_stem (template = simple, stopwords = ukrainian);

<b>Створення конфігурації</b>
CREATE TEXT SEARCH CONFIGURATION ukrainian (PARSER=default);

<b>Налаштування конфігурації</b>
ALTER TEXT SEARCH CONFIGURATION ukrainian ALTER MAPPING FOR  hword, hword_part, word WITH ukrainian_huns, ukrainian_stem;
ALTER TEXT SEARCH CONFIGURATION ukrainian ALTER MAPPING FOR  int, uint, numhword, numword, hword_numpart, email, float, file, url, url_path, version, host, sfloat WITH simple;
ALTER TEXT SEARCH CONFIGURATION ukrainian ALTER MAPPING FOR asciihword, asciiword, hword_asciipart WITH english_stem;
")
                {
                    Wrap = true,
                    UseMarkup = true,
                    Selectable = true
                };

                hBoxSql.PackStart(labelSQL, false, false, 10);
            }

            //Кнопки
            {
                HBox hBoxButton = new HBox() { Halign = Align.Start };
                vBox.PackStart(hBoxButton, false, false, 5);

                Button button = new Button() { Label = "Виконати SQL" };

                button.Clicked += async (object? sender, EventArgs args) =>
                {
                    button.Sensitive = false;

                    string[] sql_list =
                    [
                        "DROP TEXT SEARCH DICTIONARY IF EXISTS ukrainian_huns cascade",
                        "DROP TEXT SEARCH DICTIONARY IF EXISTS ukrainian_stem cascade",
                        "DROP TEXT SEARCH CONFIGURATION IF EXISTS ukrainian cascade",

                        "CREATE TEXT SEARCH DICTIONARY ukrainian_huns (TEMPLATE = ispell, DictFile = uk_UA, AffFile = uk_UA, StopWords = ukrainian)",
                        "CREATE TEXT SEARCH DICTIONARY ukrainian_stem (template = simple, stopwords = ukrainian)",
                        "CREATE TEXT SEARCH CONFIGURATION ukrainian (PARSER=default)",

                        "ALTER TEXT SEARCH CONFIGURATION ukrainian ALTER MAPPING FOR  hword, hword_part, word WITH ukrainian_huns, ukrainian_stem",
                        "ALTER TEXT SEARCH CONFIGURATION ukrainian ALTER MAPPING FOR  int, uint, numhword, numword, hword_numpart, email, float, file, url, url_path, version, host, sfloat WITH simple",
                        "ALTER TEXT SEARCH CONFIGURATION ukrainian ALTER MAPPING FOR asciihword, asciiword, hword_asciipart WITH english_stem"
                    ];

                    foreach (string sql in sql_list)
                    {
                        try
                        {
                            await Program.Kernel.DataBase.ExecuteSQL(sql);
                        }
                        catch (Exception ex)
                        {
                            Message.Error(GeneralForm, ex.Message);
                        }
                    }

                    Conf.DictTSearch = "ukrainian";
                    SetValue();

                    button.Sensitive = true;
                };

                hBoxButton.PackStart(button, false, false, 10);
            }

            hPaned.Pack2(vBox, false, false);
        }

        #region Присвоєння / зчитування значень віджетів

        public async void SetValue()
        {
            var recordResult = await Program.Kernel.DataBase.SpetialTableFullTextSearchDictList();

            comboBoxTsConfig.RemoveAll();
            foreach (var fieldType in recordResult.ListRow)
            {
                string value = fieldType["cfgname"]?.ToString() ?? "";
                comboBoxTsConfig.Append(value, value);
            }

            comboBoxTsConfig.ActiveId = Conf.DictTSearch;

            if (comboBoxTsConfig.Active == -1)
                comboBoxTsConfig.ActiveId = Configuration.DefaultDictTSearch;
        }

        void GetValue()
        {
            Conf.DictTSearch = comboBoxTsConfig.ActiveId;
        }

        #endregion

        void OnSaveClick(object? sender, EventArgs args)
        {
            GetValue();
        }
    }
}