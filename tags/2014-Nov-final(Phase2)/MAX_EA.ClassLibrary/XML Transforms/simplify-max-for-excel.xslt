<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0"
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

	<!-- Copy all elements and simplify attribute and tag -->
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
	
	<xsl:template match="tag|attribute">
		<xsl:variable name="prefix"><xsl:value-of select="name()"/>.</xsl:variable>
		<xsl:variable name="postfix">.<xsl:value-of select="replace(@name,':','_')"/></xsl:variable>
		<xsl:for-each select="@*">
			<xsl:choose>
				<xsl:when test="name()='name'" />
				<xsl:otherwise>
					<xsl:variable name="name"><xsl:value-of select="$prefix"/><xsl:value-of select="name()"/><xsl:value-of select="$postfix"/></xsl:variable>
					<xsl:element name="{$name}">
						<xsl:value-of select="." />
					</xsl:element>
				</xsl:otherwise>
			</xsl:choose>		
		</xsl:for-each>
		<xsl:variable name="notes"><xsl:value-of select="$prefix"/>notes<xsl:value-of select="$postfix"/></xsl:variable>
		<xsl:element name="{$notes}"><xsl:value-of select="."/></xsl:element>
	</xsl:template>

	<xsl:template match="relationship">
		<relationship>
			<xsl:for-each select="child::node()">
				<xsl:choose>
					<xsl:when test="name()='tag'" />
					<xsl:otherwise>
						<xsl:element name="{name()}">
							<xsl:copy-of select="@*|node()" />
						</xsl:element>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:for-each>
			<xsl:apply-templates select="tag" />
		</relationship>
	</xsl:template>
</xsl:stylesheet>
