

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixLegalDataDTO } from './RegixLegalDataDTO';
import { ScientificFishingPermitHolderDTO } from './ScientificFishingPermitHolderDTO';
import { ScientificFishingOutingDTO } from './ScientificFishingOutingDTO';
import { CancellationDetailsDTO } from './CancellationDetailsDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { ScientificPermitStatusEnum } from '@app/enums/scientific-permit-status.enum';

export class ScientificFishingPermitEditDTO { 
    public constructor(obj?: Partial<ScientificFishingPermitEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Boolean)
    public isOnlineApplication?: boolean;

    @StrictlyTyped(String)
    public eventisNum?: string;

    @StrictlyTyped(Number)
    public permitStatus?: ScientificPermitStatusEnum;

    @StrictlyTyped(Date)
    public registrationDate?: Date;

    @StrictlyTyped(Date)
    public validFrom?: Date;

    @StrictlyTyped(Date)
    public validTo?: Date;

    @StrictlyTyped(Number)
    public permitReasonsIds?: number[];

    @StrictlyTyped(Number)
    public permitLegalReasonsIds?: number[];

    @StrictlyTyped(RegixLegalDataDTO)
    public receiver?: RegixLegalDataDTO;

    @StrictlyTyped(ScientificFishingPermitHolderDTO)
    public holders?: ScientificFishingPermitHolderDTO[];

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

    @StrictlyTyped(ScientificFishingOutingDTO)
    public outings?: ScientificFishingOutingDTO[];

    @StrictlyTyped(String)
    public coordinationCommittee?: string;

    @StrictlyTyped(String)
    public coordinationLetterNo?: string;

    @StrictlyTyped(String)
    public coordinationComments?: string;

    @StrictlyTyped(Date)
    public coordinationDate?: Date;

    @StrictlyTyped(CancellationDetailsDTO)
    public cancellationDetails?: CancellationDetailsDTO;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}