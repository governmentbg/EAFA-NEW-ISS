import { Injectable } from '@angular/core';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { NotificationTypes } from '@app/shared/notifications/notification-types.enum';
import { NotificationsHubService } from '@app/shared/notifications/notifications-hub-service';
import { RequestService } from '@app/shared/services/request.service';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';


@Injectable({
    providedIn: 'root'
})
export class CheckVersionService {

    private requestService: RequestService;
    private notificationsHub: NotificationsHubService;
    private confirmDialog: TLConfirmDialog;
    private translateService: FuseTranslationLoaderService;

    private _version: string = '1.0.0';

    public get Version(): string {
        return this._version;
    }

    constructor(requestService: RequestService,
        notificationsHub: NotificationsHubService,
        confirmDialog: TLConfirmDialog,
        translateService: FuseTranslationLoaderService) {

        this.confirmDialog = confirmDialog;
        this.requestService = requestService;
        this.translateService = translateService;
        this.notificationsHub = notificationsHub;
    }

    public initialize(): void {
        this.checkVersion().subscribe(result => {
            this._version = result.version;
            this.subscribeForVersionChange();
            this.subscribeForReconnect();
        });
    }

    private subscribeForVersionChange(): void {
        this.notificationsHub.subscribeFor<string>(NotificationTypes.Version, this.handleVersionChange.bind(this));
    }

    private subscribeForReconnect(): void {
        this.notificationsHub.reconnectedEvent.subscribe((connectionId) => {
            this.checkVersion().pipe(map(result => result.version)).subscribe(this.handleVersionChange.bind(this));
        });
    }

    private checkVersion(): Observable<{ version: string }> {
        return this.requestService.get<{ version: string }>(AreaTypes.Common, 'Version', 'Get');
    }

    private handleVersionChange(version: string): void {
        if (this.Version != version) {
            this._version = version;
            this.refreshPage();
        }
    }

    private refreshPage(): void {
        this.confirmDialog.open({
            title: this.translateService.getValue('common.version-title'),
            message: this.translateService.getValue('common.version-message'),
            okBtnLabel: this.translateService.getValue('common.yes'),
            cancelBtnLabel: this.translateService.getValue('common.no'),
            hasCancelButton: true
        }).subscribe({
            next: (result: boolean) => {
                if (result) {
                    window.location.reload();
                }
            }
        });
    }
}
