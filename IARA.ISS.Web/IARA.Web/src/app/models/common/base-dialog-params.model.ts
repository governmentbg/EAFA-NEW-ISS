export class BaseDialogParamsModel {
    public id!: number;
    public viewMode: boolean = false;

    public constructor(obj?: Partial<BaseDialogParamsModel>) {
        Object.assign(this, obj);
    }
}