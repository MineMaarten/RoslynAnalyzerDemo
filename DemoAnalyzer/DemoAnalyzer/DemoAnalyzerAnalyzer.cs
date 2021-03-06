using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DemoAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DemoAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        //https://csharpcodingguidelines.com/naming-guidelines/
        public const string DiagnosticId = "AV1709";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
            => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            //Register a callback for when a TypeParameterSyntax is encountered when analyzing the source code.
            context.RegisterSyntaxNodeAction(AnalyzeTypeParameter, SyntaxKind.TypeParameter);
        }

        private void AnalyzeTypeParameter(SyntaxNodeAnalysisContext context)
        {
            var typeParameter = (TypeParameterSyntax)context.Node;

            //If the type parameter name starts with 'T', all is ok.
            if (typeParameter.Identifier.ValueText.StartsWith("T", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            //When the type parameter name does NOT start with a 'T', we report an error at the location of the declaration.
            var diagnostic = Diagnostic.Create(Rule, typeParameter.GetLocation(), typeParameter.Identifier.ValueText);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
