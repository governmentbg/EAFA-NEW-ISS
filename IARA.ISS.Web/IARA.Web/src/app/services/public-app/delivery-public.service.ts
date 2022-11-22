import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { IDeliveryService } from '../../interfaces/common-app/delivery.interface';
import { ApplicationDeliveryDTO } from '@app/models/generated/dtos/ApplicationDeliveryDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '../common-app/base-audit.service';

@Injectable({
    providedIn: 'root'
})
export class DeliveryPublicService extends BaseAuditService implements IDeliveryService {
    protected readonly controller: string = 'DeliveryAdministration';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Administrative);
    }

    public getDeliveryTypes(applicationId: number): Observable<NomenclatureDTO<number>[]> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.get(this.area, this.controller, 'GetDeliveryTypes', {
            httpParams: params,
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getDeliveryData(deliveryId: number): Observable<ApplicationDeliveryDTO> {
        const params = new HttpParams().append('deliveryId', deliveryId.toString());

        return this.requestService.get(this.area, this.controller, 'GetDeliveryData', {
            httpParams: params,
            responseTypeCtr: ApplicationDeliveryDTO
        });
    }

    public addDeliveryData(applicationId: number, deliveryData: ApplicationDeliveryDTO): Observable<number> {
        throw new Error('Method should not be called from public app');
    }

    public editDeliveryData(deliveryData: ApplicationDeliveryDTO, sendEDelivery: boolean): Observable<void> {
        throw new Error('Method should not be called from public app');
    }

    public downloadFile(fileId: number): Observable<boolean> {
        throw new Error('Method should not be called from public app');
    }
}