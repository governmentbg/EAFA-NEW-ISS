export class FileInApplicationDialogParams {
    public applicationId: number | undefined;

    public constructor(obj?: Partial<FileInApplicationDialogParams>) {
        Object.assign(this, obj);
    }
}