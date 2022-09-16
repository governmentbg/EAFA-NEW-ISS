
${
    using Typewriter.Extensions.Types;
    using System.Text.RegularExpressions;

    Template(Settings settings)
    {
        settings.IncludeProject("IARA.DomainModels");
     // converts file names from pascal-caseDTO.cs to kebab-case.model.ts
     
     /* 
        settings.OutputFilenameFactory = file => 
        {
            string name = file.Name.Replace("DTO.cs", "");
            return Regex.Replace(
                            name,
                            "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])",
                            "-$1",
                            RegexOptions.Compiled)
                        .Trim()
                        .ToLower() + ".model.ts";
        };
     */
    }

    string BaseClassTypeArguments(Class c) 
    {
        if (c.BaseClass.IsGeneric)
        {
            return $"<{string.Join(",", c.BaseClass.TypeArguments.Select(x => x.ClassName()))}>";
        }
        if (c.BaseClass.Name == "NomenclatureDTO") 
        {
            return "<number>";
        }
        return "";
    }

    string Decorator(Property p)
    {
        var type = p.Type;
        var name = type.ClassName();

        if (type.IsEnum) 
        {
            return "@StrictlyTyped(Number)";
        }
        switch (name) 
        {
            case "string":
                return "@StrictlyTyped(String)";    
            case "number":
                return "@StrictlyTyped(Number)";
            case "boolean":
                return "@StrictlyTyped(Boolean)";
        }
        if (name == "IFormFile")
        {
            name = "File";
        }

        return $"@StrictlyTyped({name})";
    }

    string PropertyType(Property p) 
    {
        var type = p.Type;
        switch (type.Name)
        {
            case "IFormFile":
                return "File";
            case "IFormFile[]":
                return "File[]";
        }
        
        return type.Name.Replace("NomenclatureDTO", "NomenclatureDTO<number>");
    }

    bool ShouldIncludeClass(Class c)
    {
        return c.Name.EndsWith("DTO") 
           && !c.Name.Contains("Mobile") 
           && !c.FullName.Contains("Mobile") 
           && c.Properties.Any() 
           && c.BaseClass == null;
    }

    string Imports(Class c)
    {
        List<string> exclude = new List<string> { "IFormFile" };

        List<string> modelImports = new List<string>(); 
        List<string> enumImports = new List<string>();

        if (c.BaseClass != null)
        {
            modelImports.Add(c.BaseClass.Name);
        }

        var modelTypes = c.Properties.Select(p => p.Type).Where(t => !t.IsPrimitive).ToHashSet();
        var enumTypes = c.Properties.Select(p => p.Type).Where(t => t.IsEnum).ToHashSet();

        modelImports.AddRange(modelTypes
            .Where(t => t.ClassName() != c.Name && !exclude.Contains(t.ClassName())) // exclude custom types
            .Where(t => !c.TypeArguments.Select(x => x.Name).Contains(t.Name)) // exclude generic params
            .Select(t => t.ClassName())
            .Distinct());

        enumImports.AddRange(enumTypes.Where(t => t.IsEnum).Select(t => t.Name).Distinct());

        string typesRes = string.Join(Environment.NewLine, modelImports.Select(t => t != "NomenclatureDTO" 
                                                            ? $"import {{ {t} }} from './{t}';"
                                                            : "import { NomenclatureDTO } from './GenericNomenclatureDTO';"));

        string enumsRes = string.Join(Environment.NewLine, enumImports.Select(t => 
            $"import {{ {t} }} from '@app/enums/" 
            + $"{Regex.Replace(t.Replace("Enum", ""), "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", "-$1", RegexOptions.Compiled).Trim().ToLower()}"
            + $".enum';")
        );

        string result = "import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';";
        if (!string.IsNullOrEmpty(typesRes)) 
        {
            result = $"{result}{Environment.NewLine}{typesRes}";
        }
        if (!string.IsNullOrEmpty(enumsRes))
        {
            result = $"{result}{Environment.NewLine}{enumsRes}";
        }
        return result;
    }
}$Classes(c => ShouldIncludeClass(c))[
$Imports

export class $Name$TypeArguments { 
    public constructor(obj?: Partial<$Name$TypeArguments>) {
        Object.assign(this, obj);
    }
$Properties[
    $Decorator
    public $name?: $PropertyType;
]}]$Classes(c => c.Name.EndsWith("DTO") && c.Properties.Any() && c.BaseClass != null)[
$Imports 

export class $Name$TypeArguments extends $BaseClass$BaseClassTypeArguments {
    public constructor(obj?: Partial<$Name$TypeArguments>) {
        if (obj != undefined) {
            super(obj as $BaseClass$BaseClassTypeArguments);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  $Properties[
    $Decorator
    public $name?: $PropertyType;
]}]