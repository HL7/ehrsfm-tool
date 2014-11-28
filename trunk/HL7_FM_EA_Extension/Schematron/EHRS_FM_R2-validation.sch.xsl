<?xml version="1.0" encoding="utf-8" standalone="yes"?>
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
		 <xsl:value-of select="$fileDirParameter" /></xsl:comment><svrl:active-pattern><xsl:attribute name="id">identity-info</xsl:attribute><xsl:attribute name="name">identity-info</xsl:attribute><xsl:apply-templates /></svrl:active-pattern><xsl:apply-templates select="/" mode="M7" /><svrl:active-pattern><xsl:attribute name="id">prioritization</xsl:attribute><xsl:attribute name="name">prioritization</xsl:attribute><xsl:apply-templates /></svrl:active-pattern><xsl:apply-templates select="/" mode="M8" /><svrl:active-pattern><xsl:attribute name="id">function-content</xsl:attribute><xsl:attribute name="name">function-content</xsl:attribute><xsl:apply-templates /></svrl:active-pattern><xsl:apply-templates select="/" mode="M9" /><svrl:active-pattern><xsl:attribute name="id">criteria-content</xsl:attribute><xsl:attribute name="name">criteria-content</xsl:attribute><xsl:apply-templates /></svrl:active-pattern><xsl:apply-templates select="/" mode="M10" /></svrl:schematron-output></xsl:template>

