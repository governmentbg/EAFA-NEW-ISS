import { Observable } from 'rxjs';

import { PageCodeEnum } from '@app/enums/page-code.enum';
import { SuspensionDataDTO } from '@app/models/generated/dtos/SuspensionDataDTO';
import { SuspensionReasonNomenclatureDTO } from '@app/models/generated/dtos/SuspensionReasonNomenclatureDTO';
import { SuspensionTypeNomenclatureDTO } from '@app/models/generated/dtos/SuspensionTypeNomenclatureDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';

export interface ISuspensionService {
    getSuspensions(recordId: number, pageCode: PageCodeEnum): Observable<SuspensionDataDTO[]>;

    getSuspensionTypes(): Observable<SuspensionTypeNomenclatureDTO[]>;
    getSuspensionReasons(): Observable<SuspensionReasonNomenclatureDTO[]>;

    addSuspension(suspension: SuspensionDataDTO, id: number, pageCode: PageCodeEnum): Observable<void>;
    addEditSuspensions(recordId: number, suspensions: SuspensionDataDTO[], pageCode: PageCodeEnum): Observable<void>;

    getPermitSuspensionAudit(id: number): Observable<SimpleAuditDTO>;
    getPermitLicenseSuspensionAudit(id: number): Observable<SimpleAuditDTO>;
}