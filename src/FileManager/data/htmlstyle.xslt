<?xml version="1.0" encoding="utf-8"?>

<!-- 
     Author	: Benjamin Peng
     Version	:1.0
-->

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output  method="html" indent="yes" doctype-public='-//W3C//DTD XHTML 1.0 Transitional//EN'/>

  <xsl:template match="/">
    <html>
      <head>
        <title>index</title>
        <link href="style.css" rel="Stylesheet" type="text/css" media="screen" />
      </head>
      <body>
        <div id="wrapper">
          <div id="header">
            <div>
              <a>
                <xsl:attribute name="href">
                  <xsl:value-of select="/Root/Path/@FullPath" disable-output-escaping="yes" />
                </xsl:attribute>
                <xsl:value-of select="/Root/Path/@Name" disable-output-escaping="yes" /> Index
              </a>
              <br />
            </div>
          </div>

          <div class="line">
          </div>

          <!--<xsl:apply-templates select="/Root/Path/Path">
          </xsl:apply-templates>-->

          <xsl:call-template name="t">
            <xsl:with-param name="collection" select="/Root/Path/Path"></xsl:with-param>
          </xsl:call-template>

        </div>
      </body>
    </html>
  </xsl:template>

  <xsl:template name="t">
    <xsl:param name="collection" />
    <xsl:for-each select="$collection">
      <div class="block">
        <div class="title">
          <span>
            <a>
              <xsl:attribute name="href">
                <xsl:value-of select="@FullPath" disable-output-escaping="yes" />
              </xsl:attribute>
              <xsl:value-of select="@Name" disable-output-escaping="yes" />
            </a>
          </span>
        </div>
        <div class="content">
          <xsl:if test="count($collection) &gt; 0">
            <xsl:call-template name="t">
              <xsl:with-param name="collection" select="Path"></xsl:with-param>
            </xsl:call-template>
          </xsl:if>
          <ul>
            <xsl:for-each select="File">
              <li style="margin-left:20px">
                <xsl:attribute name="href">
                  <xsl:value-of select="@FullPath" disable-output-escaping="yes" />
                </xsl:attribute>
                <xsl:value-of select="@Name" disable-output-escaping="yes" />
              </li>
            </xsl:for-each>
          </ul>
        </div>
      </div>
      <div class="clear"></div>
    </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>