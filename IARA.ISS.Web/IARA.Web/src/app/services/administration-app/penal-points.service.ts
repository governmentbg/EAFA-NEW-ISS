import { Injectable } from '@angular/core';
import { RequestService } from '@app/shared/services/request.service';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { BaseAuditService } from '@app/services/common-app/base-audit.service';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { Observable } from 'rxjs';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { RequestProperties } from '@app/shared/services/request-properties';
import { HttpParams } from '@angular/common/http';
import { IPenalPointsService } from '@app/interfaces/administration-app/penal-points.interface';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PenalPointsEditDTO } from '@app/models/generated/dtos/PenalPointsEditDTO';
import { PenalPointsDTO } from '@app/models/generated/dtos/PenalPointsDTO';
import { PenalPointsAuanDecreeDataDTO } from '@app/models/generated/dtos/PenalPointsAuanDecreeDataDTO';
import { PenalPointsFilters } from '@app/models/generated/filters/PenalPointsFilters';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { PermitNomenclatureDTO } from '@app/models/generated/dtos/PermitNomenclatureDTO';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { map } from 'rxjs/operators';
import { PenalPointsOrderDTO } from '@app/models/generated/dtos/PenalPointsOrderDTO';

@Injectable({
    providedIn: 'root'
})
export class PenalPointsService extends BaseAuditService implements IPenalPointsService {
    protected controller: string = 'PenalPoints';

    private readonly translate: FuseTranslationLoaderService;

    public constructor(requestService: RequestService,
        translate: FuseTranslationLoaderService
    ) {
        super(requestService, AreaTypes.Administrative);
        this.translate = translate;
    }

    public getAllPenalPoints(request: GridRequestModel<PenalPointsFilters>): Observable<GridResultModel<PenalPointsDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAllPenalPoints', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public getPenalPoints(id: number): Observable<PenalPointsEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetPenalPoints', {
            httpParams: params,
            responseTypeCtr: PenalPointsEditDTO
        });
    }

    public addPenalPoints(auan: PenalPointsEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddPenalPoints', auan, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public editPenalPoints(auan: PenalPointsEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'EditPenalPoints', auan, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public deletePenalPoints(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.delete(this.area, this.controller, 'DeletePenalPoints', { httpParams: params });
    }

    public undoDeletePenalPoints(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.patch(this.area, this.controller, 'UndoDeletePenalPoints', null, { httpParams: params });
    }

    public getPenalPointsAuanDecreeData(decreeId: number): Observable<PenalPointsAuanDecreeDataDTO> {
        const params = new HttpParams().append('decreeId', decreeId.toString());

        return this.requestService.get(this.area, this.controller, 'GetPenalPointsAuanDecreeData', {
            httpParams: params,
            responseTypeCtr: PenalPointsAuanDecreeDataDTO
        });
    }

    public downloadFile(fileId: number, fileName: string): Observable<boolean> {
        const params = new HttpParams().append('id', fileId.toString());
        return this.requestService.download(this.area, this.controller, 'DownloadFile', fileName, { httpParams: params });
    }

    public getAllPenalDecrees(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetAllPenalDecrees', { responseTypeCtr: NomenclatureDTO });
    }

    public getAllPenalPointsStatuses(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetAllPenalPointsStatuses', { responseTypeCtr: NomenclatureDTO });
    }

    public getPermitNomenclatures(shipId: number, onlyPoundNet: boolean): Observable<PermitNomenclatureDTO[]> {
        const params = new HttpParams().append('shipId', shipId.toString()).append('onlyPoundNet', onlyPoundNet.toString());

        return this.requestService.get<PermitNomenclatureDTO[]>(this.area, this.controller, 'GetShipPermits', {
            httpParams: params,
            responseTypeCtr: PermitNomenclatureDTO
        }).pipe(map((permits: PermitNomenclatureDTO[]) => {
            for (const permit of permits) {
                const shipOwnerNamesResource: string = this.translate.getValue('commercial-fishing.permit-nomenclature-ship-owner-names');

                if (permit.displayName !== null && permit.displayName !== undefined && permit.displayName!.length > 0) {
                    permit.displayName += ` | ${shipOwnerNamesResource}: ${permit.shipOwnerName}`;
                }
                else {
                    permit.displayName = `${shipOwnerNamesResource}: ${permit.shipOwnerName}`;
                }
            }

            return permits;
        }));
    }

    public getPermitLicensesNomenclatures(shipId: number): Observable<PermitNomenclatureDTO[]> {
        const params = new HttpParams().append('shipId', shipId.toString());

        return this.requestService.get<PermitNomenclatureDTO[]>(this.area, this.controller, 'GetShipPermitLicenses', {
            httpParams: params,
            responseTypeCtr: PermitNomenclatureDTO
        }).pipe(map((permits: PermitNomenclatureDTO[]) => {
            for (const permit of permits) {
                const captain: string = this.translate.getValue('commercial-fishing.permit-nomenclature-captain');

                if (permit.displayName !== null && permit.displayName !== undefined && permit.displayName!.length > 0) {
                    permit.displayName += ` | ${captain}: ${permit.captainName}`;
                }
                else {
                    permit.displayName = `${captain}: ${permit.captainName}`;
                }
            }

            return permits;
        }));
    }

    public getPermitOrders(ownerId: number, isFisher: boolean, isPermitOwnerPerson: boolean): Observable<PenalPointsOrderDTO[]> {
        const params = new HttpParams().append('ownerId', ownerId.toString())
            .append('isFisher', isFisher.toString())
            .append('isPermitOwnerPerson', isPermitOwnerPerson.toString());

        return this.requestService.get(this.area, this.controller, 'GetPermitOrders', {
            httpParams: params,
            responseTypeCtr: PenalPointsOrderDTO
        });
    }

    public getPenalPointsStatusAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetPenalPointsStatusSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }
}