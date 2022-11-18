import { HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationsActionsService } from '@app/interfaces/common-app/application-actions.interface';
import { IApplicationRegister } from '@app/interfaces/common-app/application-register.interface';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';
import { AssignedApplicationInfoDTO } from '@app/models/generated/dtos/AssignedApplicationInfoDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '@app/services/common-app/base-audit.service';

export abstract class ApplicationsRegisterPublicBaseService extends BaseAuditService implements IApplicationsActionsService {
    private readonly applicationPublicController: string = 'ApplicationsPublic';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Public);
    }

    public abstract getRegisterByApplicationId(applicationId: number, pageCode?: PageCodeEnum): Observable<unknown>;
    public abstract getApplication(id: number, getRegiXData: boolean, pageCode?: PageCodeEnum): Observable<IApplicationRegister>;
    public abstract addApplication(application: IApplicationRegister, pageCode?: PageCodeEnum): Observable<number>;
    public abstract editApplication(application: IApplicationRegister, pageCode?: PageCodeEnum, fromSaveAsDraft?: boolean): Observable<number>;

    public getCurrentUserAsSubmittedBy(...params: unknown[]): Observable<ApplicationSubmittedByDTO> {
        return this.requestService.get(this.area, this.applicationPublicController, 'GetCurrentUserAsSubmittedBy', {
            responseTypeCtr: ApplicationSubmittedByDTO
        });
    }

    public downloadFile(fileId: number, fileName: string): Observable<boolean> {
        const params = new HttpParams().append('id', fileId.toString());
        return this.requestService.download(this.area, this.controller, 'DownloadFile', fileName, { httpParams: params });
    }

    public getRegixData(id: number, pageCode?: PageCodeEnum): Observable<RegixChecksWrapperDTO<IApplicationRegister>> {
        throw new Error('This method should not be called from the public app.');
    }

    public getApplicationDataForRegister(applicationId: number, pageCode?: PageCodeEnum): Observable<IApplicationRegister> {
        throw new Error('This method should not be called from the public app.');
    }

    public assignApplicationViaAccessCode(accessCode: string): Observable<AssignedApplicationInfoDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public editApplicationDataAndStartRegixChecks(model: IApplicationRegister, pageCode?: PageCodeEnum): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }
}