import { Component, OnInit, Self } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';
import { InspectedEntityEmailDTO } from '@app/models/generated/dtos/InspectedEntityEmailDTO';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';

@Component({
    selector: 'inspected-entity-email',
    templateUrl: './inspected-entity-email.component.html',
})
export class InspectedEntityEmailComponent extends CustomFormControl<InspectedEntityEmailDTO> implements OnInit {
    public model!: InspectedEntityEmailDTO;
    public label: string = '';

    public constructor(
        @Self() ngControl: NgControl
    ) {
        super(ngControl, false);
    }

    public ngOnInit(): void {
        this.initCustomFormControl();

        this.form.valueChanges.subscribe({
            next: () => {
                const value: InspectedEntityEmailDTO | undefined = this.getValue();
                this.onChanged(value);
            }
        });
    }

    public writeValue(value: InspectedEntityEmailDTO): void {
        this.model = value;

        if (value !== undefined && value !== null) {
            this.fillForm();
        }

        setTimeout(() => {
            this.onChanged(this.getValue());
        });
    }

    protected getValue(): InspectedEntityEmailDTO {
        this.model.sendEmail = this.form.get('sendEmailControl')!.value;
        this.model.email = this.form.get('emailControl')!.value;

        return this.model;
    }

    protected buildForm(): AbstractControl {
        const form: FormGroup = new FormGroup({
            sendEmailControl: new FormControl(false),
            nameControl: new FormControl(undefined),
            inspectedPersonTypeControl: new FormControl(undefined),
            emailControl: new FormControl(undefined, Validators.maxLength(500))
        });

        form.get('sendEmailControl')!.valueChanges.subscribe({
            next: (value: boolean) => {
                form.get('emailControl')!.clearValidators();

                if (value) {
                    form.get('emailControl')!.setValidators([Validators.required, Validators.maxLength(500)]);
                }
                else {
                    form.get('emailControl')!.setValidators([Validators.maxLength(500)]);
                }

                form.get('emailControl')!.markAsPending();
                form.get('emailControl')!.updateValueAndValidity({ emitEvent: false });
            }
        });

        return form;
    }

    private fillForm(): void {
        this.form.get('sendEmailControl')!.setValue(this.model.sendEmail);
        this.form.get('nameControl')!.setValue(this.model.name);
        this.form.get('inspectedPersonTypeControl')!.setValue(this.model.inspectedPersonType);
        this.form.get('emailControl')!.setValue(this.model.email);
    }
}