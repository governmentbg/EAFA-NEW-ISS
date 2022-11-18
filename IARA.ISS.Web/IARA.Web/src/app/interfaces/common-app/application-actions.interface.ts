import { Observable } from 'rxjs';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { AssignedApplicationInfoDTO } from '@app/models/generated/dtos/AssignedApplicationInfoDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { IApplicationRegister } from '../common-app/application-register.interface';

export interface IApplicationsActionsService {
    getApplication(id: number, getRegiXData: boolean, pageCode?: PageCodeEnum): Observable<IApplicationRegister>;
    getRegixData(id: number, pageCode?: PageCodeEnum): Observable<RegixChecksWrapperDTO<IApplicationRegister>>;
    getApplicationDataForRegister(applicationId: number, pageCode?: PageCodeEnum): Observable<IApplicationRegister>;
    getRegisterByApplicationId(applicationId: number, pageCode?: PageCodeEnum): Observable<unknown>;
    addApplication(application: IApplicationRegister, pageCode?: PageCodeEnum): Observable<number>;
    editApplication(application: IApplicationRegister, pageCode?: PageCodeEnum, fromSaveAsDraft?: boolean): Observable<number>;
    downloadFile(fileId: number, fileName: string): Observable<boolean>;
    assignApplicationViaAccessCode(accessCode: string): Observable<AssignedApplicationInfoDTO>;
    editApplicationDataAndStartRegixChecks(model: IApplicationRegister): Observable<void>;
}