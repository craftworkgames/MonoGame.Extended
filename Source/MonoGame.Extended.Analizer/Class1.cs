using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MonoGame.Extended.Analizer
{
    public static class DiagnosticCategories
    {
        public const string Category = "MonoGame.Extended Analyzers";
    }
    public static class DiagnosticIds
    {
        public const string NoIShapeFRuleId = "MGE0001";
        public const string ImplementIShapeFRuleId = "MGE0002";
        public const string ImplementICollisionActorRuleId = "MGE0003";
    }

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IShapeFUsedAnalyzer : DiagnosticAnalyzer
    {
        private const string _titleNoIShapeF = "Do not use the Interface IShapeF anywher but in where clauses.";
        public const string MessageFormatNoIShapeF = "'{0}' has the type IShapeF, considering using the actual struct or use a GenericType.";
        private const string _descriptionNoIShapeF = "Do not use the Interface IShapeF anywher but in where clauses. This causes boxing of structs";

        private const string _titleImplementIShapeF = "Do not implement the Interface IShapeF.";
        public const string MessageFormatImplementIShapeF = "'{0}' implements IShapeF, using this type where IShapeF is expected may result in excpetions.";
        private const string _descriptionImplementIShapeF = "Do not implement the Interface IShapeF. Many Methods will throw a NotSupportedException if a type can't be casted to an expected struct.";

        private const string _titleImplementICollisionActor = "Do not implement the Interface IShapeF.";
        public const string MessageFormatImplementICollisionActor = "'{0}' implements IShapeF, using this type where IShapeF is expected may result in excpetions.";
        private const string _descriptionImplementICollisionActor = "Do not implement the Interface IShapeF. Many Methods will throw a NotSupportedException if a type can't be casted to an expected struct.";

        internal static DiagnosticDescriptor CollisionActorRule =
            new DiagnosticDescriptor(
                DiagnosticIds.ImplementICollisionActorRuleId,
                _titleImplementICollisionActor,
                MessageFormatImplementICollisionActor,
                DiagnosticCategories.Category,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: _descriptionImplementICollisionActor);


        internal static DiagnosticDescriptor UseRule =
            new DiagnosticDescriptor(
                DiagnosticIds.NoIShapeFRuleId,
                _titleNoIShapeF,
                MessageFormatNoIShapeF,
                DiagnosticCategories.Category,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: _descriptionNoIShapeF);

        internal static DiagnosticDescriptor ImplementRule =
            new DiagnosticDescriptor(
                DiagnosticIds.ImplementIShapeFRuleId,
                _titleImplementIShapeF,
                MessageFormatImplementIShapeF,
                DiagnosticCategories.Category,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: _descriptionImplementIShapeF);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(UseRule, ImplementRule, CollisionActorRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType,
                SymbolKind.Method,
                SymbolKind.Property,
                SymbolKind.Field,
                SymbolKind.Parameter
                );
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.LocalDeclarationStatement);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var shapeType = context.Compilation.GetTypeByMetadataName("MonoGame.Extended.IShapeF");
            if (context.Node is LocalDeclarationStatementSyntax localDeclarationStatementSyntax)
            {
                if (shapeType.Equals(context.SemanticModel.GetTypeInfo(localDeclarationStatementSyntax.Declaration.Type).Type, SymbolEqualityComparer.Default))

                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            UseRule,
                            localDeclarationStatementSyntax.GetLocation(),
                            string.Join(", ", localDeclarationStatementSyntax.Declaration.Variables)));
            }
        }


        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var shapeType = context.Compilation.GetTypeByMetadataName("MonoGame.Extended.IShapeF");

            bool shapeUsed = false;
            if (context.Symbol is INamedTypeSymbol namedTypeSymbol)
            {
                var rectType = context.Compilation.GetTypeByMetadataName("MonoGame.Extended.RectangleF");
                var circleType = context.Compilation.GetTypeByMetadataName("MonoGame.Extended.CircleF");
                var elipseType = context.Compilation.GetTypeByMetadataName("MonoGame.Extended.EllipseF");

                var iCollisionActorType = context.Compilation.GetTypeByMetadataName("MonoGame.Extended.Collisions.ICollisionActor");
                var iCollisionActorGenericType = context.Compilation.GetTypeByMetadataName("MonoGame.Extended.Collisions.ICollisionActor`1");

                if (namedTypeSymbol.Interfaces.Any(x => shapeType.Equals(x, SymbolEqualityComparer.Default))
                    // we do not want to trigger the diagnostics on our internal type
                    && !namedTypeSymbol.Equals(rectType, SymbolEqualityComparer.Default)
                    && !namedTypeSymbol.Equals(circleType, SymbolEqualityComparer.Default)
                    && !namedTypeSymbol.Equals(elipseType, SymbolEqualityComparer.Default))
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            ImplementRule,
                            context.Symbol.Locations[0],
                            context.Symbol.Name));


                if (iCollisionActorType != null && namedTypeSymbol.Interfaces.Any(x => iCollisionActorType.Equals(x, SymbolEqualityComparer.Default))
                     // we do not want to trigger the diagnostics on our internal type
                     && !namedTypeSymbol.Equals(iCollisionActorGenericType, SymbolEqualityComparer.Default)
                    )
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            CollisionActorRule,
                            context.Symbol.Locations[0],
                            context.Symbol.Name));
            }
            else if (context.Symbol is IMethodSymbol methodSymbol)
            {
                if (methodSymbol.ReturnType.Equals(shapeType, SymbolEqualityComparer.Default))
                    shapeUsed = true;


            }
            else if (context.Symbol is IPropertySymbol propertySymbol)
            {
                if (propertySymbol.Type.Equals(shapeType, SymbolEqualityComparer.Default))
                    shapeUsed = true;
            }
            else if (context.Symbol is IFieldSymbol fieldSymbol)
            {
                if (fieldSymbol.Type.Equals(shapeType, SymbolEqualityComparer.Default))
                    shapeUsed = true;
            }
            else if (context.Symbol is IParameterSymbol parameterSymbol)
            {
                if (parameterSymbol.Type.Equals(shapeType, SymbolEqualityComparer.Default))
                    shapeUsed = true;
            }

            if (shapeUsed)
                // For all such symbols, report a diagnostic.
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        UseRule,
                        context.Symbol.Locations[0],
                        context.Symbol.Name));


        }
    }


}
