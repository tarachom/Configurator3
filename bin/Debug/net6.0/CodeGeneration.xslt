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
  
  <!-- Для задання типу поля -->
  <xsl:template name="FieldType">
    <xsl:choose>
      <xsl:when test="Type = 'string'">
        <xsl:text>string</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'string[]'">
        <xsl:text>string[]</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'integer'">
        <xsl:text>int</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'integer[]'">
        <xsl:text>int[]</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'numeric'">
        <xsl:text>decimal</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'numeric[]'">
        <xsl:text>decimal[]</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'boolean'">
        <xsl:text>bool</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'time'">
        <xsl:text>TimeSpan</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'date' or Type = 'datetime'">
        <xsl:text>DateTime</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'pointer'">
        <xsl:value-of select="Pointer"/>
        <xsl:text>_Pointer</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'empty_pointer'">
        <xsl:text>EmptyPointer</xsl:text>
      </xsl:when>
	  <xsl:when test="Type = 'any_pointer'">
        <xsl:text>Guid</xsl:text>
      </xsl:when>
	  <xsl:when test="Type = 'composite_pointer'">
        <xsl:text>UuidAndText</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'enum'">
        <xsl:value-of select="Pointer"/>
      </xsl:when>
	  <xsl:when test="Type = 'bytea'">
	    <xsl:text>byte[]</xsl:text>
      </xsl:when>
    </xsl:choose>    
  </xsl:template>
  
  <!-- Для конструкторів. Значення поля по замовчуванню відповідно до типу -->
  <xsl:template name="DefaultFieldValue">
    <xsl:choose>
      <xsl:when test="Type = 'string'">
        <xsl:text>""</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'string[]'">
        <xsl:text>new string[] { }</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'integer'">
        <xsl:text>0</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'integer[]'">
        <xsl:text>new int[] { }</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'numeric'">
        <xsl:text>0</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'numeric[]'">
        <xsl:text>new decimal[] { }</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'boolean'">
        <xsl:text>false</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'time'">
        <xsl:text>DateTime.MinValue.TimeOfDay</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'date' or Type = 'datetime'">
        <xsl:text>DateTime.MinValue</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'pointer'">
        <xsl:text>new </xsl:text>
        <xsl:value-of select="Pointer"/>
        <xsl:text>_Pointer()</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'empty_pointer'">
        <xsl:text>new EmptyPointer()</xsl:text>
      </xsl:when>
	  <xsl:when test="Type = 'any_pointer'">
        <xsl:text>new Guid()</xsl:text>
      </xsl:when>
	  <xsl:when test="Type = 'composite_pointer'">
        <xsl:text>new UuidAndText()</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'enum'">
        <xsl:text>0</xsl:text>
      </xsl:when>
	  <xsl:when test="Type = 'bytea'">
	    <xsl:text>new byte[] { }</xsl:text>
      </xsl:when>
    </xsl:choose>
  </xsl:template>

  <!-- Для параметрів функцій. Значення параметра по замовчуванню відповідно до типу -->
  <xsl:template name="DefaultParamValue">
    <xsl:choose>
      <xsl:when test="Type = 'string'">
        <xsl:text>""</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'string[]'">
        <xsl:text>null</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'integer'">
        <xsl:text>0</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'integer[]'">
        <xsl:text>null</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'numeric'">
        <xsl:text>0</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'numeric[]'">
        <xsl:text>null</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'boolean'">
        <xsl:text>false</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'time'">
        <xsl:text>null</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'date' or Type = 'datetime'">
        <xsl:text>null</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'pointer'">
        <xsl:text>null</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'empty_pointer'">
        <xsl:text>null</xsl:text>
      </xsl:when>
	  <xsl:when test="Type = 'any_pointer'">
        <xsl:text>Guid.Empty</xsl:text>
      </xsl:when>
	  <xsl:when test="Type = 'composite_pointer'">
        <xsl:text>null</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'enum'">
        <xsl:text>0</xsl:text>
      </xsl:when>
	  <xsl:when test="Type = 'bytea'">
        <xsl:text>null</xsl:text>
      </xsl:when>
    </xsl:choose>
  </xsl:template>
  
  <!-- Для присвоєння значення полям -->
  <xsl:template name="ReadFieldValue">
     <xsl:param name="BaseFieldContainer" />
     
     <xsl:choose>
        <xsl:when test="Type = 'string'">
          <xsl:value-of select="$BaseFieldContainer"/><xsl:text>["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"].ToString()</xsl:text>
        </xsl:when>
        <xsl:when test="Type = 'string[]'">
          <xsl:text>(</xsl:text><xsl:value-of select="$BaseFieldContainer"/><xsl:text>["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"] != DBNull.Value) ? </xsl:text>
          <xsl:text>(string[])</xsl:text><xsl:value-of select="$BaseFieldContainer"/><xsl:text>["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"]</xsl:text>
          <xsl:text> : new string[] { }</xsl:text>
        </xsl:when>
       <xsl:when test="Type = 'integer'">
          <xsl:text>(</xsl:text><xsl:value-of select="$BaseFieldContainer"/><xsl:text>["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"] != DBNull.Value) ? </xsl:text>
          <xsl:text>(int)</xsl:text><xsl:value-of select="$BaseFieldContainer"/><xsl:text>["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"]</xsl:text>
          <xsl:text> : 0</xsl:text>
        </xsl:when>
        <xsl:when test="Type = 'integer[]'">
          <xsl:text>(</xsl:text><xsl:value-of select="$BaseFieldContainer"/><xsl:text>["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"] != DBNull.Value) ? </xsl:text>
          <xsl:text>(int[])</xsl:text><xsl:value-of select="$BaseFieldContainer"/><xsl:text>["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"]</xsl:text>
          <xsl:text> : new int[] { }</xsl:text>
        </xsl:when>
        <xsl:when test="Type = 'numeric'">
          <xsl:text>(</xsl:text><xsl:value-of select="$BaseFieldContainer"/><xsl:text>["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"] != DBNull.Value) ? </xsl:text>
          <xsl:text>(decimal)</xsl:text><xsl:value-of select="$BaseFieldContainer"/><xsl:text>["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"]</xsl:text>
          <xsl:text> : 0</xsl:text>
        </xsl:when>
        <xsl:when test="Type = 'numeric[]'">
          <xsl:text>(</xsl:text><xsl:value-of select="$BaseFieldContainer"/><xsl:text>["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"] != DBNull.Value) ? </xsl:text>
          <xsl:text>(decimal[])</xsl:text><xsl:value-of select="$BaseFieldContainer"/><xsl:text>["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"]</xsl:text>
          <xsl:text> : new decimal[] { }</xsl:text>
        </xsl:when>
        <xsl:when test="Type = 'boolean'">
          <xsl:text>(</xsl:text><xsl:value-of select="$BaseFieldContainer"/><xsl:text>["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"] != DBNull.Value) ? </xsl:text>
          <xsl:text>bool.Parse(</xsl:text><xsl:value-of select="$BaseFieldContainer"/><xsl:text>["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"].ToString())</xsl:text>
          <xsl:text> : false</xsl:text>
        </xsl:when>
        <xsl:when test="Type = 'time'">
          <xsl:text>(</xsl:text><xsl:value-of select="$BaseFieldContainer"/><xsl:text>["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"] != DBNull.Value) ? </xsl:text>
          <xsl:text>TimeSpan.Parse(</xsl:text><xsl:value-of select="$BaseFieldContainer"/><xsl:text>["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"].ToString())</xsl:text>
          <xsl:text> : DateTime.MinValue.TimeOfDay</xsl:text>
        </xsl:when>
        <xsl:when test="Type = 'date' or Type = 'datetime'">
          <xsl:text>(</xsl:text><xsl:value-of select="$BaseFieldContainer"/><xsl:text>["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"] != DBNull.Value) ? </xsl:text>
          <xsl:text>DateTime.Parse(</xsl:text><xsl:value-of select="$BaseFieldContainer"/><xsl:text>["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"].ToString())</xsl:text>
          <xsl:text> : DateTime.MinValue</xsl:text>
        </xsl:when>
        <xsl:when test="Type = 'pointer'">
          <xsl:text>new </xsl:text><xsl:value-of select="Pointer"/>
          <xsl:text>_Pointer(</xsl:text><xsl:value-of select="$BaseFieldContainer"/><xsl:text>["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"])</xsl:text>
        </xsl:when>
        <xsl:when test="Type = 'empty_pointer'">
          <xsl:text>new EmptyPointer()</xsl:text>
        </xsl:when>
		<xsl:when test="Type = 'any_pointer'">
		  <xsl:text>(</xsl:text><xsl:value-of select="$BaseFieldContainer"/><xsl:text>["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"] != DBNull.Value) ? </xsl:text>
          <xsl:text>Guid.Parse(</xsl:text><xsl:value-of select="$BaseFieldContainer"/><xsl:text>["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"].ToString())</xsl:text>
          <xsl:text> : Guid.Empty</xsl:text>
        </xsl:when>
		<xsl:when test="Type = 'composite_pointer'">
		  <xsl:text>(</xsl:text><xsl:value-of select="$BaseFieldContainer"/><xsl:text>["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"] != DBNull.Value) ? </xsl:text>
          <xsl:text>(UuidAndText)</xsl:text><xsl:value-of select="$BaseFieldContainer"/><xsl:text>["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"]</xsl:text>
          <xsl:text> : new UuidAndText()</xsl:text>
        </xsl:when>
        <xsl:when test="Type = 'enum'">
          <xsl:text>(</xsl:text><xsl:value-of select="$BaseFieldContainer"/><xsl:text>["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"] != DBNull.Value) ? </xsl:text>
          <xsl:text>(</xsl:text><xsl:value-of select="Pointer"/><xsl:text>)</xsl:text>
          <xsl:value-of select="$BaseFieldContainer"/><xsl:text>["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"]</xsl:text>
          <xsl:text> : 0</xsl:text>
        </xsl:when>
		<xsl:when test="Type = 'bytea'">
          <xsl:text>(</xsl:text><xsl:value-of select="$BaseFieldContainer"/><xsl:text>["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"] != DBNull.Value) ? </xsl:text>
          <xsl:text>(byte[])</xsl:text><xsl:value-of select="$BaseFieldContainer"/><xsl:text>["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"]</xsl:text>
          <xsl:text> : new byte[] { }</xsl:text>
        </xsl:when>
     </xsl:choose>
  </xsl:template>

  <!-- Для перетворення поля в ХМЛ стрічку -->
  <xsl:template name="SerializeFieldValue">
    <xsl:text>"&lt;</xsl:text><xsl:value-of select="Name"/><xsl:text>&gt;" + </xsl:text>
    <xsl:choose>
      <xsl:when test="Type = 'enum'">
        <xsl:text>((int)</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'boolean'">
        <xsl:text>(</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'string'">
        <xsl:text>"&lt;![CDATA[" + </xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'string[]'">
        <xsl:text>ArrayToXml&lt;string&gt;.Convert(</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'integer[]'">
        <xsl:text>ArrayToXml&lt;int&gt;.Convert(</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'numeric[]'">
        <xsl:text>ArrayToXml&lt;decimal&gt;.Convert(</xsl:text>
      </xsl:when>
    </xsl:choose>
    <xsl:value-of select="Name"/>
    <xsl:choose>
      <xsl:when test="Type = 'enum'">
        <xsl:text>).ToString()</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'boolean'">
        <xsl:text> == true ? "1" : "0")</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'integer' or Type = 'numeric' or 
                Type = 'date' or Type = 'datetime' or Type = 'time' or
                Type = 'pointer' or Type = 'empty_pointer' or Type = 'any_pointer' or Type = 'composite_pointer'">
        <xsl:text>.ToString()</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'string'">
        <xsl:text> + "]]&gt;"</xsl:text>
      </xsl:when>
      <xsl:when test="Type = 'string[]' or Type = 'integer[]' or Type = 'numeric[]'">
        <xsl:text>).ToString()</xsl:text>
      </xsl:when>
    </xsl:choose> 
    <xsl:text> + "&lt;/</xsl:text><xsl:value-of select="Name"/><xsl:text>&gt;" </xsl:text>
  </xsl:template>
  
  <!-- Документування коду -->
  <xsl:template name="CommentSummary">
    <xsl:variable name="normalize_space_Desc" select="normalize-space(Desc)" />
    <xsl:if test="$normalize_space_Desc != ''">
    <xsl:text>///&lt;summary</xsl:text>&gt;
    <xsl:text>///</xsl:text>
    <xsl:value-of select="$normalize_space_Desc"/>.
    <xsl:text>///&lt;/summary&gt;</xsl:text>
    </xsl:if>
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

using System;
using System.Collections.Generic;
using AccountingSoftware;

namespace <xsl:value-of select="Configuration/NameSpace"/>
{
    public static class Config
    {
        public static Kernel Kernel { get; set; }
        public static Kernel KernelBackgroundTask { get; set; }
        public static Kernel KernelParalelWork { get; set; }
		
        public static void ReadAllConstants()
        {
            <xsl:for-each select="Configuration/ConstantsBlocks/ConstantsBlock">
                   <xsl:text>Константи.</xsl:text><xsl:value-of select="Name"/>.ReadAll();
            </xsl:for-each>
        }
    }
}

namespace <xsl:value-of select="Configuration/NameSpace"/>.Константи
{
    <xsl:for-each select="Configuration/ConstantsBlocks/ConstantsBlock">
	#region CONSTANTS BLOCK "<xsl:value-of select="Name"/>"
    public static class <xsl:value-of select="Name"/>
    {
        public static void ReadAll()
        {
            <xsl:variable name="Constants" select="Constants/Constant" />
		    <xsl:if test="count($Constants) &gt; 0">
            Dictionary&lt;string, object&gt; fieldValue = new Dictionary&lt;string, object&gt;();
            bool IsSelect = Config.Kernel.DataBase.SelectAllConstants("tab_constants",
                 <xsl:text>new string[] { </xsl:text>
                 <xsl:for-each select="$Constants">
                   <xsl:if test="position() != 1">
                     <xsl:text>, </xsl:text>
                   </xsl:if>
                   <xsl:text>"</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"</xsl:text>
                 </xsl:for-each> }, fieldValue);
            
            if (IsSelect)
            {
                <xsl:for-each select="$Constants">
                  <xsl:text>m_</xsl:text>
                  <xsl:value-of select="Name"/>
                  <xsl:text>_Const = </xsl:text>
                  <xsl:call-template name="ReadFieldValue">
                    <xsl:with-param name="BaseFieldContainer">fieldValue</xsl:with-param>
                  </xsl:call-template>;
                </xsl:for-each>
            }
			</xsl:if>
        }
        
        <xsl:for-each select="Constants/Constant">
        static <xsl:call-template name="FieldType" />
        <xsl:text> m_</xsl:text>
        <xsl:value-of select="Name"/>
        <xsl:text>_Const = </xsl:text>
        <xsl:call-template name="DefaultFieldValue" />;
        <xsl:text>public static </xsl:text>
        <xsl:call-template name="FieldType" />
        <xsl:text> </xsl:text>
        <xsl:value-of select="Name"/>_Const
        {
            get 
            {
                return m_<xsl:value-of select="Name"/><xsl:text>_Const</xsl:text>
                <xsl:if test="Type = 'pointer'">
					<xsl:variable name="groupPointer" select="substring-before(Pointer, '.')" />
					<xsl:choose>
						<xsl:when test="$groupPointer = 'Довідники'">
							<xsl:text>.GetNewDirectoryPointer()</xsl:text>
						</xsl:when>
						<xsl:when test="$groupPointer = 'Документи'">
							<xsl:text>.GetNewDocumentPointer()</xsl:text>
						</xsl:when>
					</xsl:choose>
				</xsl:if>;
            }
            set
            {
                m_<xsl:value-of select="Name"/>_Const = value;
                Config.Kernel.DataBase.SaveConstants("tab_constants", "<xsl:value-of select="NameInTable"/><xsl:text>", </xsl:text>
			    <xsl:choose>
					<xsl:when test="Type = 'enum'">
						<xsl:text>(int)</xsl:text>
					</xsl:when>
				</xsl:choose>
                <xsl:text>m_</xsl:text>
                <xsl:value-of select="Name"/>
                <xsl:text>_Const</xsl:text>
                <xsl:choose>
                  <xsl:when test="Type = 'pointer' or Type = 'empty_pointer'">
                    <xsl:text>.UnigueID.UGuid</xsl:text>
                  </xsl:when>
                </xsl:choose>);
            }
        }
        </xsl:for-each>

        <xsl:for-each select="Constants/Constant">
          <xsl:variable name="ConstantsName" select="Name"/>
          <xsl:for-each select="TabularParts/TablePart">
            <!-- TableParts -->
            <xsl:variable name="TablePartName" select="Name"/>
            <xsl:variable name="TablePartFullName" select="concat($ConstantsName, '_', $TablePartName)"/>
        
        <!--<xsl:call-template name="CommentSummary" />-->
        public class <xsl:value-of select="$TablePartFullName"/>_TablePart : ConstantsTablePart
        {
            public <xsl:value-of select="$TablePartFullName"/>_TablePart() : base(Config.Kernel, "<xsl:value-of select="Table"/>",
                 <xsl:text>new string[] { </xsl:text>
                 <xsl:for-each select="Fields/Field">
                   <xsl:if test="position() != 1">
                     <xsl:text>, </xsl:text>
                   </xsl:if>
                   <xsl:text>"</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"</xsl:text>
                 </xsl:for-each> }) 
            {
                Records = new List&lt;Record&gt;();
            }
            
            public const string TABLE = "<xsl:value-of select="Table"/>";
            <xsl:for-each select="Fields/Field">
            public const string <xsl:value-of select="Name"/> = "<xsl:value-of select="NameInTable"/>";</xsl:for-each>
            public List&lt;Record&gt; Records { get; set; }
        
            public void Read()
            {
                Records.Clear();
                base.BaseRead();

                foreach (Dictionary&lt;string, object&gt; fieldValue in base.FieldValueList) 
                {
                    Record record = new Record();
                    record.UID = (Guid)fieldValue["uid"];
                    
                    <xsl:for-each select="Fields/Field">
                      <xsl:text>record.</xsl:text>
                      <xsl:value-of select="Name"/>
                      <xsl:text> = </xsl:text>
                      <xsl:call-template name="ReadFieldValue">
                        <xsl:with-param name="BaseFieldContainer">fieldValue</xsl:with-param>
                      </xsl:call-template>;
                    </xsl:for-each>
                    Records.Add(record);
                }
            
                base.BaseClear();
            }
        
            public void Save(bool clear_all_before_save /*= true*/) 
            {
                base.BaseBeginTransaction();
                
                if (clear_all_before_save)
                    base.BaseDelete();

                foreach (Record record in Records)
                {
                    Dictionary&lt;string, object&gt; fieldValue = new Dictionary&lt;string, object&gt;();

                    <xsl:for-each select="Fields/Field">
                        <xsl:text>fieldValue.Add("</xsl:text>
                        <xsl:value-of select="NameInTable"/><xsl:text>", </xsl:text>
						<xsl:if test="Type = 'enum'">
						    <xsl:text>(int)</xsl:text>
					    </xsl:if>
						<xsl:text>record.</xsl:text><xsl:value-of select="Name"/>
                        <xsl:choose>
                        <xsl:when test="Type = 'pointer' or Type = 'empty_pointer'">
                            <xsl:text>.UnigueID.UGuid</xsl:text>
                        </xsl:when>
                        </xsl:choose>
                        <xsl:text>)</xsl:text>;
                    </xsl:for-each>
                    base.BaseSave(record.UID, fieldValue);
                }
                
                base.BaseCommitTransaction();
            }
        
            public void Delete()
            {
                base.BaseDelete();
            }
            
            public class Record : ConstantsTablePartRecord
            {
                public Record()
                {
                    <xsl:for-each select="Fields/Field">
                      <xsl:value-of select="Name"/>
                      <xsl:text> = </xsl:text>
                      <xsl:call-template name="DefaultFieldValue" />;
                    </xsl:for-each>
                }
                <xsl:for-each select="Fields/Field">
                  <xsl:text>public </xsl:text>
                  <xsl:call-template name="FieldType" />
                  <xsl:text> </xsl:text>
                  <xsl:value-of select="Name"/>
                  <xsl:text> { get; set; </xsl:text>}
                </xsl:for-each>
            }            
        }
          </xsl:for-each>
        </xsl:for-each>     
    }
    #endregion
    </xsl:for-each>
}

namespace <xsl:value-of select="Configuration/NameSpace"/>.Довідники
{
    <xsl:for-each select="Configuration/Directories/Directory">
      <xsl:variable name="DirectoryName" select="Name"/>
    #region DIRECTORY "<xsl:value-of select="$DirectoryName"/>"
    <!--<xsl:call-template name="CommentSummary" />-->
    public static class <xsl:value-of select="$DirectoryName"/>_Const
    {
        public const string TABLE = "<xsl:value-of select="Table"/>";
        <xsl:for-each select="Fields/Field">
        public const string <xsl:value-of select="Name"/> = "<xsl:value-of select="NameInTable"/>";</xsl:for-each>
    }
	
    <!--<xsl:call-template name="CommentSummary" />-->
    public class <xsl:value-of select="$DirectoryName"/>_Objest : DirectoryObject
    {
        public <xsl:value-of select="$DirectoryName"/>_Objest() : base(Config.Kernel, "<xsl:value-of select="Table"/>",
             <xsl:text>new string[] { </xsl:text>
             <xsl:for-each select="Fields/Field">
               <xsl:if test="position() != 1">
                 <xsl:text>, </xsl:text>
               </xsl:if>
               <xsl:text>"</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"</xsl:text>
             </xsl:for-each> }) 
        {
            <xsl:for-each select="Fields/Field">
              <xsl:value-of select="Name"/>
              <xsl:text> = </xsl:text>
              <xsl:call-template name="DefaultFieldValue" />;
            </xsl:for-each>
            <xsl:if test="count(TabularParts/TablePart) &gt; 0">
            //Табличні частини
            </xsl:if>
            <xsl:for-each select="TabularParts/TablePart">
                <xsl:variable name="TablePartName" select="concat(Name, '_TablePart')"/>
                <xsl:value-of select="$TablePartName"/><xsl:text> = new </xsl:text>
                <xsl:value-of select="concat($DirectoryName, '_', $TablePartName)"/><xsl:text>(this)</xsl:text>;
            </xsl:for-each>
        }
        
        public bool Read(UnigueID uid)
        {
            if (BaseRead(uid))
            {
                <xsl:for-each select="Fields/Field">
                  <xsl:value-of select="Name"/>
                  <xsl:text> = </xsl:text>
                  <xsl:call-template name="ReadFieldValue">
                    <xsl:with-param name="BaseFieldContainer">base.FieldValue</xsl:with-param>
                  </xsl:call-template>;
                </xsl:for-each>
                BaseClear();
                return true;
            }
            else
                return false;
        }
        
        public void Save()
        {
		    <xsl:if test="normalize-space(TriggerFunctions/BeforeSave) != ''">
                <xsl:value-of select="TriggerFunctions/BeforeSave"/><xsl:text>(this)</xsl:text>;
			</xsl:if>
            <xsl:for-each select="Fields/Field">
              <xsl:text>base.FieldValue["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"] = </xsl:text>
              <xsl:if test="Type = 'enum'">
                  <xsl:text>(int)</xsl:text>      
              </xsl:if>
              <xsl:value-of select="Name"/>
              <xsl:choose>
                <xsl:when test="Type = 'pointer' or Type = 'empty_pointer'">
                  <xsl:text>.UnigueID.UGuid</xsl:text>
                </xsl:when>
              </xsl:choose>;
            </xsl:for-each>
            BaseSave();
			<xsl:if test="normalize-space(TriggerFunctions/AfterSave) != ''">
                <xsl:value-of select="TriggerFunctions/AfterSave"/><xsl:text>(this);</xsl:text>      
            </xsl:if>
        }
		<!--
        public string Serialize(string root = "<xsl:value-of select="$DirectoryName"/>")
        {
            return 
            "&lt;" + root + "&gt;" +
               <xsl:text>"&lt;uid&gt;" + base.UnigueID.ToString() + "&lt;/uid&gt;"</xsl:text> +
               <xsl:for-each select="Fields/Field">
                 <xsl:call-template name="SerializeFieldValue" /> +
               </xsl:for-each>
            <xsl:text>"&lt;/" + root + "&gt;"</xsl:text>;
        }
        -->
        public <xsl:value-of select="$DirectoryName"/>_Objest Copy()
        {
            <xsl:value-of select="$DirectoryName"/>_Objest copy = new <xsl:value-of select="$DirectoryName"/>_Objest();
			copy.New();
            <xsl:for-each select="Fields/Field">
				<xsl:text>copy.</xsl:text><xsl:value-of select="Name"/><xsl:text> = </xsl:text><xsl:value-of select="Name"/>;
			</xsl:for-each>
			return copy;
        }

        public void Delete()
        {
            <xsl:if test="normalize-space(TriggerFunctions/BeforeDelete) != ''">
                <xsl:value-of select="TriggerFunctions/BeforeDelete"/><xsl:text>(this);</xsl:text>      
            </xsl:if>
			base.BaseDelete(<xsl:text>new string[] { </xsl:text>
             <xsl:for-each select="TabularParts/TablePart">
               <xsl:if test="position() != 1">
                 <xsl:text>, </xsl:text>
               </xsl:if>
               <xsl:text>"</xsl:text><xsl:value-of select="Table"/><xsl:text>"</xsl:text>
             </xsl:for-each> });
        }
        
        public <xsl:value-of select="$DirectoryName"/>_Pointer GetDirectoryPointer()
        {
            <xsl:value-of select="$DirectoryName"/>_Pointer directoryPointer = new <xsl:value-of select="$DirectoryName"/>_Pointer(UnigueID.UGuid);
            return directoryPointer;
        }
        
        <xsl:for-each select="Fields/Field">
          <xsl:text>public </xsl:text>
          <xsl:call-template name="FieldType" />
          <xsl:text> </xsl:text>
          <xsl:value-of select="Name"/>
          <xsl:text> { get; set; </xsl:text>}
        </xsl:for-each>
        <xsl:if test="count(TabularParts/TablePart) &gt; 0">
        //Табличні частини
        </xsl:if>
        <xsl:for-each select="TabularParts/TablePart">
            <xsl:variable name="TablePartName" select="concat(Name, '_TablePart')"/>
            <xsl:text>public </xsl:text><xsl:value-of select="concat($DirectoryName, '_', $TablePartName)"/><xsl:text> </xsl:text>
            <xsl:value-of select="$TablePartName"/><xsl:text> { get; set; </xsl:text>}
        </xsl:for-each>
    }
    
    <!--<xsl:call-template name="CommentSummary" />-->
    public class <xsl:value-of select="$DirectoryName"/>_Pointer : DirectoryPointer
    {
        public <xsl:value-of select="$DirectoryName"/>_Pointer(object uid = null) : base(Config.Kernel, "<xsl:value-of select="Table"/>")
        {
            base.Init(new UnigueID(uid), null);
        }
        
        public <xsl:value-of select="$DirectoryName"/>_Pointer(UnigueID uid, Dictionary&lt;string, object&gt; fields = null) : base(Config.Kernel, "<xsl:value-of select="Table"/>")
        {
            base.Init(uid, fields);
        }
        
        public <xsl:value-of select="$DirectoryName"/>_Objest GetDirectoryObject()
        {
            if (this.IsEmpty()) return null;
            <xsl:value-of select="$DirectoryName"/>_Objest <xsl:value-of select="$DirectoryName"/>ObjestItem = new <xsl:value-of select="$DirectoryName"/>_Objest();
            return <xsl:value-of select="$DirectoryName"/>ObjestItem.Read(base.UnigueID) ? <xsl:value-of select="$DirectoryName"/>ObjestItem : null;
        }
		
        public <xsl:value-of select="$DirectoryName"/>_Pointer GetNewDirectoryPointer()
        {
            return new <xsl:value-of select="$DirectoryName"/>_Pointer(base.UnigueID);
        }
		
		public string GetPresentation()
        {
		    return base.BasePresentation(
			    <xsl:text>new string[] { </xsl:text>
                 <xsl:for-each select="Fields/Field[IsPresentation=1]">
                   <xsl:if test="position() != 1">
                     <xsl:text>, </xsl:text>
                   </xsl:if>
                   <xsl:text>"</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"</xsl:text>
                 </xsl:for-each> }
			);
        }
		
        public <xsl:value-of select="$DirectoryName"/>_Pointer GetEmptyPointer()
        {
            return new <xsl:value-of select="$DirectoryName"/>_Pointer();
        }
    }
    
    <!--<xsl:call-template name="CommentSummary" />-->
    public class <xsl:value-of select="$DirectoryName"/>_Select : DirectorySelect
    {
        public <xsl:value-of select="$DirectoryName"/>_Select() : base(Config.Kernel, "<xsl:value-of select="Table"/>") { }        
        public bool Select() { return base.BaseSelect(); }
        
        public bool SelectSingle() { if (base.BaseSelectSingle()) { MoveNext(); return true; } else { Current = null; return false; } }
        
        public bool MoveNext() { if (MoveToPosition()) { Current = new <xsl:value-of select="$DirectoryName"/>_Pointer(base.DirectoryPointerPosition.UnigueID, base.DirectoryPointerPosition.Fields); return true; } else { Current = null; return false; } }

        public <xsl:value-of select="$DirectoryName"/>_Pointer Current { get; private set; }
        
        public <xsl:value-of select="$DirectoryName"/>_Pointer FindByField(string name, object value)
        {
            <xsl:value-of select="$DirectoryName"/>_Pointer itemPointer = new <xsl:value-of select="$DirectoryName"/>_Pointer();
            DirectoryPointer directoryPointer = base.BaseFindByField(name, value);
            if (!directoryPointer.IsEmpty()) itemPointer.Init(directoryPointer.UnigueID);
            return itemPointer;
        }
        
        public List&lt;<xsl:value-of select="$DirectoryName"/>_Pointer&gt; FindListByField(string name, object value, int limit = 0, int offset = 0)
        {
            List&lt;<xsl:value-of select="$DirectoryName"/>_Pointer&gt; directoryPointerList = new List&lt;<xsl:value-of select="$DirectoryName"/>_Pointer&gt;();
            foreach (DirectoryPointer directoryPointer in base.BaseFindListByField(name, value, limit, offset)) 
                directoryPointerList.Add(new <xsl:value-of select="$DirectoryName"/>_Pointer(directoryPointer.UnigueID));
            return directoryPointerList;
        }
    }
    
      <xsl:for-each select="TabularParts/TablePart"> <!-- TableParts -->
        <xsl:variable name="TablePartName" select="Name"/>
        <xsl:variable name="TablePartFullName" select="concat($DirectoryName, '_', $TablePartName)"/>
    
    <!--<xsl:call-template name="CommentSummary" />-->
    public class <xsl:value-of select="$TablePartFullName"/>_TablePart : DirectoryTablePart
    {
        public <xsl:value-of select="$TablePartFullName"/>_TablePart(<xsl:value-of select="$DirectoryName"/>_Objest owner) : base(Config.Kernel, "<xsl:value-of select="Table"/>",
             <xsl:text>new string[] { </xsl:text>
             <xsl:for-each select="Fields/Field">
               <xsl:if test="position() != 1">
                 <xsl:text>, </xsl:text>
               </xsl:if>
               <xsl:text>"</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"</xsl:text>
             </xsl:for-each> }) 
        {
            if (owner == null) throw new Exception("owner null");
            
            Owner = owner;
            Records = new List&lt;Record&gt;();
        }
        <xsl:for-each select="Fields/Field">
        public const string <xsl:value-of select="Name"/> = "<xsl:value-of select="NameInTable"/>";</xsl:for-each>

        public <xsl:value-of select="$DirectoryName"/>_Objest Owner { get; private set; }
        
        public List&lt;Record&gt; Records { get; set; }
        
        public void Read()
        {
            Records.Clear();
            base.BaseRead(Owner.UnigueID);

            foreach (Dictionary&lt;string, object&gt; fieldValue in base.FieldValueList) 
            {
                Record record = new Record();
                record.UID = (Guid)fieldValue["uid"];
                
                <xsl:for-each select="Fields/Field">
                  <xsl:text>record.</xsl:text>
                  <xsl:value-of select="Name"/>
                  <xsl:text> = </xsl:text>
                  <xsl:call-template name="ReadFieldValue">
                    <xsl:with-param name="BaseFieldContainer">fieldValue</xsl:with-param>
                  </xsl:call-template>;
                </xsl:for-each>
                Records.Add(record);
            }
            
            base.BaseClear();
        }
        
        public void Save(bool clear_all_before_save /*= true*/) 
        {
            base.BaseBeginTransaction();
                
            if (clear_all_before_save)
                base.BaseDelete(Owner.UnigueID);

            foreach (Record record in Records)
            {
                Dictionary&lt;string, object&gt; fieldValue = new Dictionary&lt;string, object&gt;();

                <xsl:for-each select="Fields/Field">
                    <xsl:text>fieldValue.Add("</xsl:text>
                    <xsl:value-of select="NameInTable"/><xsl:text>", </xsl:text>
					<xsl:if test="Type = 'enum'">
						<xsl:text>(int)</xsl:text>
					</xsl:if>
					<xsl:text>record.</xsl:text><xsl:value-of select="Name"/>
                    <xsl:choose>
                    <xsl:when test="Type = 'pointer' or Type = 'empty_pointer'">
                        <xsl:text>.UnigueID.UGuid</xsl:text>
                    </xsl:when>
                    </xsl:choose>
                    <xsl:text>)</xsl:text>;
                </xsl:for-each>
                base.BaseSave(record.UID, Owner.UnigueID, fieldValue);
            }
                
            base.BaseCommitTransaction();
        }
        
        public void Delete()
        {
            base.BaseBeginTransaction();
            base.BaseDelete(Owner.UnigueID);
            base.BaseCommitTransaction();
        }
        
        <!--<xsl:call-template name="CommentSummary" />-->
        public class Record : DirectoryTablePartRecord
        {
            public Record()
            {
                <xsl:for-each select="Fields/Field">
                  <xsl:value-of select="Name"/>
                  <xsl:text> = </xsl:text>
                  <xsl:call-template name="DefaultFieldValue" />;
                </xsl:for-each>
            }
            <xsl:for-each select="Fields/Field">
              <xsl:text>public </xsl:text>
              <xsl:call-template name="FieldType" />
              <xsl:text> </xsl:text>
              <xsl:value-of select="Name"/>
              <xsl:text> { get; set; </xsl:text>}
            </xsl:for-each>
        }
    }
      </xsl:for-each> <!-- TableParts -->
   
    #endregion
    </xsl:for-each>
}

namespace <xsl:value-of select="Configuration/NameSpace"/>.Перелічення
{
    <xsl:for-each select="Configuration/Enums/Enum">
    #region ENUM "<xsl:value-of select="Name"/>"
    <!--<xsl:call-template name="CommentSummary" />-->
    public enum <xsl:value-of select="Name"/>
    {
         <xsl:variable name="CountEnumField" select="count(Fields/Field)" />
         <xsl:for-each select="Fields/Field">
             <xsl:value-of select="Name"/>
             <xsl:text> = </xsl:text>
             <xsl:value-of select="Value"/>
             <xsl:if test="position() &lt; $CountEnumField">
         <xsl:text>,
         </xsl:text>
             </xsl:if>
         </xsl:for-each>
    }
    #endregion
    </xsl:for-each>
}

namespace <xsl:value-of select="Configuration/NameSpace"/>.Документи
{
    <xsl:for-each select="Configuration/Documents/Document">
      <xsl:variable name="DocumentName" select="Name"/>
    #region DOCUMENT "<xsl:value-of select="$DocumentName"/>"
    <!--<xsl:call-template name="CommentSummary" />-->
    public static class <xsl:value-of select="$DocumentName"/>_Const
    {
        public const string TABLE = "<xsl:value-of select="Table"/>";
        <xsl:for-each select="Fields/Field">
        public const string <xsl:value-of select="Name"/> = "<xsl:value-of select="NameInTable"/>";</xsl:for-each>
    }
	
    <!--<xsl:call-template name="CommentSummary" />-->
    public class <xsl:value-of select="$DocumentName"/>_Objest : DocumentObject
    {
        public <xsl:value-of select="$DocumentName"/>_Objest() : base(Config.Kernel, "<xsl:value-of select="Table"/>", "<xsl:value-of select="$DocumentName"/>",
             <xsl:text>new string[] { </xsl:text>
             <xsl:for-each select="Fields/Field">
               <xsl:if test="position() != 1">
                 <xsl:text>, </xsl:text>
               </xsl:if>
               <xsl:text>"</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"</xsl:text>
             </xsl:for-each> }) 
        {
            <xsl:for-each select="Fields/Field">
              <xsl:value-of select="Name"/>
              <xsl:text> = </xsl:text>
              <xsl:call-template name="DefaultFieldValue" />;
            </xsl:for-each>
            <xsl:if test="count(TabularParts/TablePart) &gt; 0">
            //Табличні частини
            </xsl:if>
            <xsl:for-each select="TabularParts/TablePart">
                <xsl:variable name="TablePartName" select="concat(Name, '_TablePart')"/>
                <xsl:value-of select="$TablePartName"/><xsl:text> = new </xsl:text>
                <xsl:value-of select="concat($DocumentName, '_', $TablePartName)"/><xsl:text>(this)</xsl:text>;
            </xsl:for-each>
        }
        
        public bool Read(UnigueID uid)
        {
            if (BaseRead(uid))
            {
                <xsl:for-each select="Fields/Field">
                  <xsl:value-of select="Name"/>
                  <xsl:text> = </xsl:text>
                  <xsl:call-template name="ReadFieldValue">
                    <xsl:with-param name="BaseFieldContainer">base.FieldValue</xsl:with-param>
                  </xsl:call-template>;
                </xsl:for-each>
                BaseClear();
                return true;
            }
            else
                return false;
        }
        
        public void Save()
        {
            <xsl:if test="normalize-space(TriggerFunctions/BeforeSave) != ''">
                <xsl:value-of select="TriggerFunctions/BeforeSave"/><xsl:text>(this)</xsl:text>;
			</xsl:if>
            <xsl:for-each select="Fields/Field">
              <xsl:text>base.FieldValue["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"] = </xsl:text>
              <xsl:if test="Type = 'enum'">
                  <xsl:text>(int)</xsl:text>      
              </xsl:if>
              <xsl:value-of select="Name"/>
              <xsl:choose>
                <xsl:when test="Type = 'pointer' or Type = 'empty_pointer'">
                  <xsl:text>.UnigueID.UGuid</xsl:text>
                </xsl:when>
              </xsl:choose>;
            </xsl:for-each>
            BaseSave();
			<xsl:if test="normalize-space(TriggerFunctions/AfterSave) != ''">
                <xsl:value-of select="TriggerFunctions/AfterSave"/><xsl:text>(this);</xsl:text>      
            </xsl:if>
		}

		public bool SpendTheDocument(DateTime spendDate)
		{
            <xsl:choose>
                <xsl:when test="normalize-space(SpendFunctions/Spend) != ''">
		    <xsl:text>bool rezult = </xsl:text><xsl:value-of select="SpendFunctions/Spend"/><xsl:text>(this)</xsl:text>;
		    <xsl:text>BaseSpend(rezult, spendDate)</xsl:text>;
		    <xsl:text>return rezult;</xsl:text>
				</xsl:when>
                <xsl:otherwise>
		    <xsl:text>BaseSpend(false, DateTime.MinValue)</xsl:text>;
		    <xsl:text>return false;</xsl:text>
				</xsl:otherwise>
            </xsl:choose>
		}

		public void ClearSpendTheDocument()
		{
            <xsl:if test="normalize-space(SpendFunctions/ClearSpend) != ''">
                <xsl:value-of select="SpendFunctions/ClearSpend"/>
				<xsl:text>(this)</xsl:text>;
			</xsl:if>
		    <xsl:text>BaseSpend(false, DateTime.MinValue);</xsl:text>
		}

		public <xsl:value-of select="$DocumentName"/>_Objest Copy()
        {
            <xsl:value-of select="$DocumentName"/>_Objest copy = new <xsl:value-of select="$DocumentName"/>_Objest();
			copy.New();
            <xsl:for-each select="Fields/Field">
				<xsl:text>copy.</xsl:text><xsl:value-of select="Name"/><xsl:text> = </xsl:text><xsl:value-of select="Name"/>;
			</xsl:for-each>
			return copy;
        }

        public void Delete()
        {
		    <xsl:if test="normalize-space(TriggerFunctions/BeforeDelete) != ''">
                <xsl:value-of select="TriggerFunctions/BeforeDelete"/><xsl:text>(this);</xsl:text>      
            </xsl:if>
            base.BaseDelete(<xsl:text>new string[] { </xsl:text>
             <xsl:for-each select="TabularParts/TablePart">
               <xsl:if test="position() != 1">
                 <xsl:text>, </xsl:text>
               </xsl:if>
               <xsl:text>"</xsl:text><xsl:value-of select="Table"/><xsl:text>"</xsl:text>
             </xsl:for-each> });
        }
        
        public <xsl:value-of select="$DocumentName"/>_Pointer GetDocumentPointer()
        {
            <xsl:value-of select="$DocumentName"/>_Pointer directoryPointer = new <xsl:value-of select="$DocumentName"/>_Pointer(UnigueID.UGuid);
            return directoryPointer;
        }
        
        <xsl:for-each select="Fields/Field">
          <xsl:text>public </xsl:text>
          <xsl:call-template name="FieldType" />
          <xsl:text> </xsl:text>
          <xsl:value-of select="Name"/>
          <xsl:text> { get; set; </xsl:text>}
        </xsl:for-each>
        <xsl:if test="count(TabularParts/TablePart) &gt; 0">
        //Табличні частини
        </xsl:if>
        <xsl:for-each select="TabularParts/TablePart">
            <xsl:variable name="TablePartName" select="concat(Name, '_TablePart')"/>
            <xsl:text>public </xsl:text><xsl:value-of select="concat($DocumentName, '_', $TablePartName)"/><xsl:text> </xsl:text>
            <xsl:value-of select="$TablePartName"/><xsl:text> { get; set; </xsl:text>}
        </xsl:for-each>
    }
    
    <!--<xsl:call-template name="CommentSummary" />-->
    public class <xsl:value-of select="$DocumentName"/>_Pointer : DocumentPointer
    {
        public <xsl:value-of select="$DocumentName"/>_Pointer(object uid = null) : base(Config.Kernel, "<xsl:value-of select="Table"/>", "<xsl:value-of select="$DocumentName"/>")
        {
            base.Init(new UnigueID(uid), null);
        }
        
        public <xsl:value-of select="$DocumentName"/>_Pointer(UnigueID uid, Dictionary&lt;string, object&gt; fields = null) : base(Config.Kernel, "<xsl:value-of select="Table"/>", "<xsl:value-of select="$DocumentName"/>")
        {
            base.Init(uid, fields);
        }
		
		public string GetPresentation()
        {
		    return base.BasePresentation(
				<xsl:text>new string[] { </xsl:text>
                 <xsl:for-each select="Fields/Field[IsPresentation=1]">
                   <xsl:if test="position() != 1">
                     <xsl:text>, </xsl:text>
                   </xsl:if>
                   <xsl:text>"</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"</xsl:text>
                </xsl:for-each> }
			);
        }
		
        public <xsl:value-of select="$DocumentName"/>_Pointer GetNewDocumentPointer()
        {
            return new <xsl:value-of select="$DocumentName"/>_Pointer(base.UnigueID);
        }
		
        public <xsl:value-of select="$DocumentName"/>_Pointer GetEmptyPointer()
        {
            return new <xsl:value-of select="$DocumentName"/>_Pointer();
        }
		
        public <xsl:value-of select="$DocumentName"/>_Objest GetDocumentObject(bool readAllTablePart = false)
        {
            <xsl:value-of select="$DocumentName"/>_Objest <xsl:value-of select="$DocumentName"/>ObjestItem = new <xsl:value-of select="$DocumentName"/>_Objest();
            <xsl:value-of select="$DocumentName"/>ObjestItem.Read(base.UnigueID);
			<xsl:if test="count(TabularParts/TablePart) != 0">
			if (readAllTablePart)
			{   
				<xsl:for-each select="TabularParts/TablePart">
					<xsl:value-of select="$DocumentName"/>ObjestItem.<xsl:value-of select="concat(Name, '_TablePart')"/>.Read();</xsl:for-each>
			}
			</xsl:if>
            return <xsl:value-of select="$DocumentName"/>ObjestItem;
        }
    }
    
    <!--<xsl:call-template name="CommentSummary" />-->
    public class <xsl:value-of select="$DocumentName"/>_Select : DocumentSelect
    {		
        public <xsl:value-of select="$DocumentName"/>_Select() : base(Config.Kernel, "<xsl:value-of select="Table"/>") { }
        
        public bool Select() { return base.BaseSelect(); }
        
        public bool SelectSingle() { if (base.BaseSelectSingle()) { MoveNext(); return true; } else { Current = null; return false; } }
        
        public bool MoveNext() { if (MoveToPosition()) { Current = new <xsl:value-of select="$DocumentName"/>_Pointer(base.DocumentPointerPosition.UnigueID, base.DocumentPointerPosition.Fields); return true; } else { Current = null; return false; } }
        
        public <xsl:value-of select="$DocumentName"/>_Pointer Current { get; private set; }
    }
    
      <xsl:for-each select="TabularParts/TablePart"> <!-- TableParts -->
        <xsl:variable name="TablePartName" select="Name"/>
        <xsl:variable name="TablePartFullName" select="concat($DocumentName, '_', $TablePartName)"/>
    
    <!--<xsl:call-template name="CommentSummary" />-->
    public class <xsl:value-of select="$TablePartFullName"/>_TablePart : DocumentTablePart
    {
        public <xsl:value-of select="$TablePartFullName"/>_TablePart(<xsl:value-of select="$DocumentName"/>_Objest owner) : base(Config.Kernel, "<xsl:value-of select="Table"/>",
             <xsl:text>new string[] { </xsl:text>
             <xsl:for-each select="Fields/Field">
               <xsl:if test="position() != 1">
                 <xsl:text>, </xsl:text>
               </xsl:if>
               <xsl:text>"</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"</xsl:text>
             </xsl:for-each> }) 
        {
            if (owner == null) throw new Exception("owner null");
            
            Owner = owner;
            Records = new List&lt;Record&gt;();
        }
        <xsl:for-each select="Fields/Field">
        public const string <xsl:value-of select="Name"/> = "<xsl:value-of select="NameInTable"/>";</xsl:for-each>

        public <xsl:value-of select="$DocumentName"/>_Objest Owner { get; private set; }
        
        public List&lt;Record&gt; Records { get; set; }
        
        public void Read()
        {
            Records.Clear();
            base.BaseRead(Owner.UnigueID);

            foreach (Dictionary&lt;string, object&gt; fieldValue in base.FieldValueList) 
            {
                Record record = new Record();
                record.UID = (Guid)fieldValue["uid"];
                
                <xsl:for-each select="Fields/Field">
                  <xsl:text>record.</xsl:text>
                  <xsl:value-of select="Name"/>
                  <xsl:text> = </xsl:text>
                  <xsl:call-template name="ReadFieldValue">
                    <xsl:with-param name="BaseFieldContainer">fieldValue</xsl:with-param>
                  </xsl:call-template>;
                </xsl:for-each>
                Records.Add(record);
            }
            
            base.BaseClear();
        }
        
        public void Save(bool clear_all_before_save /*= true*/) 
        {
            base.BaseBeginTransaction();
                
            if (clear_all_before_save)
                base.BaseDelete(Owner.UnigueID);

            foreach (Record record in Records)
            {
                Dictionary&lt;string, object&gt; fieldValue = new Dictionary&lt;string, object&gt;();

                <xsl:for-each select="Fields/Field">
                    <xsl:text>fieldValue.Add("</xsl:text>
                    <xsl:value-of select="NameInTable"/><xsl:text>", </xsl:text>
					<xsl:if test="Type = 'enum'">
						<xsl:text>(int)</xsl:text>
					</xsl:if>
					<xsl:text>record.</xsl:text><xsl:value-of select="Name"/>
                    <xsl:choose>
                        <xsl:when test="Type = 'pointer' or Type = 'empty_pointer'">
                            <xsl:text>.UnigueID.UGuid</xsl:text>
                        </xsl:when>				
                    </xsl:choose>
                    <xsl:text>)</xsl:text>;
                </xsl:for-each>
                base.BaseSave(record.UID, Owner.UnigueID, fieldValue);
            }
                
            base.BaseCommitTransaction();
        }
        
        public void Delete()
        {
            base.BaseBeginTransaction();
            base.BaseDelete(Owner.UnigueID);
            base.BaseCommitTransaction();
        }

        public List&lt;Record&gt; Copy()
        {
            List&lt;Record&gt; copyRecords = new List&lt;Record&gt;();
            copyRecords = Records;

            foreach (Record copyRecordItem in copyRecords)
                copyRecordItem.UID = Guid.Empty;

            return copyRecords;
        }
        <!--<xsl:call-template name="CommentSummary" />-->
        public class Record : DocumentTablePartRecord
        {
            public Record()
            {
                <xsl:for-each select="Fields/Field">
                  <xsl:value-of select="Name"/>
                  <xsl:text> = </xsl:text>
                  <xsl:call-template name="DefaultFieldValue" />;
                </xsl:for-each>
            }
            <xsl:for-each select="Fields/Field">
              <xsl:text>public </xsl:text>
              <xsl:call-template name="FieldType" />
              <xsl:text> </xsl:text>
              <xsl:value-of select="Name"/>
              <xsl:text> { get; set; </xsl:text>}
            </xsl:for-each>
        }
    }
      </xsl:for-each> <!-- TableParts -->
    
    #endregion
    </xsl:for-each>
}

namespace <xsl:value-of select="Configuration/NameSpace"/>.Журнали
{
    #region Journal
    public class Journal_Select: JournalSelect
    {
        public Journal_Select() : base(Config.Kernel,
             <xsl:text>new string[] { </xsl:text>
             <xsl:for-each select="Configuration/Documents/Document">
               <xsl:if test="position() != 1">
                 <xsl:text>, </xsl:text>
               </xsl:if>
               <xsl:text>"</xsl:text><xsl:value-of select="Table"/><xsl:text>"</xsl:text>
             </xsl:for-each>},
			 <xsl:text>new string[] { </xsl:text>
             <xsl:for-each select="Configuration/Documents/Document">
               <xsl:if test="position() != 1">
                 <xsl:text>, </xsl:text>
               </xsl:if>
               <xsl:text>"</xsl:text><xsl:value-of select="Name"/><xsl:text>"</xsl:text>
             </xsl:for-each>}) { }

        public DocumentObject GetDocumentObject(bool readAllTablePart = true)
        {
            if (Current == null)
                return null;

            switch (Current.TypeDocument)
            {
			    <xsl:for-each select="Configuration/Documents/Document">
					<xsl:text>case </xsl:text>"<xsl:value-of select="Name"/>": return new Документи.<xsl:value-of select="Name"/>_Pointer(Current.UnigueID).GetDocumentObject(readAllTablePart);
				</xsl:for-each>
            }
			
			return null;
        }
    }
    #endregion
<!--
    public class Journal_Document : JournalObject
    {
        public Journal_Document(string documentType, UnigueID uid) : base(Config.Kernel)
        {
            switch (documentType)
            {
			    <xsl:for-each select="Configuration/Documents/Document">
					<xsl:variable name="DocumentName" select="Name"/>
					<xsl:text>case </xsl:text>"<xsl:value-of select="$DocumentName"/>": { base.Table = "<xsl:value-of select="Table"/>"; base.TypeDocument = "<xsl:value-of select="$DocumentName"/>"; break; }
				</xsl:for-each>
            }
            base.BaseRead(uid);
        }
    }
-->
}

namespace <xsl:value-of select="Configuration/NameSpace"/>.РегістриВідомостей
{
    <xsl:for-each select="Configuration/RegistersInformation/RegisterInformation">
       <xsl:variable name="RegisterName" select="Name"/>
    #region REGISTER "<xsl:value-of select="$RegisterName"/>"
    <!--<xsl:call-template name="CommentSummary" />-->
    public static class <xsl:value-of select="$RegisterName"/>_Const
    {
        public const string TABLE = "<xsl:value-of select="Table"/>";
        <xsl:for-each select="(DimensionFields|ResourcesFields|PropertyFields)/Fields/Field">
        public const string <xsl:value-of select="Name"/> = "<xsl:value-of select="NameInTable"/>";</xsl:for-each>
    }
	
    <!--<xsl:call-template name="CommentSummary" />-->
    public class <xsl:value-of select="$RegisterName"/>_RecordsSet : RegisterInformationRecordsSet
    {
        public <xsl:value-of select="$RegisterName"/>_RecordsSet() : base(Config.Kernel, "<xsl:value-of select="Table"/>",
             <xsl:text>new string[] { </xsl:text>
             <xsl:for-each select="(DimensionFields|ResourcesFields|PropertyFields)/Fields/Field">
               <xsl:if test="position() != 1">
                 <xsl:text>, </xsl:text>
               </xsl:if>
               <!--<xsl:value-of select="name(../..)"/>-->
               <xsl:text>"</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"</xsl:text>
             </xsl:for-each> }) 
        {
            Records = new List&lt;Record&gt;();
        }
		
        public List&lt;Record&gt; Records { get; set; }
        
        public void Read()
        {
            Records.Clear();
            base.BaseRead();
            foreach (Dictionary&lt;string, object&gt; fieldValue in base.FieldValueList) 
            {
                Record record = new Record();
                
                record.UID = (Guid)fieldValue["uid"];
				record.Period = DateTime.Parse(fieldValue["period"].ToString());
                record.Owner = (Guid)fieldValue["owner"];
                <xsl:for-each select="(DimensionFields|ResourcesFields|PropertyFields)/Fields/Field">
                  <xsl:text>record.</xsl:text>
                  <xsl:value-of select="Name"/>
                  <xsl:text> = </xsl:text>
                  <xsl:call-template name="ReadFieldValue">
                    <xsl:with-param name="BaseFieldContainer">fieldValue</xsl:with-param>
                  </xsl:call-template>;
                </xsl:for-each>
                Records.Add(record);
            }
            base.BaseClear();
        }
        
        public void Save(DateTime period, Guid owner)
        {
            base.BaseBeginTransaction();
            base.BaseDelete(owner);
            foreach (Record record in Records)
            {
                record.Period = period;
                record.Owner = owner;
                Dictionary&lt;string, object&gt; fieldValue = new Dictionary&lt;string, object&gt;();
                <xsl:for-each select="(DimensionFields|ResourcesFields|PropertyFields)/Fields/Field">
                    <xsl:text>fieldValue.Add("</xsl:text>
                    <xsl:value-of select="NameInTable"/><xsl:text>", </xsl:text>
                    <xsl:if test="Type = 'enum'">
                        <xsl:text>(int)</xsl:text>      
                    </xsl:if>
					<xsl:text>record.</xsl:text><xsl:value-of select="Name"/>
                    <xsl:if test="Type = 'pointer' or Type = 'empty_pointer'">
                    <xsl:text>.UnigueID.UGuid</xsl:text>
                    </xsl:if>
                    <xsl:text>)</xsl:text>;
                </xsl:for-each>
                base.BaseSave(record.UID, period, owner, fieldValue);
            }
            base.BaseCommitTransaction();
        }
        
        public void Delete(Guid owner)
        {
            base.BaseBeginTransaction();
            base.BaseDelete(owner);
            base.BaseCommitTransaction();
        }

        <!--<xsl:call-template name="CommentSummary" />-->
        public class Record : RegisterInformationRecord
        {
            public Record()
            {
                <xsl:for-each select="(DimensionFields|ResourcesFields|PropertyFields)/Fields/Field">
                  <xsl:value-of select="Name"/>
                  <xsl:text> = </xsl:text>
                  <xsl:call-template name="DefaultFieldValue" />;
                </xsl:for-each>
            }
        
            <xsl:for-each select="(DimensionFields|ResourcesFields|PropertyFields)/Fields/Field">
              <xsl:text>public </xsl:text>
              <xsl:call-template name="FieldType" />
              <xsl:text> </xsl:text>
              <xsl:value-of select="Name"/>
              <xsl:text> { get; set; </xsl:text>}
            </xsl:for-each>
        }
    }
	
    <!--<xsl:call-template name="CommentSummary" />-->
    public class <xsl:value-of select="$RegisterName"/>_Objest : RegisterInformationObject
    {
		public <xsl:value-of select="$RegisterName"/>_Objest() : base(Config.Kernel, "<xsl:value-of select="Table"/>",
             <xsl:text>new string[] { </xsl:text>
             <xsl:for-each select="(DimensionFields|ResourcesFields|PropertyFields)/Fields/Field">
               <xsl:if test="position() != 1">
                 <xsl:text>, </xsl:text>
               </xsl:if>
               <xsl:text>"</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"</xsl:text>
             </xsl:for-each> }) 
        {
            <xsl:for-each select="(DimensionFields|ResourcesFields|PropertyFields)/Fields/Field">
              <xsl:value-of select="Name"/>
              <xsl:text> = </xsl:text>
              <xsl:call-template name="DefaultFieldValue" />;
            </xsl:for-each>
        }
        
        public bool Read(UnigueID uid)
        {
            if (BaseRead(uid))
            {
                <xsl:for-each select="(DimensionFields|ResourcesFields|PropertyFields)/Fields/Field">
                  <xsl:value-of select="Name"/>
                  <xsl:text> = </xsl:text>
                  <xsl:call-template name="ReadFieldValue">
                    <xsl:with-param name="BaseFieldContainer">base.FieldValue</xsl:with-param>
                  </xsl:call-template>;
                </xsl:for-each>
                BaseClear();
                return true;
            }
            else
                return false;
        }
        
        public void Save()
        {
            <xsl:for-each select="(DimensionFields|ResourcesFields|PropertyFields)/Fields/Field">
              <xsl:text>base.FieldValue["</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"] = </xsl:text>
              <xsl:if test="Type = 'enum'">
                  <xsl:text>(int)</xsl:text>      
              </xsl:if>
              <xsl:value-of select="Name"/>
              <xsl:choose>
                <xsl:when test="Type = 'pointer' or Type = 'empty_pointer'">
                  <xsl:text>.UnigueID.UGuid</xsl:text>
                </xsl:when>
              </xsl:choose>;
            </xsl:for-each>
            BaseSave();
        }

        public <xsl:value-of select="$RegisterName"/>_Objest Copy()
        {
            <xsl:value-of select="$RegisterName"/>_Objest copy = new <xsl:value-of select="$RegisterName"/>_Objest();
			copy.New();
            <xsl:for-each select="(DimensionFields|ResourcesFields|PropertyFields)/Fields/Field">
				<xsl:text>copy.</xsl:text><xsl:value-of select="Name"/><xsl:text> = </xsl:text><xsl:value-of select="Name"/>;
			</xsl:for-each>
			return copy;
        }

        public void Delete()
        {
			base.BaseDelete();
        }
                
        <xsl:for-each select="(DimensionFields|ResourcesFields|PropertyFields)/Fields/Field">
          <xsl:text>public </xsl:text>
          <xsl:call-template name="FieldType" />
          <xsl:text> </xsl:text>
          <xsl:value-of select="Name"/>
          <xsl:text> { get; set; </xsl:text>}
        </xsl:for-each>
    }
	
    #endregion
  </xsl:for-each>
}

namespace <xsl:value-of select="Configuration/NameSpace"/>.РегістриНакопичення
{
    <xsl:for-each select="Configuration/RegistersAccumulation/RegisterAccumulation">
	   <xsl:variable name="Documents" select="../../Documents"/>
       <xsl:variable name="RegisterName" select="Name"/>
    #region REGISTER "<xsl:value-of select="$RegisterName"/>"
    <!--<xsl:call-template name="CommentSummary" />-->
    public static class <xsl:value-of select="$RegisterName"/>_Const
    {
        public const string TABLE = "<xsl:value-of select="Table"/>";
		public static readonly string[] AllowDocumentSpendTable = new string[] { <xsl:for-each select="AllowDocumentSpend/Name">
		    <xsl:if test="position() != 1">
                <xsl:text>, </xsl:text>
            </xsl:if>
			<xsl:variable name="AllowDocumentSpendName" select="."/>
            <xsl:text>"</xsl:text><xsl:value-of select="$Documents/Document[Name = $AllowDocumentSpendName]/Table"/><xsl:text>"</xsl:text>
		</xsl:for-each> };
		public static readonly string[] AllowDocumentSpendType = new string[] { <xsl:for-each select="AllowDocumentSpend/Name">
		    <xsl:if test="position() != 1">
                <xsl:text>, </xsl:text>
            </xsl:if>
            <xsl:text>"</xsl:text><xsl:value-of select="."/><xsl:text>"</xsl:text>
		</xsl:for-each> };
        <xsl:for-each select="(DimensionFields|ResourcesFields|PropertyFields)/Fields/Field">
        public const string <xsl:value-of select="Name"/> = "<xsl:value-of select="NameInTable"/>";</xsl:for-each>
    }
	
    <!--<xsl:call-template name="CommentSummary" />-->
    public class <xsl:value-of select="$RegisterName"/>_RecordsSet : RegisterAccumulationRecordsSet
    {
        public <xsl:value-of select="$RegisterName"/>_RecordsSet() : base(Config.Kernel, "<xsl:value-of select="Table"/>",
             <xsl:text>new string[] { </xsl:text>
             <xsl:for-each select="(DimensionFields|ResourcesFields|PropertyFields)/Fields/Field">
               <xsl:if test="position() != 1">
                 <xsl:text>, </xsl:text>
               </xsl:if>
               <!--<xsl:value-of select="name(../..)"/>-->
               <xsl:text>"</xsl:text><xsl:value-of select="NameInTable"/><xsl:text>"</xsl:text>
             </xsl:for-each> }) 
        {
            Records = new List&lt;Record&gt;();
        }
		
        public List&lt;Record&gt; Records { get; set; }
        
        public void Read()
        {
            Records.Clear();
            
            base.BaseRead();
            
            foreach (Dictionary&lt;string, object&gt; fieldValue in base.FieldValueList) 
            {
                Record record = new Record();
                record.UID = (Guid)fieldValue["uid"];
				record.Period = DateTime.Parse(fieldValue["period"].ToString());
                record.Income = (bool)fieldValue["income"];
                record.Owner = (Guid)fieldValue["owner"];
                <xsl:for-each select="(DimensionFields|ResourcesFields|PropertyFields)/Fields/Field">
                  <xsl:text>record.</xsl:text>
                  <xsl:value-of select="Name"/>
                  <xsl:text> = </xsl:text>
                  <xsl:call-template name="ReadFieldValue">
                    <xsl:with-param name="BaseFieldContainer">fieldValue</xsl:with-param>
                  </xsl:call-template>;
                </xsl:for-each>
                Records.Add(record);
            }
            
            base.BaseClear();
        }
        
        public void Save(DateTime period, Guid owner) 
        {
            base.BaseBeginTransaction();
            base.BaseDelete(owner);
            foreach (Record record in Records)
            {
                record.Period = period;
                record.Owner = owner;
                Dictionary&lt;string, object&gt; fieldValue = new Dictionary&lt;string, object&gt;();
                <xsl:for-each select="(DimensionFields|ResourcesFields|PropertyFields)/Fields/Field">
                    <xsl:text>fieldValue.Add("</xsl:text>
                    <xsl:value-of select="NameInTable"/><xsl:text>", </xsl:text>
                    <xsl:if test="Type = 'enum'">
                        <xsl:text>(int)</xsl:text>      
                    </xsl:if>
					<xsl:text>record.</xsl:text><xsl:value-of select="Name"/>
                    <xsl:if test="Type = 'pointer' or Type = 'empty_pointer'">
                    <xsl:text>.UnigueID.UGuid</xsl:text>
                    </xsl:if>
                    <xsl:text>)</xsl:text>;
                </xsl:for-each>
                base.BaseSave(record.UID, period, record.Income, owner, fieldValue);
            }
            base.BaseCommitTransaction();
        }

        public void Delete(Guid owner)
        {
            base.BaseBeginTransaction();
            base.BaseDelete(owner);
            base.BaseCommitTransaction();
        }
        
        <!--<xsl:call-template name="CommentSummary" />-->
        public class Record : RegisterAccumulationRecord
        {
            public Record()
            {
                <xsl:for-each select="(DimensionFields|ResourcesFields|PropertyFields)/Fields/Field">
                  <xsl:value-of select="Name"/>
                  <xsl:text> = </xsl:text>
                  <xsl:call-template name="DefaultFieldValue" />;
                </xsl:for-each>
            }
            <xsl:for-each select="(DimensionFields|ResourcesFields|PropertyFields)/Fields/Field">
              <xsl:text>public </xsl:text>
              <xsl:call-template name="FieldType" />
              <xsl:text> </xsl:text>
              <xsl:value-of select="Name"/>
              <xsl:text> { get; set; </xsl:text>}
            </xsl:for-each>
        }
    }
    
    #endregion
  </xsl:for-each>
}
  </xsl:template>
</xsl:stylesheet>