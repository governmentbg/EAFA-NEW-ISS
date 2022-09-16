import { TLCommonModule } from '@app/shared/tl-common.module';
import { NgModule } from '@angular/core';
import { MaterialModule } from '@app/shared/material.module';
import { ChangePasswordPageComponent } from './change-password/change-password-page.component';
import { CreateProfileComponent } from './create-profile/create-profile.component';
import { MergeProfilesComponent } from './merge-profiles/merge-profiles.component';
import { RedirectPageComponent } from './redirect-page/redirect-page.component';
import { UserRegistrationInternalService } from './services/user-registration.service';
import { SuccessfulEmailConfirmationComponent } from './success-pages/successful-email-confrimation/successful-email-confirmation.component';
import { SuccessfulPasswordChangeComponent } from './success-pages/successful-password-change/successful-password-change.component';
import { SuccessfulRegistrationComponent } from './success-pages/successful-registration/successful-registration.component';
import { TermsAndConditionsComponent } from './terms-and-conditions/terms-and-conditions.component';
import { UserRegistrationLayoutComponent } from './user-registration-layout.component';

@NgModule({
    declarations: [
        UserRegistrationLayoutComponent,
        CreateProfileComponent,
        MergeProfilesComponent,
        TermsAndConditionsComponent,
        SuccessfulRegistrationComponent,
        RedirectPageComponent,
        ChangePasswordPageComponent,
        SuccessfulPasswordChangeComponent,
        SuccessfulEmailConfirmationComponent
    ],
    imports: [
        TLCommonModule,
        MaterialModule
    ],
    exports: [
        CreateProfileComponent,
        MergeProfilesComponent,
        TermsAndConditionsComponent,
        SuccessfulRegistrationComponent,
        RedirectPageComponent,
        ChangePasswordPageComponent,
        SuccessfulPasswordChangeComponent,
        SuccessfulEmailConfirmationComponent
    ],
    providers: [UserRegistrationInternalService]
})
export class UserRegistrationModule {
}