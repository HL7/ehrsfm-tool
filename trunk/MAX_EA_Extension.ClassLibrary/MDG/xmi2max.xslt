<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:max="http://www.umcg.nl/MAX"
	xmlns:UML="omg.org/UML1.3"
	exclude-result-prefixes="UML">

	<xsl:output indent="yes" method="xml" />
	<xsl:strip-space elements="*" />

	<xsl:template match="/">
		<max:model>
			<objects>
				<xsl:for-each select="//UML:ClassifierRole">
					<xsl:variable name="xmi_id" select="@xmi.id"/>
					<xsl:variable name="package" select="UML:ModelElement.taggedValue/UML:TaggedValue[@tag='package']/@value"/>
					<xsl:variable name="packageID" select="concat('EAID',substring($package,5))"/>
					<xsl:variable name="parentId" select="//UML:ClassifierRole[@xmi.id=$packageID]/UML:ModelElement.taggedValue/UML:TaggedValue[@tag='ea_localid']/@value"/>
					<object>
				      <id><xsl:value-of select="UML:ModelElement.taggedValue/UML:TaggedValue[@tag='ea_localid']/@value"/></id>
				      <name><xsl:value-of select="@name"/></name>
				      <alias><xsl:value-of select="UML:ModelElement.taggedValue/UML:TaggedValue[@tag='alias']/@value"/></alias>
				      <notes><xsl:value-of select="UML:ModelElement.taggedValue/UML:TaggedValue[@tag='documentation']/@value"/></notes>
				      <stereotype><xsl:value-of select="UML:ModelElement.taggedValue/UML:TaggedValue[@tag='stereotype']/@value"/></stereotype>
				      <type>Package</type>
				      <parentId><xsl:value-of select="$parentId"/></parentId>
				      <modified><xsl:value-of select="replace(UML:ModelElement.taggedValue/UML:TaggedValue[@tag='date_modified']/@value,' ','T')"/></modified>
				      <xsl:for-each select="//UML:TaggedValue[@modelElement=$xmi_id]">
				      	<xsl:element name="tag">
				      		<xsl:attribute name="name"><xsl:value-of select="@tag"/></xsl:attribute>
				      		<xsl:variable name="tagvalue" select="substring-before(@value,'#NOTES#')"/>
				      		<xsl:variable name="tagnotes" select="substring-after(@value,'#NOTES#')"/>
				      		<xsl:attribute name="value"><xsl:value-of select="$tagvalue"/></xsl:attribute>
				      		<xsl:value-of select="$tagnotes"/>
				      	</xsl:element>
				      </xsl:for-each>
					</object>
				</xsl:for-each>
				<xsl:for-each select="//UML:Class[@isRoot=false]">
					<xsl:variable name="xmi_id" select="@xmi.id"/>
					<xsl:variable name="package" select="UML:ModelElement.taggedValue/UML:TaggedValue[@tag='package']/@value"/>
					<xsl:variable name="packageID" select="concat('EAID',substring($package,5))"/>
					<xsl:variable name="parentId" select="//UML:ClassifierRole[@xmi.id=$packageID]/UML:ModelElement.taggedValue/UML:TaggedValue[@tag='ea_localid']/@value"/>
					<object>
				      <id><xsl:value-of select="UML:ModelElement.taggedValue/UML:TaggedValue[@tag='ea_localid']/@value"/></id>
				      <name><xsl:value-of select="@name"/></name>
				      <alias><xsl:value-of select="UML:ModelElement.taggedValue/UML:TaggedValue[@tag='alias']/@value"/></alias>
				      <notes><xsl:value-of select="UML:ModelElement.taggedValue/UML:TaggedValue[@tag='documentation']/@value"/></notes>
				      <stereotype><xsl:value-of select="UML:ModelElement.taggedValue/UML:TaggedValue[@tag='stereotype']/@value"/></stereotype>
				      <type><xsl:value-of select="UML:ModelElement.taggedValue/UML:TaggedValue[@tag='ea_stype']/@value"/></type>
				      <parentId><xsl:value-of select="$parentId"/></parentId>
				      <modified><xsl:value-of select="replace(UML:ModelElement.taggedValue/UML:TaggedValue[@tag='date_modified']/@value,' ','T')"/></modified>
				      <xsl:for-each select="//UML:TaggedValue[@modelElement=$xmi_id]">
				      	<xsl:element name="tag">
				      		<xsl:attribute name="name"><xsl:value-of select="@tag"/></xsl:attribute>
				      		<xsl:variable name="tagvalue" select="substring-before(@value,'#NOTES#')"/>
				      		<xsl:variable name="tagnotes" select="substring-after(@value,'#NOTES#')"/>
				      		<xsl:attribute name="value"><xsl:value-of select="$tagvalue"/></xsl:attribute>
				      		<xsl:value-of select="$tagnotes"/>
				      	</xsl:element>
				      </xsl:for-each>
					</object>
				</xsl:for-each>
			</objects>
			<relationships>
				<xsl:for-each select="//UML:Generalization">
				<!--
					subtype = source
					supertype = target
				 -->
					<relationship>
				      <label />
				      <sourceId><xsl:value-of select="@subtype"/></sourceId>
				      <sourceLabel></sourceLabel>
				      <sourceCard></sourceCard>
				      <destId><xsl:value-of select="@supertype"/></destId>
				      <destLabel></destLabel>
				      <destCard></destCard>
				      <notes></notes>
				      <stereotype />
				      <type><xsl:value-of select="UML:ModelElement.taggedValue/UML:TaggedValue[@tag='ea_type']/@value"/></type>
					</relationship>
				</xsl:for-each>
			</relationships>
		</max:model>
	</xsl:template>

</xsl:stylesheet>