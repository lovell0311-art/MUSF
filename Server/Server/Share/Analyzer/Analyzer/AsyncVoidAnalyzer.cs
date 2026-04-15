using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Diagnostics;

namespace ET.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AsyncVoidAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => 
                ImmutableArray.Create(AsyncVoidAnalyzerRule.Rule);
        
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(this.Analyzer, SymbolKind.Method);
            context.RegisterSyntaxNodeAction(this.AnalyzeLocalFunctionStatement, SyntaxKind.LocalFunctionStatement);
        }
        private void Analyzer(SymbolAnalysisContext context)
        {
            if (!AnalyzerHelper.IsAssemblyNeedAnalyze(context.Compilation.AssemblyName, AnalyzeAssembly.AllModelHotfix))
            {
                return;
            }

            if(!(context.Symbol is IMethodSymbol methodSymbol))
            {
                return;
            }

            if (methodSymbol.ReturnsVoid && methodSymbol.IsAsync)
            {
                if (!CheckIsTypeOrBaseTypeHasUnsafeAsyncAttributeInherit(methodSymbol.ContainingType))
                {
                    ReportDiagnostic(methodSymbol);
                }
            }

            void ReportDiagnostic(ISymbol symbol)
            {
                foreach (SyntaxReference? declaringSyntaxReference in symbol.DeclaringSyntaxReferences)
                {
                    Diagnostic diagnostic = Diagnostic.Create(AsyncVoidAnalyzerRule.Rule, declaringSyntaxReference.GetSyntax()?.GetLocation(), symbol.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }

        }

        private void AnalyzeLocalFunctionStatement(SyntaxNodeAnalysisContext context)
        {
            if (!AnalyzerHelper.IsAssemblyNeedAnalyze(context.Compilation.AssemblyName, AnalyzeAssembly.AllModelHotfix))
            {
                return;
            }
            if(!(context.Node is LocalFunctionStatementSyntax localFunctionStatementSyntax))
            {
                return;
            }
            if(!(context.SemanticModel.GetDeclaredSymbol(localFunctionStatementSyntax) is IMethodSymbol methodSymbol))
            {
                return;
            }

            if (methodSymbol.ReturnsVoid && methodSymbol.IsAsync)
            {
                if (!CheckIsTypeOrBaseTypeHasUnsafeAsyncAttributeInherit(methodSymbol.ContainingType))
                {
                    ReportDiagnostic(methodSymbol);
                }
            }


            void ReportDiagnostic(ISymbol symbol)
            {
                foreach (SyntaxReference? declaringSyntaxReference in symbol.DeclaringSyntaxReferences)
                {
                    Diagnostic diagnostic = Diagnostic.Create(AsyncVoidAnalyzerRule.Rule, declaringSyntaxReference.GetSyntax()?.GetLocation(), symbol.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
            //             localFunctionStatementSyntax.SyntaxTree.
            //             if (localFunctionStatementSyntax.ReturnsVoid && localFunctionStatementSyntax.ChildNodes().Any(n => n.IsKind(SyntaxKind.AsyncKeyword)))
            //             {
            // 
            //             }
        }

        /// <summary>
        ///     检查该类是否有BaseAttribute的子类特性标记
        /// </summary>
        private bool CheckIsTypeOrBaseTypeHasUnsafeAsyncAttributeInherit(INamedTypeSymbol namedTypeSymbol)
        {
            var attributes = namedTypeSymbol.GetAttributes();
            foreach (AttributeData? attributeData in attributes)
            {
                if (attributeData.AttributeClass?.ToString() != Definition.UnsafeAsyncAttribute)
                {
                    continue;
                }
                return true;
            }
            return false;
        }
    }
}