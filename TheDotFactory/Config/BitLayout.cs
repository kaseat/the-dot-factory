namespace TheDotFactory.Config
{
    /// <summary>
    /// Bit Layout
    /// </summary>
    public enum BitLayout
    {
        /// <summary>
        /// '|' = 0x80,0x80,0x80  '_' = 0x00,0x00,0xFF
        /// </summary>
        RowMajor,
        /// <summary>
        /// '|' = 0xFF,0x00,0x00  '_' = 0x80,0x80,0x80
        /// </summary>
        ColumnMajor
    }
}
