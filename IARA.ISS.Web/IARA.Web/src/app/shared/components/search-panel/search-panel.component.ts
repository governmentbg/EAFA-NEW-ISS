import { DatePipe } from '@angular/common';
import { AfterViewInit, Component, ContentChild, ContentChildren, EventEmitter, HostListener, Input, QueryList, ViewChild } from '@angular/core';
import { AbstractControl, ControlContainer, FormControl, FormGroup, FormGroupDirective } from '@angular/forms';
import { MatExpansionPanel } from '@angular/material/expansion';
import { faFilter, faRedo } from '@fortawesome/free-solid-svg-icons';
import moment from 'moment';
import { TLDateAdapter } from '../../utils/date.adapter';
import { FilterEventArgs } from '../data-table/models/filter-event-args.model';
import { DateRangeData } from '../input-controls/tl-date-range/tl-date-range.component';
import { RangeInputData } from '../input-controls/tl-range-input/range-input.component';
import { TLSlideToggleComponent } from '../input-controls/tl-slide-toggle/tl-slide-toggle.component';
import { ITranslateService } from './interfaces/translate-service.interface';
import { MatChipFilterModel } from './models/mat-chip-filter.model';
import { CommonUtils } from './utils';
import { DateUtils } from './utils/date.utils';

@Component({
    selector: 'search-panel',
    templateUrl: './search-panel.component.html',
    styleUrls: ['./search-panel.component.scss'],
    providers: [
        { provide: TLDateAdapter, useClass: TLDateAdapter }
    ]
})
export class SearchPanelComponent implements AfterViewInit {
    @Input()
    public searchString: string;

    @Input()
    public translateService!: ITranslateService;

    @ContentChild(ControlContainer)
    set advancedSerachFormGroupDirective(value: FormGroupDirective) {
        if (value !== null && value !== undefined) {
            this._advancedSearchFormGroup = value.form;
        }
    }

    @ContentChildren(TLSlideToggleComponent, { descendants: true })
    public set slideToggles(value: QueryList<TLSlideToggleComponent>) {
        this._slideToggles = value;
    }

    @ViewChild(MatExpansionPanel) panel?: MatExpansionPanel;

    faFilter = faFilter;
    faRedo = faRedo;

    public _advancedSearchFormGroup: FormGroup;
    public searchTextControl: FormControl;
    public _slideToggles!: QueryList<TLSlideToggleComponent>;
    public noAdvancedSearchFiltersEnterted: boolean;
    public filtersChanged: EventEmitter<FilterEventArgs> = new EventEmitter<FilterEventArgs>();
    public appliedFilters: MatChipFilterModel[];

    private dateAdapter: TLDateAdapter;
    private datePipe: DatePipe;

    public constructor(
        dateAdapter: TLDateAdapter,
        datePipe: DatePipe
    ) {
        this.dateAdapter = dateAdapter;
        this.datePipe = datePipe;

        // Instantiate objects and set default values
        this.searchString = '';
        this.searchTextControl = new FormControl(this.searchString)
        this._advancedSearchFormGroup = new FormGroup({});
        this.noAdvancedSearchFiltersEnterted = true;
        this.appliedFilters = [];
    }

    public ngAfterViewInit(): void {
        this._advancedSearchFormGroup.valueChanges.subscribe((value) => {
            this._advancedSearchFormGroup.markAsTouched();
            this.checkFieldsEmptyness();
            if (!CommonUtils.isNullOrEmpty(value)) {
                this.searchTextControl.setValue('');
            }
        });
        this.searchTextControl.valueChanges.subscribe((value: string) => {
            if (!CommonUtils.isNullOrEmpty(value)) {
                this.clearAdvancedSearchFields();
            }
            this.checkFieldsEmptyness();
        });
    }

    @HostListener('keyup', ['$event'])
    public onKeyUp(event: KeyboardEvent): void {
        if (event.key === 'Enter') {
            this.searchClicked();
        }
    }

    private clearAdvancedSearchFields(): void {
        for (const key of Object.keys(this._advancedSearchFormGroup.controls)) {
            this._advancedSearchFormGroup.get(key)?.reset('', { emitEvent: false });
        }
    }

