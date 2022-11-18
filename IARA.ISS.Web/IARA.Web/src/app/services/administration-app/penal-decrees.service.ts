import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { IPenalDecreesService } from '@app/interfaces/administration-app/penal-decrees.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { PenalDecreeDTO } from '@app/models/generated/dtos/PenalDecreeDTO';
import { PenalDecreeEditDTO } from '@app/models/generated/dtos/PenalDecreeEditDTO';
import { PenalDecreesFilters } from '@app/models/generated/filters/PenalDecreesFilters';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PenalDecreeAuanDataDTO } from '@app/models/generated/dtos/PenalDecreeAuanDataDTO';
import { BaseAuditService } from '@app/services/common-app/base-audit.service';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { AuanConfiscationActionsNomenclatureDTO } from '@app/models/generated/dtos/AuanConfiscationActionsNomenclatureDTO';
import { InspDeliveryTypesNomenclatureDTO } from '@app/models/generated/dtos/InspDeliveryTypesNomenclatureDTO';
import { map } from 'rxjs/operators';
import { PenalDecreeStatusTypesEnum } from '@app/enums/penal-decree-status-types.enum';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { DateUtils } from '@app/shared/utils/date.utils';

@Injectable({
    providedIn: 'root'
})
export class PenalDecreesService extends BaseAuditService implements IPenalDecreesService {
    protected controller: string = 'PenalDecrees';

    private readonly translate: FuseTranslationLoaderService;

    public constructor(
        requestService: RequestService,
        translate: FuseTranslationLoaderService
    ) {
        super(requestService, AreaTypes.Administrative);

        this.translate = translate;
    }

