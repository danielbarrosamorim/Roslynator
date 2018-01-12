// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp
{
    internal static class CSharpFormatter
    {
        public static ConditionalExpressionSyntax ToMultiLine(ConditionalExpressionSyntax conditionalExpression, CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxTriviaList leadingTrivia = GetIncreasedIndentation(conditionalExpression, cancellationToken);

            leadingTrivia = leadingTrivia.Insert(0, NewLine());

            return ConditionalExpression(
                    conditionalExpression.Condition.WithoutTrailingTrivia(),
                    Token(leadingTrivia, SyntaxKind.QuestionToken, TriviaList(Space)),
                    conditionalExpression.WhenTrue.WithoutTrailingTrivia(),
                    Token(leadingTrivia, SyntaxKind.ColonToken, TriviaList(Space)),
                    conditionalExpression.WhenFalse.WithoutTrailingTrivia());
        }

        public static ParameterListSyntax ToMultiLine(ParameterListSyntax parameterList, CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxTriviaList leadingTrivia = GetIncreasedIndentation(parameterList, cancellationToken);

            var nodesAndTokens = new List<SyntaxNodeOrToken>();

            SeparatedSyntaxList<ParameterSyntax>.Enumerator en = parameterList.Parameters.GetEnumerator();

            if (en.MoveNext())
            {
                nodesAndTokens.Add(en.Current.WithLeadingTrivia(leadingTrivia));

                while (en.MoveNext())
                {
                    nodesAndTokens.Add(CommaToken().WithTrailingTrivia(NewLine()));

                    nodesAndTokens.Add(en.Current.WithLeadingTrivia(leadingTrivia));
                }
            }

            return ParameterList(
                OpenParenToken().WithTrailingTrivia(NewLine()),
                SeparatedList<ParameterSyntax>(nodesAndTokens),
                parameterList.CloseParenToken);
        }

        public static InitializerExpressionSyntax ToMultiLine(InitializerExpressionSyntax initializer, CancellationToken cancellationToken)
        {
            SyntaxNode parent = initializer.Parent;

            if (parent.IsKind(SyntaxKind.ObjectCreationExpression)
                && !initializer.IsKind(SyntaxKind.CollectionInitializerExpression))
            {
                return initializer
                    .WithExpressions(
                        SeparatedList(
                            initializer.Expressions.Select(expression => expression.WithLeadingTrivia(NewLine()))));
            }
            else
            {
                SyntaxTrivia trivia = GetIndentation(initializer, cancellationToken);

                SyntaxTriviaList braceTrivia = TriviaList(NewLine(), trivia);
                SyntaxTriviaList expressionTrivia = TriviaList(NewLine(), trivia, ComputeIndentation(trivia));

                return initializer
                    .WithExpressions(
                        SeparatedList(
                            initializer.Expressions.Select(expression => expression.WithLeadingTrivia(expressionTrivia))))
                    .WithOpenBraceToken(initializer.OpenBraceToken.WithLeadingTrivia(braceTrivia))
                    .WithCloseBraceToken(initializer.CloseBraceToken.WithLeadingTrivia(braceTrivia));
            }
        }

        public static ArgumentListSyntax ToMultiLine(ArgumentListSyntax argumentList, CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxTriviaList leadingTrivia = GetIncreasedIndentation(argumentList, cancellationToken);

            var nodesAndTokens = new List<SyntaxNodeOrToken>();

            SeparatedSyntaxList<ArgumentSyntax>.Enumerator en = argumentList.Arguments.GetEnumerator();

            if (en.MoveNext())
            {
                nodesAndTokens.Add(en.Current
                    .TrimTrailingTrivia()
                    .WithLeadingTrivia(leadingTrivia));

                while (en.MoveNext())
                {
                    nodesAndTokens.Add(CommaToken().WithTrailingTrivia(NewLine()));

                    nodesAndTokens.Add(en.Current
                        .TrimTrailingTrivia()
                        .WithLeadingTrivia(leadingTrivia));
                }
            }

            return ArgumentList(
                OpenParenToken().WithTrailingTrivia(NewLine()),
                SeparatedList<ArgumentSyntax>(nodesAndTokens),
                argumentList.CloseParenToken.WithoutLeadingTrivia());
        }

        private static SyntaxTrivia GetIndentation(SyntaxNode node, CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxTree tree = node.SyntaxTree;

            if (tree != null)
            {
                TextSpan span = node.Span;

                int lineStartIndex = span.Start - tree.GetLineSpan(span, cancellationToken).StartLinePosition.Character;

                while (!node.FullSpan.Contains(lineStartIndex))
                    node = node.Parent;

                SyntaxToken token = node.FindToken(lineStartIndex);

                if (!token.IsKind(SyntaxKind.None))
                {
                    SyntaxTriviaList leadingTrivia = token.LeadingTrivia;

                    if (leadingTrivia.Any()
                        && leadingTrivia.FullSpan.Contains(lineStartIndex))
                    {
                        SyntaxTrivia trivia = leadingTrivia.Last();

                        if (trivia.IsWhitespaceTrivia())
                            return trivia;
                    }
                }
            }

            return EmptyWhitespace();
        }

        internal static SyntaxTriviaList GetIncreasedIndentation(SyntaxNode node, CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxTrivia trivia = GetIndentation(node, cancellationToken);

            return IncreaseIndentation(trivia);
        }

        internal static SyntaxTriviaList IncreaseIndentation(SyntaxTrivia trivia)
        {
            return TriviaList(trivia, ComputeIndentation(trivia));
        }

        private static SyntaxTrivia ComputeIndentation(SyntaxTrivia trivia)
        {
            if (trivia.IsWhitespaceTrivia())
            {
                string s = trivia.ToString();

                int length = s.Length;

                if (length > 0)
                {
                    if (s.All(f => f == '\t'))
                    {
                        return Tab;
                    }
                    else if (s.All(f => f == ' '))
                    {
                        if (length % 4 == 0)
                            return Whitespace("    ");

                        if (length % 3 == 0)
                            return Whitespace("   ");

                        if (length % 2 == 0)
                            return Whitespace("  ");
                    }
                }
            }

            return DefaultIndentation;
        }
    }
}
