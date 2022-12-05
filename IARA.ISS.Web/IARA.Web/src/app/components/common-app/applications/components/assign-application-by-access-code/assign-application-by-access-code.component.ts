import { HttpErrorResponse } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';

import { IApplicationsRegisterService } from '@app/interfaces/administration-app/applications-register.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { AssignedApplicationInfoDTO } from '@app/models/generated/dtos/AssignedApplicationInfoDTO';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { ErrorSnackbarComponent } from '@app/shared/components/error-snackbar/error-snackbar.component';
import { RequestProperties } from '@app/shared/services/request-properties';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';

@Component({
    selector: 'assign-application-by-access-code',
    templateUrl: './assign-application-by-access-code.component.html'
})
export class AssignApplicationByAccessCodeComponent implements IDialogComponent {

    public accessCodeForm: FormGroup;

    private service!: IApplicationsRegisterService;
    private readonly translate: FuseTranslationLoaderService;
    private readonly snackbar: MatSnackBar;

    public constructor(snackbar: MatSnackBar, translate: FuseTranslationLoaderService) {
        this.snackbar = snackbar;
        this.translate = translate;

        this.accessCodeForm = new FormGroup({
            accessCodeControl: new FormControl('', [Validators.required, Validators.maxLength(6), Validators.minLength(6)])
        });
    }

    public setData(data: DialogParamsModel, buttons: DialogWrapperData): void {
        this.service = data.service as IApplicationsRegisterService;
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.accessCodeForm.markAllAsTouched();
        if (this.accessCodeForm.valid) {
            const accessCode: string = this.accessCodeForm.controls.accessCodeControl.value;
            this.service.assignApplicationViaAccessCode(accessCode).subscribe({
                next: (applicationData: AssignedApplicationInfoDTO) => {
                    dialogClose(applicationData);
                },
                error: (errorResponse: HttpErrorResponse) => {
                    const error = errorResponse?.error as ErrorModel;
                    if (error !== null && error !== undefined && error.code === ErrorCode.InvalidStateMachineTransitionOperation) {
                        this.snackbar.openFromComponent(ErrorSnackbarComponent, {
                            data: new ErrorModel({ messages: [this.translate.getValue('applications-register.assign-invalid-state-machine-transition-operation-error')] }),
                            duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                            panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                        });
                    }
                }
            });
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

}
