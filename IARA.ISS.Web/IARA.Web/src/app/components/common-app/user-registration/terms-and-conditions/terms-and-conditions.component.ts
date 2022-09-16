import { Component, ViewEncapsulation } from "@angular/core";
import { fuseAnimations } from "@fuse/animations";

@Component({
    selector: 'terms-and-conditions',
    templateUrl: './terms-and-conditions.component.html',
    styleUrls: ['./terms-and-conditions.component.scss'],
    encapsulation: ViewEncapsulation.None,
    animations: fuseAnimations
})
export class TermsAndConditionsComponent { }