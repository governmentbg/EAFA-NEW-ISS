import { Observable } from 'rxjs';
import { ApplicationDeliveryDTO } from '@app/models/generated/dtos/ApplicationDeliveryDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { IBaseAuditService } from '../base-audit.interface';

export interface IDeliveryService extends IBaseAuditService {
    getDeliveryTypes(applicationId: number): Observable<NomenclatureDTO<number>[]>;
    getDeliveryData(deliveryId: number): Observable<ApplicationDeliveryDTO>;
    editDeliveryData(deliveryData: ApplicationDeliveryDTO, sendEDelivery: boolean): Observable<void>;
}