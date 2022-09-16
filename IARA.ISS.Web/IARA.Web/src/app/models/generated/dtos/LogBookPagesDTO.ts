

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ShipLogBookPageRegisterDTO } from './ShipLogBookPageRegisterDTO';
import { FirstSaleLogBookPageRegisterDTO } from './FirstSaleLogBookPageRegisterDTO';
import { AdmissionLogBookPageRegisterDTO } from './AdmissionLogBookPageRegisterDTO';
import { TransportationLogBookPageRegisterDTO } from './TransportationLogBookPageRegisterDTO';
import { AquacultureLogBookPageRegisterDTO } from './AquacultureLogBookPageRegisterDTO';
import { FishInformationDTO } from './FishInformationDTO';

export class LogBookPagesDTO { 
    public constructor(obj?: Partial<LogBookPagesDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(ShipLogBookPageRegisterDTO)
    public shipPages?: ShipLogBookPageRegisterDTO[];

    @StrictlyTyped(FirstSaleLogBookPageRegisterDTO)
    public firstSalePages?: FirstSaleLogBookPageRegisterDTO[];

    @StrictlyTyped(AdmissionLogBookPageRegisterDTO)
    public admissionPages?: AdmissionLogBookPageRegisterDTO[];

    @StrictlyTyped(TransportationLogBookPageRegisterDTO)
    public transportationPages?: TransportationLogBookPageRegisterDTO[];

    @StrictlyTyped(AquacultureLogBookPageRegisterDTO)
    public aquaculturePages?: AquacultureLogBookPageRegisterDTO[];

    @StrictlyTyped(FishInformationDTO)
    public unloadingInformation?: FishInformationDTO[];

    @StrictlyTyped(FishInformationDTO)
    public firstSaleProductInformation?: FishInformationDTO[];

    @StrictlyTyped(FishInformationDTO)
    public admissionProductInformation?: FishInformationDTO[];

    @StrictlyTyped(FishInformationDTO)
    public transportationProductInformation?: FishInformationDTO[];

    @StrictlyTyped(FishInformationDTO)
    public aquacultureProductInformation?: FishInformationDTO[];
}