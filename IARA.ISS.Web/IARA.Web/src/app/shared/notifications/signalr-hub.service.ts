import { HubConnection, HubConnectionBuilder, HubConnectionState, MessageHeaders } from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { BaseNotification } from './base-notification';

export abstract class SignalRHubService {

    private connection!: HubConnection;
    private _newDataArrived: Subject<BaseNotification>;
    private startedListening: boolean = false;

    constructor(hubPath: string, apiBaseUrl: string) {
        this._newDataArrived = new Subject<BaseNotification>();

        if (apiBaseUrl == undefined) {
            apiBaseUrl = '';
        }

        let url: string = `${apiBaseUrl}${hubPath}`;
        //const authorizationValue = `Bearer ${token}`;
        //const headers = { "Authorization": authorizationValue } as MessageHeaders;

        this.connection = new HubConnectionBuilder().withUrl(url, {
            accessTokenFactory: this.getToken.bind(this),
            //headers: headers,
            //skipNegotiation: true,
            withCredentials: true,
        }).withAutomaticReconnect().build();
    }

    protected abstract getToken(): string | Promise<string>;

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

    protected startListeningFor<T>(eventName: string, handler: (result: T) => void) {
        if (this.connect()) {
            this.connection.on(eventName, handler);
        }
    }

    protected stopListeningFor(eventName: string): void {
        this.connection.off(eventName);
    }

    private connecting?: Promise<boolean>;

    private connect(): Promise<boolean> {
        if (this.connection.state == HubConnectionState.Disconnected) {
            this.connecting = this.connection.start().then(() => {
                return true;
            });

            return this.connecting;
        } else if (this.connection.state == HubConnectionState.Connecting && this.connecting != undefined) {
            return this.connecting;
        } else {
            return Promise.resolve(true);
        }
    }

    protected sendDataToHub<T>(methodName: string, data?: any): Promise<T> {
        if (this.connection.state != HubConnectionState.Connected) {
            return this.connect().then((result) => {
                return this.invoke<T>(methodName, data);
            });
        } else {
            return this.invoke<T>(methodName, data);
        }
    }

    private invoke<T>(methodName: string, data?: any) {
        if (data != undefined) {
            return this.connection.invoke<T>(methodName, data);
        } else {
            return this.connection.invoke<T>(methodName);
        }
    }
}
