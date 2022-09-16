import { BaseRequestModel } from '../../common/BaseRequestModel';

export class NewsManagementFilters extends BaseRequestModel {

    constructor(obj?: Partial<NewsManagementFilters>) {
        if (obj != undefined) {
            super((obj as BaseRequestModel));
            Object.assign(this, obj);
        } else {
            super();
        }
    }

    public title: string | undefined;
    public content: string | undefined;
    public dateFrom: Date | undefined;
    public dateTo: Date | undefined;
    public isPublished: boolean | undefined;
}