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

  <!-- Для задання значення поля -->
  <xsl:template name="FieldValue">
    <xsl:param name="ConfTypeName" />
    <xsl:choose>
        <xsl:when test="Type = 'pointer'">
            <xsl:text>Fields["</xsl:text><xsl:value-of select="Name"/><xsl:text>"].ToString() ?? ""</xsl:text>
        </xsl:when>
        <xsl:when test="Type = 'enum'">
            <xsl:text>Перелічення.ПсевдонімиПерелічення.</xsl:text><xsl:value-of select="substring-after(Pointer, '.')"/><xsl:text>_Alias((</xsl:text>
            <xsl:value-of select="Pointer"/><xsl:text>)(Fields[</xsl:text><xsl:value-of select="$ConfTypeName"/><xsl:text>_Const.</xsl:text><xsl:value-of select="Name"/>
            <xsl:text>] != DBNull.Value ? Fields[</xsl:text><xsl:value-of select="$ConfTypeName"/><xsl:text>_Const.</xsl:text><xsl:value-of select="Name"/><xsl:text>] : 0) )</xsl:text>
        </xsl:when>
        <xsl:when test="Type = 'boolean'">
            <xsl:text>(Fields[</xsl:text><xsl:value-of select="$ConfTypeName"/><xsl:text>_Const.</xsl:text><xsl:value-of select="Name"/><xsl:text>] != DBNull.Value &amp;&amp; (bool)Fields[</xsl:text><xsl:value-of select="$ConfTypeName"/><xsl:text>_Const.</xsl:text><xsl:value-of select="Name"/><xsl:text>]) ? "Так" : ""</xsl:text>
        </xsl:when>
        <xsl:otherwise>
            <xsl:text>Fields[</xsl:text><xsl:value-of select="$ConfTypeName"/><xsl:text>_Const.</xsl:text><xsl:value-of select="Name"/>]<xsl:text>.ToString() ?? ""</xsl:text>
        </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <!-- Для формування фільтрів -->
  <xsl:template name="CreateFilter">
    <xsl:param name="ConfTypeName" />
        <xsl:if test="count(Fields/Field[FilterField = 'True']) != 0">
            List&lt;FilterControl.FilterListItem&gt; filterList = [];
            <xsl:for-each select="Fields/Field[FilterField = 'True']">
            { /* <xsl:value-of select="Name"/>, <xsl:value-of select="Type"/> */
                Switch sw = Switch.New();
                <xsl:choose>
                    <xsl:when test="Type = 'string'">
                        Entry <xsl:value-of select="Name"/> = Entry.New();
                        <xsl:value-of select="Name"/>.WidthRequest = 300;
                        object get() =&gt; <xsl:value-of select="Name"/>.GetText();
                    </xsl:when>
                    <xsl:when test="Type = 'boolean'">
                        CheckButton <xsl:value-of select="Name"/> = CheckButton.New();
                        <xsl:value-of select="Name"/>.OnActivate += (_, _) =&gt; sw.Active = <xsl:value-of select="Name"/>.Active;
                        object get() =&gt; <xsl:value-of select="Name"/>.Active;
                    </xsl:when>
                    <xsl:when test="Type = 'integer'">
                        IntegerControl <xsl:value-of select="Name"/> = new();
                        object get() =&gt; <xsl:value-of select="Name"/>.Value;
                    </xsl:when>
                    <xsl:when test="Type = 'numeric'">
                        NumericControl <xsl:value-of select="Name"/> = new();
                        object get() =&gt; <xsl:value-of select="Name"/>.Value;
                    </xsl:when>
                    <xsl:when test="Type = 'date' or Type = 'datetime'">
                        DateTimeControl <xsl:value-of select="Name"/> = new() { <xsl:if test="Type = 'date'">OnlyDate = true</xsl:if> };
                        object get() =&gt; <xsl:value-of select="Name"/>.Value;
                    </xsl:when>
                    <xsl:when test="Type = 'time'">
                        TimeControl <xsl:value-of select="Name"/> = new();
                        object get() =&gt; <xsl:value-of select="Name"/>.Value;
                    </xsl:when>
                    <xsl:when test="Type = 'pointer'">
                        <xsl:value-of select="substring-after(Pointer, '.')"/>_PointerControl <xsl:value-of select="Name"/> = new() { Caption = "", AfterSelectFunc = () =&gt; sw.Active = true };
                        object get() =&gt; <xsl:value-of select="Name"/>.Pointer.UnigueID.UGuid;
                    </xsl:when>
                    <xsl:when test="Type = 'enum'">
                        ComboBoxText <xsl:value-of select="Name"/> = new();
                        foreach (var item in ПсевдонімиПерелічення.<xsl:value-of select="substring-after(Pointer, '.')"/>_List())
                            <xsl:value-of select="Name"/>.Append(item.Value.ToString(), item.Name);
                        <xsl:value-of select="Name"/>.Active = 0;
                        object get() =&gt; Enum.TryParse(<xsl:value-of select="Name"/>.ActiveId, out <xsl:value-of select="substring-after(Pointer, '.')"/> value) ? (int)value: 0;
                    </xsl:when>
                    <xsl:otherwise>
                        Label <xsl:value-of select="Name"/> = Label.New("<xsl:value-of select="Type"/>");
                        object get() =&gt; new();
                    </xsl:otherwise>
                </xsl:choose>
                filterList.Add(new(<xsl:value-of select="$ConfTypeName"/>_Const.<xsl:value-of select="Name"/>, get, sw));
                form.Filter.Append("<xsl:value-of select="normalize-space(Caption)"/>:", <xsl:value-of select="Name"/>, sw);
            }
            </xsl:for-each>
            form.Filter.GetWhere = () =&gt;
            {
                List&lt;Where&gt; where = [];
                foreach (var filter in filterList)
                    if (filter.IsOn.Active)
                        where.Add(new Where(filter.Field, Comparison.EQ, filter.GetValueFunc.Invoke()));

                form.WhereList = where;
                return where.Count != 0;
            };
        </xsl:if>
  </xsl:template>

  <xsl:template name="AddColumnImage">
    <xsl:param name="RowType" />
            //Image
            {
                SignalListItemFactory factory = SignalListItemFactory.New();
                factory.OnBind += (_, args) =&gt;
                {
                    ListItem listItem = (ListItem)args.Object;
                    <xsl:value-of select="$RowType"/>? row = (<xsl:value-of select="$RowType"/>?)listItem.Item;
                    listItem.SetChild(ImageTablePartCell.NewForPixbuf((row?.DeletionLabel ?? false) ? InterfaceGtk4.Icon.ForTabularLists.Delete : InterfaceGtk4.Icon.ForTabularLists.Normal));
                };
                ColumnViewColumn column = ColumnViewColumn.New("", factory);
                form.Grid.AppendColumn(column);
            }
  </xsl:template>

  <xsl:template name="AddColumnSpend">
            //Spend
            {
                SignalListItemFactory factory = SignalListItemFactory.New();
                factory.OnBind += (_, args) =&gt;
                {
                    ListItem listItem = (ListItem)args.Object;
                    DocumentRowJournal? row = (DocumentRowJournal?)listItem.Item;
                    listItem.SetChild(ImageTablePartCell.NewForPixbuf((row?.Spend ?? false) ? InterfaceGtk4.Icon.ForInformation.Check : null));
                };
                ColumnViewColumn column = ColumnViewColumn.New("", factory);
                form.Grid.AppendColumn(column);
            }
  </xsl:template>

  <xsl:template name="AddColumnLabel">
    <xsl:param name="ConfTypeName" />
    <xsl:param name="RowType" />
    <xsl:param name="Fields" />
        <xsl:for-each select="$Fields">
            //Назва: <xsl:value-of select="Name"/>, "<xsl:value-of select="Caption"/>"
            {
                SignalListItemFactory factory = SignalListItemFactory.New();
                factory.OnSetup += (_, args) =&gt;
                {
                    ListItem listItem = (ListItem)args.Object;
                    listItem.Child = LabelTablePartCell.NewFromType("<xsl:value-of select="Type"/>");
                };
                factory.OnBind += (_, args) =&gt;
                {
                    ListItem listItem = (ListItem)args.Object;
                    LabelTablePartCell? labelControl = (LabelTablePartCell?)listItem.Child;
                    <xsl:value-of select="$RowType"/>? row = (<xsl:value-of select="$RowType"/>?)listItem.Item;
                    if (labelControl != null &amp;&amp; row != null)
                        labelControl.SetText(row.Fields["<xsl:value-of select="Name"/>"]);
                };
                ColumnViewColumn column = ColumnViewColumn.New("<xsl:value-of select="Caption"/>", factory);
                column.Resizable = true;
                form.Grid.AppendColumn(column);
            }
        </xsl:for-each>

  </xsl:template>

  <xsl:template name="AddColumnEmpty">
            { /* Пуста колонка для заповнення вільного простору */
                ColumnViewColumn column = ColumnViewColumn.New(null, null);
                column.Resizable = true;
                column.Expand = true;
                form.Grid.AppendColumn(column);
            }
  </xsl:template>

  <!-- Вибірка -->
  <xsl:template name="Select">
    <xsl:param name="ConfTypeGroup" />
    <xsl:param name="ConfTypeName" />
    <xsl:param name="SelectType" />
            <xsl:value-of select="$ConfTypeGroup"/>.<xsl:value-of select="$ConfTypeName"/>_<xsl:value-of select="$SelectType"/><xsl:text> </xsl:text><xsl:value-of select="$ConfTypeName"/>_Select = new();
            <xsl:value-of select="$ConfTypeName"/>_Select.QuerySelect.Field.AddRange(
            [
                "deletion_label",
                <xsl:if test="$ConfTypeGroup = 'Документи'"><!-- Для документів додаткове поле spend -->
                    <xsl:text>"spend"</xsl:text>,
                </xsl:if> 
                <xsl:for-each select="Fields/Field[Type != 'pointer']">
                    <xsl:text>/*</xsl:text><xsl:value-of select="Name"/><xsl:text>*/ </xsl:text>
                    <xsl:value-of select="concat($ConfTypeGroup, '.', $ConfTypeName, '_Const.', Name)"/>,
                </xsl:for-each>
            ]);

            <!-- Добавлення Sql функції для полів тип яких date -->
            <xsl:for-each select="Fields/Field[Type = 'date']">
                /* Форматування дати */
                <xsl:value-of select="$ConfTypeName"/>_Select.QuerySelect.SqlFunc.Add(new SqlFunc(<xsl:value-of select="$ConfTypeGroup"/>.<xsl:value-of select="$ConfTypeName"/>_Const.<xsl:value-of select="Name"/>, "TO_CHAR", ["'dd.mm.yyyy'"]));
            </xsl:for-each>

            <!-- Сортування -->
            <xsl:for-each select="Fields/Field[SortField = 'True']">
                /* Сортування */
                <xsl:variable name="SortDirection">
                    <xsl:choose>
                        <xsl:when test="SortDirection = 'True'">SelectOrder.DESC</xsl:when>
                        <xsl:otherwise>SelectOrder.ASC</xsl:otherwise>
                    </xsl:choose>
                </xsl:variable>
                <xsl:value-of select="$ConfTypeName"/>_Select.QuerySelect.Order.Add(
                <xsl:choose>
                    <xsl:when test="Type = 'pointer'">"<xsl:value-of select="Name"/>"</xsl:when>
                    <xsl:otherwise> <xsl:value-of select="$ConfTypeGroup"/>.<xsl:value-of select="$ConfTypeName"/>_Const.<xsl:value-of select="Name"/></xsl:otherwise>
                </xsl:choose>
                <xsl:text>, </xsl:text><xsl:value-of select="$SortDirection"/>);
            </xsl:for-each>

            <!-- Приєднання таблиць -->
            <xsl:for-each select="Fields/Field[Type = 'pointer']">
                /* Приєднання */
                <xsl:value-of select="substring-before(Pointer, '.')"/>.<xsl:value-of select="substring-after(Pointer, '.')"/>_Pointer.GetJoin(<xsl:value-of select="$ConfTypeName"/>_Select.QuerySelect, <xsl:value-of select="$ConfTypeGroup"/>.<xsl:value-of select="$ConfTypeName"/>_Const.<xsl:value-of select="Name"/>,
                <xsl:value-of select="$ConfTypeName"/>_Select.QuerySelect.Table, "join_tab_<xsl:value-of select="position()"/>", "<xsl:value-of select="Name"/>");
            </xsl:for-each>

            <!-- Додаткові поля -->
            <xsl:for-each select="Fields/AdditionalField[Visible = 'True']">
                /* Додаткове поле: <xsl:value-of select="Name"/> */
                <xsl:value-of select="$ConfTypeName"/>_Select.QuerySelect.FieldAndAlias.Add(
                    new ValueName&lt;string&gt;(@$"(<xsl:value-of select="normalize-space(Value)"/>)", "<xsl:value-of select="Name"/>"));
            </xsl:for-each>
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

