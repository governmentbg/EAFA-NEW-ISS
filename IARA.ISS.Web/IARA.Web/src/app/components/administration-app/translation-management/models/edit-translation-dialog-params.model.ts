export class EditTranslationDialogParams {
    public id: number | undefined;
    public key: string | undefined;
    public viewMode: boolean = false;

    public constructor(params?: Partial<EditTranslationDialogParams>) {
        Object.assign(this, params);
    }
}