import { Component, Input } from "@angular/core";
import { TLError } from "../../models/tl-error.model";

@Component({
    selector: 'tl-error',
    templateUrl: './tl-error.component.html'
})
export class TLErrorComponent {
    @Input()
    public errors: TLError[] = [];

    @Input()
    public useMultipleLines: boolean = false;
}