namespace <xsl:value-of select="Configuration/NameSpaceGeneratedCode"/>.Довідники.ТабличніСписки
{
    <xsl:for-each select="Configuration/Directories/Directory">
      <xsl:variable name="DirectoryName" select="Name"/>
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
    public static class <xsl:value-of select="$DirectoryName"/>_<xsl:value-of select="$TabularListName"/>
    {
        public static void AddColumn(DirectoryFormJournalBase form)
        {
            <xsl:call-template name="AddColumnImage">
                <xsl:with-param name="RowType">DirectoryRowJournal</xsl:with-param>
            </xsl:call-template>

            <xsl:call-template name="AddColumnLabel">
                <xsl:with-param name="ConfTypeName"><xsl:value-of select="$DirectoryName"/></xsl:with-param>
                <xsl:with-param name="RowType">DirectoryRowJournal</xsl:with-param>
                <xsl:with-param name="Fields" select="Fields/Field" />
            </xsl:call-template>

            <xsl:call-template name="AddColumnLabel">
                <xsl:with-param name="ConfTypeName"><xsl:value-of select="$DirectoryName"/></xsl:with-param>
                <xsl:with-param name="RowType">DirectoryRowJournal</xsl:with-param>
                <xsl:with-param name="Fields" select="Fields/AdditionalField[Visible = 'True']" />
            </xsl:call-template>

            <xsl:call-template name="AddColumnEmpty" />
        }

