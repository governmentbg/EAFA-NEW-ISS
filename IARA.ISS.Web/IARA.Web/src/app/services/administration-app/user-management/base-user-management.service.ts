import { HttpParams } from '@angular/common/http';
import { IUserManagementService } from '@app/interfaces/administration-app/user-management.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { UserDTO } from '@app/models/generated/dtos/UserDTO';
import { UserManagementFilters } from '@app/models/generated/filters/UserManagementFilters';
import { SecurityService } from '@app/services/common-app/security.service';
import { TLSnackbar } from '@app/shared/components/snackbar/tl.snackbar';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestService } from '@app/shared/services/request.service';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { Observable } from 'rxjs';
import { BaseAuditService } from '../../common-app/base-audit.service';

export abstract class BaseUserManagementService extends BaseAuditService implements IUserManagementService {

    protected controller: string = 'UserManagement';
    protected securityService: SecurityService;
    protected translationService: FuseTranslationLoaderService;
    protected snackbar: TLSnackbar;

    public constructor(requestService: RequestService, securityService: SecurityService,
        translationService: FuseTranslationLoaderService, snackbar: TLSnackbar) {
        super(requestService, AreaTypes.Administrative);
        this.securityService = securityService;
        this.translationService = translationService;
        this.snackbar = snackbar;
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

    public impersonateUser(username: string): void {
        this.securityService.impersonateUser(username).then(result => {
            const message: string = this.translationService.getValue('service.successful-message');
            this.snackbar.success(message);
        });
    }
}