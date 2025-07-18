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
using InterfaceGtk3;

namespace Configurator
{
    class PageHome : Box
    {
        public БлокДляСторінки_АктивніКористувачі АктивніКористувачі = new БлокДляСторінки_АктивніКористувачі(Program.Kernel) { WidthRequest = 600, HeightRequest = 600 };

        public PageHome() : base(Orientation.Vertical, 0)
        {
            {
                Box hBox = new Box(Orientation.Horizontal, 0);
                hBox.PackStart(АктивніКористувачі, false, false, 5);
                PackStart(hBox, false, false, 5);
            }

            ShowAll();
        }
    }
}