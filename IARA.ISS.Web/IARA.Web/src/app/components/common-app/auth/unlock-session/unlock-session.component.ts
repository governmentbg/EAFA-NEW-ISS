import { Component, Inject, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { fuseAnimations } from '@fuse/animations';
import { FuseAlertType } from '@fuse/components/alert/alert.types';
import { SECURITY_SERVICE_TOKEN } from '../di/auth-di.tokens';
import { ISecurityService } from '../interfaces/security-service.interface';

@Component({
    selector: 'auth-unlock-session',
    templateUrl: './unlock-session.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: fuseAnimations
})
export class AuthUnlockSessionComponent implements OnInit {
    @ViewChild('unlockSessionNgForm') public unlockSessionNgForm?: NgForm;

    alert: { type: FuseAlertType; message: string } = {
        type: 'success',
        message: ''
    };

    public name?: string;
    private _email?: string;

    public showAlert: boolean = false;
    public unlockSessionForm!: FormGroup;

    private _activatedRoute: ActivatedRoute;
    private _authService: ISecurityService;
    private _formBuilder!: FormBuilder;
    private _router: Router;
    //private _userService: UserService;

    /**
     * Constructor
     */
    public constructor(activatedRoute: ActivatedRoute,
        @Inject(SECURITY_SERVICE_TOKEN) securityService: ISecurityService,
        formBuilder: FormBuilder,
        router: Router) {
        this._activatedRoute = activatedRoute;
        this._authService = securityService;
        this._router = router;
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Lifecycle hooks
    // -----------------------------------------------------------------------------------------------------

    /**
     * On init
     */
    ngOnInit(): void {

        // Create the form
        this.unlockSessionForm = this._formBuilder.group({
            name: [
                {
                    value: this.name,
                    disabled: true
                }
            ],
            password: ['', Validators.required]
        });
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Public methods
    // -----------------------------------------------------------------------------------------------------

    /**
     * Unlock
     */
    unlock(): void {
        // Return if the form is invalid
        if (this.unlockSessionForm.invalid) {
            return;
        }

        // Disable the form
        this.unlockSessionForm.disable();

        // Hide the alert
        this.showAlert = false;
    }
}
