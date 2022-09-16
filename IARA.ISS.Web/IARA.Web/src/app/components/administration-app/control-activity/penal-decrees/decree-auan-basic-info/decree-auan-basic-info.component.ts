import { Component, OnInit, Self } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl } from '@angular/forms';
import { PenalDecreeAuanDataDTO } from '@app/models/generated/dtos/PenalDecreeAuanDataDTO';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';

@Component({
    selector: 'decree-auan-basic-info',
    templateUrl: './decree-auan-basic-info.component.html'
})
export class DecreeAuanBasicInfoComponent extends CustomFormControl<PenalDecreeAuanDataDTO> implements OnInit {
    public auan: PenalDecreeAuanDataDTO | undefined;

    public readonly today: Date = new Date();

    public constructor(
        @Self() ngControl: NgControl
    ) {
        super(ngControl);
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public writeValue(value: PenalDecreeAuanDataDTO): void {
        if (value !== undefined && value !== null) {
            this.auan = value;

            this.form.get('auanNumControl')!.setValue(value.auanNum);
            this.form.get('auanDraftDateControl')!.setValue(value.draftDate);
            this.form.get('auanDrafterControl')!.setValue(value.drafter);
            this.form.get('auanLocationDescriptionControl')!.setValue(value.locationDescription);

            this.form.get('auanInspectedEntityControl')!.setValue(value.inspectedEntity);
        }
    }
    protected getValue(): PenalDecreeAuanDataDTO {
        const result: PenalDecreeAuanDataDTO = new PenalDecreeAuanDataDTO({
            auanNum: this.form.get('auanNumControl')!.value,
            draftDate: this.form.get('auanDraftDateControl')!.value,
            drafter: this.form.get('auanDrafterControl')!.value,
            locationDescription: this.form.get('auanLocationDescriptionControl')!.value,
            inspectedEntity: this.form.get('auanInspectedEntityControl')!.value
        });

        return result;
    }
    protected buildForm(): AbstractControl {
        return new FormGroup({
            auanNumControl: new FormControl({ value: null, disabled: true }),
            auanDraftDateControl: new FormControl({ value: null, disabled: true }),
            auanDrafterControl: new FormControl({ value: null, disabled: true }),
            auanLocationDescriptionControl: new FormControl({ value: null, disabled: true }),
            auanInspectedEntityControl: new FormControl({ value: null, disabled: true })
        });
    }
}