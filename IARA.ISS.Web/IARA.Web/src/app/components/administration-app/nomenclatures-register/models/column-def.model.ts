export class ColumnDef {
    public columnName: string | undefined;
    public propertyNamePascal: string | undefined;
    public propertyNameCamel: string | undefined;
    public dataType: string | undefined;
    public width: number | undefined;
    public isUnique: boolean | undefined;

    public constructor(init?: Partial<ColumnDef>) {
        Object.assign(this, init);
    }
}