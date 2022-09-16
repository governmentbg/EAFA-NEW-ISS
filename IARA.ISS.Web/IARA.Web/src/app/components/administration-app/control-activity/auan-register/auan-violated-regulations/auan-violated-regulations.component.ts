import { Component, Input, OnInit, Self, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { RecordChangedEventArgs } from '@app/shared/components/data-table/models/record-changed-event.model';
import { AuanViolatedRegulationDTO } from '@app/models/generated/dtos/AuanViolatedRegulationDTO';
import { AuanViolatedRegulationTypesEnum } from '@app/enums/auan-violated-regulation-types.enum';

@Component({
    selector: 'auan-violated-regulations',
    templateUrl: './auan-violated-regulations.component.html'
})
export class AuanViolatedRegulationsComponent extends CustomFormControl<AuanViolatedRegulationDTO[]> implements OnInit {
    @Input()
    public viewMode!: boolean;

    public violatedRegulationsForm!: FormGroup;
    public violatedRegulations: AuanViolatedRegulationDTO[] = [];
    public translate: FuseTranslationLoaderService;

    public isDisabled: boolean = false;

    public violatedRegulationsTouched: boolean = false;

    public violatedRegulationTypes: NomenclatureDTO<AuanViolatedRegulationTypesEnum>[] = [];

    @ViewChild('violatedRegulationsTable')
    private violatedRegulationsTable!: TLDataTableComponent;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService
    ) {
        super(ngControl);

        this.translate = translate;

        this.violatedRegulationTypes = [
            new NomenclatureDTO<AuanViolatedRegulationTypesEnum>({
                value: AuanViolatedRegulationTypesEnum.Law,
                displayName: this.translate.getValue('auan-register.violated-regulation-type-law'),
                isActive: true
            }),
            new NomenclatureDTO<AuanViolatedRegulationTypesEnum>({
                value: AuanViolatedRegulationTypesEnum.Regulation,
                displayName: this.translate.getValue('auan-register.violated-regulation-type-regulation'),
                isActive: true
            })
        ];
    }

    public async ngOnInit(): Promise<void> {
        this.initCustomFormControl();
    }

    public writeValue(value: AuanViolatedRegulationDTO[]): void {
        if (value !== null && value !== undefined) {
            setTimeout(() => {
                this.violatedRegulations = value;
            });
        }
        else {
            setTimeout(() => {
                this.violatedRegulations = [];
            });
        }
    }

    public onUndoAddEditRow(row: GridRow<AuanViolatedRegulationDTO>): void {
        this.violatedRegulationsTable.undoAddEditRow(row);
    }

    public setDisabledState(isDisabled: boolean): void {
        this.isDisabled = isDisabled;
        if (this.isDisabled) {
            this.violatedRegulationsForm.disable();
        }
        else {
            this.violatedRegulationsForm.enable();
        }
    }

    public violatedRegulationsRecordChanged(event: RecordChangedEventArgs<AuanViolatedRegulationDTO>): void {
        this.violatedRegulations = this.violatedRegulationsTable.rows.map(x => new AuanViolatedRegulationDTO({
            id: x.id,
            article: x.article,
            paragraph: x.paragraph,
            section: x.section,
            letter: x.letter,
            type: x.type,
            isActive: x.isActive ?? true
        }));

        this.onChanged(this.violatedRegulations);
    }

    protected getValue(): AuanViolatedRegulationDTO[] {
        this.violatedRegulations = this.violatedRegulationsTable.rows;
        return this.violatedRegulations;
    }

    protected buildForm(): AbstractControl {
        this.violatedRegulationsForm = new FormGroup({
            articleControl: new FormControl(null, [Validators.required, Validators.maxLength(10)]),
            paragraphControl: new FormControl(null, Validators.maxLength(10)),
            sectionControl: new FormControl(null, Validators.maxLength(10)),
            letterControl: new FormControl(null, Validators.maxLength(10)),
            typeControl: new FormControl(null, Validators.required)
        });

        return new FormControl(null);
    }
}