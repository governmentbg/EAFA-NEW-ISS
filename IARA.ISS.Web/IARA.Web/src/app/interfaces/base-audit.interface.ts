import { Observable } from "rxjs";
import { SimpleAuditDTO } from "../models/generated/dtos/SimpleAuditDTO";

export interface IBaseAuditService {
    getSimpleAudit(id: number): Observable<SimpleAuditDTO>;
}