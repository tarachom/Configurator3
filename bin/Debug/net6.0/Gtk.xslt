<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt">
  <xsl:output method="text" indent="yes" />

  <xsl:template match="/">

using Gtk;
using AccountingSoftware;

namespace <xsl:value-of select="Configuration/NameSpace"/>.Довідники.ТабличніСписки
{
    <xsl:for-each select="Configuration/Directories/Directory">
      <xsl:variable name="DirectoryCurrent" select="."/>
      <xsl:variable name="DirectoryName" select="Name"/>
    #region DIRECTORY "<xsl:value-of select="$DirectoryName"/>"
    
      <xsl:for-each select="TabularLists/TabularList">
        <xsl:variable name="TabularListName" select="Name"/>
        <xsl:variable name="CountFields" select="count(Fields/Field)"/>
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
            treeView.AppendColumn(new TreeViewColumn("", new CellRendererPixbuf(), "pixbuf", 0));
            treeView.AppendColumn(new TreeViewColumn("ID", new CellRendererText(), "text", 1) { Visible = false });
            /* */
            <xsl:for-each select="Fields/Field">
              <xsl:text>treeView.AppendColumn(new TreeViewColumn("</xsl:text>
              <xsl:value-of select="normalize-space(Caption)"/>
              <xsl:text>", new CellRendererText(), "text", </xsl:text>
              <xsl:value-of select="position() + 1"/>
              <xsl:text>) { SortColumnId = </xsl:text>
              <xsl:value-of select="position() + 1"/>
              <xsl:if test="Size != '0'">
                <xsl:text>, FixedWidth = </xsl:text>
                <xsl:value-of select="Size"/>
              </xsl:if>
              <xsl:text> } )</xsl:text>;
            </xsl:for-each>
        }

        public static void LoadRecords()
        {
            Store.Clear();

            Довідники.<xsl:value-of select="$DirectoryName"/>_Select <xsl:value-of select="$DirectoryName"/>_Select = new Довідники.<xsl:value-of select="$DirectoryName"/>_Select();
            <xsl:value-of select="$DirectoryName"/>_Select.QuerySelect.Field.AddRange(
                new string[]
                {
                    <xsl:for-each select="Fields/Field">
                      <xsl:variable name="FieldName" select="Name"/>
                      <xsl:variable name="DirectoryCurrentField" select="$DirectoryCurrent/Fields/Field[Name = $FieldName]"/>

                      <xsl:choose>
                        <xsl:when test="$DirectoryCurrentField/Type != 'pointer'">
                          <xsl:if test="position() &gt; 1">, </xsl:if>
                          <xsl:text>Довідники.</xsl:text>
                          <xsl:value-of select="$DirectoryName"/>
                          <xsl:text>_Const.</xsl:text>
                          <xsl:value-of select="Name"/>
                        </xsl:when>
                        <xsl:otherwise>
                          <xsl:text>/* </xsl:text>
                          <xsl:if test="position() &gt; 1">, </xsl:if>
                          <xsl:text>Довідники.</xsl:text>
                          <xsl:value-of select="$DirectoryName"/>
                          <xsl:text>_Const.</xsl:text>
                          <xsl:value-of select="Name"/>
                          <xsl:text> */</xsl:text>
                        </xsl:otherwise>
                      </xsl:choose> // [ pos = <xsl:value-of select="position()"/>, type = <xsl:value-of select="$DirectoryCurrentField/Type"/> ]
                    </xsl:for-each>
                });

            <xsl:for-each select="Fields/Field">
              <xsl:variable name="FieldName" select="Name"/>
              <xsl:variable name="DirectoryCurrentField" select="$DirectoryCurrent/Fields/Field[Name = $FieldName]"/>
              <xsl:choose>
                <xsl:when test="$DirectoryCurrentField/Type = 'pointer'">
                  <xsl:variable name="groupPointer" select="substring-before($DirectoryCurrentField/Pointer, '.')" />
                  <xsl:variable name="namePointer" select="substring-after($DirectoryCurrentField/Pointer, '.')" />
                  <xsl:variable name="PointerField">
                    <xsl:choose>
                      <xsl:when test="$groupPointer = 'Довідники'">
                        <xsl:variable name="CurrPointer" select="/Configuration/Directories/Directory[Name = $namePointer]" />
                        <xsl:choose>
                          <xsl:when test="count($CurrPointer/Fields/Field[IsPresentation = '1']) != 0">
                            <xsl:value-of select="$CurrPointer/Fields/Field[IsPresentation = '1']/Name" />
                          </xsl:when>
                          <xsl:otherwise>
                            <xsl:value-of select="$CurrPointer/Fields/Field/Name" />
                          </xsl:otherwise>
                        </xsl:choose>
                      </xsl:when>
                      <xsl:when test="$groupPointer = 'Документи'">
                        <xsl:variable name="CurrPointer" select="/Configuration/Documents/Document[Name = $namePointer]" />
                        <xsl:choose>
                          <xsl:when test="count($CurrPointer/Fields/Field[IsPresentation = '1']) != 0">
                            <xsl:value-of select="$CurrPointer/Fields/Field[IsPresentation = '1']/Name" />
                          </xsl:when>
                          <xsl:otherwise>
                            <xsl:value-of select="$CurrPointer/Fields/Field/Name" />
                          </xsl:otherwise>
                        </xsl:choose>
                      </xsl:when>
                    </xsl:choose>
                  </xsl:variable>
                  /* JOIN <xsl:value-of select="position()"/> */
                  <xsl:value-of select="$DirectoryName"/>_Select.QuerySelect.FieldAndAlias.Add(
                    new NameValue&lt;string&gt;(
                      <xsl:value-of select="$DirectoryCurrentField/Pointer"/>_Const.TABLE + "." + <xsl:value-of select="$DirectoryCurrentField/Pointer"/>_Const.<xsl:value-of select="$PointerField"/>, "join_<xsl:value-of select="position()"/>"));
                  <xsl:value-of select="$DirectoryName"/>_Select.QuerySelect.Joins.Add(
                    new Join(<xsl:value-of select="$DirectoryCurrentField/Pointer"/>_Const.TABLE, Довідники.<xsl:value-of select="$DirectoryName"/>_Const.<xsl:value-of select="$FieldName"/>, <xsl:value-of select="$DirectoryName"/>_Select.QuerySelect.Table));
                </xsl:when>
              </xsl:choose>
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
                        <xsl:for-each select="Fields/Field">
                          <xsl:variable name="FieldName" select="Name"/>
                          <xsl:variable name="DirectoryCurrentField" select="$DirectoryCurrent/Fields/Field[Name = $FieldName]"/>

                          <xsl:value-of select="Name"/>
                          <xsl:text> = </xsl:text>

                          <xsl:choose>
                            <xsl:when test="$DirectoryCurrentField/Type = 'pointer'">
                              <xsl:text>cur.Fields?["join_</xsl:text>
                              <xsl:value-of select="position()"/>
                              <xsl:text>"]?.ToString() ?? ""</xsl:text>
                            </xsl:when>
                            <xsl:when test="$DirectoryCurrentField/Type = 'enum'">
                              <xsl:text>((</xsl:text>
                              <xsl:value-of select="$DirectoryCurrentField/Pointer"/>
                              <xsl:text>)</xsl:text>
                              <xsl:text>int.Parse(cur.Fields?[</xsl:text>
                              <xsl:value-of select="$DirectoryName"/>
                              <xsl:text>_Const.</xsl:text>
                              <xsl:value-of select="Name"/>
                              <xsl:text>]?.ToString() ?? "0")).ToString()</xsl:text>
                            </xsl:when>
                            <xsl:otherwise>
                              <xsl:text>cur.Fields?[</xsl:text>
                              <xsl:value-of select="$DirectoryName"/>
                              <xsl:text>_Const.</xsl:text>
                              <xsl:value-of select="Name"/>
                              <xsl:text>]?.ToString() ?? ""</xsl:text>
                            </xsl:otherwise>
                          </xsl:choose>
                          <xsl:if test="position() &lt; $CountFields">,</xsl:if> // [ pos = <xsl:value-of select="position()"/>, type = <xsl:value-of select="$DirectoryCurrentField/Type"/> ]
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
      <xsl:variable name="DocumentCurrent" select="."/>
      <xsl:variable name="DocumentName" select="Name"/>
    #region DOCUMENT "<xsl:value-of select="$DocumentName"/>"
    
      <xsl:for-each select="TabularLists/TabularList">
        <xsl:variable name="TabularListName" select="Name"/>
        <xsl:variable name="CountFields" select="count(Fields/Field)"/>
    public class <xsl:value-of select="$DocumentName"/>_<xsl:value-of select="$TabularListName"/>
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
            treeView.AppendColumn(new TreeViewColumn("", new CellRendererPixbuf(), "pixbuf", 0));
            treeView.AppendColumn(new TreeViewColumn("ID", new CellRendererText(), "text", 1) { Visible = false });
            /* */
            <xsl:for-each select="Fields/Field">
              <xsl:text>treeView.AppendColumn(new TreeViewColumn("</xsl:text>
              <xsl:value-of select="normalize-space(Caption)"/>
              <xsl:text>", new CellRendererText(), "text", </xsl:text>
              <xsl:value-of select="position() + 1"/>
              <xsl:text>) { SortColumnId = </xsl:text>
              <xsl:value-of select="position() + 1"/>
              <xsl:if test="Size != '0'">
                <xsl:text>, FixedWidth = </xsl:text>
                <xsl:value-of select="Size"/>
              </xsl:if>
              <xsl:text> } )</xsl:text>;
            </xsl:for-each>
        }

        public static void LoadRecords()
        {
            Store.Clear();

            Документи.<xsl:value-of select="$DocumentName"/>_Select <xsl:value-of select="$DocumentName"/>_Select = new Документи.<xsl:value-of select="$DocumentName"/>_Select();
            <xsl:value-of select="$DocumentName"/>_Select.QuerySelect.Field.AddRange(
                new string[]
                {
                    <xsl:for-each select="Fields/Field">
                      <xsl:variable name="FieldName" select="Name"/>
                      <xsl:variable name="DocumentCurrentField" select="$DocumentCurrent/Fields/Field[Name = $FieldName]"/>

                      <xsl:choose>
                        <xsl:when test="$DocumentCurrentField/Type != 'pointer'">
                          <xsl:if test="position() &gt; 1">, </xsl:if>
                          <xsl:text>Документи.</xsl:text>
                          <xsl:value-of select="$DocumentName"/>
                          <xsl:text>_Const.</xsl:text>
                          <xsl:value-of select="Name"/>
                        </xsl:when>
                        <xsl:otherwise>
                          <xsl:text>/* </xsl:text>
                          <xsl:if test="position() &gt; 1">, </xsl:if>
                          <xsl:text>Документи.</xsl:text>
                          <xsl:value-of select="$DocumentName"/>
                          <xsl:text>_Const.</xsl:text>
                          <xsl:value-of select="Name"/>
                          <xsl:text> */</xsl:text>
                        </xsl:otherwise>
                      </xsl:choose> // [ pos = <xsl:value-of select="position()"/>, type = <xsl:value-of select="$DocumentCurrentField/Type"/> ]
                    </xsl:for-each>
                });

            <xsl:for-each select="Fields/Field">
              <xsl:variable name="FieldName" select="Name"/>
              <xsl:variable name="DocumentCurrentField" select="$DocumentCurrent/Fields/Field[Name = $FieldName]"/>
              <xsl:choose>
                <xsl:when test="$DocumentCurrentField/Type = 'pointer'">
                  <xsl:variable name="groupPointer" select="substring-before($DocumentCurrentField/Pointer, '.')" />
                  <xsl:variable name="namePointer" select="substring-after($DocumentCurrentField/Pointer, '.')" />
                  <xsl:variable name="PointerField">
                    <xsl:choose>
                      <xsl:when test="$groupPointer = 'Довідники'">
                        <xsl:variable name="CurrPointer" select="/Configuration/Directories/Directory[Name = $namePointer]" />
                        <xsl:choose>
                          <xsl:when test="count($CurrPointer/Fields/Field[IsPresentation = '1']) != 0">
                            <xsl:value-of select="$CurrPointer/Fields/Field[IsPresentation = '1']/Name" />
                          </xsl:when>
                          <xsl:otherwise>
                            <xsl:value-of select="$CurrPointer/Fields/Field/Name" />
                          </xsl:otherwise>
                        </xsl:choose>
                      </xsl:when>
                      <xsl:when test="$groupPointer = 'Документи'">
                        <xsl:variable name="CurrPointer" select="/Configuration/Documents/Document[Name = $namePointer]" />
                        <xsl:choose>
                          <xsl:when test="count($CurrPointer/Fields/Field[IsPresentation = '1']) != 0">
                            <xsl:value-of select="$CurrPointer/Fields/Field[IsPresentation = '1']/Name" />
                          </xsl:when>
                          <xsl:otherwise>
                            <xsl:value-of select="$CurrPointer/Fields/Field/Name" />
                          </xsl:otherwise>
                        </xsl:choose>
                      </xsl:when>
                    </xsl:choose>
                  </xsl:variable>
                  /* JOIN <xsl:value-of select="position()"/> */
                  <xsl:value-of select="$DocumentName"/>_Select.QuerySelect.FieldAndAlias.Add(
                    new NameValue&lt;string&gt;(
                      <xsl:value-of select="$DocumentCurrentField/Pointer"/>_Const.TABLE + "." + <xsl:value-of select="$DocumentCurrentField/Pointer"/>_Const.<xsl:value-of select="$PointerField"/>, "join_<xsl:value-of select="position()"/>"));
                  <xsl:value-of select="$DocumentName"/>_Select.QuerySelect.Joins.Add(
                    new Join(<xsl:value-of select="$DocumentCurrentField/Pointer"/>_Const.TABLE, Довідники.<xsl:value-of select="$DocumentName"/>_Const.<xsl:value-of select="$FieldName"/>, <xsl:value-of select="$DocumentName"/>_Select.QuerySelect.Table));
                </xsl:when>
              </xsl:choose>
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
                        <xsl:for-each select="Fields/Field">
                          <xsl:variable name="FieldName" select="Name"/>
                          <xsl:variable name="DocumentCurrentField" select="$DocumentCurrent/Fields/Field[Name = $FieldName]"/>

                          <xsl:value-of select="Name"/>
                          <xsl:text> = </xsl:text>

                          <xsl:choose>
                            <xsl:when test="$DocumentCurrentField/Type = 'pointer'">
                              <xsl:text>cur.Fields?["join_</xsl:text>
                              <xsl:value-of select="position()"/>
                              <xsl:text>"]?.ToString() ?? ""</xsl:text>
                            </xsl:when>
                            <xsl:when test="$DocumentCurrentField/Type = 'enum'">
                              <xsl:text>((</xsl:text>
                              <xsl:value-of select="$DocumentCurrentField/Pointer"/>
                              <xsl:text>)</xsl:text>
                              <xsl:text>int.Parse(cur.Fields?[</xsl:text>
                              <xsl:value-of select="$DocumentName"/>
                              <xsl:text>_Const.</xsl:text>
                              <xsl:value-of select="Name"/>
                              <xsl:text>]?.ToString() ?? "0")).ToString()</xsl:text>
                            </xsl:when>
                            <xsl:otherwise>
                              <xsl:text>cur.Fields?[</xsl:text>
                              <xsl:value-of select="$DocumentName"/>
                              <xsl:text>_Const.</xsl:text>
                              <xsl:value-of select="Name"/>
                              <xsl:text>]?.ToString() ?? ""</xsl:text>
                            </xsl:otherwise>
                          </xsl:choose>
                          <xsl:if test="position() &lt; $CountFields">,</xsl:if> // [ pos = <xsl:value-of select="position()"/>, type = <xsl:value-of select="$DocumentCurrentField/Type"/> ]
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