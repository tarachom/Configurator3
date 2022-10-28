<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="text" indent="yes" />

  <xsl:template name="License">
/*
Copyright (C) 2019-2022 TARAKHOMYN YURIY IVANOVYCH
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
    <xsl:call-template name="License" />
/*
 *
 * Конфігурації "<xsl:value-of select="Configuration/Name"/>"
 * Автор <xsl:value-of select="Configuration/Author"/>
 * Дата конфігурації: <xsl:value-of select="Configuration/DateTimeSave"/>
 *
 */
 
using Gtk;
using AccountingSoftware;

namespace <xsl:value-of select="Configuration/NameSpace"/>.Довідники.ТабличніСписки
{
    <xsl:for-each select="Configuration/Directories/Directory">
      <xsl:variable name="DirectoryName" select="Name"/>
    #region DIRECTORY "<xsl:value-of select="$DirectoryName"/>"
    
      <xsl:for-each select="TabularLists/TabularList">
        <xsl:variable name="TabularListName" select="Name"/>
    public class <xsl:value-of select="$DirectoryName"/>_<xsl:value-of select="$TabularListName"/>
    {
        string Image = "doc.png";
        string ID = "";
        <xsl:for-each select="Fields/Field">
        string <xsl:value-of select="Name"/> = "";</xsl:for-each>

        Array ToArray()
        {
            return new object[] { new Gdk.Pixbuf(Image), ID 
            /* */ <xsl:for-each select="Fields/Field">
              <xsl:text>, </xsl:text>
              <xsl:value-of select="Name"/>
            </xsl:for-each> };
        }

        public static ListStore Store = new ListStore(typeof(Gdk.Pixbuf) /* Image */, typeof(string) /* ID */
            <xsl:for-each select="Fields/Field">
              <xsl:text>, typeof(string)</xsl:text> /* <xsl:value-of select="Name"/> */
            </xsl:for-each>);

        public static void AddColumns(TreeView treeView)
        {
            treeView.AppendColumn(new TreeViewColumn("", new CellRendererPixbuf() { Ypad = 4 }, "pixbuf", 0));
            treeView.AppendColumn(new TreeViewColumn("ID", new CellRendererText(), "text", 1) { Visible = false });
            /* */
            <xsl:for-each select="Fields/Field">
              <xsl:text>treeView.AppendColumn(new TreeViewColumn("</xsl:text>
              <xsl:value-of select="normalize-space(Caption)"/>
              <xsl:text>", new CellRendererText() { Xpad = 4 }, "text", </xsl:text>
              <xsl:value-of select="position() + 1"/>
              <xsl:text>) { SortColumnId = </xsl:text>
              <xsl:value-of select="position() + 1"/>
              <xsl:if test="Size != '0'">
                <xsl:text>, FixedWidth = </xsl:text>
                <xsl:value-of select="Size"/>
              </xsl:if>
              <xsl:text> } )</xsl:text>; /*<xsl:value-of select="Name"/>*/
            </xsl:for-each>
        }

