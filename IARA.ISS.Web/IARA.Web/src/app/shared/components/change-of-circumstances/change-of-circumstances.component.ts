import { Component, Input, OnInit, Self } from '@angular/core';
import { AbstractControl, FormArray, FormControl, NgControl } from '@angular/forms';
import { Subscription } from 'rxjs';

import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ChangeOfCircumstancesDTO } from '@app/models/generated/dtos/ChangeOfCircumstancesDTO';
import { ChangeOfCircumstancesTypeDTO } from '@app/models/generated/dtos/ChangeOfCircumstancesTypeDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';

@Component({
    selector: 'change-of-circumstances',
    templateUrl: './change-of-circumstances.component.html',
    styleUrls: ['./change-of-circumstances.component.scss']
})
export class ChangeOfCircumstancesComponent extends CustomFormControl<ChangeOfCircumstancesDTO[]> implements OnInit {
    @Input()
    public pageCode!: PageCodeEnum;

    @Input()
    public disableDelete: boolean = false;

    @Input()
    public showOnlyRegiXData: boolean = false;

    public types: ChangeOfCircumstancesTypeDTO[] = [];

    private nomenclatures: CommonNomenclatures;

    private readonly loader: FormControlDataLoader;

    public constructor(@Self() ngControl: NgControl, nomenclatures: CommonNomenclatures) {
        super(ngControl);
        this.nomenclatures = nomenclatures;

        this.loader = new FormControlDataLoader(this.getChangeOfCircumstancesTypes.bind(this));
    }

    public ngOnInit(): void {
        this.initCustomFormControl();

        this.loader.load(() => {
            if (this.formArray.length === 0) {
                this.addChangeControl(new ChangeOfCircumstancesDTO());
                this.setDisabledState(this.isDisabled);
            }
        });
    }

    public writeValue(changes: ChangeOfCircumstancesDTO[]): void {
        if (changes !== undefined && changes !== null) {
            this.loader.load(() => {
                if (changes.length !== 0) {
                    this.formArray.clear();
                }

                for (const change of changes) {
                    this.addChangeControl(change);
                }

                this.setDisabledState(this.isDisabled);
            });
        }
        else {
            this.loader.load(() => {
                if (this.formArray.length === 0) {
                    this.addChangeControl(new ChangeOfCircumstancesDTO());
                    this.setDisabledState(this.isDisabled);
                }
            });
        }
    }

    protected buildForm(): AbstractControl {
        return new FormArray([]);
    }

    protected getValue(): ChangeOfCircumstancesDTO[] {
        return this.formArray.value;
    }

    public addChange(): void {
        this.addChangeControl();
        this.onChanged(this.getValue());
    }

    public removeChange(index: number): void {
        this.formArray.removeAt(index);
        this.onChanged(this.getValue());
    }

    private addChangeControl(change?: ChangeOfCircumstancesDTO): void {
        const control: FormControl = new FormControl(change ?? new ChangeOfCircumstancesDTO());
        this.formArray.push(control);

        this.onChanged(this.getValue());
    }

    private getChangeOfCircumstancesTypes(): Subscription {
        return NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.ChangeOfCircumstancesTypes, this.nomenclatures.getChangeOfCircumstancesTypes.bind(this.nomenclatures), false
        ).subscribe({
            next: (types: ChangeOfCircumstancesTypeDTO[]) => {
                this.types = types.filter(x => !x.isDeletion && x.pageCode === this.pageCode);

                this.loader.complete();
            }
        });
    }
}