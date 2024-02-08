// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

import { EnvironmentType, IEnvironmentConfig } from "./environment.interface";
import { EnvironmentConfig } from './environment.model';
import { AuthConfiguration } from './open-id-configuration';
import { OpenIdConfigurationModel } from './openid-config-model';

export const IsProduction = false;

export class Environment {

    private static instance: IEnvironmentConfig = new EnvironmentConfig({
        production: IsProduction,
        hmr: false,
        apiBasePath: '/api',
        servicesBaseUrl: 'http://localhost:5000',
        frontendBaseUrl: '',
        identityServerBaseUrl: 'https://172.31.12.168',
        hasPublicAccess: false,
        environmentType: EnvironmentType.Development,
        ClientAuthConfiguration: new OpenIdConfigurationModel(AuthConfiguration, 'public-web-client')
    });

    public static get Instance(): IEnvironmentConfig {
        return Environment.instance;
    }
}
/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.