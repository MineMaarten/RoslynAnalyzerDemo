using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

namespace UseTypeAliasesAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseTypeAliasesAnalyzerCodeFixProvider)), Shared]
    public class UseTypeAliasesAnalyzerCodeFixProvider : CodeFixProvider
    {
        private const string title = "Use C# type alias";

        private static readonly Dictionary<string, string> typeAliases = new Dictionary<string, string>()
        {
            {"String", "string"},
            {"Int32", "int"},
            {"Double", "double"},
            {"Float", "float"},
            {"Int64", "long"},
            {"Int16", "short"}
            //TODO other primitive types (bytes, unsigned variants)
        };

        public static bool IsConvertableIdentifier(string identifierName) => typeAliases.ContainsKey(identifierName);

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(UseTypeAliasesAnalyzerAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var identifier = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IdentifierNameSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => ConvertToCSharpAlias(context.Document, identifier, c),
                    equivalenceKey: title),
                diagnostic);
        }

        private async Task<Document> ConvertToCSharpAlias(Document curDocument, IdentifierNameSyntax curIdentifier, CancellationToken cancellationToken)
        {
            var syntaxRoot = await curDocument.GetSyntaxRootAsync(cancellationToken);

            var typeAlias = typeAliases[curIdentifier.Identifier.ValueText];
            var newIdentifier = curIdentifier.WithIdentifier(SyntaxFactory.Identifier(typeAlias));

            var newSyntaxRoot = syntaxRoot.ReplaceNode(curIdentifier, newIdentifier);
            var newDocument = curDocument.WithSyntaxRoot(newSyntaxRoot);
            return newDocument;
        }
    }
}
