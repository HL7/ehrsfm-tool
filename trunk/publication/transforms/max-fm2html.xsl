<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
    xmlns:mif="urn:hl7-org:v3/mif2" version="2.0">
    
    <xsl:output doctype-system="html"/>
    <xsl:param name="ballot-info-file" select="'ehr-s-fm-ballot.mif'"/>
    <xsl:variable name="ballot-info" select="document($ballot-info-file)/mif:package"/>
    
    <xsl:template match="objects/object[stereotype/text()='HL7-FM']">
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
                        <h2>EHR-S Function List Components</h2>
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
            <xsl:attribute name="id" select="alias/text()"/>
            <h2>
                <xsl:attribute name="id" select="alias"/>
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
                <xsl:attribute name="class" select="concat('function-list ','section', $order, '-background')"/>
                <thead>
                    <tr>
                        <th class="function-col">
                           Section/ID#:<br/>
                            Type:<br/>
                            Name:
                        </th>
                        <th class="criteria-col">Conformance Criteria</th>
                        <th class="reference-col">Reference</th>
                        <th class="change-col">Chg Ind</th>
                        <th class="row-col">R2 Row#</th>
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
        <tr>
            <xsl:attribute name="class" select="concat('section', $sect-no, '-lev', $level-no, '-main')"/>
            <td>
                <xsl:attribute name="id" select="alias"/>
                <xsl:value-of select="alias"></xsl:value-of>
            </td>
            <td rowspan="3"/>
            <td rowspan="3" class="value-column">
                <xsl:call-template name="get-reference"/>
            </td>
            <td rowspan="3" class="value-column">
                <xsl:call-template name="get-change-indicator"/>
            </td>
            <td rowspan="3" class="value-column">
                <xsl:value-of select="tag[@name='Row']/@value"/>
            </td>
        </tr>
        <tr>
            <xsl:attribute name="class" select="concat('section', $sect-no, '-lev', $level-no, '-main')"/>
            <td>
                <xsl:value-of select="stereotype"/>
            </td>
        </tr>
        <tr>
            <xsl:attribute name="class" select="concat('section', $sect-no, '-lev', $level-no, '-main')"/>
            <td>
                <xsl:value-of select="$plain-name"/>
            </td>
        </tr>
        <tr>
            <xsl:attribute name="class" select="concat('section', $sect-no, '-lev', $level-no, '-sub')"/>
            <td class="function-description">
                <p class="lhs-text">
                    <strong>Statement: </strong>
                    <xsl:value-of select="$statement"/>
                </p>
                <p class="lhs-text">
                    <strong>Description: </strong>
                    <xsl:value-of select="$description"/>
                </p>
            </td>
            <td colspan="4">
                <xsl:if test="count(following-sibling::object[parentId/text()=$object-id and stereotype/text()='Criteria'])">
                    <ol class="criteria-list">
                        <xsl:for-each select="following-sibling::object[parentId/text()=$object-id and stereotype/text()='Criteria']">
                            <xsl:call-template name="criteria-output"/>
                        </xsl:for-each>
                    </ol>
                </xsl:if>
            </td>
        </tr>
        <xsl:for-each select="following-sibling::object[parentId/text()=$object-id and stereotype/text()='Function']">
            <xsl:call-template name="function-output">
                <xsl:with-param name="sect-no" select="$sect-no"/>
                <xsl:with-param name="level-no" select="$level-no + 1"/>
            </xsl:call-template>
        </xsl:for-each>
    </xsl:template>
    
    <xsl:template name="criteria-output">
        <li>
            <div class="criteria-row">
                <xsl:call-template name="get-criteria-row"/>
            </div>
            <div class="criteria-change">
                <xsl:call-template name="get-change-indicator"/>
            </div>
            <div class="criteria-reference">
                <xsl:call-template name="get-functional-reference"/>
            </div>
            <div class="criteria-text">
                <xsl:call-template name="get-criteria-text"/>
            </div>
        </li>
    </xsl:template>

    <xsl:template name="get-reference">
        <xsl:if test="tag/@name='Reference.Alias'">
            <xsl:value-of select="tag[@name='Reference.FunctionID']/@value"/>
        </xsl:if>
    </xsl:template>
    
    <xsl:template name="get-change-indicator">
        <xsl:choose>
            <xsl:when test="tag[@name='Reference.ChangeInfo' and @value='Modified']">C</xsl:when>
            <xsl:when test="tag[@name='Reference.ChangeInfo' and @value='New']">N</xsl:when>
            <xsl:otherwise>NC</xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    
    <xsl:template name="get-criteria-text">
        <xsl:choose>
            <xsl:when test="contains(notes,' conform to function ')">
                <xsl:variable name="pre-text" select="substring-before(notes, ' conform to function ')"/>
                <xsl:variable name="working-text" select="substring-after(notes, ' conform to function ')"/>
                <xsl:variable name="function-ref" select="substring-before($working-text, ' ')"/>
                <xsl:variable name="post-text" select="substring-after($working-text, ' ')"/>
                <xsl:value-of select="concat($pre-text, ' conform to function ')"/>
                <a>
                    <xsl:attribute name="href" select="concat('#', $function-ref)"/>
                    <xsl:value-of select="$function-ref"/>
                </a>
                <xsl:value-of select="concat(' ', $post-text)"/>
            </xsl:when>
            <xsl:otherwise>
                <xsl:value-of select="notes"/>
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
            <xsl:attribute name="class" select="concat('section', $order, '-lev1-main')"/>
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
            <xsl:attribute name="class" select="concat('section', $order, $background-qualifier)"/>
            <a>
                <xsl:attribute name="href" select="concat('#',alias)"/>
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
            <dt>R1.1 Reference (Reference)</dt>
            <dd>Reference to the previous version of the Functional Model is included to support transition from one version to the next.   The first 2 digits indicate the source document;  FM = Functional Model, LM = Lifecycle Model.  The remainder of the reference is to the function and, if applicable, conformance criteria.</dd>
            <dt>Change Indicator</dt>
            <dd>The change indicator shows the change from previous versions.  This will be valued as follows:
                <ul>
                    <li>C - Changed</li>
                    <li>D - Deleted</li>
                    <li>N - New</li>
                    <li>NC - No Change</li>
                </ul>
            </dd>
            <dt>R2 Row #</dt>
            <dd>A unique number for the row within the section.</dd>
        </dl>
    </xsl:template>
    
    <xsl:template match="object"/>
    
    <xsl:template match="relationship"/>
</xsl:stylesheet>