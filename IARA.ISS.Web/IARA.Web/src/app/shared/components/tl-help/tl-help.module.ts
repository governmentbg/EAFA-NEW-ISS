import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TLPopoverModule } from '../tl-popover/tl-popover.module';
import { TLHelpComponent } from './tl-help.component';

@NgModule({
    imports: [
        CommonModule,
        TLPopoverModule,
    ],
    exports: [
        TLHelpComponent
    ],
    declarations: [
        TLHelpComponent
    ]
})
export class TLHelpModule {
}