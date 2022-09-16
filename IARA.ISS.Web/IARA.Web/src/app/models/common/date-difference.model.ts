export class DateDifference {
    public days: number | undefined;
    public hours: number | undefined;
    public minutes: number | undefined;
    public seconds: number | undefined;

    public constructor(obj?: Partial<DateDifference>) {
        Object.assign(this, obj);
    }
}