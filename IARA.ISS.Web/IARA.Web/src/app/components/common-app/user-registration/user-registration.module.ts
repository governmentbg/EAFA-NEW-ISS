import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MaterialModule } from '@app/shared/material.module';
import { TLCommonModule } from '@app/shared/tl-common.module';
import { ChangePasswordPageComponent } from './change-password/change-password-page.component';
import { CreateProfileComponent } from './create-profile/create-profile.component';
import { MergeProfilesComponent } from './merge-profiles/merge-profiles.component';
import { SuccessfulEmailConfirmationComponent } from './success-pages/successful-email-confrimation/successful-email-confirmation.component';
import { SuccessfulPasswordChangeComponent } from './success-pages/successful-password-change/successful-password-change.component';
import { SuccessfulRegistrationComponent } from './success-pages/successful-registration/successful-registration.component';
import { TermsAndConditionsComponent } from './terms-and-conditions/terms-and-conditions.component';
import { UserRegistrationLayoutComponent } from './user-registration-layout.component';
import { USER_REGISTRATION_ROUTES } from './user-registration.routing';

@NgModule({
    declarations: [
        UserRegistrationLayoutComponent,
        CreateProfileComponent,
        MergeProfilesComponent,
        TermsAndConditionsComponent,
        SuccessfulRegistrationComponent,
        ChangePasswordPageComponent,
        SuccessfulPasswordChangeComponent,
        SuccessfulEmailConfirmationComponent
    ],
    imports: [
        TLCommonModule,
        MaterialModule,
        RouterModule.forChild(USER_REGISTRATION_ROUTES),
    ],
    exports: [
        CreateProfileComponent,
        MergeProfilesComponent,
        TermsAndConditionsComponent,
        SuccessfulRegistrationComponent,
        ChangePasswordPageComponent,
        SuccessfulPasswordChangeComponent,
        SuccessfulEmailConfirmationComponent
    ]
})
export class UserRegistrationModule {
}