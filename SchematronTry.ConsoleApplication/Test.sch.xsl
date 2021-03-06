﻿<?xml version="1.0" encoding="utf-8" standalone="yes"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:sch="http://www.ascc.net/xml/schematron" xmlns:iso="http://purl.oclc.org/dsdl/schematron">
  <!--Implementers: please note that overriding process-prolog or process-root is 
    the preferred method for meta-stylesheets to use where possible. -->
<xsl:param name="archiveDirParameter" /><xsl:param name="archiveNameParameter" /><xsl:param name="fileNameParameter" /><xsl:param name="fileDirParameter" />

<!--PHASES-->


<!--PROLOG-->
<xsl:output method="xml" omit-xml-declaration="no" standalone="yes" indent="yes" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" />

<!--KEYS-->


<!--DEFAULT RULES-->


<!--MODE: SCHEMATRON-SELECT-FULL-PATH-->
<!--This mode can be used to generate an ugly though full XPath for locators-->
<xsl:template match="*" mode="schematron-select-full-path"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:template>

<!--MODE: SCHEMATRON-FULL-PATH-->
<!--This mode can be used to generate an ugly though full XPath for locators-->
<xsl:template match="*" mode="schematron-get-full-path"><xsl:apply-templates select="parent::*" mode="schematron-get-full-path" /><xsl:text>/</xsl:text><xsl:choose><xsl:when test="namespace-uri()=''"><xsl:value-of select="name()" /><xsl:variable name="p_1" select="1+    count(preceding-sibling::*[name()=name(current())])" /><xsl:if test="$p_1&gt;1 or following-sibling::*[name()=name(current())]">[<xsl:value-of select="$p_1" />]</xsl:if></xsl:when><xsl:otherwise><xsl:text>*[local-name()='</xsl:text><xsl:value-of select="local-name()" /><xsl:text>' and namespace-uri()='</xsl:text><xsl:value-of select="namespace-uri()" /><xsl:text>']</xsl:text><xsl:variable name="p_2" select="1+   count(preceding-sibling::*[local-name()=local-name(current())])" /><xsl:if test="$p_2&gt;1 or following-sibling::*[local-name()=local-name(current())]">[<xsl:value-of select="$p_2" />]</xsl:if></xsl:otherwise></xsl:choose></xsl:template><xsl:template match="@*" mode="schematron-get-full-path"><xsl:text>/</xsl:text><xsl:choose><xsl:when test="namespace-uri()=''">@<xsl:value-of select="name()" /></xsl:when><xsl:otherwise><xsl:text>@*[local-name()='</xsl:text><xsl:value-of select="local-name()" /><xsl:text>' and namespace-uri()='</xsl:text><xsl:value-of select="namespace-uri()" /><xsl:text>']</xsl:text></xsl:otherwise></xsl:choose></xsl:template>

<!--MODE: SCHEMATRON-FULL-PATH-2-->
<!--This mode can be used to generate prefixed XPath for humans-->
<xsl:template match="node() | @*" mode="schematron-get-full-path-2"><xsl:for-each select="ancestor-or-self::*"><xsl:text>/</xsl:text><xsl:value-of select="name(.)" /><xsl:if test="preceding-sibling::*[name(.)=name(current())]"><xsl:text>[</xsl:text><xsl:value-of select="count(preceding-sibling::*[name(.)=name(current())])+1" /><xsl:text>]</xsl:text></xsl:if></xsl:for-each><xsl:if test="not(self::*)"><xsl:text />/@<xsl:value-of select="name(.)" /></xsl:if></xsl:template>

