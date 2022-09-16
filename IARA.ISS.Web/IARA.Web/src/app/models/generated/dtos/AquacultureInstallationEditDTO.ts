

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AquacultureInstallationDTO } from './AquacultureInstallationDTO';
import { AquacultureInstallationBasinDTO } from './AquacultureInstallationBasinDTO';
import { AquacultureInstallationNetCageDTO } from './AquacultureInstallationNetCageDTO';
import { AquacultureInstallationAquariumDTO } from './AquacultureInstallationAquariumDTO';
import { AquacultureInstallationCollectorDTO } from './AquacultureInstallationCollectorDTO';
import { AquacultureInstallationRaftDTO } from './AquacultureInstallationRaftDTO';
import { AquacultureInstallationDamDTO } from './AquacultureInstallationDamDTO';
import { AquacultureInstallationRecirculatorySystemDTO } from './AquacultureInstallationRecirculatorySystemDTO';
import { AquacultureInstallationTypeEnum } from '@app/enums/aquaculture-installation-type.enum'; 

export class AquacultureInstallationEditDTO extends AquacultureInstallationDTO {
    public constructor(obj?: Partial<AquacultureInstallationEditDTO>) {
        if (obj != undefined) {
            super(obj as AquacultureInstallationDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public installationType?: AquacultureInstallationTypeEnum;

    @StrictlyTyped(AquacultureInstallationBasinDTO)
    public basins?: AquacultureInstallationBasinDTO[];

    @StrictlyTyped(AquacultureInstallationNetCageDTO)
    public netCages?: AquacultureInstallationNetCageDTO[];

    @StrictlyTyped(AquacultureInstallationAquariumDTO)
    public aquariums?: AquacultureInstallationAquariumDTO;

    @StrictlyTyped(AquacultureInstallationCollectorDTO)
    public collectors?: AquacultureInstallationCollectorDTO[];

    @StrictlyTyped(AquacultureInstallationRaftDTO)
    public rafts?: AquacultureInstallationRaftDTO[];

    @StrictlyTyped(AquacultureInstallationDamDTO)
    public dams?: AquacultureInstallationDamDTO;

    @StrictlyTyped(AquacultureInstallationRecirculatorySystemDTO)
    public recirculatorySystems?: AquacultureInstallationRecirculatorySystemDTO[];

    @StrictlyTyped(String)
    public comments?: string;
}