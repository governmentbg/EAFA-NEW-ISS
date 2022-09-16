import { Component, ContentChild, TemplateRef } from '@angular/core';

@Component({
    selector: 'group-header',
    template: ''
})
export class TLGroupHeaderComponent {
    @ContentChild(TemplateRef)
    public template!: TemplateRef<any>;
}