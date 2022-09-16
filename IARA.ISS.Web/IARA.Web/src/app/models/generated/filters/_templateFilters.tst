${
    Template(Settings settings)
    {
        settings.IncludeProject("IARA.DomainModels");
    }
}
$Classes(c=>c.Name.EndsWith("Filters") && !c.IsGeneric && c.Properties.Any() && c.BaseClass==null)[
export class $Name { 

  constructor(obj?: Partial<$Name>) {
    Object.assign(this, obj);
  }

$Properties[
  public $name: $Type | undefined;]
}]$Classes(c => c.Name.EndsWith("Filters") && !c.IsGeneric && c.Properties.Any() && c.BaseClass != null)[
import { $BaseClass } from '@app/models/common/$BaseClass[$Name]';

export class $Name extends $BaseClass {

    constructor(obj?: Partial<$Name>) {
      if (obj != undefined) { 
        super((obj as $BaseClass));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    $Properties[
    public $name: $Type | undefined;]
}]$Classes(c => c.Name.EndsWith("Filters") && c.IsGeneric && c.Properties.Any() && c.BaseClass == null)[
export class $Name$TypeParameters {

  constructor(obj?: Partial<$Name$TypeParameters>) {
    Object.assign(this, obj);
  }

  $Properties[
  public $name: $Type | undefined;]
}]$Classes(c => c.Name.EndsWith("Filters") && c.IsGeneric && c.Properties.Any() && c.BaseClass != null && !c.BaseClass.IsGeneric)[
export class $Name$TypeParameters extends $BaseClass {

  constructor(obj?: Partial<$Name$TypeParameters>) {
    if (obj != undefined) {
        super((obj as $BaseClass));
        Object.assign(this, obj);
      } else {
        super();
      }
  }

  $Properties[
  public $name: $Type | undefined;]
}]