using System;
using System.Composition;
using System.Collections.Generic;
using System.Collections.Immutable;
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
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Simplification;

namespace DigiBotExtension.Analyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IrcConnectShouldImplementIIrcConnectionCodeFixProvider)), Shared]
    public class IrcConnectShouldImplementIIrcConnectionCodeFixProvider : CodeFixProvider
    {
        // TODO: Replace with actual diagnostic id that should trigger this fix.
        public const string DiagnosticId = DIGI001IrcConnectShouldImplementIIrcConnection.DiagnosticId;

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticId); }
        }

        ///<inheritdoc/>
        public override FixAllProvider GetFixAllProvider() => null;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                if (diagnostic.Id != DiagnosticId)
                {
                    continue;
                }

                var classDeclaration = syntaxRoot.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<ClassDeclarationSyntax>();
                if (classDeclaration == null)
                {
                    continue;
                }

                var type = (ITypeSymbol)semanticModel.GetDeclaredSymbol(classDeclaration, context.CancellationToken);
                context.RegisterCodeFix(
                    CodeAction.Create("Impliment IIrcConnection.",
                        token => ImplementIIrcConnectionAsync(context, semanticModel, classDeclaration, token),
                        this.GetType().FullName),
                    diagnostic);
            }
        }

        public static async Task<Document> ImplementIIrcConnectionAsync(CodeFixContext context, SemanticModel semanticModel, ClassDeclarationSyntax classDeclaration, CancellationToken token)
        {
            var editor = await DocumentEditor.CreateAsync(context.Document, token).ConfigureAwait(false);
            ImplementIIrcConnection(context, semanticModel, classDeclaration, editor);
            return editor.GetChangedDocument();
        }

        public static void ImplementIIrcConnection(CodeFixContext context, SemanticModel semanticModel, ClassDeclarationSyntax classDeclaration, DocumentEditor editor)
        {
            var type = (ITypeSymbol)semanticModel.GetDeclaredSymbol(classDeclaration, context.CancellationToken);
            editor.AddInterfaceType(classDeclaration, SyntaxFactory.ParseTypeName("DigiBotExtension.IIrcConnection").WithTrailingTrivia(SyntaxFactory.ElasticMarker).WithAdditionalAnnotations(Simplifier.Annotation, SyntaxAnnotation.ElasticAnnotation));

        }
    }
}
