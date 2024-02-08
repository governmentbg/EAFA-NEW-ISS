import { Route } from '@angular/router';
import { ChangePasswordPageComponent } from './change-password/change-password-page.component';
import { CreateProfileComponent } from './create-profile/create-profile.component';
import { MergeProfilesComponent } from './merge-profiles/merge-profiles.component';
//import { SuccessfulEmailConfirmationComponent } from './success-pages/successful-email-confrimation/successful-email-confirmation.component';
import { SuccessfulPasswordChangeComponent } from './success-pages/successful-password-change/successful-password-change.component';
import { SuccessfulRegistrationComponent } from './success-pages/successful-registration/successful-registration.component';
import { TermsAndConditionsComponent } from './terms-and-conditions/terms-and-conditions.component';

export const USER_REGISTRATION_ROUTES: Route[] = [
    {
        path: 'registration',
        component: CreateProfileComponent
    },
    {
        path: 'merge-profiles',
        component: MergeProfilesComponent
    },
    {
        path: 'terms-and-conditions',
        component: TermsAndConditionsComponent
    },
    {
        path: 'change-password',
        component: ChangePasswordPageComponent
    },
    {
        path: 'successful-change',
        component: SuccessfulPasswordChangeComponent
    },
    {
        path: 'successful-registration',
        component: SuccessfulRegistrationComponent
    },
    //{
    //    path: 'successful-email-confirmation',
    //    component: SuccessfulEmailConfirmationComponent
    //}
];