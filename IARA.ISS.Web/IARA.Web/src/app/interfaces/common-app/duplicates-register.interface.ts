import { Observable } from 'rxjs';

import { DuplicatesRegisterEditDTO } from '@app/models/generated/dtos/DuplicatesRegisterEditDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PermitLicenseNomenclatureDTO } from '@app/models/generated/dtos/PermitLicenseNomenclatureDTO';
import { QualifiedFisherNomenclatureDTO } from '@app/models/generated/dtos/QualifiedFisherNomenclatureDTO';
import { IBaseAuditService } from '../base-audit.interface';
import { IApplicationsActionsService } from './application-actions.interface';

export interface IDuplicatesRegisterService extends IApplicationsActionsService, IBaseAuditService {
    getDuplicateRegister(id: number): Observable<DuplicatesRegisterEditDTO>;
    addDuplicateRegister(duplicate: DuplicatesRegisterEditDTO): Observable<number>;
    addAndDownloadDuplicateRegister(duplicate: DuplicatesRegisterEditDTO): Observable<boolean>
    downloadDuplicate(id: number): Observable<boolean>;

    getRegisteredBuyers(): Observable<NomenclatureDTO<number>[]>;
    getPermits(): Observable<NomenclatureDTO<number>[]>;
    getPermitLicenses(): Observable<PermitLicenseNomenclatureDTO[]>;
    getQualifiedFishers(): Observable<QualifiedFisherNomenclatureDTO[]>;
}