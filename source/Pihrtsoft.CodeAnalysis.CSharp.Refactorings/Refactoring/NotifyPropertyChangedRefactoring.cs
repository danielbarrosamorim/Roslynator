﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class NotifyPropertyChangedRefactoring
    {
        public static async Task<bool> CanRefactorAsync(
            RefactoringContext context,
            PropertyDeclarationSyntax property)
        {
            AccessorDeclarationSyntax setter = property.Setter();

            if (setter?.Body?.Statements.Count == 1)
            {
                StatementSyntax statement = setter.Body.Statements[0];

                if (statement.IsKind(SyntaxKind.ExpressionStatement))
                {
                    var expressionStatement = (ExpressionStatementSyntax)statement;

                    if (expressionStatement.Expression?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
                    {
                        var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

                        if (assignment.Left?.IsKind(SyntaxKind.IdentifierName) == true
                            && assignment.Right?.IsKind(SyntaxKind.IdentifierName) == true)
                        {
                            var identifierName = (IdentifierNameSyntax)assignment.Right;

                            if (identifierName.Identifier.ValueText == "value")
                                return await ImplementsINotifyPropertyChangedAsync(context, property);
                        }
                    }
                }
            }

            return false;
        }

        private static async Task<bool> ImplementsINotifyPropertyChangedAsync(
            RefactoringContext context,
            PropertyDeclarationSyntax property)
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync();

            IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(property, context.CancellationToken);
            if (propertySymbol != null
                && propertySymbol.ContainingType != null)
            {
                INamedTypeSymbol inotifyPropertyChanged = semanticModel.Compilation.GetTypeByMetadataName("System.ComponentModel.INotifyPropertyChanged");

                if (inotifyPropertyChanged != null)
                    return propertySymbol.ContainingType.AllInterfaces.Contains(inotifyPropertyChanged);
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            PropertyDeclarationSyntax property,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            AccessorDeclarationSyntax setter = property.Setter();

            AccessorDeclarationSyntax newSetter = CreateSetter(
                GetBackingFieldIdentifierName(setter).WithoutTrivia(),
                property.Identifier.ValueText);

            newSetter = newSetter
                .WithTriviaFrom(property)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(setter, newSetter);

            return document.WithSyntaxRoot(newRoot);
        }

        private static AccessorDeclarationSyntax CreateSetter(IdentifierNameSyntax fieldIdentifierName, string propertName)
        {
            return AccessorDeclaration(
                SyntaxKind.SetAccessorDeclaration,
                Block(
                    IfStatement(
                        BinaryExpression(
                            SyntaxKind.NotEqualsExpression,
                            fieldIdentifierName,
                            IdentifierName("value")),
                        Block(
                            ExpressionStatement(
                                AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    fieldIdentifierName,
                                    IdentifierName("value"))),
                            ExpressionStatement(
                                InvocationExpression(
                                    IdentifierName("OnPropertyChanged"),
                                    ArgumentList(
                                        SingletonSeparatedList(
                                            Argument(
                                                InvocationExpression(
                                                    IdentifierName("nameof"),
                                                    ArgumentList(
                                                        SingletonSeparatedList(
                                                            Argument(
                                                                IdentifierName(Identifier(propertName)))))))))))))));
        }

        public static IdentifierNameSyntax GetBackingFieldIdentifierName(AccessorDeclarationSyntax accessor)
        {
            var expressionStatement = (ExpressionStatementSyntax)accessor.Body.Statements[0];

            var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

            return (IdentifierNameSyntax)assignment.Left;
        }
    }
}