    public getAllPenalDecrees(request: GridRequestModel<PenalDecreesFilters>): Observable<GridResultModel<PenalDecreeDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAllPenalDecrees', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public getPenalDecree(id: number): Observable<PenalDecreeEditDTO> {
        type Result = PenalDecreeEditDTO;
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get<Result>(this.area, this.controller, 'GetPenalDecree', {
            httpParams: params,
            responseTypeCtr: PenalDecreeEditDTO
        }).pipe(map((decree: Result) => {
            const from: string = this.translate.getValue('common.from');
            const instanceAppealDate: string = this.translate.getValue('penal-decrees.status-details-instance-appeal-date');
            const decisionDate: string = this.translate.getValue('penal-decrees.status-details-decision-date');
            const enactmentDate: string = this.translate.getValue('penal-decrees.status-details-enactment-date');
            const dateOfWithdraw: string = this.translate.getValue('penal-decrees.status-details-withdraw-date');
            const dateOfChange: string = this.translate.getValue('penal-decrees.status-details-change-date');
            const compulsory: string = this.translate.getValue('penal-decrees.status-details-compulsory');
            const partiallyPaid: string = this.translate.getValue('penal-decrees.status-details-partially-paid');

            for (const status of decree.statuses ?? []) {
                switch (status.statusType) {
                    case PenalDecreeStatusTypesEnum.FirstInstAppealed:
                    case PenalDecreeStatusTypesEnum.SecondInstAppealed:
                        status.details = `${instanceAppealDate}: ${DateUtils.ToDisplayDateString(status.appealDate!)} ${from} ${status.courtName}`;
                        break;
                    case PenalDecreeStatusTypesEnum.FirstInstDecision:
                        status.details = `${decisionDate}: ${DateUtils.ToDisplayDateString(status.complaintDueDate!)} ${from} ${status.courtName}`;
                        break;
                    case PenalDecreeStatusTypesEnum.SecondInstDecision:
                        status.details = `${from} ${status.courtName}`;
                        break;
                    case PenalDecreeStatusTypesEnum.PartiallyChanged:
                        status.details = `${dateOfChange}: ${DateUtils.ToDisplayDateString(status.enactmentDate!)} ${from} ${status.courtName}`;
                        break;
                    case PenalDecreeStatusTypesEnum.PartiallyPaid:
                        status.details = `${partiallyPaid}: ${status.paidAmount}`;
                        break;
                    case PenalDecreeStatusTypesEnum.Valid:
                        status.details = `${enactmentDate}: ${DateUtils.ToDisplayDateString(status.enactmentDate!)}`;
                        break;
                    case PenalDecreeStatusTypesEnum.Withdrawn:
                        status.details = `${dateOfWithdraw}: ${DateUtils.ToDisplayDateString(status.enactmentDate!)} ${from} ${status.penalAuthorityName}`;
                        break;
                    case PenalDecreeStatusTypesEnum.Compulsory:
                        status.details = `${compulsory} ${status.confiscationInstitution}`;
                        break;
                    default:
                        status.details = undefined;
                }
            }

            return decree;
        }));
    }

    public addPenalDecree(decree: PenalDecreeEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddPenalDecree', decree, {
            properties: new RequestProperties({ asFormData: true, rethrowException: true, showException: true })
        });
    }

    public editPenalDecree(decree: PenalDecreeEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'EditPenalDecree', decree, {
            properties: new RequestProperties({ asFormData: true, rethrowException: true, showException: true })
        });
    }

    public deletePenalDecree(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.delete(this.area, this.controller, 'DeletePenalDecree', { httpParams: params });
    }

    public undoDeletePenalDecree(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.patch(this.area, this.controller, 'UndoDeletePenalDecree', null, { httpParams: params });
    }

    public getPenalDecreeAuanData(auanId: number): Observable<PenalDecreeAuanDataDTO> {
        const params = new HttpParams().append('auanId', auanId.toString());

        return this.requestService.get(this.area, this.controller, 'GetPenalDecreeAuanData', {
            httpParams: params,
            responseTypeCtr: PenalDecreeAuanDataDTO
        });
    }

    public downloadPenalDecree(decreeId: number): Observable<boolean> {
        const params = new HttpParams().append('decreeId', decreeId.toString());
        return this.requestService.download(this.area, this.controller, 'DownloadPenalDecree', '', { httpParams: params });
    }

    public downloadFile(fileId: number, fileName: string): Observable<boolean> {
        const params = new HttpParams().append('id', fileId.toString());
        return this.requestService.download(this.area, this.controller, 'DownloadFile', fileName, { httpParams: params });
    }

    public getAllAuans(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetAllAuans', { responseTypeCtr: NomenclatureDTO });
    }

    public getInspDeliveryTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetInspDeliveryTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getPenalDecreeStatusTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetPenalDecreeStatusTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getPenalDecreeAuthorityTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetPenalDecreeAuthorityTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getCourts(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetCourts', { responseTypeCtr: NomenclatureDTO });
    }

    public getPenalDecreeTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetPenalDecreeTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getPenalDecreeSanctionTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetPenalDecreeSanctionTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getConfiscationInstitutions(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetConfiscationInstitutions', { responseTypeCtr: NomenclatureDTO });
    }

    public getAuanDeliveryTypes(): Observable<InspDeliveryTypesNomenclatureDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetAuanDeliveryTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getAuanDeliveryConfirmationTypes(): Observable<InspDeliveryTypesNomenclatureDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetAuanDeliveryConfirmationTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getConfiscationActions(): Observable<AuanConfiscationActionsNomenclatureDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetConfiscationActions', { responseTypeCtr: AuanConfiscationActionsNomenclatureDTO });
    }

    public getConfiscatedAppliances(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetConfiscatedAppliances', { responseTypeCtr: NomenclatureDTO });
    }

    public getTurbotSizeGroups(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetTurbotSizeGroups', { responseTypeCtr: NomenclatureDTO });
    }

    public getInspectorUsernames(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetInspectorUsernames', { responseTypeCtr: NomenclatureDTO });
    }

    public getPenalDecreeStatusAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetPenalDecreeStatusSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }
}