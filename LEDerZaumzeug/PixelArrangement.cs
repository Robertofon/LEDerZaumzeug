namespace LEDerZaumzeug
{
    /// <summary>
    /// Standardarten, wie Pixel angeordnet sein könnten.
    /// SNH - ist Snakewise horizontal also mit horizontalen Strecken geschlängelt beginnend bei XX
    /// SNV - ist Snakewise vertical also mit vertikalen Strecken geschlängelt beginnend bei XX
    /// XX steht für
    /// TL - Top left
    /// TR - Top Right
    /// BL - Bottom Left
    /// BR - Bottom Right  
    /// </summary>
    public enum PixelArrangement
    {
        SNH_TL,
        SNH_TR,
        SNH_BL,
        SNH_BR,
        SNV_TL,
        SNV_TR,
        SNV_BL,
        SNV_BR,
    }
}
