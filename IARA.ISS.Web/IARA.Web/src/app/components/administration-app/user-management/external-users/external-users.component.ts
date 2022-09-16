import { Component } from '@angular/core';
import { ExternalUserManagementService } from '@app/services/administration-app/user-management/external-user-management.service';

@Component({
    selector: 'external-users',
    templateUrl: './external-users.component.html'
})
export class ExternalUsersComponent {
    public constructor(
        public service: ExternalUserManagementService
    ) { }
}