<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
    xmlns:mif="urn:hl7-org:v3/mif2" version="2.0"
    exclude-result-prefixes="mif">
    
    <xsl:output doctype-system="html"/>
    <xsl:param name="ballot-info-file" select="'fp-ballot.mif'"/>
    <xsl:variable name="ballot-info" select="document($ballot-info-file)/mif:package"/>
    <xsl:variable name="max-sub-levels" select="number(4)"/>
    
    <xsl:template match="objects/object[stereotype/text()='HL7-FM-Profile']">
        <html>
            <head>
                <title>
                    <xsl:value-of select="$ballot-info/@title"/>
                </title>
                <link href="functional-model.css" media="screen" rel="stylesheet" type="text/css"/>
            </head>
            <body>
                <header>
                    <div id="title-page">
                        <h1><xsl:value-of select="$ballot-info/@title"/></h1>
                        <div id="extract-date">
                            <xsl:value-of select="modified"/>
                        </div>
                        <xsl:if test="$ballot-info/mif:historyItem/mif:description/mif:text">
                            <div class="sub-title">
                                <xsl:for-each select="$ballot-info/mif:historyItem/mif:description/mif:text/mif:p">
                                    <h3><xsl:value-of select="."/></h3>
                                </xsl:for-each>
                            </div>
                        </xsl:if>
                        <div class="authors">
                            <xsl:for-each select="$ballot-info//mif:contributor">
                                <xsl:call-template name="author-item"/>
                            </xsl:for-each>
                        </div>
                        <div id="copyright">
                            <xsl:for-each select="$ballot-info/mif:header/mif:legalese/mif:notation/mif:text">
                                <p><xsl:value-of select="."/></p>
                            </xsl:for-each>
                        </div>
                    </div>
                    <xsl:if test="$ballot-info/mif:annotations/mif:documentation/mif:description">
                        <div id="">
                            <h2>Notes to Balloters</h2>
                            <xsl:for-each select="$ballot-info/mif:annotations/mif:documentation/mif:description/mif:text/mif:p">
                                <p><xsl:value-of select="."/></p>
                            </xsl:for-each>
                        </div>
                    </xsl:if>
                    <div id="toc">
                        <h2>Table of Contents</h2>
                        <ul id="toc-list" class="function-list-toc">
                            <xsl:for-each select="following-sibling::object[stereotype/text()='Section']">
                                <xsl:call-template name="section-nav">
                                    <xsl:with-param name="order" select="position()"/>
                                    <xsl:with-param name="is-top" select="'t'"/>
                                </xsl:call-template>
                            </xsl:for-each>
                        </ul>
                    </div>
                </header>
                <div class="content">
                    <section id="fm-component-description">
                        <h2>EHR-S Functional Profile Components</h2>
                        <xsl:call-template name="component-description"/>
                    </section>
                    <xsl:for-each select="following-sibling::object[stereotype/text()='Section']">
                        <xsl:call-template name="section-wrapper">
                            <xsl:with-param name="order" select="position()"/>
                        </xsl:call-template>
                    </xsl:for-each>
                </div>
                <footer>
                    <ul id="bottom-nav">
                        <xsl:for-each select="following-sibling::object[stereotype/text()='Section']">
                            <xsl:call-template name="section-nav">
                                <xsl:with-param name="order" select="position()"/>
                                <xsl:with-param name="is-top" select="'f'"/>
                            </xsl:call-template>
                        </xsl:for-each>
                    </ul>
                </footer>
            </body>
        </html>
    </xsl:template>
    
    <xsl:template name="author-item">
        <div>
            <p class="author-name"><xsl:value-of select="mif:name/@name"/></p>
            <p class="author-detail"><xsl:value-of select="mif:role"/></p>
            <p class="author-detail"><xsl:value-of select="mif:affiliation"/></p>
        </div>
    </xsl:template>
    
    <xsl:template name="section-wrapper">
        <xsl:param name="order"/>
        <xsl:param name="object-id" select="id"/>
        <xsl:param name="overview" select="substring-before(substring-after(notes, '$OV$'), '$EX$')"/>
        <section class="fm-section">
            <h2>
                <xsl:attribute name="id">
                    <xsl:value-of select="alias"/>
                </xsl:attribute>
                <xsl:value-of select="name/text()"/> Section
            </h2>
            <h3>Section Overview</h3>
            <p class="section-text"><xsl:value-of select="$overview"></xsl:value-of></p>
            
            <ul class="function-list-toc">
                <xsl:call-template name="section-nav">
                    <xsl:with-param name="order" select="$order"/>
                    <xsl:with-param name="is-top" select="'t'"/>
                </xsl:call-template>
            </ul>

            <table>
                <xsl:attribute name="class">
                    <xsl:value-of select="concat('function-list ','section', $order, '-background')"/>
                </xsl:attribute>
                <thead>
                    <tr>
                        <th class="function-col">
                           Section/ID#:<br/>
                            Type:
                        </th>
                        <th class="criteria-col">
                            <p>Header/Function Name</p>
                            <p>Description</p>
                            <p>Conformance Criteria</p>
                        </th>
                        <th class="reference-col">Reference</th>
                        <th class="change-col">Chg Ind</th>
                        <th class="row-col">Priority</th>
                    </tr>
                </thead>
                <tbody>
                    <xsl:for-each select="following-sibling::object[parentId/text()=$object-id]">
                        <xsl:call-template name="function-output">
                            <xsl:with-param name="sect-no" select="$order"/>
                            <xsl:with-param name="level-no" select="1"/>
                        </xsl:call-template>
                    </xsl:for-each>
                </tbody>
            </table>
        </section>
    </xsl:template>
    
    <xsl:template name="function-output">
        <xsl:param name="sect-no"/>
        <xsl:param name="level-no"/>
        <xsl:param name="object-id" select="id"/>
        <xsl:param name="statement" select="substring-before(substring-after(notes, '$ST$'), '$DE$')"/>
        <xsl:param name="description" select="substring-before(substring-after(notes, '$DE$'), '$EX$')"/>
        <xsl:param name="plain-name" select="substring-after(name, alias)"/>
        <xsl:param name="next-level-no">
            <xsl:choose>
                <xsl:when test="($level-no + 1) > $max-sub-levels">
                    <xsl:value-of select="$level-no"/>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:value-of select="$level-no + 1"/>
                </xsl:otherwise>
            </xsl:choose>
        </xsl:param>
        
        <xsl:variable name="main-class" select="concat('section', $sect-no, '-lev', $level-no, '-main')"/>
        <xsl:variable name="sub-class" select="concat('section', $sect-no, '-lev', $level-no, '-sub')"/>

        <tr>
            <xsl:attribute name="class">
                <xsl:value-of select="$main-class"/>
            </xsl:attribute>
            <td>
                <xsl:attribute name="id">
                    <xsl:value-of select="alias"/>
                </xsl:attribute>
                <xsl:value-of select="alias"></xsl:value-of>
            </td>
            <td rowspan="2" class="function-title">
                <xsl:value-of select="$plain-name"/>
            </td>
            <td rowspan="2" class="value-column">
                <xsl:call-template name="get-reference"/>
            </td>
            <td rowspan="2" class="value-column">
                <xsl:call-template name="get-change-indicator"/>
            </td>
            <td rowspan="2" class="value-column">
                <xsl:value-of select="tag[@name='Priority']/@value"/>
            </td>
        </tr>
        <tr>
            <xsl:attribute name="class">
                <xsl:value-of select="$main-class"/>
            </xsl:attribute>
            <td>
                <xsl:value-of select="stereotype"/>
            </td>
        </tr>
        <tr>
            <xsl:attribute name="class">
                <xsl:value-of select="$sub-class"/>
            </xsl:attribute>
            <td class="function-description" colspan="5">
                <p class="summary-text">
                    <strong>Statement: </strong>
                    <xsl:value-of select="$statement"/>
                </p>
                <p class="summary-text">
                    <strong>Description: </strong>
                    <xsl:value-of select="$description"/>
                </p>
                <xsl:if test="tag/@name='ExternalReference'">
                    <dl class="fn-references">
                        <dt>External References:</dt>
                        <dd>
                            <ul>
                                <xsl:for-each select="tag[@name='ExternalReference']">
                                    <xsl:call-template name="format-external-reference"/>
                                </xsl:for-each>
                            </ul>
                        </dd>
                    </dl>
                </xsl:if>
            </td>
        </tr>
        <xsl:for-each select="following-sibling::object[parentId/text()=$object-id and stereotype/text()='Criteria']">
            <xsl:call-template name="criteria-output">
                <xsl:with-param name="sect-no" select="$sect-no"/>
                <xsl:with-param name="level-no" select="$level-no"/>
            </xsl:call-template>
        </xsl:for-each>
        <xsl:for-each select="following-sibling::object[parentId/text()=$object-id and (stereotype/text()='Function' or stereotype/text()='Header')]">
            <xsl:call-template name="function-output">
                <xsl:with-param name="sect-no" select="$sect-no"/>
                <xsl:with-param name="level-no" select="$next-level-no"/>
            </xsl:call-template>
        </xsl:for-each>
    </xsl:template>
    
    <xsl:template name="criteria-output">
        <xsl:param name="sect-no"/>
        <xsl:param name="level-no"/>

        <tr>
            <xsl:attribute name="class">
                <xsl:value-of select="concat('section', $sect-no, '-lev', $level-no, '-sub')"/>
            </xsl:attribute>
            <td class="criteria-description" colspan="2">
                <xsl:call-template name="get-criteria-text"/>
            </td>
            <td class="fp-criteria-reference">
                <xsl:call-template name="get-functional-reference"/>
            </td>
            <td class="fp-criteria-change">
                <xsl:call-template name="get-change-indicator"/>
            </td>
            <td class="fp-criteria-priority">
                <xsl:call-template name="get-criteria-priority"/>
            </td>
        </tr>
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
        <xsl:variable name="criteria-num" select="substring-after(name, '#')"/>
        <xsl:choose>
            <xsl:when test="contains(notes,' conform to function ')">
                <xsl:variable name="pre-text" select="substring-before(notes, ' conform to function ')"/>
                <xsl:variable name="working-text" select="substring-after(notes, ' conform to function ')"/>
                <xsl:variable name="function-ref" select="substring-before($working-text, ' ')"/>
                <xsl:variable name="post-text" select="substring-after($working-text, ' ')"/>
                <strong><xsl:value-of select="$criteria-num"/></strong>
                <xsl:value-of select="concat('. ', $pre-text, ' conform to function ')"/>
                <a>
                    <xsl:attribute name="href">
                        <xsl:value-of select="concat('#', $function-ref)"/>
                    </xsl:attribute>
                    <xsl:value-of select="$function-ref"/>
                </a>
                <xsl:value-of select="concat(' ', $post-text)"/>
            </xsl:when>
            <xsl:otherwise>
                <strong><xsl:value-of select="$criteria-num"/></strong>
                <xsl:value-of select="concat('. ', notes)"/>
            </xsl:otherwise>
        </xsl:choose>
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
            <xsl:if test="tag/@name='Reference.CriterionID'">
                <xsl:value-of select="concat('#', tag[@name='Reference.CriterionID']/@value)"/>
            </xsl:if>
        </xsl:param>
        
        <xsl:value-of select="concat($func-ref, $crit-ref)"/>
    </xsl:template>
    
    <xsl:template name="get-criteria-priority">
        <xsl:if test="tag/@name='Priority'">
            <xsl:value-of select="tag[@name='Priority']/@value"/>
        </xsl:if>
    </xsl:template>
    
    <xsl:template name="format-external-reference">
        <xsl:param name="text-area" select="substring-before(@value, '$$URL$$')"/>
        <xsl:param name="text-value" select="substring-after($text-area, 'TEXT$$')"/>
        <xsl:param name="url-value" select="substring-after(@value, '$$URL$$')"/>
        
        <xsl:choose>
            <xsl:when test="string-length($url-value) > 0">
                <li>
                    <a>
                        <xsl:attribute name="href">
                            <xsl:value-of select="$url-value"/>
                        </xsl:attribute>
                        <xsl:value-of select="$text-value" disable-output-escaping="yes"/>
                    </a>
                </li>
            </xsl:when>
            <xsl:otherwise>
                <li><xsl:value-of select="$text-value" disable-output-escaping="yes"/></li>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    
    <xsl:template name="section-nav">
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
        <li>
            <xsl:attribute name="class">
                <xsl:value-of select="concat('section', $order, '-lev1-main')"/>
            </xsl:attribute>
            <xsl:value-of select="$section-title"/>
            <ul class="bottom-function-nav">
                <xsl:if test="$is-top = 'f'">
                    <xsl:call-template name="function-nav">
                        <xsl:with-param name="order" select="$order"/>
                        <xsl:with-param name="nav-text" select="concat(alias, '  ', name)"/>
                        <xsl:with-param name="background-qualifier" select="'-lev1-main'"/>
                    </xsl:call-template>
                </xsl:if>
                
                <xsl:for-each select="following-sibling::object[parentId/text()=$object-id]">
                    <xsl:call-template name="function-nav">
                        <xsl:with-param name="order" select="$order"/>
                    </xsl:call-template>
                </xsl:for-each>
            </ul>
        </li>
    </xsl:template>
    
    <xsl:template name="function-nav">
        <xsl:param name="order"/>
        <xsl:param name="nav-text" select="substring-after(name, alias)"/>
        <xsl:param name="background-qualifier" select="'-lev1-sub'"/>
        <li>
            <xsl:attribute name="class">
                <xsl:value-of select="concat('section', $order, $background-qualifier)"/>
            </xsl:attribute>
            <a>
                <xsl:attribute name="href">
                    <xsl:value-of select="concat('#',alias)"/>
                </xsl:attribute>
                <xsl:value-of select="concat(alias, '    ', $nav-text)"/>
            </a>
        </li>
    </xsl:template>
    
    <xsl:template name="component-description">
        <p>The Function List includes the following components:</p>
        <dl>
            <dt>Function ID # (Normative)</dt>
            <dd>This is the unique identifier of a function in the Function List (e.g. CP.1.1) and should be used to uniquely identify the function when referencing functions.  The Function ID also serves to identify the section within which the function exists (CP = Care Provision Section) and the hierarchy or relationship between functions (CP.1.1 is a sibling to CP.1.2, parent of CP.1.1.1 and child of CP.1).  In many cases the parent is fully expressed by the children.</dd>
            <dt>Function Type (Reference)</dt>
            <dd>Indication of the line item as being a header (H) or function (F) or conformance criteria.</dd>
            <dt>Function Name (Normative)</dt>
            <dd>This is the name of the Function and whilst expected to be unique within the Function List; it is not recommended to be used to identify the function without being accompanied by the Function ID.<br>Example: Manage Medication List</br></dd>
            <dt>Function Statement (Normative)</dt>
            <dd>This is a brief statement of the purpose of this function.  Whist not restricted to the use of structured language that is used in the Conformance Criteria (see below); the Statement should clearly identify the purpose and scope of the function.  <br>Example: Create and maintain patient-specific medication lists.</br></dd>
            <dt>Description (Reference)</dt>
            <dd>This is a more detailed description of the function, including examples if needed.  <br>Example: Medication lists are managed over time, whether over the course of a visit or stay, or the lifetime of a patient. All pertinent dates, including medication start, modification, and end dates are stored. The entire medication history for any medication, including alternative supplements and herbal medications, is viewable. Medication lists are not limited to medication orders recorded by providers, but may include, for example, pharmacy dispense/supply records, patient-reported medications and additional information such as age specific dosage.</br></dd>
            <dt>Conformance Criteria (Normative) </dt>
            <dd>Each function in the Function List includes one or more Conformance Criteria.  A Conformance Criteria, which exists as normative language in this standard, defines the requirements for conforming to the function.  The language used to express a conformance criterion is highly structured with standardized components with set meanings.  The structured language used to define conformance clauses in the Function List are defined in the Glossary (Chapter 4).</dd>
            <dt>Reference</dt>
            <dd>Reference to the Functional Model or Functional Profile the current Functional Profile was developed against. </dd>
            <dt>External Reference</dt>
            <dd>Reference to additional documentation that is relevant to the item. This documentation may include national standards or requirements that the profile was created to fulfill.</dd>
            <dt>Change Indicator</dt>
            <dd>The change indicator shows the change from previous versions.  This will be valued as follows:
                <ul>
                    <li>C   - Changed</li>
                    <li>D   - Deleted</li>
                    <li>N   - New</li>
                    <li>NC  - No Change</li>
                    <li>DEP - Deprecated</li>
                </ul>
            </dd>
            <dt>Priority</dt>
            <dd>The priority for the implementation of the item. This will be valued as follows:
                <ul>
                    <li>EN  - Essential Now</li>
                    <li>EF  - Essential Future</li>
                    <li>O   - Optional</li>
                </ul>
            </dd>
        </dl>
    </xsl:template>
    
    <xsl:template match="object"/>
    
    <xsl:template match="relationship"/>
</xsl:stylesheet>