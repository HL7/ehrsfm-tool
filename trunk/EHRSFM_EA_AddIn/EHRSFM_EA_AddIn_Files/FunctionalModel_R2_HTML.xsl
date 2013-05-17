<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" >
	<xsl:output method="html" omit-xml-declaration="yes" doctype-public="-//W3C//DTD XHTML 1.0 Transitional//EN" doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd" indent="yes"/>
	<xsl:preserve-space elements="*"/>
	<!--<xsl:output version="1.0" method="html" indent="no" encoding="UTF-8"/>-->
	<xsl:param name="SV_OutputFormat" select="'html'"/>


	<xsl:key name="ReferenceIDKey" match="//ReferenceInfo" use="Alias"/>	
	<xsl:key name="ReferenceFunctionIDKey" match="Function" use="Alias"/>
	<xsl:key name="CriteriaFunctionIDKey" match="Function" use="Alias"/>
	
	<xsl:key name="FunctionParentIDKey" match="Function" use="ParentID"/>
	
	<xsl:variable name="ReferenceChangeID">1</xsl:variable> <!-- change code for R 1.1 -->


	<xsl:variable name="IncludeSeeAlso">no</xsl:variable>
	<xsl:variable name="IncludeChgIndic">yes</xsl:variable>
	<xsl:variable name="includeGlossary">no</xsl:variable>

	<xsl:variable name="ShowReferencesInOneColumn">yes</xsl:variable>

	<xsl:variable name="TopTableHeaderColor">#ccccff</xsl:variable>
	<xsl:variable name="PrimaryAlternatingTableColor">#eeeeff</xsl:variable>
	<xsl:variable name="SecondaryAlternatingTableColor">#ffffff</xsl:variable>

	<xsl:variable name="HeaderColor">#666666</xsl:variable>
	<xsl:variable name="FooterColor">#666666</xsl:variable>
	<xsl:variable name="ChapterNumber">3</xsl:variable>

	<xsl:variable name="ColWidth1">325</xsl:variable>
	<xsl:variable name="ColWidth2">600</xsl:variable>
	<xsl:variable name="ColWidth3">66</xsl:variable>
	<xsl:variable name="ColWidthReference">70</xsl:variable>
	<xsl:variable name="ColWidth7">30</xsl:variable>
	<xsl:variable name="ColWidth8">30</xsl:variable>
	<xsl:variable name="TableWidth">
		<xsl:value-of select="$ColWidth1 + $ColWidth2 + $ColWidth3 + $ColWidthReference + $ColWidthReference + $ColWidthReference + $ColWidth7 + $ColWidth8"/>
	</xsl:variable>
<!--
	<xsl:variable name="ReferenceID1">
		<xsl:value-of select="/FunctionalModel/ReferenceInfo[1]/@ID"/>
	</xsl:variable>
	<xsl:variable name="ReferenceID2">
		<xsl:value-of select="/FunctionalModel/ReferenceInfo[2]/@ID"/>
	</xsl:variable>
	<xsl:variable name="ReferenceID3">
		<xsl:value-of select="/FunctionalModel/ReferenceInfo[3]/@ID"/>
	</xsl:variable>


	<xsl:variable name="ReferenceAlias1">
		<xsl:for-each select="/FunctionalModel/ReferenceInfo">
			<xsl:if test="@ID=$ReferenceID1">
				<xsl:value-of select="Alias"/>
			</xsl:if>
		</xsl:for-each>
	</xsl:variable>
	<xsl:variable name="ReferenceAlias2">
		<xsl:for-each select="/FunctionalModel/ReferenceInfo">
			<xsl:if test="@ID=$ReferenceID2">
				<xsl:value-of select="Alias"/>
			</xsl:if>
		</xsl:for-each>
	</xsl:variable>
	<xsl:variable name="ReferenceAlias3">
		<xsl:for-each select="/FunctionalModel/ReferenceInfo">
			<xsl:if test="@ID=$ReferenceID3">
				<xsl:value-of select="Alias"/>
			</xsl:if>
		</xsl:for-each>
	</xsl:variable>
	
	<xsl:variable name="ReferenceCaption1">
		<xsl:for-each select="/FunctionalModel/ReferenceInfo">
			<xsl:if test="@ID=$ReferenceID1">
				<xsl:value-of select="Caption"/>
			</xsl:if>
		</xsl:for-each>
	</xsl:variable>
	<xsl:variable name="ReferenceCaption2">
		<xsl:for-each select="/FunctionalModel/ReferenceInfo">
			<xsl:if test="@ID=$ReferenceID2">
				<xsl:value-of select="Caption"/>
			</xsl:if>
		</xsl:for-each>
	</xsl:variable>
	<xsl:variable name="ReferenceCaption3">
		<xsl:for-each select="/FunctionalModel/ReferenceInfo">
			<xsl:if test="@ID=$ReferenceID3">
				<xsl:value-of select="Caption"/>
			</xsl:if>
		</xsl:for-each>
	</xsl:variable>
	
	
	<xsl:variable name="ReferenceURI1">
		<xsl:for-each select="/FunctionalModel/ReferenceInfo">
			<xsl:if test="@ID=$ReferenceID1">
				<xsl:value-of select="URI"/>
			</xsl:if>
		</xsl:for-each>
	</xsl:variable>
	<xsl:variable name="ReferenceURI2">
		<xsl:for-each select="/FunctionalModel/ReferenceInfo">
			<xsl:if test="@ID=$ReferenceID2">
				<xsl:value-of select="URI"/>
			</xsl:if>
		</xsl:for-each>
	</xsl:variable>
	<xsl:variable name="ReferenceURI3">
		<xsl:for-each select="/FunctionalModel/ReferenceInfo">
			<xsl:if test="@ID=$ReferenceID3">
				<xsl:value-of select="URI"/>
			</xsl:if>
		</xsl:for-each>
	</xsl:variable>

-->
		<xsl:variable name="ReferenceID1"><xsl:value-of select="/FunctionalModel/ReferenceInfo[1]/@ID"/></xsl:variable>
	<xsl:variable name="ReferenceID2"><xsl:value-of select="/FunctionalModel/ReferenceInfo[2]/@ID"/></xsl:variable>
	<xsl:variable name="ReferenceID3"><xsl:value-of select="/FunctionalModel/ReferenceInfo[3]/@ID"/></xsl:variable>


	<xsl:variable name="ReferenceCaption1"><xsl:for-each select="/FunctionalModel/ReferenceInfo[ReferenceType='External']"><xsl:if test="@ID=$ReferenceID1"><xsl:value-of select="Caption"/></xsl:if></xsl:for-each></xsl:variable>
	<xsl:variable name="ReferenceCaption2"><xsl:for-each select="/FunctionalModel/ReferenceInfo[ReferenceType='External']"><xsl:if test="@ID=$ReferenceID2"><xsl:value-of select="Caption"/></xsl:if></xsl:for-each></xsl:variable>
	<xsl:variable name="ReferenceCaption3"><xsl:for-each select="/FunctionalModel/ReferenceInfo[ReferenceType='External']"><xsl:if test="@ID=$ReferenceID3"><xsl:value-of select="Caption"/></xsl:if></xsl:for-each></xsl:variable>
	
	<xsl:variable name="ReferenceAlias1"><xsl:for-each select="/FunctionalModel/ReferenceInfo[ReferenceType='External']"><xsl:if test="@ID=$ReferenceID1"><xsl:value-of select="Alias"/></xsl:if></xsl:for-each></xsl:variable>
