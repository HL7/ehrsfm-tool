<?xml version="1.0" encoding="UTF-8"?>
<schema xmlns="http://purl.oclc.org/dsdl/schematron">
    <let name="capital-letters" value="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'"/>
    <let name="lowercase-letters" value="'abcdefghijklmnopqrstuvwxyz'"/>
    <let name="numbers" value="'0123456789'"/>
    <let name="special-characters" value="'.,/?[]\{}|`~!@#$%^*()-_=+;: &amp;'"/>
    <let name="quote" value="'&quot;'"/>
    <let name="apostrophe" value='"&apos;"'/>
    <let name="printing-characters" value="concat($capital-letters, $lowercase-letters, $numbers, $special-characters, $quote, $apostrophe)"/>
    
    <pattern id="header">
        <rule context="object">
        </rule>
    </pattern>
    
    <pattern id="core-structure">
        <rule context="object/type">
            <let name="id" value="../id"/>
            <assert test="./text()='Package' or ./text()='Feature' or ./text()='Requirement'" diagnostics="FMST001"><value-of select="$id"/></assert>
        </rule>
        <rule context="object[type/text()='Feature']">
            <let name="id" value="id"/>
            <assert test="stereotype[text()='Header' or text()='Function']" diagnostics="FMST001"><value-of select="$id"/></assert>
        </rule>
        <rule context="object[type/text()='Requirement']">
            <let name="id" value="id"/>
            <assert test="stereotype[text()='Criteria']" diagnostics="FMST001"><value-of select="$id"/></assert>
        </rule>
    </pattern>
    
    <pattern id="header-components">
        <rule context="object[stereotype/text()='Header']">
            <let name="id" value="id"/>
            <let name="thing-id" value="alias"/>
            <let name="parent-id" value="parentId"/>
            <let name="parent-alias" value="../object[id=$parent-id]/alias"/>
            <let name="statement" value="substring-before(substring-after(notes,'$ST$'), '$DE$')"/>
            <let name="stmt-lgth" value="string-length($statement)"/>
            <let name="description" value="substring-before(substring-after(notes,'$DE$'), '$EX$')"/>
            <let name="desc-lgth" value="string-length($description)"/>
            
            <assert test="count(../object/alias[text()=$thing-id])=1" diagnostics="FMHC001"><value-of select="$id"/></assert>
            
            <assert test="name" diagnostics="FMHC002"><value-of select="$id"/></assert>
            <let name="name" value="name"/>
            <assert test="count(../object/name[text()=$name])=1" diagnostics="FMHC003"><value-of select="$id"/></assert>

            <assert test="starts-with($name,$thing-id)" diagnostics="FMUP01"><value-of select="$id"/></assert>
            
            <assert test="$stmt-lgth > 0" diagnostics="FMHC004"><value-of select="$id"/></assert>
            <assert test="substring($statement, $stmt-lgth, 1) = '.'" diagnostics="FMHC005"><value-of select="$id"/></assert>
            <assert test="$desc-lgth > 0" diagnostics="FMHC006"><value-of select="$id"/></assert>
            <assert test="substring($description, $desc-lgth, 1) = '.'" diagnostics="FMHC007"><value-of select="$id"/></assert>
            
            <assert test="count(../object[stereotype='Function' and parentId=$id]) > 0" diagnostics="FMHC009"><value-of select="$id"/></assert>

            <assert test="count(../object[stereotype/text()='Criteria' and parentId=$id]) = 0" diagnostics="FMHC010"><value-of select="$id"/></assert>
            
            <let name="start-string" value="concat(substring($statement, 1, 25), '...')"/>
            <assert test="contains($capital-letters, substring($statement,1,1))" diagnostics="FMTSR02"><value-of select="$id"/></assert>
            <let name="start-string2" value="concat(substring($description, 1, 25), '...')"/>
            <assert test="contains($capital-letters, substring($description,1,1))" diagnostics="FMTSR08"><value-of select="$id"/></assert>
            
            <let name="non-printing-text1" value="translate($statement, $printing-characters, '')"/>
            <assert test="string-length($non-printing-text1)=0" diagnostics="FMTSR04"><value-of select="$id"/></assert>
            <let name="non-printing-text2" value="translate($description, $printing-characters, '')"/>
            <assert test="string-length($non-printing-text2)=0" diagnostics="FMTSR09"><value-of select="$id"/></assert>
            
            <let name="local-alias" value="substring-after($thing-id, $parent-alias)"/>
            <assert test="starts-with($local-alias, '.')" diagnostics="FMFR002"><value-of select="$id"/></assert>
            <assert test="string-length(translate(substring($local-alias, 2), $numbers, '')) = 0" diagnostics="FMFR002"><value-of select="$id"/></assert>
        </rule>
    </pattern>
    
    <pattern id="function-components">
        <rule context="object[stereotype/text()='Function']">
            <let name="id" value="id"/>
            <let name="thing-id" value="alias"/>
            <let name="parent-id" value="parentId"/>
            <let name="parent-alias" value="../object[id=$parent-id]/alias"/>
            <let name="statement" value="substring-before(substring-after(notes,'$ST$'), '$DE$')"/>
            <let name="stmt-lgth" value="string-length($statement)"/>
            <let name="description" value="substring-before(substring-after(notes,'$DE$'), '$EX$')"/>
            <let name="desc-lgth" value="string-length($description)"/>
            
            <assert test="count(../object/alias[text()=$thing-id])=1" diagnostics="FMFLC02"><value-of select="$id"/></assert>
            <assert test="starts-with(alias/text(), $parent-alias)" diagnostics="FMFLC03"><value-of select="$id"/></assert>
            
            <let name="name" value="name"/>
            <let name="name-end" value="substring($name, string-length($name), 1)"/>

          <assert test="starts-with($name,$thing-id)" diagnostics="FMUP02"><value-of select="$id"/></assert>

          <assert test="count(../object/name[text()=$name])=1" diagnostics="FMFLC06"><value-of select="$id"/></assert>
            <assert test="starts-with(name/text(), $thing-id)" diagnostics="FMFLC07"><value-of select="$id"/></assert>
            <assert test="not($name-end='.')" diagnostics="FMFLC08"><value-of select="$id"/></assert>
            <assert test="$stmt-lgth > 0" diagnostics="FMFLC09"><value-of select="$id"/></assert>
            <assert test="substring($statement, $stmt-lgth, 1) = '.'" diagnostics="FMFLC10"><value-of select="$id"/></assert>
            <assert test="$desc-lgth > 0" diagnostics="FMFLC11"><value-of select="$id"/></assert>
            <assert test="substring($description, $desc-lgth, 1) = '.'" diagnostics="FMFLC12"><value-of select="$id"/></assert>
            
            <assert test="count(../object[stereotype/text()='Criteria' and parentId=$id]) > 0" diagnostics="FMFLC13"><value-of select="$id"/></assert>
            
            <let name="start-string" value="concat(substring($statement, 1, 25), '...')"/>
            <assert test="contains($capital-letters, substring($statement,1,1))" diagnostics="FMTSR02"><value-of select="$id"/></assert>
            <let name="start-string2" value="concat(substring($description, 1, 25), '...')"/>
            <assert test="contains($capital-letters, substring($description,1,1))" diagnostics="FMTSR08"><value-of select="$id"/></assert>
            
            <let name="non-printing-text1" value="translate($statement, $printing-characters, '')"/>
            <assert test="string-length($non-printing-text1)=0" diagnostics="FMTSR04"><value-of select="$id"/></assert>
            <let name="non-printing-text2" value="translate($description, $printing-characters, '')"/>
            <assert test="string-length($non-printing-text2)=0" diagnostics="FMTSR09"><value-of select="$id"/></assert>

            <let name="local-alias" value="substring-after($thing-id, $parent-alias)"/>
            <assert test="starts-with($local-alias, '.')" diagnostics="FMFR003"><value-of select="$id"/></assert>
            <assert test="string-length(translate(substring($local-alias, 2), $numbers, '')) = 0" diagnostics="FMFR003"><value-of select="$id"/></assert>
            
            <report test="id/text() = alias/text()" diagnostics="FMID001"><value-of select="$id"/></report>
        </rule>
    </pattern>
   
    <pattern id="criteria-components">
        <rule context="object[stereotype/text()='Criteria']">
            <let name="id" value="id"/>
            <let name="thing-id" value="name"/>
            <let name="parent-id" value="parentId"/>
            <let name="parent-alias" value="../object[id=$parent-id]/alias"/>
          
            <assert test="count(../object[name=$thing-id])=1" diagnostics="FMFLC22"><value-of select="$id"/></assert>
            
            <assert test="notes" diagnostics="FMFLC15"><value-of select="$id"/></assert>
            <assert test="string-length(normalize-space(notes/text())) > 0" diagnostics="FMFLC15"><value-of select="$id"/></assert>
            
            <let name="start-string" value="substring(notes/text(), 1, 15)"/>
            <assert test="contains($capital-letters, substring(notes/text(), 1,1))" diagnostics="FMTSR02"><value-of select="$id"/></assert>
            <assert test="notes/text() = normalize-space(notes/text())" diagnostics="FMTSR03"><value-of select="$id"/></assert>

            <let name="has-reference" value="contains(notes/text(), 'conform to function')"/>
            <let name="text-with-ref" value="substring-after(notes/text(), 'conform to function ')"/>
            <let name="ref-id" value="substring-before($text-with-ref, ' ')"/>
            
            <report test="$has-reference and count(../object[alias/text()=$ref-id]) = 0" diagnostics="FMFLC16"><value-of select="id"/></report>
            
            <report test="not($has-reference) and contains(notes/text(), '#')" diagnostics="FMFLC20"><value-of select="$id"/></report>
            
            <!-- Determine if the current Criteria is conditional -->
            <let name="is-conditional" value="tag[@name='Conditional' and @value='Y']"/>
            
            <report test="$is-conditional and not(starts-with(notes/text(), 'IF'))" diagnostics="FMCC001"><value-of select="$id"/></report>
            <report test="not($is-conditional) and starts-with(notes/text(), 'IF')" diagnostics="FMCC002"><value-of select="$id"/></report>
            
            <!-- Determine if the current Criteria is a dependent -->
            <let name="is-dependent" value="tag[@name='Dependent' and @value='Y']"/>
            <report test="$is-dependent and not(contains(notes, 'according to scope of practice') or contains(notes, 'organizational policy') or contains(notes, 'jurisdictional law'))" diagnostics="FMDC001"><value-of select="$id"/></report>
            <report test="not($is-dependent) and (contains(notes, 'according to scope of practice') or contains(notes, 'organizational policy') or contains(notes, 'jurisdictional law'))" diagnostics="FMDC002"><value-of select="$id"/></report>            
            <!--<report test="$is-dependent and not(contains(notes, 'according to scope of practice, organizational policy and/or jurisdictional law'))" diagnostics="FMDC001"><value-of select="$id"/></report>-->
            <!--<report test="not($is-dependent) and contains(notes, 'according to scope of practice, organizational policy and/or jurisdictional law')" diagnostics="FMDC002"><value-of select="$id"/></report>-->
            <!--<report test="$is-dependent and tag[@name='Optionality' and not(@value='SHALL')]" diagnostics="FMDC003"><value-of select="$id"/></report>-->
            
            <let name="option-value" value="tag[@name='Optionality']/@value"/>
            <let name="text-without-optionality" value="concat(substring-before(notes/text(), $option-value), substring-after(notes/text(), $option-value))"/>
            <assert test="tag[@name='Optionality' and (@value='SHOULD' or @value='SHALL' or @value='MAY')]" diagnostics="FMSVR01"><value-of select="$id"/></assert>
            <assert test="contains(notes/text(), 'SHOULD') or contains(notes/text(), 'SHALL') or contains(notes/text(), 'MAY')" diagnostics="FMSVR07"><value-of select="$id"/></assert>
            <report test="contains($text-without-optionality, 'SHOULD') or contains($text-without-optionality, 'SHALL') or contains($text-without-optionality, 'MAY')" diagnostics="FMSVR07"><value-of select="$id"/></report>            
            <assert test="contains(notes/text(), $option-value)" diagnostics="FMSVR08"><value-of select="$id"/></assert>
            
            <let name="non-printing-text1" value="translate(notes/text(), $printing-characters, '')"/>
            <assert test="string-length($non-printing-text1)=0" diagnostics="FMTSR04"><value-of select="$id"/></assert>
            
            <let name="local-alias" value="substring-after($thing-id, $parent-alias)"/>
            <assert test="starts-with($thing-id, $parent-alias)" diagnostics="FMFR006"><value-of select="$id"/></assert>
            <assert test="starts-with($local-alias, '#')" diagnostics="FMFR007"><value-of select="$id"/></assert>
            <assert test="string-length(translate(substring($local-alias, 2), $numbers, '')) = 0" diagnostics="FMFR007"><value-of select="$id"/></assert>
            
            <report test="id/text() = name/text()" diagnostics="FMID002"><value-of select="$id"/></report>
        </rule>
    </pattern>
    
    <diagnostics>
        <diagnostic id="FMST001" xml:lang="en">Error wrong type, must be one of Package, Feature, or Requirement</diagnostic>
        <diagnostic id="FMST002" xml:lang="en">This item uses another title than allowed.</diagnostic>
        <diagnostic id="FMHC001" xml:lang="en">The Header should have a unique ID.</diagnostic>
        <diagnostic id="FMHC002" xml:lang="en">The Header should have a name.</diagnostic>
        <diagnostic id="FMHC003" xml:lang="en">The name of the header <value-of select="$name"/> is not unique.</diagnostic>
        <diagnostic id="FMHC004" xml:lang="en">Header must have a statement about its purpose.</diagnostic>
        <diagnostic id="FMHC005" xml:lang="en">Header purpose statement must end in a period.</diagnostic>
        <diagnostic id="FMHC006" xml:lang="en">Header must have a description.</diagnostic>
        <diagnostic id="FMHC007" xml:lang="en">Header description must end in a period.</diagnostic>
        <diagnostic id="FMHC008" xml:lang="en">Conformance criteria must apply to all descendants.</diagnostic>
        <diagnostic id="FMHC009" xml:lang="en">A Header must have at least one descendant function.</diagnostic>
        <diagnostic id="FMHC010" xml:lang="en">A Header must not have criteria.</diagnostic>
        <diagnostic id="FMFLC01" xml:lang="en">The Function misses one or more components.</diagnostic>
        <diagnostic id="FMFLC02" xml:lang="en">The Function should have a unique ID.</diagnostic>
        <diagnostic id="FMFLC03" xml:lang="en">The Function ID is not consistent with the section belongs to.</diagnostic>
        <diagnostic id="FMFLC04" xml:lang="en">The Function ID is not consistent with the position in the hierarchy.</diagnostic>
        <diagnostic id="FMFLC05" xml:lang="en">The line item must have an indication of being a header, parent or leaf function or conformance criteria.</diagnostic>
        <diagnostic id="FMFLC06" xml:lang="en">The name of the function <value-of select="$name"/> is not unique.</diagnostic>
        <diagnostic id="FMFLC07" xml:lang="en">Function name must be accompanied by the Function ID.</diagnostic>
        <diagnostic id="FMFLC08" xml:lang="en">Function names must not end with a period.</diagnostic>
        <diagnostic id="FMFLC09" xml:lang="en">Function should have a statement about its purpose</diagnostic>
        <diagnostic id="FMFLC10" xml:lang="en">Function statement should end with a period.</diagnostic>
        <diagnostic id="FMFLC11" xml:lang="en">Function should have a description.</diagnostic>
        <diagnostic id="FMFLC12" xml:lang="en">Function description should end with a period.</diagnostic>
        <diagnostic id="FMFLC13" xml:lang="en">Function must have one or more conformance criteria.</diagnostic>
        <diagnostic id="FMFLC14" xml:lang="en">All function references must have a valid function ID format structure.</diagnostic>
        <diagnostic id="FMFLC15" xml:lang="en">Criterion must have a textual description.</diagnostic>
        <diagnostic id="FMFLC16" xml:lang="en">Criterion references a non-existing function or criterion.</diagnostic>
        <diagnostic id="FMFLC17" xml:lang="en">Each rule can only refer to one conditional clause at a time.</diagnostic>
        <diagnostic id="FMFLC18" xml:lang="en">**see above**</diagnostic>
        <diagnostic id="FMFLC19" xml:lang="en">Unidentified function reference, please check.</diagnostic>
        <diagnostic id="FMFLC20" xml:lang="en">Unidentified function reference, please check.</diagnostic>
        <diagnostic id="FMFLC21" xml:lang="en">Statement contains a duplicate reference.</diagnostic>
        <diagnostic id="FMCC001" xml:lang="en">The Conditional Criteria is not structured correctly</diagnostic>
        <diagnostic id="FMCC002" xml:lang="en">The Criteria is structured as conditional but conditional is not indicated.</diagnostic>
        <diagnostic id="FMCC003" xml:lang="en">The Conditional Criteria can only contain 1 function/criteria reference.</diagnostic>
        <diagnostic id="FMCC004" xml:lang="en">The non-conditional Criteria can only contain 1 function/criteria reference.</diagnostic>
        <diagnostic id="FMDC001" xml:lang="en">The dependent criteria is missing the required phrase 'according to scope of practice, organizational policy and/or jurisdictional law'.</diagnostic>
        <diagnostic id="FMDC002" xml:lang="en">The dependent criteria phrase is present in when the criteria is not designated as dependent.</diagnostic>
        <diagnostic id="FMDC003" xml:lang="en">Dependent Criteria should have an optionality of 'SHALL'.</diagnostic>
        <diagnostic id="FMDC004" xml:lang="en">Any of the words jurisdiction, organization, policy or scope is being used outside of the required phrase.</diagnostic>
        <diagnostic id="FMHS001" xml:lang="en">no message</diagnostic>
        <diagnostic id="FMSVR01" xml:lang="en">Statement does not include any of the required keywords.</diagnostic>
        <diagnostic id="FMSVR02" xml:lang="en">**</diagnostic>
        <diagnostic id="FMSVR03" xml:lang="en">**see SVR01**</diagnostic>
        <diagnostic id="FMSVR04" xml:lang="en">**</diagnostic>
        <diagnostic id="FMSVR05" xml:lang="en">**see SVR01**</diagnostic>
        <diagnostic id="FMSVR06" xml:lang="en">**</diagnostic>
        <diagnostic id="FMSVR07" xml:lang="en">Criteria can only have one occurrence of SHALL, SHOULD or MAY unless in parentheses.</diagnostic>
        <diagnostic id="FMSVR08" xml:lang="en">Optionality attribute does not match the optionality in the text</diagnostic>
        <diagnostic id="FMFF001" xml:lang="en">no message</diagnostic>
        <diagnostic id="FMFF002" xml:lang="en">no message</diagnostic>
        <diagnostic id="FMFF003" xml:lang="en">no message</diagnostic>
        <diagnostic id="FMFF004" xml:lang="en">no message</diagnostic>
        <diagnostic id="FMFF005" xml:lang="en">no message</diagnostic>
        <diagnostic id="FMTSR01" xml:lang="en">Spelling error</diagnostic>
        <diagnostic id="FMTSR02" xml:lang="en">The text (<value-of select="$start-string"/>...) does not start with a capital letter.</diagnostic>
        <diagnostic id="FMTSR03" xml:lang="en">The text is not single spaced (there are multiple spaces).</diagnostic>
        <diagnostic id="FMTSR04" xml:lang="en">The text contains non-printable characters [<value-of select="$non-printing-text1"/>].</diagnostic>
        <diagnostic id="FMTSR05" xml:lang="en">**see TSR03**</diagnostic>
        <diagnostic id="FMTSR06" xml:lang="en">The text uses soft-returns</diagnostic>
        <diagnostic id="FMTSR07" xml:lang="en">Different versions of this word are considered synonyms.</diagnostic>
        <diagnostic id="FMTSR08" xml:lang="en">The text (<value-of select="$start-string2"/>...) does not start with a capital letter.</diagnostic>
        <diagnostic id="FMTSR09" xml:lang="en">The text contains non-printable characters [<value-of select="$non-printing-text2"/>].</diagnostic>
        <diagnostic id="FMFR001" xml:lang="en">The order of the letters and the numbers is not consistent.</diagnostic>
        <diagnostic id="FMFR002" xml:lang="en">The Header ID (<value-of select="$thing-id"/>) does not have the proper format: {Parent-ID}.{local-number}.</diagnostic>
        <diagnostic id="FMFR003" xml:lang="en">The Function ID (<value-of select="$thing-id"/>) does not have the proper format: {Parent-ID}.{local-number}.</diagnostic>
        <diagnostic id="FMFR004" xml:lang="en">**see FR003**</diagnostic>
        <diagnostic id="FMFR005" xml:lang="en">**see FR003**</diagnostic>
        <diagnostic id="FMFR006" xml:lang="en">The Criteria ID must start with the ID of the parent Function.</diagnostic>
        <diagnostic id="FMFR007" xml:lang="en">The Criteria ID can only have '#' and digits following the Parent ID and cannot contain [<value-of select="$local-alias"/>].</diagnostic>
        <diagnostic id="FMID001" xml:lang="en">The Function ID is the same as the visible ID.</diagnostic>
        <diagnostic id="FMID002" xml:lang="en">The Conformance Criteria ID is the same as the visible ID.</diagnostic>
        <diagnostic id="FMID003" xml:lang="en">**</diagnostic>
        <diagnostic id="FMNO001" xml:lang="en">Headers are not in numeric order.</diagnostic>
        <diagnostic id="FMNO002" xml:lang="en">Functions are not in numeric order.</diagnostic>
        <diagnostic id="FMNO003" xml:lang="en">Conformance Criteria are not in numeric order.</diagnostic>
        <diagnostic id="FMAN01" xml:lang="en">**</diagnostic>
        <diagnostic id="FMEA01" xml:lang="en">The field contains the maximum number of characters, check for truncation and text loss.</diagnostic>
        <diagnostic id="FMTS01" xml:lang="en">This header, function or conformance criteria cannot be traced.</diagnostic>

        <diagnostic id="FMFLC22" xml:lang="en">The Criteria should have a unique ID.</diagnostic>
        <diagnostic id="FMUP01" xml:lang="en">Header ID in Name and Alias are different.</diagnostic>
        <diagnostic id="FMUP02" xml:lang="en">Function ID in Name and Alias are different.</diagnostic>
    </diagnostics>
</schema>