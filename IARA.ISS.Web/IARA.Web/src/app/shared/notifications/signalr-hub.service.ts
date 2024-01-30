import { Inject } from '@angular/core';
import { SECURITY_SERVICE_TOKEN } from '@app/components/common-app/auth/di/auth-di.tokens';
import { ISecurityService } from '@app/components/common-app/auth/interfaces/security-service.interface';
import {
    HubConnection,
    HubConnectionBuilder,
    HubConnectionState,
    LogLevel,
    MessageHeaders
} from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { BaseNotification } from './models/base-notification';

export abstract class SignalRHubService {

    private connection!: HubConnection;
    private _newDataArrived: Subject<BaseNotification>;
    protected securityService: ISecurityService;
    private startedListening: boolean = false;
    private url!: string;

    constructor(@Inject(SECURITY_SERVICE_TOKEN) securityService: ISecurityService, hubPath: string, apiBaseUrl: string) {
        this._newDataArrived = new Subject<BaseNotification>();
        this.securityService = securityService;
        if (apiBaseUrl == undefined) {
            apiBaseUrl = '';
        }

        this.url = `${apiBaseUrl}${hubPath}`;
        //const authorizationValue = `Bearer ${token}`;
        //const headers = { "Authorization": authorizationValue } as MessageHeaders;
    }

    protected getToken(): string | Promise<string> {
        return this.securityService.token ?? '';
    }

    public get newNotificationArrived(): Subject<BaseNotification> {
        return this._newDataArrived;
    }

    protected stopListening(): Promise<boolean> {
        if (this.startedListening) {
            this.startedListening = false;
            return this.disconnect();
        } else {
            return Promise.resolve(false);
        }
    }

    private disconnect(): Promise<boolean> {
        if (this.connection.state == HubConnectionState.Connected) {
            return this.connection.stop().then(() => {
                return true;
            });
        } else {
            return Promise.resolve(true);
        }
    }

    protected async startListeningFor<T>(eventName: string, handler: (result: T) => void) {
        if (await this.connect()) {
            this.connection.on(eventName, handler);
        }
    }

    protected stopListeningFor(eventName: string): void {
        this.connection.off(eventName);
    }

    private connecting?: Subject<boolean>;

    protected connect(): Promise<boolean> {

        const connection = this.buildConnection();

        if (this.connection.state == HubConnectionState.Disconnected) {
            this.connecting = new Subject<boolean>();
            connection.start().then(() => {
                this.connecting?.next(true);
                this.connecting?.complete();
                return true;
            });
        }

        return this.toConnectingResult(this.connecting);
    }

    private toConnectingResult(connecting: Subject<boolean> | undefined): Promise<boolean> {
        if (connecting != undefined && !connecting.closed) {
            return connecting.asObservable().toPromise().then(result => {
                return result ?? false;
            });
        } else if (connecting != undefined && connecting.closed) {
            return Promise.resolve(true);
        } else {
            return Promise.resolve(false);
        }
    }

    protected sendDataToHub<T>(methodName: string, data?: any): Promise<T> {

        if (this.connection == undefined || this.connection.state != HubConnectionState.Connected) {
            return this.connect().then((result) => {
                return this.invoke<T>(methodName, data);
            });
        } else {
            return this.invoke<T>(methodName, data);
        }
    }

    private buildConnection(): HubConnection {
        const headers: MessageHeaders = {
            'Authorization': `Bearer ${this.getToken()}`
        } as MessageHeaders;

        if (this.connection == undefined) {
            this.connection = new HubConnectionBuilder().withUrl(this.url, {
                accessTokenFactory: this.getToken.bind(this),
                headers: headers,
                logger: LogLevel.Error,
                //skipNegotiation: true,
                withCredentials: true,
            }).withAutomaticReconnect().build();
        }

        return this.connection;
    }

    private invoke<T>(methodName: string, data?: any) {
        if (data != undefined) {
            return this.connection.invoke<T>(methodName, data);
        } else {
            return this.connection.invoke<T>(methodName);
        }
    }
}
