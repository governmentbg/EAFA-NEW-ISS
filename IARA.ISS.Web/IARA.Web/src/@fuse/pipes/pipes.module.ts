import { NgModule } from '@angular/core';

import { KeysPipe } from './keys.pipe';
import { GetByIdPipe } from './getById.pipe';
import { HtmlToPlaintextPipe } from './htmlToPlaintext.pipe';

@NgModule({
    declarations: [
        KeysPipe,
        GetByIdPipe,
        HtmlToPlaintextPipe
    ],
    imports: [],
    exports: [
        KeysPipe,
        GetByIdPipe,
        HtmlToPlaintextPipe
    ]
})
export class FusePipesModule {
}