        public static void CreateFilter(DirectoryFormJournalBase form)
        {
            <xsl:call-template name="CreateFilter">
                <xsl:with-param name="ConfTypeName"><xsl:value-of select="$DirectoryName"/></xsl:with-param>
            </xsl:call-template>
        }

        public static async ValueTask UpdateRecords(DirectoryFormJournalBase form)
        {
            List&lt;ObjectChanged&gt; records = [];
            lock (form.Loсked)
            {
                while(form.RecordsChangedQueue.Count &gt; 0)
                    records.AddRange(form.RecordsChangedQueue.Dequeue());
            }
            
            /* Вибірка */
            <xsl:call-template name="Select">
                <xsl:with-param name="ConfTypeGroup">Довідники</xsl:with-param>
                <xsl:with-param name="ConfTypeName"><xsl:value-of select="$DirectoryName"/></xsl:with-param>
                <xsl:with-param name="SelectType"><xsl:value-of select="$SelectType"/></xsl:with-param>
            </xsl:call-template>

            /* Відбори */
            <xsl:value-of select="$DirectoryName"/>_Select.QuerySelect.Where.Add(new Where("uid", Comparison.IN, "'" + string.Join("', '", records.Select(x =&gt; x.Uid)) + "'", true));

            <!-- Вибрати дані -->
            await <xsl:value-of select="$DirectoryName"/>_Select.Select();
            while (<xsl:value-of select="$DirectoryName"/>_Select.MoveNext())
            {
                Довідники.<xsl:value-of select="$DirectoryName"/>_Pointer? curr = <xsl:value-of select="$DirectoryName"/>_Select.Current;
                if (curr != null)
                {
                    Dictionary&lt;string, object&gt; Fields = curr.Fields;
                    DirectoryRowJournal row = new() { UnigueID = curr.UnigueID, DeletionLabel = (bool)Fields["deletion_label"] };
                    <xsl:for-each select="Fields/Field">
                        <xsl:text>row.Fields.Add("</xsl:text><xsl:value-of select="Name"/>", <xsl:call-template name="FieldValue"><xsl:with-param name="ConfTypeName"><xsl:value-of select="$DirectoryName"/></xsl:with-param></xsl:call-template>);
                    </xsl:for-each>
                    <xsl:for-each select="Fields/AdditionalField[Visible = 'True']">
                        <xsl:text>row.Fields.Add("</xsl:text><xsl:value-of select="Name"/>", Fields["<xsl:value-of select="Name"/>"].ToString() ?? "");
                    </xsl:for-each>
                    ObjectChanged? objCh = records.Find(x =&gt; x.Uid.Equals(curr.UnigueID.UGuid));
                    if (objCh != null)
                    {
                        bool exist = false;
                        for (uint i = 0; i &lt; form.Store.GetNItems(); i++)
                        {
                            RowJournal? item = (RowJournal?)form.Store.GetObject(i);
                            if (item != null &amp;&amp; item.UnigueID.Equals(curr.UnigueID))
                            {
                                bool sel = form.Grid.Model.IsSelected(i);
                                form.Store.Splice(i, 1, [row], 1);
                                if (sel) form.Grid.Model.SelectItem(i, false);
                                exist = true;
                                break;
                            }
                        }
                        if (!exist &amp;&amp; objCh.Type == TypeObjectChanged.Add)
                            form.Store.Append(row);
                    }
                }
            }
        }

