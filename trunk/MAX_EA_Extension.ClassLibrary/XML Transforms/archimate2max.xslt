<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	xmlns:archimate="http://www.bolton.ac.uk/archimate" 
	xmlns:max="http://www.umcg.nl/MAX">
	
	<xsl:output indent="yes" method="xhtml" encoding="US-ASCII"/>
	
	<xsl:variable name="defaultPackageId" select="archimate:model/@id"/>
	
	<xsl:template match="archimate:model">
		<max:model>
			<objects>
				<object>
					<id><xsl:value-of select="@id"/></id>
					<name><xsl:value-of select="@name"/></name>
					<type>Package</type>
				</object>
			<xsl:apply-templates select="//folder"/>
			<xsl:apply-templates select="//element[not(ends-with(@xsi:type, 'Relationship')) and not(contains(@xsi:type,'Diagram'))]"/>
			</objects>
			<relationships>
				<xsl:apply-templates select="//element[ends-with(@xsi:type, 'Relationship')]"/>
			</relationships>
		</max:model>
	</xsl:template>
	
	<xsl:template match="//folder">
		<object>
			<id><xsl:value-of select="@id"/></id>
			<name><xsl:value-of select="@name"/></name>
			<type>Package</type>
			<xsl:choose>
				<xsl:when test="../name()='folder'">
					<parentId><xsl:value-of select="../@id"/></parentId>
				</xsl:when>
				<xsl:otherwise>
					<parentId><xsl:value-of select="$defaultPackageId"/></parentId>
				</xsl:otherwise>
			</xsl:choose>
		</object>
	</xsl:template>

	<xsl:template match="//element[not(ends-with(@xsi:type, 'Relationship')) and not(contains(@xsi:type,'Diagram'))]">
		<object>
			<id><xsl:value-of select="@id"/></id>
			<name><xsl:value-of select="@name"/></name>
			<xsl:choose>
				<!--
					Some stereotypes need other types.
				 -->
				<xsl:when test="@xsi:type='archimate:BusinessProcess'">
					<type>Activity</type>
				</xsl:when>
				<xsl:when test="@xsi:type='archimate:BusinessFunction'">
					<type>Activity</type>
				</xsl:when>
				<xsl:when test="@xsi:type='archimate:BusinessService'">
					<type>Activity</type>
				</xsl:when>
				<xsl:when test="@xsi:type='archimate:BusinessInterface'">
					<type>Interface</type>
				</xsl:when>
				<xsl:when test="@xsi:type='archimate:ApplicationComponent'">
					<type>Component</type>
				</xsl:when>
				<xsl:when test="@xsi:type='archimate:ApplicationFunction'">
					<type>Activity</type>
				</xsl:when>
				<xsl:when test="@xsi:type='archimate:ApplicationService'">
					<type>Activity</type>
				</xsl:when>
			</xsl:choose>
			<stereotype>ArchiMate_<xsl:value-of select="substring-after(@xsi:type,':')"/></stereotype>
			<xsl:choose>
				<xsl:when test="../name()='folder'">
					<parentId><xsl:value-of select="../@id"/></parentId>
				</xsl:when>
				<xsl:otherwise>
					<parentId><xsl:value-of select="$defaultPackageId"/></parentId>
				</xsl:otherwise>
			</xsl:choose>
		</object>
	</xsl:template>
	
	<xsl:template match="//element[ends-with(@xsi:type, 'Relationship')]">
		<relationship>
			<label><xsl:value-of select="@name"/></label>
			<sourceId><xsl:value-of select="@source"/></sourceId>
			<destId><xsl:value-of select="@target"/></destId>
			<xsl:choose>
				<!--
					Map relationship stereotypes to type/stereotype.
				 -->
				<xsl:when test="@xsi:type='archimate:AssociationRelationship'">
					<type>Association</type>
					<stereotype>ArchiMate_Association</stereotype>
				</xsl:when>
				<xsl:when test="@xsi:type='archimate:CompositionRelationship'">
					<type>Aggregation</type>
					<stereotype>ArchiMate_Composition</stereotype>
				</xsl:when>
				<xsl:when test="@xsi:type='archimate:AssignmentRelationship'">
					<type>Association</type>
					<stereotype>ArchiMate_Assignment</stereotype>
				</xsl:when>
			</xsl:choose>
		</relationship>
	</xsl:template>
	
</xsl:stylesheet>