<!--MODE: GENERATE-ID-FROM-PATH -->
<xsl:template match="/" mode="generate-id-from-path" /><xsl:template match="text()" mode="generate-id-from-path"><xsl:apply-templates select="parent::*" mode="generate-id-from-path" /><xsl:value-of select="concat('.text-', 1+count(preceding-sibling::text()), '-')" /></xsl:template><xsl:template match="comment()" mode="generate-id-from-path"><xsl:apply-templates select="parent::*" mode="generate-id-from-path" /><xsl:value-of select="concat('.comment-', 1+count(preceding-sibling::comment()), '-')" /></xsl:template><xsl:template match="processing-instruction()" mode="generate-id-from-path"><xsl:apply-templates select="parent::*" mode="generate-id-from-path" /><xsl:value-of select="concat('.processing-instruction-', 1+count(preceding-sibling::processing-instruction()), '-')" /></xsl:template><xsl:template match="@*" mode="generate-id-from-path"><xsl:apply-templates select="parent::*" mode="generate-id-from-path" /><xsl:value-of select="concat('.@', name())" /></xsl:template><xsl:template match="*" mode="generate-id-from-path" priority="-0.5"><xsl:apply-templates select="parent::*" mode="generate-id-from-path" /><xsl:text>.</xsl:text><xsl:value-of select="concat('.',name(),'-',1+count(preceding-sibling::*[name()=name(current())]),'-')" /></xsl:template><!--MODE: SCHEMATRON-FULL-PATH-3-->
<!--This mode can be used to generate prefixed XPath for humans 
	(Top-level element has index)-->
<xsl:template match="node() | @*" mode="schematron-get-full-path-3"><xsl:for-each select="ancestor-or-self::*"><xsl:text>/</xsl:text><xsl:value-of select="name(.)" /><xsl:if test="parent::*"><xsl:text>[</xsl:text><xsl:value-of select="count(preceding-sibling::*[name(.)=name(current())])+1" /><xsl:text>]</xsl:text></xsl:if></xsl:for-each><xsl:if test="not(self::*)"><xsl:text />/@<xsl:value-of select="name(.)" /></xsl:if></xsl:template>

<!--MODE: GENERATE-ID-2 -->
<xsl:template match="/" mode="generate-id-2">U</xsl:template><xsl:template match="*" mode="generate-id-2" priority="2"><xsl:text>U</xsl:text><xsl:number level="multiple" count="*" /></xsl:template><xsl:template match="node()" mode="generate-id-2"><xsl:text>U.</xsl:text><xsl:number level="multiple" count="*" /><xsl:text>n</xsl:text><xsl:number count="node()" /></xsl:template><xsl:template match="@*" mode="generate-id-2"><xsl:text>U.</xsl:text><xsl:number level="multiple" count="*" /><xsl:text>_</xsl:text><xsl:value-of select="string-length(local-name(.))" /><xsl:text>_</xsl:text><xsl:value-of select="translate(name(),':','.')" /></xsl:template><!--Strip characters--><xsl:template match="text()" priority="-1" />

<!--SCHEMA METADATA-->
<xsl:template match="/"><svrl:schematron-output title="" schemaVersion="" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:comment><xsl:value-of select="$archiveDirParameter" />   
		 <xsl:value-of select="$archiveNameParameter" />  
		 <xsl:value-of select="$fileNameParameter" />  
		 <xsl:value-of select="$fileDirParameter" /></xsl:comment><svrl:active-pattern><xsl:apply-templates /></svrl:active-pattern><xsl:apply-templates select="/" mode="M0" /><svrl:active-pattern><xsl:apply-templates /></svrl:active-pattern><xsl:apply-templates select="/" mode="M1" /><svrl:active-pattern><xsl:apply-templates /></svrl:active-pattern><xsl:apply-templates select="/" mode="M2" /><svrl:active-pattern><xsl:apply-templates /></svrl:active-pattern><xsl:apply-templates select="/" mode="M3" /><svrl:active-pattern><xsl:apply-templates /></svrl:active-pattern><xsl:apply-templates select="/" mode="M4" /></svrl:schematron-output></xsl:template>

<!--SCHEMATRON PATTERNS-->


<!--PATTERN -->


	<!--RULE -->
<xsl:template match="object[stereotype='Header' and type='Feature']" priority="1000" mode="M0"><svrl:fired-rule context="object[stereotype='Header' and type='Feature']" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron" />

		<!--REPORT -->
<xsl:if test="starts-with(alias,'CPS.')"><svrl:successful-report test="starts-with(alias,'CPS.')" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text>Care Provision Support</svrl:text></svrl:successful-report></xsl:if>

		<!--REPORT -->
<xsl:if test="starts-with(alias,'CP.')"><svrl:successful-report test="starts-with(alias,'CP.')" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text>Care Provision</svrl:text></svrl:successful-report></xsl:if>

		<!--REPORT -->
