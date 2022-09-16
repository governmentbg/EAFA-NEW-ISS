export class ReasonData {
    public recordId: number | undefined;
    public reason: string | undefined;

    constructor(obj?: Partial<ReasonData>) {
        Object.assign(this, obj);
    }
}