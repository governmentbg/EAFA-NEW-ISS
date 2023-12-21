import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { InspectionTypesEnum } from '@app/enums/inspection-types.enum';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { FileInfoDTO } from '@app/models/generated/dtos/FileInfoDTO';
import { FishingGearDTO } from '@app/models/generated/dtos/FishingGearDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { InspectedBuyerNomenclatureDTO } from '@app/models/generated/dtos/InspectedBuyerNomenclatureDTO';
import { InspectionCheckTypeNomenclatureDTO } from '@app/models/generated/dtos/InspectionCheckTypeNomenclatureDTO';
import { InspectionDraftDTO } from '@app/models/generated/dtos/InspectionDraftDTO';
import { InspectionDTO } from '@app/models/generated/dtos/InspectionDTO';
import { InspectionEditDTO } from '@app/models/generated/dtos/InspectionEditDTO';
import { InspectionPermitLicenseDTO } from '@app/models/generated/dtos/InspectionPermitLicenseDTO';
import { InspectionShipLogBookDTO } from '@app/models/generated/dtos/InspectionShipLogBookDTO';
import { InspectionShipSubjectNomenclatureDTO } from '@app/models/generated/dtos/InspectionShipSubjectNomenclatureDTO';
import { InspectionSubjectPersonnelDTO } from '@app/models/generated/dtos/InspectionSubjectPersonnelDTO';
import { InspectorDTO } from '@app/models/generated/dtos/InspectorDTO';
import { VesselDTO } from '@app/models/generated/dtos/VesselDTO';
import { InspectionsFilters } from '@app/models/generated/filters/InspectionsFilters';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { Observable } from 'rxjs';
import { DeclarationLogBookTypeEnum } from '@app/enums/declaration-log-book-type.enum';
import { VesselDuringInspectionDTO } from '@app/models/generated/dtos/VesselDuringInspectionDTO';
import { BaseAuditService } from '../common-app/base-audit.service';
import { InspectorDuringInspectionDTO } from '@app/models/generated/dtos/InspectorDuringInspectionDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { InspectionLogBookPageNomenclatureDTO } from '@app/models/generated/dtos/InspectionLogBookPageNomenclatureDTO';