    private checkFieldsEmptyness(): void {
        let noFiltersAssigned: boolean = true;
        for (const key of Object.keys(this._advancedSearchFormGroup.controls)) {
            if (!CommonUtils.isNullOrEmpty(this._advancedSearchFormGroup.get(key)?.value)) {
                noFiltersAssigned = false;
                break;
            }
        }
        this.noAdvancedSearchFiltersEnterted = noFiltersAssigned;
    }

    public clickedFormField(event: Event): void {
        event.stopPropagation();
    }

    public clearAllFiltersClicked(event: Event): void {
        event.stopPropagation();

        this.searchTextControl.reset('', { emitEvent: false });
        this.clearAdvancedSearchFields();
        this.noAdvancedSearchFiltersEnterted = true;

        this.openExpansionPanel();
    }

    public matChipFilterElementClearClicked(event: Event, filter: MatChipFilterModel): void {
        event.stopPropagation();

        const indexToSplice: number = this.appliedFilters.findIndex(x => x.control === filter.control && x.value === filter.value && x.displayValue === filter.displayValue);
        this.appliedFilters.splice(indexToSplice, 1);

        if (filter.partOfMultipleValue) {
            const numberOfValuesLeft: number = this.appliedFilters.filter(x => x.control === filter.control).length;
            if (numberOfValuesLeft === 0) {
                filter.control.reset();
            }
        }
        else {
            filter.control.reset();
        }

        this.filtersChanged.emit(this.getAppliedFilterArguments());
    }

    public refreshClicked(event: Event): void {
        event.stopPropagation();

        this.filtersChanged.emit(this.getAppliedFilterArguments());

        this.closeExpansionPanel();
    }

    public searchClicked(event?: Event): void {
        event?.stopPropagation();

        if (this.searchTextControl.touched && this.searchTextControl.value && this.searchTextControl.value?.length > 0) {
            this.appliedFilters = [];
            this.filtersChanged.emit(this.getFilterArguments());
            this.closeExpansionPanel();
        }
        else {
            this.appliedFilters = [];
            this.filtersChanged.emit(this.getFilterArguments());
            this.closeExpansionPanel();
        }
    }

    private getAppliedFilterArguments(): FilterEventArgs {
        const keyWordSearchValue: string = this.searchTextControl.value;
        const filters = new FilterEventArgs('');

        for (const filter of this.appliedFilters) {
            if (!filter.isKeywordSearch) {
                let value: string = '';
                if (this.isSimpleType(filter.value)) {
                    value = filter.value;
                }
                else if (typeof filter.value === 'object') {
                    value = (filter.value as any).value;
                }
                const formControlName: string = this.getFormControlName(filter.control);
                if (filters.AdvancedFilters.has(formControlName)) {
                    const elements = filters.AdvancedFilters.get(this.getFormControlName(filter.control));
                    elements.push(value);
                }
                else if (filter.partOfMultipleValue) {
                    filters.AdvancedFilters.set(this.getFormControlName(filter.control), [value]);
                }
                else {
                    filters.AdvancedFilters.set(this.getFormControlName(filter.control), value);
                }
            }
            else {
                filters.searchText = filter.value;
            }
        }
        return filters;
    }

    private getFormControlName(control: AbstractControl) {
        const parent = control.parent;
        let controlName: string = '';
        if (parent !== null && parent !== undefined) {
            for (const name of Object.keys(parent.controls)) {
                if (control === parent.get(name)) {
                    controlName = name;
                }
            }
        }
        return controlName;
    }

    private getFilterArguments(): FilterEventArgs {
        const keyWordSearchValue: string = (this.searchTextControl.value as string)?.trim();
        const filters = new FilterEventArgs(keyWordSearchValue);
        if (!CommonUtils.isNullOrEmpty(keyWordSearchValue)) {
            this.appliedFilters.push(new MatChipFilterModel(keyWordSearchValue, keyWordSearchValue, this.searchTextControl, true));
        }

        this.getAdvancedFilterValues(filters.AdvancedFilters, this._advancedSearchFormGroup);

        return filters;
    }

