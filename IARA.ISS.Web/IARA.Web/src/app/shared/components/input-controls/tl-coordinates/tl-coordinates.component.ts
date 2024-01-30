import { FuseTranslationLoaderService } from "@/@fuse/services/translation-loader.service";
import { TLTranslatePipe } from "@app/shared/pipes/tl-translate.pipe";
import { Component, Optional, Self } from "@angular/core";
import { NgControl } from "@angular/forms";
import { BaseTLControl } from "../base-tl-control";

@Component({
    selector: 'tl-coordinates',
    templateUrl: './tl-coordinates.component.html'
})
export class TLCoordinatesComponents extends BaseTLControl {

    constructor(@Self() @Optional() ngControl: NgControl, tlTranslatePipe: TLTranslatePipe) {
        super(ngControl, tlTranslatePipe);
    }


}