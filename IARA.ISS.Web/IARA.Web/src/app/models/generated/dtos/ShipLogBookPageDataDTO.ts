

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class ShipLogBookPageDataDTO { 
    public constructor(obj?: Partial<ShipLogBookPageDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public shipName?: string;

    @StrictlyTyped(String)
    public permitLicence?: string;
}