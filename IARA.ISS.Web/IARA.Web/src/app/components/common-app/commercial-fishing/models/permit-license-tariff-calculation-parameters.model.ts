import { PageCodeEnum } from '@app/enums/page-code.enum';
import { FishingGearDTO } from '@app/models/generated/dtos/FishingGearDTO';

export class PermitLicenseTariffCalculationParameters {
    public applicationId: number | undefined;
    public pageCode: PageCodeEnum | undefined;
    public shipId: number | undefined;
    public waterTypeCode: string | undefined;
    public aquaticOrganismTypeIds: number[] | undefined;
    public fishingGears: FishingGearDTO[] | undefined;
    public poundNetId: number | undefined;
    public excludedTariffsIds: number[] | undefined;

    public constructor(obj?: Partial<PermitLicenseTariffCalculationParameters>) {
        Object.assign(this, obj);
    }
}