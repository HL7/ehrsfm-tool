<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="3.0"
	xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet"
	xmlns:xs="http://www.w3.org/2001/XMLSchema"
	xmlns:max="http://www.umcg.nl/MAX"
	exclude-result-prefixes="ss xs">

    <xsl:output indent="yes" method="xml"/>
    
    <!--
		v1 26-oct-2018
		- based on fm-fods2max v4
    	- added SeeAlso links Header/Function
     -->
    
    <!--
    	Input XML Spreadsheet 2003 snippet:
    	 
 <Worksheet ss:Name="Model">
  <Names>
   <NamedRange ss:Name="_FilterDatabase" ss:RefersTo="=Model!R2C1:R1011C13"
    ss:Hidden="1"/>
  </Names>
  <Table ss:ExpandedColumnCount="1025" ss:ExpandedRowCount="1011"
   x:FullColumns="1" x:FullRows="1">
   <Column ss:StyleID="s44" ss:AutoFitWidth="0" ss:Width="45.75"/>
   <Row ss:Height="15">
    <Cell ss:StyleID="s45"><Data ss:Type="String">PHR-S FM Release 2 (Normative Ballot 1) N1 JAN2018</ss:Data></ss:Cell>
    <Cell ss:Index="3" ss:StyleID="s45"/>
    <Cell ss:StyleID="s45"/>
    <Cell ss:StyleID="s45"/>
   </Row>
   	-->
  		    
    <xsl:template match="/">
       	<max:model>
       		<!-- <xsl:copy-of select="$mappings"/> -->
	    	<objects>
			<object>
			      <id>2</id>
			      <name><xsl:value-of select="//ss:Worksheet[@ss:Name='Model']/ss:Table/ss:Row[1]/ss:Cell[1]/ss:Data"/> (import <xsl:value-of select="current-date()"/>)</name>
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
    
    <xsl:template match="//ss:Worksheet[@ss:Name='Model']/ss:Table/ss:Row" mode="objects">
   		<xsl:variable name="rownr" select="ss:Cell[1]/ss:Data"/>
   		<xsl:variable name="_parentId" select="$mappings//map[id=$rownr]/parentId"/>
   		<xsl:variable name="rowid" select="normalize-space(ss:Cell[2]/ss:Data)"/>
   		<xsl:variable name="rowtype" select="ss:Cell[3]/ss:Data"/>
   		<xsl:variable name="col3" select="ss:Cell[4]/ss:Data"/>
   		<xsl:variable name="col4" select="ss:Cell[5]/ss:Data"/>
   		<xsl:variable name="col5" select="ss:Cell[6]/ss:Data"/>
   		<xsl:variable name="col6" select="normalize-space(ss:Cell[7]/ss:Data)"/>
   		<xsl:variable name="col7" select="normalize-space(ss:Cell[8]/ss:Data)"/>
   		<xsl:variable name="col_9" select="normalize-space(ss:Cell[9]/ss:Data)"/>
   		<xsl:variable name="col_10" select="normalize-space(ss:Cell[10]/ss:Data)"/>
   		<xsl:variable name="col_11" select="normalize-space(ss:Cell[11]/ss:Data)"/>
   		
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
			      <xsl:if test="not($col_9='')">
					  <tag name="Reference.Alias"><xsl:attribute name="value" select="$col_9"/></tag>
					  <tag name="Reference.FunctionID"><xsl:attribute name="value" select="$col_10"/></tag>
				  </xsl:if>
				  <xsl:if test="not($col_11='')"><tag name="Reference.ChangeIndicator"><xsl:attribute name="value" select="$col_11"/></tag></xsl:if>
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
			      <xsl:if test="not($col_9='')">
					  <tag name="Reference.Alias"><xsl:attribute name="value" select="$col_9"/></tag>
					  <tag name="Reference.FunctionID"><xsl:attribute name="value" select="$col_10"/></tag>
				  </xsl:if>
				  <xsl:if test="not($col_11='')"><tag name="Reference.ChangeIndicator"><xsl:attribute name="value" select="$col_11"/></tag></xsl:if>
			    </object>
   			</xsl:when>
   			<xsl:when test="$rowtype='C'">
			    <object>
			      <id><xsl:value-of select="$rownr"/></id>
			      <name><xsl:choose><xsl:when test="$col4 castable as xs:numeric"><xsl:value-of select="concat($rowid,'#',format-number($col4,'00'))"/></xsl:when>
			      	<xsl:otherwise><xsl:value-of select="concat($rowid,'#',format-number($rownr,'00'))"/></xsl:otherwise>
			      </xsl:choose></name>
			      <notes><xsl:value-of select="$col5"/></notes>
			      <stereotype>Criteria</stereotype>
			      <type>Requirement</type>
			      <parentId><xsl:value-of select="$_parentId"/></parentId>
			      <tag name="Row"><xsl:attribute name="value" select="$rownr"/></tag>
			      <tag name="Optionality"><xsl:choose>
			      	<xsl:when test="contains($col5[1],' SHALL ')"><xsl:attribute name="value" select="'SHALL'"/></xsl:when>
			      	<xsl:when test="contains($col5[1],' SHOULD ')"><xsl:attribute name="value" select="'SHOULD'"/></xsl:when>
			      	<xsl:when test="contains($col5[1],' MAY ')"><xsl:attribute name="value" select="'MAY'"/></xsl:when>
			      	</xsl:choose></tag>
			      <tag name="Conditional"><xsl:choose>
			      	<xsl:when test="starts-with($col5[1],'IF ')"><xsl:attribute name="value" select="'Y'"/></xsl:when>
			      	<xsl:otherwise><xsl:attribute name="value" select="'N'"/></xsl:otherwise></xsl:choose></tag>
			      <tag name="Dependent"><xsl:choose>
			      	<xsl:when test="contains($col5[1],' according to organizational policy and/or jurisdictional law')"><xsl:attribute name="value" select="'Y'"/></xsl:when>
			      	<xsl:otherwise><xsl:attribute name="value" select="'N'"/></xsl:otherwise></xsl:choose></tag>
			      <xsl:if test="not($col6='')">
					  <tag name="Reference.Alias"><xsl:attribute name="value" select="$col6"/></tag>
				  	  <tag name="Reference.FunctionID"><xsl:attribute name="value" select="normalize-space(substring-before($col7,'cc#'))"/></tag>
					  <tag name="Reference.CriteriaID"><xsl:attribute name="value" select="substring-after($col7,'cc#')"/></tag>
				  </xsl:if>
				  <xsl:if test="not($col_9='')"><tag name="Reference.ChangeIndicator"><xsl:attribute name="value" select="$col_9"/></tag></xsl:if>
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
    <xsl:template match="//ss:Worksheet[@ss:Name='Model']/ss:Table/ss:Row" mode="relationships">
   		<xsl:variable name="rownr" select="ss:Cell[1]/ss:Data"/>
   		<xsl:variable name="_parentId" select="$mappings//map[id=$rownr]/parentId"/>
   		<xsl:variable name="rowid" select="normalize-space(ss:Cell[2]/ss:Data)"/>
   		<xsl:variable name="rowtype" select="ss:Cell[3]/ss:Data"/>
   		<xsl:variable name="col3" select="ss:Cell[4]/ss:Data"/>
   		<xsl:variable name="col4" select="ss:Cell[5]/ss:Data"/>
   		<xsl:variable name="col5" select="ss:Cell[6]/ss:Data"/>
   		<xsl:variable name="col6" select="ss:Cell[7]/ss:Data"/>
   		<xsl:variable name="col7" select="ss:Cell[8]/ss:Data"/>
   		<xsl:variable name="col8" select="ss:Cell[9]/ss:Data"/>
   		<xsl:choose>   		
   			<xsl:when test="$rowtype='H' or $rowtype='F'">
   				<xsl:if test="$col6!='-'">
   					<xsl:variable name="other" select="$col6"></xsl:variable>
	   				<relationship>
				      <sourceId><xsl:value-of select="$_parentId"/></sourceId>
				      <destId><xsl:value-of select="$mappings_tmp//map[fmid=$other]/id"/></destId>
				      <notes><xsl:value-of select="concat($rowid, ' is the source of this see also')"/></notes>
				      <stereotype>SeeAlso</stereotype>
				      <type>Association</type>
	   				</relationship>
   				</xsl:if>
			 </xsl:when>
   			<xsl:when test="$rowtype='C'">
   				<!-- all Criteria have Association to Function -->
	  			<relationship>
			     	<sourceId><xsl:value-of select="$rownr"/></sourceId>
			      	<destId><xsl:value-of select="$_parentId"/></destId>
			      	<type>Association</type>
	  			</relationship>
   				<xsl:if test="contains($col5[1], ' conform to function ')">
   					<xsl:variable name="other" select="substring-before(substring-after($col5[1], ' conform to function '), ' ')"></xsl:variable>
					<xsl:variable name="source">
						<xsl:choose><xsl:when test="$col4 castable as xs:numeric"><xsl:value-of select="concat($rowid,'#',format-number($col4,'00'))"/></xsl:when>
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
   				<xsl:if test="$col3!='-'">
   					<xsl:variable name="other" select="$col3"></xsl:variable>
					<xsl:variable name="source">
						<xsl:choose><xsl:when test="$col4 castable as xs:numeric"><xsl:value-of select="concat($rowid,'#',format-number($col4,'00'))"/></xsl:when>
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
    	<xsl:for-each select="//ss:Worksheet[@ss:Name='Model']/ss:Table/ss:Row">
	   		<xsl:variable name="rownr" select="ss:Cell[1]/ss:Data"/>
	   		<xsl:variable name="rowid" select="normalize-space(ss:Cell[2]/ss:Data)"/>
	   		<xsl:variable name="rowtype" select="ss:Cell[3]/ss:Data"/>
	   		<xsl:variable name="col5" select="ss:Cell[6]/ss:Data"/>

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
    	<xsl:for-each select="//ss:Worksheet[@ss:Name='Model']/ss:Table/ss:Row">
	   		<xsl:variable name="rownr" select="ss:Cell[1]/ss:Data"/>
	   		<xsl:variable name="rowid" select="normalize-space(ss:Cell[2]/ss:Data)"/>
	   		<xsl:variable name="rowtype" select="ss:Cell[3]/ss:Data"/>
	   		<xsl:variable name="col5" select="ss:Cell[6]/ss:Data"/>

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
</xsl:stylesheet>
