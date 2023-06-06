export class EditReportParamsModel {
    public id: number | undefined;
    public viewMode: boolean = false;
    public isAdd: boolean = false;
    public isCopy: boolean = false;

    public constructor(obj?: Partial<EditReportParamsModel>) {
        Object.assign(this, obj);
    }
}