        public static async ValueTask LoadRecords(DirectoryFormJournalBase form)
        {
            form.BeforeLoadRecords();
            UnigueID? unigueIDSelect = form.SelectPointerItem ?? form.DirectoryPointerItem;

            /* Вибірка */
            <xsl:call-template name="Select">
                <xsl:with-param name="ConfTypeGroup">Довідники</xsl:with-param>
                <xsl:with-param name="ConfTypeName"><xsl:value-of select="$DirectoryName"/></xsl:with-param>
                <xsl:with-param name="SelectType"><xsl:value-of select="$SelectType"/></xsl:with-param>
            </xsl:call-template>

            /* Відбори */
            if (form.WhereList != null) <xsl:value-of select="$DirectoryName"/>_Select.QuerySelect.Where.AddRange(form.WhereList);

            /* Cторінки */
            await form.SplitPages(<xsl:value-of select="$DirectoryName"/>_Select.SplitSelectToPages, <xsl:value-of select="$DirectoryName"/>_Select.QuerySelect, unigueIDSelect);

            <!-- Вибрати дані -->
            await <xsl:value-of select="$DirectoryName"/>_Select.Select();
            form.Store.RemoveAll();
            uint selectPosition = 0;
            while (<xsl:value-of select="$DirectoryName"/>_Select.MoveNext())
            {
                Довідники.<xsl:value-of select="$DirectoryName"/>_Pointer? curr = <xsl:value-of select="$DirectoryName"/>_Select.Current;
                if (curr != null)
                {
                    Dictionary&lt;string, object&gt; Fields = curr.Fields;
                    DirectoryRowJournal row = new() { UnigueID = curr.UnigueID, DeletionLabel = (bool)Fields["deletion_label"] };
                    <xsl:for-each select="Fields/Field">
                        <xsl:text>row.Fields.Add("</xsl:text><xsl:value-of select="Name"/>", <xsl:call-template name="FieldValue"><xsl:with-param name="ConfTypeName"><xsl:value-of select="$DirectoryName"/></xsl:with-param></xsl:call-template>);
                    </xsl:for-each>
                    <xsl:for-each select="Fields/AdditionalField[Visible = 'True']">
                        <xsl:text>row.Fields.Add("</xsl:text><xsl:value-of select="Name"/>", Fields["<xsl:value-of select="Name"/>"].ToString() ?? "");
                    </xsl:for-each>
                    form.Store.Append(row);
                    if (row.UnigueID.Equals(unigueIDSelect)) selectPosition = form.Store.GetNItems();
                }
            }
            form.AfterLoadRecords(selectPosition);
        }
    }
        </xsl:for-each>
    #endregion
    </xsl:for-each>
}

