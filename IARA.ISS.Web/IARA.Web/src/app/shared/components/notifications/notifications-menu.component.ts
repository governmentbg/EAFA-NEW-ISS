import { AfterViewInit, Component, OnInit } from '@angular/core';
import { NotificationDTO } from '@app/models/generated/dtos/NotificationDTO';
import { NotificationsDTO } from '@app/models/generated/dtos/NotificationsDTO';
import { NotificationsHubService } from '@app/shared/notifications/notifications-hub-service';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { NotificationTypes } from '../../notifications/notification-types.enum';
import { Notification } from '../../notifications/notification';
import { PagedDatasource } from '../../utils/paged-datasource/paged-datasource';

@Component({
    selector: 'notifications-menu',
    templateUrl: './notifications-menu.component.html',
    styleUrls: ['./notifications.css']
})
export class NotificationsMenuComponent implements OnInit, AfterViewInit {

    public readonly dataSource: PagedDatasource<NotificationDTO>;
    public readonly notificationsHub: NotificationsHubService;

    public totalUnread: number;
    public totalRecordsCount: number;
    public pageSize: number;

    constructor(notificationsHub: NotificationsHubService) {
        this.totalUnread = 0;
        this.totalRecordsCount = 0;
        this.notificationsHub = notificationsHub;
        this.dataSource = new PagedDatasource<NotificationDTO>(this.getNotifications.bind(this));
        this.pageSize = this.dataSource.pageSize;
    }

    ngAfterViewInit(): void {
        this.dataSource.getFirstRecords();
    }

    ngOnInit(): void {
        this.dataSource.newDataReceived.subscribe(result => {
            this.totalRecordsCount = result.totalRecordsCount;
            this.totalUnread = (result as NotificationsDTO).totalUnread;
        });

        this.notificationsHub.newNotificationArrived.subscribe(notification => {
            if (notification.type == NotificationTypes.User) {
                let userNotification = new Notification<NotificationDTO>(notification);
                this.totalRecordsCount += this.dataSource.push(userNotification.message);

                this.totalUnread++;
            }
        });
    }

    private getNotifications(page: number, pageSize?: number): Promise<GridResultModel<NotificationDTO>> {

        if (pageSize == undefined) {
            pageSize = this.pageSize;
        }

        return this.notificationsHub.getUserNotifications(page, pageSize).then(result => {
            return new NotificationsDTO(result);
        });
    }


}