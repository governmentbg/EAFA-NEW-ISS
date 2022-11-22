export class PrintConfigurationParameters {
    public userId: number | undefined;
    public position: string | undefined;

    public constructor(obj?: Partial<PrintConfigurationParameters>) {
        Object.assign(this, obj);
    }
}