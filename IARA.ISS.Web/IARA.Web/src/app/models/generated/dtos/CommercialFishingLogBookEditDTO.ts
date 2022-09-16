

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { LogBookEditDTO } from './LogBookEditDTO';
import { ShipLogBookPageRegisterDTO } from './ShipLogBookPageRegisterDTO'; 

export class CommercialFishingLogBookEditDTO extends LogBookEditDTO {
    public constructor(obj?: Partial<CommercialFishingLogBookEditDTO>) {
        if (obj != undefined) {
            super(obj as LogBookEditDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public logBookLicenseId?: number;

    @StrictlyTyped(Number)
    public lastLogBookLicenseId?: number;

    @StrictlyTyped(Number)
    public lastPermitLicenseId?: number;

    @StrictlyTyped(Number)
    public permitLicenseStartPageNumber?: number;

    @StrictlyTyped(Number)
    public permitLicenseEndPageNumber?: number;

    @StrictlyTyped(String)
    public permitLicenseRegistrationNumber?: string;

    @StrictlyTyped(Date)
    public logBookLicenseValidForm?: Date;

    @StrictlyTyped(Date)
    public logBookLicenseValidTo?: Date;

    @StrictlyTyped(Number)
    public lastPageNumber?: number;

    @StrictlyTyped(Boolean)
    public isForRenewal?: boolean;

    @StrictlyTyped(Boolean)
    public permitLicenseIsActive?: boolean;

    @StrictlyTyped(ShipLogBookPageRegisterDTO)
    public shipPagesAndDeclarations?: ShipLogBookPageRegisterDTO[];
}