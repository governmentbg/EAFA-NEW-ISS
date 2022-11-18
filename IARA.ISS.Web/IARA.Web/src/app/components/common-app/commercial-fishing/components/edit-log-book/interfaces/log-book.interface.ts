import { Observable } from 'rxjs';

import { CommercialFishingLogBookEditDTO } from '@app/models/generated/dtos/CommercialFishingLogBookEditDTO';
import { RangeOverlappingLogBooksDTO } from '@app/models/generated/dtos/RangeOverlappingLogBooksDTO';
import { OverlappingLogBooksParameters } from '@app/shared/components/overlapping-log-books/models/overlapping-log-books-parameters.model';
import { LogBookForRenewalDTO } from '@app/models/generated/dtos/LogBookForRenewalDTO';
import { LogBookEditDTO } from '@app/models/generated/dtos/LogBookEditDTO';

export interface ILogBookService {
    getPermitLicenseLogBook(logBookPermitLicenseId: number): Observable<CommercialFishingLogBookEditDTO>;
    editLogBook(model: CommercialFishingLogBookEditDTO, ignoreLogBookConflicts: boolean): Observable<void>;
    getOverlappedLogBooks(parameters: OverlappingLogBooksParameters[]): Observable<RangeOverlappingLogBooksDTO[]>;
    getLogBooksForRenewal(permitLicenseRegisterId: number, showFinished: boolean): Observable<LogBookForRenewalDTO[]>;
    getLogBooksForRenewalByIds(permitLicenseRegisterIds: number[]): Observable<CommercialFishingLogBookEditDTO[] | LogBookEditDTO[]>;
}