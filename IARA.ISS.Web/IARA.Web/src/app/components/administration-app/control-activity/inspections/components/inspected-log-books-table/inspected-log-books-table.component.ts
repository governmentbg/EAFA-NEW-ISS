import { AfterViewInit, Component, OnInit, Self, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';

import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { InspectionUtils } from '@app/shared/utils/inspection.utils';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { InspectionToggleTypesEnum } from '@app/enums/inspection-toggle-types.enum';
import { RecordChangedEventArgs } from '@app/shared/components/data-table/models/record-changed-event.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { InspectionCheckDTO } from '@app/models/generated/dtos/InspectionCheckDTO';
import { InspectedLogBookTableModel } from '../../models/inspected-log-book-table.model';
import { InspectionLogBookDTO } from '@app/models/generated/dtos/InspectionLogBookDTO';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { InspectionsService } from '@app/services/administration-app/inspections.service';
import { DeclarationLogBookTypeEnum } from '@app/enums/declaration-log-book-type.enum';
import { InspectionLogBookPageNomenclatureDTO } from '@app/models/generated/dtos/InspectionLogBookPageNomenclatureDTO';

@Component({
    selector: 'inspected-log-books-table',
    templateUrl: './inspected-log-books-table.component.html'
})
export class InspectedLogBooksTableComponent extends CustomFormControl<InspectionLogBookDTO[]> implements OnInit, AfterViewInit {
    public logBooksFormGroup!: FormGroup;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    public logBooks: InspectedLogBookTableModel[] = [];

    public logBookPages: InspectionLogBookPageNomenclatureDTO[] = [];
    public readonly options: NomenclatureDTO<InspectionToggleTypesEnum>[];

    private readonly service: InspectionsService;
    private readonly translate: FuseTranslationLoaderService;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
        service: InspectionsService
    ) {
        super(ngControl);

        this.translate = translate;
        this.service = service;

        this.options = InspectionUtils.getToggleCheckOptions(translate);

        this.onMarkAsTouched.subscribe({
            next: () => {
                this.control.updateValueAndValidity();
            }
        });
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public ngAfterViewInit(): void {
        this.logBooksFormGroup.get('numberControl')!.valueChanges.subscribe({
            next: (value: string) => {
                if (value !== null && value !== undefined) {
                    this.service.getLogBookPagesByLogBookNum(DeclarationLogBookTypeEnum.ShipLogBook, value).subscribe({
                        next: (pages: InspectionLogBookPageNomenclatureDTO[]) => {
                            this.logBookPages = pages;
                        }
                    });
                }
            }
        });
    }

    public writeValue(value: InspectionLogBookDTO[]): void {
        if (value !== undefined && value !== null) {
            const logBooks = value.map(f => {

                if (f.checkValue === null || f.checkValue === undefined) {
                    f.checkValue = InspectionToggleTypesEnum.X;
                }

                return new InspectedLogBookTableModel({
                    id: f.id,
                    checkValue: f.checkValue,
                    description: f.description,
                    endPage: f.endPage,
                    from: f.from,
                    isRegistered: f.logBookId !== null && f.logBookId !== undefined,
                    logBookId: f.logBookId,
                    number: f.number,
                    pageNum: f.pageNum,
                    page: f.pageId !== null && f.pageId !== undefined && f.pageNum !== undefined
                        ? new NomenclatureDTO<number>({
                            value: f.pageId,
                            displayName: f.pageNum,
                            isActive: true
                        }) : undefined,
                    pageId: f.pageId,
                    startPage: f.startPage,
                    checkDTO: new InspectionCheckDTO({
                        id: 0,
                        checkValue: f.checkValue,
                    })
                });
            });

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

        this.logBooksFormGroup.get('optionsControl')!.setValue(new NomenclatureDTO<InspectionToggleTypesEnum>({
            value: InspectionToggleTypesEnum.Y,
            displayName: this.translate.getValue('inspections.toggle-matches'),
            isActive: true,
        }));
    }

    public onEditRecord(record: InspectedLogBookTableModel): void {
        this.logBooksFormGroup.get('numberControl')!.disable();
        this.logBooksFormGroup.get('optionsControl')!.enable();

        if (record) {
            this.logBooksFormGroup.get('optionsControl')!.setValue(this.options.find(f => f.value === record.checkDTO?.checkValue));
            this.logBooksFormGroup.get('pageControl')!.setValue(record.page ?? record.pageNum); 
        }
    }

    public logBookRecordChanged(event: RecordChangedEventArgs<InspectedLogBookTableModel>): void {
        const nom: NomenclatureDTO<InspectionToggleTypesEnum> = this.logBooksFormGroup.get('optionsControl')!.value;

        event.Record.checkDTO = nom ? new InspectionCheckDTO({
            id: event.Record.checkDTO?.id,
            checkValue: nom.value,
        }) : undefined;
        event.Record.checkValue = nom?.value;

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

    public hideDeleteButtonWhen(row: GridRow<InspectedLogBookTableModel>): boolean {
        return !row.data.isRegistered;
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

        this.logBooksFormGroup.controls.optionsControl.valueChanges.subscribe({
            next: this.optionsChanged.bind(this)
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
            pageId: f.page?.value,
            pageNum: f.pageNum,
            startPage: f.startPage,
        }));
    }

    private optionsChanged(value: NomenclatureDTO<InspectionToggleTypesEnum>): void {
        const pageControl = this.logBooksFormGroup.controls.pageControl;

        if (value?.value === InspectionToggleTypesEnum.X) {
            pageControl.clearValidators();
        }
        else {
            pageControl.setValidators([Validators.required]);
        }

        pageControl.markAsPending();
        pageControl.updateValueAndValidity({ emitEvent: false });
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