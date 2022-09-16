using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace IARA.SourceGenerators
{
    [Generator]
    public class PartialEntitySourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is EntitiesContextReceiver syntaxReceiver && syntaxReceiver.EntityClasses.Any())
            {
                var entitiesInRootFolder = syntaxReceiver.EntityClasses.Where(x => new FileInfo(x.FilePath).DirectoryName.EndsWith(DefaultConstants.ENTITIES_FOLDER)).ToList();

                List<string> entityFiles = new List<string>();

                foreach (var entityClass in entitiesInRootFolder)
                {
                    FileInfo entityInfo = new FileInfo(entityClass.FilePath);
                    if (entityInfo.Directory.Name == DefaultConstants.ENTITIES_FOLDER)
                    {
                        ChangeIsActiveField(entityClass);
                        string fullEntityName = MoveGeneratedEntityToFolder(entityClass, entityFiles);
                        GeneratePartialEntity(entityClass, out bool isFluxEntity);

                        if (isFluxEntity)
                        {
                            OverrideProperties(fullEntityName);
                        }
                    }
                }

                DeleteMissingEntities(entityFiles);
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            //Debugger.Launch();
            context.RegisterForSyntaxNotifications(() => new EntitiesContextReceiver());
        }

        private void DeleteMissingEntities(List<string> entityFiles)
        {
            List<string[]> splitedEntityFiles = new List<string[]>();

            foreach (var fileFullPath in entityFiles)
            {
                string[] splittedFilePath = fileFullPath.Split(Path.DirectorySeparatorChar);
                splitedEntityFiles.Add(splittedFilePath);
            }

            var groupedFilesByFolder = (from x in splitedEntityFiles
                                        group x by x[x.Length - 2] into grouped
                                        select new
                                        {
                                            SchemaFolder = grouped.Key,
                                            EntitiesDir = string.Join(Path.DirectorySeparatorChar.ToString(), grouped.First().Take(grouped.First().Length - 1)),
                                            ExtendedEntitiesDir = string.Join(Path.DirectorySeparatorChar.ToString(), grouped.First().Take(grouped.First().Length - 1)).ReplaceEntityDirWithExtendedDir(),
                                            EntityFiles = grouped.Select(y => string.Join(Path.DirectorySeparatorChar.ToString(), y)).ToList(),
                                            ExtendedEntityFiles = grouped.Select(y => string.Join(Path.DirectorySeparatorChar.ToString(), y).ReplaceEntityDirWithExtendedDir()).ToList()
                                        }).ToDictionary(x => new SchemaKey
                                        {
                                            SchemaFolder = x.SchemaFolder,
                                            EntitiesDir = x.EntitiesDir,
                                            ExtendedEntitiesDir = x.ExtendedEntitiesDir
                                        }, x => new SchemaFiles
                                        {
                                            EntityFiles = x.EntityFiles,
                                            ExtendedEntityFiles = x.ExtendedEntityFiles
                                        });

            foreach (var entitiesSubFolder in groupedFilesByFolder)
            {
                List<string> filesInEntityDir = Directory.GetFiles(entitiesSubFolder.Key.EntitiesDir).ToList();
                List<string> filesInExtendedDir = Directory.GetFiles(entitiesSubFolder.Key.ExtendedEntitiesDir).ToList();

                if (filesInEntityDir.Any())
                {
                    foreach (var file in filesInEntityDir.Except(entitiesSubFolder.Value.EntityFiles))
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch (Exception)
                        { }
                    }
                }

                if (filesInExtendedDir.Any())
                {
                    foreach (var file in filesInExtendedDir.Except(entitiesSubFolder.Value.ExtendedEntityFiles))
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch (Exception)
                        { }
                    }
                }
            }

        }

        private void ChangeIsActiveField(ClassContext entityClass)
        {
            if (entityClass.IsSoftDeletable())
            {
                string content = File.ReadAllText(entityClass.FilePath);
                if (!content.Contains("this.IsActive = true;"))
                {
                    Match match = Regex.Match(content, DefaultConstants.CONSTRUCTOR_PATTERN);
                    if (match.Success)
                    {
                        int lastIndex = content.LastIndexOf(match.Value) + match.Value.Length;
                        StringBuilder builder = new StringBuilder();
                        builder.Append(content.Substring(0, (lastIndex - 1)));
                        builder.AppendLine("    this.IsActive = true;");
                        builder.AppendLine("        }");
                        builder.Append(content.Substring(lastIndex, content.Length - lastIndex));
                        content = builder.ToString();
                    }
                    else
                    {
                        match = Regex.Match(content, DefaultConstants.CLASS_PATTERN);
                        if (match.Success)
                        {
                            int lastIndex = content.LastIndexOf(match.Value) + match.Value.Length;
                            StringBuilder builder = new StringBuilder();
                            builder.Append(content.Substring(0, lastIndex + 1));
                            builder.AppendLine($"" +
                                $"         public {entityClass.ClassSyntax.Identifier.ValueText}()" +
                                $"\n        {{" +
                                $"\n            this.IsActive = true;" +
                                $"\n        }}\n");

                            builder.AppendLine(content.Substring(lastIndex, content.Length - lastIndex));
                            content = builder.ToString();
                        }
                    }
                }

                content = content.Replace("public bool? IsActive { get; set; }", "public bool IsActive { get; set; }");
                File.WriteAllText(entityClass.FilePath, content);
            }
        }


        private string MoveGeneratedEntityToFolder(ClassContext entityClass, List<string> entityFiles)
        {
            string folderName = SchemaToDirectoryMapping.GetDirectoryOrDefault(entityClass.GetSchema());

            if (folderName != string.Empty)
            {
                FileInfo fileInfo = new FileInfo(entityClass.FilePath);
                string subDirectory = Path.Combine(fileInfo.Directory.FullName, folderName);
                if (!Directory.Exists(subDirectory))
                {
                    Directory.CreateDirectory(subDirectory);
                }

                string newFileLocation = Path.Combine(subDirectory, fileInfo.Name);

                entityFiles.Add(newFileLocation);

                if (File.Exists(newFileLocation))
                {
                    GeneratorUtils.RemoveReadOnlyAttribute(newFileLocation);
                    GeneratorUtils.RemoveReadOnlyAttribute(fileInfo.FullName);
                    File.WriteAllText(newFileLocation, File.ReadAllText(fileInfo.FullName));
                    File.Delete(fileInfo.FullName);
                }
                else
                {
                    File.Move(fileInfo.FullName, newFileLocation);
                }

                return newFileLocation;
            }

            return null;
        }

        private void GeneratePartialEntity(ClassContext entityClass, out bool isFluxEntity)
        {
            List<string> interfaceList = new List<string>();
            isFluxEntity = false;

            if (entityClass.IsAuditEntity())
            {
                interfaceList.Add(DefaultConstants.IAUDIT_ENTITY);
            }

            if (entityClass.IsSoftDeletable())
            {
                interfaceList.Add(DefaultConstants.ISOFTDELETABLE_ENTITY);
            }

            if (entityClass.IsNomeclature())
            {
                interfaceList.Add(DefaultConstants.INOMENCLATURE_ENTITY);

                if (entityClass.IsCodeNomeclature())
                {
                    interfaceList.Add(DefaultConstants.ICODED_ENTITY);
                }
            }

            if (entityClass.IsIdentityRecord())
            {
                interfaceList.Add(DefaultConstants.IIDENTITY_ENTITY);
            }

            if (entityClass.IsNullableApplicationRegister())
            {
                interfaceList.Add(DefaultConstants.NULLABLE_APPLICATION_REGISTER_ENTITY);
            }

            if (entityClass.IsNullableApplicationRegisterValidity())
            {
                interfaceList.Add(DefaultConstants.NULLABLE_APPLICATION_REGISTER_VALIDITY_ENTITY);
            }

            if (entityClass.IsChangeOfCircumstancesEntity())
            {
                interfaceList.Add(DefaultConstants.CHANGE_OF_CIRCUMSTANCES_ENTITY);
            }

            if (entityClass.IsApplicationEntity())
            {
                interfaceList.Add(DefaultConstants.APPLICATION_ENTITY);
            }
            else if (entityClass.IsApplicationRegister())
            {
                interfaceList.Add(DefaultConstants.APPLICATION_REGISTER_ENTITY);
            }

            if (entityClass.IsApplicationRegisterValidity())
            {
                interfaceList.Add(DefaultConstants.APPLICATION_REGISTER_VALIDITY_ENTITY);
            }

            if (entityClass.IsValidityEntity())
            {
                interfaceList.Add(DefaultConstants.IVALIDITY_ENTITY);
            }

            if (entityClass.IsFileEntity())
            {
                interfaceList.Add(string.Format(DefaultConstants.FILE_ENTITY, entityClass.ClassSyntax.Identifier.ValueText.Replace("File", "")));
            }

            if (entityClass.IsCancellableEntity())
            {
                interfaceList.Add(DefaultConstants.ICANCELLABLE_ENTITY);
            }

            if (entityClass.IsLogBookPageDecimalEntity())
            {
                interfaceList.Add(DefaultConstants.ILOGBOOK_PAGE_DECIMAL_ENTITY);
            }

            if (entityClass.IsLogBookPageStringEntity())
            {
                interfaceList.Add(DefaultConstants.ILOGBOOK_PAGE_STRING_ENTITY);
            }

            if (entityClass.IsFluxNomenclature())
            {
                isFluxEntity = true;
                interfaceList.Insert(0, DefaultConstants.FLUX_NOMENCLATURE_BASE);
            }

            if (interfaceList.Any())
            {
                //string lastNamespaceFragment = entityClass.NamespaceSyntax.Name.ToString().Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Last();
                //string filePath = $"ExtendedEntities/{lastNamespaceFragment}";

                string source = $@"using IARA.EntityModels.Interfaces;

																					 namespace {entityClass.NamespaceSyntax.Name}
																					 {{
																					 	  public partial class {entityClass.ClassSyntax.Identifier} : {string.Join(", ", interfaceList)}
																					 	  {{
																					 	  }}
																					 }}".Replace("																					 ", "");

                SourceText sourceText = SourceText.From(source, Encoding.UTF8);

                FileInfo entityFileInfo = new FileInfo(entityClass.FilePath);

                string folderName = SchemaToDirectoryMapping.GetDirectoryOrDefault(entityClass.GetSchema());

                var directoryInfo = Directory.CreateDirectory(Path.Combine(entityFileInfo.Directory.Parent.FullName, DefaultConstants.EXTENDED_ENTITIES_FOLDER, folderName));

                FileInfo file = new FileInfo(Path.Combine(directoryInfo.FullName, entityFileInfo.Name));

                if (!file.Directory.Exists)
                {
                    file.Directory.Create();
                }

                UpdateExtendendEntityFile(file, sourceText.ToString());

                //context.AddSource($"{entityClass.ClassSyntax.Identifier.ValueText}.cs", sourceText);
            }
        }

        private bool IsEntityFileModified(FileInfo file, string sourceText)
        {
            string oldSource = File.ReadAllText(file.FullName, Encoding.UTF8);
            SHA1 generator = SHA1.Create();
            byte[] oldFileHash = generator.ComputeHash(Encoding.UTF8.GetBytes(oldSource));
            byte[] newSourceHash = generator.ComputeHash(Encoding.UTF8.GetBytes(sourceText));

            for (int i = 0; i < oldFileHash.Length; i++)
            {
                if (oldFileHash[i] != newSourceHash[i])
                    return true;
            }

            return false;
        }

        private void OverrideProperties(string fullFilePath)
        {
            string content = File.ReadAllText(fullFilePath, Encoding.UTF8);
            StringBuilder stringBuilder = new StringBuilder(content);
            stringBuilder.Replace("public int Id { get; set; }", "public override int Id { get; set; }");
            stringBuilder.Replace("public string Code { get; set; }", "public override string Code { get; set; }");
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }

        private void UpdateExtendendEntityFile(FileInfo file, string sourceText)
        {
            //Debugger.Launch();

            if (!file.Exists)
            {
                using (FileStream fileStream = file.Create())
                {
                    StreamWriter writer = new StreamWriter(fileStream);
                    writer.Write(sourceText);
                    writer.Flush();
                    writer.Close();
                    fileStream.Close();
                }
            }
            else if (IsEntityFileModified(file, sourceText))
            {
                GeneratorUtils.RemoveReadOnlyAttribute(file.FullName);

                using (FileStream fileStream = file.Open(FileMode.Truncate, FileAccess.Write, FileShare.None))
                {
                    StreamWriter writer = new StreamWriter(fileStream);
                    writer.Write(sourceText);
                    writer.Flush();
                    writer.Close();
                    fileStream.Close();
                }
            }
        }


    }
}
