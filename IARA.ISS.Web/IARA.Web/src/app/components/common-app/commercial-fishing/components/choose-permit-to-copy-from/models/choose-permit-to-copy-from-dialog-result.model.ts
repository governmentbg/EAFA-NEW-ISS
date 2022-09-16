export class ChoosePermitToCopyFromDialogResult {
    public permitId: number | undefined;
    public permitNumber: string | undefined;

    public constructor(obj?: Partial<ChoosePermitToCopyFromDialogResult>) {
        Object.assign(this, obj);
    }
}