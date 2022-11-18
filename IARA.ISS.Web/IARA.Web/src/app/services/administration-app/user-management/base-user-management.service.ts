import { HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '../../common-app/base-audit.service';
import { IUserManagementService } from '@app/interfaces/administration-app/user-management.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { UserDTO } from '@app/models/generated/dtos/UserDTO';
import { UserManagementFilters } from '@app/models/generated/filters/UserManagementFilters';

export abstract class BaseUserManagementService extends BaseAuditService implements IUserManagementService {

    protected controller: string = 'UserManagement';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Administrative);
    }

    public abstract getAll(request: GridRequestModel<UserManagementFilters>): Observable<GridResultModel<UserDTO>>;

    public abstract getUser(id: number): Observable<any>;

    public delete(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.delete(this.area, this.controller, 'Delete', {
            httpParams: params,
            successMessage: 'succ-deleted-user'
        });
    }

    public undoDelete(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.patch(this.area, this.controller, 'UndoDelete', null, {
            httpParams: params,
            successMessage: 'succ-undo-delete-user'
        });
    }

    public sendChangePasswordEmail(userId: number): Observable<void> {
        const params = new HttpParams().append('userId', userId.toString());
        return this.requestService.patch(this.area, this.controller, 'SendChangePasswordEmail', null, { httpParams: params });
    }

    public impersonateUser(userId: number): Observable<string> {
        const params = new HttpParams().append('userId', userId.toString());
        return this.requestService.get(this.area, this.controller, 'Impersonate', { httpParams: params, responseType: 'text' });
    }
}