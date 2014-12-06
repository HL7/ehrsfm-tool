<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:max="http://www.umcg.nl/MAX"
    version="1.0">
    
    <xsl:output indent="yes" method="xml"/>
    <xsl:param name="base-file" select="'base-profile.max'"/>
    <xsl:variable name="base-objects" select="document($base-file)//objects"/>
    <xsl:variable name="blanks" select="'                                                                       '"/>
    
    <xsl:template match="objects/object[stereotype/text()='HL7-FM-Profile']">
        <xsl:variable name="compare-object" select="$base-objects/object[stereotype/text()='HL7-FM-Profile']"/>
        
        <xsl:message>Processing Profile Base for <xsl:value-of select="name"/></xsl:message>
        <xsl:choose>
            <xsl:when test="$compare-object">
                <xsl:if test="not(tag[@name='Rationale'] = $compare-object/tag[@name='Rationale'])">
                    <xsl:message> +--Rationale not equal <xsl:value-of select="tag[@name='Rationale']"/> !=
                        <xsl:value-of select="$compare-object/tag[@name='Rationale']"/>
                    </xsl:message>
                </xsl:if>
                <xsl:if test="not(tag[@name='Scope'] = $compare-object/tag[@name='Scope'])">
                    <xsl:message> +--Scope not equal <xsl:value-of select="tag[@name='Scope']"/> !=
                        <xsl:value-of select="$compare-object/tag[@name='Scope']"/>
                    </xsl:message>
                </xsl:if>
            </xsl:when>
            <xsl:otherwise>
                <xsl:message>  ## Base object not found, nothong to compare against!</xsl:message>                
            </xsl:otherwise>
        </xsl:choose>
        
        <xsl:apply-templates select="parent::object[stereotype/text()='Section']"/>
    </xsl:template>
    
    <xsl:template match="object[stereotype/text()='Section']">
        <xsl:param name="indent"></xsl:param>
        <xsl:variable name="external-id" select="alias"/>
        <xsl:variable name="object-id" select="id"/>
        <xsl:variable name="compare-object" select="$base-objects/object[alias/text()=$external-id]"/>

        <xsl:message><xsl:value-of select="$indent"/>-Processing Section <xsl:value-of select="name"/></xsl:message>
        
        <xsl:choose>
            <xsl:when test="$compare-object">
                <xsl:if test="not(name = $compare-object/name)">
                    <xsl:message><xsl:value-of select="$indent"/>  +--Name not equal <xsl:value-of select="name"/> != <xsl:value-of select="$compare-object/name"/>
                    </xsl:message>
                </xsl:if>
                <xsl:if test="not(notes = $compare-object/notes)">
                    <xsl:message><xsl:value-of select="$indent"/>  +--Notes not equal <xsl:value-of select="notes"/> != <xsl:value-of select="$compare-object/notes"/>
                    </xsl:message>
                </xsl:if>
                <xsl:if test="not(tag[@name='Reference.Alias']/@value = $compare-object/tag[@name='Reference.Alias']/@value)">
                    <xsl:message><xsl:value-of select="$indent"/>  +--Reference.Alias not equal <xsl:value-of select="tag[@name='Reference.Alias']/@value"/> != <xsl:value-of select="$compare-object/tag[@name='Reference.Alias']/@value"/>
                    </xsl:message>
                </xsl:if>
                <xsl:if test="not(tag[@name='Reference.SectionID']/@value = $compare-object/tag[@name='Reference.SectionID']/@value)">
                    <xsl:message><xsl:value-of select="$indent"/>  +--Reference.SectionID not equal <xsl:value-of select="tag[@name='Reference.SectionID']/@value"/> != <xsl:value-of select="$compare-object/tag[@name='Reference.SectionID']/@value"/>
                    </xsl:message>
                </xsl:if>
                <xsl:if test="not(tag[@name='Reference.ChangeIndicator']/@value = $compare-object/tag[@name='Reference.ChangeIndicator']/@value)">
                    <xsl:message><xsl:value-of select="$indent"/>  +--Reference.ChangeIndicator not equal <xsl:value-of select="tag[@name='Reference.ChangeIndicator']/@value"/> != <xsl:value-of select="$compare-object/tag[@name='Reference.ChangeIndicator']/@value"/>
                    </xsl:message>
                </xsl:if>
                <xsl:if test="not(tag[@name='Priority']/@value = $compare-object/tag[@name='Priority']/@value)">
                    <xsl:message><xsl:value-of select="$indent"/>  +--Priority not equal <xsl:value-of select="tag[@name='Priority']/@value"/> != <xsl:value-of select="$compare-object/tag[@name='Priority']/@value"/>
                    </xsl:message>
                </xsl:if>
            </xsl:when>
            <xsl:otherwise>
                <xsl:message><xsl:value-of select="$indent"/>    ## Base object not found, nothing to compare against!</xsl:message>                
            </xsl:otherwise>
        </xsl:choose>
        
        <xsl:apply-templates select="following-sibling::object[parentId/text()=$object-id]" mode="compare">
            <xsl:with-param name="indent" select="1"/>
        </xsl:apply-templates>