        public static void LoadRecords()
        {
            Store.Clear();

            Довідники.<xsl:value-of select="$DirectoryName"/>_Select <xsl:value-of select="$DirectoryName"/>_Select = new Довідники.<xsl:value-of select="$DirectoryName"/>_Select();
            <xsl:value-of select="$DirectoryName"/>_Select.QuerySelect.Field.AddRange(
                new string[]
                {
                    <xsl:for-each select="Fields/Field[Type != 'pointer']">
                        <xsl:if test="position() &gt; 1">, </xsl:if>
                        <xsl:text>Довідники.</xsl:text>
                        <xsl:value-of select="$DirectoryName"/>
                        <xsl:text>_Const.</xsl:text>
                        <xsl:value-of select="Name"/> /* <xsl:value-of select="position()"/> */
                    </xsl:for-each>
                });

            <xsl:for-each select="Fields/Field[SortField = 'True' and Type != 'pointer']">
              /* ORDER */
              <xsl:value-of select="$DirectoryName"/>_Select.QuerySelect.Order.Add(Довідники.<xsl:value-of select="$DirectoryName"/>_Const.<xsl:value-of select="Name"/>, SelectOrder.ASC);
            </xsl:for-each>

            <xsl:for-each select="Fields/Field[Type = 'pointer']">
                /* Join Table */
                <xsl:value-of select="$DirectoryName"/>_Select.QuerySelect.Joins.Add(
                    new Join(<xsl:value-of select="Join/table"/>, Довідники.<xsl:value-of select="$DirectoryName"/>_Const.<xsl:value-of select="Join/field"/>, <xsl:value-of select="$DirectoryName"/>_Select.QuerySelect.Table, "<xsl:value-of select="Join/alias"/>"));
                <xsl:for-each select="FieldAndAlias">
                  /* Field */
                  <xsl:value-of select="$DirectoryName"/>_Select.QuerySelect.FieldAndAlias.Add(
                    new NameValue&lt;string&gt;("<xsl:value-of select="table"/>." + <xsl:value-of select="field"/>, "<xsl:value-of select="table"/>_field_<xsl:value-of select="position()"/>"));
                  <xsl:if test="../SortField = 'True'">
                    /* ORDER */
                    <xsl:value-of select="$DirectoryName"/>_Select.QuerySelect.Order.Add("<xsl:value-of select="table"/>_field_<xsl:value-of select="position()"/>", SelectOrder.ASC);
                  </xsl:if>
                </xsl:for-each>
            </xsl:for-each>

            /* SELECT */
            <xsl:value-of select="$DirectoryName"/>_Select.Select();
            while (<xsl:value-of select="$DirectoryName"/>_Select.MoveNext())
            {
                Довідники.<xsl:value-of select="$DirectoryName"/>_Pointer? cur = <xsl:value-of select="$DirectoryName"/>_Select.Current;

                if (cur != null)
                    Store.AppendValues(new <xsl:value-of select="$DirectoryName"/>_<xsl:value-of select="$TabularListName"/>
                    {
                        ID = cur.UnigueID.ToString(),
                        <xsl:variable name="CountPointer" select="count(Fields/Field[Type = 'pointer'])"/>
                        <xsl:variable name="CountNotPointer" select="count(Fields/Field[Type != 'pointer'])"/>
                        <xsl:for-each select="Fields/Field[Type = 'pointer']">
                          <xsl:value-of select="Name"/>
                          <xsl:text> = </xsl:text>
                          <xsl:variable name="CountAlias" select="count(FieldAndAlias)"/>
                          <xsl:for-each select="FieldAndAlias">
                            <xsl:if test="position() &gt; 1"> + " " + </xsl:if>
                            <xsl:text>cur.Fields?[</xsl:text>"<xsl:value-of select="table"/>_field_<xsl:value-of select="position()"/><xsl:text>"]?.ToString()</xsl:text>
                            <xsl:if test="$CountAlias = 1"> ?? ""</xsl:if>
                          </xsl:for-each>
                          <xsl:if test="$CountNotPointer != 0 or position() != $CountPointer">,</xsl:if> /**/
                        </xsl:for-each>
                        <xsl:for-each select="Fields/Field[Type != 'pointer']">
                          <xsl:value-of select="Name"/>
                          <xsl:text> = </xsl:text>
                          <xsl:choose>
                            <xsl:when test="Type = 'enum'">
                              <xsl:text>((</xsl:text>
                              <xsl:value-of select="Pointer"/>
                              <xsl:text>)</xsl:text>
                              <xsl:text>(cur.Fields?[</xsl:text>
                              <xsl:value-of select="$DirectoryName"/>
                              <xsl:text>_Const.</xsl:text>
                              <xsl:value-of select="Name"/>
                              <xsl:text>]!)).ToString()</xsl:text>
                            </xsl:when>
                            <xsl:otherwise>
                              <xsl:text>cur.Fields?[</xsl:text>
                              <xsl:value-of select="$DirectoryName"/>
                              <xsl:text>_Const.</xsl:text>
                              <xsl:value-of select="Name"/>
                              <xsl:text>]?.ToString() ?? ""</xsl:text>
                            </xsl:otherwise>
                          </xsl:choose>
                          <xsl:if test="position() != $CountNotPointer">,</xsl:if> /**/
                        </xsl:for-each>
                    }.ToArray());
            }
        }
    }
	    </xsl:for-each>
    #endregion
    </xsl:for-each>
}

