import { Observable } from 'rxjs';

import { CommercialFishingLogBookEditDTO } from '@app/models/generated/dtos/CommercialFishingLogBookEditDTO';
import { RangeOverlappingLogBooksDTO } from '@app/models/generated/dtos/RangeOverlappingLogBooksDTO';
import { OverlappingLogBooksParameters } from '@app/shared/components/overlapping-log-books/models/overlapping-log-books-parameters.model';
import { LogBookForRenewalDTO } from '@app/models/generated/dtos/LogBookForRenewalDTO';
import { LogBookEditDTO } from '@app/models/generated/dtos/LogBookEditDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';

export interface ILogBookService {
    getLogBook(logBookId: number): Observable<LogBookEditDTO>;
    getPermitLicenseLogBook(logBookPermitLicenseId: number): Observable<CommercialFishingLogBookEditDTO>;

    addLogBook(model: CommercialFishingLogBookEditDTO | LogBookEditDTO, registerId: number, ignoreLogBookConflicts: boolean): Observable<void>;
    editLogBook(model: CommercialFishingLogBookEditDTO | LogBookEditDTO, registerId: number, ignoreLogBookConflicts: boolean): Observable<void>;

    getOverlappedLogBooks(parameters: OverlappingLogBooksParameters[]): Observable<RangeOverlappingLogBooksDTO[]>;

    getLogBooksForRenewal(permitLicenseRegisterId: number, showFinished: boolean): Observable<LogBookForRenewalDTO[]>;
    getLogBooksForRenewalByIds(permitLicenseRegisterIds: number[]): Observable<CommercialFishingLogBookEditDTO[] | LogBookEditDTO[]>;

    getLogBookAudit(id: number): Observable<SimpleAuditDTO>;

    deleteLogBook(logBookId: number): Observable<void>;

    undoDeleteLogBook(logBookId: number): Observable<void>;
}