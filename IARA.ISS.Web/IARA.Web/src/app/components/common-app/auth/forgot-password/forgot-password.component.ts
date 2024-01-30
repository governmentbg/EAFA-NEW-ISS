import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TRANSLATE_SERVICE_TOKEN } from '@app/shared/di/shared-di.tokens';
import { ITranslationService } from '@app/shared/interfaces/translate-service.interface';
import { fuseAnimations } from '@fuse/animations';
import { USER_SERVICE_TOKEN } from '../di/auth-di.tokens';
import { IUserService } from '../interfaces/user-service.interface';

@Component({
    selector: 'auth-forgot-password',
    templateUrl: './forgot-password.component.html',
    styleUrls: ['./forgot-password.component.scss'],
    encapsulation: ViewEncapsulation.None,
    animations: fuseAnimations
})
export class AuthForgotPasswordComponent implements OnInit {
    public form!: FormGroup;
    public responseMessage: string | undefined;
    private userService: IUserService;
    private translateService: ITranslationService;
    private router: Router;

    public constructor(@Inject(USER_SERVICE_TOKEN) userService: IUserService,
        @Inject(TRANSLATE_SERVICE_TOKEN) translateService: ITranslationService,
        router: Router) {
        this.userService = userService;
        this.translateService = translateService;
        this.router = router;
    }

    public ngOnInit(): void {
        this.form = new FormGroup({
            email: new FormControl(undefined, [Validators.required, Validators.email])
        });
    }

    public sendResetLink(): void {
        this.form.markAllAsTouched();
        this.form.updateValueAndValidity({ onlySelf: true });

        if (this.form.invalid) {
            return;
        }

        this.form.disable();

        this.userService.forgotPassword(this.form.controls.email.value)
            .subscribe({
                complete: () => {
                    this.responseMessage = this.translateService.getValue('auth.forgot-password-success');
                }
            });
    }

    public navigateToHome(): void {
        this.router.navigateByUrl('/');
    }
}
