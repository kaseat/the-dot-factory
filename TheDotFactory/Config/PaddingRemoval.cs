namespace TheDotFactory.Config
{
    /// <summary>
    /// Padding removal option.
    /// </summary>
    public enum PaddingRemoval
    {
        /// <summary>
        /// No padding removal.
        /// </summary>
        None,
        /// <summary>
        /// Remove padding as much as possible, per bitmap.
        /// </summary>
        Tighest,
        /// <summary>
        /// Remove padding as much as the bitmap with least padding.
        /// </summary>
        Fixed
    }
}
