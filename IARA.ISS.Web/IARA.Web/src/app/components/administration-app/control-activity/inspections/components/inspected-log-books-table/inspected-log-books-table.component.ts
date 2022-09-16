import { Component, OnInit, Self, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';

import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { InspectionUtils } from '@app/shared/utils/inspection.utils';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { InspectionToggleTypesEnum } from '@app/enums/inspection-toggle-types.enum';
import { InspectionCheckModel } from '../../models/inspection-check.model';
import { RecordChangedEventArgs } from '@app/shared/components/data-table/models/record-changed-event.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { InspectionCheckDTO } from '@app/models/generated/dtos/InspectionCheckDTO';
import { InspectedLogBookTableModel } from '../../models/inspected-log-book-table.model';
import { InspectionLogBookDTO } from '@app/models/generated/dtos/InspectionLogBookDTO';

@Component({
    selector: 'inspected-log-books-table',
    templateUrl: './inspected-log-books-table.component.html'
})
export class InspectedLogBooksTableComponent extends CustomFormControl<InspectionLogBookDTO[]> implements OnInit {
    public logBooksFormGroup!: FormGroup;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    public logBooks: InspectedLogBookTableModel[] = [];

    public readonly options: NomenclatureDTO<InspectionToggleTypesEnum>[];
    public readonly fakeToggle: InspectionCheckModel;

    private readonly translate: FuseTranslationLoaderService;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
    ) {
        super(ngControl);

        this.translate = translate;

        this.options = InspectionUtils.getToggleCheckOptions(translate);

        this.fakeToggle = new InspectionCheckModel({
            value: 0,
            isMandatory: true
        });

        this.onMarkAsTouched.subscribe({
            next: () => {
                this.control.updateValueAndValidity();
            }
        });
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public writeValue(value: InspectionLogBookDTO[]): void {
        if (value !== undefined && value !== null) {
            const logBooks = value.map(f => new InspectedLogBookTableModel({
                id: f.id,
                checkValue: f.checkValue,
                description: f.description,
                endPage: f.endPage,
                from: f.from,
                isRegistered: f.logBookId !== null && f.logBookId !== undefined,
                logBookId: f.logBookId,
                number: f.number,
                pageNum: f.pageNum,
                page: f.logBookId !== null && f.logBookId !== undefined
                    ? new NomenclatureDTO<number>({
                        value: f.logBookId,
                        displayName: f.pageNum,
                    }) : undefined,
                pageId: f.pageId,
                startPage: f.startPage,
                checkDTO: new InspectionCheckDTO({
                    id: 0,
                    checkValue: f.checkValue,
                })
            }));

            setTimeout(() => {
                this.logBooks = logBooks;
            });
        }
        else {
            setTimeout(() => {
                this.logBooks = [];
            });
        }
    }

    public findOption(check?: InspectionCheckDTO): string | undefined {
        if (check === null || check === undefined || check.checkValue === null || check.checkValue === undefined) {
            return this.translate.getValue('inspections.toggle-unchosen');
        }

        return this.options.find(f => f.value === check?.checkValue)?.displayName;
    }

    public onAddRecord(): void {
        this.logBooksFormGroup.get('numberControl')!.enable();
        this.logBooksFormGroup.get('optionsControl')!.disable();
    }

    public onEditRecord(record: InspectedLogBookTableModel): void {
        this.logBooksFormGroup.get('numberControl')!.disable();
        this.logBooksFormGroup.get('optionsControl')!.enable();
        this.logBooksFormGroup.get('optionsControl')!.setValue(record.checkValue);
        this.logBooksFormGroup.get('pageControl')!.setValue(record.page ?? record.pageNum);
    }

    public logBookRecordChanged(event: RecordChangedEventArgs<InspectedLogBookTableModel>): void {
        event.Record.checkDTO = this.logBooksFormGroup.get('optionsControl')!.value;
        event.Record.checkValue = event.Record.checkDTO?.checkValue;

        const page: NomenclatureDTO<number> | string = this.logBooksFormGroup.get('pageControl')!.value;

        if (typeof page === 'string') {
            event.Record.pageNum = page;
        }
        else {
            event.Record.page = page;
            event.Record.pageNum = page?.displayName;
        }

        this.logBooks = this.datatable.rows;
        this.control.updateValueAndValidity();
        this.onChanged(this.getValue());
    }

    protected buildForm(): AbstractControl {
        this.logBooksFormGroup = new FormGroup({
            numberControl: new FormControl({ value: undefined, disabled: true }, [Validators.required]),
            fromControl: new FormControl({ value: undefined, disabled: true }),
            startPageControl: new FormControl({ value: undefined, disabled: true }),
            endPageControl: new FormControl({ value: undefined, disabled: true }),
            pageControl: new FormControl(undefined, [Validators.required]),
            descriptionControl: new FormControl(undefined),
            optionsControl: new FormControl(undefined, [Validators.required]),
        });

        return new FormControl(undefined, this.logBooksValidator());
    }

    protected getValue(): InspectionLogBookDTO[] {
        return this.logBooks.map(f => new InspectionLogBookDTO({
            id: f.id,
            checkValue: f.checkDTO?.checkValue,
            description: f.description,
            endPage: f.endPage,
            from: f.from,
            logBookId: f.logBookId,
            number: f.number,
            pageId: f.pageId,
            pageNum: f.pageNum,
            startPage: f.startPage,
        }));
    }

    private logBooksValidator(): ValidatorFn {
        return (): ValidationErrors | null => {
            if (this.logBooks !== undefined && this.logBooks !== null) {
                for (const logBook of this.logBooks) {
                    if (logBook.checkValue === null || logBook.checkValue === undefined) {
                        return { 'logBooksMustBeChecked': true };
                    }
                }
            }
            return null;
        };
    }
}