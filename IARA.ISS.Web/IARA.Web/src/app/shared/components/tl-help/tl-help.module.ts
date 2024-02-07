import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { TLPopoverModule } from '../tl-popover/tl-popover.module';
import { TLHelpComponent } from './tl-help.component';

@NgModule({
    imports: [
        BrowserModule,
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