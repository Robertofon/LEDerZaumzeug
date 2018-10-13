namespace LEDerZaumzeug
{
    public enum SizeMode
    {
        /// <summary>
        /// Matrix is dynamicly extensible and can accept dimensions from the engine
        /// for example a software matrix on a monitor. Values for Width and Height
        /// shall not be determined but rather set indirectly by other outputs
        /// or by a global setting in the engine's config to then apply to all outputs.
        /// </summary>
        DynamicAssignable,

        /// <summary>
        /// Matrix is HW or SW but fixed in size. However the engine
        /// or the output can query the size from (i.e. a µC) and values for
        /// Width and Height can be auto determined.
        /// It is expected that With and Height are delivered.
        /// </summary>
        QueryableAutoDet,

        /// <summary>
        /// Matrix is hardware and fixed in size. Set the size statically
        /// as it is not determinable. Output is therefore fixed static const.
        /// Width and Height must be set to the acutal size.
        /// </summary>
        StaticSetting
    }
}