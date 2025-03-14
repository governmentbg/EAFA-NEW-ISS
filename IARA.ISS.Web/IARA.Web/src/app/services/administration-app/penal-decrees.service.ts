﻿import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

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
import { map, switchMap } from 'rxjs/operators';
import { PenalDecreeStatusTypesEnum } from '@app/enums/penal-decree-status-types.enum';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { DateUtils } from '@app/shared/utils/date.utils';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { PenalDecreeStatusEditDTO } from '@app/models/generated/dtos/PenalDecreeStatusEditDTO';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { IInspDeliveryService } from '@app/interfaces/administration-app/insp-delivery.interface';
import { AuanDeliveryDataDTO } from '@app/models/generated/dtos/AuanDeliveryDataDTO';
import { InspectorUserNomenclatureDTO } from '@app/models/generated/dtos/InspectorUserNomenclatureDTO';
import { AuanStatusEnum } from '@app/enums/auan-status.enum';
import { InspectedEntityControlActivityInfoDTO } from '@app/models/generated/dtos/InspectedEntityControlActivityInfoDTO';
import { EgnLncDTO } from '@app/models/generated/dtos/EgnLncDTO';

@Injectable({
    providedIn: 'root'
})
export class PenalDecreesService extends BaseAuditService implements IPenalDecreesService, IInspDeliveryService {
    protected controller: string = 'PenalDecrees';

    private readonly translate: FuseTranslationLoaderService;
    private readonly permissions: PermissionsService;

    public constructor(
        requestService: RequestService,
        translate: FuseTranslationLoaderService,
        permissions: PermissionsService
    ) {
        super(requestService, AreaTypes.Administrative);

        this.translate = translate;
        this.permissions = permissions;
    }

