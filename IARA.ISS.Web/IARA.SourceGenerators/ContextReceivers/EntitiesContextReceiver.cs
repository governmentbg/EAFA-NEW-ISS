using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IARA.SourceGenerators
{
    internal class EntitiesContextReceiver : ISyntaxReceiver
    {
        public EntitiesContextReceiver()
        {
            entityClasses = new List<ClassContext>();
        }

        private List<ClassContext> entityClasses;
        public IReadOnlyList<ClassContext> EntityClasses { get { return entityClasses; } }

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax cds
                && GeneratorUtils.TryGetParentSyntax(cds, out NamespaceDeclarationSyntax nds)
                && nds.Name.ToString().StartsWith(DefaultConstants.ENTITIES_NAMESPACE))
            {
                entityClasses.Add(new ClassContext(cds, nds, syntaxNode.SyntaxTree.FilePath));
            }
        }


    }
}
