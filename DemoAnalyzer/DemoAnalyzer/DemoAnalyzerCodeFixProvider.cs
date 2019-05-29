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

namespace DemoAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DemoAnalyzerCodeFixProvider)), Shared]
    public class DemoAnalyzerCodeFixProvider : CodeFixProvider
    {
        private const string title = "Add T in front of the generic type name";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DemoAnalyzerAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type parameter declaration identified by the diagnostic.
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<TypeParameterSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedSolution: c => PrefixTAsync(context.Document, declaration, c),
                    equivalenceKey: title),
                diagnostic);
        }

        private async Task<Solution> PrefixTAsync(Document document, TypeParameterSyntax declaration, CancellationToken cancellationToken)
        {
            //We need to acquire the semantic model, to answer where the type parameter we would like to change is used. 
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);

            //Get the symbol that belongs to the declaration syntax from the semantic model
            var typeSymbol = semanticModel.GetDeclaredSymbol(declaration, cancellationToken);

            //The new type name becomes TType (e.g. Bar -> TBar)
            var newName = "T" + declaration.Identifier.ValueText;

            //Use the built-in helper to rename the symbol in the whole solution.
            return await Renamer.RenameSymbolAsync(document.Project.Solution, typeSymbol, newName, document.Project.Solution.Options, cancellationToken);
        }
    }
}
