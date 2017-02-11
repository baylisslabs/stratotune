<?xml version="1.0"?>

<xsl:stylesheet version="1.0"         
        xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
        xmlns:svg="http://www.w3.org/2000/svg">
    
<xsl:output method="text" omit-xml-declaration="yes" />

<xsl:template match="/">             
    <xsl:for-each select="svg:svg/svg:rect[@id]"> 
            public static readonly RectangleF <xsl:value-of select="@id"/> = new RectangleF(
                 x:<xsl:value-of select="@x"/>
                ,y:<xsl:value-of select="@y"/>       
                ,width:<xsl:value-of select="@width"/>
                ,height:<xsl:value-of select="@height"/>);                           
    </xsl:for-each>
    <xsl:for-each select="svg:svg/svg:image[@id]"> 
            public static readonly RectangleF <xsl:value-of select="@id"/> = new RectangleF(
                 x:<xsl:value-of select="@x"/>
                ,y:<xsl:value-of select="@y"/>       
                ,width:<xsl:value-of select="@width"/>
                ,height:<xsl:value-of select="@height"/>);                           
    </xsl:for-each>        
</xsl:template>

</xsl:stylesheet>
