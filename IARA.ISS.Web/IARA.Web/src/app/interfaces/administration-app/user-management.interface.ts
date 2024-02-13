import { GridRequestModel } from "@app/models/common/grid-request.model";
import { GridResultModel } from "@app/models/common/grid-result.model";
import { UserDTO } from "@app/models/generated/dtos/UserDTO";
import { UserManagementFilters } from "@app/models/generated/filters/UserManagementFilters";
import { Observable } from "rxjs";
import { IBaseAuditService } from "../base-audit.interface";

export interface IUserManagementService extends IBaseAuditService {
    getAll(request: GridRequestModel<UserManagementFilters>): Observable<GridResultModel<UserDTO>>;
    getUser(id: number): Observable<any>;
    delete(id: number): Observable<void>;
    undoDelete(id: number): Observable<void>;
    sendChangePasswordEmail(userId: number): Observable<void>;
    impersonateUser(username: string): void;
}