import { Component, ContentChild, TemplateRef } from '@angular/core';

@Component({
    selector: 'row-detail',
    templateUrl: './row-detail.component.html'
})
export class TLRowDetailComponent {
    @ContentChild(TemplateRef)
    public template!: TemplateRef<any>;

    public constructor() { }
}