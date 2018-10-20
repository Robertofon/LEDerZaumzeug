namespace LEDerZaumzeug
{
    /// <summary>
    /// Standardarten, wie Pixel angeordnet sein könnten.
    /// SNH - ist Snakewise horizontal also mit horizontalen Strecken geschlängelt, beginnend bei XX
    /// SNV - ist Snakewise vertical also mit vertikalen Strecken geschlängelt, beginnend bei XX
    /// LNH - ist Linewise horizontal also mit horinzontalen Strecken immer eine Richtung, beginnend bei XX
    /// LNV - ist Linewise vertical also mit vertikalen Strecken immer eine Richtung, beginnend bei XX
    /// XX steht für
    /// TL - Top left
    /// TR - Top Right
    /// BL - Bottom Left
    /// BR - Bottom Right  
    /// </summary>
    public enum PixelArrangement
    {
        /// <summary>SnakeWeiseHorizontal, start Top left</summary>
        SNH_TL,
        SNH_TR,
        SNH_BL,
        SNH_BR,
        SNV_TL,
        SNV_TR,
        SNV_BL,
        SNV_BR,
        LNH_TL,
        LNH_TR,
        LNH_BL,
        LNH_BR,
        LNV_TL,
        LNV_TR,
        LNV_BL,
        LNV_BR,
    }

    /// <summary>
    /// Subpixel orderungen. Üblich sind nur RGB und BGR.
    /// Rest kann irgendwann hinzugebaut werden.
    /// </summary>
    public enum SubPixelOrder
    {
        RGB,
        //RBG,
        BGR,
        //BRG,
        //GRB,
        //GBR,        
    }
}
