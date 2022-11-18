

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { BaseRegixChecksDTO } from './BaseRegixChecksDTO';
import { ApplicationSubmittedByDTO } from './ApplicationSubmittedByDTO';
import { ApplicationSubmittedForDTO } from './ApplicationSubmittedForDTO';
import { StatisticalFormReworkRawMaterialDTO } from './StatisticalFormReworkRawMaterialDTO';
import { StatisticalFormReworkProductDTO } from './StatisticalFormReworkProductDTO';
import { StatisticalFormEmployeeInfoGroupDTO } from './StatisticalFormEmployeeInfoGroupDTO';
import { StatisticalFormNumStatGroupDTO } from './StatisticalFormNumStatGroupDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { StatisticalFormReworkRegixDataDTO } from './StatisticalFormReworkRegixDataDTO'; 

export class StatisticalFormReworkApplicationEditDTO extends BaseRegixChecksDTO {
    public constructor(obj?: Partial<StatisticalFormReworkApplicationEditDTO>) {
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
    public year?: number;

    @StrictlyTyped(String)
    public vetRegistrationNum?: string;

    @StrictlyTyped(String)
    public licenceNum?: string;

    @StrictlyTyped(Date)
    public licenceDate?: Date;

    @StrictlyTyped(Number)
    public totalRawMaterialTons?: number;

    @StrictlyTyped(Number)
    public totalReworkedProductTons?: number;

    @StrictlyTyped(Number)
    public totalYearTurnover?: number;

    @StrictlyTyped(Boolean)
    public isOwnerEmployee?: boolean;

    @StrictlyTyped(StatisticalFormReworkRawMaterialDTO)
    public rawMaterial?: StatisticalFormReworkRawMaterialDTO[];

    @StrictlyTyped(StatisticalFormReworkProductDTO)
    public products?: StatisticalFormReworkProductDTO[];

    @StrictlyTyped(StatisticalFormEmployeeInfoGroupDTO)
    public employeeInfoGroups?: StatisticalFormEmployeeInfoGroupDTO[];

    @StrictlyTyped(StatisticalFormNumStatGroupDTO)
    public numStatGroups?: StatisticalFormNumStatGroupDTO[];

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];

    @StrictlyTyped(StatisticalFormReworkRegixDataDTO)
    public regiXDataModel?: StatisticalFormReworkRegixDataDTO;
}