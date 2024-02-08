import { Inject, Injectable } from '@angular/core';
import { BaseGridRequestModel } from '@app/models/common/base-grid-request.model';
import { BaseNotificationsHubService } from './base-notifications-hub.service';
import { INotificationSecurity } from './models/notification-security.interface';
import { UserNotificationsList } from './models/user-notifications-list.model';

@Injectable({
    providedIn: 'root'
})
export class NotificationsHubService extends BaseNotificationsHubService {

    public constructor(@Inject("INotificationSecurity") securityService: INotificationSecurity, hubPath: string, apiBaseUrl: string) {
        super(securityService, hubPath, apiBaseUrl);
    }

    public getUserNotifications(page: number, pageSize: number): Promise<UserNotificationsList> {
        const request = new BaseGridRequestModel();
        request.pageNumber = page;
        request.pageSize = pageSize;

        try {
            return this.sendDataToHub<UserNotificationsList>('GetUserNotifications', request);
        } catch (e) {
            return Promise.reject(e);
        }
    }

    public markAsRead(notificationId: number): Promise<boolean> {
        return this.sendDataToHub<boolean>('MarkNotificationAsRead', notificationId);
    }

    public markAsUnRead(notificationId: number): Promise<boolean> {
        return this.sendDataToHub<boolean>('MarkNotificationAsUnRead', notificationId);
    }

    public markAllNotificationsAsRead(): Promise<boolean> {
        return this.sendDataToHub<boolean>('MarkAllNotificationsAsRead');
    }
}
