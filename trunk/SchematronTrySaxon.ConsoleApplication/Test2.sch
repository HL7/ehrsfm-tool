<?xml version="1.0" encoding="utf-8" ?>
<schema xmlns="http://purl.oclc.org/dsdl/schematron" xmlns:max="http://www.umcg.nl/MAX">

  <!-- This should fix my problem ??!! -->
  <ns prefix="max" uri="http://www.umcg.nl/MAX"/>

  <pattern name="Header id format">
    <rule context="object[stereotype='Header' and type='Feature']">
      <!--<assert test="matches(/id, '^(CP|CPS|RI|TI)\.\d+')">id not formatted properly; expected "AA.#"</assert>-->
      <report test="starts-with(alias,'CPS.')">Care Provision Support</report>
      <report test="starts-with(alias,'CP.')">Care Provision</report>
      <report test="starts-with(alias,'RI.')">Record Infrastructure</report>
    </rule>
  </pattern>

  <!--
    All Conditional criteria must have the structure “IF condition, THEN consequence”.
    All criteria with the structure above are conditional criteria.
    A conditional criterion may have at most two function/criteria references (not contained within parentheses) pursuant the two rules above.  
    A non-conditional criterion may have one function/criteria reference (not contained within parentheses) pursuant the one rule above.
  -->
  <pattern name="">
    <rule context="object[stereotype='Criteria' and tag.Conditional='N']">
      <assert test="not(contains(notes, 'IF') and contains (notes, 'THEN'))">
        All criteria with the structure above are conditional criteria. <value-of select="name"/>
      </assert>
    </rule>
  </pattern>

  <pattern name="">
    <rule context="object[stereotype='Criteria' and tag.Conditional='Y']">
      <assert test="contains(notes, 'IF') and contains (notes, 'THEN')">
        Conditional Criteria <value-of select="name"/> must have the structure “IF condition, THEN consequence”.
      </assert>
    </rule>
  </pattern>

  <pattern name="">
    <rule context="object[stereotype='Header']">
      <assert test="number(substring-after(alias,'.'))">Header ID not formatted properly <value-of select="alias"/>.</assert>
    </rule>
  </pattern>

  <pattern name="Optionality">
    <rule context="object[stereotype='Criteria']">
      <assert test="tag.Optionality">Optionality must be specified for Criteria <value-of select="name"/>.</assert>
    </rule>
  </pattern>
</schema>
