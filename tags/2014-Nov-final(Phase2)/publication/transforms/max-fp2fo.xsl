<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
    xmlns:mif="urn:hl7-org:v3/mif2" 
    xmlns:fo="http://www.w3.org/1999/XSL/Format" version="2.0">
    
    <xsl:output indent="yes" method="xml"/>
    <xsl:param name="iso-format" select="false()"/>
    <xsl:param name="include-tech-dates" select="false()"/>
    <xsl:param name="fill-columns">
        <xsl:choose>
            <xsl:when test="$iso-format">2</xsl:when>
            <xsl:otherwise>4</xsl:otherwise>
        </xsl:choose>
    </xsl:param>
    <xsl:param name="description-column-width">
        <xsl:choose>
            <xsl:when test="$iso-format">47</xsl:when>
            <xsl:otherwise>35</xsl:otherwise>
        </xsl:choose>
    </xsl:param>
    <xsl:param name="ballot-info-file" select="'ehrs-fm-ballot.mif'"/>
    <xsl:variable name="ballot-info" select="document($ballot-info-file)/mif:package"/>
    
    <xsl:template match="objects/object[stereotype/text()='HL7-FM-Profile']">
        <fo:root xmlns:fo="http://www.w3.org/1999/XSL/Format">
            <fo:layout-master-set>
                <fo:simple-page-master page-height="11.0in" page-width="8.5in" margin-top="0.25in" margin-bottom=".2in" margin-left="0.5in" margin-right="0.5in" master-name="FM-TitlePage">
                    <fo:region-body margin-top="1.5in" margin-bottom="0in"/>
                    <fo:region-before region-name="doc-heading" extent="1.75in"/>
                    <fo:region-after region-name="doc-footer" extent="0.25in"/>
                </fo:simple-page-master>
                <fo:simple-page-master page-height="11.0in" page-width="8.5in" margin-top="0.25in" margin-bottom=".2in" margin-left="0.5in" margin-right="0.5in" master-name="FM-Body">
                    <fo:region-body margin-top="0.25in" margin-bottom="0.15in"/>
                    <fo:region-before region-name="right-heading" extent="0.25in"/>
                    <fo:region-after extent="0.1in"/>
                </fo:simple-page-master>
            </fo:layout-master-set>
            <fo:bookmark-tree>
                <fo:bookmark internal-destination="toc">
                    <fo:bookmark-title>Table of Contents</fo:bookmark-title>
                    <fo:bookmark internal-destination="fn-list-components">
                        <fo:bookmark-title>Functional Profile Components</fo:bookmark-title>
                    </fo:bookmark>
                    <xsl:for-each select="following-sibling::object[stereotype/text()='Section']">
                        <xsl:call-template name="section-bookmark">
                            <xsl:with-param name="order" select="position()"/>
                            <xsl:with-param name="is-top" select="'t'"/>
                        </xsl:call-template>
                    </xsl:for-each>
                </fo:bookmark>
            </fo:bookmark-tree>
            <fo:page-sequence master-reference="FM-TitlePage" force-page-count="no-force">
                <fo:static-content flow-name="doc-heading">
                    <fo:block font-family="Arial" font-size="1.4em" text-align="right">
                        <xsl:value-of select="$ballot-info/@title"/>
                    </fo:block>
                    <xsl:if test="not($iso-format)">
                        <fo:block font-size="1.1em" text-align="right" >
                            <fo:block><xsl:value-of select="modified"/></fo:block>
                        </fo:block>
                    </xsl:if>
                    <xsl:if test="$ballot-info/mif:historyItem/mif:description/mif:text">
                        <fo:block font-size="0.85em" text-align="right" >
                            <xsl:for-each select="$ballot-info/mif:historyItem/mif:description/mif:text/mif:p">
                                <fo:block><xsl:value-of select="."/></fo:block>
                            </xsl:for-each>
                        </fo:block>
                    </xsl:if>
                </fo:static-content>
                <fo:static-content flow-name="doc-footer">
                    <fo:block font-size="8pt" font-style="italic" border-top="groove 1pt gray" border-bottom="groove 1pt gray">
                        <xsl:for-each select="$ballot-info/mif:header/mif:legalese/mif:notation/mif:text">
                            <fo:block><xsl:value-of select="."/></fo:block>
                        </xsl:for-each>
                    </fo:block>
                </fo:static-content>
                <fo:flow flow-name="xsl-region-body">
                    <fo:block text-align="right">
                        <xsl:for-each select="$ballot-info//mif:contributor">
                            <xsl:call-template name="author-item"/>
                        </xsl:for-each>
                    </fo:block>
                </fo:flow>
            </fo:page-sequence>

            <fo:page-sequence master-reference="FM-Body" break-before="page" format="i">
                <fo:static-content flow-name="xsl-region-after">
                    <fo:block font-family="Arial" font-size="8pt" text-align="center" border-top="dotted 0.5pt black" font-style="italic">
                        - <fo:page-number/> -
                    </fo:block>
                </fo:static-content>
                <fo:static-content flow-name="right-heading">
                    <fo:block font-family="Arial" font-size="10pt" text-align="right" border-bottom="solid 0.1pt blue">
                        <xsl:value-of select="$ballot-info/@title"/>
                    </fo:block>
                </fo:static-content>
                <fo:flow flow-name="xsl-region-body" font-size="10pt">
                    <fo:block font-family="Arial">
                        <fo:block border-style="inset" border-width="thin" border-color="black" background-color="#EEEEEE" padding="1em">
                            <fo:block id="toc" font-size="20pt" font-weight="bold" space-before="14pt" space-after="5pt" keep-with-next.within-page="always">Table of Contents</fo:block>
                            <xsl:if test="$ballot-info/mif:annotations/mif:documentation/mif:description">
                                <fo:block text-align-last="justify" margin-left="0em" space-before="5pt" font-size="1em" font-weight="700">
                                    <fo:basic-link internal-destination="notes-to-balloters">
                                        <fo:inline font-style="italic">Notes to Balloters</fo:inline>
                                    </fo:basic-link>
                                    <fo:leader leader-pattern="dots"/>
                                    <fo:inline font-style="italic">
                                        <fo:page-number-citation ref-id="notes-to-balloters"/>
                                    </fo:inline>
                                </fo:block>
                            </xsl:if>
                            <fo:block text-align-last="justify" margin-left="0em" space-before="5pt" font-size="1em" font-weight="700">
                                <fo:basic-link internal-destination="fn-list-components">
                                    <fo:inline font-style="italic">Functional Profile Components</fo:inline>
                                </fo:basic-link>
                                <fo:leader leader-pattern="dots"/>
                                <fo:inline font-style="italic">
                                    <fo:page-number-citation ref-id="fn-list-components"/>
                                </fo:inline>
                            </fo:block>
                            <xsl:for-each select="following-sibling::object[stereotype/text()='Section']">
                                <xsl:call-template name="section-toc">
                                    <xsl:with-param name="order" select="position()"/>
                                    <xsl:with-param name="is-top" select="'t'"/>
                                    <xsl:with-param name="level" select="0"/>
                                </xsl:call-template>
                            </xsl:for-each>
                        </fo:block>
                    </fo:block>
                    <xsl:if test="$ballot-info/mif:annotations/mif:documentation/mif:description">
                        <fo:block break-before="page">
                            <fo:block font-size="1.1em" font-weight="bold" id="notes-to-balloters" space-after=".6em">Notes to Balloters</fo:block>
                            <xsl:for-each select="$ballot-info/mif:annotations/mif:documentation/mif:description/mif:text/mif:p">
                                <fo:block><xsl:value-of select="."/></fo:block>
                            </xsl:for-each>
                        </fo:block>                    
                    </xsl:if>
                    <fo:block break-before="page">
                        <fo:block font-size="1.1em" font-weight="bold" id="fn-list-components">Functional Profile Components</fo:block>
                        <fo:block>
                            <xsl:call-template name="component-description"/>
                        </fo:block>
                    </fo:block>                    
                </fo:flow>
            </fo:page-sequence>
            
            <fo:page-sequence master-reference="FM-Body" initial-page-number="1" break-before="page">
                <fo:static-content flow-name="xsl-region-after">
                    <fo:block font-family="Times" font-size="8pt" text-align="center" border-top="dotted 0.5pt black" font-style="italic">
                         Page: <fo:page-number/>
                    </fo:block>
                </fo:static-content>
                <fo:static-content flow-name="right-heading">
                    <fo:block font-family="Times" font-size="10pt" text-align="right" border-bottom="solid 0.1pt blue">
                        <xsl:value-of select="$ballot-info/@title"/>
                    </fo:block>
                </fo:static-content>
                <fo:flow flow-name="xsl-region-body" font-size="10pt">
                    <xsl:for-each select="following-sibling::object[stereotype/text()='Section']">
                        <xsl:call-template name="section-wrapper">
                            <xsl:with-param name="order" select="position()"/>
                        </xsl:call-template>
                    </xsl:for-each>
                </fo:flow>
            </fo:page-sequence>
        </fo:root>
    </xsl:template>
    
    <xsl:template name="author-item">
        <fo:block margin-bottom="1.4em">
            <fo:block font-weight="bold" font-size="1.2em"><xsl:value-of select="mif:name/@name"/></fo:block>
            <fo:block font-size=".7em"><xsl:value-of select="mif:role"/></fo:block>
            <fo:block font-size=".7em"><xsl:value-of select="mif:affiliation"/></fo:block>
        </fo:block>
    </xsl:template>
    
    <xsl:template name="section-wrapper">
        <xsl:param name="order"/>
        <xsl:param name="object-id" select="id"/>
        <xsl:param name="overview" select="substring-before(substring-after(notes, '$OV$'), '$EX$')"/>
        <fo:block>
            <xsl:attribute name="id">
                <xsl:value-of select="alias/text()"/>
            </xsl:attribute>
            <xsl:attribute name="break-before">
                <xsl:choose>
                    <xsl:when test="$order=1">auto</xsl:when>
                    <xsl:otherwise>page</xsl:otherwise>
                </xsl:choose>
            </xsl:attribute>
            <fo:block font-size="1.2em" font-weight="bold" space-after="1.3em">
                <xsl:value-of select="concat($order, '. ', name/text())"/> Section
            </fo:block>
            <fo:block font-weight="bold" space-after="1em">Section Overview</fo:block>
            <fo:block space-after="1em" text-align="justify"><xsl:value-of select="$overview"></xsl:value-of></fo:block>
            <fo:table font-size=".8em" border-collapse="collapse">
                <fo:table-column column-number="1" column-width="15em"/>
                <fo:table-column column-number="2">
                    <xsl:attribute name="column-width">
                        <xsl:value-of select="concat($description-column-width,'em')"/>
                    </xsl:attribute>
                </fo:table-column>
                <xsl:choose>
                    <xsl:when test="$iso-format">
                        <fo:table-column column-number="3" column-width="6em"/>
                    </xsl:when>
                    <xsl:otherwise>
                        <fo:table-column column-number="3" column-width="6em"/>
                        <fo:table-column column-number="4" column-width="6em"/>
                        <fo:table-column column-number="5" column-width="6em"/>
                    </xsl:otherwise>
                </xsl:choose>
                <fo:table-header>
                    <fo:table-row border="solid 0.5pt black" display-align="center" font-weight="600" background-color="#a151e7">
                        <fo:table-cell padding-left=".5em" border-right="solid 0.5pt white">
                            <fo:block font-size="0.8em">Section/Id#:</fo:block>
                            <fo:block font-size="0.8em">Type:</fo:block>
                        </fo:table-cell>
                        <fo:table-cell padding-left="1.2em" border-right="solid 0.5pt white">
                            <fo:block font-size="1.2em">Header/Function Name</fo:block>
                            <fo:block font-size="0.8em">Conformance Criteria</fo:block>
                        </fo:table-cell>
                        <xsl:if test="not($iso-format)">
                            <fo:table-cell text-align="center" border-right="solid 0.5pt white">
                                <fo:block font-size="0.8em">Reference</fo:block>
                            </fo:table-cell>
                            <fo:table-cell text-align="center" border-right="solid 0.5pt white">
                                <fo:block font-size="0.8em">Chg Ind</fo:block>
                            </fo:table-cell>
                        </xsl:if>
                        <fo:table-cell text-align="center">
                            <fo:block font-size="0.8em">Priority</fo:block>
                        </fo:table-cell>
                    </fo:table-row>
                </fo:table-header>
                <fo:table-body>
                    <xsl:for-each select="following-sibling::object[parentId/text()=$object-id]">
                        <xsl:call-template name="function-output">
                            <xsl:with-param name="sect-no" select="$order"/>
                            <xsl:with-param name="level-no" select="1"/>
                        </xsl:call-template>
                    </xsl:for-each>
                </fo:table-body>
            </fo:table>
        </fo:block>
    </xsl:template>
    
    <xsl:template name="function-output">
        <xsl:param name="sect-no"/>
        <xsl:param name="level-no"/>
        <xsl:param name="object-id" select="id"/>
        <xsl:param name="statement" select="substring-before(substring-after(notes, '$ST$'), '$DE$')"/>
        <xsl:param name="description" select="substring-before(substring-after(notes, '$DE$'), '$EX$')"/>
        <xsl:param name="plain-name" select="substring-after(name, alias)"/>

        <fo:table-row border="solid 0.5pt black" padding-left=".5em" keep-with-next.within-page="always" border-top="solid 1.25pt black">
            <fo:table-cell border-right="solid 0.5pt black" keep-together="always">
                <fo:block border-bottom="solid 0.2 black" space-before=".7em" margin-left=".5em" keep-with-next.within-page="always">
                    <xsl:attribute name="id">
                        <xsl:value-of select="alias"/>
                    </xsl:attribute>
                    <xsl:value-of select="alias"/>
                </fo:block>
                <fo:block margin-left=".5em">
                    <xsl:value-of select="stereotype"/>
                </fo:block>
            </fo:table-cell>
            <fo:table-cell border-right="solid 0.5pt black" display-align="center">
                <fo:block display-align="center" text-align="center" font-size="1.2em" font-weight="bolder">
                    <xsl:value-of select="$plain-name"/>
                </fo:block>
            </fo:table-cell>
            <xsl:if test="not($iso-format)">
                <fo:table-cell border-right="solid 0.5pt black" display-align="center">
                    <fo:block text-align="center">
                        <xsl:call-template name="get-reference"/>
                    </fo:block>
                </fo:table-cell>
                <fo:table-cell border-right="solid 0.5pt black" display-align="center">
                    <fo:block display-align="center" text-align="center">
                        <xsl:call-template name="get-change-indicator"/>
                    </fo:block>
                </fo:table-cell>
            </xsl:if>
            <fo:table-cell border-right="solid 0.5pt black" display-align="center">
                <fo:block display-align="center" text-align="center">
                    <xsl:value-of select="tag[@name='Priority']/@value"/>
                </fo:block>
            </fo:table-cell>
        </fo:table-row>
        
        <fo:table-row border="solid 0.5pt black">
            <fo:table-cell>
                <xsl:attribute name="number-columns-spanned">
                    <xsl:value-of select="$fill-columns + 1"/>
                </xsl:attribute>
                <fo:block padding-left=".5em" space-before=".7em" text-align="justify" margin-left="4em" margin-right="4em" keep-together.within-page="always">
                    <fo:block space-after=".7em" margin-top=".5em">
                        <fo:inline font-weight="bold">Statement: </fo:inline>
                        <xsl:call-template name="text-with-link">
                            <xsl:with-param name="the-text" select="$statement"/>
                        </xsl:call-template>
                        <xsl:value-of select="$statement"/>
                    </fo:block>
                    <fo:block margin-bottom=".5em">
                        <xsl:call-template name="description-output">
                            <xsl:with-param name="is-first" select="true()"/>
                            <xsl:with-param name="full-text" select="$description"></xsl:with-param>
                        </xsl:call-template>
                    </fo:block>
                    <xsl:if test="tag/@name='ExternalReference'">
                        <xsl:call-template name="produce-external-references"/>
                    </xsl:if>
                </fo:block>
            </fo:table-cell>
        </fo:table-row>
        
        <xsl:for-each select="following-sibling::object[parentId/text()=$object-id and stereotype/text()='Criteria']">
            <xsl:call-template name="criteria-output"/>
        </xsl:for-each>
        
        <xsl:for-each select="following-sibling::object[parentId/text()=$object-id and not(stereotype/text()='Criteria')]">
            <xsl:call-template name="function-output">
                <xsl:with-param name="sect-no" select="$sect-no"/>
                <xsl:with-param name="level-no" select="$level-no + 1"/>
            </xsl:call-template>
        </xsl:for-each>
    </xsl:template>
    
    <xsl:template name="description-output">
        <xsl:param name="is-first" select="false()"/>
        <xsl:param name="full-text"/>
        <xsl:variable name="para-text">
            <xsl:choose>
                <xsl:when test="contains($full-text,'&#xA;')">
                    <xsl:value-of select="substring-before($full-text, '&#xA;')"/>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:value-of select="$full-text"/>
                </xsl:otherwise>
            </xsl:choose>
        </xsl:variable>
        <xsl:variable name="remain-text" select="substring-after($full-text, '&#xA;')"/>
        
        <fo:block>
            <xsl:if test="not($is-first)">
                <xsl:attribute name="space-before">.4em</xsl:attribute>
            </xsl:if>
            <xsl:if test="$is-first">
                <fo:inline font-weight="bold">Description: </fo:inline>
            </xsl:if>
            <xsl:call-template name="text-with-link">
                <xsl:with-param name="the-text" select="$para-text"/>
            </xsl:call-template>
        </fo:block>
        
        <xsl:if test="string-length($remain-text) > 0">
            <xsl:call-template name="description-output">
                <xsl:with-param name="full-text" select="$remain-text"/>
            </xsl:call-template>
        </xsl:if>
    </xsl:template>
    
    <xsl:template name="criteria-output">
        <fo:table-row border="solid 0.5pt black" margin-bottom=".4em">
            <fo:table-cell border-right="solid 0.5pt black" padding-left=".4em" padding-right=".4em" text-align="justify" number-columns-spanned="2">
                <fo:list-block provisional-label-separation=".7em" provisional-distance-between-starts="2.4em" margin-left="4em">            
                    <fo:list-item space-after=".7em" margin-top=".3em">
                        <fo:list-item-label end-indent="label-end()">
                            <fo:block font-weight="bold" text-align="right">
                                <xsl:value-of select="concat(number(substring-after(name, '#')), '.')"/>
                            </fo:block>
                        </fo:list-item-label>
                        <fo:list-item-body start-indent="body-start()">
                            <fo:block>
                                <xsl:call-template name="get-criteria-text"/>
                            </fo:block>
                        </fo:list-item-body>
                    </fo:list-item>
                </fo:list-block>
            </fo:table-cell>
            <xsl:if test="not($iso-format)">
                <fo:table-cell border-right="solid 0.5pt black" display-align="center">
                    <fo:block text-align="center">
                        <xsl:call-template name="get-functional-reference"/>
                    </fo:block>
                </fo:table-cell>
                <fo:table-cell border-right="solid 0.5pt black" display-align="center">
                    <fo:block text-align="center">
                        <xsl:call-template name="get-change-indicator"/>
                    </fo:block>
                </fo:table-cell>
            </xsl:if>
            <fo:table-cell display-align="center">
                <fo:block text-align="center">
                    <xsl:value-of select="tag[@name='Priority']/@value"/>
                </fo:block>
            </fo:table-cell>
        </fo:table-row>
    </xsl:template>
    
    <xsl:template name="produce-external-references">
        <fo:list-block provisional-label-separation="1em" provisional-distance-between-starts="10em">            
            <fo:list-item space-after=".7em">
                <fo:list-item-label end-indent="label-end()"><fo:block font-weight="bold">External References:</fo:block></fo:list-item-label>
                <fo:list-item-body start-indent="body-start()">
                    <xsl:for-each select="tag[@name='ExternalReference']">
                        <xsl:call-template name="format-external-reference"/>
                    </xsl:for-each>
                </fo:list-item-body>
            </fo:list-item>
        </fo:list-block>
    </xsl:template>

    <xsl:template name="format-external-reference">
        <xsl:param name="text-area" select="substring-before(@value, '$$URL$$')"/>
        <xsl:param name="text-value" select="substring-after($text-area, 'TEXT$$')"/>
        <xsl:param name="url-value" select="substring-after(@value, '$$URL$$')"/>
        
        <xsl:variable name="quote">'</xsl:variable>
        
        <xsl:choose>
            <xsl:when test="string-length($url-value) > 0">
                <fo:block>
                    <fo:basic-link color="blue" text-decoration="underline">
                        <xsl:attribute name="external-destination">
                            <xsl:value-of select="concat('url(', $quote, $url-value, $quote, ')')"/>
                        </xsl:attribute>
                        <xsl:value-of select="$text-value" disable-output-escaping="yes"/>
                    </fo:basic-link>
                </fo:block>
            </xsl:when>
            <xsl:otherwise>                
                <fo:block><xsl:value-of select="$text-value" disable-output-escaping="yes"/></fo:block>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    
    <xsl:template name="get-reference">
        <xsl:if test="tag/@name='Reference.Alias'">
            <xsl:value-of select="tag[@name='Reference.FunctionID']/@value"/>
        </xsl:if>
    </xsl:template>
    
    <xsl:template name="get-change-indicator">
        <xsl:choose>
            <xsl:when test="tag/@name='Reference.ChangeIndicator'">
                <xsl:value-of select="tag[@name='Reference.ChangeIndicator']/@value"/>
            </xsl:when>
            <xsl:otherwise>NC</xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    
    <xsl:template name="get-criteria-text">
        <xsl:call-template name="text-with-link">
            <xsl:with-param name="the-text" select="notes"/>
        </xsl:call-template>
    </xsl:template>

    <xsl:template name="get-functional-reference">
        <xsl:param name="func-ref">
            <xsl:choose>
                <xsl:when test="tag/@name='Reference.FunctionID'">
                    <xsl:value-of select="tag[@name='Reference.FunctionID']/@value"/>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:value-of select="' '"/>
                </xsl:otherwise>
            </xsl:choose>
        </xsl:param>
        <xsl:param name="crit-ref">
            <xsl:if test="tag/@name='Reference.CriteriaID'">
                <xsl:value-of select="concat('#', tag[@name='Reference.CriteriaID']/@value)"/>
            </xsl:if>
        </xsl:param>
        
        <xsl:value-of select="concat($func-ref, $crit-ref)"/>
    </xsl:template>
    
    <xsl:template name="get-criteria-row">
        <xsl:if test="tag/@name='Row'">
            <xsl:value-of select="tag[@name='Row']/@value"/>
        </xsl:if>
    </xsl:template>
    
    <xsl:template name="text-with-link">
        <xsl:param name="the-text"/>
        
        <xsl:choose>
            <xsl:when test="contains($the-text,' [[')">
                <xsl:variable name="pre-text" select="substring-before($the-text, '[[')"/>
                <xsl:variable name="working-text" select="substring-after($the-text, '[[')"/>
                <xsl:variable name="function-ref" select="substring-before($working-text, ']]')"/>
                <xsl:variable name="post-text" select="substring-after($working-text, ']]')"/>
                
                <xsl:value-of select="$pre-text"/>
                <fo:basic-link>
                    <xsl:attribute name="internal-destination">
                        <xsl:value-of select="$function-ref"/>
                    </xsl:attribute>
                    <fo:inline  text-decoration="underline" color="blue">
                        <xsl:value-of select="$function-ref"/>
                    </fo:inline>
                </fo:basic-link>
                <xsl:call-template name="text-with-link">
                    <xsl:with-param name="the-text" select="$post-text"/>
                </xsl:call-template>
            </xsl:when>
            <xsl:otherwise>
                <xsl:value-of select="$the-text"/>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    
    <xsl:template name="section-toc">
        <xsl:param name="order"/>
        <xsl:param name="is-top"/>
        <xsl:param name="level"/>
        <xsl:param name="object-id" select="id"/>
        <xsl:param name="section-title">
            <xsl:choose>
                <xsl:when test="$is-top = 't'">
                    <xsl:value-of select="name"/>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:value-of select="alias"/>
                </xsl:otherwise>
            </xsl:choose>
        </xsl:param>

        <fo:block text-align-last="justify" margin-left="0em" space-before="5pt" font-size="1em" font-weight="700">
            <fo:basic-link>
                <xsl:attribute name="internal-destination">
                    <xsl:value-of select="alias"/>
                </xsl:attribute>
                <xsl:choose>
                    <xsl:when test="$is-top = 't'">
                        <xsl:value-of select="concat($order, '. ', $section-title, ' (', alias, ')')"/>
                    </xsl:when>
                    <xsl:otherwise>
                        <xsl:value-of select="concat($order, '. ', $section-title)"/>
                    </xsl:otherwise>
                </xsl:choose>
            </fo:basic-link>
            <fo:leader leader-pattern="dots"/>
            <fo:page-number-citation>
                <xsl:attribute name="ref-id">
                    <xsl:value-of select="alias"/>
                </xsl:attribute>
            </fo:page-number-citation>
        </fo:block>

        <xsl:for-each select="following-sibling::object[parentId/text()=$object-id]">
            <xsl:call-template name="function-toc"/>
        </xsl:for-each>
    </xsl:template>
    
    <xsl:template name="function-toc">
        <xsl:param name="nav-text" select="substring-after(name, alias)"/>

        <fo:block text-align-last="justify" margin-left="3em" space-before="5pt" font-size="1em" font-weight="400">
            <fo:basic-link>
                <xsl:attribute name="internal-destination">
                    <xsl:value-of select="alias"/>
                </xsl:attribute>
                <xsl:value-of select="name"/>
            </fo:basic-link>
            <fo:leader leader-pattern="dots"/>
            <fo:page-number-citation>
                <xsl:attribute name="ref-id">
                    <xsl:value-of select="alias"/>
                </xsl:attribute>
            </fo:page-number-citation>
        </fo:block>
    </xsl:template>
    
    <xsl:template name="section-bookmark">
        <xsl:param name="order"/>
        <xsl:param name="is-top"/>
        <xsl:param name="object-id" select="id"/>
        <xsl:param name="section-title">
            <xsl:choose>
                <xsl:when test="$is-top = 't'">
                    <xsl:value-of select="name"/>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:value-of select="alias"/>
                </xsl:otherwise>
            </xsl:choose>
        </xsl:param>
        
        <fo:bookmark>
            <xsl:attribute name="internal-destination">
                <xsl:value-of select="alias"/>
            </xsl:attribute>
            <fo:bookmark-title>
                <xsl:choose>
                    <xsl:when test="$is-top = 't'">
                        <xsl:value-of select="concat($section-title, ' (', alias, ')')"/>
                    </xsl:when>
                    <xsl:otherwise>
                        <xsl:value-of select="$section-title"/>
                    </xsl:otherwise>
                </xsl:choose>
            </fo:bookmark-title>
            <xsl:for-each select="following-sibling::object[parentId/text()=$object-id]">
                <xsl:call-template name="function-bookmark"/>
            </xsl:for-each>
        </fo:bookmark>
    </xsl:template>

    <xsl:template name="function-bookmark">
        <fo:bookmark>
            <xsl:attribute name="internal-destination">
                <xsl:value-of select="alias"/>
            </xsl:attribute>
            <fo:bookmark-title>
                <xsl:value-of select="name"/>
            </fo:bookmark-title>
        </fo:bookmark>
    </xsl:template>
    
    <xsl:template name="component-description">
        <fo:block space-before="1em" space-after="1em">The Function List includes the following components:</fo:block>
        <fo:list-block provisional-label-separation="1em" provisional-distance-between-starts="10em">            
            <fo:list-item space-after=".7em">
                <fo:list-item-label end-indent="label-end()"><fo:block font-weight="bold">Function ID # (Normative)</fo:block></fo:list-item-label>
                <fo:list-item-body start-indent="body-start()"><fo:block>This is the unique identifier of a function in the Function List (e.g. CP.1.1) and should be used to uniquely identify the function when referencing functions.  The Function ID also serves to identify the section within which the function exists (CP = Care Provision Section) and the hierarchy or relationship between functions (CP.1.1 is a sibling to CP.1.2, parent of CP.1.1.1 and child of CP.1).  In many cases the parent is fully expressed by the children.</fo:block></fo:list-item-body>
            </fo:list-item>
            <fo:list-item space-after=".7em">
                <fo:list-item-label end-indent="label-end()"><fo:block font-weight="bold">Function Type (Reference)</fo:block></fo:list-item-label>
                <fo:list-item-body start-indent="body-start()"><fo:block>Indication of the line item as being a header (H) or function (F) or conformance criteria.</fo:block></fo:list-item-body>
            </fo:list-item>
            <fo:list-item space-after=".7em">
                <fo:list-item-label end-indent="label-end()"><fo:block font-weight="bold">Header/Function Name (Normative)</fo:block></fo:list-item-label>
                <fo:list-item-body start-indent="body-start()"><fo:block>This is the name of the Function and whilst expected to be unique within the Function List; it is not recommended to be used to identify the function without being accompanied by the Function ID.<fo:block>Example: Manage Medication List</fo:block></fo:block></fo:list-item-body>
            </fo:list-item>
            <fo:list-item space-after=".7em">
                <fo:list-item-label end-indent="label-end()"><fo:block font-weight="bold">Function Statement (Normative)</fo:block></fo:list-item-label>
                <fo:list-item-body start-indent="body-start()">
                    <fo:block>This is a brief statement of the purpose of this function.  Whist not restricted to the use of structured language that is used in the Conformance Criteria (see below); the Statement should clearly identify the purpose and scope of the function.</fo:block> 
                    <fo:block>Example: Create and maintain patient-specific medication lists.</fo:block>
                </fo:list-item-body>
            </fo:list-item>
            <fo:list-item space-after=".7em">
                <fo:list-item-label end-indent="label-end()"><fo:block font-weight="bold">Description (Reference)</fo:block></fo:list-item-label>
                <fo:list-item-body start-indent="body-start()">
                    <fo:block>This is a more detailed description of the function, including examples if needed.</fo:block>
                    <fo:block>Example: Medication lists are managed over time, whether over the course of a visit or stay, or the lifetime of a patient. All pertinent dates, including medication start, modification, and end dates are stored. The entire medication history for any medication, including alternative supplements and herbal medications, is viewable. Medication lists are not limited to medication orders recorded by providers, but may include, for example, pharmacy dispense/supply records, patient-reported medications and additional information such as age specific dosage.</fo:block>
                </fo:list-item-body>
            </fo:list-item>
            <fo:list-item space-after=".7em">
                <fo:list-item-label end-indent="label-end()">
                    <fo:block font-weight="bold">Conformance Criteria (Normative) </fo:block>
                </fo:list-item-label>
                <fo:list-item-body start-indent="body-start()">
                    <fo:block>Each function in the Function List includes one or more Conformance Criteria.  A Conformance Criteria, which exists as normative language in this standard, defines the requirements for conforming to the function.  The language used to express a conformance criterion is highly structured with standardized components with set meanings.  The structured language used to define conformance clauses in the Function List are defined in the Glossary (Chapter 4).</fo:block>
                </fo:list-item-body>
            </fo:list-item>
            <xsl:if test="not($iso-format)">
                <fo:list-item space-after=".7em">
                    <fo:list-item-label end-indent="label-end()">
                        <fo:block font-weight="bold">Reference (Reference)</fo:block>
                    </fo:list-item-label>
                    <fo:list-item-body start-indent="body-start()">
                        <fo:block>Reference to the Functional Model or Functional Profile the current Functional Profile was developed against.</fo:block>
                    </fo:list-item-body>
                </fo:list-item>
                <fo:list-item space-after=".7em">
                    <fo:list-item-label end-indent="label-end()">
                        <fo:block font-weight="bold">Change Indicator</fo:block>
                    </fo:list-item-label>
                    <fo:list-item-body start-indent="body-start()">
                        <fo:block>The change indicator shows the change from previous versions.  This will be valued as follows:</fo:block>
                        <fo:list-block>
                            <fo:list-item>
                                <fo:list-item-label end-indent="label-end()"><fo:block/></fo:list-item-label>
                                <fo:list-item-body start-indent="body-start()"><fo:block>C - Changed</fo:block></fo:list-item-body></fo:list-item>
                            <fo:list-item>
                                <fo:list-item-label end-indent="label-end()"><fo:block/></fo:list-item-label>
                                <fo:list-item-body start-indent="body-start()"><fo:block>D - Deleted</fo:block></fo:list-item-body></fo:list-item>
                            <fo:list-item>
                                <fo:list-item-label end-indent="label-end()"><fo:block/></fo:list-item-label>
                                <fo:list-item-body start-indent="body-start()"><fo:block>N - New</fo:block></fo:list-item-body></fo:list-item>
                            <fo:list-item>
                                <fo:list-item-label end-indent="label-end()"><fo:block/></fo:list-item-label>
                                <fo:list-item-body start-indent="body-start()"><fo:block>NC - No Change</fo:block></fo:list-item-body></fo:list-item>
                            <fo:list-item>
                                <fo:list-item-label end-indent="label-end()"><fo:block/></fo:list-item-label>
                                <fo:list-item-body start-indent="body-start()"><fo:block>DEP - Deprecated</fo:block></fo:list-item-body></fo:list-item>
                        </fo:list-block>
                    </fo:list-item-body>
                </fo:list-item>
            </xsl:if>
            <fo:list-item>
                <fo:list-item-label end-indent="label-end()">
                    <fo:block font-weight="bold">Priority</fo:block>
                </fo:list-item-label>
                <fo:list-item-body start-indent="body-start()">
                    <fo:block>The priority for the implementation of the item. This will be valued as follows:</fo:block>
                    <fo:list-block>
                        <fo:list-item>
                            <fo:list-item-label end-indent="label-end()"><fo:block/></fo:list-item-label>
                            <fo:list-item-body start-indent="body-start()"><fo:block>EN - Essential Now</fo:block></fo:list-item-body></fo:list-item>
                        <fo:list-item>
                            <fo:list-item-label end-indent="label-end()"><fo:block/></fo:list-item-label>
                            <fo:list-item-body start-indent="body-start()"><fo:block>EF  - Essential Future</fo:block></fo:list-item-body></fo:list-item>
                        <fo:list-item>
                            <fo:list-item-label end-indent="label-end()"><fo:block/></fo:list-item-label>
                            <fo:list-item-body start-indent="body-start()"><fo:block>O   - Optional</fo:block></fo:list-item-body></fo:list-item>
                    </fo:list-block>
                </fo:list-item-body>
            </fo:list-item>
        </fo:list-block>
    </xsl:template>
    
    <xsl:template match="object"/>
    
    <xsl:template match="relationship"/>
</xsl:stylesheet>