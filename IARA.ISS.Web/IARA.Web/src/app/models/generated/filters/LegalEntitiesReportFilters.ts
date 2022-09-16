
import { BaseRequestModel } from '../../common/BaseRequestModel';

export class LegalEntitiesReportFilters extends BaseRequestModel {

    constructor(obj?: Partial<LegalEntitiesReportFilters>) {
        if (obj != undefined) {
            super((obj as BaseRequestModel));
            Object.assign(this, obj);
        } else {
            super();
        }
    }


    public legalName: string | undefined;
    public eik: string | undefined;
    public countryId: number | undefined;
    public populatedAreaId: number | undefined;
}