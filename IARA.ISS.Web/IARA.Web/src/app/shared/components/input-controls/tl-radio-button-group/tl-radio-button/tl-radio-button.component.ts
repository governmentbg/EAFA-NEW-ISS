import { AfterContentInit, AfterViewInit, Component, ContentChild, ElementRef, Input, OnInit, Optional, Self, TemplateRef, ViewChild } from "@angular/core";
import { ControlContainer, NgControl } from "@angular/forms";
import { pairwise, startWith } from "rxjs/operators";
import { NomenclatureDTO } from "@app/models/generated/dtos/GenericNomenclatureDTO";


@Component({
    selector: 'tl-radio-button',
    templateUrl: './tl-radio-button.component.html'
})
export class TLRadioButtonComponent<T> {
    @Input()
    public option!: NomenclatureDTO<T>;

    @Input()
    public labelPosition: 'below' | 'after' = 'after';

}