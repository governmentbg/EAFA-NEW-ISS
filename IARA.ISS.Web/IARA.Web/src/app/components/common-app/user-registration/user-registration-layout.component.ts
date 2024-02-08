import { Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfigService } from '@fuse/services/config.service';



@Component({
    selector: 'user-registration-layout',
    templateUrl: './user-registration-layout.component.html',
    styleUrls: ['./user-registration-layout.component.scss'],
    encapsulation: ViewEncapsulation.None,
    animations: fuseAnimations
})
export class UserRegistrationLayoutComponent implements OnInit, OnDestroy {

    private fuseConfigService: FuseConfigService;
    constructor(fuseConfigService: FuseConfigService) {
        this.fuseConfigService = fuseConfigService;
    }

    public ngOnInit(): void {
        this.fuseConfigService.hidePanels();
    }

    public ngOnDestroy(): void {
        this.fuseConfigService.restoreConfig();
    }
}

/**
 * Confirm password validator
 *
 * @param {AbstractControl} control
 * @returns {ValidationErrors | null}
 */
export const confirmPasswordValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {

    if (!control.parent || !control) {
        return null;
    }

    const password = control.parent.get('password');
    const passwordConfirmation = control.parent.get('passwordConfirmation');

    if (!password || !passwordConfirmation) {
        return null;
    }

    if (passwordConfirmation.value === '') {
        return null;
    }

    if (password.value === passwordConfirmation.value) {
        return null;
    }

    return { passwordsNotMatching: true };
};