namespace <xsl:value-of select="Configuration/NameSpaceGeneratedCode"/>.Документи.ТабличніСписки
{
    <xsl:for-each select="Configuration/Documents/Document">
      <xsl:variable name="DocumentName" select="Name"/>
      <xsl:variable name="SelectType">Select</xsl:variable>
    #region DOCUMENT "<xsl:value-of select="$DocumentName"/>"
        <xsl:for-each select="TabularLists/TabularList">
            <xsl:variable name="TabularListName" select="Name"/>
    public static class <xsl:value-of select="$DocumentName"/>_<xsl:value-of select="$TabularListName"/>
    {
        public static void AddColumn(DocumentFormJournalBase form)
        {
            <xsl:call-template name="AddColumnImage">
                <xsl:with-param name="RowType">DocumentRowJournal</xsl:with-param>
            </xsl:call-template>

            <xsl:call-template name="AddColumnSpend" />

            <xsl:call-template name="AddColumnLabel">
                <xsl:with-param name="ConfTypeName"><xsl:value-of select="$DocumentName"/></xsl:with-param>
                <xsl:with-param name="RowType">DocumentRowJournal</xsl:with-param>
                <xsl:with-param name="Fields" select="Fields/Field" />
            </xsl:call-template>

            <xsl:call-template name="AddColumnLabel">
                <xsl:with-param name="ConfTypeName"><xsl:value-of select="$DocumentName"/></xsl:with-param>
                <xsl:with-param name="RowType">DocumentRowJournal</xsl:with-param>
                <xsl:with-param name="Fields" select="Fields/AdditionalField[Visible = 'True']" />
            </xsl:call-template>

            <xsl:call-template name="AddColumnEmpty" />
        }

