import { AfterContentInit, AfterViewInit, Component, ContentChild, ElementRef, Input, OnInit, Optional, Self, TemplateRef, ViewChild } from "@angular/core";
import { ControlContainer, NgControl } from "@angular/forms";
import { pairwise, startWith } from "rxjs/operators";
import { FuseTranslationLoaderService } from "@fuse/services/translation-loader.service";
import { NomenclatureDTO } from "@app/models/generated/dtos/GenericNomenclatureDTO";
import { TLTranslatePipe } from "../../../pipes/tl-translate.pipe";
import { BaseTLControl } from "../base-tl-control";

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
        fuseTranslationService: FuseTranslationLoaderService,
        tlTranslatePipe: TLTranslatePipe
    ) {
        super(ngControl, fuseTranslationService, tlTranslatePipe);
    }
}