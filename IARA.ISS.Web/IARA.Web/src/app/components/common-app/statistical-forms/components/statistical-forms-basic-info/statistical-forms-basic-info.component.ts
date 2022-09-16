import { Component, OnInit, Self } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl } from '@angular/forms';
import { Subscription } from 'rxjs';

import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { AuthService } from '@app/shared/services/auth.service';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { StatisticalFormBasicInfoDTO } from '@app/models/generated/dtos/StatisticalFormBasicInfoDTO';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';

@Component({
    selector: 'statistical-forms-basic-info',
    templateUrl: './statistical-forms-basic-info.component.html'
})
export class StatisticalFormsBasicInfoComponent extends CustomFormControl<StatisticalFormBasicInfoDTO> implements OnInit {
    public processUsers: NomenclatureDTO<number>[] = [];

    private nomenclatures: CommonNomenclatures;
    private authService: AuthService;

    private readonly loader: FormControlDataLoader;

    public constructor(
        @Self() ngControl: NgControl,
        nomenclatures: CommonNomenclatures,
        authService: AuthService
    ) {
        super(ngControl);
        this.nomenclatures = nomenclatures;
        this.authService = authService;

        this.loader = new FormControlDataLoader(this.getNomenclatures.bind(this));
    }

    public ngOnInit(): void {
        this.initCustomFormControl();

        this.loader.load(() => {
            this.form.get('processUserControl')!.setValue(this.processUsers.find(x => x.value === this.authService.userRegistrationInfo!.id));
        });
    }

    public writeValue(value: StatisticalFormBasicInfoDTO): void {
        if (value !== undefined && value !== null) {
            this.loader.load(() => {
                this.form.get('processUserControl')!.setValue(this.processUsers.find(x => x.value === value!.processUserId));
            });
            this.form.get('yearControl')!.setValue(new Date(value.year!, 0, 1));
            this.form.get('submissionDateControl')!.setValue(value.submissionDate);
            this.form.get('submissionPersonControl')!.setValue(value.submissionPerson);
            this.form.get('submissionPersonWorkPositionControl')!.setValue(value.submissionPersonWorkPosition);

        }
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
            processUserControl: new FormControl({ value: null, disabled: true }),
            yearControl: new FormControl(),
            submissionDateControl: new FormControl(),
            submissionPersonControl: new FormControl(),
            submissionPersonWorkPositionControl: new FormControl()
        });
    }

    protected getValue(): StatisticalFormBasicInfoDTO {
        const result: StatisticalFormBasicInfoDTO = new StatisticalFormBasicInfoDTO({
            processUserId: this.form.get('processUserControl')!.value?.value,
            year: (this.form.get('yearControl')!.value as Date)?.getFullYear(),
            submissionDate: this.form.get('submissionDateControl')!.value,
            submissionPerson: this.form.get('submissionPersonControl')!.value,
            submissionPersonWorkPosition: this.form.get('submissionPersonWorkPositionControl')!.value
        });

        return result;
    }

    private getNomenclatures(): Subscription {
        const subscription: Subscription = this.nomenclatures.getUserNames().subscribe((result: NomenclatureDTO<number>[]) => {
            this.processUsers = result;

            this.loader.complete();

        });

        return subscription;
    }
}