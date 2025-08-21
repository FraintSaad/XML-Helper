<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:key name="by-person" match="item" use="concat(@surname,'|',@name)"/>

	<xsl:output method="xml" indent="yes"/>

	<xsl:template match="/Pay">
		<Employees>
			<xsl:for-each select="item[generate-id() = generate-id(key('by-person', concat(@surname,'|',@name))[1])]">
				<Employee surname="{@surname}" name="{@name}">
					<xsl:for-each select="key('by-person', concat(@surname,'|',@name))">
						<salary mount="{@mount}" amount="{@amount}"/>
					</xsl:for-each>
				</Employee>
			</xsl:for-each>
		</Employees>
	</xsl:template>
</xsl:stylesheet>
