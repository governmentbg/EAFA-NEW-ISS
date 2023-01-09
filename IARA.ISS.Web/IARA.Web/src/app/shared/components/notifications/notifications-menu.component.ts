import { AfterViewInit, Component, Inject, OnInit } from '@angular/core';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { NotificationDTO } from '@app/models/generated/dtos/NotificationDTO';
import { INotificationSecurity } from '@app/shared/notifications/models/notification-security.interface';
import { UserNotification } from '@app/shared/notifications/models/user-notification.model';
import { UserNotificationsList } from '@app/shared/notifications/models/user-notifications-list.model';
import { NotificationsHubService } from '@app/shared/notifications/notifications-hub-service';
import { Environment } from '@env/environment';
import { NotificationTypes } from '../../notifications/notification-types.enum';
import { PagedDatasource } from '../../utils/paged-datasource/paged-datasource';

@Component({
    selector: 'notifications-menu',
    templateUrl: './notifications-menu.component.html',
    styleUrls: ['./notifications.scss']
})
export class NotificationsMenuComponent implements OnInit, AfterViewInit {
    public readonly DEFAULT_RECORD_SIZE: number = 155;
    public readonly NOTIFICATIONS_GAP: number = 7;

    public readonly notificationsHub: NotificationsHubService;
    public readonly dataSource: PagedDatasource<NotificationDTO>;

    public totalUnread: number;
    public totalRecordsCount: number;
    public pageSize: number;
    public recordSize: number;

    public constructor(@Inject('INotificationSecurity') notificationsSecurity: INotificationSecurity) {
        this.notificationsHub = new NotificationsHubService(notificationsSecurity, "/notifications", Environment.Instance.apiBaseUrl ?? '');
        this.dataSource = new PagedDatasource<NotificationDTO>(this.getNotifications.bind(this));

        this.totalUnread = 0;
        this.totalRecordsCount = 0;
        this.pageSize = this.dataSource.pageSize;
        this.recordSize = this.DEFAULT_RECORD_SIZE;
    }

    public ngOnInit(): void {
        this.subscribeOnNewDataReceived();
    }

    private subscribeOnNewDataReceived(): void {
        this.dataSource.newDataReceived.subscribe(result => {
            this.totalRecordsCount = result.totalRecordsCount;
            this.totalUnread = (result as UserNotificationsList).totalUnread;
            this.resize();
        });
    }

    public ngAfterViewInit(): void {
        this.notificationsHub.subscribeFor<UserNotification>(NotificationTypes.User, notification => {
            const userNotification = new UserNotification(notification);
            this.totalRecordsCount += this.dataSource.push(userNotification);
            this.totalUnread++;
            this.dataSource.getFirstRecords();
        }).then(isAuthenticated => {
            if (isAuthenticated)
                this.dataSource.getFirstRecords();
        });
    }

    public markedAsRead(notificationId: number): void {
        --this.totalUnread;
    }

    public onMenuOpened(): void {
        this.resize();
    }

    private getNotifications(page: number, pageSize?: number): Promise<GridResultModel<UserNotification>> {
        if (pageSize == undefined) {
            pageSize = this.pageSize;
        }

        return this.notificationsHub.getUserNotifications(page, pageSize).then(result => {
            return new UserNotificationsList(result);
        });
    }

    private resize(): void {
        // setTimeout necessary to wait for the panel to open
        setTimeout(() => {
            const elements: HTMLElement[] = Array.from(document.getElementsByTagName('notification')) as HTMLElement[];

            if (elements.length > 0) {
                let height: number = elements[0].offsetHeight;

                for (let i = 1; i < elements.length; ++i) {
                    if (elements[i].offsetHeight > height) {
                        height = elements[i].offsetHeight;
                    }
                }

                this.recordSize = height;
            }
            else {
                this.recordSize = this.DEFAULT_RECORD_SIZE;
            }
        });
    }
}