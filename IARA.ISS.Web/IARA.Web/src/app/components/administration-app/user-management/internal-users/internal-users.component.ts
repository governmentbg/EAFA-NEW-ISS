import { Component } from '@angular/core';
import { InternalUserManagementService } from '@app/services/administration-app/user-management/internal-user-management.service';

@Component({
    selector: 'internal-users',
    templateUrl: './internal-users.component.html'
})
export class InternalUsersComponent {
    public constructor(
        public service: InternalUserManagementService
    ) { }
}