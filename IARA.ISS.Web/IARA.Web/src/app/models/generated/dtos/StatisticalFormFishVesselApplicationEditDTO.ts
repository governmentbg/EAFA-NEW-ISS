

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { BaseRegixChecksDTO } from './BaseRegixChecksDTO';
import { ApplicationSubmittedByDTO } from './ApplicationSubmittedByDTO';
import { ApplicationSubmittedForDTO } from './ApplicationSubmittedForDTO';
import { StatisticalFormsSeaDaysDTO } from './StatisticalFormsSeaDaysDTO';
import { StatisticalFormEmployeeInfoGroupDTO } from './StatisticalFormEmployeeInfoGroupDTO';
import { StatisticalFormNumStatGroupDTO } from './StatisticalFormNumStatGroupDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { StatisticalFormFishVesselRegixDataDTO } from './StatisticalFormFishVesselRegixDataDTO'; 

export class StatisticalFormFishVesselApplicationEditDTO extends BaseRegixChecksDTO {
    public constructor(obj?: Partial<StatisticalFormFishVesselApplicationEditDTO>) {
        if (obj != undefined) {
            super(obj as BaseRegixChecksDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Boolean)
    public isOnlineApplication?: boolean;

    @StrictlyTyped(ApplicationSubmittedByDTO)
    public submittedBy?: ApplicationSubmittedByDTO;

    @StrictlyTyped(String)
    public submittedByWorkPosition?: string;

    @StrictlyTyped(ApplicationSubmittedForDTO)
    public submittedFor?: ApplicationSubmittedForDTO;

    @StrictlyTyped(Number)
    public shipId?: number;

    @StrictlyTyped(Number)
    public year?: number;

    @StrictlyTyped(Number)
    public registrationNum?: number;

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

    @StrictlyTyped(StatisticalFormFishVesselRegixDataDTO)
    public regiXDataModel?: StatisticalFormFishVesselRegixDataDTO;
}