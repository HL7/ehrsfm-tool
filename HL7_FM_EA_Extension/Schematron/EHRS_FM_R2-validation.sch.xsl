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
		 <xsl:value-of select="$fileDirParameter" /></xsl:comment><svrl:active-pattern><xsl:attribute name="id">header</xsl:attribute><xsl:attribute name="name">header</xsl:attribute><xsl:apply-templates /></svrl:active-pattern><xsl:apply-templates select="/" mode="M7" /><svrl:active-pattern><xsl:attribute name="id">core-structure</xsl:attribute><xsl:attribute name="name">core-structure</xsl:attribute><xsl:apply-templates /></svrl:active-pattern><xsl:apply-templates select="/" mode="M8" /><svrl:active-pattern><xsl:attribute name="id">header-components</xsl:attribute><xsl:attribute name="name">header-components</xsl:attribute><xsl:apply-templates /></svrl:active-pattern><xsl:apply-templates select="/" mode="M9" /><svrl:active-pattern><xsl:attribute name="id">function-components</xsl:attribute><xsl:attribute name="name">function-components</xsl:attribute><xsl:apply-templates /></svrl:active-pattern><xsl:apply-templates select="/" mode="M10" /><svrl:active-pattern><xsl:attribute name="id">criteria-components</xsl:attribute><xsl:attribute name="name">criteria-components</xsl:attribute><xsl:apply-templates /></svrl:active-pattern><xsl:apply-templates select="/" mode="M11" /></svrl:schematron-output></xsl:template>

