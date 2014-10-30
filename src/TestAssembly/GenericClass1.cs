using System;
using System.Collections.Generic;

namespace TestAssembly
{
    /// <summary>
    /// This is a class with generic type parameters.
    /// </summary>
    public class GenericClass1<T>
        where T : IEnumerable<int>
    {
        /// <summary>
        /// This is a method with a generic type parameter.
        /// </summary>
        /// <typeparam name="U">The generic type parameter.</typeparam>
        public Tuple<T, U> GenericMethod<U>()
        {
            return null;
        }
    }
}