import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CancellationDetailsDTO } from '@app/models/generated/dtos/CancellationDetailsDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { CancellationReasonGroupEnum } from '@app/enums/cancellation-reason-group.enum';
import { CancellationReasonDTO } from '@app/models/generated/dtos/CancellationReasonDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { CancellationReasonsEnum } from '@app/enums/cancellation-reasons.enum';
import { IActionInfo } from '../dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '../dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '../dialog-wrapper/models/dialog-action-buttons.model';
import { CancellationDialogParams } from './cancellation-dialog-params.model';

@Component({
    selector: 'cancellation-dialog',
    templateUrl: './cancellation-dialog.component.html'
})
export class CancellationDialogComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;
    public model!: CancellationDetailsDTO;
    public showIssueOrderNum: boolean = false;

    public reasons: CancellationReasonDTO[] = [];

    private nomenclatures: CommonNomenclatures;
    private group!: CancellationReasonGroupEnum;

    public constructor(nomenclatures: CommonNomenclatures) {
        this.nomenclatures = nomenclatures;
    }

    public async ngOnInit(): Promise<void> {
        this.reasons = await NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.CancellationReasons, this.nomenclatures.getCancellationReasons.bind(this.nomenclatures), false
        ).toPromise();

        this.reasons = this.reasons.filter(x => x.group === this.group);

        this.fillForm(this.model);
    }

    public setData(data: CancellationDialogParams, buttons: DialogWrapperData): void {
        this.group = data.group;

        if (data.model !== undefined && data.model !== null) {
            this.model = data.model;
        }

        this.buildForm();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.form.disabled) {
            dialogClose(this.getValue());
        }
        else {
            this.form.markAllAsTouched();
            if (this.form.valid) {
                dialogClose(this.getValue());
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
            reasonControl: new FormControl(null, Validators.required),
            dateControl: new FormControl(new Date(), Validators.required),
            issueOrderNumControl: new FormControl(null, Validators.maxLength(200)),
            descriptionControl: new FormControl(null, Validators.maxLength(4000))
        });

        this.form.get('reasonControl')!.valueChanges.subscribe({
            next: (reason: CancellationReasonDTO | undefined) => {
                if (reason === undefined || reason === null) {
                    this.showIssueOrderNum = false;
                    this.form.get('issueOrderNumControl')!.clearValidators();
                    this.form.get('descriptionControl')!.clearValidators();
                }
                else {
                    switch (CancellationReasonsEnum[reason.code! as keyof typeof CancellationReasonsEnum]) {
                        case CancellationReasonsEnum.IARAHeadDecision:
                        case CancellationReasonsEnum.MZHHeadDecision:
                            this.showIssueOrderNum = true;
                            this.form.get('issueOrderNumControl')!.setValidators([Validators.required, Validators.maxLength(200)]);
                            this.form.get('descriptionControl')!.clearValidators();

                            this.form.get('issueOrderNumControl')!.markAsPending();
                            break;
                        case CancellationReasonsEnum.Other:
                            this.showIssueOrderNum = false;
                            this.form.get('issueOrderNumControl')!.clearValidators();
                            this.form.get('descriptionControl')!.setValidators([Validators.required, Validators.maxLength(4000)]);

                            this.form.get('descriptionControl')!.markAsPending();
                            break;
                        default:
                            this.showIssueOrderNum = false;
                            this.form.get('issueOrderNumControl')!.clearValidators();
                            this.form.get('descriptionControl')!.clearValidators();
                            break;
                    }
                }
            }
        });
    }

    private fillForm(value: CancellationDetailsDTO): void {
        if (value !== undefined && value !== null) {
            this.form.get('reasonControl')!.setValue(this.reasons.find(x => x.value === value.reasonId));
            this.form.get('dateControl')!.setValue(value.date);
            this.form.get('issueOrderNumControl')?.setValue(value.issueOrderNum);
            this.form.get('descriptionControl')!.setValue(value.description);

            if (value.isActive === true) {
                this.form.disable();
            }
        }
    }

    private getValue(): CancellationDetailsDTO {
        return new CancellationDetailsDTO({
            reasonId: this.form.get('reasonControl')!.value!.value,
            date: this.form.get('dateControl')!.value,
            issueOrderNum: this.form.get('issueOrderNumControl')?.value ?? undefined,
            description: this.form.get('descriptionControl')!.value,
            isActive: this.model ? !this.model.isActive : true
        });
    }
}
