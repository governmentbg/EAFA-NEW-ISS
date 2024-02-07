import { Component, Input, Optional, Self } from '@angular/core';
import { NgControl } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { TLTranslatePipe } from '../../../pipes/tl-translate.pipe';
import { BaseTLControl } from '../base-tl-control';

@Component({
    selector: 'tl-select',
    templateUrl: './tl-select.component.html'
})
export class TLSelectComponent<T> extends BaseTLControl {

    constructor(@Self() @Optional() ngControl: NgControl, fuseTranslationService: FuseTranslationLoaderService, tlTranslatePipe: TLTranslatePipe) {
        super(ngControl, fuseTranslationService, tlTranslatePipe);
        this.ngControlInitialized.subscribe(() => {
            if (this._selectedValue !== undefined && this._selectedValue !== null && this.formControl !== undefined && this.formControl !== null) {
                this.formControl.setValue(this._selectedValue);
            }
        });
    }

    public _collection!: NomenclatureDTO<T>[] | string[];

    @Input() public set options(value: NomenclatureDTO<T>[] | string[]) {
        this._collection = value;
        if (value !== null && value !== undefined) {
            if (typeof value[0] === 'string') {
                this.hasStringOptions = true;
            }
            else {
                this.hasStringOptions = false;
            }
        }
    }

    public hasStringOptions: boolean = false;

    private _selectedValue: NomenclatureDTO<T> | NomenclatureDTO<T>[] | string | string[] | undefined;
    @Input() public set selectedValue(value: NomenclatureDTO<T> | NomenclatureDTO<T>[] | string | string[]) {
        this._selectedValue = value;
        if (this._selectedValue !== undefined && this._selectedValue !== null && this.formControl !== undefined && this.formControl !== null) {
            this.formControl.setValue(this._selectedValue);
        }
    }

    @Input() public isMultiple: boolean = false;
}