namespace <xsl:value-of select="Configuration/NameSpace"/>.Документи.ТабличніСписки
{
    <xsl:for-each select="Configuration/Documents/Document">
      <xsl:variable name="DocumentName" select="Name"/>
    #region DOCUMENT "<xsl:value-of select="$DocumentName"/>"
    
      <xsl:for-each select="TabularLists/TabularList">
        <xsl:variable name="TabularListName" select="Name"/>
    public class <xsl:value-of select="$DocumentName"/>_<xsl:value-of select="$TabularListName"/>
    {
        string Image = "doc.png";
        bool Spend = false;
        string ID = "";
        <xsl:for-each select="Fields/Field">
        string <xsl:value-of select="Name"/> = "";</xsl:for-each>

        Array ToArray()
        {
            return new object[] { new Gdk.Pixbuf(Image), ID, Spend /*Проведений документ*/
            /* */ <xsl:for-each select="Fields/Field">
              <xsl:text>, </xsl:text>
              <xsl:value-of select="Name"/>
            </xsl:for-each> };
        }

        public static ListStore Store = new ListStore(typeof(Gdk.Pixbuf) /* Image */, typeof(string) /* ID */, typeof(bool) /* Spend Проведений документ*/
            <xsl:for-each select="Fields/Field">
              <xsl:text>, typeof(string)</xsl:text> /* <xsl:value-of select="Name"/> */
            </xsl:for-each>);

        public static void AddColumns(TreeView treeView)
        {
            treeView.AppendColumn(new TreeViewColumn("", new CellRendererPixbuf() { Ypad = 4 }, "pixbuf", 0)); /*Image*/
            treeView.AppendColumn(new TreeViewColumn("ID", new CellRendererText(), "text", 1) { Visible = false }); /*UID*/
            treeView.AppendColumn(new TreeViewColumn("", new CellRendererToggle(), "active", 2)); /*Проведений документ*/
            /* */
            <xsl:for-each select="Fields/Field">
              <xsl:text>treeView.AppendColumn(new TreeViewColumn("</xsl:text>
              <xsl:value-of select="normalize-space(Caption)"/>
              <xsl:text>", new CellRendererText() { Xpad = 4 }, "text", </xsl:text>
              <xsl:value-of select="position() + 2"/>
              <xsl:text>)</xsl:text>
              <xsl:if test="Size != '0'">
                <xsl:text> { FixedWidth = </xsl:text>
                <xsl:value-of select="Size"/>
                <xsl:text> } </xsl:text>
              </xsl:if>); /*<xsl:value-of select="Name"/>*/
            </xsl:for-each>
        }

