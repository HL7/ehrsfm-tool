<?xml version="1.0" encoding="UTF-8"?>
<!--
	This XML Transform will simplify the MAX XML into something Microsoft Excel can handle.
	It will flatten the hierarchy and move the XML attributes in the element names
	for MAX-**-tags and MAX-object-attribute.
	
	[2015-may-13]
	Combine multi value tags into 1 tag.
 -->
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

	<!-- Copy all elements and simplify attribute and tag -->
	<xsl:template match="object">
		<object>
			<xsl:for-each select="child::node()">
				<!-- copy all child elements except attribute and tag -->
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
			
			<!-- <xsl:apply-templates select="tag" /> -->
			<xsl:for-each-group select="tag" group-by="@name">
				<xsl:variable name="name" select="replace(translate(@name,':()','___'),' ','')"/>
				<xsl:element name="{concat('tag.',$name,'.value')}">
					<xsl:for-each select="current-group()">
						<xsl:value-of select="@value"/>
					</xsl:for-each>
				</xsl:element>
				<xsl:element name="{concat('tag.',$name,'.notes')}">
					<xsl:for-each select="current-group()">
						<xsl:value-of select="text()"/>
					</xsl:for-each>
				</xsl:element>
			</xsl:for-each-group>
		</object>
	</xsl:template>
	
	<!-- simplify tag and attribute to element and value combos -->
	<xsl:template match="tag|attribute">
		<xsl:variable name="name" select="@name"/>
		<xsl:variable name="count" select="count(../tag[@name=$name])"/>
	
		<xsl:variable name="prefix"><xsl:value-of select="name()"/>.</xsl:variable>
		<!-- Remove space and translate illegal tag name chars to underscore -->
		<xsl:variable name="postfix">.<xsl:value-of select="replace(translate(@name,':()','___'),' ','')"/></xsl:variable>
		<xsl:for-each select="@*">
			<xsl:choose>
				<xsl:when test="name()='name'" />
				<xsl:otherwise>
					<xsl:variable name="name"><xsl:value-of select="concat($prefix,name(),$postfix)"/></xsl:variable>
					<xsl:element name="{$name}">
						<xsl:value-of select="." />
					</xsl:element>
				</xsl:otherwise>
			</xsl:choose>		
		</xsl:for-each>
		<xsl:variable name="notes"><xsl:value-of select="concat($prefix,'notes',$postfix)"/></xsl:variable>
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