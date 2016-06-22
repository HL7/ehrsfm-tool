<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:max="http://www.umcg.nl/MAX">

	<xsl:output indent="yes" method="xml" />
	<xsl:strip-space elements="*" />

	<xsl:template match="max:model">
		<model>
			<classes>
				<xsl:apply-templates mode="classes"/>
			</classes>
			<attributes>
				<xsl:apply-templates mode="attributes"/>
			</attributes>
			<valuesets>
				<xsl:apply-templates mode="valuesets"/>
			</valuesets>
			<values>
				<xsl:apply-templates mode="values"/>
			</values>
		</model>
	</xsl:template>

	<xsl:template match="object" mode="classes">
		<xsl:if test="stereotype ne 'enumeration' and type eq 'Class'">
			<class>
				<id><xsl:value-of select="id"/></id>
				<name><xsl:value-of select="name"/></name>
				<definition><xsl:value-of select="notes"/></definition>
			</class>
		</xsl:if>
	</xsl:template>

	<xsl:template match="object" mode="attributes">
		<xsl:if test="stereotype ne 'enumeration'">
			<xsl:variable name="class_name" select="name"/>
			<xsl:for-each select="attribute">
				<attribute>
					<id><xsl:value-of select="@id"/></id>
					<name><xsl:value-of select="@name"/></name>
					<class_name><xsl:value-of select="$class_name"/></class_name>
					<definition><xsl:value-of select="text()"/></definition>
					<datatype_name><xsl:value-of select="@type"/></datatype_name>
				</attribute>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>

	<xsl:template match="object" mode="valuesets">
		<xsl:if test="stereotype eq 'enumeration'">
			<valueset>
				<id><xsl:value-of select="id"/></id>
				<name><xsl:value-of select="name"/></name>
				<description><xsl:value-of select="text()"/></description>
				<!-- <delimitedValues><xsl:value-of select="string-join(attribute/@name,';')"/></delimitedValues> -->
			</valueset>
		</xsl:if>
	</xsl:template>

	<xsl:template match="object" mode="values">
		<xsl:if test="stereotype eq 'enumeration'">
			<xsl:variable name="valueset_name" select="name"/>
			<xsl:for-each select="attribute">
				<value>
					<id><xsl:value-of select="@id"/></id>
					<name><xsl:value-of select="@name"/></name>
					<valueset_name><xsl:value-of select="$valueset_name"/></valueset_name>
					<definition><xsl:value-of select="text()"/></definition>
				</value>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>
	
	<xsl:template match="relationship" mode="classes"/>

	<xsl:template match="relationship" mode="attributes"/>

	<xsl:template match="relationship" mode="valuesets"/>

	<xsl:template match="relationship" mode="values"/>

</xsl:stylesheet>