import { AfterViewInit, Component } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { RegixPersonDataDTO } from '@app/models/generated/dtos/RegixPersonDataDTO';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { FishingAssociationPersonDTO } from '@app/models/generated/dtos/FishingAssociationPersonDTO';
import { EditLegalAssociationPersonDialogParams } from '../models/edit-legal-association-person-dialog-params.model';
import { EditLegalAssociationPersonResult } from '../models/edit-legal-association-person-result.model';

@Component({
    selector: 'edit-legal-association-person',
    templateUrl: './edit-legal-association-person.component.html'
})
export class EditLegalAssociationPersonComponent implements AfterViewInit, IDialogComponent {
    public form!: FormGroup;

    public model!: FishingAssociationPersonDTO;
    public expectedResults: FishingAssociationPersonDTO;
    public readOnly!: boolean;
    public showOnlyRegixData: boolean = false;
    public isEditing!: boolean;

    private isTouched: boolean = false;

    public constructor() {
        this.expectedResults = new FishingAssociationPersonDTO({
            person: new RegixPersonDataDTO()
        });

        this.buildForm();
    }

    public ngAfterViewInit(): void {
        setTimeout(() => {
            if (this.isTouched === false) {
                this.form.valueChanges.subscribe(() => {
                    this.isTouched = true;
                });
            }
        });
    }

    public setData(data: EditLegalAssociationPersonDialogParams, buttons: DialogWrapperData): void {
        this.readOnly = data.readOnly;
        this.showOnlyRegixData = data.showOnlyRegiXData;
        this.isEditing = data.isEgnLncReadOnly;

        if (data.expectedResults) {
            this.expectedResults = data.expectedResults;
        }

        if (data.model === undefined) {
            this.model = new FishingAssociationPersonDTO({ isActive: true });
        }
        else {
            if (this.readOnly) {
                this.form.disable();
            }

            this.model = data.model;
            this.fillForm();
        }

        if (this.showOnlyRegixData) {
            this.form.markAllAsTouched();
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.readOnly) {
            dialogClose(new EditLegalAssociationPersonResult(this.model, this.isTouched));
        }
        else {
            this.form.markAllAsTouched();

            if (this.form.valid) {
                this.fillModel();
                CommonUtils.sanitizeModelStrings(this.model);
                dialogClose(new EditLegalAssociationPersonResult(this.model, this.isTouched));
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private buildForm(): void {
        this.form = new FormGroup({
            personControl: new FormControl()
        });
    }

    private fillForm(): void {
        this.form.get('personControl')!.setValue(this.model.person);
    }

    private fillModel(): void {
        this.model.person = this.form.get('personControl')!.value;
    }
}