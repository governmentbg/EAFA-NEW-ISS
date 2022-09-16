

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ApplicationSubmittedForDTO } from './ApplicationSubmittedForDTO';
import { StatisticalFormsSeaDaysDTO } from './StatisticalFormsSeaDaysDTO';
import { StatisticalFormEmployeeInfoGroupDTO } from './StatisticalFormEmployeeInfoGroupDTO';
import { StatisticalFormNumStatGroupDTO } from './StatisticalFormNumStatGroupDTO';
import { FileInfoDTO } from './FileInfoDTO';

export class StatisticalFormFishVesselEditDTO { 
    public constructor(obj?: Partial<StatisticalFormFishVesselEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(ApplicationSubmittedForDTO)
    public submittedFor?: ApplicationSubmittedForDTO;

    @StrictlyTyped(Boolean)
    public isOnlineApplication?: boolean;

    @StrictlyTyped(String)
    public submittedByWorkPosition?: string;

    @StrictlyTyped(Number)
    public shipId?: number;

    @StrictlyTyped(Number)
    public year?: number;

    @StrictlyTyped(String)
    public formNum?: string;

    @StrictlyTyped(Number)
    public shipYears?: number;

    @StrictlyTyped(Number)
    public shipPrice?: number;

    @StrictlyTyped(Number)
    public shipLengthId?: number;

    @StrictlyTyped(Number)
    public shipTonnageId?: number;

    @StrictlyTyped(Boolean)
    public hasEngine?: boolean;

    @StrictlyTyped(Number)
    public fuelTypeId?: number;

    @StrictlyTyped(Number)
    public shipEnginePower?: number;

    @StrictlyTyped(Number)
    public freeLaborAmount?: number;

    @StrictlyTyped(Boolean)
    public isShipHolderPartOfCrew?: boolean;

    @StrictlyTyped(String)
    public shipHolderPosition?: string;

    @StrictlyTyped(Boolean)
    public isFishingMainActivity?: boolean;

    @StrictlyTyped(Number)
    public workedOutHours?: number;

    @StrictlyTyped(StatisticalFormsSeaDaysDTO)
    public seaDays?: StatisticalFormsSeaDaysDTO[];

    @StrictlyTyped(StatisticalFormEmployeeInfoGroupDTO)
    public employeeInfoGroups?: StatisticalFormEmployeeInfoGroupDTO[];

    @StrictlyTyped(StatisticalFormNumStatGroupDTO)
    public numStatGroups?: StatisticalFormNumStatGroupDTO[];

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}