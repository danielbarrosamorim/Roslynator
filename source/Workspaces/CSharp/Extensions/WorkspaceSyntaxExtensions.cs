// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp
{
    public static class WorkspaceSyntaxExtensions
    {
        #region ExpressionSyntax
        public static ParenthesizedExpressionSyntax Parenthesize(
            this ExpressionSyntax expression,
            bool includeElasticTrivia = true,
            bool simplifiable = true)
        {
            ParenthesizedExpressionSyntax parenthesizedExpression = null;

            if (includeElasticTrivia)
            {
                parenthesizedExpression = ParenthesizedExpression(expression.WithoutTrivia());
            }
            else
            {
                parenthesizedExpression = ParenthesizedExpression(
                    Token(SyntaxTriviaList.Empty, SyntaxKind.OpenParenToken, SyntaxTriviaList.Empty),
                    expression.WithoutTrivia(),
                    Token(SyntaxTriviaList.Empty, SyntaxKind.CloseParenToken, SyntaxTriviaList.Empty));
            }

            return parenthesizedExpression
                .WithTriviaFrom(expression)
                .WithSimplifierAnnotationIf(simplifiable);
        }

        internal static ExpressionSyntax ParenthesizeIf(
            this ExpressionSyntax expression,
            bool condition,
            bool includeElasticTrivia = true,
            bool simplifiable = true)
        {
            return (condition) ? Parenthesize(expression, includeElasticTrivia, simplifiable) : expression;
        }
        #endregion ExpressionSyntax

        #region SimpleNameSyntax
        public static MemberAccessExpressionSyntax QualifyWithThis(this SimpleNameSyntax simpleName, bool simplifiable = true)
        {
            return SimpleMemberAccessExpression(ThisExpression(), simpleName).WithSimplifierAnnotationIf(simplifiable);
        }
        #endregion SimpleNameSyntax
    }
}
