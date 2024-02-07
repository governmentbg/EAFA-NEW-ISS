import { Inject, Injectable } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { Environment } from '@env/environment';
import { SignalRHubService } from '@app/shared/notifications/signalr-hub.service';
import { INotificationSecurity } from '@app/shared/notifications/models/notification-security.interface';

@Injectable({
    providedIn: 'root'
})
export class StatisticsService extends SignalRHubService {

    protected getToken(): string | Promise<string> {
        return this.oidSecurityService.getToken();
    }

    private oidSecurityService: OidcSecurityService;

    constructor(@Inject("INotificationSecurity") securityService: INotificationSecurity, oidSecurityService: OidcSecurityService) {
        super(securityService, `${Environment.Instance.apiBasePath}/statistics`, Environment.Instance.servicesBaseUrl);
        this.oidSecurityService = oidSecurityService;
    }
}

export enum StatisticMethods {
    GetRawUsageData = "GetRawUsageData",
    GetStatistics = "GetStatistics"
}