        public static void CreateFilter(DocumentFormJournalBase form)
        {
            <xsl:call-template name="CreateFilter">
                <xsl:with-param name="ConfTypeName"><xsl:value-of select="$DocumentName"/></xsl:with-param>
            </xsl:call-template>
        }

        public static async ValueTask UpdateRecords(DocumentFormJournalBase form)
        {
            List&lt;ObjectChanged&gt; records = [];
            lock (form.Loсked)
            {
                while(form.RecordsChangedQueue.Count &gt; 0)
                    records.AddRange(form.RecordsChangedQueue.Dequeue());
            }
            
            /* Вибірка */
            <xsl:call-template name="Select">
                <xsl:with-param name="ConfTypeGroup">Документи</xsl:with-param>
                <xsl:with-param name="ConfTypeName"><xsl:value-of select="$DocumentName"/></xsl:with-param>
                <xsl:with-param name="SelectType"><xsl:value-of select="$SelectType"/></xsl:with-param>
            </xsl:call-template>

            /* Відбори */
            <xsl:value-of select="$DocumentName"/>_Select.QuerySelect.Where.Add(new Where("uid", Comparison.IN, "'" + string.Join("', '", records.Select(x =&gt; x.Uid)) + "'", true));

            <!-- Вибрати дані -->
            await <xsl:value-of select="$DocumentName"/>_Select.Select();
            while (<xsl:value-of select="$DocumentName"/>_Select.MoveNext())
            {
                Документи.<xsl:value-of select="$DocumentName"/>_Pointer? curr = <xsl:value-of select="$DocumentName"/>_Select.Current;
                if (curr != null)
                {
                    Dictionary&lt;string, object&gt; Fields = curr.Fields;
                    DocumentRowJournal row = new() { UnigueID = curr.UnigueID, DeletionLabel = (bool)Fields["deletion_label"], Spend = (bool)Fields["spend"] };
                    <xsl:for-each select="Fields/Field">
                        <xsl:text>row.Fields.Add("</xsl:text><xsl:value-of select="Name"/>", <xsl:call-template name="FieldValue"><xsl:with-param name="ConfTypeName"><xsl:value-of select="$DocumentName"/></xsl:with-param></xsl:call-template>);
                    </xsl:for-each>
                    <xsl:for-each select="Fields/AdditionalField[Visible = 'True']">
                        <xsl:text>row.Fields.Add("</xsl:text><xsl:value-of select="Name"/>", Fields["<xsl:value-of select="Name"/>"].ToString() ?? "");
                    </xsl:for-each>
                    ObjectChanged? objCh = records.Find(x =&gt; x.Uid.Equals(curr.UnigueID.UGuid));
                    if (objCh != null)
                    {
                        bool exist = false;
                        for (uint i = 0; i &lt; form.Store.GetNItems(); i++)
                        {
                            RowJournal? item = (RowJournal?)form.Store.GetObject(i);
                            if (item != null &amp;&amp; item.UnigueID.Equals(curr.UnigueID))
                            {
                                bool sel = form.Grid.Model.IsSelected(i);
                                form.Store.Splice(i, 1, [row], 1);
                                if (sel) form.Grid.Model.SelectItem(i, false);
                                exist = true;
                                break;
                            }
                        }
                        if (!exist &amp;&amp; objCh.Type == TypeObjectChanged.Add)
                            form.Store.Append(row);
                    }
                }
            }
        }

