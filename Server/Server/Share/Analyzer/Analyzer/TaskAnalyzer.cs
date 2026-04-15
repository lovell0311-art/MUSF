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
    public class TaskAnalyzer:DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => 
                ImmutableArray.Create(TaskInSyncMethodAnalyzerRule.Rule,TaskInAsyncMethodAnalyzerRule.Rule);
        
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            //context.RegisterSyntaxNodeAction(this.AnalyzeMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);
            context.RegisterSyntaxNodeAction(this.AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
        }
        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            if (!AnalyzerHelper.IsAssemblyNeedAnalyze(context.Compilation.AssemblyName, AnalyzeAssembly.AllModelHotfix))
            {
                return;
            }
            if (!(context.Node is InvocationExpressionSyntax invocationExpressionSyntax))
            {
                return;
            }

            if(!(context.SemanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol is IMethodSymbol methodSymbol))
            {
                return;
            }

            //忽略void返回值函数
            if (methodSymbol.ReturnsVoid)
            {
                return;
            }

            if (!(methodSymbol.ReturnType is INamedTypeSymbol namedTypeSymbol))
            {
                return;
            }

            // 筛选出返回值为Task 和Task<T>的函数
            if (namedTypeSymbol.Name != Definition.Task)
            {
                return;
            }

            // 获取Task函数调用处所在的函数体
            var containingMethodDeclarationSyntax = invocationExpressionSyntax?.GetNeareastAncestor<MethodDeclarationSyntax>();
            if (containingMethodDeclarationSyntax == null)
            {
                return;
            }

            IMethodSymbol? containingMethodSymbol = context.SemanticModel.GetDeclaredSymbol(containingMethodDeclarationSyntax);
            if (containingMethodSymbol == null)
            {
                return;
            }

            // Task函数在 ()=>Function(); 形式的lanmda表达式中时 
            if (invocationExpressionSyntax?.Parent is ParenthesizedLambdaExpressionSyntax)
            {
                Diagnostic diagnostic = Diagnostic.Create(TaskInSyncMethodAnalyzerRule.Rule, invocationExpressionSyntax?.GetLocation(),
                    invocationExpressionSyntax?.ToString());
                context.ReportDiagnostic(diagnostic);
                return;
            }


            // 方法体内Task单独调用时
            if (invocationExpressionSyntax?.Parent is ExpressionStatementSyntax)
            {
                if (containingMethodSymbol.IsAsync)
                {
                    Diagnostic diagnostic = Diagnostic.Create(TaskInAsyncMethodAnalyzerRule.Rule, invocationExpressionSyntax?.GetLocation(),
                        invocationExpressionSyntax?.ToString());
                    context.ReportDiagnostic(diagnostic);
                }
                else
                {
                    Diagnostic diagnostic = Diagnostic.Create(TaskInSyncMethodAnalyzerRule.Rule, invocationExpressionSyntax?.GetLocation(),
                        invocationExpressionSyntax?.ToString());
                    context.ReportDiagnostic(diagnostic);
                }
            }
            // 方法体内Task使用弃元
            if (invocationExpressionSyntax?.Parent is AssignmentExpressionSyntax assignmentExpressionSyntax)
            {
                SyntaxNode syntaxNode = assignmentExpressionSyntax.ChildNodes().Where(n => n is IdentifierNameSyntax).First();
                if(!(syntaxNode is IdentifierNameSyntax identifierNameSyntax))
                {
                    return;
                }
                if(identifierNameSyntax.Identifier.ValueText == "_")
                {
                    if (containingMethodSymbol.IsAsync)
                    {
                        Diagnostic diagnostic = Diagnostic.Create(TaskInAsyncMethodAnalyzerRule.Rule, invocationExpressionSyntax?.GetLocation(),
                            invocationExpressionSyntax?.ToString());
                        context.ReportDiagnostic(diagnostic);
                    }
                    else
                    {
                        Diagnostic diagnostic = Diagnostic.Create(TaskInSyncMethodAnalyzerRule.Rule, invocationExpressionSyntax?.GetLocation(),
                            invocationExpressionSyntax?.ToString());
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }

        }

    }
}