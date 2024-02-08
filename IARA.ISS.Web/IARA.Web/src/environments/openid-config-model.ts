import { LogLevel, OpenIdConfiguration } from 'angular-auth-oidc-client';

export class OpenIdConfigurationModel implements OpenIdConfiguration {

    constructor(config: OpenIdConfiguration, client: string) {
        this.authWellknownEndpoint = config.authWellknownEndpoint;
        this.autoCleanStateAfterAuthentication = config.autoCleanStateAfterAuthentication;
        this.autoUserinfo = config.autoUserinfo;
        this.clientId = client;
        this.disableIatOffsetValidation = config.disableIatOffsetValidation;
        this.disableRefreshIdTokenAuthTimeValidation = config.disableRefreshIdTokenAuthTimeValidation;
        this.eagerLoadAuthWellKnownEndpoints = config.eagerLoadAuthWellKnownEndpoints;
        this.forbiddenRoute = config.forbiddenRoute;
        this.historyCleanupOff = config.historyCleanupOff;
        this.ignoreNonceAfterRefresh = config.ignoreNonceAfterRefresh;
        this.issValidationOff = config.issValidationOff;
        this.logLevel = config.logLevel;
        this.maxIdTokenIatOffsetAllowedInSeconds = config.maxIdTokenIatOffsetAllowedInSeconds;
        this.postLoginRoute = config.postLoginRoute;
        this.postLogoutRedirectUri = config.postLogoutRedirectUri;
        this.redirectUrl = config.redirectUrl;
        this.renewUserInfoAfterTokenRenew = config.renewUserInfoAfterTokenRenew;
        this.responseType = config.responseType;
        this.scope = config.scope;
        this.silentRenew = config.silentRenew;
        this.silentRenewUrl = config.silentRenewUrl;
        this.startCheckSession = config.startCheckSession;
        this.stsServer = config.stsServer;
        this.tokenRefreshInSeconds = config.tokenRefreshInSeconds;
        this.triggerAuthorizationResultEvent = config.triggerAuthorizationResultEvent;
        this.unauthorizedRoute = config.unauthorizedRoute;
        this.useRefreshToken = config.useRefreshToken;
        this.usePushedAuthorisationRequests = config.usePushedAuthorisationRequests;
    }

    public hdParam?: string;
    public renewTimeBeforeTokenExpiresInSeconds?: number;
    public secureRoutes?: string[];
    public storage?: any;
    public authWellknownEndpoint?: string;
    public autoCleanStateAfterAuthentication?: boolean;
    public autoUserinfo?: boolean;
    public clientId?: string;
    public disableIatOffsetValidation?: boolean;
    public disableRefreshIdTokenAuthTimeValidation?: boolean;
    public eagerLoadAuthWellKnownEndpoints?: boolean;
    public forbiddenRoute?: string;
    public historyCleanupOff?: boolean;
    public ignoreNonceAfterRefresh?: boolean;
    public issValidationOff?: boolean;
    public logLevel?: LogLevel;
    public maxIdTokenIatOffsetAllowedInSeconds?: number;
    public postLoginRoute?: string;
    public postLogoutRedirectUri?: string;
    public redirectUrl?: string;
    public renewUserInfoAfterTokenRenew?: boolean;
    public responseType?: string;
    public scope?: string;
    public silentRenew?: boolean;
    public silentRenewUrl?: string;
    public startCheckSession?: boolean;
    public stsServer?: string;
    public tokenRefreshInSeconds?: number;
    public triggerAuthorizationResultEvent?: boolean;
    public unauthorizedRoute?: string;
    public useRefreshToken?: boolean;
    public usePushedAuthorisationRequests?: boolean;
}