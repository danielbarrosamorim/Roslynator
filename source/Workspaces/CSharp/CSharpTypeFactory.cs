// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp
{
    public static class CSharpTypeFactory
    {
        internal static TypeSyntax BoolType(bool simplifiable = true)
        {
            return ParseTypeName("global::System.Boolean").WithSimplifierAnnotationIf(simplifiable);
        }

        internal static TypeSyntax ByteType(bool simplifiable = true)
        {
            return ParseTypeName("global::System.Byte").WithSimplifierAnnotationIf(simplifiable);
        }

        internal static TypeSyntax SByteType(bool simplifiable = true)
        {
            return ParseTypeName("global::System.SByte").WithSimplifierAnnotationIf(simplifiable);
        }

        internal static TypeSyntax IntType(bool simplifiable = true)
        {
            return ParseTypeName("global::System.Int32").WithSimplifierAnnotationIf(simplifiable);
        }

        internal static TypeSyntax UIntType(bool simplifiable = true)
        {
            return ParseTypeName("global::System.UInt32").WithSimplifierAnnotationIf(simplifiable);
        }

        internal static TypeSyntax ShortType(bool simplifiable = true)
        {
            return ParseTypeName("global::System.Int16").WithSimplifierAnnotationIf(simplifiable);
        }

        internal static TypeSyntax UShortType(bool simplifiable = true)
        {
            return ParseTypeName("global::System.UInt16").WithSimplifierAnnotationIf(simplifiable);
        }

        internal static TypeSyntax LongType(bool simplifiable = true)
        {
            return ParseTypeName("global::System.Int64").WithSimplifierAnnotationIf(simplifiable);
        }

        internal static TypeSyntax ULongType(bool simplifiable = true)
        {
            return ParseTypeName("global::System.UInt64").WithSimplifierAnnotationIf(simplifiable);
        }

        internal static TypeSyntax FloatType(bool simplifiable = true)
        {
            return ParseTypeName("global::System.Single").WithSimplifierAnnotationIf(simplifiable);
        }

        internal static TypeSyntax DoubleType(bool simplifiable = true)
        {
            return ParseTypeName("global::System.Double").WithSimplifierAnnotationIf(simplifiable);
        }

        internal static TypeSyntax DecimalType(bool simplifiable = true)
        {
            return ParseTypeName("global::System.Decimal").WithSimplifierAnnotationIf(simplifiable);
        }

        internal static TypeSyntax StringType(bool simplifiable = true)
        {
            return ParseTypeName("global::System.String").WithSimplifierAnnotationIf(simplifiable);
        }

        internal static TypeSyntax CharType(bool simplifiable = true)
        {
            return ParseTypeName("global::System.Char").WithSimplifierAnnotationIf(simplifiable);
        }

        internal static TypeSyntax ObjectType(bool simplifiable = true)
        {
            return ParseTypeName("global::System.Object").WithSimplifierAnnotationIf(simplifiable);
        }
    }
}
