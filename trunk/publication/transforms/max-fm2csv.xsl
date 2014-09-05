<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    version="1.0">
    
    <xsl:output method="text" indent="no"/>

    <xsl:param name="line-feed" select="'&#xa;'"/>
    <xsl:variable name="quote" select="'&#x22;'"/>
    <xsl:variable name="single-quote" select='"&#x27;"'/>
    <xsl:variable name="comma" select="','"/>
    
    <xsl:template match="objects">
        <xsl:apply-templates select="object[stereotype/text()='HL7-FM']"/>
    </xsl:template>

    <xsl:template match="object[stereotype/text()='HL7-FM']">
        <xsl:text>ID#,Type,Name,Statement,Description,See Also,CC#,Conformance Criteria,Model Row#,Function ID,Parent Function ID,Optionality,Conditional</xsl:text>

        <xsl:apply-templates select="../object[stereotype/text()='Section']"/>
    </xsl:template>

    <xsl:template match="object[stereotype/text()='Section']">
        <xsl:variable name="object-id" select="id"/>
        <xsl:value-of select="concat($line-feed,alias,',T,',name)"/>        
        
        <xsl:apply-templates select="../object[parentId/text()=$object-id]"/>
    </xsl:template>
        
    <xsl:template match="object[stereotype/text()='Header' or stereotype/text()='Function']">
        <xsl:variable name="object-id" select="id"/>
        <xsl:variable name="parent-id" select="parentId"/>
        <xsl:variable name="statement" select="translate(substring-before(substring-after(notes, '$ST$'), '$DE$'), $quote, $single-quote)"/>
        <xsl:variable name="description" select="translate(substring-before(substring-after(notes, '$DE$'), '$EX$'), $quote, $single-quote)"/>
        <xsl:variable name="plain-name" select="concat($quote, translate(substring-after(name, alias), $quote, $single-quote), $quote)"/>
        <xsl:variable name="function-type">
            <xsl:choose>
                <xsl:when test="stereotype/text()='Function'">F</xsl:when>
                <xsl:when test="stereotype/text()='Header'">H</xsl:when>
                <xsl:otherwise>U</xsl:otherwise>
            </xsl:choose>
        </xsl:variable>
        <xsl:variable name="main-section">
            <xsl:value-of select="concat($comma,$quote,$statement,$quote,$comma,$quote,$description,$quote,$comma,$comma,$comma,$comma,tag[@name='Row']/@value)"/>
        </xsl:variable>
        <xsl:variable name="object-attributes">
            <xsl:value-of select="concat(alias,$comma,../object[id/text()=$parent-id]/alias,$comma,$comma,tag[@name='Conditional']/@value)"/>
        </xsl:variable>

        <xsl:value-of select="concat($line-feed,alias,$comma, $function-type, $comma, $plain-name, $main-section, $comma, $object-attributes)"/>        
        
        <xsl:apply-templates select="../object[parentId/text()=$object-id]"/>
    </xsl:template>

    <xsl:template match="object[stereotype/text()='Criteria']">
        <xsl:variable name="object-id" select="id"/>
        <xsl:variable name="parent-id" select="parentId"/>
        <xsl:variable name="criteria" select="translate(notes, $quote, $single-quote)"/>
        <xsl:variable name="description" select="translate(substring-before(substring-after(notes, '$DE$'), '$EX$'), $quote, $single-quote)"/>
        <xsl:variable name="fn-id" select="substring-before(name, '#')"/>
        <xsl:variable name="criteria-id" select="substring-after(name, '#')"/>
        <xsl:variable name="function-type">
            <xsl:choose>
                <xsl:when test="stereotype/text()='Function'">F</xsl:when>
                <xsl:when test="stereotype/text()='Header'">H</xsl:when>
                <xsl:otherwise>U</xsl:otherwise>
            </xsl:choose>
        </xsl:variable>
        <xsl:variable name="main-section">
            <xsl:value-of select="concat($comma,$comma,$comma,$comma,$criteria-id,$comma,$quote,$criteria,$quote,$comma,tag[@name='Row']/@value)"/>
        </xsl:variable>
        <xsl:variable name="object-attributes">
            <xsl:value-of select="concat($fn-id,$comma,$comma,tag[@name='Optionality']/@value,$comma,tag[@name='Conditional']/@value)"/>
        </xsl:variable>
        
        <xsl:value-of select="concat($line-feed, $fn-id, $comma, 'C', $comma, $main-section, $comma, $object-attributes)"/>        
    </xsl:template>
    
    <xsl:template match="object"></xsl:template>
    <xsl:template match="name"></xsl:template>
    <xsl:template match="notes"></xsl:template>
    <xsl:template match="id"></xsl:template>
    <xsl:template match="stereotype"></xsl:template>
    <xsl:template match="type"></xsl:template>
    <xsl:template match="modified"></xsl:template>
    <xsl:template match="tag"></xsl:template>
    <xsl:template match="parentId"></xsl:template>
    <xsl:template match="alias"></xsl:template>
    <xsl:template match="relationships"></xsl:template>
    <xsl:template match="relationship"></xsl:template>
</xsl:stylesheet>