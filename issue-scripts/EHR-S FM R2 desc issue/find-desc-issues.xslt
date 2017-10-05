<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:max="http://www.umcg.nl/MAX" 
	exclude-result-prefixes="max">

	<xsl:output indent="yes" method="xml"/>
	<xsl:strip-space elements="*"/>
	
	<xsl:template match="/max:model">
		<report>
			<xsl:apply-templates/>
		</report>
	</xsl:template>
	
	<xsl:template match="/max:model/objects/object">
		<xsl:if test="matches(notes,'\[\[.+\]\]')">
			<xsl:variable name="id">
				<xsl:choose>
					<xsl:when test="stereotype eq 'Criteria'"><xsl:value-of select="substring-before(name,'#')"/></xsl:when>
					<xsl:otherwise><xsl:value-of select="alias"/></xsl:otherwise>
				</xsl:choose>
			</xsl:variable>
			<xsl:variable name="other_id" select="substring-before(substring-after(notes, '[['), ']]')"/>
			<xsl:variable name="sourceId" select="/max:model/objects/object[alias=$id]/id"/>
			<xsl:variable name="destId" select="/max:model/objects/object[alias=$other_id]/id"/>
			<!-- <debug>
				<xsl:attribute name="id" select="$id"/>
				<xsl:attribute name="other_id" select="$other_id"/>
				<xsl:attribute name="sourceId" select="$sourceId"/>
				<xsl:attribute name="destId" select="$destId"/>
			</debug> -->
			<xsl:if test="not(exists(/max:model/relationships/relationship[sourceId=$sourceId and destId=$destId]))">
				<error>Missing relationship from <xsl:value-of select="$id"/> to <xsl:value-of select="$other_id"/></error>
			</xsl:if>
		</xsl:if>
	</xsl:template>

	<xsl:template match="/max:model/relationships">
	</xsl:template>
	
</xsl:stylesheet>