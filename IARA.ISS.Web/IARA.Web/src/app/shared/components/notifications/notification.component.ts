import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router } from '@angular/router';
import { NotificationDTO } from '@app/models/generated/dtos/NotificationDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { NotificationsHubService } from '../../notifications/notifications-hub-service';

@Component({
    selector: 'notification',
    templateUrl: './notification.component.html',
    styleUrls: ['./notifications.scss']
})
export class NotificationComponent {
    @Input()
    public notification!: NotificationDTO;

    @Input()
    public notificationsHub!: NotificationsHubService;

    @Output()
    public markedAsRead: EventEmitter<number> = new EventEmitter<number>();

    private readonly router: Router;

    public constructor(router: Router) {
        this.router = router;
    }

    public markAsRead(event: PointerEvent): void {
        event.stopPropagation();
        event.preventDefault();

        if (!this.notification.isRead) {
            this.notificationsHub.markAsRead(this.notification.id).then((isRead: boolean) => {
                this.notification.isRead = isRead;
                this.markedAsRead.emit(this.notification.id);
            });
        }
    }

    public navigate(): void {
        if (this.notification.url && this.notification.url.length !== 0) {
            if (this.notification.url.startsWith('/')) {
                const url: string = this.notification.url.split('?')[0];
                const tableId: string | undefined = this.notification.url.split('?')[1]?.split('&')?.find(x => x.startsWith('tableId'))?.split('=')[1];
                if (tableId) {
                    this.router.navigateByUrl(url, { state: { tableId: Number(tableId) } });
                }
                else {
                    this.router.navigateByUrl(this.notification.url);
                }
            }
            else {
                window.location.href = this.notification.url;
            }
        }
        else if (this.notification.pageCode && this.notification.tableId) {
            switch (this.notification.pageCode) {
                case PageCodeEnum.AquaFarmReg:
                    this.router.navigateByUrl('/aquaculture-farms', { state: { tableId: this.notification.tableId } });
                    break;
                case PageCodeEnum.AuanRegister:
                    this.router.navigateByUrl('/auan-register', { state: { tableId: this.notification.tableId } });
                    break;
            }
        }
    }
}