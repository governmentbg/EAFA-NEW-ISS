

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ApplicationSubmittedForDTO } from './ApplicationSubmittedForDTO';
import { StatisticalFormReworkRawMaterialDTO } from './StatisticalFormReworkRawMaterialDTO';
import { StatisticalFormReworkProductDTO } from './StatisticalFormReworkProductDTO';
import { StatisticalFormEmployeeInfoGroupDTO } from './StatisticalFormEmployeeInfoGroupDTO';
import { StatisticalFormNumStatGroupDTO } from './StatisticalFormNumStatGroupDTO';
import { StatisticalFormEmployeeInfoDTO } from './StatisticalFormEmployeeInfoDTO';
import { FileInfoDTO } from './FileInfoDTO';

export class StatisticalFormReworkEditDTO { 
    public constructor(obj?: Partial<StatisticalFormReworkEditDTO>) {
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

    @StrictlyTyped(Number)
    public year?: number;

    @StrictlyTyped(String)
    public formNum?: string;

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

    @StrictlyTyped(StatisticalFormReworkRawMaterialDTO)
    public rawMaterial?: StatisticalFormReworkRawMaterialDTO[];

    @StrictlyTyped(StatisticalFormReworkProductDTO)
    public products?: StatisticalFormReworkProductDTO[];

    @StrictlyTyped(StatisticalFormEmployeeInfoGroupDTO)
    public employeeInfoGroups?: StatisticalFormEmployeeInfoGroupDTO[];

    @StrictlyTyped(StatisticalFormNumStatGroupDTO)
    public numStatGroups?: StatisticalFormNumStatGroupDTO[];

    @StrictlyTyped(StatisticalFormEmployeeInfoDTO)
    public workDayDuration?: StatisticalFormEmployeeInfoDTO[];

    @StrictlyTyped(StatisticalFormEmployeeInfoDTO)
    public employeeAge?: StatisticalFormEmployeeInfoDTO[];

    @StrictlyTyped(StatisticalFormEmployeeInfoDTO)
    public employeeEducation?: StatisticalFormEmployeeInfoDTO[];

    @StrictlyTyped(StatisticalFormEmployeeInfoDTO)
    public employeeNationality?: StatisticalFormEmployeeInfoDTO[];

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}