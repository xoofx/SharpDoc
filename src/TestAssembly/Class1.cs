namespace TestAssembly
{
    /// <summary>
    /// This class is the first class of this assembly
    /// </summary>
    public class Class1
    {
        /// <summary>
        /// The string constant
        /// </summary>
        public const string StringConstant = "StringConstant";

        /// <summary>
        /// The static instance of this class.
        /// </summary>
        public static readonly Class1 StaticInstance = new Class1();

        /// <summary>
        /// Initializes a new instance of the <see cref="Class1"/> class.
        /// </summary>
        public Class1()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Class1"/> class.
        /// </summary>
        /// <param name="intProperty">The int property.</param>
        public Class1(int intProperty)
        {
            IntProperty = intProperty;
        }

        /// <summary>
        /// Gets or sets the int property.
        /// </summary>
        /// <value>The int property.</value>
        public int IntProperty { get; set; }

        /// <summary>
        /// Gets or sets the static int property.
        /// </summary>
        /// <value>The static int property.</value>
        public static int StaticIntProperty { get; set; }

        /// <summary>
        /// First method this instance.
        /// </summary>
        public void Method1()
        {
        }

        /// <summary>
        /// Overloaded method of <see cref="Method1()"/>
        /// </summary>
        /// <param name="stringArg">The string argument.</param>
        /// <param name="intArg">The int argument.</param>
        public virtual void Method1(string stringArg, int intArg)
        {
        }
    }
}
