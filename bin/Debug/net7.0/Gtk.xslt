<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="text" indent="yes" />

  <xsl:template name="License">
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
        string Image = "images/doc.png";
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
            treeView.AppendColumn(new TreeViewColumn("", new CellRendererPixbuf(), "pixbuf", 0)); /* { Ypad = 4 } */
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
            //Пустишка
            treeView.AppendColumn(new TreeViewColumn());
        }

        public static List&lt;Where&gt; Where { get; set; } = new List&lt;Where&gt;();

        public static Довідники.<xsl:value-of select="$DirectoryName"/>_Pointer? DirectoryPointerItem { get; set; }
        public static Довідники.<xsl:value-of select="$DirectoryName"/>_Pointer? SelectPointerItem { get; set; }
        public static TreePath? SelectPath;
        public static TreePath? CurrentPath;

        public static void LoadRecords()
        {
            Store.Clear();
            SelectPath = null;

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

            /* Where */
            <xsl:value-of select="$DirectoryName"/>_Select.QuerySelect.Where = Where;

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
                {
                    <xsl:value-of select="$DirectoryName"/>_<xsl:value-of select="$TabularListName"/> Record = new <xsl:value-of select="$DirectoryName"/>_<xsl:value-of select="$TabularListName"/>
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
                              <xsl:variable name="namePointer" select="substring-after(Pointer, '.')" />
                              <xsl:text>Перелічення.ПсевдонімиПерелічення.</xsl:text>
                              <xsl:value-of select="$namePointer"/>
                              <xsl:text>_Alias( </xsl:text>
                              <xsl:text>((</xsl:text>
                              <xsl:value-of select="Pointer"/>
                              <xsl:text>)</xsl:text>
                              <xsl:text>(cur.Fields?[</xsl:text>
                              <xsl:value-of select="$DirectoryName"/>
                              <xsl:text>_Const.</xsl:text>
                              <xsl:value-of select="Name"/>
                              <xsl:text>]! != DBNull.Value ? cur.Fields?[</xsl:text>
                              <xsl:value-of select="$DirectoryName"/>
                              <xsl:text>_Const.</xsl:text>
                              <xsl:value-of select="Name"/>
                              <xsl:text>]! : 0)) )</xsl:text>
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
                    };

                    TreeIter CurrentIter = Store.AppendValues(Record.ToArray());
                    CurrentPath = Store.GetPath(CurrentIter);

                    if (DirectoryPointerItem != null || SelectPointerItem != null)
                    {
                        string UidSelect = SelectPointerItem != null ? SelectPointerItem.UnigueID.ToString() : DirectoryPointerItem!.UnigueID.ToString();

                        if (Record.ID == UidSelect)
                            SelectPath = CurrentPath;
                    }
                }
            }
        }
    }
	    </xsl:for-each>
    #endregion
    </xsl:for-each>
}

namespace <xsl:value-of select="Configuration/NameSpace"/>.Документи.ТабличніСписки
{
    public static class Інтерфейс
    {
        public static ComboBoxText СписокВідбірПоПеріоду()
        {
            ComboBoxText сomboBox = new ComboBoxText();

            if (Config.Kernel != null)
            {
                ConfigurationEnums ТипПеріодуДляЖурналівДокументів = Config.Kernel.Conf.Enums["ТипПеріодуДляЖурналівДокументів"];

                foreach (ConfigurationEnumField field in ТипПеріодуДляЖурналівДокументів.Fields.Values)
                    сomboBox.Append(field.Name, field.Desc);
            }

            return сomboBox;
        }

