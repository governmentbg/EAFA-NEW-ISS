import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { RecreationalFishingAssociationDTO } from '@app/models/generated/dtos/RecreationalFishingAssociationDTO';
import { RecreationalFishingAssociationEditDTO } from '@app/models/generated/dtos/RecreationalFishingAssociationEditDTO';
import { RecreationalFishingPossibleAssociationLegalDTO } from '@app/models/generated/dtos/RecreationalFishingPossibleAssociationLegalDTO';
import { RecreationalFishingAssociationsFilters } from '@app/models/generated/filters/RecreationalFishingAssociationsFilters';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '../common-app/base-audit.service';

@Injectable({
    providedIn: 'root'
})
export class RecreationalFishingAssociationService extends BaseAuditService {
    protected controller: string = 'RecreationalFishingAssociations';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Administrative);
    }

    public getAllAssociations(request: GridRequestModel<RecreationalFishingAssociationsFilters>): Observable<GridResultModel<RecreationalFishingAssociationDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAllAssociations', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public getAssociation(id: number): Observable<RecreationalFishingAssociationEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetAssociation', {
            httpParams: params,
            responseTypeCtr: RecreationalFishingAssociationEditDTO
        });
    }

    public getPossibleAssociationLegals(): Observable<RecreationalFishingPossibleAssociationLegalDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetPossibleAssociationLegals', {
            responseTypeCtr: RecreationalFishingPossibleAssociationLegalDTO
        });
    }

    public getLegalForAssociation(id: number): Observable<RecreationalFishingAssociationEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetLegalForAssociation', {
            httpParams: params,
            responseTypeCtr: RecreationalFishingAssociationEditDTO
        });
    }

    public addAssociation(association: RecreationalFishingAssociationEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddAssociation', association, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public editAssociation(association: RecreationalFishingAssociationEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'EditAssociation', association, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public deleteAssociation(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.delete(this.area, this.controller, 'DeleteAssociation', { httpParams: params });
    }

    public undoDeleteAssociation(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.patch(this.area, this.controller, 'UndoDeleteAssociation', null, { httpParams: params });
    }

    public downloadAssociationFile(fileId: number, fileName: string): Observable<boolean> {
        const params = new HttpParams().append('id', fileId.toString());
        return this.requestService.download(this.area, this.controller, 'DownloadAssociationFile', fileName, { httpParams: params });
    }
}
