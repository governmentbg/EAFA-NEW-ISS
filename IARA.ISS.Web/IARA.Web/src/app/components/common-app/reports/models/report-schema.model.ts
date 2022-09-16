export class ReportSchema {
    public propertyName: string = '';
    public propertyDisplayName: string = '';

    public constructor(obj?: Partial<ReportSchema>) {
        Object.assign(this, obj);
    }
}