<xsl:if test="starts-with(alias,'RI.')"><svrl:successful-report test="starts-with(alias,'RI.')" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text>Record Infrastructure</svrl:text></svrl:successful-report></xsl:if><xsl:apply-templates select="*|comment()|processing-instruction()" mode="M0" /></xsl:template><xsl:template match="text()" priority="-1" mode="M0" /><xsl:template match="@*|node()" priority="-2" mode="M0"><xsl:apply-templates select="*|comment()|processing-instruction()" mode="M0" /></xsl:template>

<!--PATTERN -->


	<!--RULE -->
<xsl:template match="object[stereotype='Criteria' and tag.Conditional='N']" priority="1000" mode="M1"><svrl:fired-rule context="object[stereotype='Criteria' and tag.Conditional='N']" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="not(contains(notes, 'IF') and contains (notes, 'THEN'))" /><xsl:otherwise><svrl:failed-assert test="not(contains(notes, 'IF') and contains (notes, 'THEN'))" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text>
        All criteria with the structure above are conditional criteria. <xsl:text /><xsl:value-of select="name" /><xsl:text />
      </svrl:text></svrl:failed-assert></xsl:otherwise></xsl:choose><xsl:apply-templates select="*|comment()|processing-instruction()" mode="M1" /></xsl:template><xsl:template match="text()" priority="-1" mode="M1" /><xsl:template match="@*|node()" priority="-2" mode="M1"><xsl:apply-templates select="*|comment()|processing-instruction()" mode="M1" /></xsl:template>

<!--PATTERN -->


	<!--RULE -->
<xsl:template match="object[stereotype='Criteria' and tag.Conditional='Y']" priority="1000" mode="M2"><svrl:fired-rule context="object[stereotype='Criteria' and tag.Conditional='Y']" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="contains(notes, 'IF') and contains (notes, 'THEN')" /><xsl:otherwise><svrl:failed-assert test="contains(notes, 'IF') and contains (notes, 'THEN')" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text>
        Conditional Criteria <xsl:text /><xsl:value-of select="name" /><xsl:text /> must have the structure “IF condition, THEN consequence”.
      </svrl:text></svrl:failed-assert></xsl:otherwise></xsl:choose><xsl:apply-templates select="*|comment()|processing-instruction()" mode="M2" /></xsl:template><xsl:template match="text()" priority="-1" mode="M2" /><xsl:template match="@*|node()" priority="-2" mode="M2"><xsl:apply-templates select="*|comment()|processing-instruction()" mode="M2" /></xsl:template>

<!--PATTERN -->


	<!--RULE -->
<xsl:template match="object[stereotype='Header']" priority="1000" mode="M3"><svrl:fired-rule context="object[stereotype='Header']" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="number(substring-after(alias,'.'))" /><xsl:otherwise><svrl:failed-assert test="number(substring-after(alias,'.'))" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text>Header ID not formatted properly <xsl:text /><xsl:value-of select="alias" /><xsl:text />.</svrl:text></svrl:failed-assert></xsl:otherwise></xsl:choose><xsl:apply-templates select="*|comment()|processing-instruction()" mode="M3" /></xsl:template><xsl:template match="text()" priority="-1" mode="M3" /><xsl:template match="@*|node()" priority="-2" mode="M3"><xsl:apply-templates select="*|comment()|processing-instruction()" mode="M3" /></xsl:template>

<!--PATTERN -->


	<!--RULE -->
<xsl:template match="object[stereotype='Criteria']" priority="1000" mode="M4"><svrl:fired-rule context="object[stereotype='Criteria']" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="tag.Optionality" /><xsl:otherwise><svrl:failed-assert test="tag.Optionality" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text>Optionality must be specified for Criteria <xsl:text /><xsl:value-of select="name" /><xsl:text />.</svrl:text></svrl:failed-assert></xsl:otherwise></xsl:choose><xsl:apply-templates select="*|comment()|processing-instruction()" mode="M4" /></xsl:template><xsl:template match="text()" priority="-1" mode="M4" /><xsl:template match="@*|node()" priority="-2" mode="M4"><xsl:apply-templates select="*|comment()|processing-instruction()" mode="M4" /></xsl:template></xsl:stylesheet>