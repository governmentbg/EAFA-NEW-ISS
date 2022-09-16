import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { NotificationDTO } from '@app/models/generated/dtos/NotificationDTO';
import { NotificationsHubService } from '../../notifications/notifications-hub-service';

@Component({
    selector: 'notification',
    templateUrl: './notification.component.html',
    styleUrls: ['./notifications.css']
})
export class NotificationComponent {

    private router: Router;

    constructor(router: Router) {
        this.router = router;
    }

    @Input() public notification?: NotificationDTO;

    @Input() public notificationsHub!: NotificationsHubService;

    public markAsRead() {
        if (this.notification != undefined && this.notification.id != 0) {
            this.notificationsHub.markAsRead(this.notification?.id).then(isRead => {
                if (this.notification != undefined) {
                    this.notification.isRead = isRead;
                }
            });

        }

        this.navigate();
    }

    public navigate() {
        if (this.notification != undefined && this.notification.url != undefined) {
            if (this.notification.url.startsWith('/')) {
                this.router.navigateByUrl(this.notification.url);
            } else {
                window.location.href = this.notification.url;
            }
        }
    }
}