
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';

export class BuyerPremiseUsageDocumentDTO {
    public constructor(obj?: Partial<BuyerPremiseUsageDocumentDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public buyerID?: number;

    @StrictlyTyped(NomenclatureDTO)
    public docType?: NomenclatureDTO<number>;

    @StrictlyTyped(String)
    public docNum?: string;

    @StrictlyTyped(String)
    public docIssuer?: string;

    @StrictlyTyped(Date)
    public docValidFrom?: Date;

    @StrictlyTyped(Date)
    public docValidTo?: Date;

    @StrictlyTyped(Boolean)
    public isUnlimited?: boolean;

    @StrictlyTyped(Number)
    public landlordLegalId?: number;

    @StrictlyTyped(String)
    public landlordName?: string;

    @StrictlyTyped(String)
    public landlordEik?: string;

    @StrictlyTyped(AddressRegistrationDTO)
    public landlordAddress?: AddressRegistrationDTO;

    @StrictlyTyped(String)
    public comment?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}