import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpParams } from '@angular/common/http';

import { BaseAuditService } from '@app/services/common-app/base-audit.service';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestService } from '@app/shared/services/request.service';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { PrintConfigurationDTO } from '@app/models/generated/dtos/PrintConfigurationDTO';
import { PrintConfigurationFilters } from '@app/models/generated/filters/PrintConfigurationFilters';
import { RequestProperties } from '@app/shared/services/request-properties';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PrintConfigurationEditDTO } from '@app/models/generated/dtos/PrintConfigurationEditDTO';
import { PrintUserNomenclatureDTO } from '@app/models/generated/dtos/PrintUserNomenclatureDTO';

@Injectable({
    providedIn: 'root'
})
export class PrintConfigurationsService extends BaseAuditService {
    protected readonly controller: string = 'PrintConfiguration';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Administrative);
    }

    public getAllPrintConfigurations(request: GridRequestModel<PrintConfigurationFilters>): Observable<GridResultModel<PrintConfigurationDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAllPrintConfigurations', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public getPrintConfiguration(id: number): Observable<PrintConfigurationEditDTO> {
        const params: HttpParams = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetPrintConfiguration', {
            httpParams: params,
            responseTypeCtr: PrintConfigurationEditDTO
        });
    }

    public addPrintConfiguratoin(model: PrintConfigurationEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddPrintConfiguration', model, {
            successMessage: 'succ-add-print-configuration'
        });
    }

    public editPrintConfiguration(model: PrintConfigurationEditDTO): Observable<void> {
        return this.requestService.put(this.area, this.controller, 'EditPrintConfiguration', model, {
            successMessage: 'succ-edit-print-configuration'
        });
    }

    public deletePrintConfiguration(id: number): Observable<void> {
        const params: HttpParams = new HttpParams().append('id', id.toString());

        return this.requestService.delete(this.area, this.controller, 'DeletePrintConfiguration', {
            httpParams: params,
            successMessage: 'succ-delete-print-configuration'
        });
    }

    public undoDeletePrintConfiguration(id: number): Observable<void> {
        const params: HttpParams = new HttpParams().append('id', id.toString());

        return this.requestService.patch(this.area, this.controller, 'UndoDeletePrintConfiguration', undefined, {
            httpParams: params,
            successMessage: 'succ-restore-print-configuration'
        });
    }

    // Nomenclatures

    public getApplicationTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetApplicationTypes', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getUsersNomenclature(): Observable<PrintUserNomenclatureDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetUsersNomenclature', {
            responseTypeCtr: PrintUserNomenclatureDTO
        });
    }
}