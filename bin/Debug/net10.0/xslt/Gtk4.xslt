<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="text" indent="yes" />

  <xsl:template name="License">
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
  </xsl:template>

  <xsl:template match="/">
/*
 *
 * Конфігурації "<xsl:value-of select="Configuration/Name"/>"
 * Автор <xsl:value-of select="Configuration/Author"/>
 * Дата конфігурації: <xsl:value-of select="Configuration/DateTimeSave"/>
 *
 *
 * Цей код згенерований в Конфігураторі 3. Шаблон Gtk4.xslt
 *
 */

using Gtk;
using GObject;
using InterfaceGtk4;
using AccountingSoftware;
using <xsl:value-of select="Configuration/NameSpaceGeneratedCode"/>.Перелічення;
using <xsl:value-of select="Configuration/NameSpace"/>;

namespace <xsl:value-of select="Configuration/NameSpaceGeneratedCode"/>.Rows
{
    [Subclass&lt;GObject.Object&gt;]
    public partial class Row
    {
        public UnigueID UID { get; set; } = new();
        public bool DeletionLabel { get; set; } = false;
    }

    <xsl:for-each select="Configuration/Directories/Directory">
        <xsl:variable name="DirectoryName" select="Name"/>
        <xsl:variable name="DirectoryIndex" select="position()"/>
    #region DIRECTORY "<xsl:value-of select="$DirectoryName"/>"
        <xsl:for-each select="TabularLists/TabularList">
    public partial class Row_<xsl:value-of select="$DirectoryIndex"/>_<xsl:value-of select="position()"/> : Row
    {
        <xsl:for-each select="Fields/Field">
        public string <xsl:value-of select="Name"/> { get; set; } = "";
        </xsl:for-each>
        <xsl:for-each select="Fields/AdditionalField[Visible = 'True']">
        public string <xsl:value-of select="Name"/> { get; set; } = "";
        </xsl:for-each>
    }
        </xsl:for-each>
    #endregion
    </xsl:for-each>
}

namespace <xsl:value-of select="Configuration/NameSpaceGeneratedCode"/>.Довідники.ТабличніСписки
{
    <xsl:for-each select="Configuration/Directories/Directory">
      <xsl:variable name="DirectoryName" select="Name"/>
      <xsl:variable name="DirectoryIndex" select="position()"/>
      <!-- Для ієрархії -->
      <xsl:variable name="DirectoryType" select="Type"/>
      <xsl:variable name="SelectType">
          <xsl:choose>
              <xsl:when test="$DirectoryType = 'Hierarchical'">SelectHierarchical</xsl:when>
              <xsl:otherwise>Select</xsl:otherwise>
          </xsl:choose>
      </xsl:variable>
    #region DIRECTORY "<xsl:value-of select="$DirectoryName"/>"
        <xsl:for-each select="TabularLists/TabularList">
            <xsl:variable name="TabularListName" select="Name"/>
            <xsl:variable name="RowClassName">
                <xsl:value-of select="/Configuration/NameSpaceGeneratedCode"/>.Rows.Row_<xsl:value-of select="$DirectoryIndex"/>_<xsl:value-of select="position()"/>
            </xsl:variable>
    public partial class <xsl:value-of select="$DirectoryName"/>_<xsl:value-of select="$TabularListName"/>
    {
        public static Gio.ListStore Create(ColumnView columnView)
        {
            Gio.ListStore store = Gio.ListStore.New(<xsl:value-of select="$RowClassName"/>.GetGType());

            SingleSelection model = SingleSelection.New(store);
            model.Autoselect = true;

            columnView.Model = model;

            //Image
            {
                SignalListItemFactory factory = SignalListItemFactory.New();
                factory.OnBind += (_, args) =&gt;
                {
                    ListItem listItem = (ListItem)args.Object;
                    <xsl:value-of select="$RowClassName"/>? row = (<xsl:value-of select="$RowClassName"/>?)listItem.Item;
                    listItem.SetChild(Picture.NewForPixbuf((row?.DeletionLabel ?? false) ? InterfaceGtk4.Іконки.ДляТабличногоСписку.Delete : InterfaceGtk4.Іконки.ДляТабличногоСписку.Normal));
                };

                ColumnViewColumn column = ColumnViewColumn.New("", factory);
                columnView.AppendColumn(column);
            }

            <xsl:for-each select="Fields/Field">
            {
                SignalListItemFactory factory = SignalListItemFactory.New();
                factory.OnSetup += (_, args) =&gt;
                {
                    ListItem listItem = (ListItem)args.Object;
                    Label label = Label.New(null);
                    label.Halign = Align.Start;
                    listItem.Child = label;
                };
                factory.OnBind += (_, args) =&gt;
                {
                    ListItem listItem = (ListItem)args.Object;
                    Label? label = (Label?)listItem.Child;
                    <xsl:value-of select="$RowClassName"/>? row = (<xsl:value-of select="$RowClassName"/>?)listItem.Item;
                    if (label != null &amp;&amp; row != null)
                        label.SetText(row.<xsl:value-of select="Name"/>);
                };

                ColumnViewColumn column = ColumnViewColumn.New("<xsl:value-of select="Caption"/>", factory);
                column.Resizable = true;
                columnView.AppendColumn(column);
            }
            </xsl:for-each>

            <xsl:for-each select="Fields/AdditionalField[Visible = 'True']">
            {
                SignalListItemFactory factory = SignalListItemFactory.New();
                factory.OnSetup += (_, args) =&gt;
                {
                    ListItem listItem = (ListItem)args.Object;
                    Label label = Label.New(null);
                    label.Halign = Align.Start;
                    listItem.Child = label;
                };
                factory.OnBind += (_, args) =&gt;
                {
                    ListItem listItem = (ListItem)args.Object;
                    Label? label = (Label?)listItem.Child;
                    <xsl:value-of select="$RowClassName"/>? row = (<xsl:value-of select="$RowClassName"/>?)listItem.Item;
                    if (label != null &amp;&amp; row != null)
                        label.SetText(row.<xsl:value-of select="Name"/>);
                };

                ColumnViewColumn column = ColumnViewColumn.New("<xsl:value-of select="Caption"/>", factory);
                column.Resizable = true;
                columnView.AppendColumn(column);
            }
            </xsl:for-each>

            return store;
        }

