import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { InspectionTypesEnum } from '@app/enums/inspection-types.enum';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { AuanRegisterDTO } from '@app/models/generated/dtos/AuanRegisterDTO';
import { DeclarationLogBookPageDTO } from '@app/models/generated/dtos/DeclarationLogBookPageMobileDTO';
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
import { Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import { DeclarationLogBookTypeEnum } from '../../enums/declaration-log-book-type.enum';
import { BaseAuditService } from '../common-app/base-audit.service';


@Injectable({
    providedIn: 'root',
})
export class InspectionsService extends BaseAuditService {
    protected controller: string = 'Inspections';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Administrative);
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

    public getPatrolVehicles(isWaterVehicle: boolean): Observable<NomenclatureDTO<number>[]> {
        const params = new HttpParams().append('isWaterVehicle', isWaterVehicle.toString());

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

    public getDeclarationLogBookPages(type: DeclarationLogBookTypeEnum, shipId: number): Observable<DeclarationLogBookPageDTO[]> {
        const params = new HttpParams()
            .append('type', type.toString())
            .append('shipId', shipId.toString());

        return this.requestService.get(this.area, this.controller, 'GetDeclarationLogBookPages', {
            httpParams: params,
            responseTypeCtr: DeclarationLogBookPageDTO
        });
    }

    public getAquacultureOwner(aquacultureId: number): Observable<InspectionSubjectPersonnelDTO> {
        const params = new HttpParams().append('aquacultureId', aquacultureId.toString());

        return this.requestService.get(this.area, this.controller, 'GetAquacultureOwner', {
            httpParams: params,
            responseTypeCtr: InspectionSubjectPersonnelDTO,
        });
    }

    public get(id: number): Observable<InspectionEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'Get', {
            httpParams: params,
            responseTypeCtr: InspectionEditDTO,
        });
    }

    public getAll(request: GridRequestModel<InspectionsFilters>): Observable<GridResultModel<InspectionDTO>> {
        type Result = GridResultModel<InspectionDTO>;
        type Body = GridRequestModel<InspectionsFilters>;

        return this.requestService.post<Result, Body>(this.area, this.controller, 'GetAll', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel,
        }).pipe(switchMap((entries: Result) => {
            const ids = entries.records.map((insp: InspectionDTO) => insp.id!);

            if (ids.length === 0) {
                return of(entries);
            }

            return this.getInspectionAUANs(ids).pipe(map((auans) => {
                for (const auan of auans) {
                    const found = entries.records.find((entry: InspectionDTO) => {
                        return entry.id === auan.inspectionId;
                    });

                    if (found !== undefined) {
                        if (found.auaNs !== undefined && found.auaNs !== null) {
                            found.auaNs.push(auan);
                        }
                        else {
                            found.auaNs = [auan];
                        }
                    }
                }

                return entries;
            }));
        }));
    }

    public getInspectionAUANs(inspectionIds: number[]): Observable<AuanRegisterDTO[]> {
        return this.requestService.post(this.area, this.controller, 'GetInspectionAUANs', inspectionIds, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: AuanRegisterDTO,
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
}
