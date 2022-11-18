

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { QualifiedFisherRegixDataDTO } from './QualifiedFisherRegixDataDTO';
import { LetterOfAttorneyDTO } from './LetterOfAttorneyDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { ApplicationPaymentInformationDTO } from './ApplicationPaymentInformationDTO';
import { ApplicationBaseDeliveryDTO } from './ApplicationBaseDeliveryDTO';

export class QualifiedFisherApplicationEditDTO extends QualifiedFisherRegixDataDTO {
    public constructor(obj?: Partial<QualifiedFisherApplicationEditDTO>) {
        if (obj != undefined) {
            super(obj as QualifiedFisherRegixDataDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(LetterOfAttorneyDTO)
    public letterOfAttorney?: LetterOfAttorneyDTO;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public egn?: string;

    @StrictlyTyped(Boolean)
    public hasExam?: boolean;

    @StrictlyTyped(Number)
    public examTerritoryUnitId?: number;

    @StrictlyTyped(String)
    public comments?: string;

    @StrictlyTyped(ApplicationPaymentInformationDTO)
    public paymentInformation?: ApplicationPaymentInformationDTO;

    @StrictlyTyped(ApplicationBaseDeliveryDTO)
    public deliveryData?: ApplicationBaseDeliveryDTO;

    @StrictlyTyped(Boolean)
    public isPaid?: boolean;

    @StrictlyTyped(Boolean)
    public hasDelivery?: boolean;

    @StrictlyTyped(Boolean)
    public isOnlineApplication?: boolean;

    @StrictlyTyped(QualifiedFisherRegixDataDTO)
    public regiXDataModel?: QualifiedFisherRegixDataDTO;
}