// Copyright (c) 2010-2013 SharpDoc - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// -----------------------------------------------------------------------------
// Helper methods to extract method inheritance.
// Original code from https://github.com/mono/mono/blob/master/mcs/tools/linker/Mono.Linker.Steps/TypeMapStep.cs
// -----------------------------------------------------------------------------
//
// TypeMapStep.cs
//
// Author:
// Jb Evain (jbevain@novell.com)
//
// (C) 2009 Novell, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// ----------------------------------------------------------------------
// Reuse of some methods to navigate Mono.Cecil type definitions.
// https://github.com/mono/mono/blob/master/mcs/tools/linker/Mono.Linker.Steps/TypeMapStep.cs
// ----------------------------------------------------------------------
//
// TypeMapStep.cs
//
// Author:
// Jb Evain (jbevain@novell.com)
//
// (C) 2009 Novell, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using Mono.Cecil;

namespace SharpDoc
{
    /// <summary>
    /// Helper methods for Mono.Cecil.
    /// </summary>
    public class MonoCecilHelper
    {
        void MapType(TypeDefinition type)
        {
            MapVirtualMethods(type);
            MapInterfaceMethodsInTypeHierarchy(type);
        }

        void MapInterfaceMethodsInTypeHierarchy(TypeDefinition type)
        {
            if (!type.HasInterfaces)
                return;

            foreach (TypeReference @interface in type.Interfaces)
            {
                var iface = @interface.Resolve();
                if (iface == null || !iface.HasMethods)
                    continue;

                foreach (MethodDefinition method in iface.Methods)
                {
                    if (TryMatchDef(type, method) != null)
                        continue;

                    var @base = GetBaseOverrideInTypeHierarchy(type, method);
                    if (@base == null)
                        continue;

                    //Annotations.AddPreservedMethod(type, @base);
                }
            }
        }

        void MapVirtualMethods(TypeDefinition type)
        {
            if (!type.HasMethods)
                return;

            foreach (MethodDefinition method in type.Methods)
            {
                if (!method.IsVirtual)
                    continue;

                MapVirtualMethod(method);

                if (method.HasOverrides)
                    MapOverrides(method);
            }
        }

        void MapVirtualMethod(MethodDefinition method)
        {
            MapVirtualBaseMethod(method);
            MapVirtualInterfaceMethod(method);
        }

        void MapVirtualBaseMethod(MethodDefinition method)
        {
            MethodDefinition @base = GetBaseOverrideInTypeHierarchy(method) as MethodDefinition;
            if (@base == null)
                return;

            //AnnotateMethods(@base, method);
        }

        void MapVirtualInterfaceMethod(MethodDefinition method)
        {
            MethodDefinition @base = GetBaseImplementInInterfaceHierarchy(method) as MethodDefinition;
            if (@base == null)
                return;

            //AnnotateMethods(@base, method);
        }

        void MapOverrides(MethodDefinition method)
        {
            foreach (MethodReference override_ref in method.Overrides)
            {
                MethodDefinition @override = override_ref.Resolve();
                if (@override == null)
                    continue;

                //AnnotateMethods(@override, method);
            }
        }




        public static IMemberDefinition GetBaseOverrideInTypeHierarchy(IMemberDefinition memberDef)
        {
            return GetBaseOverrideInTypeHierarchy(memberDef.DeclaringType, memberDef);
        }

        public static IMemberDefinition GetBaseImplementInInterfaceHierarchy(IMemberDefinition memberDef)
        {
            return GetBaseImplementInInterfaceHierarchy(memberDef.DeclaringType, memberDef);
        }



        public static IMemberDefinition GetBaseOverrideInTypeHierarchy(TypeDefinition type, IMemberDefinition memberDef)
        {
            TypeDefinition @base = GetBaseType(type);
            while (@base != null)
            {
                IMemberDefinition base_memberDef = TryMatchDef(@base, memberDef);
                if (base_memberDef != null)
                    return base_memberDef;

                @base = GetBaseType(@base);
            }

            return null;
        }

        public static IMemberDefinition GetBaseImplementInInterfaceHierarchy(TypeDefinition type, IMemberDefinition memberDef)
        {
            if (!type.HasInterfaces)
                return null;

            foreach (TypeReference interface_ref in type.Interfaces)
            {
                TypeDefinition @interface = interface_ref.Resolve();
                if (@interface == null)
                    continue;

                IMemberDefinition base_memberDef = TryMatchDef(@interface, memberDef);
                if (base_memberDef != null)
                    return base_memberDef;

                base_memberDef = GetBaseImplementInInterfaceHierarchy(@interface, memberDef);
                if (base_memberDef != null)
                    return base_memberDef;
            }

            return null;
        }