@Injectable({
    providedIn: 'root',
})
export class InspectionsService extends BaseAuditService {
    protected controller: string = 'Inspections';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Administrative);
    }

    public getIsInspector(): Observable<boolean> {
        return this.requestService.get(this.area, this.controller, 'GetIsInspector');
    }

    public getInspectors(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetInspectors', {
            responseTypeCtr: NomenclatureDTO,
        });
    }

    public getInspector(id: number): Observable<InspectorDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetInspector', {
            httpParams: params,
            responseTypeCtr: InspectorDTO,
        });
    }

    public getCurrentInspector(): Observable<InspectorDTO> {
        return this.requestService.get(this.area, this.controller, 'GetCurrentInspector', {
            responseTypeCtr: InspectorDTO,
        });
    }

    public getPatrolVehicles(isWaterVehicle: boolean | undefined): Observable<NomenclatureDTO<number>[]> {
        let params = new HttpParams();

        if (isWaterVehicle != undefined) {
            params = params.append('isWaterVehicle', isWaterVehicle.toString());
        }

        return this.requestService.get(this.area, this.controller, 'GetPatrolVehicles', {
            httpParams: params,
            responseTypeCtr: NomenclatureDTO,
        });
    }

    public getPatrolVehicle(id: number): Observable<VesselDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetPatrolVehicle', {
            httpParams: params,
            responseTypeCtr: VesselDTO,
        });
    }

    public getShip(id: number): Observable<VesselDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetShip', {
            httpParams: params,
            responseTypeCtr: VesselDTO,
        });
    }

    public getCheckTypesForInspection(
        inspectionType: InspectionTypesEnum
    ): Observable<InspectionCheckTypeNomenclatureDTO[]> {
        const params = new HttpParams().append('inspectionType', inspectionType.toString());

        return this.requestService.get(this.area, this.controller, 'GetCheckTypesForInspection', {
            httpParams: params,
            responseTypeCtr: InspectionCheckTypeNomenclatureDTO,
        });
    }

    public getShipPersonnel(shipId: number): Observable<InspectionShipSubjectNomenclatureDTO[]> {
        const params = new HttpParams().append('shipId', shipId.toString());

        return this.requestService.get(this.area, this.controller, 'getShipPersonnel', {
            httpParams: params,
            responseTypeCtr: InspectionShipSubjectNomenclatureDTO,
        });
    }

    public getShipPermitLicenses(shipId: number): Observable<InspectionPermitLicenseDTO[]> {
        const params = new HttpParams().append('shipId', shipId.toString());

        return this.requestService.get(this.area, this.controller, 'GetShipPermitLicenses', {
            httpParams: params,
            responseTypeCtr: InspectionPermitLicenseDTO,
        });
    }

    public getPoundNetPermitLicenses(poundNetId: number): Observable<InspectionPermitLicenseDTO[]> {
        const params = new HttpParams().append('poundNetId', poundNetId.toString());

        return this.requestService.get(this.area, this.controller, 'GetPoundNetPermitLicenses', {
            httpParams: params,
            responseTypeCtr: InspectionPermitLicenseDTO,
        });
    }

    public getShipPermits(shipId: number): Observable<InspectionPermitLicenseDTO[]> {
        const params = new HttpParams().append('shipId', shipId.toString());

        return this.requestService.get(this.area, this.controller, 'GetShipPermits', {
            httpParams: params,
            responseTypeCtr: InspectionPermitLicenseDTO,
        });
    }

    public getShipLogBooks(shipId: number): Observable<InspectionShipLogBookDTO[]> {
        const params = new HttpParams().append('shipId', shipId.toString());

        return this.requestService.get(this.area, this.controller, 'GetShipLogBooks', {
            httpParams: params,
            responseTypeCtr: InspectionShipLogBookDTO,
        });
    }

    public getShipFishingGears(shipId: number): Observable<FishingGearDTO[]> {
        const params = new HttpParams().append('shipId', shipId.toString());

        return this.requestService.get(this.area, this.controller, 'GetShipFishingGears', {
            httpParams: params,
            responseTypeCtr: FishingGearDTO,
        });
    }

    public getPoundNetFishingGears(poundNetId: number): Observable<FishingGearDTO[]> {
        const params = new HttpParams().append('poundNetId', poundNetId.toString());

        return this.requestService.get(this.area, this.controller, 'GetPoundNetFishingGears', {
            httpParams: params,
            responseTypeCtr: FishingGearDTO,
        });
    }

    public getBuyers(): Observable<InspectedBuyerNomenclatureDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetBuyers', {
            responseTypeCtr: InspectedBuyerNomenclatureDTO,
        });
    }

    public getAquacultures(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetAquacultures', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getInspectionVesselTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetInspectionVesselTypes', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getInspectionStates(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetInspectionStates', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getLogBookPages(type: DeclarationLogBookTypeEnum, shipId: number | undefined, aquacultureId: number | undefined): Observable<InspectionLogBookPageNomenclatureDTO[]> {
        let params = new HttpParams().append('type', type.toString());

        if (shipId !== undefined && shipId !== null) {
            params = params.append('shipId', shipId.toString());
        }

        if (aquacultureId !== undefined && aquacultureId !== null) {
            params = params.append('aquacultureId', aquacultureId.toString());
        }

        return this.requestService.get(this.area, this.controller, 'GetLogBookPages', {
            httpParams: params,
            responseTypeCtr: InspectionLogBookPageNomenclatureDTO
        });
    }

    public getAquacultureOwner(aquacultureId: number): Observable<InspectionSubjectPersonnelDTO> {
        const params = new HttpParams().append('aquacultureId', aquacultureId.toString());

        return this.requestService.get(this.area, this.controller, 'GetAquacultureOwner', {
            httpParams: params,
            responseTypeCtr: InspectionSubjectPersonnelDTO,
        });
    }

    public get(id: number, inspectionCode: InspectionTypesEnum): Observable<InspectionEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'Get' + InspectionTypesEnum[inspectionCode], {
            httpParams: params,
            responseTypeCtr: InspectionEditDTO,
        });
    }

    public getAll(request: GridRequestModel<InspectionsFilters>): Observable<GridResultModel<InspectionDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAll', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel,
        });
    }

    public delete(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.delete(this.area, this.controller, 'Delete', {
            httpParams: params,
        });
    }

    public restore(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.patch(this.area, this.controller, 'UndoDelete', null, { httpParams: params });
    }

    public getNextReportNumber(userId: number): Observable<{ num: string }> {
        const params = new HttpParams().append('userId', userId.toString());
        return this.requestService.get(this.area, this.controller, 'GetNextReportNumber', { httpParams: params });
    }

    public downloadFile(fileId: number, fileName: string): Observable<boolean> {
        const params = new HttpParams().append('id', fileId.toString());
        return this.requestService.download(this.area, this.controller, 'DownloadFile', fileName, { httpParams: params });
    }

    public sign(inspectionId: number, files: FileInfoDTO[]): Observable<void> {
        const params = new HttpParams().append('inspectionId', inspectionId.toString());

        return this.requestService.post(this.area, this.controller, 'Sign', { files }, {
            httpParams: params,
            properties: new RequestProperties({
                asFormData: true
            })
        });
    }

    public sendForFurtherCorrections(inspection: InspectionDraftDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'SendForFurtherCorrections', inspection, {
            properties: new RequestProperties({
                asFormData: true
            })
        });
    }

    public add(inspection: InspectionDraftDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'Add', inspection, {
            properties: new RequestProperties({
                asFormData: true
            })
        });
    }

    public edit(inspection: InspectionDraftDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'Edit', inspection, {
            properties: new RequestProperties({
                asFormData: true
            })
        });
    }

    public submit(inspection: InspectionEditDTO): Observable<number> {
        const service: string = 'Submit' + InspectionTypesEnum[inspection.inspectionType!];

        return this.requestService.post(this.area, this.controller, service, inspection, {
            properties: new RequestProperties({
                asFormData: true
            })
        });
    }

    public downloadReport(inspectionId: number, reportNum: string): Observable<boolean> {
        const params = new HttpParams().append('inspectionId', inspectionId.toString());

        return this.requestService.download(this.area, this.controller, 'DownloadReport', reportNum + '.pdf', { httpParams: params });
    }

    public downloadFluxXml(id: number, type: InspectionTypesEnum): Observable<boolean> {
        const service: string = 'DownloadFluxXml' + InspectionTypesEnum[type];

        return this.requestService.downloadPost(this.area, this.controller, service, '', id);
    }

    public unregisteredInspectorExists(inspector: InspectorDuringInspectionDTO): Observable<boolean> {
        return this.requestService.post(this.area, this.controller, 'UnregisteredInspectorExists', inspector);
    }

    public unregisteredPatrolVehicleExists(vessel: VesselDuringInspectionDTO): Observable<boolean> {
        return this.requestService.post(this.area, this.controller, 'UnregisteredPatrolVehicleExists', vessel);
    }

    //Audits
    public getInspectionInspectorSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetInspectionInspectorSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public getInspectionPatrolVehicleSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetInspectionPatrolVehicleSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public getInspectionVesselSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetInspectionVesselSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public getInspectedFishingGearSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetInspectedFishingGearSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public getInspectionCatchMeasureSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetInspectionCatchMeasureSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public getInspectionEngineSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetInspectionEngineSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }
}
