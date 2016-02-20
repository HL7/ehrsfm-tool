<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:max="http://www.umcg.nl/MAX">

	<xsl:output indent="yes" method="xml" />
	<xsl:strip-space elements="*" />

	<xsl:template match="/max:model">
		<max:model>
			<xsl:apply-templates />
		</max:model>
	</xsl:template>

	<xsl:template match="objects">
		<objects>
			<xsl:apply-templates select="object" />
		</objects>
	</xsl:template>

	<xsl:template match="relationships">
		<relationships>
			<xsl:apply-templates select="relationship" />
		</relationships>
	</xsl:template>

	<xsl:template match="object">
		<object>
			<xsl:apply-templates select="." mode="to-full-max"/>
		</object>
	</xsl:template>

	<xsl:template match="relationship">
		<relationship>
			<xsl:apply-templates select="." mode="to-full-max"/>
		</relationship>
	</xsl:template>
	
	<xsl:template match="object|relationship" mode="to-full-max">
		<xsl:for-each select="child::node()">
			<xsl:choose>
				<xsl:when test="starts-with(name(), 'attribute.')">
					<!-- <xsl:variable name="name" select=""/> -->
				</xsl:when>
				<xsl:when test="starts-with(name(), 'tag.notes.')">
					<xsl:variable name="name" select="substring(name(), 11)"/>
					<xsl:element name="tag">
						<xsl:attribute name="name"><xsl:value-of select="$name"/></xsl:attribute>
						<xsl:attribute name="value">&lt;memo&gt;</xsl:attribute>
						<xsl:value-of select="text()"/>
					</xsl:element>
				</xsl:when>
				<xsl:when test="starts-with(name(), 'tag.value.')">
					<xsl:variable name="name" select="substring(name(), 11)"/>
					<xsl:element name="tag">
						<xsl:attribute name="name"><xsl:value-of select="$name"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="text()"/></xsl:attribute>
					</xsl:element>
				</xsl:when>
				<xsl:otherwise>
					<xsl:element name="{name()}">
						<xsl:copy-of select="@*|node()" />
					</xsl:element>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:for-each>
	</xsl:template>

</xsl:stylesheet>