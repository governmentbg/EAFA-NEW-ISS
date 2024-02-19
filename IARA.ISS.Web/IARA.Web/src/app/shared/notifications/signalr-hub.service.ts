import { EventEmitter, Inject } from '@angular/core';
import {
    HttpTransportType,
    HubConnection,
    HubConnectionBuilder,
    HubConnectionState,
    LogLevel,
    MessageHeaders
} from '@microsoft/signalr';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { ISecurityService } from '@app/components/common-app/auth/interfaces/security-service.interface';
import { BaseNotification } from './models/base-notification';
import { SECURITY_SERVICE_TOKEN } from '@app/components/common-app/auth/di/auth-di.tokens';

export abstract class SignalRHubService {

    private connection!: HubConnection;
    private _newDataArrived: Subject<BaseNotification>;
    protected securityService: ISecurityService;
    private startedListening: boolean = false;
    private url!: string;

    private _isConnected: BehaviorSubject<boolean>;
    private _connectionFailedEvent: EventEmitter<string>;
    private _reconnectedEvent: EventEmitter<string>;

    public get connectionFailedEvent(): Observable<string> {
        return this._connectionFailedEvent;
    }

    private _connectionSuccessEvent: EventEmitter<boolean>;
    public get connectionSuccessEvent(): Observable<boolean> {
        return this._connectionSuccessEvent;
    }

    public get isConnected(): boolean {
        return this._isConnected.value;
    }

    public get isConnectedEvent(): Observable<boolean> {
        return this._isConnected;
    }

    public constructor(@Inject(SECURITY_SERVICE_TOKEN) securityService: ISecurityService, hubPath: string, apiBaseUrl: string) {
        this._isConnected = new BehaviorSubject<boolean>(false);
        this.url = `${apiBaseUrl}${hubPath}`;
        this._newDataArrived = new Subject<BaseNotification>();
        this._connectionFailedEvent = new EventEmitter<string>();
        this._connectionSuccessEvent = new EventEmitter<boolean>();
        this._reconnectedEvent = new EventEmitter<string>();

        this.securityService = securityService;

        if (apiBaseUrl == undefined) {
            apiBaseUrl = '';
        }

        //const authorizationValue = `Bearer ${token}`;
        //const headers = { "Authorization": authorizationValue } as MessageHeaders;
    }

    protected getToken(): string | Promise<string> {
        return this.securityService.token ?? '';
    }

    public get newNotificationArrived(): Subject<BaseNotification> {
        return this._newDataArrived;
    }

    public get reconnectedEvent(): Subject<string> {
        return this._reconnectedEvent;
    }

    public stopListening(): Promise<boolean> {
        if (this.startedListening) {
            this.startedListening = false;
            return this.disconnect();
        } else {
            return Promise.resolve(false);
        }
    }

    private disconnect(): Promise<boolean> {
        if (this.connection.state == HubConnectionState.Connected) {
            return this.connection.stop().then(() => true);
        } else {
            return Promise.resolve(true);
        }
    }

    protected async startListeningFor<T>(eventName: string, handler: (result: T) => void): Promise<void> {
        if (await this.connect()) {
            this.connection.on(eventName, (result) => {
                handler(result);
            });
        }
    }

    protected stopListeningFor(eventName: string): void {
        this.connection.off(eventName);
    }

    public connecting?: Subject<boolean>;

    protected connect(): Promise<boolean> {

        this.connection = this.buildConnection();

        this.connection.onreconnecting(() => {
            this._isConnected.next(false);
        });

        this.connection.onclose(() => {
            this._isConnected.next(false);
        });

        this.connection.onreconnected(() => {
            this._isConnected.next(true);
        });

        if (this.connection.state == HubConnectionState.Disconnected) {
            this.connecting = new Subject<boolean>();
            const connecting = this.connecting;
            this.connection.start().then(() => {
                this.startedListening = true;
                this._isConnected.next(true);
                connecting?.next(true);
                connecting?.complete();
                return true;
            });
        }

        return this.toConnectingResult(this.connecting);
    }

    private toConnectingResult(connecting: Subject<boolean> | undefined): Promise<boolean> {
        if (connecting != undefined && !connecting.isStopped) {
            return connecting.asObservable().toPromise().then(result => {
                result ??= false;
                if (result) {
                    this._connectionSuccessEvent.emit(result);
                }

                return result;
            }, reason => {
                this._connectionFailedEvent.emit(reason);
                return false;
            });
        } else if (connecting != undefined && connecting.isStopped) {
            return Promise.resolve(true);
        } else {
            return Promise.resolve(false);
        }
    }

    protected sendDataToHub<T>(methodName: string, data?: any): Promise<T> {

        if (this.connection == undefined || this.connection.state != HubConnectionState.Connected) {
            return this.connect().then(() => this.invoke<T>(methodName, data));
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
                logger: LogLevel.Warning,
                skipNegotiation: false,
                withCredentials: true,
                logMessageContent: false,
                transport: HttpTransportType.WebSockets | HttpTransportType.LongPolling | HttpTransportType.ServerSentEvents
            }).withAutomaticReconnect().build();

            this.connection.onreconnected((connectionId) => {
                this._reconnectedEvent.emit(connectionId);
            });
        }

        return this.connection;
    }

    private invoke<T>(methodName: string, data?: any): Promise<T> {
        if (data != undefined) {
            return this.connection.invoke<T>(methodName, data);
        } else {
            return this.connection.invoke<T>(methodName);
        }
    }
}
