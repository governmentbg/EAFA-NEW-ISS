

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { CatchRecordDTO } from './CatchRecordDTO';
import { OriginDeclarationFishDTO } from './OriginDeclarationFishDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { LogBookPageStatusesEnum } from '@app/enums/log-book-page-statuses.enum';
import { WaterTypesEnum } from '@app/enums/water-types.enum';

export class ShipLogBookPageEditDTO { 
    public constructor(obj?: Partial<ShipLogBookPageEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public pageNumber?: string;

    @StrictlyTyped(Number)
    public permitLicenseId?: number;

    @StrictlyTyped(String)
    public permitLicenseNumber?: string;

    @StrictlyTyped(Number)
    public statusCode?: LogBookPageStatusesEnum;

    @StrictlyTyped(String)
    public status?: string;

    @StrictlyTyped(Number)
    public logBookPermitLicenseId?: number;

    @StrictlyTyped(String)
    public permitLicenseName?: string;

    @StrictlyTyped(Number)
    public permitLicenseWaterType?: WaterTypesEnum;

    @StrictlyTyped(String)
    public permitLicenseWaterTypeName?: string;

    @StrictlyTyped(Number)
    public permitLicenseAquaticOrganismTypeIds?: number[];

    @StrictlyTyped(Number)
    public logBookId?: number;

    @StrictlyTyped(Number)
    public logBookTypeId?: number;

    @StrictlyTyped(Date)
    public fillDate?: Date;

    @StrictlyTyped(Date)
    public iaraAcceptanceDateTime?: Date;

    @StrictlyTyped(Number)
    public shipId?: number;

    @StrictlyTyped(String)
    public shipName?: string;

    @StrictlyTyped(Number)
    public fishingGearRegisterId?: number;

    @StrictlyTyped(Number)
    public fishingGearCount?: number;

    @StrictlyTyped(Number)
    public fishingGearHookCount?: number;

    @StrictlyTyped(Number)
    public partnerShipId?: number;

    @StrictlyTyped(Date)
    public fishTripStartDateTime?: Date;

    @StrictlyTyped(Date)
    public fishTripEndDateTime?: Date;

    @StrictlyTyped(Number)
    public departurePortId?: number;

    @StrictlyTyped(Number)
    public arrivalPortId?: number;

    @StrictlyTyped(Date)
    public unloadDateTime?: Date;

    @StrictlyTyped(Number)
    public unloadPortId?: number;

    @StrictlyTyped(Boolean)
    public allCatchIsTransboarded?: boolean;

    @StrictlyTyped(CatchRecordDTO)
    public catchRecords?: CatchRecordDTO[];

    @StrictlyTyped(Number)
    public originDeclarationId?: number;

    @StrictlyTyped(OriginDeclarationFishDTO)
    public originDeclarationFishes?: OriginDeclarationFishDTO[];

    @StrictlyTyped(Boolean)
    public hasNoUnloadedCatch?: boolean;

    @StrictlyTyped(Boolean)
    public needRelatedLogBookPage?: boolean;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}