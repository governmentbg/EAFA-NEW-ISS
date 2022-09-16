
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { BuyerPremiseUsageDocumentDTO } from './BuyerPremiseUsageDocumentDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';

export class PremisesDTO {
    public constructor(obj?: Partial<PremisesDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(BuyerPremiseUsageDocumentDTO)
    public document?: BuyerPremiseUsageDocumentDTO;

    @StrictlyTyped(String)
    public landlord?: string;

    @StrictlyTyped(String)
    public egnEik?: string;

    @StrictlyTyped(AddressRegistrationDTO)
    public address?: AddressRegistrationDTO;
}