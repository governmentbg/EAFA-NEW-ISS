import { Component, Inject, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { fuseAnimations } from '@fuse/animations';
import { FuseAlertType } from '@fuse/components/alert/alert.types';
import { SECURITY_SERVICE_TOKEN } from '../di/auth-di.tokens';
import { ISecurityService } from '../interfaces/security-service.interface';

@Component({
    selector: 'auth-sign-up',
    templateUrl: './sign-up.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: fuseAnimations
})
export class AuthSignUpComponent {
    @ViewChild('signUpNgForm') public signUpNgForm?: NgForm;

    alert: { type: FuseAlertType; message: string } = {
        type: 'success',
        message: ''
    };

    public signUpForm!: FormGroup;
    public showAlert: boolean = false;


    private securityService: ISecurityService;
    private formBuilder: FormBuilder;
    private router: Router;

    /**
     * Constructor
     */
    public constructor(@Inject(SECURITY_SERVICE_TOKEN) securityService: ISecurityService, formBuilder: FormBuilder, router: Router) {
        this.securityService = securityService;
        this.formBuilder = formBuilder;
        this.router = router;
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Lifecycle hooks
    // -----------------------------------------------------------------------------------------------------



    // -----------------------------------------------------------------------------------------------------
    // @ Public methods
    // -----------------------------------------------------------------------------------------------------

    /**
     * Sign up
     */
    signUp(): void {
        // Do nothing if the form is invalid
        if (this.signUpForm.invalid) {
            return;
        }

        // Disable the form
        this.signUpForm.disable();

        // Hide the alert
        this.showAlert = false;
    }
}