<!--        
        <xsl:for-each select="following-sibling::object[parentId/text()=$object-id]">
            <xsl:call-template name="function-compare">
                <xsl:with-param name="indent" select="1"/>
            </xsl:call-template>
        </xsl:for-each> -->
    </xsl:template>
    
    <xsl:template match="object[stereotype/text()='Header' or stereotype/text()='Function']" mode="compare">
        <xsl:param name="indent"/>
        <xsl:variable name="object-id" select="id"/>
        
        <xsl:call-template name="function-compare">
            <xsl:with-param name="indent" select="$indent"/>
        </xsl:call-template>
        
        <xsl:apply-templates select="following-sibling::object[parentId/text()=$object-id and stereotype/text() = 'Criteria']" mode="compare">
            <xsl:with-param name="indent" select="$indent + 1"/>
        </xsl:apply-templates>
        
        <xsl:apply-templates select="following-sibling::object[parentId/text()=$object-id and (stereotype/text() = 'Header' or stereotype/text() = 'Function')]" mode="compare">
            <xsl:with-param name="indent" select="$indent + 1"/>
        </xsl:apply-templates>
    </xsl:template>
    
    <xsl:template match="object[stereotype/text()='Criteria']" mode="compare">
        <xsl:param name="indent"/>
        
        <xsl:call-template name="criteria-compare">
            <xsl:with-param name="indent" select="$indent"/>
        </xsl:call-template>
    </xsl:template>
    
    <xsl:template name="function-compare">
        <xsl:param name="indent"/>
        <xsl:variable name="line-indent" select="substring($blanks,1,$indent)"/>
        <xsl:variable name="external-id" select="alias"/>
        <xsl:variable name="object-id" select="id"/>
        <xsl:variable name="compare-object" select="$base-objects/object[alias/text()=$external-id]"/>
        
        <xsl:message><xsl:value-of select="concat($line-indent, '-')"/><xsl:value-of select="name"/></xsl:message>
        
        <xsl:choose>
            <xsl:when test="$compare-object">
                <xsl:if test="not(name = $compare-object/name)">
                    <xsl:message><xsl:value-of select="$line-indent"/>  +--Name not equal <xsl:value-of select="name"/> != <xsl:value-of select="$compare-object/name"/>
                    </xsl:message>
                </xsl:if>
                <xsl:if test="not(notes = $compare-object/notes)">
                    <xsl:message><xsl:value-of select="$line-indent"/>  +--Notes not equal <xsl:value-of select="notes"/> != <xsl:value-of select="$compare-object/notes"/>
                    </xsl:message>
                </xsl:if>
                <xsl:if test="tag[@name='Reference.Alias'] and not(tag[@name='Reference.Alias']/@value = $compare-object/tag[@name='Reference.Alias']/@value)">
                    <xsl:message><xsl:value-of select="$line-indent"/>  +--Reference.Alias not equal <xsl:value-of select="tag[@name='Reference.Alias']/@value"/> != <xsl:value-of select="$compare-object/tag[@name='Reference.Alias']/@value"/>
                    </xsl:message>
                </xsl:if>
                <xsl:if test="tag[@name='Reference.FunctionID'] and not(tag[@name='Reference.FunctionID']/@value = $compare-object/tag[@name='Reference.FunctionID']/@value)">
                    <xsl:message><xsl:value-of select="$line-indent"/>  +--Reference.FunctionID not equal <xsl:value-of select="tag[@name='Reference.FunctionID']/@value"/> != <xsl:value-of select="$compare-object/tag[@name='Reference.FunctionID']/@value"/>
                    </xsl:message>
                </xsl:if>
                <xsl:if test="tag[@name='Reference.ChangeIndicator'] and not(tag[@name='Reference.ChangeIndicator']/@value = $compare-object/tag[@name='Reference.ChangeIndicator']/@value)">
                    <xsl:message><xsl:value-of select="$line-indent"/>  +--Reference.ChangeIndicator not equal <xsl:value-of select="tag[@name='Reference.ChangeIndicator']/@value"/> != <xsl:value-of select="$compare-object/tag[@name='Reference.ChangeIndicator']/@value"/>
                    </xsl:message>
                </xsl:if>
                <xsl:choose>
                    <xsl:when test="tag[@name='Priority']">
                        <xsl:if test="not(tag[@name='Priority']/@value = $compare-object/tag[@name='Priority']/@value)">
                            <xsl:message><xsl:value-of select="$line-indent"/>  +--Priority not equal <xsl:value-of select="tag[@name='Priority']/@value"/> != <xsl:value-of select="$compare-object/tag[@name='Priority']/@value"/>
                            </xsl:message>
                        </xsl:if>
                    </xsl:when>
                    <xsl:otherwise>
                        <xsl:message><xsl:value-of select="$line-indent"/>  +--**Priority is missing**</xsl:message>
                    </xsl:otherwise>
                </xsl:choose>
                <xsl:if test="tag[@name='ExternalReference']">
                    <xsl:call-template name="reference-check">
                        <xsl:with-param name="indent" select="$indent"/>
                        <xsl:with-param name="compare-items" select="$compare-object/tag[@name='ExternalReference']"/>
                    </xsl:call-template>
                </xsl:if>
            </xsl:when>
            <xsl:otherwise>
                <xsl:message><xsl:value-of select="$line-indent"/>    ## Base object not found, nothing to compare against!</xsl:message>                
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    
    <xsl:template name="criteria-compare">
        <xsl:param name="indent"/>
        <xsl:variable name="line-indent" select="substring($blanks,1,$indent)"/>
        <xsl:variable name="external-id" select="name"/>
        <xsl:variable name="object-id" select="id"/>
        <xsl:variable name="compare-object" select="$base-objects/object[name/text()=$external-id]"/>
        
        <xsl:message><xsl:value-of select="concat($line-indent, '- ')"/><xsl:value-of select="substring-after(name, '#')"/></xsl:message>
        
        <xsl:choose>
            <xsl:when test="$compare-object">
                <xsl:if test="not(notes = $compare-object/notes)">
                    <xsl:message><xsl:value-of select="$line-indent"/>  +--Notes not equal <xsl:value-of select="notes"/> != <xsl:value-of select="$compare-object/notes"/>
                    </xsl:message>
                </xsl:if>
                <xsl:if test="tag[@name='Optionality'] and not(tag[@name='Optionality']/@value = $compare-object/tag[@name='Optionality']/@value)">
                    <xsl:message><xsl:value-of select="$line-indent"/>  +--Optionality not equal <xsl:value-of select="tag[@name='Optionality']/@value"/> != <xsl:value-of select="$compare-object/tag[@name='Optionality']/@value"/>
                    </xsl:message>
                </xsl:if>
                <xsl:if test="tag[@name='Conditional'] and not(tag[@name='Conditional']/@value = $compare-object/tag[@name='Conditional']/@value)">
                    <xsl:message><xsl:value-of select="$line-indent"/>  +--Conditional not equal <xsl:value-of select="tag[@name='Conditional']/@value"/> != <xsl:value-of select="$compare-object/tag[@name='Conditional']/@value"/>
                    </xsl:message>
                </xsl:if>
                <xsl:if test="tag[@name='Dependent'] and not(tag[@name='Dependent']/@value = $compare-object/tag[@name='Dependent']/@value)">
                    <xsl:message><xsl:value-of select="$line-indent"/>  +--Dependent not equal <xsl:value-of select="tag[@name='Dependent']/@value"/> != <xsl:value-of select="$compare-object/tag[@name='Dependent']/@value"/>
                    </xsl:message>
                </xsl:if>
                <xsl:if test="tag[@name='Reference.Alias'] and not(tag[@name='Reference.Alias']/@value = $compare-object/tag[@name='Reference.Alias']/@value)">
                    <xsl:message><xsl:value-of select="$line-indent"/>  +--Reference.Alias not equal <xsl:value-of select="tag[@name='Reference.Alias']/@value"/> != <xsl:value-of select="$compare-object/tag[@name='Reference.Alias']/@value"/>
                    </xsl:message>
                </xsl:if>
                <xsl:if test="tag[@name='Reference.FunctionID'] and not(tag[@name='Reference.FunctionID']/@value = $compare-object/tag[@name='Reference.FunctionID']/@value)">
                    <xsl:message><xsl:value-of select="$line-indent"/>  +--Reference.FunctionID not equal <xsl:value-of select="tag[@name='Reference.FunctionID']/@value"/> != <xsl:value-of select="$compare-object/tag[@name='Reference.FunctionID']/@value"/>
                    </xsl:message>
                </xsl:if>
                <xsl:if test="tag[@name='Reference.CriterionID'] and not(tag[@name='Reference.CriterionID']/@value = $compare-object/tag[@name='Reference.CriterionID']/@value)">
                    <xsl:message><xsl:value-of select="$line-indent"/>  +--Reference.CriterionID not equal <xsl:value-of select="tag[@name='Reference.CriterionID']/@value"/> != <xsl:value-of select="$compare-object/tag[@name='Reference.CriterionID']/@value"/>
                    </xsl:message>
                </xsl:if>
                <xsl:if test="tag[@name='Reference.ChangeIndicator'] and not(tag[@name='Reference.ChangeIndicator']/@value = $compare-object/tag[@name='Reference.ChangeIndicator']/@value)">
                    <xsl:message><xsl:value-of select="$line-indent"/>  +--Reference.ChangeIndicator not equal <xsl:value-of select="tag[@name='Reference.ChangeIndicator']/@value"/> != <xsl:value-of select="$compare-object/tag[@name='Reference.ChangeIndicator']/@value"/>
                    </xsl:message>
                </xsl:if>
                <xsl:choose>
                    <xsl:when test="tag[@name='Priority']">
                        <xsl:if test="not(tag[@name='Priority']/@value = $compare-object/tag[@name='Priority']/@value)">
                            <xsl:message><xsl:value-of select="$line-indent"/>  +--Priority not equal <xsl:value-of select="tag[@name='Priority']/@value"/> != <xsl:value-of select="$compare-object/tag[@name='Priority']/@value"/>
                            </xsl:message>
                        </xsl:if>
                    </xsl:when>
                    <xsl:otherwise>
                        <xsl:message><xsl:value-of select="$line-indent"/>  +--**Priority is missing**</xsl:message>
                    </xsl:otherwise>
                </xsl:choose>
            </xsl:when>
            <xsl:otherwise>
                <xsl:message><xsl:value-of select="$line-indent"/>    ## Base object not found, nothing to compare against!</xsl:message>                
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    
    <xsl:template name="reference-check">
        <xsl:param name="indent"/>
        <xsl:param name="compare-items"/>
        <xsl:variable name="line-indent" select="substring($blanks,1,$indent)"/>
        
        <xsl:choose>
            <xsl:when test="count($compare-items) = 0">
                <xsl:message><xsl:value-of select="$line-indent"/>  *** No External References in base item ***</xsl:message>
            </xsl:when>
            <xsl:otherwise>
                <xsl:for-each select="tag[@name='ExternalReference']">
                    <xsl:call-template name="reference-error">
                        <xsl:with-param name="indent" select="$indent"/>
                        <xsl:with-param name="compare-items" select="$compare-items"/>
                    </xsl:call-template>
                </xsl:for-each>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    
    <xsl:template name="reference-error">
        <xsl:param name="indent"/>
        <xsl:param name="compare-items"/>
        <xsl:variable name="line-indent" select="substring($blanks,1,$indent)"/>
        <xsl:variable name="the-text" select="@value"/>
        
        <xsl:if test="count($compare-items[@value = $the-text]) = 0">
            <xsl:message><xsl:value-of select="$line-indent"/>  +-- ExternalReferences have nothing matching {<xsl:value-of select="$the-text"/>}</xsl:message>
        </xsl:if>
    </xsl:template>
    
    <xsl:template match="object"/>
    
    <xsl:template match="relationship"/>
    
    <xsl:template match="text()"/>
</xsl:stylesheet>