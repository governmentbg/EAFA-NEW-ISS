using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IARA.SourceGenerators.ContextReceivers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IARA.SourceGenerators
{
    [Generator]
    public class IARADbContextSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is DbContextReceiver syntaxReceiver)
            {
                if (syntaxReceiver.BaseIARADbClassContext != null)
                {
                    RemoveIsActiveDefaultSqlValue(syntaxReceiver.BaseIARADbClassContext);

                    if (syntaxReceiver.IARADbClassContext != null)
                    {
                        //AddExtendedEntities(syntaxReceiver.BaseIARADbClassContext, syntaxReceiver.IARADbClassContext.FilePath);
                    }
                }
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            //Debugger.Launch();
            context.RegisterForSyntaxNotifications(() => new DbContextReceiver());
        }

        private static string IsActive = @"entity.Property(e => e.IsActive)
                    .HasDefaultValueSql(""true"")";


        private void RemoveIsActiveDefaultSqlValue(ClassContext entityClass)
        {
            string content = File.ReadAllText(entityClass.FilePath);

            if (content.Contains(IsActive))
            {
                content = content.Replace(IsActive, "entity.Property(e => e.IsActive)");
                //content = Regex.Replace(content, $"({IsActive})", "");
                GeneratorUtils.RemoveReadOnlyAttribute(entityClass.FilePath);
                File.WriteAllText(entityClass.FilePath, content);
            }
        }

        private void AddExtendedEntities(ClassContext dbContext, string fileFullPath)
        {
            GeneratorUtils.RemoveReadOnlyAttribute(fileFullPath);

            List<string> dbSets = new List<string>();


            var genericProperties = dbContext.PropertiesSyntax.Where(x => x.Type is GenericNameSyntax).Select(x => new
            {
                PropertyType = (x.Type as GenericNameSyntax).Identifier.ValueText,
                PropertyName = x.Identifier.ValueText,
                TypeArgument = ((x.Type as GenericNameSyntax).TypeArgumentList.Arguments[0] as IdentifierNameSyntax).Identifier.ValueText
            });

            StringBuilder propertiesBuilder = new StringBuilder();
            propertiesBuilder.AppendLine(DefaultConstants.EXTENDED_DB_CONTEXT_CLASS);

            foreach (var dbSet in genericProperties.Where(x => x.PropertyType == "DbSet"))
            {
                propertiesBuilder.AppendLine($"        public DbSet<{dbSet.TypeArgument}> {dbSet.PropertyName} {{ get {{ return dbContext.{dbSet.PropertyName}; }} }}");
            }

            propertiesBuilder.AppendLine("}");
            propertiesBuilder.AppendLine("}");

            File.WriteAllText(fileFullPath, propertiesBuilder.ToString());
        }
    }
}
