<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="2.0"
	xmlns:office="urn:oasis:names:tc:opendocument:xmlns:office:1.0"
	xmlns:table="urn:oasis:names:tc:opendocument:xmlns:table:1.0"
	xmlns:text="urn:oasis:names:tc:opendocument:xmlns:text:1.0"
	xmlns:xs="http://www.w3.org/2001/XMLSchema"
	xmlns:max="http://www.umcg.nl/MAX"
	exclude-result-prefixes="office table text xs">

    <xsl:output indent="yes" method="xml"/>
    <xsl:variable name="fm" select="document('HL7 EHR-S FM ERRATA - 20160802 fixed.max')"/>
    
    <!--
    	Input flat Open Document Spreadsheet snipet:
    	 
		   <table:table table:name="Sheet1" table:style-name="ta1">
		    <table:table-column table:style-name="co1" table:number-columns-repeated="2" table:default-cell-style-name="Default"/>
		    <table:table-row table:style-name="ro1">
		     <table:table-cell office:value-type="string" calcext:value-type="string"><text:p>TI.2.1 CC#4</text:p><text:p>RI.2 CC#5</text:p>
		     </table:table-cell>
		     <table:table-cell/>
		    </table:table-row>
		    
		MAX FM Object template Criteria:
			<tag name="Conditional" value="">Values: Y,N</tag>
			<tag name="Dependent" value="">Values: Y,N</tag>
			<tag name="Optionality" value="">Values: SHALL,SHOULD,MAY</tag>
			<tag name="Row" value=""></tag>
			<tag name="Reference.ChangeIndicator" value="">Values: NC,N,C,D,DEP</tag>
			<tag name="Reference.Alias" value=""></tag>
			<tag name="Reference.FunctionID" value=""></tag>
			<tag name="Reference.CriteriaID" value=""></tag>
			<tag name="ExternalReference" value=""></tag>
	    
	    2 sheets
	    Sheet #1 "objects"
	    	kolommen zijn attributen van object
	    Sheet #2 "relationships"
	    	kolommen zijn attributen van relationship
	    	
	    Now specific rules based on the EHR IFP!
     -->
    
    <xsl:template match="/">
    	<max:model>
	    	<objects>
	    		<xsl:apply-templates mode="objects"/>
	    	</objects>
	    	<relationships>
    			<xsl:apply-templates mode="relationships"/>
			</relationships>    			
    	</max:model>
    </xsl:template>
    
    <xsl:template match="//table:table[@table:name='objects']/table:table-row" mode="objects">
   		<xsl:variable name="name" select="table:table-cell[1]/text:p"/>
   		<xsl:variable name="notes" select="table:table-cell[2]/text:p"/>
   		<xsl:variable name="row" select="position()"/>
   		
   		<xsl:choose>
   			<xsl:when test="starts-with($name[1], 'NEW ')">
   				<xsl:variable name="destFunct" select="substring-before(substring-after($name[1], 'NEW - '), ' ')"/>
	    		<object>
	    			<id><xsl:value-of select="concat('IFP', $row)"/></id>
	    			<name><xsl:value-of select="concat($destFunct, '#', format-number(40+$row/2,'00'))"/></name>
	    			<notes><xsl:value-of select="$notes"/></notes>
	    			<stereotype>Criteria</stereotype>
	    			<type>Requirement</type>
					<!-- <tag name="Row"><xsl:attribute name="value" select="$row"/></tag> -->
	    		</object>
   			</xsl:when>
   			<xsl:when test="$name!=''">
   				<xsl:for-each select="$name">
    				<xsl:variable name="parts" select="tokenize(replace(., ', ',' '),'[\s,]')"/>
    				<xsl:variable name="FunctionID" select="$parts[1]"/>
    				<xsl:variable name="CriteriaID" select="replace($parts[2], 'CC#', '')"/>
    				<xsl:variable name="rowrow" select="concat($row,'-',position())"/>
    				
    				<xsl:choose>
    					<xsl:when test="$CriteriaID castable as xs:numeric">
				    		<object>
				    			<id><xsl:value-of select="concat('IFP', $rowrow)"/></id>
				    			<name><xsl:value-of select="concat($FunctionID,'#',format-number(number($CriteriaID), '00'))"/></name>
				    			<stereotype>CI</stereotype>
								<!-- <tag name="Row"><xsl:attribute name="value" select="$row"/></tag> -->
				    		</object>
    					</xsl:when>
    					<xsl:otherwise>
			    			<IGNORED>!! <xsl:value-of select="concat($FunctionID,'#',$CriteriaID, ' ', .)"/></IGNORED>
    					</xsl:otherwise>
    				</xsl:choose>
		    	</xsl:for-each>
   			</xsl:when>
   		</xsl:choose>    	
    </xsl:template>
    
    <xsl:template match="//table:table[@table:name='objects']/table:table-row" mode="relationships">
   		<xsl:variable name="name" select="table:table-cell[1]/text:p"/>
   		<xsl:variable name="row" select="position()"/>
   		
   		<xsl:choose>
   			<xsl:when test="starts-with($name[1], 'NEW ')">
   				<xsl:variable name="destFunct" select="substring-before(substring-after($name[1], 'NEW - '), ' ')"/>
   				<xsl:variable name="destId" select="$fm//object[alias=$destFunct]/id"/>
	    		<relationship>
	    			<destFunct><xsl:value-of select="$destFunct"/></destFunct>
	    			<sourceId><xsl:value-of select="concat('IFP', $row)"/></sourceId>
	    			<destId><xsl:value-of select="$destId"/></destId>
	    			<type>Aggregation</type>
	    		</relationship>
   			</xsl:when>
   			<xsl:when test="$name!=''">
   				<xsl:for-each select="$name">
    				<xsl:variable name="parts" select="tokenize(replace(., ', ',' '),'[\s,]')"/>
    				<xsl:variable name="FunctionID" select="$parts[1]"/>
    				<xsl:variable name="CriteriaID" select="replace($parts[2], 'CC#', '')"/>
    				<xsl:variable name="rowrow" select="concat($row,'-',position())"/>
    				
    				<xsl:choose>
    					<xsl:when test="$CriteriaID castable as xs:numeric">
    						<xsl:variable name="destAlias" select="concat($FunctionID,'#',format-number(number($CriteriaID), '00'))"/>
    						<xsl:variable name="destId" select="$fm//object[name=$destAlias]/id"/>
				    		<relationship>
				    			<sourceId><xsl:value-of select="concat('IFP', $rowrow)"/></sourceId>
						    	<destId><xsl:value-of select="$destId"/></destId>
								<type>Generalization</type>
				    		</relationship>
    					</xsl:when>
    					<xsl:otherwise>
			    			<!-- <IGNORED>!! <xsl:value-of select="concat($FunctionID,'#',$CriteriaID)"/></IGNORED> -->
    					</xsl:otherwise>
    				</xsl:choose>
		    	</xsl:for-each>
   			</xsl:when>
   		</xsl:choose>    	
    </xsl:template>
        
    <xsl:template match="text()" mode="objects"/>

    <xsl:template match="text()" mode="relationships"/>
    
</xsl:stylesheet>