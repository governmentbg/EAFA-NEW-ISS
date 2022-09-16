import { Component, Input, OnInit, Optional, Self } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl } from '@angular/forms';

import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';
import { ValidityCheckerDirective } from '@app/shared/directives/validity-checker/validity-checker.directive';
import { NotifyingCustomFormControl } from '@app/shared/utils/notifying-custom-form-control';
import { NotifierDirective } from '@app/shared/directives/notifier/notifier.directive';
import { Notifier } from '@app/shared/directives/notifier/notifier.class';

@Component({
    selector: 'application-submitted-by',
    templateUrl: './application-submitted-by.component.html'
})
export class ApplicationSubmittedByComponent extends NotifyingCustomFormControl<ApplicationSubmittedByDTO> implements OnInit {
    @Input()
    public label: string = '';

    @Input()
    public isIdReadOnly: boolean = false;

    @Input()
    public showOnlyBasicData: boolean = false;

    @Input()
    public isForeigner: boolean = false;

    @Input()
    public hideDocument: boolean = false;

    @Input()
    public expectedResults!: ApplicationSubmittedByDTO;

    public notifierGroup: Notifier = new Notifier();

    public constructor(
        @Self() ngControl: NgControl,
        @Optional() @Self() validityChecker: ValidityCheckerDirective,
        @Optional() @Self() notifier: NotifierDirective
    ) {
        super(ngControl, true, validityChecker, notifier);
    }

    public ngOnInit(): void {
        this.initNotifyingCustomFormControl(this.notifierGroup, () => { this.notify(); });
    }

    public writeValue(value: ApplicationSubmittedByDTO): void {
        setTimeout(() => {
            if (value !== null && value !== undefined) {
                this.form.get('personControl')!.setValue(value.person);
                this.form.get('addressesControl')!.setValue(value.addresses);
            }
            else {
                this.form.reset();
            }
        });
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
            personControl: new FormControl(),
            addressesControl: new FormControl()
        });
    }

    protected getValue(): ApplicationSubmittedByDTO {
        return new ApplicationSubmittedByDTO({
            person: this.form.get('personControl')!.value ?? undefined,
            addresses: this.form.get('addressesControl')!.value ?? undefined
        });
    }
}