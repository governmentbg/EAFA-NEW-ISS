import { AfterViewInit, Component, Inject, OnInit } from '@angular/core';
import { SECURITY_SERVICE_TOKEN } from '@app/components/common-app/auth/di/auth-di.tokens';
import { ISecurityService } from '@app/components/common-app/auth/interfaces/security-service.interface';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { NotificationDTO } from '@app/models/generated/dtos/NotificationDTO';
import { UserNotification } from '@app/shared/notifications/models/user-notification.model';
import { UserNotificationsList } from '@app/shared/notifications/models/user-notifications-list.model';
import { NotificationsHubService } from '@app/shared/notifications/notifications-hub-service';
import { Environment } from '@env/environment';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NotificationTypes } from '@app/shared/notifications/notification-types.enum';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { PagedDatasource } from '@app/shared/utils/paged-datasource/paged-datasource';

@Component({
    selector: 'notifications-menu',
    templateUrl: './notifications-menu.component.html',
    styleUrls: ['./notifications.scss']
})
export class NotificationsMenuComponent implements OnInit, AfterViewInit {
    public readonly DEFAULT_RECORD_SIZE: number = 155;
    public readonly NOTIFICATIONS_GAP: number = 7;
    public readonly faIconSize: number = CommonUtils.FA_ICON_SIZE;

    public readonly notificationsHub: NotificationsHubService;
    public dataSource: PagedDatasource<NotificationDTO>;

    public totalUnread: number;
    public totalRecordsCount: number;
    public pageSize: number;
    public recordSize: number;

    public constructor(@Inject(SECURITY_SERVICE_TOKEN) securityService: ISecurityService) {
        this.notificationsHub = new NotificationsHubService(securityService, "/notifications", Environment.Instance.apiBaseUrl ?? '');
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

            const unread: number = (result as UserNotificationsList).totalUnread;
            this.totalUnread = unread > 0 ? unread : 0;
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
        if (this.totalUnread > 0) {
            --this.totalUnread;
        }
        else {
            this.totalUnread = 0;
        }
    }

    public onMenuOpened(): void {
        this.resize();
    }

    public markAllAsRead(): void {
        this.notificationsHub.markAllNotificationsAsRead().then(result => {
            if (result) {
                this.totalUnread = 0;
                this.dataSource = new PagedDatasource<NotificationDTO>(this.getNotifications.bind(this));
                this.pageSize = this.dataSource.pageSize;
                this.subscribeOnNewDataReceived();
                this.dataSource.getFirstRecords();
            }
        });
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