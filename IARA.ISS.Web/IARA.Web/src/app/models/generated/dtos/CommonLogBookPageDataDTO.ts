

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PossibleLogBooksForPageDTO } from './PossibleLogBooksForPageDTO';

export class CommonLogBookPageDataDTO { 
    public constructor(obj?: Partial<CommonLogBookPageDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(PossibleLogBooksForPageDTO)
    public possibleLogBooks?: PossibleLogBooksForPageDTO[];

    @StrictlyTyped(Number)
    public originDeclarationId?: number;

    @StrictlyTyped(String)
    public originDeclarationNumber?: string;

    @StrictlyTyped(Date)
    public originDeclarationDate?: Date;

    @StrictlyTyped(Number)
    public transportationDocumentId?: number;

    @StrictlyTyped(Number)
    public transportationDocumentNumber?: number;

    @StrictlyTyped(Date)
    public transportationDocumentDate?: Date;

    @StrictlyTyped(Number)
    public admissionDocumentId?: number;

    @StrictlyTyped(Number)
    public admissionDocumentNumber?: number;

    @StrictlyTyped(Date)
    public admissionHandoverDate?: Date;

    @StrictlyTyped(Number)
    public shipId?: number;

    @StrictlyTyped(String)
    public captainName?: string;

    @StrictlyTyped(String)
    public unloadingInformation?: string;

    @StrictlyTyped(Boolean)
    public isImportNotByShip?: boolean;

    @StrictlyTyped(String)
    public placeOfImport?: string;

    @StrictlyTyped(Boolean)
    public hasAvailableProducts?: boolean;

    @StrictlyTyped(String)
    public vendorName?: string;
}