<!--SCHEMATRON PATTERNS-->
<xsl:param name="capital-letters" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'" /><xsl:param name="lowercase-letters" select="'abcdefghijklmnopqrstuvwxyz'" /><xsl:param name="numbers" select="'0123456789'" /><xsl:param name="special-characters" select="'.,/?[]\{}|`~!@#$%^*()-_=+;: &amp;'" /><xsl:param name="quote" select="'&quot;'" /><xsl:param name="apostrophe" select="&quot;'&quot;" /><xsl:param name="printing-characters" select="concat($capital-letters, $lowercase-letters, $numbers, $special-characters, $quote, $apostrophe)" />

<!--PATTERN identity-info-->


	<!--RULE -->
<xsl:template match="object[stereotype/text()='HL7-FM-Profile']" priority="1000" mode="M7"><svrl:fired-rule context="object[stereotype/text()='HL7-FM-Profile']" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron" /><xsl:variable name="id" select="../id" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="tag[@name='Rationale']" /><xsl:otherwise><svrl:failed-assert test="tag[@name='Rationale']" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text /> <svrl:diagnostic-reference diagnostic="FPID01"><xsl:attribute name="xml:lang">en</xsl:attribute>
You have not included a description of the Functional Profile.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="tag[@name='Version']" /><xsl:otherwise><svrl:failed-assert test="tag[@name='Version']" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text /> <svrl:diagnostic-reference diagnostic="FPID02"><xsl:attribute name="xml:lang">en</xsl:attribute>
You have not included the version of the Functional Profile.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="tag[@name='IssuanceDate']" /><xsl:otherwise><svrl:failed-assert test="tag[@name='IssuanceDate']" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text /> <svrl:diagnostic-reference diagnostic="FPID03"><xsl:attribute name="xml:lang">en</xsl:attribute>
You have not included the issuance date of the Functional Profile.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="tag[@name='NormativeLanguage']" /><xsl:otherwise><svrl:failed-assert test="tag[@name='NormativeLanguage']" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text /> <svrl:diagnostic-reference diagnostic="FPID04"><xsl:attribute name="xml:lang">en</xsl:attribute>
IF you have made changes to the normative language you shall include a description of the changes or differences in that.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="tag[@name='SourceVersion']" /><xsl:otherwise><svrl:failed-assert test="tag[@name='SourceVersion']" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text /> <svrl:diagnostic-reference diagnostic="FMTC02"><xsl:attribute name="xml:lang">en</xsl:attribute>
You have not included the version of the Functional Model from which the Functional Profile is derived.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="tag[@name='SourceVersionDate']" /><xsl:otherwise><svrl:failed-assert test="tag[@name='SourceVersionDate']" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text /> <svrl:diagnostic-reference diagnostic="FMTC03"><xsl:attribute name="xml:lang">en</xsl:attribute>
You have not included the date of the Functional Model from which the Functional Profile is derived.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose><xsl:apply-templates select="@*|*" mode="M7" /></xsl:template><xsl:template match="text()" priority="-1" mode="M7" /><xsl:template match="@*|node()" priority="-2" mode="M7"><xsl:apply-templates select="@*|*" mode="M7" /></xsl:template>

<!--PATTERN prioritization-->


	<!--RULE -->
<xsl:template match="object[stereotype/text()='Function' or stereotype/text()='Criteria']" priority="1001" mode="M8"><svrl:fired-rule context="object[stereotype/text()='Function' or stereotype/text()='Criteria']" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="tag[@name='Priority']" /><xsl:otherwise><svrl:failed-assert test="tag[@name='Priority']" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text /> <svrl:diagnostic-reference diagnostic="FMPR01"><xsl:attribute name="xml:lang">en</xsl:attribute>
You have not included the prioritization of this function.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="tag[@name='Priority' and (@value='EN' or @value='EF' or @value='O')]" /><xsl:otherwise><svrl:failed-assert test="tag[@name='Priority' and (@value='EN' or @value='EF' or @value='O')]" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text /> <svrl:diagnostic-reference diagnostic="FMPR01"><xsl:attribute name="xml:lang">en</xsl:attribute>
You have not included the prioritization of this function.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose><xsl:apply-templates select="@*|*" mode="M8" /></xsl:template>

	<!--RULE -->
<xsl:template match="object[stereotype/text()='Function'or stereotype/text()='Criteria']/tag[@name='Priority' and @value='EF']" priority="1000" mode="M8"><svrl:fired-rule context="object[stereotype/text()='Function'or stereotype/text()='Criteria']/tag[@name='Priority' and @value='EF']" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="../tag[@name='Timeframe']" /><xsl:otherwise><svrl:failed-assert test="../tag[@name='Timeframe']" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text /> <svrl:diagnostic-reference diagnostic="FMPR02"><xsl:attribute name="xml:lang">en</xsl:attribute>
You did not inlcude a timeframe for when the function is required te be implemented.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose><xsl:apply-templates select="@*|*" mode="M8" /></xsl:template><xsl:template match="text()" priority="-1" mode="M8" /><xsl:template match="@*|node()" priority="-2" mode="M8"><xsl:apply-templates select="@*|*" mode="M8" /></xsl:template>

<!--PATTERN function-content-->


	<!--RULE -->
<xsl:template match="object[stereotype/text()='Function']" priority="1000" mode="M9"><svrl:fired-rule context="object[stereotype/text()='Function']" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron" /><xsl:variable name="id" select="id" /><xsl:variable name="statement" select="substring-before(substring-after(notes,'$ST$'), '$DE$')" /><xsl:variable name="stmt-lgth" select="string-length($statement)" /><xsl:variable name="description" select="substring-before(substring-after(notes,'$DE$'), '$EX$')" /><xsl:variable name="desc-lgth" select="string-length($description)" /><xsl:variable name="has-criteria" select="count(../object[stereotype/text()='Criteria' and parentId=$id]) &gt; 0" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="id" /><xsl:otherwise><svrl:failed-assert test="id" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text /> <svrl:diagnostic-reference diagnostic="FMST17"><xsl:attribute name="xml:lang">en</xsl:attribute>
The function is missing an ID.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="name" /><xsl:otherwise><svrl:failed-assert test="name" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text /> <svrl:diagnostic-reference diagnostic="FMST26"><xsl:attribute name="xml:lang">en</xsl:attribute>
The function is missing a Name</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="$stmt-lgth &gt; 0" /><xsl:otherwise><svrl:failed-assert test="$stmt-lgth &gt; 0" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text /> <svrl:diagnostic-reference diagnostic="FMST27"><xsl:attribute name="xml:lang">en</xsl:attribute>
The function is missing a Statement</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="$desc-lgth &gt; 0" /><xsl:otherwise><svrl:failed-assert test="$desc-lgth &gt; 0" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text /> <svrl:diagnostic-reference diagnostic="FMST28"><xsl:attribute name="xml:lang">en</xsl:attribute>
The function is missing a Description</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="count(../object[parentId=$id]) &gt; 0" /><xsl:otherwise><svrl:failed-assert test="count(../object[parentId=$id]) &gt; 0" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text /> <svrl:diagnostic-reference diagnostic="FMST29"><xsl:attribute name="xml:lang">en</xsl:attribute>
The function is missing conformance criteria</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--REPORT -->
<xsl:if test="$has-criteria and count(../object[stereotype/text()='Criteria' and parentId=$id]/tag[@name='Optionality' and @value='SHALL']) = 0"><svrl:successful-report test="$has-criteria and count(../object[stereotype/text()='Criteria' and parentId=$id]/tag[@name='Optionality' and @value='SHALL']) = 0" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text /> <svrl:diagnostic-reference diagnostic="FMST12"><xsl:attribute name="xml:lang">en</xsl:attribute>
For this function a mandatory criterion is missing.</svrl:diagnostic-reference></svrl:successful-report></xsl:if><xsl:apply-templates select="@*|*" mode="M9" /></xsl:template><xsl:template match="text()" priority="-1" mode="M9" /><xsl:template match="@*|node()" priority="-2" mode="M9"><xsl:apply-templates select="@*|*" mode="M9" /></xsl:template>

<!--PATTERN criteria-content-->


	<!--RULE -->
<xsl:template match="object[stereotype/text()='Criteria']" priority="1000" mode="M10"><svrl:fired-rule context="object[stereotype/text()='Criteria']" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="tag[@name='Reference.ChangeIndicator']" /><xsl:otherwise><svrl:failed-assert test="tag[@name='Reference.ChangeIndicator']" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text /> <svrl:diagnostic-reference diagnostic="FMTC06"><xsl:attribute name="xml:lang">en</xsl:attribute>
The traceability to the original criterion is missing.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--REPORT -->
<xsl:if test="./tag[@name='Reference.ChangeIndicator' and (@value='C' or @value='NC' or @value='D' or @value='DEP')] and count(./tag[@name='Reference.CriterionID'])=0"><svrl:successful-report test="./tag[@name='Reference.ChangeIndicator' and (@value='C' or @value='NC' or @value='D' or @value='DEP')] and count(./tag[@name='Reference.CriterionID'])=0" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text /> <svrl:diagnostic-reference diagnostic="FMTC06"><xsl:attribute name="xml:lang">en</xsl:attribute>
The traceability to the original criterion is missing.</svrl:diagnostic-reference></svrl:successful-report></xsl:if>

		<!--ASSERT -->
<xsl:choose><xsl:when test="tag[@name='Reference.ChangeIndicator' and (@value='N' or @value='C' or @value='NC' or @value='D' or @value='DEP' or @value='A')]" /><xsl:otherwise><svrl:failed-assert test="tag[@name='Reference.ChangeIndicator' and (@value='N' or @value='C' or @value='NC' or @value='D' or @value='DEP' or @value='A')]" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text /> <svrl:diagnostic-reference diagnostic="FMTC07"><xsl:attribute name="xml:lang">en</xsl:attribute>
The status of the criterion is not included (changes or unchanged).</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose><xsl:apply-templates select="@*|*" mode="M10" /></xsl:template><xsl:template match="text()" priority="-1" mode="M10" /><xsl:template match="@*|node()" priority="-2" mode="M10"><xsl:apply-templates select="@*|*" mode="M10" /></xsl:template></xsl:stylesheet>