        public static void ДодатиВідбірПоПеріоду(List&lt;Where&gt; Where, string fieldWhere, Перелічення.ТипПеріодуДляЖурналівДокументів типПеріоду)
        {
            switch (типПеріоду)
            {
                case Перелічення.ТипПеріодуДляЖурналівДокументів.ЗПочаткуРоку:
                {
                    Where.Add(new Where(fieldWhere, Comparison.QT_EQ, new DateTime(DateTime.Now.Year, 1, 1)));
                    break;
                }
                case Перелічення.ТипПеріодуДляЖурналівДокументів.Квартал:
                {
                    DateTime ДатаТриМісцяНазад = DateTime.Now.AddMonths(-3);
                    Where.Add(new Where(fieldWhere, Comparison.QT_EQ, new DateTime(ДатаТриМісцяНазад.Year, ДатаТриМісцяНазад.Month, 1)));
                    break;
                }
                case Перелічення.ТипПеріодуДляЖурналівДокументів.ЗМинулогоМісяця:
                {
                    DateTime ДатаМісцьНазад = DateTime.Now.AddMonths(-1);
                    Where.Add(new Where(fieldWhere, Comparison.QT_EQ, new DateTime(ДатаМісцьНазад.Year, ДатаМісцьНазад.Month, 1)));
                    break;
                }
                case Перелічення.ТипПеріодуДляЖурналівДокументів.ЗПочаткуМісяця:
                {
                    Where.Add(new Where(fieldWhere, Comparison.QT_EQ, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)));
                    break;
                }
                case Перелічення.ТипПеріодуДляЖурналівДокументів.ЗПочаткуТижня:
                {
                    DateTime СімДнівНазад = DateTime.Now.AddDays(-7);
                    Where.Add(new Where(fieldWhere, Comparison.QT_EQ, new DateTime(СімДнівНазад.Year, СімДнівНазад.Month, СімДнівНазад.Day)));
                    break;
                }
                case Перелічення.ТипПеріодуДляЖурналівДокументів.ПоточнийДень:
                {
                    Where.Add(new Where(fieldWhere, Comparison.QT_EQ, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)));
                    break;
                }
            }
        }
    }

    <xsl:for-each select="Configuration/Documents/Document">
      <xsl:variable name="DocumentName" select="Name"/>
    #region DOCUMENT "<xsl:value-of select="$DocumentName"/>"
    
      <xsl:for-each select="TabularLists/TabularList">
        <xsl:variable name="TabularListName" select="Name"/>
    public class <xsl:value-of select="$DocumentName"/>_<xsl:value-of select="$TabularListName"/>
    {
        string Image = "images/doc.png";
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
            treeView.AppendColumn(new TreeViewColumn("", new CellRendererPixbuf(), "pixbuf", 0)); /*Image*/ /* { Ypad = 0 } */
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
            //Пустишка
            treeView.AppendColumn(new TreeViewColumn());
        }

        public static List&lt;Where&gt; Where { get; set; } = new List&lt;Where&gt;();

        public static void ДодатиВідбірПоПеріоду(Перелічення.ТипПеріодуДляЖурналівДокументів типПеріоду)
        {
            Where.Clear();
            Інтерфейс.ДодатиВідбірПоПеріоду(Where, Документи.<xsl:value-of select="$DocumentName"/>_Const.ДатаДок, типПеріоду);
        }

        public static Документи.<xsl:value-of select="$DocumentName"/>_Pointer? DocumentPointerItem { get; set; }
        public static Документи.<xsl:value-of select="$DocumentName"/>_Pointer? SelectPointerItem { get; set; }
        public static TreePath? SelectPath;
        public static TreePath? CurrentPath;

        public static void LoadRecords()
        {
            Store.Clear();
            SelectPath = null;

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

            /* Where */
            <xsl:value-of select="$DocumentName"/>_Select.QuerySelect.Where = Where;

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
                {
                    <xsl:value-of select="$DocumentName"/>_<xsl:value-of select="$TabularListName"/> Record = new <xsl:value-of select="$DocumentName"/>_<xsl:value-of select="$TabularListName"/>
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
                              <xsl:variable name="namePointer" select="substring-after(Pointer, '.')" />
                              <xsl:text>Перелічення.ПсевдонімиПерелічення.</xsl:text>
                              <xsl:value-of select="$namePointer"/>
                              <xsl:text>_Alias( </xsl:text>
                              <xsl:text>((</xsl:text>
                              <xsl:value-of select="Pointer"/>
                              <xsl:text>)</xsl:text>
                              <xsl:text>(cur.Fields?[</xsl:text>
                              <xsl:value-of select="$DocumentName"/>
                              <xsl:text>_Const.</xsl:text>
                              <xsl:value-of select="Name"/>
                              <xsl:text>]! != DBNull.Value ? cur.Fields?[</xsl:text>
                              <xsl:value-of select="$DocumentName"/>
                              <xsl:text>_Const.</xsl:text>
                              <xsl:value-of select="Name"/>
                              <xsl:text>]! : 0)) )</xsl:text>
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
                    };

                    TreeIter CurrentIter = Store.AppendValues(Record.ToArray());
                    CurrentPath = Store.GetPath(CurrentIter);

                    if (DocumentPointerItem != null || SelectPointerItem != null)
                    {
                        string UidSelect = SelectPointerItem != null ? SelectPointerItem.UnigueID.ToString() : DocumentPointerItem!.UnigueID.ToString();

                        if (Record.ID == UidSelect)
                            SelectPath = CurrentPath;
                    }
                }
            }
        }
    }
	    </xsl:for-each>
    #endregion
    </xsl:for-each>
}

