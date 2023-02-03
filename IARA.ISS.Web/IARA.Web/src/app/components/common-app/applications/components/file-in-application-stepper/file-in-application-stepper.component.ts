import { StepperSelectionEvent } from '@angular/cdk/stepper';
import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { ApplicationsAdministrationService } from '@app/services/administration-app/applications-administration.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { FileInApplicationDialogParams } from './models/file-in-application-stepper-dialog-params.model';
import { ApplicationTypeDTO } from '@app/models/generated/dtos/ApplicationTypeDTO';
import { RequestProperties } from '@app/shared/services/request-properties';
import { IGroupedOptions } from '@app/shared/components/input-controls/tl-autocomplete/interfaces/grouped-options.interface';
import { AddApplicationResultDTO } from '@app/models/generated/dtos/AddApplicationResultDTO';

@Component({
    selector: 'file-in-application-stepper',
    templateUrl: './file-in-application-stepper.component.html'
})
export class FileInApplicationStepperComponent implements IDialogComponent {

    public createApplicationFormGroup: FormGroup;
    public accessCodeFormGroup: FormGroup;
    public eventisDataFormGroup: FormGroup;
    public componentMode: 'AddAndFileIn' | 'FileIn' = 'FileIn';
    public groupedApplicationTypes: IGroupedOptions<number>[] = [];

    private applicationTypes: ApplicationTypeDTO[] = [];

    private snackBar: MatSnackBar;
    private translationService: FuseTranslationLoaderService;
    private service: IApplicationsService;
    private applicationId?: number;

    public constructor(snackBar: MatSnackBar,
        translationService: FuseTranslationLoaderService,
        service: ApplicationsAdministrationService
    ) {
        this.snackBar = snackBar;
        this.translationService = translationService;
        this.service = service;

        this.createApplicationFormGroup = new FormGroup({
            applicationType: new FormControl('', Validators.required)
        });
        this.accessCodeFormGroup = new FormGroup({
            accessCode: new FormControl()
        });
        this.eventisDataFormGroup = new FormGroup({
            eventisNumber: new FormControl('', Validators.required)
        });
    }

    public setData(data: FileInApplicationDialogParams, buttons: DialogWrapperData): void {
        this.applicationId = data?.applicationId;

        if (this.applicationId === undefined) {
            this.componentMode = 'AddAndFileIn';
            this.service.getApplicationTypesForChoice().subscribe((result: ApplicationTypeDTO[]) => {
                this.applicationTypes = result;
                this.buildApplicationTypeGroupedOptions();
            });
        }
        else {
            this.componentMode = 'FileIn';
            this.service.getApplicationAccessCode(this.applicationId).subscribe((accessCode: string) => {
                this.accessCodeFormGroup.controls.accessCode.setValue(accessCode);
            });
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.componentMode === 'AddAndFileIn') {
            this.createApplicationFormGroup.markAllAsTouched();
        }
        this.accessCodeFormGroup.markAllAsTouched();
        this.eventisDataFormGroup.markAllAsTouched();

        if ((this.createApplicationFormGroup.valid || this.componentMode === 'FileIn') && this.accessCodeFormGroup.valid && this.eventisDataFormGroup.valid) {
            const eventisNumber: string = this.eventisDataFormGroup.controls.eventisNumber.value;
            this.service.enterEventisNumber(this.applicationId!, eventisNumber).subscribe(() => {
                dialogClose(this.applicationId);
            });
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public selectedStepChanged(stepperSelectionEvent: StepperSelectionEvent): void {
        if (this.componentMode === 'AddAndFileIn' && stepperSelectionEvent.previouslySelectedIndex === 0 && stepperSelectionEvent.selectedIndex === 1) {
            const applicationType: ApplicationTypeDTO = this.createApplicationFormGroup.controls.applicationType.value;

            this.service.addApplication(applicationType.value!).subscribe((applicationIdentification: AddApplicationResultDTO) => {
                this.applicationId = applicationIdentification.applicationId;
                this.accessCodeFormGroup.controls.accessCode.setValue(applicationIdentification.accessCode);
            });
        }
    }

    public copyAccessCodeToClipboard(): string {
        return this.accessCodeFormGroup.controls.accessCode.value;
    }

    public accessCodeCopied(copied: boolean): void {
        let message: string = '';
        if (copied === true) {
            message = this.translationService.getValue('applications-register.access-code-copied-successfully');
            this.snackBar.open(message, undefined, {
                duration: RequestProperties.DEFAULT.showExceptionDurationSucc,
                panelClass: RequestProperties.DEFAULT.showExceptionColorClassSucc
            });
        }
        else {
            message = this.translationService.getValue('applications-register.access-code-copy-failed');
            this.snackBar.open(message, undefined, {
                duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
            });
        }
    }

    private buildApplicationTypeGroupedOptions(): void {
        const groupedOptions = new Map<number | undefined, ApplicationTypeDTO[]>();

        for (const type of this.applicationTypes) {
            const groupId: number | undefined = type.groupId! < 0 ? undefined : type.groupId;

            const group: ApplicationTypeDTO[] | undefined = groupedOptions.get(groupId);
            if (group !== undefined) {
                groupedOptions.set(groupId, [...group, type]);
            }
            else {
                groupedOptions.set(groupId, [type]);
            }
        }

        for (const [groupId, group] of groupedOptions) {
            const groupName: string = groupId === undefined
                ? this.translationService.getValue('applications-register.other-group')
                : this.applicationTypes.find(x => x.groupId === groupId)!.groupName!;

            this.groupedApplicationTypes.push({
                name: groupName,
                options: group
            });
        }

        this.groupedApplicationTypes = this.groupedApplicationTypes.slice();
    }
}