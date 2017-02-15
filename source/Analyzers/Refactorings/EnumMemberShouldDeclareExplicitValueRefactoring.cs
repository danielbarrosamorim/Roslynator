﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class EnumMemberShouldDeclareExplicitValueRefactoring
    {
        public static void AnalyzeEnumMemberDeclaration(SyntaxNodeAnalysisContext context)
        {
            var enumMember = (EnumMemberDeclarationSyntax)context.Node;

            if (HasImplicitValue(enumMember, context.SemanticModel, context.CancellationToken))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.EnumMemberShouldDeclareExplicitValue,
                    enumMember.Identifier);
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            EnumMemberDeclarationSyntax enumMember,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            object value = GetValue(enumMember, semanticModel, cancellationToken);

            Debug.Assert(value != null, "");

            if (value != null)
            {
                EqualsValueClauseSyntax equalsValue = EqualsValueClause(ConstantExpression(value));

                EnumMemberDeclarationSyntax newNode = enumMember.WithEqualsValue(equalsValue);

                return await document.ReplaceNodeAsync(enumMember, newNode, cancellationToken).ConfigureAwait(false);
            }

            return document;
        }

        private static object GetValue(
            EnumMemberDeclarationSyntax enumMember,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            IFieldSymbol fieldSymbol = semanticModel.GetDeclaredSymbol(enumMember, cancellationToken);

            INamedTypeSymbol enumSymbol = fieldSymbol.ContainingType;

            if (Symbol.IsEnumWithFlagsAttribute(enumSymbol, semanticModel))
            {
                var enumDeclaration = (EnumDeclarationSyntax)enumMember.Parent;
                object[] values = GetExplicitValues(enumDeclaration, semanticModel, cancellationToken).ToArray();
                SpecialType specialType = enumSymbol.EnumUnderlyingType.SpecialType;

                Optional<object> optional = EnumHelper.GetUniquePowerOfTwo(
                    specialType,
                    values,
                    startFromHighestExistingValue: false);

                Debug.Assert(optional.HasValue, "");

                if (optional.HasValue)
                {
                    object value = optional.Value;

                    SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclaration.Members;
                    int index = members.IndexOf(enumMember);
                    int count = members.Take(index).Count(f => HasImplicitValue(f, semanticModel, cancellationToken));

                    switch (specialType)
                    {
                        case SpecialType.System_SByte:
                            return GetUniquePowerOfTwo((sbyte)value, count, values.Cast<sbyte>().ToArray());
                        case SpecialType.System_Byte:
                            return GetUniquePowerOfTwo((byte)value, count, values.Cast<byte>().ToArray());
                        case SpecialType.System_Int16:
                            return GetUniquePowerOfTwo((short)value, count, values.Cast<short>().ToArray());
                        case SpecialType.System_UInt16:
                            return GetUniquePowerOfTwo((ushort)value, count, values.Cast<ushort>().ToArray());
                        case SpecialType.System_Int32:
                            return GetUniquePowerOfTwo((int)value, count, values.Cast<int>().ToArray());
                        case SpecialType.System_UInt32:
                            return GetUniquePowerOfTwo((uint)value, count, values.Cast<uint>().ToArray());
                        case SpecialType.System_Int64:
                            return GetUniquePowerOfTwo((long)value, count, values.Cast<long>().ToArray());
                        case SpecialType.System_UInt64:
                            return GetUniquePowerOfTwo((ulong)value, count, values.Cast<ulong>().ToArray());
                    }
                }
            }
            else
            {
                return fieldSymbol.ConstantValue;
            }

            return null;
        }

        private static bool HasImplicitValue(EnumMemberDeclarationSyntax enumMember, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            EqualsValueClauseSyntax equalsValue = enumMember.EqualsValue;

            if (equalsValue == null)
            {
                return semanticModel
                    .GetDeclaredSymbol(enumMember, cancellationToken)?
                    .HasConstantValue == true;
            }

            return false;
        }

        private static IEnumerable<object> GetExplicitValues(
            EnumDeclarationSyntax enumDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            foreach (EnumMemberDeclarationSyntax enumMember in enumDeclaration.Members)
            {
                ExpressionSyntax value = enumMember.EqualsValue?.Value;

                if (value != null)
                {
                    var fieldSymbol = semanticModel.GetDeclaredSymbol(enumMember, cancellationToken) as IFieldSymbol;

                    if (fieldSymbol?.HasConstantValue == true)
                        yield return fieldSymbol.ConstantValue;
                }
            }
        }

        private static object GetUniquePowerOfTwo(sbyte value, int count, sbyte[] reservedValues)
        {
            int i = 0;
            while (i < count)
            {
                value *= 2;

                if (value < 0)
                    return null;

                if (Array.IndexOf(reservedValues, value) == -1)
                    i++;
            }

            return value;
        }

        private static object GetUniquePowerOfTwo(byte value, int count, byte[] reservedValues)
        {
            int i = 0;
            while (i < count)
            {
                value *= 2;

                if (value < 0)
                    return null;

                if (Array.IndexOf(reservedValues, value) == -1)
                    i++;
            }

            return value;
        }

        private static object GetUniquePowerOfTwo(short value, int count, short[] reservedValues)
        {
            int i = 0;
            while (i < count)
            {
                value *= 2;

                if (value < 0)
                    return null;

                if (Array.IndexOf(reservedValues, value) == -1)
                    i++;
            }

            return value;
        }

        private static object GetUniquePowerOfTwo(ushort value, int count, ushort[] reservedValues)
        {
            int i = 0;
            while (i < count)
            {
                value *= 2;

                if (value < 0)
                    return null;

                if (Array.IndexOf(reservedValues, value) == -1)
                    i++;
            }

            return value;
        }

        private static object GetUniquePowerOfTwo(int value, int count, int[] reservedValues)
        {
            int i = 0;
            while (i < count)
            {
                value *= 2;

                if (value < 0)
                    return null;

                if (Array.IndexOf(reservedValues, value) == -1)
                    i++;
            }

            return value;
        }

        private static object GetUniquePowerOfTwo(uint value, int count, uint[] reservedValues)
        {
            int i = 0;
            while (i < count)
            {
                value *= 2;

                if (value < 0)
                    return null;

                if (Array.IndexOf(reservedValues, value) == -1)
                    i++;
            }

            return value;
        }

        private static object GetUniquePowerOfTwo(long value, int count, long[] reservedValues)
        {
            int i = 0;
            while (i < count)
            {
                value *= 2;

                if (value < 0)
                    return null;

                if (Array.IndexOf(reservedValues, value) == -1)
                    i++;
            }

            return value;
        }

        private static object GetUniquePowerOfTwo(ulong value, int count, ulong[] reservedValues)
        {
            int i = 0;
            while (i < count)
            {
                value *= 2;

                if (value < 0)
                    return null;

                if (Array.IndexOf(reservedValues, value) == -1)
                    i++;
            }

            return value;
        }
    }
}