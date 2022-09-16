import { Component } from '@angular/core';
import { BasePageComponent } from '@app/components/common-app/base-page.component';
import { TranslationResourceTypeEnum } from '@app/enums/translation-resource-type.enum';

@Component({
    selector: 'translation-labels',
    templateUrl: './translation-labels.component.html'
})
export class TranslationLabelsComponent extends BasePageComponent {
    public readonly resourceTypes: typeof TranslationResourceTypeEnum = TranslationResourceTypeEnum;
}