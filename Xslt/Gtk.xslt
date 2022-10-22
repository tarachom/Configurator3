<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="text" indent="yes" />

  <xsl:template match="/">

using Gtk;
using AccountingSoftware;

namespace <xsl:value-of select="Configuration/NameSpace"/>.Довідники
{
    <xsl:for-each select="Configuration/Directories/Directory">
      <xsl:variable name="DirectoryName" select="Name"/>
    #region DIRECTORY "<xsl:value-of select="$DirectoryName"/>"
    
    #endregion
    </xsl:for-each>
}

  </xsl:template>
</xsl:stylesheet>