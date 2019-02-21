<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="3.0"
	xmlns:office="urn:oasis:names:tc:opendocument:xmlns:office:1.0"
	xmlns:table="urn:oasis:names:tc:opendocument:xmlns:table:1.0"
	xmlns:text="urn:oasis:names:tc:opendocument:xmlns:text:1.0"
	xmlns:xs="http://www.w3.org/2001/XMLSchema"
	xmlns:max="http://www.umcg.nl/MAX"
	exclude-result-prefixes="office table text xs">

    <xsl:output indent="yes" method="xml"/>
    
    <!--
    	TODO
    	- also handle '[[', ']]' formatting?
    	- Sections mark as Namespace?? <- for now manually
    	- NOTE: SeeAlso type is WRONG in EHR-S FM R2, it should be Association instead of Dependency
    	- NOTE: Reference.ChangeInfo should be ChangeIndicator in EHR-S FM R2
    	- Give variables meaningful names
    	
    	NOTES
    	- 'Model' sheet
    		- First row is used as the name for this Functional Model
    		- Second row (headers) is ignored
    	- eap should be empty when importing, just root(id=0) and initial view(id=1) packages, fm starts with (id=2)
    	
    	v4 22-oct-2018
    	- Also handle Reference columns.
    	- Added function to handle empty collapsed columns: "number-columns-repeated"
    	v3 19-oct-2018
    	- use parentId from auto-derived hierarchy instead of parentId column
    	- removed parentId column also from spreadsheet template (most current is PHR_S_FM 20180819.fods)
    	- now expect model in 'Model' sheet
    	v2 19-sep-2018
    	- now set Conditional and Dependent tags
    	- "according to organizational policy and/or jurisdictional law" if it is in the criterion text not just at the end > Dependent
    	- add relationships initial work
    	- add "Column H" "See Also" as relationship
    	- Criteria should have Association relationship to Function
    	- create hierarchy (e.g. S.1 should be child of "S", etc) instead of using parentId
    	- lookup relationship destination ids
    	- use row# for section ID tagged value
    	v1
    	- initial
     -->
    
    <!--
    	Input flat Open Document Spreadsheet snippet:
    	 
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
	    
	    1 sheet
	    Based on the full PHR-S FM R2 N1 sheet. Only keep (Clean) columns.
	    
	    Row1 = Title  
	    Row2 = Column headings
		Row3+	    
			Col0: row#
		    Col1: ID#
			Col2: Type
			Col3: R2 Name (Clean)
			Col4: R2 Statement (Clean)
			Col5: R2 Description (Clean)
			Col6: See Also in R1 PHR-S FM
			Col7: R1 CC#
			Col8: R2 Conformance Criteria (Clean)

		For-each(Row3+)	    	    
	    Col2 = 'T' 
		   <object>
		      <id>[Row#]</id>
		      <name>[Col3]</name>
		      <alias>[Col1]</alias>
		      <notes>$OV$[col4]$EX$$AC$</notes>
		      <stereotype>Section</stereotype>
		      <type>Package</type>
		      <parentId>??</parentId>
		      <tag name="ID" value="??" />
		    </object>
		   <object>
		      <id>5</id>
		      <name>Overarching</name>
		      <alias>OV</alias>
		      <notes>$OV$The Overarching Section contains Conformance Criteria that apply to all EHR Systems and consequently must be included in all EHR-S FM compliant profiles.  These criteria are grouped under a single Function.All functions within the Overarching Section have an identifier starting with "O".$EX$$AC$</notes>
		      <stereotype>Section</stereotype>
		      <type>Package</type>
		      <parentId>4</parentId>
		      <tag name="ID" value="1" />
		    </object>
	    Col2 = 'H'/'F'
		    <object>
		      <id>[Row#]</id>
		      <name>[Col1] [Col3]</name>
		      <alias>[Col1]</alias>
		      <notes>$ST$[Col4]$DE$[Col5]$EX$[part of Col5]</notes>
		      <stereotype>['H'>Header:'F'>Function]</stereotype>
		      <type>Feature</type>
		      <parentId>[base on Col1]</parentId>
		      <tag name="Row" value="[Row#]" />
		      <tag name="Reference.ChangeInfo" value="??" />
		    </object>
		    <object>
		      <id>6</id>
		      <name>OV.1 Overarching Criteria</name>
		      <alias>OV.1</alias>
		      <notes>$ST$Overarching criteria are those that apply to all EHR Systems.$DE$The Overarching Section contains Conformance Criteria that apply to all EHR Systems and consequently must be included in all EHR-S FM compliant profiles. These criteria are grouped under a single Function.$EX$</notes>
		      <stereotype>Function</stereotype>
		      <type>Feature</type>
		      <parentId>5</parentId>
		      <tag name="Row" value="1" />
		      <tag name="Reference.ChangeInfo" value="New" />
		    </object>
	    Col2 = 'C'	   
		    <object>
		      <id>[Row#]</id>
		      <name>[Col1]#[Col7]</name>
		      <notes>[Col8]</notes>
		      <stereotype>Criteria</stereotype>
		      <type>Requirement</type>
		      <parentId>[base on Col1]</parentId>
		      <tag name="Row" value="[Row#]" />
		      <tag name="Optionality" value="??" />
		      <tag name="Conditional" value="??" />
		      <tag name="Dependent" value="??" />
		    </object>
		    <object>
		      <id>7</id>
		      <name>OV.1#01</name>
		      <notes>The system SHALL conform to function CP.9.1 (Produce a Summary Record of Care).</notes>
		      <stereotype>Criteria</stereotype>
		      <type>Requirement</type>
		      <parentId>6</parentId>
		      <tag name="Row" value="2" />
		      <tag name="Optionality" value="SHALL" />
		      <tag name="Conditional" value="N" />
		      <tag name="Dependent" value="N" />
		    </object>
     -->
    
    <xsl:template match="/">
       	<max:model>
       		<!-- <xsl:copy-of select="$mappings"/> -->
	    	<objects>
			<object>
			      <id>2</id>
			      <name><xsl:value-of select="//table:table[@table:name='Model']/table:table-row[1]/table:table-cell[1]/text:p"/> (import <xsl:value-of select="current-date()"/>)</name>
			      <notes>&lt;b&gt;
&lt;font size="16"&gt;EHR-S Function List Components&lt;/font&gt;
&lt;/b&gt;

The Function List includes the following components:  

&lt;b&gt;Function ID # (Normative)&lt;/b&gt;
This is the unique identifier of a function in the Function List (e.g. CP.1.1) and should be used to uniquely identify the function when referencing functions.  The Function ID also serves to identify the section within which the function exists (CP = Care Provision Section) and the hierarchy or relationship between functions (CP.1.1 is a sibling to CP.1.2, parent of CP.1.1.1 and child of CP.1).  In many cases the parent is fully expressed by the children.  

&lt;b&gt;Function Type (Reference)&lt;/b&gt;
Indication of the line item as being a header (H) or function (F) or conformance criteria.

&lt;b&gt;Function Name (Normative) &lt;/b&gt;
This is the name of the Function and whilst expected to be unique within the Function List; it is not recommended to be used to identify the function without being accompanied by the Function ID. 
Example: Manage Medication List 

&lt;b&gt;Function Statement (Normative)&lt;/b&gt;
This is a brief statement of the purpose of this function.  Whist not restricted to the use of structured language that is used in the Conformance Criteria (see below); the Statement should clearly identify the purpose and scope of the function.  
Example: Create and maintain patient-specific medication lists.

&lt;b&gt;Description (Reference)&lt;/b&gt;
This is a more detailed description of the function, including examples if needed.  
Example: Medication lists are managed over time, whether over the course of a visit or stay, or the lifetime of a patient. All pertinent dates, including medication start, modification, and end dates are stored. The entire medication history for any medication, including alternative supplements and herbal medications, is viewable. Medication lists are not limited to medication orders recorded by providers, but may include, for example, pharmacy dispense/supply records, patient-reported medications and additional information such as age specific dosage.

&lt;b&gt;Conformance Criteria (Normative) &lt;/b&gt;
Each function in the Function List includes one or more Conformance Criteria.  A Conformance Criteria, which exists as normative language in this standard, defines the requirements for conforming to the function.  The language used to express a conformance criterion is highly structured with standardized components with set meanings.  The structured language used to define conformance clauses in the Function List are defined in the Glossary (Chapter 4).

&lt;b&gt;R1.1 Reference (Reference)&lt;/b&gt;
Reference to the previous version of the Functional Model is included to support transition from one version to the next.   The first 2 digits indicate the source document;  FM = Functional Model, LM = Lifecycle Model.  The remainder of the reference is to the function and, if applicable, conformance criteria.

&lt;b&gt;Change Indicator&lt;/b&gt;
The change indicator shows the change from previous versions.  This will be valued as follows:
   C   - Changed
   D   - Deleted
   N   - New
   NC - No Change

&lt;b&gt;R2 Row #&lt;/b&gt;
A unique number for the row within the section. 
			</notes>
			      <stereotype>HL7-FM</stereotype>
			      <type>Package</type>
			    </object>	    
	    		<xsl:apply-templates mode="objects"/>
	    	</objects>
	    	<relationships>
    			<xsl:apply-templates mode="relationships"/>
			</relationships>    			
    	</max:model>
    </xsl:template>
    
    <xsl:template match="//table:table[@table:name='Model']/table:table-row" mode="objects">
   		<xsl:variable name="rownr" select="table:table-cell[1]/text:p"/>
   		<xsl:variable name="_parentId" select="$mappings//map[id=$rownr]/parentId"/>
   		<xsl:variable name="rowid" select="normalize-space(table:table-cell[2]/text:p)"/>
   		<xsl:variable name="rowtype" select="table:table-cell[3]/text:p"/>
   		<xsl:variable name="col3" select="table:table-cell[4]/text:p"/>
   		<xsl:variable name="col4" select="table:table-cell[5]/text:p"/>
   		<xsl:variable name="col5" select="table:table-cell[6]/text:p"/>
   		<xsl:variable name="col6" select="table:table-cell[7]/text:p"/>
   		<xsl:variable name="col7" select="max:getColumnValue(.,11,1)"/><!-- Reference.Alias -->
   		<xsl:variable name="col8" select="max:getColumnValue(.,12,1)"/><!-- Reference.xxxID -->
   		<xsl:variable name="col9" select="max:getColumnValue(.,13,1)"/><!-- Reference.ChangeIndicator -->   		
   		
   		<xsl:choose>
   			<xsl:when test="$rowtype='T'">
			   <object>
			      <id><xsl:value-of select="$rownr"/></id>
			      <name><xsl:value-of select="$col3"/></name>
			      <alias><xsl:value-of select="$rowid"/></alias>
			      <notes>$OV$<xsl:value-of select="$col4"/>$EX$$AC$</notes>
			      <stereotype>Section</stereotype>
			      <type>Package</type>
			      <parentId><xsl:value-of select="$_parentId"/></parentId>
			      <tag name="ID"><xsl:attribute name="value" select="$rownr"/></tag>
			    </object>
   			</xsl:when>
   			<xsl:when test="$rowtype='H'">
			    <object>
			      <id><xsl:value-of select="$rownr"/></id>
			      <name><xsl:value-of select="concat($rowid,' ',$col3)"/></name>
			      <alias><xsl:value-of select="$rowid"/></alias>
			      <notes>$ST$<xsl:value-of select="$col4"/>$DE$<xsl:value-of select="$col5"/>$EX$</notes>
			      <stereotype>Header</stereotype>
			      <type>Feature</type>
			      <parentId><xsl:value-of select="$_parentId"/></parentId>
			      <tag name="Row"><xsl:attribute name="value" select="$rownr"/></tag>
			      <xsl:if test="not($col7='')">
					  <tag name="Reference.Alias"><xsl:attribute name="value" select="$col7"/></tag>
					  <tag name="Reference.FunctionID"><xsl:attribute name="value" select="$col8"/></tag>
				  </xsl:if>
				  <xsl:if test="not($col9='')"><tag name="Reference.ChangeIndicator"><xsl:attribute name="value" select="$col9"/></tag></xsl:if>
			    </object>
   			</xsl:when>
   			<xsl:when test="$rowtype='F'">
			    <object>
			      <id><xsl:value-of select="$rownr"/></id>
			      <name><xsl:value-of select="concat($rowid,' ',$col3)"/></name>
			      <alias><xsl:value-of select="$rowid"/></alias>
			      <notes>$ST$<xsl:value-of select="$col4"/>$DE$<xsl:value-of select="$col5"/>$EX$</notes>
			      <stereotype>Function</stereotype>
			      <type>Feature</type>
			      <parentId><xsl:value-of select="$_parentId"/></parentId>
			      <tag name="Row"><xsl:attribute name="value" select="$rownr"/></tag>
			      <xsl:if test="not($col7='')">
					  <tag name="Reference.Alias"><xsl:attribute name="value" select="$col7"/></tag>
					  <tag name="Reference.FunctionID"><xsl:attribute name="value" select="$col8"/></tag>
				  </xsl:if>
				  <xsl:if test="not($col9='')"><tag name="Reference.ChangeIndicator"><xsl:attribute name="value" select="$col9"/></tag></xsl:if>
			    </object>
   			</xsl:when>
   			<xsl:when test="$rowtype='C'">
			    <object>
			      <id><xsl:value-of select="$rownr"/></id>
			      <name><xsl:choose><xsl:when test="$col5 castable as xs:numeric"><xsl:value-of select="concat($rowid,'#',format-number($col5,'00'))"/></xsl:when>
			      	<xsl:otherwise><xsl:value-of select="concat($rowid,'#',format-number($rownr,'00'))"/></xsl:otherwise>
			      </xsl:choose></name>
			      <notes><xsl:value-of select="$col6"/></notes>
			      <stereotype>Criteria</stereotype>
			      <type>Requirement</type>
			      <parentId><xsl:value-of select="$_parentId"/></parentId>
			      <tag name="Row"><xsl:attribute name="value" select="$rownr"/></tag>
			      <tag name="Optionality"><xsl:choose>
			      	<xsl:when test="contains($col6[1],' SHALL ')"><xsl:attribute name="value" select="'SHALL'"/></xsl:when>
			      	<xsl:when test="contains($col6[1],' SHOULD ')"><xsl:attribute name="value" select="'SHOULD'"/></xsl:when>
			      	<xsl:when test="contains($col6[1],' MAY ')"><xsl:attribute name="value" select="'MAY'"/></xsl:when>
			      	</xsl:choose></tag>
			      <tag name="Conditional"><xsl:choose>
			      	<xsl:when test="starts-with($col6[1],'IF ')"><xsl:attribute name="value" select="'Y'"/></xsl:when>
			      	<xsl:otherwise><xsl:attribute name="value" select="'N'"/></xsl:otherwise></xsl:choose></tag>
			      <tag name="Dependent"><xsl:choose>
			      	<xsl:when test="contains($col6[1],' according to organizational policy and/or jurisdictional law')"><xsl:attribute name="value" select="'Y'"/></xsl:when>
			      	<xsl:otherwise><xsl:attribute name="value" select="'N'"/></xsl:otherwise></xsl:choose></tag>
			      <xsl:if test="not($col7='')">
					  <tag name="Reference.Alias"><xsl:attribute name="value" select="$col7"/></tag>
				  	  <tag name="Reference.FunctionID"><xsl:attribute name="value" select="normalize-space(substring-before($col8,'cc#'))"/></tag>
					  <tag name="Reference.CriteriaID"><xsl:attribute name="value" select="substring-after($col8,'cc#')"/></tag>
				  </xsl:if>
				  <xsl:if test="not($col9='')"><tag name="Reference.ChangeIndicator"><xsl:attribute name="value" select="$col9"/></tag></xsl:if>
			    </object>
			 </xsl:when>
   		</xsl:choose>    	
    </xsl:template>

	<!--     
		[Col6].contains('conform to function CP.9.1')
		lookup id (=Row#) of referenced function 
		<relationship>
         	<label/>
         	<sourceId>6</sourceId> is function id with this criteria (@parentId)
         	<sourceLabel/>
         	<sourceCard/>
         	<destId>621</destId> id referenced function
         	<destLabel/>
         	<destCard/>
         	<notes>OV.1#01 is the source of this link</notes>
         	<stereotype>ConsequenceLink</stereotype>
         	<type>Dependency</type>
      	</relationship>
    -->
    <xsl:template match="//table:table[@table:name='Model']/table:table-row" mode="relationships">
   		<xsl:variable name="rownr" select="table:table-cell[1]/text:p"/>
   		<xsl:variable name="_parentId" select="$mappings//map[id=$rownr]/parentId"/>
   		<xsl:variable name="rowid" select="normalize-space(table:table-cell[2]/text:p)"/>
   		<xsl:variable name="rowtype" select="table:table-cell[3]/text:p"/>
   		<xsl:variable name="col3" select="table:table-cell[4]/text:p"/>
   		<xsl:variable name="col4" select="table:table-cell[5]/text:p"/>
   		<xsl:variable name="col5" select="table:table-cell[6]/text:p"/>
   		<xsl:variable name="col6" select="table:table-cell[7]/text:p"/>
   		<xsl:variable name="col7" select="table:table-cell[8]/text:p"/>
   		<xsl:variable name="col8" select="table:table-cell[9]/text:p"/>
   		<xsl:choose>   		
   			<xsl:when test="$rowtype='C'">
   				<!-- all Criteria have Association to Function -->
	  			<relationship>
			     	<sourceId><xsl:value-of select="$rownr"/></sourceId>
			      	<destId><xsl:value-of select="$_parentId"/></destId>
			      	<type>Association</type>
	  			</relationship>
   				<xsl:if test="contains($col6[1], ' conform to function ')">
   					<xsl:variable name="other" select="substring-before(substring-after($col6[1], ' conform to function '), ' ')"></xsl:variable>
					<xsl:variable name="source">
						<xsl:choose><xsl:when test="$col5 castable as xs:numeric"><xsl:value-of select="concat($rowid,'#',format-number($col5,'00'))"/></xsl:when>
			      			<xsl:otherwise><xsl:value-of select="concat($rowid,'#',format-number($rownr,'00'))"/></xsl:otherwise>
			      		</xsl:choose>
			      	</xsl:variable>   					
	   				<relationship>
				      <sourceId><xsl:value-of select="$_parentId"/></sourceId>
				      <destId><xsl:value-of select="$mappings_tmp//map[fmid=$other]/id"/></destId>
				      <notes><xsl:value-of select="concat($source, ' is the source of this link')"/></notes>
				      <stereotype>ConsequenceLink</stereotype>
				      <type>Dependency</type>
	   				</relationship>
   				</xsl:if>
   				<xsl:if test="$col4!='-'">
   					<xsl:variable name="other" select="$col4"></xsl:variable>
					<xsl:variable name="source">
						<xsl:choose><xsl:when test="$col5 castable as xs:numeric"><xsl:value-of select="concat($rowid,'#',format-number($col5,'00'))"/></xsl:when>
			      			<xsl:otherwise><xsl:value-of select="concat($rowid,'#',format-number($rownr,'00'))"/></xsl:otherwise>
			      		</xsl:choose>
			      	</xsl:variable>   					
	   				<relationship>
				      <sourceId><xsl:value-of select="$_parentId"/></sourceId>
				      <destId><xsl:value-of select="$mappings_tmp//map[fmid=$other]/id"/></destId>
				      <notes><xsl:value-of select="concat($source, ' is the source of this see also')"/></notes>
				      <stereotype>SeeAlso</stereotype>
				      <type>Association</type>
	   				</relationship>
   				</xsl:if>
			 </xsl:when>
   		</xsl:choose>    	
    </xsl:template>
        
    <xsl:template match="text()" mode="objects"/>

    <xsl:template match="text()" mode="relationships"/>

<!-- CREATE MAPPING LOOKUP TABLES -->

	<xsl:variable name="mappings_tmp">
		<mappings>
			<xsl:call-template name="pass1"></xsl:call-template>
		</mappings>
	</xsl:variable>

	<xsl:variable name="mappings">
		<mappings>
			<xsl:call-template name="pass2"></xsl:call-template>
		</mappings>
	</xsl:variable>
    
    <xsl:template name="pass1">
    	<xsl:for-each select="//table:table[@table:name='Model']/table:table-row">
	   		<xsl:variable name="rownr" select="table:table-cell[1]/text:p"/>
	   		<xsl:variable name="rowid" select="normalize-space(table:table-cell[2]/text:p)"/>
	   		<xsl:variable name="rowtype" select="table:table-cell[3]/text:p"/>
	   		<xsl:variable name="col5" select="table:table-cell[6]/text:p"/>

   			<xsl:if test="$rowtype='T' or $rowtype='H' or $rowtype='F' or $rowtype='C'">
			    <xsl:variable name="_fmid">
		   			<xsl:if test="$rowtype='T' or $rowtype='H' or $rowtype='F'">
			   			<xsl:value-of select="$rowid"/>
				   	</xsl:if>
		   			<xsl:if test="$rowtype='C'">
				      <xsl:choose><xsl:when test="$col5 castable as xs:numeric"><xsl:value-of select="concat($rowid,'#',format-number($col5,'00'))"/></xsl:when>
				      	<xsl:otherwise><xsl:value-of select="concat($rowid,'#',format-number($rownr,'00'))"/></xsl:otherwise>
				      </xsl:choose>
		   			</xsl:if>
		   		</xsl:variable>
		   		<map>
		   			<id><xsl:value-of select="$rownr"/></id>
		   			<fmid><xsl:value-of select="$_fmid"/></fmid>
		   		</map>
		   	</xsl:if>
   		</xsl:for-each>
	</xsl:template>
	
    <xsl:template name="pass2">
    	<xsl:for-each select="//table:table[@table:name='Model']/table:table-row">
	   		<xsl:variable name="rownr" select="table:table-cell[1]/text:p"/>
	   		<xsl:variable name="rowid" select="normalize-space(table:table-cell[2]/text:p)"/>
	   		<xsl:variable name="rowtype" select="table:table-cell[3]/text:p"/>
	   		<xsl:variable name="col5" select="table:table-cell[6]/text:p"/>

   			<xsl:if test="$rowtype='T' or $rowtype='H' or $rowtype='F' or $rowtype='C'">
			    <xsl:variable name="_fmid">
		   			<xsl:if test="$rowtype='T' or $rowtype='H' or $rowtype='F'">
			   			<xsl:value-of select="$rowid"/>
				   	</xsl:if>
		   			<xsl:if test="$rowtype='C'">
				      <xsl:choose><xsl:when test="$col5 castable as xs:numeric"><xsl:value-of select="concat($rowid,'#',format-number($col5,'00'))"/></xsl:when>
				      	<xsl:otherwise><xsl:value-of select="concat($rowid,'#',format-number($rownr,'00'))"/></xsl:otherwise>
				      </xsl:choose>
		   			</xsl:if>
		   		</xsl:variable>
				<xsl:variable name="_parentId">
		   			<xsl:if test="$rowtype='T'">2</xsl:if>
		   			<xsl:if test="$rowtype='H' or $rowtype='F'">
		   				<xsl:variable name="parentfmid" select="replace($_fmid,'(.+)\.\d+','$1')"/>
			   			<xsl:value-of select="$mappings_tmp//map[fmid=$parentfmid]/id"/>
		   			</xsl:if>
			   		<xsl:if test="$rowtype='C'">
			   			<xsl:value-of select="$mappings_tmp//map[fmid=$rowid]/id"/>
			   		</xsl:if>
				</xsl:variable>
		   		<map>
		   			<id><xsl:value-of select="$rownr"/></id>
		   			<parentId><xsl:value-of select="$_parentId"/></parentId>
		   			<fmid><xsl:value-of select="$_fmid"/></fmid>
		   		</map>
		   	</xsl:if>
   		</xsl:for-each>
	</xsl:template>
	
	<xsl:function name="max:getColumnValue">
      <xsl:param name="tableRow" as="node()"/>
      <xsl:param name="colIndex"/>
      <xsl:param name="currentIndex"/>
      <xsl:choose>
         <xsl:when test="$currentIndex &lt; $colIndex">
            <xsl:variable name="repeatColumns" select="$tableRow/table:table-cell[$currentIndex]/@table:number-columns-repeated"/>
            <xsl:choose>
               <xsl:when test="$repeatColumns">
                  <xsl:choose>
                     <xsl:when test="$currentIndex + $repeatColumns - 1 &gt;= $colIndex"><xsl:value-of select="$tableRow/table:table-cell[$currentIndex]/text:p"/></xsl:when>
                     <xsl:otherwise><xsl:value-of select="max:getColumnValue($tableRow, $colIndex - $repeatColumns + 1, $currentIndex + 1)"/></xsl:otherwise>
                  </xsl:choose>
               </xsl:when>
               <xsl:otherwise><xsl:value-of select="max:getColumnValue($tableRow, $colIndex, $currentIndex + 1)"/></xsl:otherwise>
            </xsl:choose>
         </xsl:when>
         <xsl:otherwise><xsl:value-of select="$tableRow/table:table-cell[$colIndex]/text:p"/></xsl:otherwise>
      </xsl:choose>
   	</xsl:function>	
	    
</xsl:stylesheet>
