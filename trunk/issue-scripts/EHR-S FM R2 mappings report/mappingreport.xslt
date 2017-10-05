<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:max="http://www.umcg.nl/MAX">

	<xsl:output indent="yes" method="xml"/>
	
	<xsl:template match="/max:model/objects">
		<mappings>
		<xsl:for-each select="/max:model/objects/object[tag/@name='Reference.Alias' and tag/@value='EHR-S_FM_R1.1']">
			<xsl:choose>
				<xsl:when test="stereotype='Criteria'">
					<mapping>
						<xsl:element name="ref"><xsl:value-of select="tag[@name='Reference.FunctionID']/@value"/>#<xsl:value-of select="tag[@name='Reference.CriteriaID']/@value"/></xsl:element>
						<xsl:element name="id"><xsl:value-of select="name"/></xsl:element>
						<xsl:element name="ci"><xsl:value-of select="tag[@name='Reference.ChangeInfo']/@value"/></xsl:element>
					</mapping>
				</xsl:when>
				<xsl:otherwise>
				<mapping>
					<xsl:element name="ref"><xsl:value-of select="tag[@name='Reference.FunctionID']/@value"/></xsl:element>
					<xsl:element name="id"><xsl:value-of select="alias"/></xsl:element>
					<xsl:element name="ci"><xsl:value-of select="tag[@name='Reference.ChangeInfo']/@value"/></xsl:element>
				</mapping>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:for-each>
		</mappings>
	</xsl:template>
		
	<xsl:template match="/max:model/relationships">
	</xsl:template>
	
</xsl:stylesheet>