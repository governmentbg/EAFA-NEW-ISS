using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace IARA.SourceGenerators
{
    internal static class GeneratorUtils
    {
        public static string ReplaceEntityDirWithExtendedDir(this string entitiesDir)
        {
            return entitiesDir.Replace($"{Path.DirectorySeparatorChar}{DefaultConstants.ENTITIES_FOLDER}{Path.DirectorySeparatorChar}", $"{Path.DirectorySeparatorChar}{DefaultConstants.EXTENDED_ENTITIES_FOLDER}{Path.DirectorySeparatorChar}");
        }

        public static string GetSchema(this ClassContext context)
        {
            Regex regex = new Regex(DefaultConstants.TABLE_ATTRIBUTE_REGEX);

            //string pattern = $"{Regex.Escape("[Table(")}([a-zA-Z]+)\",\\s{{0,1}}Schema\\s{{0,1}}=\\s{{0,1}}\"([a-zA-Z]+)\"{Regex.Escape(")]")}";

            var listAttributes = context.ClassSyntax.AttributeLists.SelectMany(x => x.Attributes).Select(x => x.ToFullString().Trim()).ToList();

            foreach (var attribute in listAttributes)
            {
                var match = regex.Match(attribute);
                if (match.Success)
                {
                    return match.Groups[2].Value;
                }
            }

            return string.Empty;
        }

        public static bool TryGetParentSyntax<T>(SyntaxNode syntaxNode, out T result)
                 where T : SyntaxNode
        {
            result = null;

            if (syntaxNode == null)
            {
                return false;
            }

            try
            {
                syntaxNode = syntaxNode.Parent;

                if (syntaxNode == null)
                {
                    return false;
                }

                if (syntaxNode.GetType() == typeof(T))
                {
                    result = syntaxNode as T;
                    return true;
                }

                return TryGetParentSyntax<T>(syntaxNode, out result);
            }
            catch
            {
                return false;
            }
        }

        public static void RemoveReadOnlyAttribute(string fullFileName)
        {
            FileAttributes attributes = File.GetAttributes(fullFileName);

            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                // Make the file RW
                attributes = RemoveAttribute(attributes, FileAttributes.ReadOnly);
                File.SetAttributes(fullFileName, attributes);
            }
        }

        public static FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
        }
    }
}
