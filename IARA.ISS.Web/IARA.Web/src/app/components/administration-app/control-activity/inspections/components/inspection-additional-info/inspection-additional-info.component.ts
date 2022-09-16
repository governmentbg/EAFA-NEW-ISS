import { Component, OnInit, Self } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl } from '@angular/forms';

import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { InspectionObservationTextDTO } from '@app/models/generated/dtos/InspectionObservationTextDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { InspectionAdditionalInfoModel } from '../../models/inspection-additional-info.model';
import { InspectionObservationCategoryEnum } from '@app/enums/inspection-observation-category.enum';

@Component({
    selector: 'inspection-additional-info',
    templateUrl: './inspection-additional-info.component.html',
})
export class InspectionAdditionalInfoComponent extends CustomFormControl<InspectionAdditionalInfoModel> implements OnInit {
    public constructor(@Self() ngControl: NgControl) {
        super(ngControl);
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public writeValue(value: InspectionAdditionalInfoModel): void {
        if (value !== undefined && value !== null) {
            this.form.get('violationControl')!.setValue(value.violation?.text);
            this.form.get('inspectorCommentControl')!.setValue(value.inspectorComment);
            this.form.get('actionsTakenControl')!.setValue(value.actionsTaken);
            this.form.get('administrativeViolationControl')!.setValue(value.administrativeViolation);
        } else {
            this.form.get('violationControl')!.setValue(null);
            this.form.get('inspectorCommentControl')!.setValue(null);
            this.form.get('actionsTakenControl')!.setValue(null);
            this.form.get('administrativeViolationControl')!.setValue(false);
        }
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
            violationControl: new FormControl(undefined),
            inspectorCommentControl: new FormControl(undefined),
            actionsTakenControl: new FormControl(undefined),
            administrativeViolationControl: new FormControl(false),
        });
    }

    protected getValue(): InspectionAdditionalInfoModel {
        const violation: string = this.form.get('violationControl')!.value;

        return new InspectionAdditionalInfoModel({
            violation: !CommonUtils.isNullOrWhiteSpace(violation)
                ? new InspectionObservationTextDTO({
                    category: InspectionObservationCategoryEnum.AdditionalInfo,
                    text: violation
                }) : undefined,
            inspectorComment: this.form.get('inspectorCommentControl')!.value,
            actionsTaken: this.form.get('actionsTakenControl')!.value,
            administrativeViolation: this.form.get('administrativeViolationControl')!.value ?? false,
        });
    }
}
