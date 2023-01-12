import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { IdentifierTypeEnum } from '@app/enums/identifier-type.enum';
import { LegalFullDataDTO } from '@app/models/generated/dtos/LegalFullDataDTO';
import { PersonFullDataDTO } from '@app/models/generated/dtos/PersonFullDataDTO';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from './base-audit.service';

@Injectable({
    providedIn: 'root'
})
export class PersonLegalExtractorService extends BaseAuditService{
    protected controller: string = 'PersonLegalExtractor';

    public constructor(http: RequestService) {
        super(http, AreaTypes.Common);
    }

    public tryGetPerson(identifierType: IdentifierTypeEnum, identifier: string): Observable<PersonFullDataDTO | undefined> {
        const params = new HttpParams()
            .append('identifierType', identifierType.toString())
            .append('identifier', identifier);

        return this.requestService.get(this.area, this.controller, 'TryGetPerson', {
            httpParams: params,
            responseTypeCtr: PersonFullDataDTO
        });
    }

    public tryGetLegal(eik: string): Observable<LegalFullDataDTO | undefined> {
        const params = new HttpParams().append('eik', eik);

        return this.requestService.get(this.area, this.controller, 'TryGetLegal', {
            httpParams: params,
            responseTypeCtr: LegalFullDataDTO
        });
    }
}