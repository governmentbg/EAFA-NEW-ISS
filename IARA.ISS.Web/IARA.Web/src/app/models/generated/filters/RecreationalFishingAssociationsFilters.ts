
import { BaseRequestModel } from '../../common/BaseRequestModel';

export class RecreationalFishingAssociationsFilters extends BaseRequestModel {

    constructor(obj?: Partial<RecreationalFishingAssociationsFilters>) {
        if (obj != undefined) {
            super((obj as BaseRequestModel));
            Object.assign(this, obj);
        } else {
            super();
        }
    }


    public name: string | undefined;
    public eik: string | undefined;
    public territoryUnitId: number | undefined;
    public representativePersonId: number | undefined;
    public showCanceled: boolean | undefined;
}