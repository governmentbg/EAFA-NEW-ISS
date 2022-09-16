export class BaseRequestModel {
    constructor(obj?: BaseRequestModel) {
        if (obj != null) {
            Object.assign((this as BaseRequestModel), (obj as BaseRequestModel));
        }

        this.freeTextSearch = "";
        this.showInactiveRecords = false;
    }

    public showInactiveRecords: boolean;
    public freeTextSearch: string;
}