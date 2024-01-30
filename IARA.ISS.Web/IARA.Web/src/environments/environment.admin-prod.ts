// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { EnvironmentType, IEnvironmentConfig } from "./environment.interface";
import { EnvironmentConfig } from './environment.model';


export const IsProduction = true;
export class Environment {

    private static instance: IEnvironmentConfig = new EnvironmentConfig({
        production: IsProduction,
        isPublicApp: IS_PUBLIC_APP,
        hmr: false,
        apiBasePath: '/api',
        servicesBaseUrl: '',
        frontendBaseUrl: '',
        identityServerBaseUrl: '',
        hasPublicAccess: false,
        environmentType: EnvironmentType.Production
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