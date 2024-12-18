import { PageCodeEnum } from '@app/enums/page-code.enum';
import { InspectedFishingGearTableModel } from './inspected-fishing-gear-table.model';

export class InspectedFishingGearTableParams {
    public readOnly: boolean = false;
    public isEdit: boolean = false;
    public isRegistered: boolean = false;
    public hasAttachedAppliances: boolean = false;
    public pageCode: PageCodeEnum | undefined;
    public filterTypes: boolean = false;
    public model: InspectedFishingGearTableModel | undefined;

    public constructor(params?: Partial<InspectedFishingGearTableParams>) {
        Object.assign(this, params);
    }
}