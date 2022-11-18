import { Component, ElementRef, Input, OnChanges, OnInit, Optional, Self, SimpleChange, SimpleChanges, ViewChild } from '@angular/core';
import { AbstractControl, NgControl, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { MatAutocompleteTrigger } from '@angular/material/autocomplete';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLTranslatePipe } from '@app/shared/pipes/tl-translate.pipe';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { BaseTLControl } from '../base-tl-control';
import { IGroupedOptions } from './interfaces/grouped-options.interface';

type NomenclatureFilterFn = <T>(object: NomenclatureDTO<T> | string, options: NomenclatureDTO<T>[]) => NomenclatureDTO<T>[];
type StringFilterFn = (object: string, options: string[]) => string[];

@Component({
    selector: 'tl-autocomplete',
    templateUrl: './tl-autocomplete.component.html',
    styleUrls: ['./tl-autocomplete.component.scss']
})
export class TLInputAutocompleteComponent<T> extends BaseTLControl implements OnInit, OnChanges {
    @Input()
    public options: NomenclatureDTO<T>[] | string[] | undefined;

    @Input()
    public groupedOptions: IGroupedOptions<T>[] | undefined;

    @Input()
    public showClearButton: boolean = true;

    @Input()
    public hasSelectedValueFromDropdownValidator: boolean = true;

    @Input()
    public autoActiveFirstOption: boolean = false;

    @Input()
    public autoMatchTextToOption: boolean = false;

    @Input()
    public codeInTemplateOptions: boolean = false;

    @Input()
    public templateOptions: boolean = false;

    @Input()
    public keepPanelOpenAfterSelect: boolean = false;

    @Input()
    public focusoutOnSelect: boolean = false;

    @Input()
    public displayFn: ((object: NomenclatureDTO<T> | string | null) => string) | undefined;

    @Input()
    public filterFn: NomenclatureFilterFn & StringFilterFn | undefined;

    public warningHint: string | undefined;

    public hasOptionsCollection: boolean = false;
    public hasGroupedOptionsCollection: boolean = false;

    public filteredOptions: NomenclatureDTO<T>[] | string[] | undefined;
    public groupedFilteredOptions: IGroupedOptions<T>[] | undefined;

    public itemHeightPx: number = 55;
    public height: string = '200px';

    @ViewChild('autoCompleteInput', { read: MatAutocompleteTrigger })
    private autoCompleteTrigger!: MatAutocompleteTrigger;

    @ViewChild('autoCompleteInput', { read: ElementRef })
    private autoCompleteInputRef!: ElementRef<HTMLInputElement>;

    private activeOptions: NomenclatureDTO<T>[] | string[] = [];

    public constructor(
        @Self() @Optional() ngControl: NgControl,
        fuseTranslationService: FuseTranslationLoaderService,
        tlTranslatePipe: TLTranslatePipe
    ) {
        super(ngControl, fuseTranslationService, tlTranslatePipe);
    }

    public ngOnInit(): void {
        super.ngOnInit();

        // Register subscriber to filter options on change
        if (this.ngControl.control !== null && this.ngControl.control !== undefined) {
            this.ngControl.control.valueChanges.subscribe({
                next: (value: NomenclatureDTO<T> | string | undefined) => {
                    this.updateFilteredCollection(value);

                    if (this.autoMatchTextToOption && typeof value === 'string') {
                        this.autoMatchFn(value);
                    }

                    if (this.keepPanelOpenAfterSelect) {
                        if (!this.autoCompleteTrigger.panelOpen) {
                            this.autoCompleteTrigger.openPanel();
                        }
                    }

                    if (this.focusoutOnSelect && value instanceof NomenclatureDTO) {
                        this.inputFocusout();
                    }
                }
            });

            // Set options
            this.setOptions();

            // Add selectedValueFromDropdownValidator
            if (this.hasSelectedValueFromDropdownValidator) {
                this.setHasSelectedValueFromDropdownValidator();
            }

            // Override setValidators
            this.overrideSetValidators();
        }
    }

    public ngOnChanges(changes: SimpleChanges): void {
        const options: SimpleChange | undefined = changes['options'];
        const groupedOptions: SimpleChange | undefined = changes['groupedOptions'];
        const hasSelectedValueFromDropdownValidator: SimpleChange | undefined = changes['hasSelectedValueFromDropdownValidator'];

        if (options || groupedOptions) {
            this.setOptions();
        }

        if (hasSelectedValueFromDropdownValidator && !hasSelectedValueFromDropdownValidator.firstChange) {
            this.setHasSelectedValueFromDropdownValidator();
        }
    }

    public displayFunction = (object: NomenclatureDTO<T> | string | null): string => {
        if (this.displayFn !== undefined && this.displayFn !== null) {
            return this.displayFn(object);
        }

        if (object !== null && object !== undefined) {
            if (typeof object === 'string') {
                return object;
            }
            else if (object.displayName !== null && object.displayName !== undefined) {
                return object.displayName;
            }
        }
        return '';
    };

