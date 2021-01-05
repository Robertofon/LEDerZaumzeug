namespace LEDerWand
{
    public sealed class WandConfig
    {
        public const string CFG_NAME = "WandConfig.json";

        /// <summary>
        /// Start in maximized.
        /// </summary>
        public bool StartFullScreen { get; set; }

        /// <summary>
        /// Spalten der LEDs.
        /// </summary>
        public int LedCols { get; set; }

        /// <summary>
        /// Reihen der LEDs
        /// </summary>
        public int LedRows { get; set; }

        /// <summary>
        /// Form der LEDs.
        /// </summary>
        public LedShape LedShape { get; set; }

        /// <summary>
        /// In pixel
        /// </summary>
        public int LedSpacing { get; set; }

        public int ListenPort { get; set; }

        public string ListenInterface { get; set; }

        public LedProto Protocol { get; set; }
    }
}