        public static async ValueTask LoadRecords(Gio.ListStore store)
        {
            <!-- Вибірка -->
            Довідники.<xsl:value-of select="$DirectoryName"/>_<xsl:value-of select="$SelectType"/><xsl:text> </xsl:text><xsl:value-of select="$DirectoryName"/>_Select = new();
            <xsl:value-of select="$DirectoryName"/>_Select.QuerySelect.Field.AddRange(
            [
                "deletion_label",
                <xsl:for-each select="Fields/Field[Type != 'pointer']">
                    <xsl:text>/*</xsl:text><xsl:value-of select="Name"/><xsl:text>*/ </xsl:text>
                    <xsl:text>Довідники.</xsl:text>
                    <xsl:value-of select="$DirectoryName"/>
                    <xsl:text>_Const.</xsl:text>
                    <xsl:value-of select="Name"/>,
                </xsl:for-each>
            ]);

            <!-- Добавлення Sql функції для полів тип яких date -->
            <xsl:for-each select="Fields/Field[Type = 'date']">
                <xsl:value-of select="$DirectoryName"/>_Select.QuerySelect.SqlFunc.Add(new SqlFunc(Довідники.<xsl:value-of select="$DirectoryName"/>_Const.<xsl:value-of select="Name"/>, "TO_CHAR", ["'dd.mm.yyyy'"]));
            </xsl:for-each>

            <!-- Відбори -->
            //var where = treeView.Data["Where"];
            //if (where != null) <xsl:value-of select="$DirectoryName"/>_Select.QuerySelect.Where = (List&lt;Where&gt;)where;

            <!-- Сортування -->
             <xsl:for-each select="Fields/Field[SortField = 'True']">
              <xsl:variable name="SortDirection">
                  <xsl:choose>
                      <xsl:when test="SortDirection = 'True'">SelectOrder.DESC</xsl:when>
                      <xsl:otherwise>SelectOrder.ASC</xsl:otherwise>
                  </xsl:choose>
              </xsl:variable>
              <xsl:value-of select="$DirectoryName"/>_Select.QuerySelect.Order.Add(
              <xsl:choose>
                  <xsl:when test="Type = 'pointer'">"<xsl:value-of select="Name"/>"</xsl:when>
                  <xsl:otherwise> Довідники.<xsl:value-of select="$DirectoryName"/>_Const.<xsl:value-of select="Name"/></xsl:otherwise>
              </xsl:choose>
              <xsl:text>, </xsl:text><xsl:value-of select="$SortDirection"/>);
            </xsl:for-each>

            /* Приєднання таблиць, JOIN */
            <xsl:for-each select="Fields/Field[Type = 'pointer']">
                <xsl:value-of select="substring-before(Pointer, '.')"/>.<xsl:value-of select="substring-after(Pointer, '.')"/>_Pointer.GetJoin(<xsl:value-of select="$DirectoryName"/>_Select.QuerySelect, Довідники.<xsl:value-of select="$DirectoryName"/>_Const.<xsl:value-of select="Name"/>,
                <xsl:value-of select="$DirectoryName"/>_Select.QuerySelect.Table, "join_tab_<xsl:value-of select="position()"/>", "<xsl:value-of select="Name"/>");
            </xsl:for-each>

            <xsl:if test="Fields/AdditionalField[Visible = 'True']">
            <!-- Додаткові поля -->
            <xsl:for-each select="Fields/AdditionalField[Visible = 'True']">
                /* Додаткове поле: <xsl:value-of select="Name"/> */
                <xsl:value-of select="$DirectoryName"/>_Select.QuerySelect.FieldAndAlias.Add(
                    new ValueName&lt;string&gt;(@$"(<xsl:value-of select="normalize-space(Value)"/>)", "<xsl:value-of select="Name"/>"));
            </xsl:for-each>
            </xsl:if>

            <!-- Вибрати дані -->
            await <xsl:value-of select="$DirectoryName"/>_Select.Select();
            store.RemoveAll();
            while (<xsl:value-of select="$DirectoryName"/>_Select.MoveNext())
            {
                Довідники.<xsl:value-of select="$DirectoryName"/>_Pointer? curr = <xsl:value-of select="$DirectoryName"/>_Select.Current;
                if (curr != null)
                {
                    Dictionary&lt;string, object&gt; fields = curr.Fields;
                    <xsl:value-of select="$RowClassName"/> row = new()
                    {
                        UID = curr.UnigueID,
                        DeletionLabel = (bool)fields["deletion_label"],
                        <xsl:for-each select="Fields/Field">
                          <xsl:value-of select="Name"/><xsl:text> = </xsl:text>
                          <xsl:choose>
                            <xsl:when test="Type = 'pointer'">
                              <xsl:text>fields["</xsl:text><xsl:value-of select="Name"/>"].ToString() ?? "",
                            </xsl:when>
                            <xsl:when test="Type = 'enum'">
                              <xsl:text>Перелічення.ПсевдонімиПерелічення.</xsl:text><xsl:value-of select="substring-after(Pointer, '.')"/>_Alias((
                              <xsl:text>(</xsl:text><xsl:value-of select="Pointer"/>)(fields[<xsl:value-of select="$DirectoryName"/>_Const.<xsl:value-of select="Name"/>] != DBNull.Value ? fields[<xsl:value-of select="$DirectoryName"/>_Const.<xsl:value-of select="Name"/>] : 0)) ),
                            </xsl:when>
                            <xsl:when test="Type = 'boolean'">
                              <xsl:text>(fields[</xsl:text><xsl:value-of select="$DirectoryName"/>_Const.<xsl:value-of select="Name"/>] != DBNull.Value ? (bool)fields[<xsl:value-of select="$DirectoryName"/>_Const.<xsl:value-of select="Name"/>] : false) ? "Так" : "",
                            </xsl:when>
                            <xsl:otherwise>
                              <xsl:text>fields[</xsl:text><xsl:value-of select="$DirectoryName"/>_Const.<xsl:value-of select="Name"/>].ToString() ?? "",
                            </xsl:otherwise>
                          </xsl:choose>
                        </xsl:for-each>
                        <xsl:for-each select="Fields/AdditionalField[Visible = 'True']">
                            <xsl:value-of select="Name"/> = fields["<xsl:value-of select="Name"/>"].ToString() ?? "",
                        </xsl:for-each>
                    };
                    store.Append(row);
                }
            }
        }
    }
        </xsl:for-each>
    #endregion
    </xsl:for-each>
}

  </xsl:template>
</xsl:stylesheet>