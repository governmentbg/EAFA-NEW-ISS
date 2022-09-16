import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { Observable } from 'rxjs';

export interface IHeaderAuditButton {
    tooltip?: string;
    getAuditRecordData: (id: number) => Observable<SimpleAuditDTO>;
    id: number;
    tableName?: string;
}