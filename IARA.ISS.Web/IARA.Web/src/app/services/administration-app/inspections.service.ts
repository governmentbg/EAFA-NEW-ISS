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
import { map } from 'rxjs/operators';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { InspectionCatchMeasureDTO } from '@app/models/generated/dtos/InspectionCatchMeasureDTO';
import { LogBookPageStatusesEnum } from '@app/enums/log-book-page-statuses.enum';
import { DatePipe } from '@angular/common';
import { InspectedLogBookPageDataDTO } from '@app/models/generated/dtos/InspectedLogBookPageDataDTO';


@Injectable({
    providedIn: 'root',
})
export class InspectionsService extends BaseAuditService {
    protected controller: string = 'Inspections';
    private translate: FuseTranslationLoaderService;
    private datePipe: DatePipe;

    public constructor(requestService: RequestService, translate: FuseTranslationLoaderService, datePipe: DatePipe) {
        super(requestService, AreaTypes.Administrative);
        this.translate = translate;
        this.datePipe = datePipe;
    }

    public getIsInspector(): Observable<boolean> {
        return this.requestService.get(this.area, this.controller, 'GetIsInspector');
    }

    public canResolveCrossChecks(): Observable<boolean> {
        return this.requestService.get(this.area, this.controller, 'CanResolveCrossChecks');
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

    public getCheckTypesForInspection(inspectionType: InspectionTypesEnum): Observable<InspectionCheckTypeNomenclatureDTO[]> {
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

        return this.requestService.get<InspectionPermitLicenseDTO[]>(this.area, this.controller, 'GetShipPermitLicenses', {
            httpParams: params,
            responseTypeCtr: InspectionPermitLicenseDTO,
        }).pipe(map((licenses: InspectionPermitLicenseDTO[]) => {
            this.setPermitLicensesDisplayNames(licenses);

            return licenses;
        }));
    }

    public getPoundNetPermitLicenses(poundNetId: number): Observable<InspectionPermitLicenseDTO[]> {
        const params = new HttpParams().append('poundNetId', poundNetId.toString());

        return this.requestService.get<InspectionPermitLicenseDTO[]>(this.area, this.controller, 'GetPoundNetPermitLicenses', {
            httpParams: params,
            responseTypeCtr: InspectionPermitLicenseDTO,
        }).pipe(map((licenses: InspectionPermitLicenseDTO[]) => {
            this.setPermitLicensesDisplayNames(licenses);

            return licenses;
        }));
    }

    public getShipPermits(shipId: number): Observable<InspectionPermitLicenseDTO[]> {
        const params = new HttpParams().append('shipId', shipId.toString());

        return this.requestService.get<InspectionPermitLicenseDTO[]>(this.area, this.controller, 'GetShipPermits', {
            httpParams: params,
            responseTypeCtr: InspectionPermitLicenseDTO,
        }).pipe(map((licenses: InspectionPermitLicenseDTO[]) => {
            this.setPermitLicensesDisplayNames(licenses);

            return licenses;
        }));
    }

    public getShipLogBooks(shipId: number): Observable<InspectionShipLogBookDTO[]> {
        const params = new HttpParams().append('shipId', shipId.toString());

        return this.requestService.get(this.area, this.controller, 'GetShipLogBooks', {
            httpParams: params,
            responseTypeCtr: InspectionShipLogBookDTO,
        });
    }

    public getLogBookPagesByLogBookNum(type: DeclarationLogBookTypeEnum, logBookNum: string): Observable<InspectionLogBookPageNomenclatureDTO[]> {
        const params = new HttpParams().append('type', type.toString()).append('logBookNum', logBookNum);

        return this.requestService.get<InspectionLogBookPageNomenclatureDTO[]>(this.area, this.controller, 'GetLogBookPagesByLogBookNum', {
            httpParams: params,
            responseTypeCtr: InspectionLogBookPageNomenclatureDTO
        }).pipe(map((pages: InspectionLogBookPageNomenclatureDTO[]) => {
            for (const page of pages) {
                const status: string = this.getLogBookPageStatusName(page.status!);

                if (page.logBookPageDate !== null && page.logBookPageDate !== undefined) {
                    page.description = `${this.datePipe.transform(page.logBookPageDate, 'dd.MM.yyyy')} | ${status}`;
                }
                else {
                    page.description = `${status}`;
                }
            }

            return pages;
        }));
    }

    public getInspectedLogBookPageData(logBookPageId: number, type: DeclarationLogBookTypeEnum): Observable<InspectedLogBookPageDataDTO> {
        const params = new HttpParams().append('logBookPageId', logBookPageId.toString()).append('type', type.toString());

        return this.requestService.get(this.area, this.controller, 'GetInspectedLogBookPageData', {
            httpParams: params,
            responseTypeCtr: InspectedLogBookPageDataDTO
        });
    }

    public getCatchRecordsByShipLogBookPageId(shipLogBookPageId: number): Observable<InspectionCatchMeasureDTO[]> {
        const params = new HttpParams().append('shipLogBookPageId', shipLogBookPageId.toString());

        return this.requestService.get(this.area, this.controller, 'GetCatchRecordsByShipLogBookPageId', {
            httpParams: params,
            responseTypeCtr: InspectionCatchMeasureDTO
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

    public getValidFishingTicketsByEgn(egn: string): Observable<NomenclatureDTO<number>[]> {
        const params: HttpParams = new HttpParams().append('egn', egn);

        return this.requestService.get(this.area, this.controller, 'GetValidFishingTicketsByEgn', {
            httpParams: params,
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

        return this.requestService.get<InspectionLogBookPageNomenclatureDTO[]>(this.area, this.controller, 'GetLogBookPages', {
            httpParams: params,
            responseTypeCtr: InspectionLogBookPageNomenclatureDTO
        }).pipe(map((pages: InspectionLogBookPageNomenclatureDTO[]) => {
            for (const page of pages) {
                const status: string = this.getLogBookPageStatusName(page.status!);
                const logBook: string = this.translate.getValue('inspections.log-book');
                const pageNum: string = page.displayName!;

                page.displayName = `${pageNum} | ${status}`;
            }

            return pages;
        }));;
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

    public sendInspectionToFlux(inspectionId: number): Observable<void> {
        const params = new HttpParams().append('inspectionId', inspectionId.toString());

        return this.requestService.post(this.area, this.controller, 'SendInspectionToFlux', undefined, {
            httpParams: params,
            successMessage: 'inspection-sent-to-flux-success'
        });
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

    private setPermitLicensesDisplayNames(licenses: InspectionPermitLicenseDTO[]): void {
        for (const license of licenses) {
            const captain: string = this.translate.getValue('inspections.permit-license-nomenclature-captain');

            if (license.displayName !== null && license.displayName !== undefined && license.displayName!.length > 0) {
                license.displayName += ` | ${license.waterType} | ${captain}: ${license.captainName}`;
            }
            else {
                license.displayName = `${license.waterType} | ${captain}: ${license.captainName}`;
            }
        }
    }

    private getLogBookPageStatusName(status: LogBookPageStatusesEnum): string {
        switch (status) {
            case LogBookPageStatusesEnum.InProgress:
                return this.translate.getValue('inspections.log-book-page-status-in-progress');
            case LogBookPageStatusesEnum.Submitted:
                return this.translate.getValue('inspections.log-book-page-status-submitted');
            default:
                return '';
        }
    }
}
