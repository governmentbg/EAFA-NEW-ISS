import { AfterViewInit, Component, EventEmitter, Input, OnInit, Optional, Output, Self } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { FishCodesEnum } from '@app/enums/fish-codes.enum';
import { FishNomenclatureDTO } from '@app/models/generated/dtos/FishNomenclatureDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { InspectedDeclarationCatchDTO } from '@app/models/generated/dtos/InspectedDeclarationCatchDTO';
import { ValidityCheckerDirective } from '@app/shared/directives/validity-checker/validity-checker.directive';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { CatchSizeCodesEnum } from '@app/enums/catch-size-codes.enum';

@Component({
    selector: 'inspected-catch',
    templateUrl: './inspected-catch.component.html'
})
export class InspectedCatchComponent extends CustomFormControl<InspectedDeclarationCatchDTO | undefined> implements OnInit, AfterViewInit {
    @Input()
    public viewMode: boolean = false;

    @Input()
    public fishes: FishNomenclatureDTO[] = [];

    @Input()
    public catchTypes: NomenclatureDTO<number>[] = [];

    @Input()
    public presentations: NomenclatureDTO<number>[] = [];

    @Input()
    public turbotSizeGroups: NomenclatureDTO<number>[] = [];

    @Input()
    public hasCatchType: boolean = true;

    @Input()
    public hasUndersizedCheck: boolean = false;

    @Input()
    public hasDeclaration: boolean = false;

    @Output()
    public deletePanelBtnClicked: EventEmitter<void> = new EventEmitter<void>();

    public expansionPanelTitle: string = '';
    public defaultExpansionPanelTitle: string = '';

    public showTurbotControl: boolean = false;

    public model: InspectedDeclarationCatchDTO | undefined;

    private readonly translate: FuseTranslationLoaderService;

    public constructor(
        @Self() ngControl: NgControl,
        @Self() @Optional() validityChecker: ValidityCheckerDirective,
        translate: FuseTranslationLoaderService
    ) {
        super(ngControl, false, validityChecker);

        this.translate = translate;

        this.defaultExpansionPanelTitle = this.translate.getValue('inspections.market-single-product-title');
    }

    public ngOnInit(): void {
        this.initCustomFormControl();

        this.form.valueChanges.subscribe({
            next: () => {
                const value: InspectedDeclarationCatchDTO | undefined = this.getValue();
                this.onChanged(value);
                this.setExpansionPanelTitle(value);
            }
        });
    }

    public ngAfterViewInit(): void {
        this.form.get('typeControl')?.valueChanges.subscribe({
            next: (value: NomenclatureDTO<number> | undefined) => {
                if (value === undefined || value === null || value instanceof NomenclatureDTO) {
                    this.setTurbotFlag(value);
                }
                else {
                    this.setTurbotFlag(undefined);
                }
            }
        });
    }

    public writeValue(value: InspectedDeclarationCatchDTO | undefined): void {
        this.model = value;

        if (value !== undefined && value !== null) {
            this.fillForm();
        }
        else {
            this.form.reset(new InspectedDeclarationCatchDTO());
        }

        setTimeout(() => {
            this.onChanged(this.getValue());
        });
    }

    public deletePanel(): void {
        this.deletePanelBtnClicked.emit();
    }

    protected buildForm(): AbstractControl {
        const form = new FormGroup({
            typeControl: new FormControl(undefined, [Validators.required]),
            countControl: new FormControl(undefined, [TLValidators.number(0, undefined, 0)]),
            catchTypeControl: new FormControl(undefined),
            quantityControl: new FormControl(undefined, [Validators.required, TLValidators.number(0)]),
            presentationControl: new FormControl(undefined, [Validators.required]),
            turbotSizeGroupControl: new FormControl(undefined),
            undersizedControl: new FormControl(false)
        });

        return form;
    }

    protected getValue(): InspectedDeclarationCatchDTO | undefined {
        if (this.model === undefined || this.model === null) {
            this.model = new InspectedDeclarationCatchDTO({
                // TODO
            });
        }

        this.model.fishTypeId = this.form.get('typeControl')!.value?.value;
        this.model.fishName = this.form.get('typeControl')!.value?.displayName;
        this.model.catchCount = this.form.get('countControl')!.value;
        this.model.catchQuantity = this.form.get('quantityControl')!.value;
        this.model.presentationId = this.form.get('presentationControl')!.value?.value;
        this.model.turbotSizeGroupId = this.form.get('turbotSizeGroupControl')!.value?.value;
        this.model.undersized = this.form.get('undersizedControl')!.value;

        if (this.hasUndersizedCheck) {
            this.model.catchTypeId = this.model.undersized === true
                ? this.catchTypes.find(x => x.code === CatchSizeCodesEnum[CatchSizeCodesEnum.BMS])?.value
                : this.catchTypes.find(x => x.code === CatchSizeCodesEnum[CatchSizeCodesEnum.LSC])?.value;
        }
        else {
            this.model.catchTypeId = this.form.get('catchTypeControl')!.value?.value;
        }

        return this.model;
    }

    private fillForm(): void {
        if (this.model !== undefined && this.model !== null) {
            if (this.model.fishTypeId !== undefined && this.model.fishTypeId !== null) {
                const fish: NomenclatureDTO<number> = this.fishes.find(x => x.value === this.model!.fishTypeId)!;
                this.form.get('typeControl')!.setValue(fish);
                this.setTurbotFlag(fish);
            }

            this.form.get('countControl')!.setValue(this.model.catchCount);
            this.form.get('quantityControl')!.setValue(this.model.catchQuantity);
            this.form.get('catchTypeControl')!.setValue(this.fishes.find(x => x.value === this.model!.catchTypeId));
            this.form.get('undersizedControl')!.setValue(this.model.undersized);
            this.form.get('turbotSizeGroupControl')!.setValue(this.turbotSizeGroups.find(x => x.value === this.model!.turbotSizeGroupId));

            this.form.get('presentationControl')!.setValue(this.presentations.find(x => x.value === this.model!.presentationId) ?? this.presentations.find(x => x.code === 'WHL'));

            this.setExpansionPanelTitle(this.model);
        }
    }

    private setExpansionPanelTitle(value: InspectedDeclarationCatchDTO | undefined): void {
        if (value !== undefined && value !== null) {
            if (value.fishName !== undefined && value.fishName !== null && value.fishName.length > 0) {
                this.expansionPanelTitle = `${value.fishName}`;

                if (value.catchQuantity !== undefined && value.catchQuantity !== null) {
                    this.expansionPanelTitle = `${this.expansionPanelTitle} - ${value.catchQuantity}kg`;
                }
            }
        }
        else {
            this.expansionPanelTitle = '';
        }
    }

    private setTurbotFlag(value: NomenclatureDTO<number> | undefined): void {
        if (value !== undefined && value !== null) {
            if (FishCodesEnum[value.code as keyof typeof FishCodesEnum] === FishCodesEnum.TUR) {
                this.showTurbotControl = true;

                if (this.viewMode) {
                    this.form.get('turbotSizeGroupControl')!.disable();
                }
            }
            else {
                this.showTurbotControl = false;
            }
        }
        else {
            this.showTurbotControl = false;
        }
    }
}