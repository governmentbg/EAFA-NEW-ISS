

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ScientificFishingPermitBaseRegixDataDTO } from './ScientificFishingPermitBaseRegixDataDTO';
import { LetterOfAttorneyDTO } from './LetterOfAttorneyDTO';
import { ScientificFishingPermitHolderDTO } from './ScientificFishingPermitHolderDTO';
import { ApplicationBaseDeliveryDTO } from './ApplicationBaseDeliveryDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { ScientificFishingPermitRegixDataDTO } from './ScientificFishingPermitRegixDataDTO'; 

export class ScientificFishingApplicationEditDTO extends ScientificFishingPermitBaseRegixDataDTO {
    public constructor(obj?: Partial<ScientificFishingApplicationEditDTO>) {
        if (obj != undefined) {
            super(obj as ScientificFishingPermitBaseRegixDataDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Boolean)
    public isOnlineApplication?: boolean;

    @StrictlyTyped(String)
    public eventisNum?: string;

    @StrictlyTyped(Date)
    public registrationDate?: Date;

    @StrictlyTyped(Date)
    public validFrom?: Date;

    @StrictlyTyped(Date)
    public validTo?: Date;

    @StrictlyTyped(Number)
    public permitReasonsIds?: number[];

    @StrictlyTyped(String)
    public requesterPosition?: string;

    @StrictlyTyped(LetterOfAttorneyDTO)
    public requesterLetterOfAttorney?: LetterOfAttorneyDTO;

    @StrictlyTyped(ScientificFishingPermitHolderDTO)
    public holders?: ScientificFishingPermitHolderDTO[];

    @StrictlyTyped(Date)
    public researchPeriodFrom?: Date;

    @StrictlyTyped(Date)
    public researchPeriodTo?: Date;

    @StrictlyTyped(String)
    public researchWaterArea?: string;

    @StrictlyTyped(String)
    public researchGoalsDescription?: string;

    @StrictlyTyped(String)
    public fishTypesDescription?: string;

    @StrictlyTyped(String)
    public fishTypesApp4ZBRDesc?: string;

    @StrictlyTyped(String)
    public fishTypesCrayFish?: string;

    @StrictlyTyped(String)
    public fishingGearDescription?: string;

    @StrictlyTyped(Boolean)
    public isShipRegistered?: boolean;

    @StrictlyTyped(Number)
    public shipId?: number;

    @StrictlyTyped(String)
    public shipName?: string;

    @StrictlyTyped(String)
    public shipExternalMark?: string;

    @StrictlyTyped(String)
    public shipCaptainName?: string;

    @StrictlyTyped(Boolean)
    public hasDelivery?: boolean;

    @StrictlyTyped(ApplicationBaseDeliveryDTO)
    public deliveryData?: ApplicationBaseDeliveryDTO;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];

    @StrictlyTyped(ScientificFishingPermitRegixDataDTO)
    public regiXDataModel?: ScientificFishingPermitRegixDataDTO;
}