        public static async ValueTask LoadRecords(DocumentFormJournalBase form)
        {
            form.BeforeLoadRecords();
            UnigueID? unigueIDSelect = form.SelectPointerItem ?? form.DocumentPointerItem;

            /* Вибірка */
            <xsl:call-template name="Select">
                <xsl:with-param name="ConfTypeGroup">Документи</xsl:with-param>
                <xsl:with-param name="ConfTypeName"><xsl:value-of select="$DocumentName"/></xsl:with-param>
                <xsl:with-param name="SelectType"><xsl:value-of select="$SelectType"/></xsl:with-param>
            </xsl:call-template>

            /* Відбори */
            if (form.WhereList != null) <xsl:value-of select="$DocumentName"/>_Select.QuerySelect.Where.AddRange(form.WhereList);

            /* Відбір за період */
             if (form.TypeWhereState == InterfaceGtk4.FormJournal.TypeWhere.Standart || (form.TypeWhereState == InterfaceGtk4.FormJournal.TypeWhere.Filter &amp;&amp; form.Filter.IsUsePeriod))
            {
                Where? where = InterfaceGtk4.PeriodForJournal.ВідбірПоПеріоду(Документи.<xsl:value-of select="$DocumentName"/>_Const.ДатаДок, form.Period.Period, form.Period.DateStart, form.Period.DateStop);
                if (where != null) <xsl:value-of select="$DocumentName"/>_Select.QuerySelect.Where.Add(where);
            }

            /* Cторінки */
            await form.SplitPages(<xsl:value-of select="$DocumentName"/>_Select.SplitSelectToPages, <xsl:value-of select="$DocumentName"/>_Select.QuerySelect, unigueIDSelect);

            <!-- Вибрати дані -->
            await <xsl:value-of select="$DocumentName"/>_Select.Select();
            form.Store.RemoveAll();
            uint selectPosition = 0;
            while (<xsl:value-of select="$DocumentName"/>_Select.MoveNext())
            {
                Документи.<xsl:value-of select="$DocumentName"/>_Pointer? curr = <xsl:value-of select="$DocumentName"/>_Select.Current;
                if (curr != null)
                {
                    Dictionary&lt;string, object&gt; Fields = curr.Fields;
                    DocumentRowJournal row = new() { UnigueID = curr.UnigueID, DeletionLabel = (bool)Fields["deletion_label"], Spend = (bool)Fields["spend"] };
                    <xsl:for-each select="Fields/Field">
                        <xsl:text>row.Fields.Add("</xsl:text><xsl:value-of select="Name"/>", <xsl:call-template name="FieldValue"><xsl:with-param name="ConfTypeName"><xsl:value-of select="$DocumentName"/></xsl:with-param></xsl:call-template>);
                    </xsl:for-each>
                    <xsl:for-each select="Fields/AdditionalField[Visible = 'True']">
                        <xsl:text>row.Fields.Add("</xsl:text><xsl:value-of select="Name"/>", Fields["<xsl:value-of select="Name"/>"].ToString() ?? "");
                    </xsl:for-each>
                    form.Store.Append(row);
                    if (row.UnigueID.Equals(unigueIDSelect)) selectPosition = form.Store.GetNItems();
                }
            }
            form.AfterLoadRecords(selectPosition);
        }
    }
        </xsl:for-each>
    #endregion
    </xsl:for-each>
}

  </xsl:template>
</xsl:stylesheet>