<!-- TODO need to deal with other references, other reference function references will need to go in correct column -->
	<xsl:variable name="ReferenceAlias2"><xsl:for-each select="/FunctionalModel/ReferenceInfo[ReferenceType='External']"><xsl:if test="@ID=$ReferenceID2"><xsl:value-of select="Alias"/></xsl:if></xsl:for-each></xsl:variable>
	<xsl:variable name="ReferenceAlias3"><xsl:for-each select="/FunctionalModel/ReferenceInfo[ReferenceType='External']"><xsl:if test="@ID=$ReferenceID3"><xsl:value-of select="Alias"/></xsl:if></xsl:for-each></xsl:variable>
	
	<xsl:variable name="ReferenceURI1"><xsl:for-each select="/FunctionalModel/ReferenceInfo[ReferenceType='External']"><xsl:if test="@ID=$ReferenceID1"><xsl:value-of select="URI"/></xsl:if></xsl:for-each></xsl:variable>
	<xsl:variable name="ReferenceURI2"><xsl:for-each select="/FunctionalModel/ReferenceInfo[ReferenceType='External']"><xsl:if test="@ID=$ReferenceID2"><xsl:value-of select="URI"/></xsl:if></xsl:for-each></xsl:variable>
	<xsl:variable name="ReferenceURI3"><xsl:for-each select="/FunctionalModel/ReferenceInfo[ReferenceType='External']"><xsl:if test="@ID=$ReferenceID3"><xsl:value-of select="URI"/></xsl:if></xsl:for-each></xsl:variable>
	
	<xsl:variable name="ReferenceURILower1"><xsl:value-of select="translate($ReferenceURI1, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ ', 'abcdefghijklmnopqrstuvwxyz')"/></xsl:variable>
	<xsl:variable name="ReferenceURILower2"><xsl:value-of select="translate($ReferenceURI2, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ ', 'abcdefghijklmnopqrstuvwxyz')"/></xsl:variable>
	<xsl:variable name="ReferenceURILower3"><xsl:value-of select="translate($ReferenceURI3, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ ', 'abcdefghijklmnopqrstuvwxyz')"/></xsl:variable>

	
	
	<!-- load  reference documents -->
	<xsl:variable name="ReferenceDocument1" select="document($ReferenceURI1, /)"/>
	<xsl:variable name="ReferenceDocument2" select="document($ReferenceURI2, /)"/>
	<xsl:variable name="ReferenceDocument3" select="document($ReferenceURI3, /)"/>

	<xsl:key name="ReferenceFunctionIDKey" match="Function" use="ID"/>
	<xsl:key name="CriteriaFunctionIDKey" match="Function" use="ID"/>


	<!-- change code for R 1.1 -->

	<xsl:template match="/FunctionalModel">
		<html xmlns="http://www.w3.org/1999/xhtml">
			<head>
				<title>
					<xsl:value-of select="Name"/>, <xsl:value-of select="Version"/>
				</title>
				<style type="text/css" media="screen">
					.Header { background: #CCCCFF; }
					<xsl:for-each select="Chapter">
						.<xsl:value-of select="Alias"/>_Header { background: <xsl:value-of select="Style/Header/Title/BackColor"/>; }
						.<xsl:value-of select="Alias"/>_Function { background: <xsl:value-of select="Style/Function/Title/BackColor"/>; }
						.<xsl:value-of select="Alias"/>_Header_Body { background: <xsl:value-of select="Style/Header/Body/BackColor"/>; }
						.<xsl:value-of select="Alias"/>_Function_Body { background: <xsl:value-of select="Style/Function/Body/BackColor"/>; }
					</xsl:for-each>
					<!--.DC_Header { background: #FFFF00; }
					.DC_Function  { background: #FFFF99; }
					.S_Header { background: #FFCC00; }
					.S_Function { background: #FFCC99; }
					.IN_Header  { background: #00CCFF; }
					.IN_Function  { background: #99CCFF; }

					.DC_Header_Body  { background: #FFFFBB; }
					.DC_Function_Body  { background: #FFFFCC; }
					.S_Header_Body  { background: #FFCCBB; }
					.S_Function_Body  { background: #FFCCBB; }
					.IN_Header_Body  { background: #BBCCFF; }
					.IN_Function_Body  { background: #CCCCFF; }-->
					

					
#popitmenu{
position: absolute;
background-color: white;
border:1px solid black;
font: normal 12px Verdana;
line-height: 18px;
z-index: 100;
visibility: hidden;
background-color: <xsl:value-of select="$PrimaryAlternatingTableColor"/>;

}

#popitmenu a{
text-decoration: none;
padding-left: 6px;
color: black;
display: block;
}

#popitmenu a:hover{ /*hover background color*/
background-color: <xsl:value-of select="$TopTableHeaderColor"/>;
}


					
					#lastThingInTheBody {
					position:fixed;
					bottom:0;
					left:0;
					width:100%;
					height:auto;
					}

					a {text-decoration: none;}
					.TOC
					{
					margin-bottom: 0cm;
					margin-top: 0cm;
					border-color: #666666;
					border-width: 1px;
					}
					td.TOC
					{
					border-color: #FFFFFF;
					border-top-width: 1px;
					border-bottom-width: 0px;
					border-left-width: 0px;
					border-right-width: 0px;
					}
					a:link
					{
					color: #000099;
					}

					.noborder
					{
					border-color: #FFFFFF;
					border-width: 0px;
					}

					.visible { visibility:"visible" }
					.hidden { visibility:"hidden" }

					.nobr {
					white-space: nowrap;
					}
					
					td.bordered { 
						border-top-width: 1px;
						border-top-style: solid;

						border-bottom: thin solid black;
						border-left: thin solid black;
						border-right: thin solid black;
					}
					
					td.test { 
						border-top-width: 0px;
						border-bottom-width: 0px;
						border-left-width: 0px;
						border-right-width: 0px;
					}
					
					.box_rotates { 
						display:block; /* for firefox, safari, chrome, etc. */ 
						-webkit-transform: rotate(-90deg); 
						-moz-transform: rotate(-90deg); 
						-o-transform: rotate(-90deg); /* For Opera*/ 
						-khtml-transform: rotate(-90deg); /* For Lunix*/ 
						filter: progid:DXImageTransform.Microsoft.BasicImage(rotation=3); /* for ie */
						-ms-filter: progid:DXImageTransform.Microsoft.BasicImage(rotation=3); /* for ie */
						<!--height: 200px;
						width: 100px;
						text-align: left;-->
						
					}
					<!--{

						-o-transform: rotate(-90deg);  /* Opera 10.5 */
						-webkit-transform-origin: top left;
						-webkit-transform: rotate(-90deg);
						-moz-transform-origin: top left;
						-moz-transform: rotate(-90deg);
						 -filter:  progid:DXImageTransform.Microsoft.BasicImage(rotation=3);  /* IE6,IE7 */
						-ms-filter: "progid:DXImageTransform.Microsoft.BasicImage(rotation=3)"; /* IE8 */-
						writing-mode: bt-rl;
						padding: 0;
						margin: 0;
						height: 125px;
						text-align: left;
						vertical-align: bottom;
					}-->
					.box_rotate  {
						-webkit-transform: rotate(-90deg) translate(0, 10px);
						-moz-transform: rotate(-90deg)  translate(0, 10px);
						filter: progid:DXImageTransform.Microsoft.BasicImage(rotation=3); /* for ie */
						
						padding: 0;
						margin: 0;
						width: 50px;
						height: 200px;
						text-align: left;
						vertical-align: bottom;
						
					}

					

				</style>
				<script type="text/javascript">
				<xsl:comment>
				
					<![CDATA[
					
						/***********************************************
						* Pop-it menu- © Dynamic Drive (www.dynamicdrive.com)
						* This notice MUST stay intact for legal use
						* Visit http://www.dynamicdrive.com/ for full source code
						***********************************************/

						var defaultMenuWidth="200px" //set default menu width.

						var linkset=new Array()
						//SPECIFY MENU SETS AND THEIR LINKS. FOLLOW SYNTAX LAID OUT

						]]>

						<xsl:for-each select="Chapter">	
							linkset[<xsl:value-of select="ID"/>]='&lt;table width="100%" style="background: <xsl:value-of select="Style/Header/Title/BackColor"/>; border-bottom: thin solid <xsl:value-of select="Style/Header/Body/BackColor"/>;"&gt;&lt;tr&gt;&lt;td&gt;&lt;b&gt;&lt;a href="#<xsl:value-of select="Alias"/>"&gt;<xsl:value-of select="Alias"/><xsl:text> - </xsl:text><xsl:value-of select="Name"/>&#160;&lt;/a&gt;&lt;/b&gt;&lt;/td&gt;&lt;/tr&gt;&lt;/table&gt;'
							<xsl:for-each select="Function[ParentID=../Alias]">
								linkset[<xsl:value-of select="../ID"/>]+='&lt;table width="100%" style="background: <xsl:value-of select="../Style/Function/Title/BackColor"/>; border-bottom: thin solid <xsl:value-of select="../Style/Function/Body/BackColor"/>;"&gt;&lt;tr&gt;&lt;td&gt;&lt;a href="#Function_<xsl:value-of select="ID"/>"&gt;<xsl:value-of select="ID"/><xsl:text> - </xsl:text><xsl:value-of select="Name"/>&#160;&lt;/a&gt;&lt;/td&gt;&lt;/tr&gt;&lt;/table&gt;'
							</xsl:for-each>
						</xsl:for-each>
						<![CDATA[

						////No need to edit beyond here

						var ie5=document.all && !window.opera
						var ns6=document.getElementById

						if (ie5||ns6)
							document.write('<div id="popitmenu" onMouseover="clearhidemenu();" onMouseout="dynamichide(event)"></div>')

						function iecompattest()
						{
							return (document.compatMode && document.compatMode.indexOf("CSS")!=-1)? document.documentElement : document.body
						}

						function showmenu(e, which, optWidth)
						{
							if (!document.all&&!document.getElementById)
								return
							clearhidemenu()
							menuobj=ie5? document.all.popitmenu : document.getElementById("popitmenu")
							menuobj.innerHTML=which
							//menuobj.style.width=(typeof optWidth!="undefined")? optWidth : defaultMenuWidth
							menuobj.contentwidth=menuobj.offsetWidth
							menuobj.contentheight=menuobj.offsetHeight
							eventX=ie5? event.clientX : e.clientX
							eventY=ie5? event.clientY : e.clientY
							
							//Find out how close the mouse is to the corner of the window
							var rightedge=ie5? iecompattest().clientWidth-eventX : window.innerWidth-eventX
							var bottomedge=ie5? iecompattest().clientHeight-eventY : window.innerHeight-eventY
							//if the horizontal distance isn't enough to accomodate the width of the context menu
							if (rightedge<menuobj.contentwidth)
								//move the horizontal position of the menu to the left by it's width
								menuobj.style.left=ie5? iecompattest().scrollLeft+eventX-menuobj.contentwidth+"px" : window.pageXOffset+eventX-menuobj.contentwidth+"px"
							else
								//position the horizontal position of the menu where the mouse was clicked
								menuobj.style.left=ie5? iecompattest().scrollLeft+eventX+"px" : window.pageXOffset+eventX+"px"
								//same concept with the vertical position
							
							if (bottomedge<menuobj.contentheight)
								menuobj.style.top=ie5? iecompattest().scrollTop+eventY-menuobj.contentheight+"px" : window.pageYOffset+eventY-menuobj.contentheight+"px"
							else
								menuobj.style.top=ie5? iecompattest().scrollTop+event.clientY+"px" : window.pageYOffset+eventY+"px"
							
							menuobj.style.visibility="visible"
							
							return false
						}

						function contains_ns6(a, b) 
						{
							//Determines if 1 element in contained in another- by Brainjar.com
							while (b.parentNode)
								if ((b = b.parentNode) == a)
									return true;
							return false;
						}

						function hidemenu()
						{
							if (window.menuobj)
								menuobj.style.visibility="hidden"
						}

						function dynamichide(e)
						{
							if (ie5&&!menuobj.contains(e.toElement))
								hidemenu()
							else if (ns6&&e.currentTarget!= e.relatedTarget&& !contains_ns6(e.currentTarget, e.relatedTarget))
								hidemenu()
						}

						function delayhidemenu()
						{
							delayhide=setTimeout("hidemenu()",500)
						}

						function clearhidemenu()
						{
							if (window.delayhide)
							clearTimeout(delayhide)
						}

						if (ie5||ns6)
						document.onclick=hidemenu
						]]>
					</xsl:comment>
				</script>
			</head>
			<body>
				<center>
					<!--
			<a href="javascript:hidediv()">hide div</a>
			 
			<a href="javascript:showdiv()">show div</a>
			<div id="HideShow">Test</div>-->
					<p/>
					<!--
			<div id="div1_showLink" style="display: block;">
			<a href="#" onclick="setDivVisibility('div1', 'show');">Show me</a>
			</div>
			 
			<div id="div1_hideLink" style="display: none;">
			<a href="#" onclick="setDivVisibility('div1', 'hide');">Hide me</a>
			</div>
			 
			<div id="div1" style="display: none;">This is the content</div>
-->
					<table border="0" width="{$TableWidth}" class="visi2">
						<tr>
							<td align="right">
								<h2>HL7 EHR Work Group</h2>
							</td>
						</tr>
						<tr>
							<td align="right"/>
						</tr>
						<tr>
							<td align="right">
								<h1>
									<xsl:value-of select="Name"/>, <xsl:value-of select="Version"/>
								</h1>
							</td>
						</tr>
						<tr>
							<td align="right">
								<h3>
									<xsl:apply-templates select="Date" mode="DateDisplay"/>
								</h3>
							</td>
						</tr>
						<tr>
							<td height="20"/>
						</tr>
						<xsl:if test="count(Credits/Entry)&gt;0">
							<xsl:for-each select="Credits/Entry">
								<tr height="10">
									<td/>
								</tr>
								<tr>
									<td align="right">
										<font size="4">
											<b>
												<xsl:value-of select="Name/Given"/>
												<xsl:text> </xsl:text>
												<xsl:value-of select="Name/Family"/>
												<xsl:if test="string-length(Name/Suffix)&gt;0">, <xsl:value-of select="Name/Suffix"/>
												</xsl:if>
											</b>
										</font>
										<xsl:for-each select="Organization">
											<br/>
											<font size="2">
												<xsl:if test="string-length(Role)&gt;0">
													<xsl:value-of select="Role"/>, </xsl:if>
												<xsl:value-of select="Name"/>
											</font>
										</xsl:for-each>
									</td>
								</tr>
							</xsl:for-each>
						</xsl:if>
						<tr>
							<td height="20"/>
						</tr>
						<tr>
							<td height="20">
								<hr width="95%"/>
							</td>
						</tr>
						<xsl:for-each select="Copyright">
							<tr>
								<td>
									<font size="1" color="{$FooterColor}">
										<xsl:value-of select="."/>
									</font>
								</td>
							</tr>
						</xsl:for-each>
						<tr>
							<td height="6"/>
						</tr>
						<xsl:for-each select="Trademark">
							<tr>
								<td>
									<font size="1" color="{$FooterColor}">
										<xsl:value-of select="."/>
									</font>
								</td>
							</tr>
						</xsl:for-each>
						<tr>
							<td height="20">
								<hr width="95%"/>
							</td>
						</tr>
						<tr>
							<td height="20"/>
						</tr>
					</table>
					<xsl:apply-templates select="." mode="TopTOC"/>
					<p/>
					<br/>
					<hr width="80%"/>
					<br/>
					<xsl:apply-templates select="." mode="ReferenceTable"><xsl:with-param name="ReferenceType" select="'Internal'"/></xsl:apply-templates>
					<p/>
					<xsl:apply-templates select="." mode="ReferenceTable"><xsl:with-param name="ReferenceType" select="'External'"/></xsl:apply-templates>
					<p/>
					<br/>
					<hr width="80%"/>
					<br/>
					<table width="800">
						<tr>
							<td align="left"><xsl:copy-of select="Overview"/></td>
						</tr>
					</table>
					<p/>
					<br/>
					<hr width="80%"/>
					<br/>
					<xsl:apply-templates select="Chapter"><xsl:with-param name="ChapterCount"><xsl:value-of select="count(Chapter)"/></xsl:with-param></xsl:apply-templates>
					<xsl:if test="$includeGlossary='yes'">
						<p/>
						<br/>
						<hr width="80%"/>
						<br/>
						<table width="1000" border="0" cellspacing="0" cellpadding="0">
							<tr>
								<td align="left">
									<a name="Glossary"><h2>Glossary</h2></a>
									<p/>
								</td>
							</tr>
							<tr>
								<td>
									<table width="1000" cellpadding="3" cellspacing="0" style="border: thin solid black;">
										<thead>
											<tr bgcolor="{$TopTableHeaderColor}">
												<td width="25" align="left"><b><font size="3">Status</font></b></td>
												<td width="150" align="left"><b><font size="3">Term</font></b></td>
												<td width="400" align="left"><b><font size="3">Definition</font></b></td>
												<td width="175" align="left"><b><font size="3">Reference</font></b></td>
												<td width="250" align="left"><b><font size="3">URI</font></b></td>
											</tr>
										</thead>
										<xsl:for-each select="Glossary/Entry[Status='Active']">
											<xsl:variable name="BackgroundColor"><xsl:choose><xsl:when test="position() mod 2 = 0"><xsl:value-of select="$PrimaryAlternatingTableColor"/></xsl:when><xsl:otherwise><xsl:value-of select="$SecondaryAlternatingTableColor"/></xsl:otherwise></xsl:choose></xsl:variable>
											<tr bgcolor="{$BackgroundColor}">
												<td><i><xsl:value-of select="substring(Status, 1, 1)"/></i></td>
												<td align="left"><a name="#{translate(Term, ' ', '_')}"><xsl:value-of select="Term"/></a></td>
												<td align="left"><xsl:copy-of select="Definition"/> </td>
												<td align="left"><div style="word-break:break-all"><xsl:value-of select="Reference/Description"/></div> </td>
												<td align="left"><xsl:if test="string-length(Reference/URI)&gt;1"><div style="word-break:break-all"><a href="{Reference/URI}"><xsl:value-of select="Reference/URI"/></a></div></xsl:if> </td>
											</tr>
										</xsl:for-each>
										<xsl:for-each select="Glossary/Entry[Status='Deprecated']">
											<xsl:variable name="BackgroundColor"><xsl:choose><xsl:when test="position() mod 2 = 0"><xsl:value-of select="$PrimaryAlternatingTableColor"/></xsl:when><xsl:otherwise><xsl:value-of select="$SecondaryAlternatingTableColor"/></xsl:otherwise></xsl:choose></xsl:variable>
											<tr bgcolor="{$BackgroundColor}">
												<td><i><xsl:value-of select="substring(Status, 1, 1)"/></i></td>
												<td align="left"><xsl:value-of select="Term"/></td>
												<td align="left"><xsl:copy-of select="Definition"/> </td>
												<td align="left"><div style="word-break:break-all"><xsl:value-of select="Reference/Description"/></div> </td>
												<td align="left"><xsl:if test="string-length(Reference/URI)&gt;1"><div style="word-break:break-all"><a href="{Reference/URI}"><xsl:value-of select="Reference/URI"/></a></div></xsl:if> </td>
											</tr>
										</xsl:for-each>
									</table>
								</td>
							</tr>
						</table>
					</xsl:if>					
					<p /><br />
				</center>

				<!-- extra statistics for QA only, not for publication -->
				<!--
        <div id="statisticsDiv" class="hidden">
          <xsl:apply-templates select="." mode="Statistics"/>
        </div>
-->
				<div id="lastThingInTheBody">
					<center>
						<table width="100%" style="border: 1px solid #9999ff; border-bottom: 0px solid #9999ff; border-collapse:collapse; background: {$TopTableHeaderColor}">
							<tr>
								<td>
									<center>
										<table border="0">
											<tr>
												<xsl:for-each select="Chapter">
													<td align="center" width="100" style="background: {Style/Function/Title/BackColor}"><a href="#{Alias}" onMouseover="showmenu(event,linkset[{ID}])" onMouseout="delayhidemenu()"><font size="2"><xsl:value-of select="Alias"/></font></a></td>
												</xsl:for-each>
												<xsl:if test="$includeGlossary='yes'"><td align="center" width="100" style="background: #cccccc"><a href="#Glossary" ><font size="2">Glossary</font></a></td></xsl:if>
											</tr>
										</table>
									</center>
								</td>
							</tr>
						</table>
					</center>
				</div>

			</body>
		</html>
	</xsl:template>
	
	<!-- <xsl:template match="FIELD[@name='body']/key">
        <strong><xsl:apply-templates select="@*|node()"/></strong>
    </xsl:template>

    - Template to match all nodes, copy them and then apply templates to children. 
    <xsl:template match="@*|node()">
      <xsl:copy>
        <xsl:apply-templates select="@*|node()"/>
      </xsl:copy>
    </xsl:template>-->

	<xsl:template match="*" mode="TopTOC">
		<table width="800" border="0" cellspacing="0" cellpadding="0">
			<tr>
				<td colspan="2" align="left">
					<h2>Table of Contents</h2>
					<p/>
				</td>
			</tr>
			<tr>
				<td>
					<table width="800" border="0" cellspacing="0" cellpadding="0" style="border: thin solid black;">
						<tr>
							<td colspan="2" align="left">
								<xsl:apply-templates select="Chapter" mode="TOC"><xsl:with-param name="ChapterCount" select="count(Chapter)"/></xsl:apply-templates>
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</xsl:template>

	<xsl:template match="Chapter" mode="TOC">
		<xsl:param name="ChapterCount">1</xsl:param>
		<xsl:variable name="ChapterPosition"><xsl:value-of select="position()"/></xsl:variable>
		<tr class="{Alias}_Header" height="40" valign="bottom" style="border: thin solid black; border-bottom: thin solid {Style/Header/Body/BackColor}; border-collapse:collapse;">
			<td colspan="2" align="left" style="padding-left : 0px; padding-bottom : 5px">
				<font size="4">
					<a href="#{Alias}" style="text-decoration:none;">
						<b>&#160;&#160;<xsl:value-of select="Name"/></b>
					</a>
				</font>
			</td>
		</tr>
		<xsl:for-each select="Function[ParentID=../Alias]">
			<tr>
			<xsl:attribute name="class" style="border-left: thin solid black;">
				<xsl:value-of select="../Alias"/>_Function<!--<xsl:choose>
					<xsl:when test="Type='H'">Function</xsl:when>
					<xsl:when test="Type='F'">Function_Body</xsl:when>
				</xsl:choose>-->
			</xsl:attribute>
				<xsl:variable name="BottomBorder">padding-left : 20px; <xsl:choose><xsl:when test="position()&lt;count(../Function[ParentID=../Alias])">border-bottom: thin solid <xsl:value-of select="../Style/Function/Body/BackColor"/>;</xsl:when><xsl:when test="$ChapterPosition&lt;$ChapterCount">border-bottom: 2px solid black;border-collapse:collapse;</xsl:when></xsl:choose> padding-bottom: 5px; padding-top: 4px;</xsl:variable>
				<td width="80" style="{$BottomBorder}" align="left">
					<font size="4">
						<a href="#Function_{ID}" style="text-decoration:none;">
							<xsl:value-of select="ID"/>
						</a>
					</font>
				</td>
				<td align="left" width="720" style="{$BottomBorder}">
					<font size="4">
						<a href="#Function_{ID}" style="text-decoration:none;">
							<xsl:value-of select="Name"/>
						</a>
					</font>
				</td>
			</tr>
		</xsl:for-each>
	
	<!--  attempt to rotate, may try again later
	<table width="50" border="1" cellspacing="0" height="200"><tr><td width="50" valign="bottom"><div class="box_rotate">Hello and how are you?</div></td></tr></table>
		
		<table width="800" border="1" cellspacing="0">
			<tr valign="bottom" height="200">
				
				<td width="50" align="center" valign="bottom">
				<xsl:attribute name="class">
						<xsl:value-of select="Alias"/>_Header
					</xsl:attribute>
				<div class="box_rotate">
					<font size="5">
						<nobr>&#160;&#160;<xsl:value-of select="Name"/></nobr>
					</font>
				</div>
					-<xsl:attribute name="class">
						<xsl:value-of select="Alias"/>_Header
					</xsl:attribute>
					<div style="writing-mode: tb-rl; filter: flipv() fliph(); text-align: left">
						<font size="5">
							&#160;&#160;<xsl:value-of select="Name"/>
						</font>
					</div>-
				</td>
				<td width="750" valign="top">
					<table width="750" cellspacing="0">
						<xsl:for-each select="Function[ParentID=../Alias]">
							<tr>
								<td width="100">
									<font size="4">
										<span class="nobr">
											<a>
												<xsl:attribute name="href">#Function_<xsl:value-of select="ID"/>
												</xsl:attribute>
												<xsl:attribute name="style">text-decoration:none;</xsl:attribute>
												<xsl:value-of select="ID"/>
											</a>
										</span>
									</font>
								</td>
								<td align="left" width="650">
									<font size="4">
										<a>
											<xsl:attribute name="href">#Function_<xsl:value-of select="ID"/>
											</xsl:attribute>
											<xsl:attribute name="style">text-decoration:none;</xsl:attribute>
											<xsl:value-of select="Name"/>
										</a>
									</font>
								</td>
							</tr>
						</xsl:for-each>
					</table>
				</td>
			</tr>
		</table>-->
			<!--<xsl:for-each select="Function">
				<xsl:if test="string-length(ID)-string-length(translate(ID,'.',''))=1">
					<tr>
						<xsl:attribute name="class">
							<xsl:value-of select="../Alias"/>_Function
						</xsl:attribute>
						<xsl:if test="position()=1">
							<td width="40" align="center" valign="bottom">
								<xsl:attribute name="class">
									<xsl:value-of select="../Alias"/>_Header
								</xsl:attribute>
								<xsl:attribute name="rowspan">
									<xsl:value-of select="count(../Function)"/>
								</xsl:attribute>
								<div style="writing-mode: bt-rl; text-align: center">
									<font size="5">
										<span class="nobr">&#160;&#160;&#160;<xsl:value-of select="../Name"/>&#160;&#160;&#160;</span>
									</font>
								</div>
							</td>
						</xsl:if>
						<td valign="middle">
							<table border="1">
								<tr>
									<td width="20"/>
									<td width="100">
										<font size="4">
											<span class="nobr">
												<a>
													<xsl:attribute name="href">#Function_<xsl:value-of select="ID"/>
													</xsl:attribute>
													<xsl:attribute name="style">text-decoration:none;</xsl:attribute>
													<xsl:value-of select="ID"/>
												</a>
											</span>
										</font>
									</td>
									<td align="left" width="300">
										<font size="4">
											<span class="nobr">
												<div style="word-break:break-all;"><a>
													<xsl:attribute name="href">#Function_<xsl:value-of select="ID"/>
													</xsl:attribute>
													<xsl:attribute name="style">text-decoration:none;</xsl:attribute>
													<xsl:value-of select="Name"/>
												</a>
												</div>
											</span>
										</font>
									</td>
								</tr>
							</table>
						</td>
					</tr>
				</xsl:if>
			</xsl:for-each>
		</table>-->
	</xsl:template>

	<xsl:template match="Function" mode="TOC">
		<xsl:param name="ChapterAlias"/>
		<xsl:param name="depth">0</xsl:param>
		<!-- only display the base functions -->

		<xsl:if test="string-length(ID)-string-length(translate(ID,'.',''))=1">
			<tr class="TOC">
				<xsl:attribute name="class">
					<xsl:value-of select="$ChapterAlias"/>_<xsl:choose>
						<xsl:when test="Type='H'">Header</xsl:when>
						<xsl:when test="Type='F'">Function</xsl:when>
					</xsl:choose>
				</xsl:attribute>
				<!--<div id="{translate(Priority, ' ', '_')}" style="display: none;">-->

				<td class="TOC">
					<span class="nobr">
						<xsl:call-template name="indent">
							<xsl:with-param name="depth" select="$depth"/>
						</xsl:call-template>
						<a>
							<xsl:attribute name="href">#Function_<xsl:value-of select="ID"/>
							</xsl:attribute>
							<xsl:value-of select="ID"/>&#160;&#160;&#160;
						</a>
					</span>

				</td>
				<td width="100%" class="TOC">
					<a>
						<xsl:attribute name="href">#Function_<xsl:value-of select="ID"/>
						</xsl:attribute>
						<xsl:value-of select="Name"/>
					</a>
				</td>

			</tr>

			<!--
		This was for recursive schemas
		<xsl:apply-templates select="Function" mode="TOC">
			<xsl:with-param name="depth" select="$depth + 1"/>
      <xsl:with-param name="ChapterAlias" select="$ChapterAlias"/>
		</xsl:apply-templates>
-->		
		</xsl:if>
	</xsl:template>

	<xsl:template match="*" mode="ReferenceTable">
		<xsl:param name="ReferenceType">Internal</xsl:param>
		<!--<xsl:if test="count(ReferenceInfo[ReferenceType=$ReferenceType])&gt;0">-->
		<xsl:variable name="ReferenceCount"><xsl:for-each select="ReferenceInfo"><xsl:if test="ReferenceType=$ReferenceType">1</xsl:if></xsl:for-each></xsl:variable>
		<xsl:if test="string-length($ReferenceCount)&gt;0">
			<table width="800" border="0">
				<tr>
					<td align="left">
						<xsl:choose>
							<xsl:when test="$ReferenceType='Internal'">
								The <xsl:value-of select="Name"/> Package includes the following materials:
							</xsl:when>
							<xsl:when test="$ReferenceType='External'">
								<a name="#ReferenceTable_External">The <xsl:value-of select="Name"/> includes references to the following external references:</a>
							</xsl:when>
						</xsl:choose>
					</td>
				</tr>
				<tr height="20">
					<td> </td>
				</tr>
				<tr>
					<td>
						<table width="800" cellspacing="0" cellpadding="3" style="border: thin solid gray;">
							<thead>
								<tr bgcolor="{$TopTableHeaderColor}">
									<xsl:if test="$ReferenceType='External'">
										<td width="50" align="center"><b><font size="3">ID</font></b></td>
									</xsl:if>
									<td width="300" align="left"><b><font size="3">Document Title</font></b></td>
									<td align="left"><b><font size="3">File Name</font></b></td>
								</tr>
							</thead>
							
							<tbody>
								<xsl:for-each select="ReferenceInfo">
									<xsl:if test="ReferenceType=$ReferenceType">
										<xsl:variable name="BackgroundColor"><xsl:choose><xsl:when test="position() mod 2 = 0"><xsl:value-of select="$PrimaryAlternatingTableColor"/></xsl:when><xsl:otherwise><xsl:value-of select="$SecondaryAlternatingTableColor"/></xsl:otherwise></xsl:choose></xsl:variable>
										<tr bgcolor="{$BackgroundColor}">
											<xsl:if test="$ReferenceType='External'">
												<td align="left"><font size="2"><xsl:value-of select="Caption"/></font></td>
											</xsl:if>
											<td align="left"><font size="2"><xsl:value-of select="Name"/></font></td>
											<td align="left">
												<xsl:variable name="URILower"><xsl:value-of select="translate(URI, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ ', 'abcdefghijklmnopqrstuvwxyz')"/></xsl:variable>
												<xsl:choose>
													<xsl:when test="contains(substring($URILower, string-length($URILower)-5, 6), '.pdf') or contains(substring($URILower, string-length($URILower)-5, 6), '.doc') or contains(substring($URILower, string-length($URILower)-5, 6), '.xml') or contains(substring($URILower, 1, 7), 'http://')"><font size="2"><a href="{URI}"><div style="word-break:break-all"><xsl:value-of select="URI"/></div></a></font></xsl:when>
													<xsl:otherwise><font size="2"><xsl:apply-templates select="URI" mode="FormattedText"/></font></xsl:otherwise>
												</xsl:choose>
											</td>
										</tr>
									</xsl:if>
								</xsl:for-each>
							</tbody>-
						</table>
					</td>
				</tr>	
			</table>
		</xsl:if>
	</xsl:template>

	
	<xsl:template match="Chapter">
		<xsl:param name="ChapterCount"></xsl:param>
		<table width="800" border="0">
			<tr>
				<td align="left">
					<font size="5"><b><a name="{Alias}"><xsl:value-of select="$ChapterNumber"/>.<xsl:value-of select="ID"/><xsl:text>  </xsl:text><xsl:value-of select="Name"/> Section</a></b></font><p/>
				</td>
			</tr>
			<tr>
				<td align="left">
					<font size="4"><i><xsl:value-of select="$ChapterNumber"/>.<xsl:value-of select="ID"/>.1<xsl:text>  </xsl:text>Section Introduction</i></font><br/>
				</td>
			</tr>
			<tr>
				<td height="2"/>
			</tr>
			<tr>
				<td align="left">
				
					<xsl:copy-of select="Overview" />
				</td>
			</tr>
			<tr>
				<td height="20"/>
			</tr>
			<tr>
				<td>
					<center>
						<table width="800" border="0" cellspacing="0" style="border: thin solid black;">
							<xsl:apply-templates select="." mode="TOC"><xsl:with-param name="ChapterCount" select="'1'"/></xsl:apply-templates>
						</table>
					</center>
				</td>
			</tr>
			<tr>
				<td height="25"/>
			</tr>
		</table>
		<hr width="1000"/>
		<table>
			<tr>
				<td height="25"/>
			</tr>
			<tr>
				<td>
					<table border="1" width="{$TableWidth}" cellspacing="0" cellpadding="5" style="border: 1px solid black;border-collapse:collapse;">
						<thead style="display: table-header-group;">
							<!-- FUNCTION LIST -->
							<tr class="Header" style="border: 1px solid black;border-collapse:collapse;">
								<th align="left" width="{$ColWidth1}" style="border: 1px solid black;border-collapse:collapse;">
									<b>Section/ID#:</b><br/>
									<b>Type:</b><br/>
									<b>Name:</b>
								</th>
								<th align="center" valign="middle" width="{$ColWidth2}" style="border: 1px solid black;border-collapse:collapse;">
									<b>Conformance Criteria</b>
								</th>
								<xsl:if test="$IncludeSeeAlso!='no'">
									<th align="center" valign="middle" width="{$ColWidth3}" style="border: 1px solid black;border-collapse:collapse;">
										<b>See Also</b>
									</th>
								</xsl:if>
								<!-- optional Reference Columns -->
								<xsl:choose>
									<xsl:when test="$ShowReferencesInOneColumn='yes'">
										<xsl:if test="string-length($ReferenceAlias1)&gt;0">
											<th align="center" valign="middle" width="{$ColWidthReference}" style="border: 1px solid black;border-collapse:collapse;">
												<div> <!--style="writing-mode: tb-rl; filter: flipv() fliph(); text-align: center"-->
													<a href="#ReferenceTable_External"><b>Reference</b></a>
												</div>
											</th>
										</xsl:if>
									</xsl:when>
									<xsl:otherwise>
										<xsl:if test="string-length($ReferenceAlias1)&gt;0">
											<th align="center" valign="middle" width="{$ColWidthReference}" style="border: 1px solid black;border-collapse:collapse;">
												<div> <!--style="writing-mode: tb-rl; filter: flipv() fliph(); text-align: center"-->
													<b>
														<xsl:value-of select="$ReferenceCaption1"/> Ref.</b>
												</div>
											</th>
										</xsl:if>
										<xsl:if test="string-length($ReferenceAlias2)&gt;0">
											<th align="center" valign="middle" width="{$ColWidthReference}" style="border: 1px solid black;border-collapse:collapse;">
												<div> <!--style="writing-mode: tb-rl; filter: flipv() fliph(); text-align: center"-->
													<b>
														<xsl:value-of select="$ReferenceCaption2"/> Ref.</b>
												</div>
											</th>
										</xsl:if>
										<xsl:if test="string-length($ReferenceAlias3)&gt;0">
											<th align="center" valign="middle" width="{$ColWidthReference}" style="border: 1px solid black;border-collapse:collapse;">
												<div> <!--style="writing-mode: tb-rl; filter: flipv() fliph(); text-align: center"-->
													<b>
														<xsl:value-of select="$ReferenceCaption3"/> Ref.</b>
												</div>
											</th>
										</xsl:if>
									</xsl:otherwise>
								</xsl:choose>
								<xsl:if test="$IncludeChgIndic!='no'">
									<th align="center" valign="middle" width="{$ColWidth8}" style="border: 1px solid black;border-collapse:collapse;">
										<div> <!--style="writing-mode: tb-rl; filter: flipv() fliph(); text-align: center"-->
											<b>Chg Indic</b>
										</div>
									</th>
								</xsl:if>
								<th align="center" valign="middle" width="{$ColWidth7}" style="border: 1px solid black;border-collapse:collapse;">
									<div> <!--style="writing-mode: tb-rl; filter: flipv() fliph(); text-align: center"-->
										<b><xsl:value-of select="../Caption"/> Row#</b>
									</div>
								</th>
							</tr>
						</thead>
						<!--
						<tr>
							<xsl:attribute name="class"><xsl:value-of select="Alias"/>_Header</xsl:attribute>
							<th><span class="nobr">ID#</span></th>
							<th><span class="nobr"><div style="writing-mode: tb-rl; filter: flipv() fliph(); text-align: center">Type</div></span></th>
							<th><span class="nobr">Name</span></th>
							<th><span class="nobr">Statement/Description</span></th>
							<th>See Also</th>
							<th><span class="nobr">Conformance Criteria</span></th>
						</tr>
						-->
						<tbody>
							<xsl:apply-templates select="Function"/>
						</tbody>
					</table>
				</td>
			</tr>
			<tr>
				<td height="40"/>
			</tr>
			<xsl:if test="position()&lt;$ChapterCount">
				<tr>
					<td height="40"><hr width="100%" size="3"/></td>
				</tr>
				<tr>
					<td height="40"/>
				</tr>
			</xsl:if>
		</table>
	</xsl:template>

	<xsl:template match="Function">
		<xsl:param name="depth">0</xsl:param>
		<xsl:variable name="ChapterAlias">
			<xsl:value-of select="substring-before(ID, '.')" />
		</xsl:variable>
		<xsl:variable name="Type">
			<xsl:choose>
				<xsl:when test="Type='H'">Header</xsl:when>
				<xsl:when test="Type='F'">Function</xsl:when>
				<xsl:otherwise>ERROR</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="CriteriaCount">
			<xsl:value-of select="count(Criteria)"/>
		</xsl:variable>
		<xsl:variable name="RowSpan">
			<xsl:choose>
				<xsl:when test="$CriteriaCount&gt;0">
					<xsl:value-of select="count(Criteria)"/>
				</xsl:when>
				<xsl:otherwise>1</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<tr class="{$ChapterAlias}_{$Type}">
			<td width="{$ColWidth1}" align="left" style="border: 1px solid black;border-collapse:collapse;">
				<a>
					<xsl:attribute name="name">Function_<xsl:value-of select="ID"/>
					</xsl:attribute>
					<b>
						<xsl:value-of select="ID"/>
					</b>
				</a>
			</td>
			<td rowspan="3" style="border: 1px solid black;border-collapse:collapse;">&#160;</td>
			<xsl:if test="$IncludeSeeAlso!='no'">
				<td rowspan="3" align="center" style="border: 1px solid black;border-collapse:collapse;">
					<xsl:choose>
						<xsl:when test="count(SeeAlso)&gt;0">
							<xsl:apply-templates select="SeeAlso"/>
						</xsl:when>
						<xsl:otherwise>&#160;</xsl:otherwise>
					</xsl:choose>
				</td>
			</xsl:if>
			<xsl:choose>
				<xsl:when test="$ShowReferencesInOneColumn='yes'">
					<xsl:if test="string-length($ReferenceAlias1)&gt;0">
						<td rowspan="3" width="{$ColWidthReference}" align="left" valign="top" style="border: 1px solid black;border-collapse:collapse;">
							<xsl:if test="count(Reference)&gt;0 and string-length(Reference/Alias)&gt;0"><xsl:value-of select="key('ReferenceIDKey',Reference/Alias)/Caption"/> - <br/><xsl:apply-templates select="Reference"/></xsl:if></td>
					</xsl:if>
				</xsl:when>
				<xsl:otherwise>
					<xsl:if test="string-length($ReferenceAlias1)&gt;0">
						<td rowspan="3" width="{$ColWidthReference}" align="left" valign="top" style="border: 1px solid black;border-collapse:collapse;">
							<xsl:apply-templates select="Reference[Alias=$ReferenceAlias1]"/>&#160;</td>
					</xsl:if>
					<xsl:if test="string-length($ReferenceAlias2)&gt;0">
						<td rowspan="3" width="{$ColWidthReference}" align="left" valign="top" style="border: 1px solid black;border-collapse:collapse;">
							<xsl:apply-templates select="Reference[Alias=$ReferenceAlias2]"/>&#160;</td>
					</xsl:if>
					<xsl:if test="string-length($ReferenceAlias3)&gt;0">
						<td rowspan="3" width="{$ColWidthReference}" align="left" valign="top" style="border: 1px solid black;border-collapse:collapse;">
							<xsl:apply-templates select="Reference[Alias=$ReferenceAlias3]"/>&#160;</td>
					</xsl:if>
				</xsl:otherwise>
			</xsl:choose>
			<xsl:if test="$IncludeChgIndic!='no'">
				<td rowspan="3" align="center" style="border: 1px solid black;border-collapse:collapse;">
					<xsl:choose>
						<xsl:when test="count(Reference[$ReferenceChangeID]/ChangeInfo)&gt;0">
							<xsl:apply-templates select="Reference[$ReferenceChangeID]/ChangeInfo" />
						</xsl:when>
						<xsl:otherwise>&#160;</xsl:otherwise>
					</xsl:choose>
				</td>
			</xsl:if>

			<td rowspan="3" align="center" style="border: 1px solid black;border-collapse:collapse;">
				<xsl:choose>
					<xsl:when test="string-length(@Row)&gt;0">
						<xsl:value-of select="@Row"/>
					</xsl:when>
					<xsl:otherwise>&#160;</xsl:otherwise>
				</xsl:choose>
			</td>
		</tr>
		<tr class="{$ChapterAlias}_{$Type}">
			<td width="{$ColWidth1}" align="left" style="border: 1px solid black;border-collapse:collapse;">
				<b>
					<xsl:value-of select="$Type" />
				</b>
			</td>
		</tr>
		<tr class="{$ChapterAlias}_{$Type}" align="left">
			<td width="{$ColWidth1}" style="border: 1px solid black;border-collapse:collapse;">
				<xsl:apply-templates select="Name" mode="FunctionTag">
					<xsl:with-param name="Depth" select="string-length(ID)-string-length(translate(ID,'.',''))"/>
				</xsl:apply-templates>

				<!--			<b><xsl:value-of select="Name"/></b>-->
			</td>
		</tr>
		<tr class="{$ChapterAlias}_{$Type}_Body">
			<td rowspan="{$RowSpan}" width="{$ColWidth1}" valign="top" align="left" style="border: 1px solid black;border-collapse:collapse;">
				<xsl:if test="string-length(Statement)&gt;2">
					<b>Statement</b>: <xsl:apply-templates select="Statement" mode="FormattedText"/>
					<!--<xsl:copy-of select="Statement/*"/>-->
					<p/>
				</xsl:if>
				<!--<xsl:if test="string-length(Statement)&gt;2 and (string-length(Description)&gt;2 or string-length(Example)&gt;2)"> </xsl:if>-->
				<xsl:if test="string-length(Description)&gt;2">
					<b>Description</b>: <xsl:apply-templates select="Description" mode="FormattedText"/>
					<p/>
				</xsl:if>
				<!--<xsl:if test="string-length(Description)&gt;2 and string-length(Example)&gt;2"> </xsl:if>-->
				<xsl:if test="string-length(Example)&gt;2">
					<b>Example</b>: <xsl:apply-templates select="Example" mode="FormattedText"/>
					<p/>
				</xsl:if>
				&#160;
			</td>
			<xsl:choose>
				<xsl:when test="$CriteriaCount&gt;0">
					<xsl:apply-templates select="Criteria[1]" mode="NoRow">
						<xsl:with-param name="ChapterAlias" select="$ChapterAlias"/>
						<xsl:with-param name="FunctionType" select="$Type"/>
					</xsl:apply-templates>
				</xsl:when>
				<xsl:otherwise>
					<xsl:if test="$IncludeSeeAlso!='no'">
						<td style="border: 1px solid black;border-collapse:collapse;">&#160;</td>
					</xsl:if>
					<td align="center" valign="top">&#160;</td>
					<xsl:choose>
						<xsl:when test="$ShowReferencesInOneColumn='yes'">
							<xsl:if test="string-length($ReferenceAlias1)&gt;0">
								<td align="center" style="border: 1px solid black;border-collapse:collapse;">&#160;</td>
							</xsl:if>
						</xsl:when>
						<xsl:otherwise>
							<xsl:if test="string-length($ReferenceAlias1)&gt;0">
								<td align="center" style="border: 1px solid black;border-collapse:collapse;">&#160;</td>
							</xsl:if>
							<xsl:if test="string-length($ReferenceAlias2)&gt;0">
								<td align="center" style="border: 1px solid black;border-collapse:collapse;">&#160;</td>
							</xsl:if>
							<xsl:if test="string-length($ReferenceAlias3)&gt;0">
								<td align="center" style="border: 1px solid black;border-collapse:collapse;">&#160;</td>
							</xsl:if>
						</xsl:otherwise>
					</xsl:choose>
					<td align="center" valign="top">&#160;</td>
					<xsl:if test="$IncludeChgIndic!='no'">
						<td align="center" valign="top" style="border: 1px solid black;border-collapse:collapse;">&#160;</td>
					</xsl:if>
				</xsl:otherwise>
			</xsl:choose>
		</tr>

		<xsl:if test="$CriteriaCount&gt;0">
			<xsl:apply-templates select="Criteria" mode="NewRow">
				<xsl:with-param name="ChapterAlias" select="$ChapterAlias"/>
				<xsl:with-param name="FunctionType" select="$Type"/>
			</xsl:apply-templates>
		</xsl:if>
	</xsl:template>
	
<!--	<xsl:template match="*" mode="copy-of">
        <xsl:element name="{local-name(.)}">
                <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>

    <xsl:template match="@*|node()">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()"/>
		</xsl:copy>
    </xsl:template>-
	
	<xsl:template match="@*|comment()|processing-instruction()|text()">
		<xsl:copy-of select="."/>
	</xsl:template>

	- elements: create a new element with the same name, but no namespace -
	<xsl:template match="*" mode="copy-of">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*|node()"/>
		</xsl:element>
	</xsl:template>-->
	
	<xsl:template match="Criteria" mode="NewRow">
		<xsl:param name="ChapterAlias"/>
		<xsl:param name="FunctionType"/>
		<xsl:if test="position()&gt;1">
			<tr class="{$ChapterAlias}_{$FunctionType}_Body">
				<xsl:apply-templates select="." mode="NoRow">
					<xsl:with-param name="ChapterAlias" select="$ChapterAlias"/>
					<xsl:with-param name="FunctionType" select="$FunctionType"/>
				</xsl:apply-templates>
			</tr>
		</xsl:if>
	</xsl:template>

	<xsl:template match="Criteria" mode="NoRow">
		<xsl:param name="ChapterAlias"/>
		<xsl:param name="FunctionType"/>

		<td width="{$ColWidth2}" valign="top" align="left" style="border: 1px solid black;border-collapse:collapse;">
			<a>
				<xsl:attribute name="name">Function_<xsl:value-of select="translate(../ID,' ','')"/>__Criteria_<xsl:value-of select="ID"/>
				</xsl:attribute>
				<xsl:value-of select="ID"/>.&#160;&#160;
			</a>
			<xsl:call-template name="FormatConformanceCriteria">
				<xsl:with-param name="Text">
					<xsl:copy-of select="Text"/>
				</xsl:with-param>
				<xsl:with-param name="Optionality">
					<xsl:value-of select="Optionality"/>
				</xsl:with-param>
			</xsl:call-template>&#160;
		</td>
		<xsl:if test="$IncludeSeeAlso!='no'">
			<td width="{$ColWidth3}" align="center" style="border: 1px solid black;border-collapse:collapse;">
				<xsl:choose>
					<xsl:when test="count(SeeAlso)&gt;0">
						<xsl:apply-templates select="SeeAlso"/>
					</xsl:when>
					<xsl:otherwise>&#160;</xsl:otherwise>
				</xsl:choose>
			</td>
		</xsl:if>
		<xsl:choose>
			<xsl:when test="$ShowReferencesInOneColumn='yes'">
				<xsl:if test="string-length($ReferenceAlias1)&gt;0">
					<td align="left" style="border: 1px solid black;border-collapse:collapse;">
					<xsl:if test="count(Reference)&gt;0 and string-length(Reference/Alias)&gt;0"><xsl:value-of select="key('ReferenceIDKey',Reference/Alias)/Caption"/> - <br/><xsl:apply-templates select="Reference"/></xsl:if>&#160;</td>
				</xsl:if>
			</xsl:when>
			<xsl:otherwise>
				<xsl:if test="string-length($ReferenceAlias1)&gt;0">
					<td width="{$ColWidthReference}" align="left" valign="top" style="border: 1px solid black;border-collapse:collapse;">
						<xsl:apply-templates select="Reference[Alias=$ReferenceAlias1]"/>&#160;</td>
				</xsl:if>
				<xsl:if test="string-length($ReferenceAlias2)&gt;0">
					<td width="{$ColWidthReference}" align="left" valign="top" style="border: 1px solid black;border-collapse:collapse;">
						<xsl:apply-templates select="Reference[Alias=$ReferenceAlias2]"/>&#160;</td>
				</xsl:if>
				<xsl:if test="string-length($ReferenceAlias3)&gt;0">
					<td width="{$ColWidthReference}" align="left" valign="top" style="border: 1px solid black;border-collapse:collapse;">
						<xsl:apply-templates select="Reference[Alias=$ReferenceAlias3]"/>&#160;</td>
				</xsl:if>
			</xsl:otherwise>
		</xsl:choose>
		<xsl:if test="$IncludeChgIndic!='no'">
			<td width="{$ColWidth8}" align="center" valign="top" style="border: 1px solid black;border-collapse:collapse;">
				<xsl:choose>
					<xsl:when test="count(Reference[$ReferenceChangeID]/ChangeInfo)&gt;0">
						<xsl:apply-templates select="Reference[$ReferenceChangeID]/ChangeInfo" />
					</xsl:when>
					<xsl:otherwise>&#160;</xsl:otherwise>
				</xsl:choose>
			</td>
		</xsl:if>		
		<td width="{$ColWidth7}" align="center" valign="top" style="border: 1px solid black;border-collapse:collapse;">
			<xsl:choose>
				<xsl:when test="string-length(@Row)&gt;0">
					<xsl:value-of select="@Row"/>
				</xsl:when>
				<xsl:otherwise>&#160;</xsl:otherwise>
			</xsl:choose>
		</td>
	</xsl:template>

	<xsl:template match="SeeAlso">
		<xsl:if test="position()&gt;1"/>
		<xsl:choose>
			<xsl:when test="CriteriaID!=''">
				<a>
					<xsl:attribute name="href">#<xsl:apply-templates select="." mode="LinkLocation"/>
					</xsl:attribute>
					<xsl:apply-templates select="Link" mode="LinkText"/>
				</a>
			</xsl:when>
			<xsl:otherwise>
				<a>
					<xsl:attribute name="href">#<xsl:apply-templates select="." mode="LinkLocation"/>
					</xsl:attribute>
					<xsl:apply-templates select="Link" mode="LinkText"/>
				</a>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="Reference">
		<xsl:variable name="URI">
			<xsl:value-of select="key('ReferenceIDKey',ID)/URI"/>
		</xsl:variable>
		<xsl:for-each select="Link">
			<xsl:variable name="LinkText">
				<xsl:apply-templates select="." mode="LinkText"/>
			</xsl:variable>
			<xsl:choose>
				<xsl:when test="string-length($URI)&gt;0">
					<xsl:variable name="Link">
						<xsl:value-of select="$URI" />#<xsl:apply-templates select="." mode="LinkLocation"/>
					</xsl:variable><a href="{$Link}">
						<xsl:value-of select="$LinkText"/>
					</a>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$LinkText"/>
				</xsl:otherwise>
			</xsl:choose>

		</xsl:for-each>
		<!--
		<p/>
		<xsl:variable name="FunctionID"><xsl:value-of select="Link/FunctionID"/></xsl:variable>
		-<xsl:value-of select="$ReferenceDocument1/FunctionalModel/"/>-
		
		<xsl:value-of select="$ReferenceURI1" />
		<xsl:for-each select="$ReferenceDocument1">
			~ence<xsl:value-of select="key('ReferenceFunctionIDKey', $FunctionID)" />
		</xsl:for-each>
		
		-->

	</xsl:template>

	<xsl:template match="Link" mode="LinkText">
		<xsl:choose>
			<xsl:when test="CriteriaID!=''">
				<span class="nobr">
					<xsl:value-of select="FunctionID"/>&#160;cc#<xsl:value-of select="CriteriaID"/>
				</span>
			</xsl:when>
			<xsl:otherwise>
				<span class="nobr">
					<xsl:value-of select="FunctionID"/>
				</span>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="ChangeInfo">
		<xsl:choose>
			<xsl:when test="Type='New'">A</xsl:when>
			<xsl:when test="Type='Modified'">C</xsl:when>
			<xsl:when test="Type='Moved'">M</xsl:when>
			<xsl:when test="Type='Deleted'">D</xsl:when>
			<xsl:when test="Type='Unchanged'">NC</xsl:when>
			<xsl:otherwise>&#160;</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="*" mode="LinkLocation">
		<xsl:choose>
			<xsl:when test="CriteriaID!=''">Function_<xsl:value-of select="translate(FunctionID,' ','')"/>__Criteria_<xsl:value-of select="CriteriaID"/>
			</xsl:when>
			<xsl:otherwise>Function_<xsl:value-of select="FunctionID"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="indent">
		<xsl:param name="depth"/>
		<xsl:if test="$depth > 0">
			<xsl:text>&#160;&#160;</xsl:text>
			<xsl:call-template name="indent">
				<xsl:with-param name="depth" select="$depth - 1"/>
			</xsl:call-template>
		</xsl:if>
	</xsl:template>

	<!--	<xsl:template match="*" mode="DateDisplay">
		<xsl:variable name="month" select="substring(., 6, 2)"/>
		<xsl:choose>
			<xsl:when test="$month='01'">
				<xsl:text>January </xsl:text>
			</xsl:when>
			<xsl:when test="$month='02'">
				<xsl:text>February </xsl:text>
			</xsl:when>
			<xsl:when test="$month='03'">
				<xsl:text>March </xsl:text>
			</xsl:when>
			<xsl:when test="$month='04'">
				<xsl:text>April </xsl:text>
			</xsl:when>
			<xsl:when test="$month='05'">
				<xsl:text>May </xsl:text>
			</xsl:when>
			<xsl:when test="$month='06'">
				<xsl:text>June </xsl:text>
			</xsl:when>
			<xsl:when test="$month='07'">
				<xsl:text>July </xsl:text>
			</xsl:when>
			<xsl:when test="$month='08'">
				<xsl:text>August </xsl:text>
			</xsl:when>
			<xsl:when test="$month='09'">
				<xsl:text>September </xsl:text>
			</xsl:when>
			<xsl:when test="$month='10'">
				<xsl:text>October </xsl:text>
			</xsl:when>
			<xsl:when test="$month='11'">
				<xsl:text>November </xsl:text>
			</xsl:when>
			<xsl:when test="$month='12'">
				<xsl:text>December </xsl:text>
			</xsl:when>
		</xsl:choose>
		<xsl:choose>
			<xsl:when test='substring(., 9, 1)="0"'>
				<xsl:value-of select="substring(., 10, 1)"/>
				<xsl:text>, </xsl:text>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="substring(., 9, 2)"/>
				<xsl:text>, </xsl:text>
			</xsl:otherwise>
		</xsl:choose>
		<xsl:value-of select="substring(., 1, 4)"/>
	</xsl:template>	
-->

	<xsl:template match="*" mode="DateDisplay">
		<xsl:param name="ShowYear">yes</xsl:param>
		<xsl:param name="ShowMonth">yes</xsl:param>
		<xsl:param name="ShowDay">yes</xsl:param>

		<xsl:variable name="month" select="substring(., 6, 2)"/>
		<xsl:if test="not($ShowMonth='no')">
			<xsl:choose>
				<xsl:when test="$month='01'">
					<xsl:text>January </xsl:text>
				</xsl:when>
				<xsl:when test="$month='02'">
					<xsl:text>February </xsl:text>
				</xsl:when>
				<xsl:when test="$month='03'">
					<xsl:text>March </xsl:text>
				</xsl:when>
				<xsl:when test="$month='04'">
					<xsl:text>April </xsl:text>
				</xsl:when>
				<xsl:when test="$month='05'">
					<xsl:text>May </xsl:text>
				</xsl:when>
				<xsl:when test="$month='06'">
					<xsl:text>June </xsl:text>
				</xsl:when>
				<xsl:when test="$month='07'">
					<xsl:text>July </xsl:text>
				</xsl:when>
				<xsl:when test="$month='08'">
					<xsl:text>August </xsl:text>
				</xsl:when>
				<xsl:when test="$month='09'">
					<xsl:text>September </xsl:text>
				</xsl:when>
				<xsl:when test="$month='10'">
					<xsl:text>October </xsl:text>
				</xsl:when>
				<xsl:when test="$month='11'">
					<xsl:text>November </xsl:text>
				</xsl:when>
				<xsl:when test="$month='12'">
					<xsl:text>December </xsl:text>
				</xsl:when>
			</xsl:choose>
		</xsl:if>
		<xsl:if test="not($ShowDay='no')">
			<xsl:choose>
				<xsl:when test='substring(., 9, 1)="0"'>
					<xsl:value-of select="substring(., 10, 1)"/>
					<xsl:text>, </xsl:text>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="substring(., 9, 2)"/>
					<xsl:text>, </xsl:text>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:if>
		<xsl:if test="not($ShowYear='no')">
			<xsl:value-of select="substring(., 1, 4)"/>
		</xsl:if>
	</xsl:template>	


	<xsl:template name="FormatText">
		<xsl:param name="Text"/>
		<xsl:choose>
			<xsl:when test="contains($Text,'&#10;')">
				<xsl:value-of select="substring-before($Text,'&#10;')"/>
				&#160;
				<xsl:call-template name="FormatText">
					<xsl:with-param name="Text" select="substring-after($Text,'&#10;')"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$Text"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>


	<xsl:template name="FormatConformanceCriteria">
		<xsl:param name="Text"/>
		<xsl:param name="Optionality"/>
		<xsl:param name="ConditionFunctionIteration"/>
		<xsl:param name="ConditionCriteriaIteration"/>
		<xsl:param name="OptionalityIteration"/>

		<xsl:variable name="ConditionCriteriaString">Conformance Criteria #<xsl:value-of select="ConditionLink/CriteriaID"/>
		</xsl:variable>		
		<xsl:variable name="ConsequenceCriteriaString">Conformance Criteria #<xsl:value-of select="ConsequenceLink/CriteriaID"/>
		</xsl:variable>		
		<xsl:choose>
			<xsl:when test="ConditionLink/FunctionID!='' and contains(substring-before($Text, 'THEN'),ConditionLink/FunctionID)">
				<xsl:value-of select="substring-before($Text,ConditionLink/FunctionID)"/>
				<a>
					<xsl:attribute name="href">#<xsl:apply-templates select="ConditionLink" mode="LinkLocation"/>
					</xsl:attribute>
					<xsl:value-of select="ConditionLink/FunctionID"/>
				</a>
				<xsl:call-template name="FormatConformanceCriteria">
					<xsl:with-param name="Text" select="substring-after($Text,ConditionLink/FunctionID)"/>
					<xsl:with-param name="Optionality" select="$Optionality"/>

				</xsl:call-template>
			</xsl:when>
			<xsl:when test="ConditionLink/CriteriaID!='' and contains(substring-before($Text, 'THEN'),$ConditionCriteriaString)">
				<xsl:value-of select="substring-before($Text,$ConditionCriteriaString)"/>
				<a>
					<xsl:attribute name="href">#<xsl:apply-templates select="ConditionLink" mode="LinkLocation"/>
					</xsl:attribute>
					<xsl:value-of select="$ConditionCriteriaString"/>
				</a>

				<xsl:call-template name="FormatConformanceCriteria">
					<xsl:with-param name="Text" select="substring-after($Text,$ConditionCriteriaString)"/>
					<xsl:with-param name="Optionality" select="$Optionality"/>

				</xsl:call-template>
			</xsl:when>
			<xsl:when test="$OptionalityIteration='' and $Optionality!='' and contains($Text,$Optionality)">
				<xsl:value-of select="substring-before($Text,$Optionality)"/>
				<b>
					<xsl:value-of select="$Optionality"/>
				</b>
				<xsl:call-template name="FormatConformanceCriteria">
					<xsl:with-param name="Text" select="substring-after($Text,$Optionality)"/>
					<xsl:with-param name="Optionality" select="$Optionality"/>
					<xsl:with-param name="OptionalityIteration" select="1"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="ConsequenceLink/FunctionID!='' and contains($Text,ConsequenceLink/FunctionID)">
				<xsl:value-of select="substring-before($Text,ConsequenceLink/FunctionID)"/>
				<a>
					<xsl:attribute name="href">#<xsl:apply-templates select="ConsequenceLink" mode="LinkLocation"/>
					</xsl:attribute>
					<xsl:value-of select="ConsequenceLink/FunctionID"/>
				</a>
				<xsl:call-template name="FormatConformanceCriteria">
					<xsl:with-param name="Text" select="substring-after($Text,ConsequenceLink/FunctionID)"/>
					<xsl:with-param name="Optionality" select="$Optionality"/>
					<xsl:with-param name="OptionalityIteration" select="$OptionalityIteration"/>

				</xsl:call-template>
			</xsl:when>
			<xsl:when test="ConsequenceLink/CriteriaID!='' and contains($Text,$ConsequenceCriteriaString)">
				<xsl:value-of select="substring-before($Text,$ConsequenceCriteriaString)"/>
				<a>
					<xsl:attribute name="href">#<xsl:apply-templates select="ConsequenceLink" mode="LinkLocation"/>
					</xsl:attribute>
					<xsl:value-of select="$ConsequenceCriteriaString"/>
				</a>

				<xsl:call-template name="FormatConformanceCriteria">
					<xsl:with-param name="Text" select="substring-after($Text,$ConsequenceCriteriaString)"/>
					<xsl:with-param name="Optionality" select="$Optionality"/>
					<xsl:with-param name="OptionalityIteration" select="$OptionalityIteration"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$Text"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<!-- extra statistics for QA only, not for publication -->
	<xsl:template match="*" mode="Statistics">
		<xsl:variable name="HeaderCount">
			<xsl:for-each select="//Function">
				<xsl:if test="Type='H'">1</xsl:if>
			</xsl:for-each>
		</xsl:variable>
		<xsl:variable name="FunctionCount">
			<xsl:for-each select="//Function">
				<xsl:if test="Type='F'">1</xsl:if>
			</xsl:for-each>
		</xsl:variable>
		<xsl:variable name="DependentCount">
			<xsl:for-each select="//ConformanceCriteria">
				<xsl:if test="Type='Dependent'">1</xsl:if>
			</xsl:for-each>
		</xsl:variable>
		<xsl:variable name="ConditionalCount">
			<xsl:for-each select="//ConformanceCriteria">
				<xsl:if test="Type='Conditional'">1</xsl:if>
			</xsl:for-each>
		</xsl:variable>
		<xsl:variable name="FunctionLinkCount">
			<xsl:for-each select="//ConformanceCriteria/Link">
				<xsl:if test="FunctionID!=''">1</xsl:if>
			</xsl:for-each>
		</xsl:variable>
		<xsl:variable name="CriteriaLinkCount">
			<xsl:for-each select="//ConformanceCriteria/Link">
				<xsl:if test="CriteriaID!=''">1</xsl:if>
			</xsl:for-each>
		</xsl:variable>


		<xsl:variable name="ShallCriteriaCount">
			<xsl:for-each select="//ConformanceCriteria">
				<xsl:if test="Optionality='SHALL'">1</xsl:if>
			</xsl:for-each>
		</xsl:variable>
		<xsl:variable name="ShouldCriteriaCount">
			<xsl:for-each select="//ConformanceCriteria">
				<xsl:if test="Optionality='SHOULD'">1</xsl:if>
			</xsl:for-each>
		</xsl:variable>
		<xsl:variable name="MayCriteriaCount">
			<xsl:for-each select="//ConformanceCriteria">
				<xsl:if test="Optionality='MAY'">1</xsl:if>
			</xsl:for-each>
		</xsl:variable>

		<xsl:variable name="OtherCriteriaCount">
			<xsl:for-each select="//ConformanceCriteria">
				<xsl:if test="Optionality!='SHALL' and Optionality!='SHOULD' and Optionality!='MAY'">1</xsl:if>
			</xsl:for-each>
		</xsl:variable>
		<xsl:variable name="OtherCriteria">
			<xsl:for-each select="//ConformanceCriteria">
				<xsl:if test="Optionality!='SHALL' and Optionality!='SHOULD' and Optionality!='MAY'">
          Function <xsl:value-of select="../ID"/>, Criteria <xsl:value-of select="ID"/>;&#160;&#160;
				</xsl:if>
			</xsl:for-each>
		</xsl:variable>

		<xsl:variable name="OtherOptionalityCriteriaCount">
			<xsl:for-each select="//ConformanceCriteria">
				<xsl:if test="not(contains(Text,Optionality)) or Optionality=''">1</xsl:if>
			</xsl:for-each>
		</xsl:variable>
		<xsl:variable name="OtherOptionalityCriteria">
			<xsl:for-each select="//ConformanceCriteria">
				<xsl:if test="not(contains(Text,Optionality)) or Optionality=''">
          Function <xsl:value-of select="../ID"/>, Criteria <xsl:value-of select="ID"/>;&#160;&#160;
				</xsl:if>
			</xsl:for-each>
		</xsl:variable>

		<xsl:variable name="OtherTypeCriteriaCount">
			<xsl:for-each select="//ConformanceCriteria">
				<xsl:if test="Type!='' and Type!='Conditional' and Type!='Dependent'">1</xsl:if>
			</xsl:for-each>
		</xsl:variable>
		<xsl:variable name="OtherTypeCriteria">
			<xsl:for-each select="//ConformanceCriteria">
				<xsl:if test="Type!='' and Type!='Conditional' and Type!='Dependent'">
          Function <xsl:value-of select="../ID"/>, Criteria <xsl:value-of select="ID"/>;&#160;&#160;
				</xsl:if>
			</xsl:for-each>
		</xsl:variable>

		<h2>Stats</h2>
		<b>Chapter Count:</b>
		<xsl:value-of select="count(//Chapter)"/>
		<p/>
		<b>Function Count:</b>
		<xsl:value-of select="count(//Function)"/>
    &#160;&#160;&#160;Header:  <xsl:value-of select="string-length($HeaderCount)"/>
    &#160;&#160;&#160;Function:  <xsl:value-of select="string-length($FunctionCount)"/>
		<p/>
		<b>SeeAlso Count:</b>
		<xsl:value-of select="count(//SeeAlso)"/>
		<p/>
		<b>Criteria Count:</b>
		<xsl:value-of select="count(//ConformanceCriteria)"/>
		<p/>
    &#160;&#160;&#160;Dependent:  <xsl:value-of select="string-length($DependentCount)"/>
    &#160;&#160;&#160;Conditional:  <xsl:value-of select="string-length($ConditionalCount)"/> 
    &#160;&#160;&#160;Function Link:  <xsl:value-of select="string-length($FunctionLinkCount)"/> 
    &#160;&#160;&#160;Criteria Link:  <xsl:value-of select="string-length($CriteriaLinkCount)"/> 
    &#160;&#160;&#160;Shall Criteria:  <xsl:value-of select="string-length($ShallCriteriaCount)"/> 
    &#160;&#160;&#160;Should Criteria:  <xsl:value-of select="string-length($ShouldCriteriaCount)"/> 
    &#160;&#160;&#160;May Criteria:  <xsl:value-of select="string-length($MayCriteriaCount)"/>
		<p/>
    &#160;&#160;&#160;Err Criteria:  <xsl:value-of select="string-length($OtherCriteriaCount)"/> 
    &#160;&#160;&#160;&#160;&#160;&#160;<xsl:value-of select="$OtherCriteria"/>
		<p/>

    &#160;&#160;&#160;Err CC Optionality:  <xsl:value-of select="string-length($OtherOptionalityCriteriaCount)"/> 
    &#160;&#160;&#160;&#160;&#160;&#160;<xsl:value-of select="$OtherOptionalityCriteria"/>
		<p/>

    &#160;&#160;&#160;Err CC Type:  <xsl:value-of select="string-length($OtherTypeCriteriaCount)"/> 
    &#160;&#160;&#160;&#160;&#160;&#160;<xsl:value-of select="$OtherTypeCriteria"/>
		<p/>
		<hr/>
		<h2>Rule checking</h2>
		<h3>Not Conditional?</h3>
		<xsl:for-each select="//ConformanceCriteria">
			<xsl:if test="(contains(Text,'IF') and contains(Text,'THEN')) and (Type!='Conditional' or not(Type))">
        Function <xsl:value-of select="../ID"/>, Criteria <xsl:value-of select="ID"/>;&#160;&#160;

        &#160;&#160;&#160;&#160;<xsl:value-of select="Text"/>
				<p/>
			</xsl:if>
		</xsl:for-each>
		<p/>
		<h3>Conditional?</h3>
		<xsl:for-each select="//ConformanceCriteria">
			<xsl:if test="(not(contains(Text,'IF')) and not(contains(Text,'THEN'))) and (Type='Conditional')">
        Function <xsl:value-of select="../ID"/>, Criteria <xsl:value-of select="ID"/>;&#160;&#160;

        &#160;&#160;&#160;&#160;<xsl:value-of select="Text"/>
				<p/>
			</xsl:if>
		</xsl:for-each>
		<p/>
		<h3>Not Linked?</h3>
		<xsl:for-each select="//ConformanceCriteria">
			<xsl:if test="(contains(Text,'DC.') or contains(Text,'S.') or contains(Text,'IN.')) and (Link='' or not(Link))">
        Function <xsl:value-of select="../ID"/>, Criteria <xsl:value-of select="ID"/>;&#160;&#160;

        &#160;&#160;&#160;&#160;<xsl:value-of select="Text"/>
				<p/>
			</xsl:if>
		</xsl:for-each>

		<h3>Not Dependent?</h3>
		<xsl:for-each select="//ConformanceCriteria">
			<xsl:if test="contains(Text,'jurisdiction') and (Type!='Dependent' or not(Type))">
        Function <xsl:value-of select="../ID"/>, Criteria <xsl:value-of select="ID"/>;&#160;&#160;

        &#160;&#160;&#160;&#160;<xsl:value-of select="Text"/>
				<p/>
			</xsl:if>
		</xsl:for-each>
		<h3>Dependent?</h3>
		<xsl:for-each select="//ConformanceCriteria">
			<xsl:if test="not(contains(Text,'jurisdiction')) and Type='Dependent'">
        Function <xsl:value-of select="../ID"/>, Criteria <xsl:value-of select="ID"/>;&#160;&#160;

        &#160;&#160;&#160;&#160;<xsl:value-of select="Text"/>
				<p/>
			</xsl:if>
		</xsl:for-each>
		<p/>



		<!--Verify Dependents are shalls-->
		<h3>Non-Shall Dependent Criteria</h3>
		<xsl:for-each select="//ConformanceCriteria">
			<xsl:if test="Type='Dependent' and Optionality!='SHALL'">
        Function <xsl:value-of select="../ID"/>, Criteria <xsl:value-of select="ID"/>;&#160;&#160;

        &#160;&#160;&#160;&#160;<xsl:value-of select="Text"/>
				<p/>
			</xsl:if>
		</xsl:for-each>
		<!--Verify Dependents are formed correctly-->
		<h3>Malformed Dependent Criteria <i>(expecting 'scope of practice, organizational policy, or jurisdictional law.'</i>
		</h3>
		<xsl:for-each select="//ConformanceCriteria">
			<xsl:if test="Type='Dependent' and not(contains(Text, 'scope of practice, organizational policy, or jurisdictional law.'))">
        Function <xsl:value-of select="../ID"/>, Criteria <xsl:value-of select="ID"/>;&#160;&#160;

        &#160;&#160;&#160;&#160;<xsl:value-of select="Text"/>
				<p/>
			</xsl:if>
		</xsl:for-each>

		<!--Verify all functions type f have a statement-->
		<h3>Funtions without a Statement</h3>
		<xsl:for-each select="//Function">
			<xsl:if test="Type='F' and string-length(Statement)=0">
        Function <xsl:value-of select="ID"/>&#160;&#160;<xsl:value-of select="Name"/> 
			</xsl:if>
		</xsl:for-each>
		<p/>
		<h3>Headers without Children</h3>
		<xsl:for-each select="//Function">
			<xsl:if test="Type='H' and count(Function)=0">
        Function <xsl:value-of select="ID"/>&#160;&#160;<xsl:value-of select="Name"/> 
			</xsl:if>
		</xsl:for-each>
		<p/>
		<h3>Functions with Children</h3>
		<xsl:for-each select="//Function">
			<xsl:if test="Type='F' and count(Function)&gt;0">
        Function <xsl:value-of select="ID"/>&#160;&#160;<xsl:value-of select="Name"/> 
			</xsl:if>
		</xsl:for-each>

		<!--Check no Periods at end of Function name-->
		<h3>Function Names with Periods</h3>
		<xsl:for-each select="//Function">
			<xsl:if test="substring(Name, string-length(Name), 1)='.'">
        Function <xsl:value-of select="ID"/>&#160;&#160;<xsl:value-of select="Name"/> 
			</xsl:if>
		</xsl:for-each><p/>

		<!--Check Periods at end of statement-->
		<h3>Statements without Periods</h3>
		<xsl:for-each select="//Function">
			<xsl:if test="Statement!='' and substring(Statement, string-length(Statement), 1)!='.'">
        Function <xsl:value-of select="ID"/>&#160;&#160;<xsl:value-of select="Name"/> 
			</xsl:if>
		</xsl:for-each><p/>
		<!--Check Periods at end of Description-->
		<h3>Descriptions without Periods</h3>
		<xsl:for-each select="//Function">
			<xsl:if test="Description!='' and substring(Description, string-length(Description), 1)!='.'">
        Function <xsl:value-of select="ID"/>&#160;&#160;<xsl:value-of select="Name"/> 
			</xsl:if>
		</xsl:for-each><p/>
		<!--Check Periods at end of cc text-->
		<h3>Criteria without Periods</h3>
		<xsl:for-each select="//ConformanceCriteria">
			<xsl:if test="substring(Text, string-length(Text), 1)!='.'">
        Function <xsl:value-of select="../ID"/>, Criteria <xsl:value-of select="ID"/>;&#160;&#160; 
			</xsl:if>
		</xsl:for-each><p/>

		<p/>
		<h3>Redundant See Also?</h3>
		<!--    <xsl:variable name="ReSee">-->
		<xsl:for-each select="//Function">
			<xsl:for-each select="SeeAlso">
				<xsl:variable name="SeeAlso">
					<xsl:value-of select="FunctionID"/>
				</xsl:variable>
				<xsl:for-each select="../ConformanceCriteria">
					<xsl:if test="Link/FunctionID=$SeeAlso">
              Function <xsl:value-of select="../ID"/>, Criteria <xsl:value-of select="ID"/>;&#160;&#160;

              &#160;&#160;&#160;&#160;<xsl:value-of select="$SeeAlso"/>
						<p/>
					</xsl:if>
				</xsl:for-each>
			</xsl:for-each>
		</xsl:for-each>
		<p/>
	</xsl:template>

	<xsl:template match="Functions">		
		<xsl:param name="ChapterAlias"/>
		<xsl:param name="depth">0</xsl:param>
		<xsl:variable name="CriteriaCount">
			<xsl:value-of select="count(ConformanceCriteria)"/>
		</xsl:variable>
		<tr valign="top" align="left">
			<xsl:attribute name="class">
				<xsl:value-of select="$ChapterAlias"/>
				<xsl:choose>
					<xsl:when test="Type='H'">_Header</xsl:when>
					<xsl:otherwise>_Function</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
			<td>
				<xsl:if test="$CriteriaCount&gt;0">
					<xsl:attribute name="rowspan">
						<xsl:value-of select="$CriteriaCount"/>
					</xsl:attribute>
				</xsl:if>
				<a>
					<xsl:attribute name="name">Function_<xsl:value-of select="ID"/>
					</xsl:attribute>
					<b>
						<xsl:value-of select="ID"/>
					</b>
				</a>
			</td>
			<td>
				<xsl:if test="$CriteriaCount&gt;0">
					<xsl:attribute name="rowspan">
						<xsl:value-of select="$CriteriaCount"/>
					</xsl:attribute>
				</xsl:if>
				<b>
					<xsl:value-of select="Type"/>&#160;</b>
			</td>
			<td>
				<xsl:if test="$CriteriaCount&gt;0">
					<xsl:attribute name="rowspan">
						<xsl:value-of select="$CriteriaCount"/>
					</xsl:attribute>
				</xsl:if>
				<xsl:choose>
					<xsl:when test="$depth=0">
						<b>
							<xsl:value-of select="Name"/>&#160;
						</b>    
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="Name"/>&#160;
					</xsl:otherwise>
				</xsl:choose>
			</td>
			<!--			
			<th>
				<xsl:attribute name="rowspan"><xsl:value-of select="$CriteriaCount"/></xsl:attribute>
				<xsl:call-template name="FormatText">
					<xsl:with-param name="Text"><xsl:value-of select="Statement"/></xsl:with-param>
				</xsl:call-template>&#160;
			</th>
			<th>
				<xsl:attribute name="rowspan"><xsl:value-of select="$CriteriaCount"/></xsl:attribute>
				<xsl:call-template name="FormatText">
					<xsl:with-param name="Text"><xsl:value-of select="Description"/></xsl:with-param>
				</xsl:call-template>&#160;
			</th>
-->			
			<td>
				<xsl:if test="$CriteriaCount&gt;0">
					<xsl:attribute name="rowspan">
						<xsl:value-of select="$CriteriaCount"/>
					</xsl:attribute>
				</xsl:if>
				<xsl:if test="Statement!=''">
					<b>Statement:</b>&#160;&#160;
					<xsl:call-template name="FormatText">
						<xsl:with-param name="Text">
							<xsl:value-of select="Statement"/>
						</xsl:with-param>
					</xsl:call-template>
					<p/>
				</xsl:if>
				<xsl:if test="Description!=''">
					<b>Description:</b>&#160;&#160;
					<xsl:call-template name="FormatText">
						<xsl:with-param name="Text">
							<xsl:value-of select="Description"/>
						</xsl:with-param>
					</xsl:call-template>
				</xsl:if>
				&#160;
			</td>
			<td>
				<xsl:if test="$CriteriaCount&gt;0">
					<xsl:attribute name="rowspan">
						<xsl:value-of select="$CriteriaCount"/>
					</xsl:attribute>
				</xsl:if>
				<xsl:apply-templates select="SeeAlso"/>&#160;
			</td>
			<!-- handle the case where there is no conformance criteria, but want something to have a cell bgcolor-->
			<xsl:if test="$CriteriaCount=0">
				<td width="250">&#160;</td>
			</xsl:if>
			<xsl:apply-templates select="ConformanceCriteria" mode="First">
				<xsl:with-param name="ChapterAlias" select="$ChapterAlias"/>
			</xsl:apply-templates>
		</tr>
		<xsl:apply-templates select="ConformanceCriteria" mode="Other">
			<xsl:with-param name="ChapterAlias" select="$ChapterAlias"/>
		</xsl:apply-templates>

		<xsl:apply-templates select="Function">
			<xsl:with-param name="ChapterAlias" select="$ChapterAlias"/>
			<xsl:with-param name="depth" select="$depth + 1"/>
		</xsl:apply-templates>
	</xsl:template>

	<xsl:template match="ConformanceCriterias" mode="First">
		<xsl:param name="ChapterAlias"/>
		<xsl:if test="position()=1">
			<td width="350">
				<a>
					<xsl:attribute name="name">Function_<xsl:value-of select="translate(../ID,' ','')"/>__Criteria_<xsl:value-of select="ID"/>
					</xsl:attribute>
					<xsl:value-of select="ID"/>.&#160;&#160;
				</a>
				<xsl:call-template name="FormatConformanceCriteria">
					<xsl:with-param name="Text">
						<xsl:value-of select="Text"/>
					</xsl:with-param>
					<xsl:with-param name="Optionality">
						<xsl:value-of select="Optionality"/>
					</xsl:with-param>
				</xsl:call-template>&#160;
			</td>
		</xsl:if>
	</xsl:template>

	<xsl:template match="ConformanceCriterias" mode="Other">
		<xsl:param name="ChapterAlias"/>
		<xsl:if test="position()!=1">
			<tr valign="top">
				<xsl:attribute name="class">
					<xsl:value-of select="$ChapterAlias"/>
					<xsl:choose>
						<xsl:when test="../Type='H'">_Header</xsl:when>
						<xsl:otherwise>_Function</xsl:otherwise>
					</xsl:choose>
				</xsl:attribute>
				<td width="350">
					<a>
						<xsl:attribute name="name">Function_<xsl:value-of select="translate(../ID,' ','')"/>__Criteria_<xsl:value-of select="ID"/>
						</xsl:attribute>
						<xsl:value-of select="ID"/>.&#160;&#160;
					</a>
					<xsl:call-template name="FormatConformanceCriteria">
						<xsl:with-param name="Text">
							<xsl:value-of select="Text"/>
						</xsl:with-param>
						<xsl:with-param name="Optionality">
							<xsl:value-of select="Optionality"/>
						</xsl:with-param>
					</xsl:call-template>&#160;
				</td>
			</tr>
		</xsl:if>
	</xsl:template>


	<xsl:template match="*" mode="FunctionTag">
		<xsl:param name="Depth"/>
		<!--
	<xsl:choose>
		<xsl:when test="$Depth='1'"><h1><xsl:value-of select="."/></h1></xsl:when>
		<xsl:when test="$Depth='2'"><h2><xsl:value-of select="."/></h2></xsl:when>
		<xsl:when test="$Depth='3'"><h3><xsl:value-of select="."/></h3></xsl:when>
		<xsl:when test="$Depth='4'"><h4><xsl:value-of select="."/></h4></xsl:when>
		<xsl:when test="$Depth='5'"><h5><xsl:value-of select="."/></h5></xsl:when>
		<xsl:when test="$Depth='6'"><h6><xsl:value-of select="."/></h6></xsl:when>
		<xsl:otherwise><xsl:value-of select="."/></xsl:otherwise>
	</xsl:choose>
	-->
		<div title="{.}">
			<b>
				<xsl:value-of select="."/>
			</b>
		</div>
	</xsl:template>
	
	<!-- For handling linking of glossary terms -->
	<xsl:template match="*" name="LinkGlossaryTerm">
		<xsl:call-template name="replace-string">
			<xsl:with-param name="text" select="."/>
			<xsl:with-param name="replace" select="capture"/>
			<xsl:with-param name="with" select="**capture**"/>
		</xsl:call-template>
	</xsl:template>
	
	<xsl:template name="replace-string">
		<xsl:param name="text"/>
		<xsl:param name="replace"/>
		<xsl:param name="with"/>
		<xsl:choose>
			<xsl:when test="contains($text,$replace)">
				<xsl:value-of select="substring-before($text,$replace)"/>
				<xsl:value-of select="$with"/>
				<xsl:call-template name="replace-string">
					<xsl:with-param name="text" select="substring-after($text,$replace)"/>
					<xsl:with-param name="replace" select="$replace"/>
					<xsl:with-param name="with" select="$with"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$text"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="*" mode="NumberToEnglish">
		<xsl:param name="num" as="item()" />
		<!--<xsl:variable name="one-twenty" as="item()*" select="('one', 'two', 'three', 'four', 'five', 'six', 'seven', 'eight', 'nine', 'ten', 'eleven', 'twelve', 'thirteen', 'fourteen', 'fifteen', 'sixteen', 'seventeen', 'eighteen', 'nineteen')" />-->
		<!--<xsl:variable name="twenty-hundred" select="('NaN', 'twenty', 'thirty', 'fourty', 'fifty', 'sixty', 'seventy', 'eighty', 'ninety')" />-->
		<xsl:choose>
			<xsl:when test="$num=0"/>
			<xsl:when test="$num=1">one</xsl:when>
			<xsl:when test="$num=2">two</xsl:when>
			<xsl:when test="$num=3">three</xsl:when>
			<xsl:when test="$num=4">four</xsl:when>
			<xsl:when test="$num=5">five</xsl:when>
			<xsl:when test="$num=6">six</xsl:when>
			<xsl:when test="$num=7">seven</xsl:when>
			<xsl:when test="$num=8">eight</xsl:when>
			<xsl:when test="$num=9">nine</xsl:when>
			<xsl:when test="$num=10">ten</xsl:when>
			<xsl:when test="$num=11">eleven</xsl:when>
			<xsl:when test="$num=12">twelve</xsl:when>
			<xsl:when test="$num=13">thirteen</xsl:when>
			<xsl:when test="$num=14">fourteen</xsl:when>
			<xsl:when test="$num=15">fifteen</xsl:when>
			<xsl:when test="$num=16">sixteen</xsl:when>
			<xsl:when test="$num=17">seventeen</xsl:when>
			<xsl:when test="$num=18">eighteen</xsl:when>
			<xsl:when test="$num=19">nineteen</xsl:when>
			<xsl:when test="$num&gt;89">ninety<xsl:apply-templates select="." mode="NumberToEnglish">
					<xsl:with-param name="num" select="$num - 90"/>
				</xsl:apply-templates>
			</xsl:when>
			<xsl:when test="$num&gt;79">eighty<xsl:apply-templates select="." mode="NumberToEnglish">
					<xsl:with-param name="num" select="$num - 80"/>
				</xsl:apply-templates>
			</xsl:when>
			<xsl:when test="$num&gt;69">seventy<xsl:apply-templates select="." mode="NumberToEnglish">
					<xsl:with-param name="num" select="$num - 70"/>
				</xsl:apply-templates>
			</xsl:when>
			<xsl:when test="$num&gt;59">sixty<xsl:apply-templates select="." mode="NumberToEnglish">
					<xsl:with-param name="num" select="$num - 60"/>
				</xsl:apply-templates>
			</xsl:when>
			<xsl:when test="$num&gt;49">fifty<xsl:apply-templates select="." mode="NumberToEnglish">
					<xsl:with-param name="num" select="$num - 50"/>
				</xsl:apply-templates>
			</xsl:when>
			<xsl:when test="$num&gt;39">forty<xsl:apply-templates select="." mode="NumberToEnglish">
					<xsl:with-param name="num" select="$num - 40"/>
				</xsl:apply-templates>
			</xsl:when>
			<xsl:when test="$num&gt;29">thirty<xsl:apply-templates select="." mode="NumberToEnglish">
					<xsl:with-param name="num" select="$num - 30"/>
				</xsl:apply-templates>
			</xsl:when>
			<xsl:when test="$num&gt;19">twenty<xsl:apply-templates select="." mode="NumberToEnglish">
					<xsl:with-param name="num" select="$num - 20"/>
				</xsl:apply-templates>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$num"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<!-- Formatted text rendering -
	-   Text   -
	<xsl:template match="text" mode="componentText">
		<xsl:apply-templates/>

	</xsl:template>

	-      Tables   -
	<xsl:template match="table/@*|thead/@*|tfoot/@*|tbody/@*|colgroup/@*|col/@*|tr/@*|th/@*|td/@*">
		<xsl:copy>
			<xsl:apply-templates/>
		</xsl:copy>
	</xsl:template>

	<xsl:template match="table">
		<table class="second" width="100%">
			<xsl:apply-templates/>
		</table>
	</xsl:template>

	<xsl:template match="thead">
		<thead class="fifth">
			<xsl:apply-templates/>
		</thead>
	</xsl:template>

	<xsl:template match="tfoot">
		<tfoot>
			<xsl:apply-templates/>
		</tfoot>
	</xsl:template>

	<xsl:template match="tbody">
		<tbody>
			<xsl:apply-templates/>
		</tbody>
	</xsl:template>

	<xsl:template match="colgroup">
		<colgroup>
			<xsl:apply-templates/>
		</colgroup>
	</xsl:template>

	<xsl:template match="col">
		<col>
			<xsl:apply-templates/>
		</col>
	</xsl:template>

	<xsl:template match="tr">
		<xsl:variable name="Class">
			<xsl:if test="name(parent::node())!='thead'">
				<xsl:choose>
					<xsl:when test="position() mod 2 = 0">first</xsl:when>
					<xsl:otherwise>second</xsl:otherwise>
				</xsl:choose>
			</xsl:if>
		</xsl:variable>
		<tr class="{$Class}">
			<xsl:apply-templates/>
		</tr>
	</xsl:template>

	<xsl:template match="th">
		<th class="content">
			<xsl:apply-templates/>
		</th>
	</xsl:template>


	<xsl:template match="td">
		<td class="content">
			<xsl:apply-templates/>
		</td>
	</xsl:template>

	<xsl:template match="table/caption">
		<span style="font-weight:bold; ">
			<xsl:apply-templates/>
		</span>
	</xsl:template>



	-   paragraph  -
	<xsl:template match="paragraph">
		<xsl:apply-templates/>

	</xsl:template>

	-     Content w/ deleted text is hidden -
	<xsl:template match="content[@revised='delete']"/>

	-   content  -
	<xsl:template match="content">
		<xsl:apply-templates/>
	</xsl:template>

	-   list  -
	<xsl:template match="list">
		<xsl:if test="caption">
			<span style="font-weight:bold; ">
				<xsl:apply-templates select="caption"/>
			</span>
		</xsl:if>
		<ul>
			<xsl:for-each select="item">
				<li>
					<xsl:apply-templates/>
				</li>
			</xsl:for-each>
		</ul>
	</xsl:template>

	<xsl:template match="list[@listType='ordered']">
		<xsl:if test="caption">
			<span style="font-weight:bold; ">
				<xsl:apply-templates select="caption"/>
			</span>
		</xsl:if>
		<ol>
			<xsl:for-each select="item">
				<li>
					<xsl:apply-templates/>
				</li>
			</xsl:for-each>
		</ol>
	</xsl:template>

	-   caption  -
	<xsl:template match="caption">
		<xsl:apply-templates/>
		<xsl:text>: </xsl:text>
	</xsl:template>

-->

	<!--   RenderMultiMedia 

this currently only handles GIF's and JPEG's.  It could, however,
be extended by including other image MIME types in the predicate
and/or by generating <object> or <applet> tag with the correct
params depending on the media type  @ID  =$imageRef     referencedObject
-->
	<xsl:template match="renderMultiMedia">
		<xsl:variable name="imageRef" select="@referencedObject"/>
		<xsl:choose>
			<xsl:when test="//regionOfInterest[@ID=$imageRef]">
				<!-- Here is where the Region of Interest image referencing goes -->
				<xsl:if test='//regionOfInterest[@ID=$imageRef]//observationMedia/value[@mediaType="image/gif" or @mediaType="image/jpeg"]'>
					<br clear='all'/>
					<xsl:element name='img'>
						<xsl:attribute name='src'>graphics/
							<xsl:value-of select='//regionOfInterest[@ID=$imageRef]//observationMedia/value/reference/@value'/>
						</xsl:attribute>
					</xsl:element>
				</xsl:if>
			</xsl:when>
			<xsl:otherwise>
				<!-- Here is where the direct MultiMedia image referencing goes -->
				<xsl:if test='//observationMedia[@ID=$imageRef]/value[@mediaType="image/gif" or @mediaType="image/jpeg"]'>
					<br clear='all'/>
					<xsl:element name='img'>
						<xsl:attribute name='src'>graphics/
							<xsl:value-of select='//observationMedia[@ID=$imageRef]/value/reference/@value'/>
						</xsl:attribute>
					</xsl:element>
				</xsl:if>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<!-- 	Stylecode processing   
Supports Bold, Underline and Italics display



	<xsl:template match="//*[@styleCode]">
		<xsl:if test="@styleCode='Bold'">
			<xsl:element name='b'>
				<xsl:apply-templates/>
			</xsl:element>
		</xsl:if>

		<xsl:if test="@styleCode='Italics'">
			<xsl:element name='i'>
				<xsl:apply-templates/>
			</xsl:element>
		</xsl:if>

		<xsl:if test="@styleCode='Underline'">
			<xsl:element name='u'>
				<xsl:apply-templates/>
			</xsl:element>
		</xsl:if>

		<xsl:if test="contains(@styleCode,'Bold') and contains(@styleCode,'Italics') and not (contains(@styleCode, 'Underline'))">
			<xsl:element name='b'>
				<xsl:element name='i'>
					<xsl:apply-templates/>
				</xsl:element>
			</xsl:element>
		</xsl:if>

		<xsl:if test="contains(@styleCode,'Bold') and contains(@styleCode,'Underline') and not (contains(@styleCode, 'Italics'))">
			<xsl:element name='b'>
				<xsl:element name='u'>
					<xsl:apply-templates/>
				</xsl:element>
			</xsl:element>
		</xsl:if>

		<xsl:if test="contains(@styleCode,'Italics') and contains(@styleCode,'Underline') and not (contains(@styleCode, 'Bold'))">
			<xsl:element name='i'>
				<xsl:element name='u'>
					<xsl:apply-templates/>
				</xsl:element>
			</xsl:element>
		</xsl:if>

		<xsl:if test="contains(@styleCode,'Italics') and contains(@styleCode,'Underline') and contains(@styleCode, 'Bold')">
			<xsl:element name='b'>
				<xsl:element name='i'>
					<xsl:element name='u'>
						<xsl:apply-templates/>
					</xsl:element>
				</xsl:element>
			</xsl:element>
		</xsl:if>

	</xsl:template>

	- 	Superscript or Subscript   -
	<xsl:template match="sup">
		<xsl:element name='sup'>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="sub">
		<xsl:element name='sub'>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
-->
	<xsl:template match="b|i|u|font|sup|sub|a" mode="FormattedText">
		<xsl:element name="{local-name(.)}">
			<xsl:if test="local-name(.)='a'">
				<xsl:attribute name="href"><xsl:value-of select="@href"/></xsl:attribute>
			</xsl:if>
                <xsl:apply-templates mode="FormattedText"/>
        </xsl:element>
	</xsl:template>
	
	<!-- END Formatted text rendering -->

</xsl:stylesheet>