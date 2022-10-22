<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="text" indent="yes" />

  <xsl:template match="/">

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
        public string Image = "doc.png";
        public string ID = "";
        <xsl:for-each select="Fields/Field">
        public string <xsl:value-of select="Name"/> = "";</xsl:for-each>

        public Array ToArray()
        {
            return new object[] { new Gdk.Pixbuf(Image), ID /* */<xsl:for-each select="Fields/Field">
              <xsl:text>, </xsl:text>
              <xsl:value-of select="Name"/>
            </xsl:for-each> };
        }

        public static ListStore Store = new ListStore(typeof(Gdk.Pixbuf), typeof(string) /* */<xsl:for-each select="Fields/Field">
              <xsl:text>, typeof(string)</xsl:text>
            </xsl:for-each>);

        public static void AddColumns(TreeView treeView)
        {
            treeView.AppendColumn(new TreeViewColumn("", new CellRendererPixbuf(), "pixbuf", 0));
            treeView.AppendColumn(new TreeViewColumn("ID", new CellRendererText(), "text", 1) { Visible = false });
            /* */
            <xsl:for-each select="Fields/Field">
              <xsl:text>treeView.AppendColumn(new TreeViewColumn("</xsl:text>
              <xsl:value-of select="Name"/>
              <xsl:text>", new CellRendererText(), "text", </xsl:text>
              <xsl:value-of select="position() + 1"/>
              <xsl:text>))</xsl:text>;
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
                      <xsl:if test="position() != 1">
                        <xsl:text>, </xsl:text>
                      </xsl:if>
                      <xsl:text>Довідники.</xsl:text>
                      <xsl:value-of select="$DirectoryName"/>
                      <xsl:text>_Const.</xsl:text>
                      <xsl:value-of select="Name"/>
                    </xsl:for-each>
                });

            <xsl:value-of select="$DirectoryName"/>_Select.Select();
            while (<xsl:value-of select="$DirectoryName"/>_Select.MoveNext())
            {
                Довідники.<xsl:value-of select="$DirectoryName"/>_Pointer? cur = <xsl:value-of select="$DirectoryName"/>_Select.Current;

                if (cur != null)
                    Store.AppendValues(new <xsl:value-of select="$DirectoryName"/>_<xsl:value-of select="$TabularListName"/>
                    {
                        ID = cur.UnigueID.ToString(),
                        <xsl:variable name="CountFields" select="count(Fields/Field)"/>
                        <xsl:for-each select="Fields/Field">
                          <xsl:value-of select="Name"/>
                          <xsl:text> = cur.Fields?[Довідники.</xsl:text>
                          <xsl:value-of select="$DirectoryName"/>
                          <xsl:text>_Const.</xsl:text>
                          <xsl:value-of select="Name"/>
                          <xsl:text>]?.ToString() ?? ""</xsl:text>
                          <xsl:if test="position() &lt; $CountFields">,</xsl:if> //
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