import { Inject, Injectable } from '@angular/core';
import { SECURITY_SERVICE_TOKEN } from '@app/components/common-app/auth/di/auth-di.tokens';
import { ISecurityService } from '@app/components/common-app/auth/interfaces/security-service.interface';
import { SignalRHubService } from '@app/shared/notifications/signalr-hub.service';
import { Environment } from '@env/environment';

@Injectable({
    providedIn: 'root'
})
export class StatisticsService extends SignalRHubService {

    protected getToken(): string | Promise<string> {
        return this.securityService.token ?? '';
    }

    constructor(@Inject(SECURITY_SERVICE_TOKEN) securityService: ISecurityService) {
        super(securityService, `${Environment.Instance.apiBasePath}/statistics`, Environment.Instance.servicesBaseUrl);
    }
}

export enum StatisticMethods {
    GetRawUsageData = "GetRawUsageData",
    GetStatistics = "GetStatistics"
}