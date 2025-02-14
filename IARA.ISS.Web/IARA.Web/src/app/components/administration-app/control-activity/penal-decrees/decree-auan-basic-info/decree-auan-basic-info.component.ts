import { Component, Input, OnInit, Optional, Self } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';
import { PenalDecreeAuanDataDTO } from '@app/models/generated/dtos/PenalDecreeAuanDataDTO';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PenalDecreesService } from '@app/services/administration-app/penal-decrees.service';
import { ValidityCheckerDirective } from '@app/shared/directives/validity-checker/validity-checker.directive';
import { InspectorUserNomenclatureDTO } from '@app/models/generated/dtos/InspectorUserNomenclatureDTO';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';

@Component({
    selector: 'decree-auan-basic-info',
    templateUrl: './decree-auan-basic-info.component.html'
})
export class DecreeAuanBasicInfoComponent extends CustomFormControl<PenalDecreeAuanDataDTO> implements OnInit {
    @Input()
    public isAdding: boolean = false;

    @Input()
    public isFromRegister: boolean = false;

    public auan: PenalDecreeAuanDataDTO | undefined;

    public readonly today: Date = new Date();

    private readonly service: PenalDecreesService;
    private readonly translate: FuseTranslationLoaderService;

    public constructor(
        @Self() ngControl: NgControl,
        @Self() @Optional() validityChecker: ValidityCheckerDirective,
        service: PenalDecreesService,
        translate: FuseTranslationLoaderService
    ) {
        super(ngControl, true, validityChecker);

        this.service = service;
        this.translate = translate;
    }

    public ngOnInit(): void {
        this.initCustomFormControl();

        if (this.isAdding) {
            this.form.valueChanges.subscribe({
                next: () => {
                    const auan: PenalDecreeAuanDataDTO = this.getValue();
                    this.onChanged(auan);
                }
            })
        }
    }

    public writeValue(value: PenalDecreeAuanDataDTO): void {
        if (value !== undefined && value !== null) {
            this.auan = value;
          
            this.form.get('auanNumControl')!.setValue(value.auanNum);
            this.form.get('auanDraftDateControl')!.setValue(value.draftDate);

            if (value.isExternal) {
                this.form.get('drafterNameControl')!.setValue(value.inspectorName);
                this.form.get('locationDescriptionControl')!.setValue(value.locationDescription);
            }
            else {
                this.form.get('auanDrafterControl')!.setValue(value.drafter);
                this.form.get('auanLocationDescriptionControl')!.setValue(value.locationDescription);
            }

            this.form.get('auanInspectedEntityControl')!.setValue(value.inspectedEntity);
        }
    }

    protected getValue(): PenalDecreeAuanDataDTO {
        const result: PenalDecreeAuanDataDTO = new PenalDecreeAuanDataDTO({
            auanNum: this.form.get('auanNumControl')!.value,
            draftDate: this.form.get('auanDraftDateControl')!.value,
            userId: this.form.get('drafterUserControl')!.value?.value,
            inspectedEntity: this.form.get('auanInspectedEntityControl')!.value
        });

        if (!this.isFromRegister) {
            result.inspectorName = this.form.get('drafterNameControl')!.value;
            result.locationDescription = this.form.get('locationDescriptionControl')!.value;
        }

        return result;
    }

    protected buildForm(): AbstractControl {
        const form: FormGroup = new FormGroup({
            auanNumControl: new FormControl(null, [Validators.required, Validators.maxLength(20)]),
            auanDraftDateControl: new FormControl(null, Validators.required),
            auanDrafterControl: new FormControl(null),
            drafterUserControl: new FormControl(null),
            auanLocationDescriptionControl: new FormControl(null, Validators.maxLength(400)),
            locationDescriptionControl: new FormControl(null, Validators.maxLength(400)),
            drafterNameControl: new FormControl(null),
            auanInspectedEntityControl: new FormControl(null)
        });

        if (!this.isFromRegister) {
            form.get('drafterNameControl')!.setValidators([Validators.required, Validators.maxLength(4000)]);
            form.get('locationDescriptionControl')!.setValidators([Validators.required, Validators.maxLength(400)]);
        }

        return form;
    }
}