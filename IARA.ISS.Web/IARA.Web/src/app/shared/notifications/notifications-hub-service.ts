import { Injectable } from '@angular/core';
import { BaseGridRequestModel } from '@app/models/common/base-grid-request.model';
import { NotificationDTO } from '@app/models/generated/dtos/NotificationDTO';
import { NotificationsDTO } from '@app/models/generated/dtos/NotificationsDTO';
import { Environment } from '@env/environment';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { Notification } from './notification';
import { NotificationTypes } from './notification-types.enum';
import { SignalRHubService } from './signalr-hub.service';

@Injectable({
    providedIn: 'root'
})
export class NotificationsHubService extends SignalRHubService {

    private oidcSecurityService: OidcSecurityService;

    constructor(oidcSecurityService: OidcSecurityService) {
        const urlPath = `${Environment.Instance.apiBasePath}/notifications`;
        super(urlPath, Environment.Instance.servicesBaseUrl);
        console.log(urlPath);
        console.log(Environment.Instance.servicesBaseUrl);
        this.oidcSecurityService = oidcSecurityService;
    }

    public updateUserData(token: string): Promise<boolean> {
        try {
            return super.sendDataToHub<boolean>('UpdateUserData', token).then(result => {
                if (result) {
                    this.listenForUserNotifications();
                }

                return result;
            });
        } catch (e) {
            return Promise.reject(e);
        }
    }

    public markAsRead(notificationId: number): Promise<boolean> {
        return this.sendDataToHub<boolean>('MarkNotificationAsRead', notificationId);
    }

    public listenForUserNotifications() {
        this.startListeningFor<Notification<NotificationDTO>>('ReceiveUser', (result) => {
            this.newNotificationArrived.next(result);
        });
    }

    public subscribeFor(type: NotificationTypes): Promise<boolean> {
        return super.sendDataToHub<boolean>('SubscribeFor', type);
    }

    public unsubscribeFrom(type: NotificationTypes): Promise<boolean> {
        return super.sendDataToHub<boolean>('UnsubscribeFrom', type);
    }

    public getUserNotifications(page: number, pageSize: number): Promise<NotificationsDTO> {
        const request = new BaseGridRequestModel();
        request.pageNumber = page;
        request.pageSize = pageSize;

        try {
            return this.sendDataToHub<NotificationsDTO>('GetUserNotifications', request);
        } catch (e) {
            return Promise.reject(e);
        }
    }

    protected getToken(): string | Promise<string> {
        return this.oidcSecurityService.getToken();
    }
}