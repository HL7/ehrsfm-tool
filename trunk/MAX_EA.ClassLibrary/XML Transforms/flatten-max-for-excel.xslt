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

	<xsl:template match="object">
		<object>
			<xsl:for-each select="child::node()">
				<xsl:choose>
					<xsl:when test="name()='attribute'" />
					<xsl:when test="name()='tag'" />
					<xsl:otherwise>
						<xsl:element name="{name()}">
							<xsl:copy-of select="@*|node()" />
						</xsl:element>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:for-each>
			<xsl:apply-templates select="attribute" />
			<xsl:apply-templates select="tag" />
		</object>
	</xsl:template>
	
	<xsl:template match="attribute">
		<xsl:variable name="prefix">attribute.<xsl:value-of select="@name"/>.</xsl:variable>
		<xsl:for-each select="@*">
			<xsl:variable name="name"><xsl:value-of select="$prefix"/><xsl:value-of select="name()"/></xsl:variable>
			<xsl:element name="{$name}">
				<xsl:value-of select="." />
			</xsl:element>
		</xsl:for-each>
		<xsl:variable name="notes"><xsl:value-of select="$prefix"/>notes</xsl:variable>
		<xsl:element name="{$notes}"><xsl:value-of select="."/></xsl:element>
	</xsl:template>
	
	<xsl:template match="tag">
		<xsl:variable name="prefix">tag.<xsl:value-of select="replace(@name,':','_')"/>.</xsl:variable>
		<xsl:for-each select="@*">
			<xsl:variable name="name"><xsl:value-of select="$prefix"/><xsl:value-of select="name()"/></xsl:variable>
			<xsl:element name="{$name}">
				<xsl:value-of select="." />
			</xsl:element>
		</xsl:for-each>
		<xsl:variable name="notes"><xsl:value-of select="$prefix"/>notes</xsl:variable>
		<xsl:element name="{$notes}"><xsl:value-of select="."/></xsl:element>
	</xsl:template>

	<xsl:template match="/max:model/relationships">
	</xsl:template>

</xsl:stylesheet>