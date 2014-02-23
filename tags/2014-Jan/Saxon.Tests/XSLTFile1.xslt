<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:fn="http://www.w3.org/2005/xpath-functions">

    <xsl:output method="xml" indent="yes"/>

  <xsl:template match="/">
    <xsl:element name="root">
      <xsl:apply-templates select="//value"/>
    </xsl:element>
  </xsl:template>
  
    <xsl:template match="//value">
      <xsl:value-of select="fn:replace('TEST123','\d','#')"/>
      <xsl:element name="waarde">
        <xsl:value-of select="."/>
      </xsl:element>
    </xsl:template>
</xsl:stylesheet>