<!--SCHEMATRON PATTERNS-->
<xsl:param name="capital-letters" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'" /><xsl:param name="lowercase-letters" select="'abcdefghijklmnopqrstuvwxyz'" /><xsl:param name="numbers" select="'0123456789'" /><xsl:param name="special-characters" select="'.,/?[]\{}|`~!@#$%^*()-_=+;: &amp;'" /><xsl:param name="quote" select="'&quot;'" /><xsl:param name="apostrophe" select="&quot;'&quot;" /><xsl:param name="printing-characters" select="concat($capital-letters, $lowercase-letters, $numbers, $special-characters, $quote, $apostrophe)" />

<!--PATTERN header-->


	<!--RULE -->
<xsl:template match="object" priority="1000" mode="M7"><svrl:fired-rule context="object" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron" /><xsl:apply-templates select="*" mode="M7" /></xsl:template><xsl:template match="text()" priority="-1" mode="M7" /><xsl:template match="@*|node()" priority="-2" mode="M7"><xsl:apply-templates select="*" mode="M7" /></xsl:template>

<!--PATTERN core-structure-->


	<!--RULE -->
<xsl:template match="object/type" priority="1002" mode="M8"><svrl:fired-rule context="object/type" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron" /><xsl:variable name="id" select="../id" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="./text()='Package' or ./text()='Feature' or ./text()='Requirement'" /><xsl:otherwise><svrl:failed-assert test="./text()='Package' or ./text()='Feature' or ./text()='Requirement'" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMST001"><xsl:attribute name="xml:lang">en</xsl:attribute>
Error wrong type, must be one of Package, Feature, or Requirement</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose><xsl:apply-templates select="*" mode="M8" /></xsl:template>

	<!--RULE -->
<xsl:template match="object[type/text()='Feature']" priority="1001" mode="M8"><svrl:fired-rule context="object[type/text()='Feature']" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron" /><xsl:variable name="id" select="id" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="stereotype[text()='Header' or text()='Function']" /><xsl:otherwise><svrl:failed-assert test="stereotype[text()='Header' or text()='Function']" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMST001"><xsl:attribute name="xml:lang">en</xsl:attribute>
Error wrong type, must be one of Package, Feature, or Requirement</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose><xsl:apply-templates select="*" mode="M8" /></xsl:template>

	<!--RULE -->
<xsl:template match="object[type/text()='Requirement']" priority="1000" mode="M8"><svrl:fired-rule context="object[type/text()='Requirement']" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron" /><xsl:variable name="id" select="id" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="stereotype[text()='Criteria']" /><xsl:otherwise><svrl:failed-assert test="stereotype[text()='Criteria']" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMST001"><xsl:attribute name="xml:lang">en</xsl:attribute>
Error wrong type, must be one of Package, Feature, or Requirement</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose><xsl:apply-templates select="*" mode="M8" /></xsl:template><xsl:template match="text()" priority="-1" mode="M8" /><xsl:template match="@*|node()" priority="-2" mode="M8"><xsl:apply-templates select="*" mode="M8" /></xsl:template>

<!--PATTERN header-components-->


	<!--RULE -->
<xsl:template match="object[stereotype/text()='Header']" priority="1000" mode="M9"><svrl:fired-rule context="object[stereotype/text()='Header']" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron" /><xsl:variable name="id" select="id" /><xsl:variable name="thing-id" select="alias" /><xsl:variable name="parent-id" select="parentId" /><xsl:variable name="parent-alias" select="../object[id=$parent-id]/alias" /><xsl:variable name="statement" select="substring-before(substring-after(notes,'$ST$'), '$DE$')" /><xsl:variable name="stmt-lgth" select="string-length($statement)" /><xsl:variable name="description" select="substring-before(substring-after(notes,'$DE$'), '$EX$')" /><xsl:variable name="desc-lgth" select="string-length($description)" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="count(../object/alias[text()=$thing-id])=1" /><xsl:otherwise><svrl:failed-assert test="count(../object/alias[text()=$thing-id])=1" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMHC001"><xsl:attribute name="xml:lang">en</xsl:attribute>
The Header should have a unique ID.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="name" /><xsl:otherwise><svrl:failed-assert test="name" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMHC002"><xsl:attribute name="xml:lang">en</xsl:attribute>
The Header should have a name.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose><xsl:variable name="name" select="name" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="count(../object/name[text()=$name])=1" /><xsl:otherwise><svrl:failed-assert test="count(../object/name[text()=$name])=1" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMHC003"><xsl:attribute name="xml:lang">en</xsl:attribute>
The name of the header <xsl:text /><xsl:value-of select="$name" /><xsl:text /> is not unique.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="starts-with($name,$thing-id)" /><xsl:otherwise><svrl:failed-assert test="starts-with($name,$thing-id)" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text>
              <xsl:text /><xsl:value-of select="$id" /><xsl:text />
            </svrl:text> <svrl:diagnostic-reference diagnostic="MZ02"><xsl:attribute name="xml:lang">en</xsl:attribute>
Header ID in Name and Alias are different.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="$stmt-lgth &gt; 0" /><xsl:otherwise><svrl:failed-assert test="$stmt-lgth &gt; 0" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMHC004"><xsl:attribute name="xml:lang">en</xsl:attribute>
Header must have a statement about its purpose.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="substring($statement, $stmt-lgth, 1) = '.'" /><xsl:otherwise><svrl:failed-assert test="substring($statement, $stmt-lgth, 1) = '.'" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMHC005"><xsl:attribute name="xml:lang">en</xsl:attribute>
Header purpose statement must end in a period.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="$desc-lgth &gt; 0" /><xsl:otherwise><svrl:failed-assert test="$desc-lgth &gt; 0" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMHC006"><xsl:attribute name="xml:lang">en</xsl:attribute>
Header must have a description.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="substring($description, $desc-lgth, 1) = '.'" /><xsl:otherwise><svrl:failed-assert test="substring($description, $desc-lgth, 1) = '.'" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMHC007"><xsl:attribute name="xml:lang">en</xsl:attribute>
Header description must end in a period.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="count(../object[stereotype='Function' and parentId=$id]) &gt; 0" /><xsl:otherwise><svrl:failed-assert test="count(../object[stereotype='Function' and parentId=$id]) &gt; 0" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMHC009"><xsl:attribute name="xml:lang">en</xsl:attribute>
A Header must have at least one descendant function.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="count(../object[stereotype/text()='Criteria' and parentId=$id]) = 0" /><xsl:otherwise><svrl:failed-assert test="count(../object[stereotype/text()='Criteria' and parentId=$id]) = 0" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMHC010"><xsl:attribute name="xml:lang">en</xsl:attribute>
A Header must not have criteria.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose><xsl:variable name="start-string" select="concat(substring($statement, 1, 25), '...')" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="contains($capital-letters, substring($statement,1,1))" /><xsl:otherwise><svrl:failed-assert test="contains($capital-letters, substring($statement,1,1))" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMTSR02"><xsl:attribute name="xml:lang">en</xsl:attribute>
The text (<xsl:text /><xsl:value-of select="$start-string" /><xsl:text />...) does not start with a capital letter.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose><xsl:variable name="start-string2" select="concat(substring($description, 1, 25), '...')" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="contains($capital-letters, substring($description,1,1))" /><xsl:otherwise><svrl:failed-assert test="contains($capital-letters, substring($description,1,1))" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMTSR08"><xsl:attribute name="xml:lang">en</xsl:attribute>
The text (<xsl:text /><xsl:value-of select="$start-string2" /><xsl:text />...) does not start with a capital letter.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose><xsl:variable name="non-printing-text1" select="translate($statement, $printing-characters, '')" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="string-length($non-printing-text1)=0" /><xsl:otherwise><svrl:failed-assert test="string-length($non-printing-text1)=0" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMTSR04"><xsl:attribute name="xml:lang">en</xsl:attribute>
The text contains non-printable characters [<xsl:text /><xsl:value-of select="$non-printing-text1" /><xsl:text />].</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose><xsl:variable name="non-printing-text2" select="translate($description, $printing-characters, '')" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="string-length($non-printing-text2)=0" /><xsl:otherwise><svrl:failed-assert test="string-length($non-printing-text2)=0" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMTSR09"><xsl:attribute name="xml:lang">en</xsl:attribute>
The text contains non-printable characters [<xsl:text /><xsl:value-of select="$non-printing-text2" /><xsl:text />].</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose><xsl:variable name="local-alias" select="substring-after($thing-id, $parent-alias)" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="starts-with($local-alias, '.')" /><xsl:otherwise><svrl:failed-assert test="starts-with($local-alias, '.')" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMFR002"><xsl:attribute name="xml:lang">en</xsl:attribute>
The Header ID (<xsl:text /><xsl:value-of select="$thing-id" /><xsl:text />) does not have the proper format: {Parent-ID}.{local-number}.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="string-length(translate(substring($local-alias, 2), $numbers, '')) = 0" /><xsl:otherwise><svrl:failed-assert test="string-length(translate(substring($local-alias, 2), $numbers, '')) = 0" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMFR002"><xsl:attribute name="xml:lang">en</xsl:attribute>
The Header ID (<xsl:text /><xsl:value-of select="$thing-id" /><xsl:text />) does not have the proper format: {Parent-ID}.{local-number}.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose><xsl:apply-templates select="*" mode="M9" /></xsl:template><xsl:template match="text()" priority="-1" mode="M9" /><xsl:template match="@*|node()" priority="-2" mode="M9"><xsl:apply-templates select="*" mode="M9" /></xsl:template>

<!--PATTERN function-components-->


	<!--RULE -->
<xsl:template match="object[stereotype/text()='Function']" priority="1000" mode="M10"><svrl:fired-rule context="object[stereotype/text()='Function']" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron" /><xsl:variable name="id" select="id" /><xsl:variable name="thing-id" select="alias" /><xsl:variable name="parent-id" select="parentId" /><xsl:variable name="parent-alias" select="../object[id=$parent-id]/alias" /><xsl:variable name="statement" select="substring-before(substring-after(notes,'$ST$'), '$DE$')" /><xsl:variable name="stmt-lgth" select="string-length($statement)" /><xsl:variable name="description" select="substring-before(substring-after(notes,'$DE$'), '$EX$')" /><xsl:variable name="desc-lgth" select="string-length($description)" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="count(../object/alias[text()=$thing-id])=1" /><xsl:otherwise><svrl:failed-assert test="count(../object/alias[text()=$thing-id])=1" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMFLC02"><xsl:attribute name="xml:lang">en</xsl:attribute>
The Function should have a unique ID.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="starts-with(alias/text(), $parent-alias)" /><xsl:otherwise><svrl:failed-assert test="starts-with(alias/text(), $parent-alias)" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMFLC03"><xsl:attribute name="xml:lang">en</xsl:attribute>
The Function ID is not consistent with the section belongs to.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose><xsl:variable name="name" select="name" /><xsl:variable name="name-end" select="substring($name, string-length($name), 1)" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="starts-with($name,$thing-id)" /><xsl:otherwise><svrl:failed-assert test="starts-with($name,$thing-id)" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text>
            <xsl:text /><xsl:value-of select="$id" /><xsl:text />
          </svrl:text> <svrl:diagnostic-reference diagnostic="MZ03"><xsl:attribute name="xml:lang">en</xsl:attribute>
Function ID in Name and Alias are different.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="count(../object/name[text()=$name])=1" /><xsl:otherwise><svrl:failed-assert test="count(../object/name[text()=$name])=1" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMFLC06"><xsl:attribute name="xml:lang">en</xsl:attribute>
The name of the function <xsl:text /><xsl:value-of select="$name" /><xsl:text /> is not unique.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="starts-with(name/text(), $thing-id)" /><xsl:otherwise><svrl:failed-assert test="starts-with(name/text(), $thing-id)" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMFLC07"><xsl:attribute name="xml:lang">en</xsl:attribute>
Function name must be accompanied by the Function ID.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="not($name-end='.')" /><xsl:otherwise><svrl:failed-assert test="not($name-end='.')" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMFLC08"><xsl:attribute name="xml:lang">en</xsl:attribute>
Function names must not end with a period.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="$stmt-lgth &gt; 0" /><xsl:otherwise><svrl:failed-assert test="$stmt-lgth &gt; 0" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMFLC09"><xsl:attribute name="xml:lang">en</xsl:attribute>
Function should have a statement about its purpose</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="substring($statement, $stmt-lgth, 1) = '.'" /><xsl:otherwise><svrl:failed-assert test="substring($statement, $stmt-lgth, 1) = '.'" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMFLC10"><xsl:attribute name="xml:lang">en</xsl:attribute>
Function statement should end with a period.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="$desc-lgth &gt; 0" /><xsl:otherwise><svrl:failed-assert test="$desc-lgth &gt; 0" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMFLC11"><xsl:attribute name="xml:lang">en</xsl:attribute>
Function should have a description.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="substring($description, $desc-lgth, 1) = '.'" /><xsl:otherwise><svrl:failed-assert test="substring($description, $desc-lgth, 1) = '.'" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMFLC12"><xsl:attribute name="xml:lang">en</xsl:attribute>
Function description should end with a period.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="count(../object[stereotype/text()='Criteria' and parentId=$id]) &gt; 0" /><xsl:otherwise><svrl:failed-assert test="count(../object[stereotype/text()='Criteria' and parentId=$id]) &gt; 0" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMFLC13"><xsl:attribute name="xml:lang">en</xsl:attribute>
Function must have one or more conformance criteria.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose><xsl:variable name="start-string" select="concat(substring($statement, 1, 25), '...')" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="contains($capital-letters, substring($statement,1,1))" /><xsl:otherwise><svrl:failed-assert test="contains($capital-letters, substring($statement,1,1))" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMTSR02"><xsl:attribute name="xml:lang">en</xsl:attribute>
The text (<xsl:text /><xsl:value-of select="$start-string" /><xsl:text />...) does not start with a capital letter.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose><xsl:variable name="start-string2" select="concat(substring($description, 1, 25), '...')" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="contains($capital-letters, substring($description,1,1))" /><xsl:otherwise><svrl:failed-assert test="contains($capital-letters, substring($description,1,1))" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMTSR08"><xsl:attribute name="xml:lang">en</xsl:attribute>
The text (<xsl:text /><xsl:value-of select="$start-string2" /><xsl:text />...) does not start with a capital letter.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose><xsl:variable name="non-printing-text1" select="translate($statement, $printing-characters, '')" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="string-length($non-printing-text1)=0" /><xsl:otherwise><svrl:failed-assert test="string-length($non-printing-text1)=0" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMTSR04"><xsl:attribute name="xml:lang">en</xsl:attribute>
The text contains non-printable characters [<xsl:text /><xsl:value-of select="$non-printing-text1" /><xsl:text />].</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose><xsl:variable name="non-printing-text2" select="translate($description, $printing-characters, '')" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="string-length($non-printing-text2)=0" /><xsl:otherwise><svrl:failed-assert test="string-length($non-printing-text2)=0" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMTSR09"><xsl:attribute name="xml:lang">en</xsl:attribute>
The text contains non-printable characters [<xsl:text /><xsl:value-of select="$non-printing-text2" /><xsl:text />].</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose><xsl:variable name="local-alias" select="substring-after($thing-id, $parent-alias)" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="starts-with($local-alias, '.')" /><xsl:otherwise><svrl:failed-assert test="starts-with($local-alias, '.')" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMFR003"><xsl:attribute name="xml:lang">en</xsl:attribute>
The Function ID (<xsl:text /><xsl:value-of select="$thing-id" /><xsl:text />) does not have the proper format: {Parent-ID}.{local-number}.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="string-length(translate(substring($local-alias, 2), $numbers, '')) = 0" /><xsl:otherwise><svrl:failed-assert test="string-length(translate(substring($local-alias, 2), $numbers, '')) = 0" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMFR003"><xsl:attribute name="xml:lang">en</xsl:attribute>
The Function ID (<xsl:text /><xsl:value-of select="$thing-id" /><xsl:text />) does not have the proper format: {Parent-ID}.{local-number}.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--REPORT -->
<xsl:if test="id/text() = alias/text()"><svrl:successful-report test="id/text() = alias/text()" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMID001"><xsl:attribute name="xml:lang">en</xsl:attribute>
The Function ID is the same as the visible ID.</svrl:diagnostic-reference></svrl:successful-report></xsl:if><xsl:apply-templates select="*" mode="M10" /></xsl:template><xsl:template match="text()" priority="-1" mode="M10" /><xsl:template match="@*|node()" priority="-2" mode="M10"><xsl:apply-templates select="*" mode="M10" /></xsl:template>

<!--PATTERN criteria-components-->


	<!--RULE -->
<xsl:template match="object[stereotype/text()='Criteria']" priority="1000" mode="M11"><svrl:fired-rule context="object[stereotype/text()='Criteria']" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron" /><xsl:variable name="id" select="id" /><xsl:variable name="thing-id" select="name" /><xsl:variable name="parent-id" select="parentId" /><xsl:variable name="parent-alias" select="../object[id=$parent-id]/alias" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="count(../object[name=$thing-id])=1" /><xsl:otherwise><svrl:failed-assert test="count(../object[name=$thing-id])=1" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text>
              <xsl:text /><xsl:value-of select="$id" /><xsl:text />
            </svrl:text> <svrl:diagnostic-reference diagnostic="MZ01"><xsl:attribute name="xml:lang">en</xsl:attribute>
The Criteria should have a unique ID.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="notes" /><xsl:otherwise><svrl:failed-assert test="notes" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMFLC15"><xsl:attribute name="xml:lang">en</xsl:attribute>
Criterion must have a textual description.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="string-length(normalize-space(notes/text())) &gt; 0" /><xsl:otherwise><svrl:failed-assert test="string-length(normalize-space(notes/text())) &gt; 0" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMFLC15"><xsl:attribute name="xml:lang">en</xsl:attribute>
Criterion must have a textual description.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose><xsl:variable name="start-string" select="substring(notes/text(), 1, 15)" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="contains($capital-letters, substring(notes/text(), 1,1))" /><xsl:otherwise><svrl:failed-assert test="contains($capital-letters, substring(notes/text(), 1,1))" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMTSR02"><xsl:attribute name="xml:lang">en</xsl:attribute>
The text (<xsl:text /><xsl:value-of select="$start-string" /><xsl:text />...) does not start with a capital letter.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="notes/text() = normalize-space(notes/text())" /><xsl:otherwise><svrl:failed-assert test="notes/text() = normalize-space(notes/text())" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMTSR03"><xsl:attribute name="xml:lang">en</xsl:attribute>
The text is not single spaced (there are multiple spaces).</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose><xsl:variable name="has-reference" select="contains(notes/text(), 'conform to function')" /><xsl:variable name="text-with-ref" select="substring-after(notes/text(), 'conform to function ')" /><xsl:variable name="ref-id" select="substring-before($text-with-ref, ' ')" />

		<!--REPORT -->
<xsl:if test="$has-reference and count(../object[alias/text()=$ref-id]) = 0"><svrl:successful-report test="$has-reference and count(../object[alias/text()=$ref-id]) = 0" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMFLC16"><xsl:attribute name="xml:lang">en</xsl:attribute>
Criterion references a non-existing function or criterion.</svrl:diagnostic-reference></svrl:successful-report></xsl:if>

		<!--REPORT -->
<xsl:if test="not($has-reference) and contains(notes/text(), '#')"><svrl:successful-report test="not($has-reference) and contains(notes/text(), '#')" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMFLC20"><xsl:attribute name="xml:lang">en</xsl:attribute>
Unidentified function reference, please check.</svrl:diagnostic-reference></svrl:successful-report></xsl:if><xsl:variable name="is-conditional" select="tag[@name='Conditional' and @value='Y']" />

		<!--REPORT -->
<xsl:if test="$is-conditional and not(starts-with(notes/text(), 'IF'))"><svrl:successful-report test="$is-conditional and not(starts-with(notes/text(), 'IF'))" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMCC001"><xsl:attribute name="xml:lang">en</xsl:attribute>
The Conditional Criteria is not structured correctly</svrl:diagnostic-reference></svrl:successful-report></xsl:if>

		<!--REPORT -->
<xsl:if test="not($is-conditional) and starts-with(notes/text(), 'IF')"><svrl:successful-report test="not($is-conditional) and starts-with(notes/text(), 'IF')" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMCC002"><xsl:attribute name="xml:lang">en</xsl:attribute>
The Criteria is structured as conditional but conditional is not indicated.</svrl:diagnostic-reference></svrl:successful-report></xsl:if><xsl:variable name="is-dependent" select="tag[@name='Dependent' and @value='Y']" />

		<!--REPORT -->
<xsl:if test="$is-dependent and not(contains(notes, 'according to scope of practice') or contains(notes, 'organizational policy') or contains(notes, 'jurisdictional law'))"><svrl:successful-report test="$is-dependent and not(contains(notes, 'according to scope of practice') or contains(notes, 'organizational policy') or contains(notes, 'jurisdictional law'))" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMDC001"><xsl:attribute name="xml:lang">en</xsl:attribute>
The dependent criteria is missing the required phrase 'according to scope of practice, organizational policy and/or jurisdictional law'.</svrl:diagnostic-reference></svrl:successful-report></xsl:if>

		<!--REPORT -->
<xsl:if test="not($is-dependent) and (contains(notes, 'according to scope of practice') or contains(notes, 'organizational policy') or contains(notes, 'jurisdictional law'))"><svrl:successful-report test="not($is-dependent) and (contains(notes, 'according to scope of practice') or contains(notes, 'organizational policy') or contains(notes, 'jurisdictional law'))" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMDC002"><xsl:attribute name="xml:lang">en</xsl:attribute>
The dependent criteria phrase is present in when the criteria is not designated as dependent.</svrl:diagnostic-reference></svrl:successful-report></xsl:if><xsl:variable name="option-value" select="tag[@name='Optionality']/@value" /><xsl:variable name="text-without-optionality" select="concat(substring-before(notes/text(), $option-value), substring-after(notes/text(), $option-value))" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="tag[@name='Optionality' and (@value='SHOULD' or @value='SHALL' or @value='MAY')]" /><xsl:otherwise><svrl:failed-assert test="tag[@name='Optionality' and (@value='SHOULD' or @value='SHALL' or @value='MAY')]" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMSVR01"><xsl:attribute name="xml:lang">en</xsl:attribute>
Statement does not include any of the required keywords.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="contains(notes/text(), 'SHOULD') or contains(notes/text(), 'SHALL') or contains(notes/text(), 'MAY')" /><xsl:otherwise><svrl:failed-assert test="contains(notes/text(), 'SHOULD') or contains(notes/text(), 'SHALL') or contains(notes/text(), 'MAY')" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMSVR07"><xsl:attribute name="xml:lang">en</xsl:attribute>
Criteria can only have one occurrence of SHALL, SHOULD or MAY unless in parentheses.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--REPORT -->
<xsl:if test="contains($text-without-optionality, 'SHOULD') or contains($text-without-optionality, 'SHALL') or contains($text-without-optionality, 'MAY')"><svrl:successful-report test="contains($text-without-optionality, 'SHOULD') or contains($text-without-optionality, 'SHALL') or contains($text-without-optionality, 'MAY')" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMSVR07"><xsl:attribute name="xml:lang">en</xsl:attribute>
Criteria can only have one occurrence of SHALL, SHOULD or MAY unless in parentheses.</svrl:diagnostic-reference></svrl:successful-report></xsl:if>

		<!--ASSERT -->
<xsl:choose><xsl:when test="contains(notes/text(), $option-value)" /><xsl:otherwise><svrl:failed-assert test="contains(notes/text(), $option-value)" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMSVR08"><xsl:attribute name="xml:lang">en</xsl:attribute>
Optionality attribute does not match the optionality in the text</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose><xsl:variable name="non-printing-text1" select="translate(notes/text(), $printing-characters, '')" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="string-length($non-printing-text1)=0" /><xsl:otherwise><svrl:failed-assert test="string-length($non-printing-text1)=0" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMTSR04"><xsl:attribute name="xml:lang">en</xsl:attribute>
The text contains non-printable characters [<xsl:text /><xsl:value-of select="$non-printing-text1" /><xsl:text />].</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose><xsl:variable name="local-alias" select="substring-after($thing-id, $parent-alias)" />

		<!--ASSERT -->
<xsl:choose><xsl:when test="starts-with($thing-id, $parent-alias)" /><xsl:otherwise><svrl:failed-assert test="starts-with($thing-id, $parent-alias)" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMFR006"><xsl:attribute name="xml:lang">en</xsl:attribute>
The Criteria ID must start with the ID of the parent Function.</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="starts-with($local-alias, '#')" /><xsl:otherwise><svrl:failed-assert test="starts-with($local-alias, '#')" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMFR007"><xsl:attribute name="xml:lang">en</xsl:attribute>
The Criteria ID can only have '#' and digits following the Parent ID and cannot contain [<xsl:text /><xsl:value-of select="$local-alias" /><xsl:text />].</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--ASSERT -->
<xsl:choose><xsl:when test="string-length(translate(substring($local-alias, 2), $numbers, '')) = 0" /><xsl:otherwise><svrl:failed-assert test="string-length(translate(substring($local-alias, 2), $numbers, '')) = 0" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMFR007"><xsl:attribute name="xml:lang">en</xsl:attribute>
The Criteria ID can only have '#' and digits following the Parent ID and cannot contain [<xsl:text /><xsl:value-of select="$local-alias" /><xsl:text />].</svrl:diagnostic-reference></svrl:failed-assert></xsl:otherwise></xsl:choose>

		<!--REPORT -->
<xsl:if test="id/text() = name/text()"><svrl:successful-report test="id/text() = name/text()" xmlns:svrl="http://purl.oclc.org/dsdl/svrl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:schold="http://www.ascc.net/xml/schematron"><xsl:attribute name="location"><xsl:apply-templates select="." mode="schematron-get-full-path" /></xsl:attribute><svrl:text><xsl:text /><xsl:value-of select="$id" /><xsl:text /></svrl:text> <svrl:diagnostic-reference diagnostic="FMID002"><xsl:attribute name="xml:lang">en</xsl:attribute>
The Conformance Criteria ID is the same as the visible ID.</svrl:diagnostic-reference></svrl:successful-report></xsl:if><xsl:apply-templates select="*" mode="M11" /></xsl:template><xsl:template match="text()" priority="-1" mode="M11" /><xsl:template match="@*|node()" priority="-2" mode="M11"><xsl:apply-templates select="*" mode="M11" /></xsl:template></xsl:stylesheet>