<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:max="http://www.umcg.nl/MAX">

	<xsl:output indent="yes" method="xml" />
	<xsl:strip-space elements="*" />

	<xsl:template match="model">
		<max:model>
			<objects>
				<xsl:apply-templates mode="objects"/>
			</objects>
			<relationships>
				<xsl:apply-templates mode="relationships"/>
			</relationships>
		</max:model>
	</xsl:template>

	<xsl:template match="class" mode="objects">
		<object>
			<xsl:variable name="class_name" select="normalize-space(name)"/>
			<id><xsl:value-of select="id"/></id>
			<name><xsl:value-of select="$class_name"/></name>
      		<notes><xsl:value-of select="definition"/></notes>
			<xsl:for-each select="/model/attributes/attribute">
				<xsl:if test="class_name = $class_name">
					<xsl:call-template name="attribute"/>
				</xsl:if>
			</xsl:for-each>
		</object>
	</xsl:template>
	
	<xsl:template name="attribute">
		<xsl:element name="attribute">
			<xsl:attribute name="id"><xsl:value-of select="id"/></xsl:attribute>
			<xsl:attribute name="name"><xsl:value-of select="normalize-space(name)"/></xsl:attribute>
			<xsl:attribute name="type"><xsl:value-of select="datatype_name"/></xsl:attribute>
			<xsl:variable name="valueset_name" select="valueset_name"/>
			<xsl:variable name="valueset_id" select="/model/valuesets/valueset[name=$valueset_name]/id"/>
			<xsl:if test="normalize-space($valueset_name)">
				<xsl:element name="tag">
					<xsl:attribute name="name" select="ValueSetName"/>
					<xsl:attribute name="value" select="$valueset_name"/>
				</xsl:element>
				<xsl:element name="tag">
					<xsl:attribute name="name" select="ValueSetId"/>
					<xsl:attribute name="value" select="$valueset_id"/>
				</xsl:element>
			</xsl:if>
			<xsl:value-of select="definition"/>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="attribute" mode="objects"/>

	<xsl:template match="class" mode="relationships"/>

	<xsl:template match="valueset" mode="objects">
		<object>
			<xsl:variable name="valueset_name" select="normalize-space(name)"/>
			<id><xsl:value-of select="id"/></id>
			<name><xsl:value-of select="$valueset_name"/></name>
			<stereotype>enumeration</stereotype>
			<xsl:for-each select="/model/values/value">
				<xsl:if test="valueset_name = $valueset_name">
					<xsl:call-template name="value"/>
				</xsl:if>
			</xsl:for-each>
		</object>
	</xsl:template>
	
	<xsl:template name="value">
		<xsl:element name="attribute">
			<xsl:attribute name="id"><xsl:value-of select="id"/></xsl:attribute>
			<xsl:attribute name="name"><xsl:value-of select="normalize-space(name)"/></xsl:attribute>
			<xsl:attribute name="stereotype">enum</xsl:attribute>
			<xsl:value-of select="definition"/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="value" mode="objects"/>
	
	<xsl:template match="attribute" mode="relationships"/>
	
	<xsl:template match="valueset" mode="relationships"/>

	<xsl:template match="value" mode="relationships"/>

</xsl:stylesheet>