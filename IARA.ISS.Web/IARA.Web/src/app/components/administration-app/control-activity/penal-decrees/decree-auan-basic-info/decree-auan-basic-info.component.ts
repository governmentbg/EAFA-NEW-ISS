import { Component, Input, OnInit, Optional, Self } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';
import { PenalDecreeAuanDataDTO } from '@app/models/generated/dtos/PenalDecreeAuanDataDTO';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PenalDecreesService } from '@app/services/administration-app/penal-decrees.service';
import { ValidityCheckerDirective } from '@app/shared/directives/validity-checker/validity-checker.directive';
import { InspectorUserNomenclatureDTO } from '@app/models/generated/dtos/InspectorUserNomenclatureDTO';

@Component({
    selector: 'decree-auan-basic-info',
    templateUrl: './decree-auan-basic-info.component.html'
})
export class DecreeAuanBasicInfoComponent extends CustomFormControl<PenalDecreeAuanDataDTO> implements OnInit {
    @Input()
    public isAdding: boolean = false;

    public auan: PenalDecreeAuanDataDTO | undefined;
    public isFromRegister: boolean = false;

    public users: InspectorUserNomenclatureDTO[] = [];

    public readonly today: Date = new Date();

    private readonly service: PenalDecreesService;

    public constructor(
        @Self() ngControl: NgControl,
        @Self() @Optional() validityChecker: ValidityCheckerDirective,
        service: PenalDecreesService
    ) {
        super(ngControl, true, validityChecker);

        this.service = service;
    }

    public ngOnInit(): void {
        this.initCustomFormControl();

        if (!this.isAdding) {
            this.isFromRegister = true;
        }
        else {
            this.service.getInspectorUsernames().subscribe({
                next: (result: InspectorUserNomenclatureDTO[]) => {
                    this.users = result;
                }
            });

            this.form.get('drafterUserControl')!.setValidators(Validators.required);
            this.form.get('auanLocationDescriptionControl')!.setValidators([Validators.required, Validators.maxLength(400)]);
        }
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
            userId: this.form.get('drafterUserControl')!.value?.value,
            locationDescription: this.form.get('auanLocationDescriptionControl')!.value,
            inspectedEntity: this.form.get('auanInspectedEntityControl')!.value
        });

        return result;
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
            auanNumControl: new FormControl(null, [Validators.required, Validators.maxLength(20)]),
            auanDraftDateControl: new FormControl(null, Validators.required),
            auanDrafterControl: new FormControl(null),
            drafterUserControl: new FormControl(null),
            auanLocationDescriptionControl: new FormControl(null, Validators.maxLength(400)),
            auanInspectedEntityControl: new FormControl(null)
        });
    }
}