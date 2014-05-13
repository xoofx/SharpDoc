namespace TestAssembly
{
    /// <summary>
    /// This is the documentation for a struct
    /// </summary>
    public class Struct1
    {
        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>The x.</value>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>The y.</value>
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the z.
        /// </summary>
        /// <value>The z.</value>
        public int Z { get; set; }

        // no doc on this field
        public int W { get; set; }

        public override string ToString()
        {
            return base.ToString() + "Overloaded";
        }
    }
}