import { Route } from "@angular/router";
import { AuthConfirmationRequiredComponent } from './confirmation-required/confirmation-required.component';
import { AuthForgotPasswordComponent } from "./forgot-password/forgot-password.component";
import { AuthGuard } from './guards/auth.guard';
import { NoAuthGuard } from './guards/noAuth.guard';
import { SecurityConfig } from './interfaces/security-config.interface';
import { AuthRedirectComponent } from './redirects/auth-redirect.component';
import { ExternalAuthRedirectComponent } from './redirects/external-auth-redirect.component';
import { AuthResetPasswordComponent } from "./reset-password/reset-password.component";
import { AuthSignInComponent } from "./sign-in/sign-in.component";
import { AuthSignOutComponent } from "./sign-out/sign-out.component";
import { SuccessfulEmailConfirmationComponent } from './successful-email-confirmation/successful-email-confirmation.component';
import { AuthUnlockSessionComponent } from "./unlock-session/unlock-session.component";

export function GetAccountRoutes(securityConfig: SecurityConfig): Route {
    const childRoutes = ACCOUNT_ROUTES;

    let modulePath = securityConfig.authModulePath;

    if (securityConfig.authModulePath.startsWith('/')) {
        modulePath = securityConfig.authModulePath.substring(1);
    }

    childRoutes[0].path = modulePath;

    return {
        path: modulePath,
        children: childRoutes
    };
}

export const ACCOUNT_ROUTES: Route[] = [
    {
        path: '',
        component: AuthSignInComponent,
        canActivate: [NoAuthGuard]
    },
    {
        path: 'sign-in',
        component: AuthSignInComponent,
        canActivate: [NoAuthGuard]
    },
    {
        path: 'auth-redirect',
        component: AuthRedirectComponent,
        //canActivate: [NoAuthGuard],
        data: {
            layout: 'empty'
        }
    },
    {
        path: 'external-auth-redirect',
        component: ExternalAuthRedirectComponent,
        canActivate: [NoAuthGuard],
        data: {
            layout: 'empty'
        }
    },
    {
        path: 'sign-out',
        component: AuthSignOutComponent,
        canActivate: [AuthGuard]
    },
    {
        path: 'forgot-password',
        component: AuthForgotPasswordComponent,
        canActivate: [NoAuthGuard],
        data: {
            layout: 'empty'
        }
    },
    {
        path: 'reset-password',
        component: AuthResetPasswordComponent,
        data: {
            layout: 'empty'
        }
    },
    {
        path: 'unlock-session',
        component: AuthUnlockSessionComponent,
        canActivate: [NoAuthGuard],
        data: {
            layout: 'empty'
        }
    },
    {
        path: 'successful-email-confirmation',
        component: SuccessfulEmailConfirmationComponent,
        canActivate: [NoAuthGuard],
    },
    //{
    //    path: 'change-password',
    //    component: ChangePasswordComponent,
    //    data: {
    //        layout: 'modern'
    //    },
    //    canActivate: [AuthGuard]
    //},
    {
        path: 'confirmation-required',
        component: AuthConfirmationRequiredComponent,
        canActivate: [NoAuthGuard],
    }
];
