import { Component, Input, Optional, Self } from '@angular/core';
import { NgControl } from '@angular/forms';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { TLTranslatePipe } from '@app/shared/pipes/tl-translate.pipe';
import { BaseTLControl } from '../base-tl-control';

@Component({
    selector: 'tl-radio-button-group',
    templateUrl: './tl-radio-button-group.component.html'
})
export class TLRadioButtonGroupComponent<T> extends BaseTLControl {
    @Input()
    public options!: NomenclatureDTO<T>[];

    @Input()
    public color: string = 'accent';

    @Input()
    public labelPosition: 'below' | 'after' = 'after';

    @Input()
    public direction: 'column' | 'row' = 'column';

    @Input()
    public gap: string | undefined;

    @Input()
    public readonly: boolean = false;

    constructor(@Self() @Optional() ngControl: NgControl,
        tlTranslatePipe: TLTranslatePipe) {
        super(ngControl, tlTranslatePipe);
    }
}