    public clearBtnClicked(): void {
        if (this.ngControl.control) {
            this.ngControl.control.reset();
            this.ngControl.control.markAsTouched();
        }
    }

    protected buildErrorsCollection(): void {
        super.buildErrorsCollection();

        if (this.ngControl && this.ngControl.control) {
            const errors: ValidationErrors | null = this.noOptionsValidator()(this.ngControl.control);
            if (errors && errors['novaluesindropdown'] === true) {
                this.warningHint = this.tlTranslatePipe.transform('validation.novaluesindropdown', 'cap');
            }
            else {
                this.warningHint = undefined;
            }
        }
    }

    private setOptions(): void {
        if (this.options !== undefined && this.options !== null) {
            this.hasOptionsCollection = true;
            this.setActiveOptions(this.options);
            this.updateFilteredCollection(this.ngControl.control?.value);
        }

        if (this.groupedOptions !== undefined && this.groupedOptions !== null) {
            this.hasGroupedOptionsCollection = true;
            this.updateFilteredCollection(this.ngControl.control?.value);
        }

        if (this.hasOptionsCollection && this.hasGroupedOptionsCollection) {
            throw new Error('Cannot have both [options] and [groupedOptions] in tl-autocomplete.');
        }
    }

    private setActiveOptions(options: NomenclatureDTO<T>[] | string[]): void {
        if (options.length > 0 && this.isNomenclatures(options)) {
            this.activeOptions = options.filter(x => x.isActive);
        }
        else {
            this.activeOptions = options;
        }
    }

    private setHasSelectedValueFromDropdownValidator(): void {
        if (this.ngControl && this.ngControl.control) {
            if (this.hasSelectedValueFromDropdownValidator) {
                if (this.ngControl.control.validator) {
                    this.ngControl.control.validator = Validators.compose([this.ngControl.control.validator, this.selectedValueFromDropdownValidator()]);
                }
                else {
                    this.ngControl.control.validator = this.selectedValueFromDropdownValidator();
                }
                this.ngControl.control.updateValueAndValidity({ onlySelf: true, emitEvent: false });
            }
        }
    }

    private updateFilteredCollection(value: NomenclatureDTO<T> | string | undefined): void {
        // Filter options
        if (this.hasOptionsCollection && this.activeOptions) {
            if (this.activeOptions.length > 0) {
                if (this.isNomenclatures(this.activeOptions)) {
                    this.filteredOptions = this.filterNomenclatureOptions(this.activeOptions, value);
                }
                else {
                    this.filteredOptions = this.filterStringOptions(this.activeOptions, value as string);
                }

                this.setDropdownHeight(this.filteredOptions);
            }
            else {
                this.filteredOptions = [];
            }
        }
        // Filter grouped options
        else if (this.hasGroupedOptionsCollection && this.groupedOptions) {
            if (this.groupedOptions.length > 0 && this.groupedOptions[0].options?.length > 0) {
                if (this.isNomenclatures(this.groupedOptions[0].options)) {
                    this.groupedFilteredOptions = this.filterNomenclatureGroups(this.groupedOptions, value);
                }
                else {
                    this.groupedFilteredOptions = this.filterStringGroups(this.groupedOptions, value as string);
                }

                this.setDropdownHeight(this.groupedFilteredOptions);
            }
            else {
                this.groupedFilteredOptions = [];
            }
        }
    }

    private filterNomenclatureOptions(options: NomenclatureDTO<T>[], value: string | NomenclatureDTO<T> | undefined): NomenclatureDTO<T>[] {
        if (value !== null && value !== undefined) {
            if (this.filterFn !== undefined && this.filterFn !== null) {
                return this.filterFn(value, options);
            }

            let filterValue: string = '';

            if (this.isNomenclature(value)) {
                filterValue = value.displayName?.toLowerCase() ?? '';
            }
            else {
                filterValue = value.toLowerCase();
            }

            return options.filter((option: NomenclatureDTO<T>) => {
                if (option.displayName?.toLowerCase()?.includes(filterValue)) {
                    return true;
                }

                if (this.templateOptions === true && option.description?.toLocaleLowerCase()?.includes(filterValue)) {
                    return true;
                }
                return false;
            });
        }

        return options.slice();
    }

    private filterStringOptions(options: string[], value: string | undefined): string[] {
        if (value !== null && value !== undefined) {
            if (this.filterFn !== undefined && this.filterFn !== null) {
                return this.filterFn(value, options);
            }

            const filterValue: string = value.toLowerCase();

            return options.filter((option: string) => {
                return option.toLowerCase().includes(filterValue);
            });
        }

        return options.slice();
    }

    private filterNomenclatureGroups(groupedOptions: IGroupedOptions<T>[], value: string | NomenclatureDTO<T> | undefined): IGroupedOptions<T>[] {
        if (groupedOptions !== null && groupedOptions !== undefined) {
            return groupedOptions.map((group: IGroupedOptions<T>) => {
                return {
                    name: group.name,
                    options: this.filterNomenclatureOptions(group.options as NomenclatureDTO<T>[], value)
                }
            });
        }

        return [];
    }

