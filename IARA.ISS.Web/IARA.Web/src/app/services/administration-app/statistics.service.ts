import { Injectable } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { Environment } from '@env/environment';
import { SignalRHubService } from '@app/shared/notifications/signalr-hub.service';

@Injectable({
    providedIn: 'root'
})
export class StatisticsService extends SignalRHubService {

    protected getToken(): string | Promise<string> {
        return this.oidSecurityService.getToken();
    }

    private oidSecurityService: OidcSecurityService;

    constructor(oidSecurityService: OidcSecurityService) {
        super(`${Environment.Instance.apiBasePath}/statistics`, Environment.Instance.servicesBaseUrl);
        this.oidSecurityService = oidSecurityService;
    }
}

export enum StatisticMethods {
    GetRawUsageData = "GetRawUsageData",
    GetStatistics = "GetStatistics"
}