        static IMemberDefinition TryMatchDef(TypeDefinition type, IMemberDefinition memberDef)
        {
            if (memberDef is MethodDefinition)
                return TryMatchDef(type, memberDef as MethodDefinition);
            if (memberDef is PropertyDefinition)
                return TryMatchDef(type, memberDef as PropertyDefinition);
            if (memberDef is EventDefinition)
                return TryMatchDef(type, memberDef as EventDefinition);
            else
                return null;
        }


        static MethodDefinition TryMatchDef(TypeDefinition type, MethodDefinition method)
        {
            if (!type.HasMethods)
                return null;

            foreach (MethodDefinition candidate in type.Methods)
                if (MethodMatch(candidate, method))
                    return candidate;

            return null;
        }
        
        static PropertyDefinition TryMatchDef(TypeDefinition type, PropertyDefinition property)
        {
            if (!type.HasProperties)
                return null;

            foreach (PropertyDefinition candidate in type.Properties)
                if (PropertyMatch(candidate, property))
                    return candidate;

            return null;
        }

        static EventDefinition TryMatchDef(TypeDefinition type, EventDefinition property)
        {
            if (!type.HasEvents)
                return null;

            foreach (EventDefinition candidate in type.Events)
                if (EventMatch(candidate, property))
                    return candidate;

            return null;
        }



    
        static bool MethodMatch(MethodDefinition candidate, MethodDefinition method)
        {
            if (!candidate.IsVirtual)
                return false;

            if (candidate.Name != method.Name)
                return false;

            if (!TypeMatch(candidate.ReturnType, method.ReturnType))
                return false;

            if (candidate.Parameters.Count != method.Parameters.Count)
                return false;

            for (int i = 0; i < candidate.Parameters.Count; i++)
                if (!TypeMatch(candidate.Parameters[i].ParameterType, method.Parameters[i].ParameterType))
                    return false;

            return true;
        }

        static bool PropertyMatch(PropertyDefinition candidate, PropertyDefinition method)
        {
            if (candidate.Name != method.Name)
                return false;

            if (!TypeMatch(candidate.PropertyType, method.PropertyType))
                return false;

            return true;
        }

        static bool EventMatch(EventDefinition candidate, EventDefinition method)
        {
            if (candidate.Name != method.Name)
                return false;

            if (!TypeMatch(candidate.EventType, method.EventType))
                return false;

            return true;
        }





















        static bool TypeMatch(IModifierType a, IModifierType b)
        {
            if (!TypeMatch(a.ModifierType, b.ModifierType))
                return false;

            return TypeMatch(a.ElementType, b.ElementType);
        }

        static bool TypeMatch(TypeSpecification a, TypeSpecification b)
        {
            if (a is GenericInstanceType)
                return TypeMatch((GenericInstanceType)a, (GenericInstanceType)b);

            if (a is IModifierType)
                return TypeMatch((IModifierType)a, (IModifierType)b);

            return TypeMatch(a.ElementType, b.ElementType);
        }

        static bool TypeMatch(GenericInstanceType a, GenericInstanceType b)
        {
            if (!TypeMatch(a.ElementType, b.ElementType))
                return false;

            if (a.GenericArguments.Count != b.GenericArguments.Count)
                return false;

            if (a.GenericArguments.Count == 0)
                return true;

            for (int i = 0; i < a.GenericArguments.Count; i++)
                if (!TypeMatch(a.GenericArguments[i], b.GenericArguments[i]))
                    return false;

            return true;
        }

        static bool TypeMatch(TypeReference a, TypeReference b)
        {
            if (a is GenericParameter)
                return true;

            if (a is TypeSpecification || b is TypeSpecification)
            {
                if (a.GetType() != b.GetType())
                    return false;

                return TypeMatch((TypeSpecification)a, (TypeSpecification)b);
            }

            return a.FullName == b.FullName;
        }

        public static TypeDefinition GetBaseType(TypeDefinition type)
        {
            if (type == null || type.BaseType == null)
                return null;

            return type.BaseType.Resolve();
        }

        public static TypeDefinition GetBaseTypeAfterSystemObject(TypeDefinition type)
        {
            var baseType = GetBaseType(type);
            if (baseType == null)
                return type;

            if (baseType.FullName == "System.Object")
                return type;

            return GetBaseTypeAfterSystemObject(baseType);
        }

        public static bool IsDelegate(TypeDefinition type)
        {
            var baseType = GetBaseTypeAfterSystemObject(type);
            if (baseType == null)
                return false;
            return (baseType.FullName == "System.Delegate");
        }
    }
}