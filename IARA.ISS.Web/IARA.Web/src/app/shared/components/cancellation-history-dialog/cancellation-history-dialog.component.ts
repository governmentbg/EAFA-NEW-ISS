import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CancellationReasonGroupEnum } from '@app/enums/cancellation-reason-group.enum';
import { CancellationReasonsEnum } from '@app/enums/cancellation-reasons.enum';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CancellationReasonDTO } from '@app/models/generated/dtos/CancellationReasonDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { CancellationHistoryEntryDTO } from '@app/models/generated/dtos/CancellationHistoryEntryDTO';
import { CancellationHistoryDialogParams } from './cancellation-history-dialog-params.model';

@Component({
    selector: 'cancellation-history-dialog',
    templateUrl: './cancellation-history-dialog.component.html'
})
export class CancellationHistoryDialogComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;
    public cancelling: boolean = true;
    public showIssueOrderNum: boolean = false;

    public reasons: CancellationReasonDTO[] = [];
    public allReasons: CancellationReasonDTO[] = [];
    public statuses: CancellationHistoryEntryDTO[] = [];

    private nomenclatures: CommonNomenclatures;
    private group!: CancellationReasonGroupEnum;

    public constructor(nomenclatures: CommonNomenclatures) {
        this.nomenclatures = nomenclatures;
    }

    public ngOnInit(): void {
        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.CancellationReasons, this.nomenclatures.getCancellationReasons.bind(this.nomenclatures), false
        ).subscribe({
            next: (reasons: CancellationReasonDTO[]) => {
                this.reasons = reasons.filter(x => x.group === this.group);
                this.allReasons = reasons;
            }
        });
    }

    public setData(data: CancellationHistoryDialogParams, wrapperData: DialogWrapperData): void {
        this.group = data.group;
        this.cancelling = data.cancelling;

        setTimeout(() => {
            this.statuses = data.statuses ?? [];
        });

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
            dateControl: new FormControl({ value: new Date(), disabled: true }),
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

    private getValue(): CancellationHistoryEntryDTO {
        const result = new CancellationHistoryEntryDTO({
            isCancelled: this.cancelling,
            cancellationReasonId: this.form.get('reasonControl')!.value.value,
            issueOrderNum: this.form.get('issueOrderNumControl')!.value,
            description: this.form.get('descriptionControl')!.value
        });

        return result;
    }
}