    public getAllPenalDecrees(request: GridRequestModel<PenalDecreesFilters>): Observable<GridResultModel<PenalDecreeDTO>> {
        type Result = GridResultModel<PenalDecreeDTO>;
        type Body = GridRequestModel<PenalDecreesFilters>;

        return this.requestService.post<Result, Body>(this.area, this.controller, 'GetAllPenalDecrees', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        }).pipe(switchMap((entries: Result) => {
            for (const entry of entries.records) {
                if (entry.penalDecreeStatus !== undefined && entry.penalDecreeStatus !== null) {
                    entry.penalDecreeStatusName = this.getPenalDecreeStatusName(entry.penalDecreeStatus);
                }
            }

            const decreeIds: number[] = entries.records.map((decree: PenalDecreeDTO) => {
                return decree.id!;
            });

            if (decreeIds.length === 0) {
                return of(entries);
            }

            if (this.permissions.has(PermissionsEnum.PenalDecreeStatusesRead)) {
                return this.getPenalDecreeStatusesForTableHelper(this.controller, decreeIds).pipe(map((statuses: PenalDecreeStatusEditDTO[]) => {
                    for (const status of statuses) {
                        const found = entries.records.find((entry: PenalDecreeDTO) => {
                            return entry.id === status.penalDecreeId;
                        });

                        if (found !== undefined) {
                            this.getStatusDetails(status);

                            if (found.statuses !== undefined && found.statuses !== null) {
                                found.statuses.push(new PenalDecreeStatusEditDTO(status));
                            }
                            else {
                                found.statuses = [status];
                            }
                        }
                    }

                    return entries;
                }));
            }
            else {
                return of(entries);
            }
        }));
    }

    public getPenalDecree(id: number): Observable<PenalDecreeEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetPenalDecree', {
            httpParams: params,
            responseTypeCtr: PenalDecreeEditDTO
        });
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

    public downloadFile(fileId: number): Observable<boolean> {
        const params = new HttpParams().append('id', fileId.toString());
        return this.requestService.download(this.area, this.controller, 'DownloadFile', '', { httpParams: params });
    }

    public getDeliveryData(id: number): Observable<AuanDeliveryDataDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetPenalDecreeDeliveryData', {
            httpParams: params,
            responseTypeCtr: AuanDeliveryDataDTO
        });
    }

    public addDeliveryData(decreeId: number, deliveryData: AuanDeliveryDataDTO): Observable<number> {
        const params = new HttpParams().append('decreeId', decreeId.toString());

        return this.requestService.post(this.area, this.controller, 'AddPenalDecreeDeliveryData', deliveryData, {
            httpParams: params,
            successMessage: 'succ-updated-delivery-data',
            properties: new RequestProperties({
                asFormData: true
            })
        });
    }

    public editDeliveryData(decreeId: number, deliveryData: AuanDeliveryDataDTO, sendEDelivery: boolean): Observable<void> {
        const params = new HttpParams()
            .append('decreeId', decreeId.toString())
            .append('deliveryId', deliveryData.id!.toString())
            .append('sendEDelivery', sendEDelivery.toString());

        return this.requestService.post(this.area, this.controller, 'UpdatePenalDecreeDeliveryData', deliveryData, {
            httpParams: params,
            successMessage: 'succ-updated-delivery-data',
            properties: new RequestProperties({
                asFormData: true
            })
        });
    }

    public getInspectedPersonControlActivityInfo(egnLnc: EgnLncDTO): Observable<InspectedEntityControlActivityInfoDTO> {
        const params = new HttpParams()
            .append('egnLnc', egnLnc.egnLnc!.toString())
            .append('dentifierType', egnLnc.identifierType!.toString());

        return this.requestService.get(this.area, this.controller, 'GetInspectedPersonControlActivityInfo', {
            httpParams: params,
            responseTypeCtr: InspectedEntityControlActivityInfoDTO
        });
    }

    public getInspectedLegalControlActivityInfo(eik: string): Observable<InspectedEntityControlActivityInfoDTO> {
        const params = new HttpParams().append('eik', eik.toString());

        return this.requestService.get(this.area, this.controller, 'GetInspectedLegalControlActivityInfo', {
            httpParams: params,
            responseTypeCtr: InspectedEntityControlActivityInfoDTO
        });
    }

    //Statuses
    public addPenalDecreeStatus(status: PenalDecreeStatusEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddPenalDecreeStatus', status, {
            properties: new RequestProperties({ asFormData: true, rethrowException: true, showException: true })
        });
    }

    public editPenalDecreeStatus(status: PenalDecreeStatusEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'EditPenalDecreeStatus', status, {
            properties: new RequestProperties({ asFormData: true, rethrowException: true, showException: true })
        });
    }

    public deletePenalDecreeStatus(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.delete(this.area, this.controller, 'DeletePenalDecreeStatus', { httpParams: params });
    }

    public undoDeletePenalDecreeStatus(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.patch(this.area, this.controller, 'UndoDeletePenalDecreeStatus', null, { httpParams: params });
    }

    //Nomenclatures
    public getAllAuans(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetAllAuans', { responseTypeCtr: NomenclatureDTO });
    }

    public getDeliveryTypes(): Observable<NomenclatureDTO<number>[]> {
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

    public getInspectorUsernames(): Observable<InspectorUserNomenclatureDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetInspectorUsernames', { responseTypeCtr: InspectorUserNomenclatureDTO });
    }

    //Audits
    public getPenalDecreeStatusAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetPenalDecreeStatusSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public getInspDeliverySimpleAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetInspDeliverySimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public updateDecreeStatus(id: number, status: AuanStatusEnum): Observable<void> {
        const params = new HttpParams()
            .append('decreeId', id.toString())
            .append('status', status.toString());

        return this.requestService.put(this.area, this.controller, 'UpdatePenalDecreeStatus', null, {
            httpParams: params
        });
    }

    //Helpers
    private getPenalDecreeStatusesForTableHelper(controller: string, decreeIds: number[]): Observable<PenalDecreeStatusEditDTO[]> {
        return this.requestService.post(this.area, controller, 'GetPenalDecreeStatuses', decreeIds, {
            responseTypeCtr: PenalDecreeStatusEditDTO
        });
    }

    private getStatusDetails(status: PenalDecreeStatusEditDTO): void {
        const from: string = this.translate.getValue('common.from');
        const instanceAppealDate: string = this.translate.getValue('penal-decrees.status-details-instance-appeal-date');
        const decisionDate: string = this.translate.getValue('penal-decrees.status-details-decision-date');
        const enactmentDate: string = this.translate.getValue('penal-decrees.status-details-enactment-date');
        const dateOfWithdraw: string = this.translate.getValue('penal-decrees.status-details-withdraw-date');
        const dateOfChange: string = this.translate.getValue('penal-decrees.status-details-change-date');
        const compulsory: string = this.translate.getValue('penal-decrees.status-details-compulsory');
        const partiallyPaid: string = this.translate.getValue('penal-decrees.status-details-partially-paid');

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

    private getPenalDecreeStatusName(penalDecreeStatus: AuanStatusEnum): string {
        switch (penalDecreeStatus) {
            case AuanStatusEnum.Draft:
                return this.translate.getValue('penal-decrees.penal-decree-status-draft');
            case AuanStatusEnum.Canceled:
                return this.translate.getValue('penal-decrees.penal-decree-status-canceled');
            case AuanStatusEnum.Submitted:
                return this.translate.getValue('penal-decrees.penal-decree-status-submitted');
        }
    }
}