        public static void LoadRecords()
        {
            Store.Clear();

            Документи.<xsl:value-of select="$DocumentName"/>_Select <xsl:value-of select="$DocumentName"/>_Select = new Документи.<xsl:value-of select="$DocumentName"/>_Select();
            <xsl:value-of select="$DocumentName"/>_Select.QuerySelect.Field.AddRange(
                new string[]
                { "spend" /*Проведений документ*/
                    <xsl:for-each select="Fields/Field[Type != 'pointer']">
                        <xsl:text>, </xsl:text>
                        <xsl:text>Документи.</xsl:text>
                        <xsl:value-of select="$DocumentName"/>
                        <xsl:text>_Const.</xsl:text>
                        <xsl:value-of select="Name"/> /* <xsl:value-of select="position()"/> */
                    </xsl:for-each>
                });

            <xsl:for-each select="Fields/Field[SortField = 'True' and Type != 'pointer']">
              /* ORDER */
              <xsl:value-of select="$DocumentName"/>_Select.QuerySelect.Order.Add(Документи.<xsl:value-of select="$DocumentName"/>_Const.<xsl:value-of select="Name"/>, SelectOrder.ASC);
            </xsl:for-each>

            <xsl:for-each select="Fields/Field[Type = 'pointer']">
                /* Join Table */
                <xsl:value-of select="$DocumentName"/>_Select.QuerySelect.Joins.Add(
                    new Join(<xsl:value-of select="Join/table"/>, Документи.<xsl:value-of select="$DocumentName"/>_Const.<xsl:value-of select="Join/field"/>, <xsl:value-of select="$DocumentName"/>_Select.QuerySelect.Table, "<xsl:value-of select="Join/alias"/>"));
                <xsl:for-each select="FieldAndAlias">
                  /* Field */
                  <xsl:value-of select="$DocumentName"/>_Select.QuerySelect.FieldAndAlias.Add(
                    new NameValue&lt;string&gt;("<xsl:value-of select="table"/>." + <xsl:value-of select="field"/>, "<xsl:value-of select="table"/>_field_<xsl:value-of select="position()"/>"));
                  <xsl:if test="../SortField = 'True'">
                    /* ORDER */
                    <xsl:value-of select="$DocumentName"/>_Select.QuerySelect.Order.Add("<xsl:value-of select="table"/>_field_<xsl:value-of select="position()"/>", SelectOrder.ASC);
                  </xsl:if>
                </xsl:for-each>
            </xsl:for-each>

            /* SELECT */
            <xsl:value-of select="$DocumentName"/>_Select.Select();
            while (<xsl:value-of select="$DocumentName"/>_Select.MoveNext())
            {
                Документи.<xsl:value-of select="$DocumentName"/>_Pointer? cur = <xsl:value-of select="$DocumentName"/>_Select.Current;

                if (cur != null)
                    Store.AppendValues(new <xsl:value-of select="$DocumentName"/>_<xsl:value-of select="$TabularListName"/>
                    {
                        ID = cur.UnigueID.ToString(),
                        Spend = (bool)cur.Fields?["spend"]!, /*Проведений документ*/
                        <xsl:variable name="CountPointer" select="count(Fields/Field[Type = 'pointer'])"/>
                        <xsl:variable name="CountNotPointer" select="count(Fields/Field[Type != 'pointer'])"/>
                        <xsl:for-each select="Fields/Field[Type = 'pointer']">
                          <xsl:value-of select="Name"/>
                          <xsl:text> = </xsl:text>
                          <xsl:variable name="CountAlias" select="count(FieldAndAlias)"/>
                          <xsl:for-each select="FieldAndAlias">
                            <xsl:if test="position() &gt; 1"> + " " + </xsl:if>
                            <xsl:text>cur.Fields?[</xsl:text>"<xsl:value-of select="table"/>_field_<xsl:value-of select="position()"/><xsl:text>"]?.ToString()</xsl:text>
                            <xsl:if test="$CountAlias = 1"> ?? ""</xsl:if>
                          </xsl:for-each>
                          <xsl:if test="$CountNotPointer != 0 or position() != $CountPointer">,</xsl:if> /**/
                        </xsl:for-each>
                        <xsl:for-each select="Fields/Field[Type != 'pointer']">
                          <xsl:value-of select="Name"/>
                          <xsl:text> = </xsl:text>
                          <xsl:choose>
                            <xsl:when test="Type = 'enum'">
                              <xsl:text>((</xsl:text>
                              <xsl:value-of select="Pointer"/>
                              <xsl:text>)</xsl:text>
                              <xsl:text>(cur.Fields?[</xsl:text>
                              <xsl:value-of select="$DocumentName"/>
                              <xsl:text>_Const.</xsl:text>
                              <xsl:value-of select="Name"/>
                              <xsl:text>]!)).ToString()</xsl:text>
                            </xsl:when>
                            <xsl:otherwise>
                              <xsl:text>cur.Fields?[</xsl:text>
                              <xsl:value-of select="$DocumentName"/>
                              <xsl:text>_Const.</xsl:text>
                              <xsl:value-of select="Name"/>
                              <xsl:text>]?.ToString() ?? ""</xsl:text>
                            </xsl:otherwise>
                          </xsl:choose>
                          <xsl:if test="position() != $CountNotPointer">,</xsl:if> /**/
                        </xsl:for-each>
                    }.ToArray());
            }
        }
    }
	    </xsl:for-each>
    #endregion
    </xsl:for-each>
}

  </xsl:template>
</xsl:stylesheet>