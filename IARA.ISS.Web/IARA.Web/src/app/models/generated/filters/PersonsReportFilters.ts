
import { BaseRequestModel } from '../../common/BaseRequestModel';

export class PersonsReportFilters extends BaseRequestModel {

    constructor(obj?: Partial<PersonsReportFilters>) {
        if (obj != undefined) {
            super((obj as BaseRequestModel));
            Object.assign(this, obj);
        } else {
            super();
        }
    }


    public firstName: string | undefined;
    public middleName: string | undefined;
    public lastName: string | undefined;
    public egn: string | undefined;
    public countryId: number | undefined;
    public populatedAreaId: number | undefined;
}