    private filterStringGroups(groupedOptions: IGroupedOptions<T>[], value: string | undefined): IGroupedOptions<T>[] {
        if (groupedOptions !== null && groupedOptions !== undefined) {
            return groupedOptions.map((group: IGroupedOptions<T>) => {
                return {
                    name: group.name,
                    options: this.filterStringOptions(group.options as string[], value)
                }
            });
        }

        return [];
    }

    private autoMatchFn(value: string | undefined) {
        if (this.filteredOptions && this.filteredOptions.length > 0 && this.isNomenclatures(this.filteredOptions)) {
            if (value !== undefined) {
                const matches: NomenclatureDTO<T>[] = this.filteredOptions.filter(x => x.displayName === value);

                if (matches.length === 1 && matches[0].isActive) {
                    this.formControl?.setValue(matches[0]);
                }
            }
        }
    }

    private setDropdownHeight(filteredOptions: NomenclatureDTO<T>[] | string[] | IGroupedOptions<T>[] | undefined): void {
        let visibleOptions: NomenclatureDTO<T>[] | string[] | IGroupedOptions<T>[];

        if (filteredOptions && filteredOptions.length !== 0) {
            if (filteredOptions.length > 0
                && (filteredOptions[0] instanceof NomenclatureDTO
                    || ((filteredOptions[0] as IGroupedOptions<T>) !== undefined
                        && (filteredOptions[0] as IGroupedOptions<T>).options !== undefined
                        && (filteredOptions[0] as IGroupedOptions<T>).options[0] instanceof NomenclatureDTO
                    )
                )
            ) {
                visibleOptions = (filteredOptions as NomenclatureDTO<T>[]).filter(x => x.isActive);
            }
            else {
                visibleOptions = filteredOptions;
            }

            this.itemHeightPx = 48;

            if (visibleOptions.length < 5) {
                this.height = `${visibleOptions.length * this.itemHeightPx}px`;
            }
            else {
                this.height = `${5 * this.itemHeightPx}px`;
            }
        }
    }

    private selectedValueFromDropdownValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (control.value !== null && control.value !== undefined && control.value !== '') {
                if (this.hasOptionsCollection && this.activeOptions && this.activeOptions.length > 0) {
                    if (this.isNomenclatures(this.activeOptions)) {
                        if (!this.isNomenclature(control.value)) {
                            return { 'selectedvaluefromdropdown': true };
                        }
                    }
                    else {
                        if (!this.activeOptions.some(x => { return x === control.value })) {
                            return { 'selectedvaluefromdropdown': true };
                        }
                    }
                }
                else if (this.hasGroupedOptionsCollection && this.groupedOptions) {
                    if (this.isNomenclatures(this.groupedOptions[0].options)) {
                        if (!this.isNomenclature(control.value)) {
                            return { 'selectedvaluefromdropdown': true };
                        }
                    }
                    else {
                        if (!this.groupedOptions.some(x => { return (x.options as string[])?.some(y => { return y === control.value }) })) {
                            return { 'selectedvaluefromdropdown': true };
                        }
                    }
                }
            }
            return null;
        }
    }

    private noOptionsValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (this.hasSelectedValueFromDropdownValidator) {
                if (this.hasOptionsCollection && (!this.activeOptions || this.activeOptions.length === 0)) {
                    return { 'novaluesindropdown': true };
                }
                else if (this.hasGroupedOptionsCollection && (!this.groupedOptions || this.groupedOptions.length === 0)) {
                    return { 'novaluesindropdown': true };
                }
            }
            return null;
        };
    }

    private isNomenclature<T>(obj: NomenclatureDTO<T> | string): obj is NomenclatureDTO<T> {
        if (obj !== null && obj !== undefined && typeof obj === 'object') {
            return 'value' in obj && 'displayName' in obj;
        }
        return false;
    }

    private isNomenclatures<T>(obj: NomenclatureDTO<T>[] | string[]): obj is NomenclatureDTO<T>[] {
        if (obj !== null && obj !== undefined && Array.isArray(obj)) {
            if (typeof obj[0] === 'object') {
                return 'value' in obj[0] && 'displayName' in obj[0];
            }
        }
        return false;
    }

    private inputFocusout(): void {
        setTimeout(() => {
            this.autoCompleteInputRef.nativeElement.blur();
        });
    }

    private overrideSetValidators(): void {
        if (this.ngControl && this.ngControl.control) {
            const self = this;

            const setValidators = this.ngControl.control.setValidators;
            this.ngControl.control.setValidators = function (newValidator: ValidatorFn | ValidatorFn[] | null) {
                setValidators.apply(this, [newValidator]);
                self.setHasSelectedValueFromDropdownValidator();
            };

            const clearValidators = this.ngControl.control.clearValidators;
            this.ngControl.control.clearValidators = function () {
                clearValidators.apply(this);
                self.setHasSelectedValueFromDropdownValidator();
            };
        }
    }
}