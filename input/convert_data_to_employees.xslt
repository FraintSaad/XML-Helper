<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  
  <xsl:output method="xml" indent="yes" encoding="UTF-8"/>
  <xsl:key name="employeeKey" match="item" use="concat(@name,'|',@surname)"/>

  <xsl:template match="/Pay">
    <Employees>
      <xsl:for-each select="//item[generate-id() = generate-id(key('employeeKey', concat(@name,'|',@surname))[1])]">
        <Employee name="{@name}" surname="{@surname}">
          <xsl:for-each select="key('employeeKey', concat(@name,'|',@surname))">
            <salary mount="{@mount}" amount="{@amount}"/>
          </xsl:for-each>
        </Employee>
      </xsl:for-each>
    </Employees>
  </xsl:template>

</xsl:stylesheet>