namespace <xsl:value-of select="Configuration/NameSpace"/>.РегістриВідомостей.ТабличніСписки
{
    <xsl:for-each select="Configuration/RegistersInformation/RegisterInformation">
      <xsl:variable name="RegisterName" select="Name"/>
    #region REGISTER "<xsl:value-of select="$RegisterName"/>"
    
      <xsl:for-each select="TabularLists/TabularList">
        <xsl:variable name="TabularListName" select="Name"/>
    public class <xsl:value-of select="$RegisterName"/>_<xsl:value-of select="$TabularListName"/>
    {
        string Image = "images/doc.png";
        string ID = "";
        string Період = "";
        <xsl:for-each select="Fields/Field">
        string <xsl:value-of select="Name"/> = "";</xsl:for-each>

        Array ToArray()
        {
            return new object[] { new Gdk.Pixbuf(Image), ID, Період
            /* */ <xsl:for-each select="Fields/Field">
              <xsl:text>, </xsl:text>
              <xsl:value-of select="Name"/>
            </xsl:for-each> };
        }

        public static ListStore Store = new ListStore(typeof(Gdk.Pixbuf) /* Image */, typeof(string) /* ID */, typeof(string) /* Період */
            <xsl:for-each select="Fields/Field">
              <xsl:text>, typeof(string)</xsl:text> /* <xsl:value-of select="Name"/> */
            </xsl:for-each>);

        public static void AddColumns(TreeView treeView)
        {
            treeView.AppendColumn(new TreeViewColumn("", new CellRendererPixbuf(), "pixbuf", 0)); /* { Ypad = 0 } */
            treeView.AppendColumn(new TreeViewColumn("ID", new CellRendererText(), "text", 1) { Visible = false });
            treeView.AppendColumn(new TreeViewColumn("Період", new CellRendererText(), "text", 2));
            /* */
            <xsl:for-each select="Fields/Field">
              <xsl:text>treeView.AppendColumn(new TreeViewColumn("</xsl:text>
              <xsl:value-of select="normalize-space(Caption)"/>
              <xsl:text>", new CellRendererText() { Xpad = 4 }, "text", </xsl:text>
              <xsl:value-of select="position() + 2"/>
              <xsl:text>) { SortColumnId = </xsl:text>
              <xsl:value-of select="position() + 2"/>
              <xsl:if test="Size != '0'">
                <xsl:text>, FixedWidth = </xsl:text>
                <xsl:value-of select="Size"/>
              </xsl:if>
              <xsl:text> } )</xsl:text>; /*<xsl:value-of select="Name"/>*/
            </xsl:for-each>
            //Пустишка
            treeView.AppendColumn(new TreeViewColumn());
        }

        public static List&lt;Where&gt; Where { get; set; } = new List&lt;Where&gt;();

        public static TreePath? SelectPath;
        public static TreePath? CurrentPath;

