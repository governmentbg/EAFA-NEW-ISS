
export interface IEnvironmentConfig {
    production: boolean;
    hmr: boolean;
    apiBaseUrl?: string;
    apiBasePath: string;
    hasPublicAccess: boolean;
    isPublicApp: boolean;
    servicesBaseUrl: string;
    frontendBaseUrl: string;
    identityServerBaseUrl: string;
    environmentType: EnvironmentType;
    egovPaymentHref: string;
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
