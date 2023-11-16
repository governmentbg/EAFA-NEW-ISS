import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { IAuanRegisterService } from '@app/interfaces/administration-app/auan-register.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { AuanRegisterDTO } from '@app/models/generated/dtos/AuanRegisterDTO';
import { AuanRegisterEditDTO } from '@app/models/generated/dtos/AuanRegisterEditDTO';
import { AuanRegisterFilters } from '@app/models/generated/filters/AuanRegisterFilters';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '@app/services/common-app/base-audit.service';
import { RequestProperties } from '@app/shared/services/request-properties';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { AuanReportDataDTO } from '@app/models/generated/dtos/AuanReportDataDTO';
import { AuanConfiscationActionsNomenclatureDTO } from '@app/models/generated/dtos/AuanConfiscationActionsNomenclatureDTO';
import { InspDeliveryTypesNomenclatureDTO } from '@app/models/generated/dtos/InspDeliveryTypesNomenclatureDTO';
import { AuanInspectionDTO } from '@app/models/generated/dtos/AuanInspectionDTO';
import { AuanLawSectionDTO } from '@app/models/generated/dtos/AuanLawSectionDTO';
import { AuanDeliveryDataDTO } from '@app/models/generated/dtos/AuanDeliveryDataDTO';
import { IInspDeliveryService } from '@app/interfaces/administration-app/insp-delivery.interface';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';

@Injectable({
    providedIn: 'root'
})
export class AuanRegisterService extends BaseAuditService implements IAuanRegisterService, IInspDeliveryService {
    protected controller: string = 'AuanRegister';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Administrative);
    }

    public getAllAuans(request: GridRequestModel<AuanRegisterFilters>): Observable<GridResultModel<AuanRegisterDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAllAuans', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public getAuan(id: number): Observable<AuanRegisterEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetAuan', {
            httpParams: params,
            responseTypeCtr: AuanRegisterEditDTO
        });
    }

    public addAuan(auan: AuanRegisterEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddAuan', auan, {
            properties: new RequestProperties({ asFormData: true, rethrowException: true, showException: true })
        });
    }

    public editAuan(auan: AuanRegisterEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'EditAuan', auan, {
            properties: new RequestProperties({ asFormData: true, rethrowException: true, showException: true })
        });
    }

    public deleteAuan(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.delete(this.area, this.controller, 'DeleteAuan', { httpParams: params });
    }

    public undoDeleteAuan(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.patch(this.area, this.controller, 'UndoDeleteAuan', null, { httpParams: params });
    }

    public addAuanInspection(inspection: AuanInspectionDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddAuanInspection', inspection);
    }

    public getAuanReportData(inspectionId: number): Observable<AuanReportDataDTO> {
        const params = new HttpParams().append('inspectionId', inspectionId.toString());

        return this.requestService.get(this.area, this.controller, 'GetAuanReportData', {
            httpParams: params,
            responseTypeCtr: AuanReportDataDTO
        });
    }

    public downloadAuan(auanId: number): Observable<boolean> {
        const params = new HttpParams().append('auanId', auanId.toString());
        return this.requestService.download(this.area, this.controller, 'DownloadAuan', 'AUAN', { httpParams: params });
    }

    public downloadFile(fileId: number): Observable<boolean> {
        const params = new HttpParams().append('id', fileId.toString());
        return this.requestService.download(this.area, this.controller, 'DownloadFile', '', { httpParams: params });
    }

    public getAllDrafters(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetAllDrafters', { responseTypeCtr: NomenclatureDTO });
    }

    public getInspectionDrafters(inspectionId: number): Observable<NomenclatureDTO<number>[]> {
        const params = new HttpParams().append('inspectionId', inspectionId.toString());

        return this.requestService.get(this.area, this.controller, 'GetAllDrafters', {
            httpParams: params,
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getDeliveryData(id: number): Observable<AuanDeliveryDataDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetAuanDeliveryData', {
            httpParams: params,
            responseTypeCtr: AuanDeliveryDataDTO
        });
    }

    public addDeliveryData(auanId: number, deliveryData: AuanDeliveryDataDTO): Observable<number> {
        const params = new HttpParams().append('auanId', auanId.toString());

        return this.requestService.post(this.area, this.controller, 'AddAuanDeliveryData', deliveryData, {
            httpParams: params,
            successMessage: 'succ-updated-delivery-data',
            properties: new RequestProperties({
                asFormData: true
            })
        });
    }

    public editDeliveryData(auanId: number, deliveryData: AuanDeliveryDataDTO, sendEDelivery: boolean): Observable<void> {
        const params = new HttpParams()
            .append('auanId', auanId.toString())
            .append('deliveryId', deliveryData.id!.toString())
            .append('sendEDelivery', sendEDelivery.toString());

        return this.requestService.post(this.area, this.controller, 'UpdateAuanDeliveryData', deliveryData, {
            httpParams: params,
            successMessage: 'succ-updated-delivery-data',
            properties: new RequestProperties({
                asFormData: true
            })
        });
    }

    public getInspDeliverySimpleAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetInspDeliveryAuditInfo', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public getAllInspectionReports(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetAllInspectionReports', { responseTypeCtr: NomenclatureDTO });
    }

    public getConfiscationActions(): Observable<AuanConfiscationActionsNomenclatureDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetConfiscationActions', { responseTypeCtr: AuanConfiscationActionsNomenclatureDTO });
    }

    public getDeliveryTypes(): Observable<InspDeliveryTypesNomenclatureDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetAuanDeliveryTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getAuanDeliveryConfirmationTypes(): Observable<InspDeliveryTypesNomenclatureDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetAuanDeliveryConfirmationTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getLaws(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetLaws', { responseTypeCtr: NomenclatureDTO });
    }

    public getAuanLawSections(): Observable<AuanLawSectionDTO[]> {
        return this.requestService.get<AuanLawSectionDTO[]>(this.area, this.controller, 'GetAuanLawSections', { responseTypeCtr: AuanLawSectionDTO });
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
}