        public static void LoadRecords()
        {
            Store.Clear();

            РегістриВідомостей.<xsl:value-of select="$RegisterName"/>_RecordsSet <xsl:value-of select="$RegisterName"/>_RecordsSet = new РегістриВідомостей.<xsl:value-of select="$RegisterName"/>_RecordsSet();

            /* Where */
            <xsl:value-of select="$RegisterName"/>_RecordsSet.QuerySelect.Where = Where;

            /* DEFAULT ORDER */
            <xsl:value-of select="$RegisterName"/>_RecordsSet.QuerySelect.Order.Add("period", SelectOrder.ASC);

            <xsl:for-each select="Fields/Field[SortField = 'True' and Type != 'pointer']">
              /* ORDER */
              <xsl:value-of select="$RegisterName"/>_RecordsSet.QuerySelect.Order.Add(РегістриВідомостей.<xsl:value-of select="$RegisterName"/>_Const.<xsl:value-of select="Name"/>, SelectOrder.ASC);
            </xsl:for-each>

            <xsl:for-each select="Fields/Field[Type = 'pointer']">
                /* Join Table */
                <xsl:value-of select="$RegisterName"/>_RecordsSet.QuerySelect.Joins.Add(
                    new Join(<xsl:value-of select="Join/table"/>, РегістриВідомостей.<xsl:value-of select="$RegisterName"/>_Const.<xsl:value-of select="Join/field"/>, <xsl:value-of select="$RegisterName"/>_RecordsSet.QuerySelect.Table, "<xsl:value-of select="Join/alias"/>"));
                <xsl:for-each select="FieldAndAlias">
                  /* Field */
                  <xsl:value-of select="$RegisterName"/>_RecordsSet.QuerySelect.FieldAndAlias.Add(
                    new NameValue&lt;string&gt;("<xsl:value-of select="table"/>." + <xsl:value-of select="field"/>, "<xsl:value-of select="table"/>_field_<xsl:value-of select="position()"/>"));
                  <xsl:if test="../SortField = 'True'">
                    /* ORDER */
                    <xsl:value-of select="$RegisterName"/>_RecordsSet.QuerySelect.Order.Add("<xsl:value-of select="table"/>_field_<xsl:value-of select="position()"/>", SelectOrder.ASC);
                  </xsl:if>
                </xsl:for-each>
            </xsl:for-each>

            /* Read */
            <xsl:value-of select="$RegisterName"/>_RecordsSet.Read();
            foreach (<xsl:value-of select="$RegisterName"/>_RecordsSet.Record record in <xsl:value-of select="$RegisterName"/>_RecordsSet.Records)
            {
                <xsl:value-of select="$RegisterName"/>_<xsl:value-of select="$TabularListName"/> Record = new <xsl:value-of select="$RegisterName"/>_<xsl:value-of select="$TabularListName"/>
                {
                    ID = record.UID.ToString(),
                    Період = record.Period.ToString(),
                    <xsl:variable name="CountPointer" select="count(Fields/Field[Type = 'pointer'])"/>
                    <xsl:variable name="CountNotPointer" select="count(Fields/Field[Type != 'pointer'])"/>
                    <xsl:for-each select="Fields/Field[Type = 'pointer']">
                      <xsl:value-of select="Name"/>
                      <xsl:text> = </xsl:text>
                      <xsl:variable name="CountAlias" select="count(FieldAndAlias)"/>
                      <xsl:for-each select="FieldAndAlias">
                        <xsl:if test="position() &gt; 1"> + " " + </xsl:if>
                        <xsl:value-of select="$RegisterName"/>_RecordsSet.JoinValue[record.UID.ToString()]["<xsl:value-of select="table"/>_field_<xsl:value-of select="position()"/><xsl:text>"].ToString()</xsl:text>
                        <xsl:if test="$CountAlias = 1"> ?? ""</xsl:if>
                      </xsl:for-each>
                      <xsl:if test="$CountNotPointer != 0 or position() != $CountPointer">,</xsl:if> /**/
                    </xsl:for-each>
                    <xsl:for-each select="Fields/Field[Type != 'pointer']">
                      <xsl:value-of select="Name"/>
                      <xsl:text> = </xsl:text>
                      <xsl:choose>
                        <xsl:when test="Type = 'enum'">
                          <xsl:variable name="namePointer" select="substring-after(Pointer, '.')" />
                          <xsl:text>Перелічення.ПсевдонімиПерелічення.</xsl:text>
                          <xsl:value-of select="$namePointer"/>
                          <xsl:text>_Alias( </xsl:text>
                          <xsl:text>((</xsl:text>
                          <xsl:value-of select="Pointer"/>
                          <xsl:text>)</xsl:text>
                          <xsl:text>(record.</xsl:text>
                          <xsl:value-of select="Name"/>
                          <xsl:text> != DBNull.Value ? record.</xsl:text>
                          <xsl:value-of select="Name"/>
                          <xsl:text> : 0)) )</xsl:text>
                        </xsl:when>
                        <xsl:otherwise>
                          <xsl:text>record.</xsl:text>
                          <xsl:value-of select="Name"/>
                          <xsl:text>.ToString() ?? ""</xsl:text>
                        </xsl:otherwise>
                      </xsl:choose>
                      <xsl:if test="position() != $CountNotPointer">,</xsl:if> /**/
                    </xsl:for-each>
                };

                TreeIter CurrentIter = Store.AppendValues(Record.ToArray());
                CurrentPath = Store.GetPath(CurrentIter);
            }
        }
    }
	    </xsl:for-each>
    #endregion
    </xsl:for-each>
}

  </xsl:template>
</xsl:stylesheet>