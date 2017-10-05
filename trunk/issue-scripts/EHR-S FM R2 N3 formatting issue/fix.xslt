<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:max="http://www.umcg.nl/MAX" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
              xmlns:xsd="http://www.w3.org/2001/XMLSchema">

	<xsl:output indent="yes" method="xml"/>
	
	<xsl:template match="/max:model">
		<max:model>
			<xsl:apply-templates/>
		</max:model>
	</xsl:template>
	
	<xsl:template match="/max:model/objects">
		<objects>
			<xsl:apply-templates/>
		</objects>
	</xsl:template>

	<xsl:template match="/max:model/objects/object">
		<xsl:copy>
			<xsl:apply-templates/>
		</xsl:copy>
	</xsl:template>
	
	<xsl:template match="/max:model/objects/object/id"><xsl:copy-of select="."/></xsl:template>
	<xsl:template match="/max:model/objects/object/name"><xsl:copy-of select="."/></xsl:template>
	<xsl:template match="/max:model/objects/object/alias"><xsl:copy-of select="."/></xsl:template>
	<xsl:template match="/max:model/objects/object/stereotype"><xsl:copy-of select="."/></xsl:template>
	<xsl:template match="/max:model/objects/object/type"><xsl:copy-of select="."/></xsl:template>
	<xsl:template match="/max:model/objects/object/parentId"><xsl:copy-of select="."/></xsl:template>
	<xsl:template match="/max:model/objects/object/modified"><xsl:copy-of select="."/></xsl:template>
	<xsl:template match="/max:model/objects/object/tag"><xsl:copy-of select="."/></xsl:template>

	<xsl:template match="/max:model/objects/object/notes">
		<notes><xsl:value-of select="replace(replace(.,'([\.,:;])\s*-','$1&#xD;-'),'([a-z])\.([A-Z])','$1.&#xD;&#xD;$2')"/></notes>
		<!-- <xsl:value-of select="replace(.,'([a-z])\.([A-Z])','$1.&#xD;&#xD;$2')"/> -->
		<!-- <xsl:value-of select="replace(.,'([\.,:;])\s*-','$1&#xD;-')"/> -->
	</xsl:template>
		
	<xsl:template match="/max:model/relationships">
		<xsl:copy-of select="."/>
	</xsl:template>
	
</xsl:stylesheet>