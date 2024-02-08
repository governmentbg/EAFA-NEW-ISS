import { Inject, Injectable } from '@angular/core';
import { SECURITY_SERVICE_TOKEN } from '@app/components/common-app/auth/di/auth-di.tokens';
import { ISecurityService } from '@app/components/common-app/auth/interfaces/security-service.interface';
import { Subject } from 'rxjs';
import { WebNotification } from './models/notification';
import { INotificationSecurity } from './models/notification-security.interface';
import { NotificationTypes } from './notification-types.enum';
import { SignalRHubService } from './signalr-hub.service';

@Injectable({
    providedIn: 'root'
})
export abstract class BaseNotificationsHubService extends SignalRHubService {

    private listeningForCheck: Map<NotificationTypes, boolean>;

    constructor(@Inject(SECURITY_SERVICE_TOKEN) securityService: ISecurityService, hubPath: string, apiBaseUrl: string) {
        super(securityService, hubPath, apiBaseUrl);
        this.listeningForCheck = new Map<NotificationTypes, boolean>();
        this.securityService = securityService;
    }

    public subscribeFor<T>(type: NotificationTypes, handler: (result: T) => void): Promise<boolean> {
        const subject: Subject<boolean> = new Subject<boolean>();
        this.securityService.isAuthenticated().then((isAuthenticated: boolean | undefined) => {

            if (isAuthenticated) {
                this.listenFor<T>(type, handler).then(result => {
                    if (isAuthenticated)
                        super.sendDataToHub<boolean>('SubscribeFor', type);

                    subject.next(isAuthenticated);
                    subject.complete();
                });
            } else {
                this.securityService.isAuthenticatedEvent.subscribe((result: boolean | undefined) => {

                    if (result)
                        super.sendDataToHub<boolean>('SubscribeFor', type);

                    subject.next(result);
                    subject.complete();
                });
            }
        });

        return subject.asObservable().toPromise().then(result => {
            return result ?? false;
        });
    }

    public unsubscribeFrom(type: NotificationTypes): Promise<boolean> {
        return super.sendDataToHub<boolean>('UnsubscribeFrom', type);
    }

    public notifyHub<T>(notificationType: NotificationTypes, message: T): Promise<void> {
        return this.sendDataToHub(`Notify${NotificationTypes[notificationType]}`, message);
    }

    protected listenFor<T>(notificationType: NotificationTypes, handler: (result: T) => void): Promise<boolean> {
        if (!this.checkListeningForType(notificationType)) {
            this.setCheckListeningForType(notificationType);
            const receiveHandler = `Receive${NotificationTypes[notificationType]}`;

            return this.startListeningFor<WebNotification<T>>(receiveHandler, result => {
                handler(result.message);
            }).then(() => {
                return true;
            });
        } else {
            return Promise.resolve(true);
        }
    }

    private checkListeningForType(notificationType: NotificationTypes): boolean {
        const listeningForType = this.listeningForCheck.get(notificationType);

        if (listeningForType == undefined) {
            this.listeningForCheck.set(notificationType, false);

            return false;
        } else {
            return listeningForType;
        }
    }

    private setCheckListeningForType(notificationType: NotificationTypes, value: boolean = true): boolean {
        this.listeningForCheck.set(notificationType, value);
        return value;
    }
}
