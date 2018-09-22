using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DigiBotExtension.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DIGI001IrcConnectShouldImplementIIrcConnection : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DIGI001";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Implement IIrcConnection",
            messageFormat: "{0} to use IrcConnectionAttribute.",
            category: "DigiBotExtensionAnalyzer",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(Handle, SyntaxKind.ClassDeclaration);
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
            if (context.Node == null ||
                context.Node.IsMissing ||
                context.SemanticModel == null ||
                context.SemanticModel.SyntaxTree.FilePath.EndsWith(".g.cs"))
            {
                return;
            }

            var type = (ITypeSymbol)context.ContainingSymbol;
            if (type.IsStatic ||
                !type.GetAttributes().Any((attr) => attr.AttributeClass.ContainingNamespace.Name == "DigiBotExtension" && attr.AttributeClass.Name == "IrcConnectionAttribute") ||
                type.AllInterfaces.Any((inter) => inter.ContainingNamespace.Name == "DigiBotExtension" && inter.Name == "IIrcConnection"))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.FirstAncestorOrSelf<ClassDeclarationSyntax>().Identifier.GetLocation(), $"The class {type.Name} should impliment: IIrcConnection"));
        }
    }
}
