<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:output method="xml" indent="yes" encoding="UTF-8"/>

  <xsl:template match="/">
    <Employees>
      <xsl:for-each select="Pay/item[generate-id() = generate-id(key('by-person', concat(@surname,'|',@name))[1])]">
        <Employee surname="{@surname}" name="{@name}">
          <xsl:for-each select="key('by-person', concat(@surname,'|',@name))">
            <salary mount="{@mount}" amount="{@amount}"/>
          </xsl:for-each>
        </Employee>
      </xsl:for-each>
    </Employees>
  </xsl:template>

  <xsl:key name="by-person" match="item" use="concat(@surname,'|',@name)"/>

</xsl:stylesheet>