    private getAdvancedFilterValues(filters: Map<string, any>, formGroup: FormGroup): void {
        const formControls = formGroup.controls;
        const keys = Object.keys(formControls);

        if (keys.length > 0) {
            for (const key of keys) {
                const controlObject = formControls[key];

                if (controlObject instanceof FormGroup) {
                    this.getAdvancedFilterValues(filters, controlObject);
                }
                else if (controlObject instanceof FormControl && controlObject.valid) {
                    const filterValue = controlObject.value;
                    if (this.isSimpleType(filterValue) || filterValue === null) {
                        if (typeof filterValue === 'string') {
                            filters.set(key, filterValue?.trim());
                        }
                        else {
                            filters.set(key, filterValue);
                        }

                        if (typeof filterValue === 'boolean') {
                            if (filterValue === true) {
                                const formControlName: string = this.getFormControlName(controlObject);
                                const slideToggleComponent = this._slideToggles.find(x => this.getFormControlName(x.ngControl.control as AbstractControl) === formControlName);
                                if (slideToggleComponent !== undefined) {
                                    this.appliedFilters.push(new MatChipFilterModel(filterValue, slideToggleComponent.label, controlObject));
                                }
                            }
                        }
                        else if (!CommonUtils.isNullOrEmpty(filterValue)) {
                            if (filterValue instanceof Date) {
                                this.appliedFilters.push(new MatChipFilterModel(filterValue, DateUtils.ToDateString(filterValue as Date), controlObject));
                            }
                            else if ((filterValue as any) instanceof moment) { //MatDateTimePicker
                                const dateTime: string = this.datePipe.transform(filterValue, 'dd.MM.yyyy HH:mm')!;
                                this.appliedFilters.push(new MatChipFilterModel(filterValue, dateTime, controlObject));
                            }
                            else {
                                this.appliedFilters.push(new MatChipFilterModel(filterValue, filterValue.toString(), controlObject));
                            }
                        }
                    }
                    else if (Array.isArray(filterValue)) {
                        const filterArray = [];
                        for (const filter of filterValue) {
                            filterArray.push(filter.value);
                            if (!CommonUtils.isNullOrEmpty(filter) && !CommonUtils.isNullOrEmpty((filter as any).displayName)) {
                                this.appliedFilters.push(new MatChipFilterModel(filter, (filter as any).displayName, controlObject, false, true));
                            }
                        }
                        filters.set(key, filterArray);
                    }
                    else if (typeof filterValue === 'object') {
                        let displayValue: string = '';

                        if (filterValue instanceof RangeInputData) {
                            filters.set(key, filterValue);

                            if (filterValue.start !== undefined && filterValue.start !== null) {
                                displayValue = `${filterValue.start}` + '–';
                            }
                            if (filterValue.end !== undefined && filterValue.end !== null) {
                                if (!displayValue || displayValue.length === 0) {
                                    displayValue = '–';
                                }
                                displayValue += `${filterValue.end}`;
                            }
                        }
                        else if (filterValue instanceof DateRangeData) {
                            filters.set(key, filterValue);

                            if (filterValue.start !== undefined && filterValue.start !== null) {
                                displayValue = this.dateAdapter.format(filterValue.start, 'input') + '–';
                            }
                            if (filterValue.end !== undefined && filterValue.end !== null) {
                                if (!displayValue || displayValue.length === 0) {
                                    displayValue = '–';
                                }
                                displayValue += this.dateAdapter.format(filterValue.end, 'input');
                            }
                        }
                        else {
                            filters.set(key, (filterValue as any).value);

                            if (!CommonUtils.isNullOrEmpty(filterValue) && !CommonUtils.isNullOrEmpty((filterValue as any).displayName)) {
                                displayValue = (filterValue as any).displayName;
                            }
                        }

                        if (displayValue && displayValue.length > 0) {
                            this.appliedFilters.push(new MatChipFilterModel(filterValue, displayValue, controlObject));
                        }
                    }
                }
            }
        }
    }

    private isSimpleType(value: unknown): boolean {
        return typeof value === 'string' || typeof value === 'number' || typeof value === 'boolean' || value instanceof Date || (value as any) instanceof moment;
    }

    private closeExpansionPanel(): void {
        if (this.panel !== null && this.panel !== undefined) {
            this.panel.close();
        }
    }

    private openExpansionPanel(): void {
        if (this.panel !== null && this.panel !== undefined) {
            this.panel.open();
        }
    }
}