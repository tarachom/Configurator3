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

/*

Сортування таблиці для табличного списку та форми

*/

using Gtk;

namespace Configurator
{
    class TreeViewFunc
    {
        /// <summary>
        /// Переміщує вибрану позицію в таблиці вверх або вниз
        /// </summary>
        /// <param name="treeView">TreeView</param>
        /// <param name="column">Колонка типу int для значення по якому відбувається сортування</param>
        /// <param name="income">Ввер(false) або вниз</param>
        public static void SortTreeView(TreeView treeView, int column, bool income)
        {
            if (treeView.Selection.CountSelectedRows() != 0)
            {
                TreePath treePath = treeView.Selection.GetSelectedRows()[0];
                treeView.Model.GetIter(out TreeIter iter, treePath);
                int sortNum = (int)treeView.Model.GetValue(iter, column);

                if (!income && treePath.Prev())
                {
                    treeView.Model.GetIter(out TreeIter iterPrev, treePath);
                    int sortPrev = (int)treeView.Model.GetValue(iterPrev, column);

                    treeView.Model.SetValue(iter, column, sortPrev == sortNum ? --sortPrev : sortPrev);
                    treeView.Model.SetValue(iterPrev, column, sortNum);
                }
                else if (income)
                {
                    treePath.Next();

                    if (treeView.Model.GetIter(out TreeIter iterNext, treePath))
                    {
                        int sortNext = (int)treeView.Model.GetValue(iterNext, column);

                        treeView.Model.SetValue(iter, column, sortNext == sortNum ? ++sortNext : sortNext);
                        treeView.Model.SetValue(iterNext, column, sortNum);
                    }
                }
            }
        }
    }
}