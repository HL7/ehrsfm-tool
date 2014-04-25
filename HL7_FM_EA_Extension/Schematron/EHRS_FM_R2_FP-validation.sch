<?xml version="1.0" encoding="UTF-8"?>
<schema xmlns="http://purl.oclc.org/dsdl/schematron">
    <let name="capital-letters" value="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'"/>
    <let name="lowercase-letters" value="'abcdefghijklmnopqrstuvwxyz'"/>
    <let name="numbers" value="'0123456789'"/>
    <let name="special-characters" value="'.,/?[]\{}|`~!@#$%^*()-_=+;: &amp;'"/>
    <let name="quote" value="'&quot;'"/>
    <let name="apostrophe" value='"&apos;"'/>
    <let name="printing-characters" value="concat($capital-letters, $lowercase-letters, $numbers, $special-characters, $quote, $apostrophe)"/>
    
    <pattern id="identity-info">
        <rule context="object[stereotype/text()='HL7-FM-Profile']">
            <let name="id" value="../id"/>
            <assert test="tag[@name='Description']" diagnostics="FPID01"/>
            
            <assert test="tag[@name='Version']" diagnostics="FPID02"/>
            <assert test="tag[@name='IssuanceDate']" diagnostics="FPID03"/>
            <assert test="tag[@name='NormativeLanguage']" diagnostics="FPID04"/>
            <assert test="tag[@name='SourceVersion']" diagnostics="FMTC02"/>
            <assert test="tag[@name='SourceVersionDate']" diagnostics="FMTC03"/>
        </rule>
    </pattern>
    
    <pattern id="prioritization">
        <rule context="object[stereotype/text()='Function'| stereotype/text()='Criteria']">
            <assert test="tag[@name='Priority']" diagnostics="FMPR01"/>
            <assert test="tag[@name='Priority']/[text()='en' | text()='ef' | text()='O']" diagnostics="FMPR01"/>
        </rule>
        <rule context="object[stereotype/text()='Function'| stereotype/text()='Criteria']/tag[@name='Priority']">
            <assert test="text()='ef' and ../tag[@name='Timeframe']" diagnostics="FMPR02"/>
        </rule>
    </pattern>
    
    <pattern id="funciton-content">
        <rule context="object[stereotype/text()='Function']">
            <let name="statement" value="substring-before(substring-after(notes,'$ST$'), '$DE$')"/>
            <let name="stmt-lgth" value="string-length($statement)"/>
            <let name="description" value="substring-before(substring-after(notes,'$DE$'), '$EX$')"/>
            <let name="desc-lgth" value="string-length($description)"/>

            <assert test="id" diagnostics="FMST17"/>
            <assert test="name" diagnostics="FMST26"/>

            <assert test="$stmt-lgth > 0" diagnostics="FMST27"/>
            <assert test="$desc-lgth > 0" diagnostics="FMST28"/>
            
            <assert test="count(../object/alias[text()=$thing-id])=1" diagnostics="FMST29"><value-of select="$id"/></assert>
        </rule>
    </pattern>
        
    <diagnostics>
        <diagnostic id="FPID01" xml:lang="en">You have not included a description of the Functional Profile.</diagnostic>
        <diagnostic id="FPID02" xml:lang="en">You have not included the version of the Functional Profile.</diagnostic>
        <diagnostic id="FPID03" xml:lang="en">You have not included the issuance date of the Functional Profile.</diagnostic>
        <diagnostic id="FPID04" xml:lang="en">IF you have made changes to the normative language you shall include a description of the changes or differences in that.</diagnostic>
        <diagnostic id="FMTC02" xml:lang="en">You have not included the version of the Functional Model from which the Functional Profile is derived.</diagnostic>
        <diagnostic id="FMTC03" xml:lang="en">You have not included the date of the Functional Model from which the Functional Profile is derived.</diagnostic>
        <diagnostic id="FMPR01" xml:lang="en">You have not included the prioritization of this function.</diagnostic>
        <diagnostic id="FMPR02" xml:lang="en">You did not inlcude a timeframe for when the function is required te be implemented.</diagnostic>
        <diagnostic id="FMST33" xml:lang="en">The timeframe you have defined is not in the right format.</diagnostic>
        <diagnostic id="FMPR03" xml:lang="en">You did not include a definition of Essential Future.</diagnostic>
        <diagnostic id="FMWP01" xml:lang="en">You did not assign the applicability of the function.</diagnostic>
        <diagnostic id="FMID05" xml:lang="en">You did not include a country code in the function ID.</diagnostic>
        <diagnostic id="FMST30" xml:lang="en">The changes you made to the functions name is not allowed.</diagnostic>
        <diagnostic id="FMST31" xml:lang="en">The changes you made to the functions statement is not allowed.</diagnostic>
        <diagnostic id="FMST32" xml:lang="en">The FP you are defining is not a Realm FP so you cannot change the name and/or statement.</diagnostic>
        <diagnostic id="FMID06" xml:lang="en">The country code you used is not conform the ISO 3166-1.</diagnostic>
        <diagnostic id="FMST15" xml:lang="en">The function you created is not a child function. </diagnostic>
        <diagnostic id="FMNO02" xml:lang="en">Function %% has no SHALL criterion.</diagnostic>
        <diagnostic id="FMST16" xml:lang="en">The function you atempting to split is a parent function. Splitting is not allowed.</diagnostic>
        <diagnostic id="FMST17" xml:lang="en">The function is missing an ID.</diagnostic>
        <diagnostic id="FMST26" xml:lang="en">The function is missing a Name</diagnostic>
        <diagnostic id="FMST27" xml:lang="en">The function is missing a Statement</diagnostic>
        <diagnostic id="FMST28" xml:lang="en">The function is missing a Description</diagnostic>
        <diagnostic id="FMST29" xml:lang="en">The function is missing conformance criteria</diagnostic>
        <diagnostic id="FMST18" xml:lang="en">The function is missing a 'F' in the Tagged Value.</diagnostic>
        <diagnostic id="FMST19" xml:lang="en">The action verb you are using in the conformance criterion is not in the Glossary Action verbs.</diagnostic>
        <diagnostic id="FMST20" xml:lang="en">The action verb you are using in the conformance criterion is not in the Glossary Action verbs.</diagnostic>
        <diagnostic id="FMTC08" xml:lang="en">Not every term and definition in the Glossary is assigned a status.</diagnostic>
        <diagnostic id="FMNO03" xml:lang="en">There must be at least one SHALL criterion of mandatory criterion for this function</diagnostic>
        <diagnostic id="FMST21" xml:lang="en">The functions in the Overarching Section are not (all) in your Profile.</diagnostic>
        <diagnostic id="FMPR04" xml:lang="en">Not all the functions in the Overarching Section has the priority Essential Now.</diagnostic>
        <diagnostic id="FMST22" xml:lang="en">Not all the criterion listed in the parent function are in your Profile.</diagnostic>
        <diagnostic id="FMNO04" xml:lang="en">In the inherited function you changed the mandatory conformance statement. This is not allowed.</diagnostic>
        <diagnostic id="FMST23" xml:lang="en">The change of the optionallity is not allowed in the Functional domain Profile.</diagnostic>
        <diagnostic id="FMST24" xml:lang="en">The change of the optionallity is not allowed in the Functional domain Profile.</diagnostic>
        <diagnostic id="FMST99" xml:lang="en">In the criterion the concept 'standard(s)-based' is used. Please replace this with specific standards and/or specifications.</diagnostic>
        <diagnostic id="FMST25" xml:lang="en">You did not inherite the 'dependent shall' criterion that is originally in the function.</diagnostic>
        <diagnostic id="FMST98" xml:lang="en">You didn't use the correct test for this dependent shall criteria.</diagnostic>
        <diagnostic id="FMST06" xml:lang="en">You didn't replicate the dependent shall criteria for this function.</diagnostic>
        <diagnostic id="FMID09" xml:lang="en">The specific scope of practice is missing.</diagnostic>
        <diagnostic id="FMID10" xml:lang="en">Please provide the reason(s) why the dependencies are not applicable in the Functional Profile.</diagnostic>
        <diagnostic id="FMST07" xml:lang="en">The reference to another function in the profile is incomplete. It shall be done as "X.n.n (Name)"</diagnostic>
        <diagnostic id="FMST08" xml:lang="en">The reference to another criterion in the profile is missing or incomplete.</diagnostic>
        <diagnostic id="FMST09" xml:lang="en">There is a mandatory criterion missing (a 'shall' criterion).</diagnostic>
        <diagnostic id="FMST10" xml:lang="en">This 'shall' criterion in the Functional Model is missing in the Functional profile.</diagnostic>
        <diagnostic id="FMST11" xml:lang="en">The original 'shall' criteria are missing.</diagnostic>
        <diagnostic id="FMST12" xml:lang="en">For this function a mandatory criterion is missing.</diagnostic>
        <diagnostic id="FMST13" xml:lang="en">Not all the shall criteria are inherited.</diagnostic>
        <diagnostic id="FMTC06" xml:lang="en">The traceability to the original criterion is missing.</diagnostic>
        <diagnostic id="FMTC07" xml:lang="en">The status of the criterion is not included (changes or unchanged).</diagnostic>
        <diagnostic id="FMST14" xml:lang="en">Not all relationships for this function is included.</diagnostic>
        <diagnostic id="FMID11" xml:lang="en">If the functional profile is registrated, then describe this in your profile.</diagnostic>
        <diagnostic id="FMID12" xml:lang="en">If the functional profile is balloted, then describe this in your profile.</diagnostic>
    </diagnostics>
</schema>