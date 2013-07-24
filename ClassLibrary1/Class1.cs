using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <webdoc>Documentation</webdoc>
namespace ClassLibrary1
{

    /// <summary>
    /// Class that extends the ExtendedClass class.
    /// </summary>
    public static class Extension
    {
        /// <summary>
        /// Function that allow to get the value of an ExtendedClass.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="j">one float</param>
        /// <param name="c">one char</param>
        /// <returns> the value of the ClassTest</returns>
        public static int extensionFunction<T>(this ExtendedClass<T> source, float j, char c) { return source.value; }
    }

    /// <summary>
    /// Class extended by the Extension class.
    /// </summary>
    public  class ExtendedClass<T>
    {
        /// <summary>
        /// The value returned by the extension function
        /// </summary>
        public int value;

    }

    /// <summary>
    /// Child class of the extended class.
    /// </summary>
    public  class ExtendedClassChild<T> : ExtendedClass<T>
    {
    }








    /// <summary>
    /// A genericClass
    /// </summary>
    public class GenericClass<T>
    {
        /// <summary>
        /// A genericFunction
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <returns></returns>
        public int genericFunction<T, U>()
            where T : GenericClassChild<T>, IEnumerable
            where U : IComparable<U>, new()
        {
            return 2;
        }

        /// <summary>
        /// An other generic function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public float genericFunction<T>() where T : ExtendedClass<T>, IEnumerable, ICloneable, IConvertible, new()
        {
            return 5.0f;
        }
    }

    /// <summary>
    /// A child class of genericClass
    /// </summary>
    public class GenericClassChild<T> : GenericClass<T>
    {
    }
















    /// <summary>
    /// The Commented class
    /// </summary>
    /// <remarks> Commented class remarks</remarks>
    /// <webdoc>Licensing</webdoc>
     public class CommentedClass
    {
         /// <summary>
         /// A commented int field
         /// </summary>
         /// <remarks>The commented int field remarks</remarks>
         /// <webdoc>UNKNOWN</webdoc>
         public int commentedField;

         /// <summary>
         /// A commented float property
         /// </summary>
         /// <remarks> The commented float property remarks</remarks>
         public virtual float CommentedProperty {get; set;}

         /// <summary>
         /// An other commented float property
         /// </summary>
         /// <remarks> The 2nd commented float property remarks</remarks>
         public float CommentedProperty2 { get; set; }

        /// <summary>
        /// Commented empty constructor
        /// </summary>
        /// <webdoc>Assets+loading</webdoc>
        public CommentedClass()
        {
        }

         /// <summary>
         /// Commented (int,IEnumerable) constructor
         /// </summary>
         /// <param name="i">commented iiii</param>
         /// <param name="k">commented kkkkkkk</param>
         /// <remarks>commented remarks</remarks>
         /// <webdoc>Paradox+Roadmap</webdoc>
        public CommentedClass(int i, IEnumerable k)
        {
        }


        /// <summary>
        /// The commented function.
        /// </summary>
        /// <typeparam name="T">The commented typeParameter</typeparam>
        /// <param name="i">The commented i parameter.</param>
        /// <param name="j">The commented j parameter.</param>
        /// <param name="instance">The commented T instance.</param>
        /// <returns>The commented returned value</returns>
        /// <remarks>The remarks</remarks>
        /// <webdoc>Build+Asset+Pipeline</webdoc>
        /// <obsolete>This function is commented as 'obsolete'</obsolete>
        public virtual int CommentedFunction<T>(int i, float j, T instance) { return 0; }

        ///// <summary>
        ///// A commented (int|CommentedClass) delegate
        ///// </summary>
        ///// <param name="instance">The delegate instance parameter</param>
        ///// <returns> The integer returned by the delegate </returns>
        //public delegate int CommentedDelegate(CommentedClass instance);

        ///// <summary>
        ///// A commented event
        ///// </summary>
        //public virtual event CommentedDelegate CommentedEvent;

    }

    /// <summary>
    /// A commented interface
    /// </summary>
     public interface ICommentedInterface
     {
         /// <summary>
         /// A commented property from the interface
         /// </summary>
         int ICommentedProperty { get; set; }

         /// <summary>
         /// A commented function from the interface
         /// </summary>
         float ICommentedfunction(int value);

     }

    /// <inheritdoc/>
     public class NonCommentedClass : CommentedClass, ICommentedInterface
    {
         // empty constructor (overrided)
        /// <inheritdoc/>
        public NonCommentedClass()
        {
        }

        // (int/IEnumerable) constructor (overrided)
        /// <inheritdoc/>
        public NonCommentedClass(int i, IEnumerable k)
        {
        }

        // (IDictionnary) constructor (non overrided)-> no doc to inherit
        /// <inheritdoc/>
        public NonCommentedClass(IDictionary d)
        {
        }

        /// <inheritdoc/>
        public override float CommentedProperty { get; set; }

        /// <inheritdoc/>
        public override int CommentedFunction<T>(int i, float j, T instance) { return 1; }


        /// <inheritdoc/>
        public int ICommentedProperty { get; set; }

        /// <inheritdoc/>
        public float ICommentedfunction(int value) { return 0.0f; }

        ///// <inheritdoc/>
        //public override event CommentedDelegate CommentedEvent;

    }



    /// <inheritdoc/>
    public class NonCommentedClassChild : NonCommentedClass
    {
    }


}
