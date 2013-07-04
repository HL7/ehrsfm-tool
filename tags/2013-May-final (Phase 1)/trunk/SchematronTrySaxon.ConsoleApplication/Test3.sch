<?xml version="1.0" encoding="utf-8" ?>
<schema xmlns="http://purl.oclc.org/dsdl/schematron">

  <pattern name="Blaat">
    <rule context="//data">
      <report test="value='1'">One</report>
      <report test="value='2'">Two</report>
      <report test="value='3'">Three</report>
    </rule>
  </pattern>
</schema>