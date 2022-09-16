using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IARA.SourceGenerators
{
    public class ClassContext
    {
        public ClassContext(ClassDeclarationSyntax @class, NamespaceDeclarationSyntax @namespace, string filePath)
        {
            this.FilePath = filePath;
            this.ClassSyntax = @class;
            this.NamespaceSyntax = @namespace;
            this.PropertiesSyntax = this.ClassSyntax
                                        .Members
                                        .Where(x => x.Kind() == SyntaxKind.PropertyDeclaration)
                                        .Cast<PropertyDeclarationSyntax>()
                                        .ToList();
        }

        public string FilePath { get; private set; }
        public ClassDeclarationSyntax ClassSyntax { get; private set; }
        public NamespaceDeclarationSyntax NamespaceSyntax { get; private set; }

        public List<PropertyDeclarationSyntax> PropertiesSyntax { get; private set; }

        public bool IsSoftDeletable()
        {
            return this.HasProperty("IsActive");
        }

        public bool IsFileEntity()
        {
            var properties = new string[] { "Id", "RecordId", "FileId", "FileTypeId", "Record", "File" };

            return this.HasProperties(properties);
        }

        public bool IsCancellableEntity()
        {
            var properties = new string[] { "CancellationDetailsId", "CancellationDetails" };

            return this.HasProperties(properties);
        }

        public bool IsIdentityRecord()
        {
            return this.HasProperty("Id");
        }

        public bool IsAuditEntity()
        {
            var properties = new string[] { "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn" };

            return HasProperties(properties);
        }

        public bool IsValidityEntity()
        {
            var properties = new string[] { "ValidFrom", "ValidTo" };

            return this.HasProperties(properties);
        }

        public bool IsNomeclature()
        {
            var properties = new string[] { "Id", "Name" };

            return this.ClassStartsWith(this.ClassSyntax, "N") && this.HasProperties(properties);
        }

        public bool IsCodeNomeclature()
        {
            var properties = new string[] { "Id", "Name", "Code" };
            return this.ClassStartsWith(this.ClassSyntax, "N") && this.HasProperties(properties);
        }

        public bool IsApplicationRegister()
        {
            var properties = new string[] { "Id", "ApplicationId", "RegisterApplicationId", "RecordType", "IsActive" };

            return this.HasProperties(properties) && this.IsPropertyNullable("RegisterApplicationId") && !this.IsPropertyNullable("ApplicationId");
        }

        public bool IsApplicationRegisterValidity()
        {
            var properties = new string[] { "Id", "ApplicationId", "RegisterApplicationId", "RecordType", "ValidFrom", "ValidTo" };

            return this.HasProperties(properties) && this.IsPropertyNullable("RegisterApplicationId") && !this.IsPropertyNullable("ApplicationId");
        }

        public bool IsNullableApplicationRegister()
        {
            var properties = new string[] { "Id", "ApplicationId", "RegisterApplicationId", "RecordType", "IsActive" };

            return this.HasProperties(properties) && this.IsPropertyNullable("RegisterApplicationId") && this.IsPropertyNullable("ApplicationId");
        }

        public bool IsFluxNomenclature()
        {
            var properties = new string[] { "Id", "Code" };

            return this.HasProperties(properties) && this.GetSchema().StartsWith("FLUX_");
        }

        public bool IsChangeOfCircumstancesEntity()
        {
            var properties = new string[] {
                                              "Id",
                                              "ChangeOfCircumstancesTypeId",
                                              "ApplicationId"
                                          };

            return this.HasProperties(properties);
        }

        public bool IsApplicationEntity()
        {
            var properties = new string[] {
                                              "Id",
                                              "RecordType",
                                              "ApplicationId",
                                              "SubmittedForPersonId",
                                              "SubmittedForPerson",
                                              "SubmittedForLegalId",
                                              "SubmittedForLegal"
                                          };

            return this.HasProperties(properties) && this.IsPropertyNullable("RegisterApplicationId") && !this.IsPropertyNullable("ApplicationId");
        }

        public bool IsNullableApplicationRegisterValidity()
        {
            var properties = new string[] { "Id", "ApplicationId", "RegisterApplicationId", "RecordType", "ValidFrom", "ValidTo" };

            return this.HasProperties(properties) && this.IsPropertyNullable("RegisterApplicationId") && this.IsPropertyNullable("ApplicationId");
        }

        public bool IsLogBookPageDecimalEntity()
        {
            var properties = new string[] { "Id", "LogBookId", "Status", "PageNum" };
            return this.HasProperties(properties) && this.IsPropertyDecimal("PageNum");
        }

        public bool IsLogBookPageStringEntity()
        {
            var properties = new string[] { "Id", "LogBookId", "Status", "PageNum" };
            return this.HasProperties(properties) && this.IsPropertyString("PageNum");
        }

        private bool HasProperties(IReadOnlyCollection<string> properties)
        {
            return this.PropertiesSyntax
                            .Select(x => x.Identifier.ValueText)
                            .Intersect(properties)
                            .Count() == properties.Count;
        }

        private bool HasProperty(string propertyName)
        {
            return this.PropertiesSyntax.Any(x => x.Identifier.ValueText == propertyName);
        }

        private bool IsPropertyNullable(string propertyName)
        {
            //Debugger.Launch();
            return this.PropertiesSyntax.First(x => x.Identifier.ValueText == propertyName).Type is NullableTypeSyntax;
        }

        private bool IsPropertyDecimal(string propertyName)
        {
            TypeSyntax type = this.PropertiesSyntax.First(x => x.Identifier.ValueText == propertyName).Type;
            return type is PredefinedTypeSyntax predefinedType && predefinedType.Keyword.Kind() == SyntaxKind.DecimalKeyword;
        }

        private bool IsPropertyString(string propertyName)
        {
            TypeSyntax type = this.PropertiesSyntax.First(x => x.Identifier.ValueText == propertyName).Type;
            return type is PredefinedTypeSyntax predefinedType && predefinedType.Keyword.Kind() == SyntaxKind.StringKeyword;
        }

        private bool ClassStartsWith(ClassDeclarationSyntax classContext, string searchValue)
        {
            return classContext.Identifier.ValueText.StartsWith(searchValue);
        }
    }
}
