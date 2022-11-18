import { OpenIdConfiguration } from "angular-auth-oidc-client";
import { EnvironmentType, IEnvironmentConfig } from './environment.interface';


export class EnvironmentConfig implements IEnvironmentConfig {

    constructor(env: IEnvironmentConfig) {
        this._apiBaseUrl = env.apiBaseUrl;
        this.hasPublicAccess = env.hasPublicAccess;
        this._frontendBaseUrl = env.frontendBaseUrl;
        this.hmr = env.hmr;
        this._identityServerBaseUrl = env.identityServerBaseUrl;
        this._serviceBaseUrl = env.servicesBaseUrl;
        this._apiBasePath = env.apiBasePath;

        if (env.appBaseHref != undefined) {
            this.appBaseHref = env.appBaseHref;
        }

        this.ClientAuthConfiguration = env.ClientAuthConfiguration;
        this.ClientAuthConfiguration.authWellknownEndpoint = this.buildUrlFromParts([this.identityServerBaseUrl, this.ClientAuthConfiguration?.authWellknownEndpoint]);
        this.ClientAuthConfiguration.postLogoutRedirectUri = this.buildUrlFromParts([this.identityServerBaseUrl, this.ClientAuthConfiguration.postLogoutRedirectUri]);
        this.ClientAuthConfiguration.redirectUrl = this.buildUrlFromParts([this.frontendBaseUrl, this.ClientAuthConfiguration.redirectUrl]);
        this.ClientAuthConfiguration.silentRenewUrl = this.buildUrlFromParts([this.frontendBaseUrl, this.ClientAuthConfiguration.silentRenewUrl]);
        this.ClientAuthConfiguration.stsServer = this.identityServerBaseUrl;
    }

    public get production(): boolean {
        return this.environmentType == EnvironmentType.Production;
    }

    public appBaseHref?: string = '/';
    public hmr!: boolean;
    public hasPublicAccess!: boolean;
    public environmentType!: EnvironmentType;

    private _apiBaseUrl?: string = undefined;

    public get apiBaseUrl(): string {
        if (this._apiBaseUrl == undefined) {
            this._apiBaseUrl = this.buildUrlFromParts([this.servicesBaseUrl, this.apiBasePath]);
        }

        return this._apiBaseUrl;
    }

    private _apiBasePath: string = '';

    public set apiBasePath(value: string) {
        this._apiBasePath = value;
    }

    public get apiBasePath(): string {
        return this._apiBasePath;
    }

    private _serviceBaseUrl!: string;
    public get servicesBaseUrl(): string {
        if (this._serviceBaseUrl == null || this._serviceBaseUrl == undefined || this._serviceBaseUrl == "") {
            return window.location.origin;
        } else {
            return this._serviceBaseUrl;
        }
    }

    public set servicesBaseUrl(value: string) {
        this._serviceBaseUrl = value;
    }

    private _frontendBaseUrl!: string;
    public get frontendBaseUrl(): string {
        if (this._frontendBaseUrl == null || this._frontendBaseUrl == undefined || this._frontendBaseUrl == "") {
            return window.location.origin;
        } else {
            return this._frontendBaseUrl;
        }
    }

    public set frontendBaseUrl(value: string) {
        this._frontendBaseUrl = value;
    }

    private _identityServerBaseUrl!: string;
    public get identityServerBaseUrl(): string {
        if (this._identityServerBaseUrl == null || this._identityServerBaseUrl == undefined || this._identityServerBaseUrl == "") {
            return window.location.origin;
        } else {
            return this._identityServerBaseUrl;
        }
    }

    public set identityServerBaseUrl(value: string) {
        this._identityServerBaseUrl = value;
    }

    public ClientAuthConfiguration!: OpenIdConfiguration;


    private buildUrlFromParts(urlParts: (string | undefined)[]): string {
        let url: string = '';

        for (let part of urlParts) {
            let trimmedPart: string = part == undefined ? '' : part;

            if (trimmedPart.startsWith('/')) {
                trimmedPart = trimmedPart.substr(1, trimmedPart.length - 1);
            }

            if (trimmedPart.endsWith('/')) {
                trimmedPart = trimmedPart.substr(0, trimmedPart.length - 1);
            }

            url += trimmedPart + '/';
        }

        while (url.endsWith('/')) {
            url = url.substr(0, url.length - 1);
        }

        return url;
    }
}
