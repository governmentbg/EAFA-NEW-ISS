import { OpenIdConfiguration } from "angular-auth-oidc-client";

export interface IEnvironmentConfig {
    production: boolean;
    hmr: boolean;
    apiBaseUrl?: string;
    apiBasePath: string;
    hasPublicAccess: boolean;
    servicesBaseUrl: string;
    frontendBaseUrl: string;
    identityServerBaseUrl: string;
    ClientAuthConfiguration: OpenIdConfiguration;
    environmentType: EnvironmentType;
    appBaseHref?: string;
}

export interface IConfiguration {
    apiBasePath: string;
    servicesBaseUrl: string;
    identityServerBaseUrl: string;
    environmentType: number;
    appBaseHref?: string;
}


export enum EnvironmentType {
    Development = 0,
    InternalStaging = 1,
    Staging = 2,
    Production = 3
}
