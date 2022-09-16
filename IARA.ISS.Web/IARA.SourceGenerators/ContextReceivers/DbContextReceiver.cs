using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IARA.SourceGenerators.ContextReceivers
{
    internal class DbContextReceiver : ISyntaxReceiver
    {
        public ClassContext BaseIARADbClassContext { get; private set; }
        public ClassContext IARADbClassContext { get; private set; }

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            //Debugger.Launch();
            if (syntaxNode is ClassDeclarationSyntax cds
            && cds.Identifier.ValueText == DefaultConstants.BASE_DB_CONTEXT
            && GeneratorUtils.TryGetParentSyntax(cds, out NamespaceDeclarationSyntax nds)
            && Path.GetFileName(syntaxNode.SyntaxTree.FilePath) == "BaseIARADbContext.cs")
            {
                BaseIARADbClassContext = new ClassContext(cds, nds, syntaxNode.SyntaxTree.FilePath);
            }
            else if (syntaxNode is ClassDeclarationSyntax cds1
               && cds1.Identifier.ValueText == DefaultConstants.DB_CONTEXT
               && GeneratorUtils.TryGetParentSyntax(cds1, out nds))
            {
                string fileName = Path.GetFileName(syntaxNode.SyntaxTree.FilePath);

                if (!string.IsNullOrEmpty(fileName))
                {
                    fileName = fileName.Replace(Path.GetExtension(fileName), "");

                    if (fileName == DefaultConstants.EXTENDED_DB_CONTEXT)
                    {
                        IARADbClassContext = new ClassContext(cds1, nds, syntaxNode.SyntaxTree.FilePath);
                    }
                }
            }
        }
    }
}
