import { Component } from '@angular/core';
import { BasePageComponent } from '@app/components/common-app/base-page.component';
import { TranslationResourceTypeEnum } from '@app/enums/translation-resource-type.enum';

@Component({
    selector: 'translation-help',
    templateUrl: './translation-help.component.html'
})
export class TranslationHelpComponent extends BasePageComponent {
    public readonly resourceTypes: typeof TranslationResourceTypeEnum = TranslationResourceTypeEnum;
}