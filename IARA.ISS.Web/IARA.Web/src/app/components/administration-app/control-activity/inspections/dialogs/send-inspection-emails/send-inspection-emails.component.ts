import { Component, OnInit } from '@angular/core';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { InspectionsService } from '@app/services/administration-app/inspections.service';
import { FormArray, FormControl, FormGroup } from '@angular/forms';
import { InspectionEmailDTO } from '@app/models/generated/dtos/InspectionEmailDTO';
import { InspectedEntityEmailDTO } from '@app/models/generated/dtos/InspectedEntityEmailDTO';

@Component({
    selector: 'send-inspection-emails',
    templateUrl: './send-inspection-emails.component.html',
})
export class SendInspectionEmailsComponent implements IDialogComponent, OnInit {
    public form: FormGroup;
    public emails: InspectedEntityEmailDTO[] = [];

    public get emailsFormArray(): FormArray {
        return this.form?.get('emailsArray') as FormArray;
    }

    private id!: number;
    private model!: InspectionEmailDTO;

    private readonly service: InspectionsService;

    public constructor(service: InspectionsService) {
        this.service = service;

        this.form = this.buildForm();
    }

    public ngOnInit(): void {
        this.service.GetInspectedEntityEmail(this.id!).subscribe({
            next: (result: InspectedEntityEmailDTO[]) => {
                this.emails = result;
                this.buildEmailsFormArray(result);
            }
        });
    }

    public setData(data: DialogParamsModel, wrapperData: DialogWrapperData): void {
        this.id = data.id;
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();

        if (this.form.valid) {
            this.fillModel();
            const emailsCount: number = this.model.inspectedEntityEmails?.filter(x => x.sendEmail).length ?? 0;
     
            if (emailsCount > 0) {
                this.service.sendInspectedEntityEmailNotification(this.model).subscribe({
                    next: () => {
                        dialogClose();
                    }
                });
            }
            else {
                dialogClose();
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public setSendEmailValues(sendEmail: boolean): void {
        const emails: InspectedEntityEmailDTO[] = (this.emailsFormArray.value as InspectedEntityEmailDTO[]);

        for (const email of emails) {
            email.sendEmail = sendEmail;
        }

        this.form.get('emailsArray')!.setValue(emails);
    }

    private buildForm(): FormGroup {
        return new FormGroup({
            emailsArray: new FormArray([])
        });
    }

    private fillModel(): void {
        this.model = new InspectionEmailDTO({
            inspectionId: this.id
        });

        this.model.inspectedEntityEmails = this.emailsFormArray.value as InspectedEntityEmailDTO[];
    }

    private buildEmailsFormArray(emails: InspectedEntityEmailDTO[]) {
        for (const email of emails) {
            this.emailsFormArray.push(new FormControl(email));
        }
    }
}