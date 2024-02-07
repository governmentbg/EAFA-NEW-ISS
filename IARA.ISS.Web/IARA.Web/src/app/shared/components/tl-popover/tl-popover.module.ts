import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { BrowserModule } from '@angular/platform-browser';
import { MdePopoverModule } from '@material-extended/mde';
import { TLIconButtonModule } from '../tl-icon-button/tl-icon-button.module';
import { TLPopoverComponent } from './tl-popover.component';

@NgModule({
    imports: [
        FlexLayoutModule,
        MdePopoverModule,
        MatIconModule,
        MatTooltipModule,
        MatCardModule,
        TLIconButtonModule,
        BrowserModule,
        CommonModule
    ],
    exports: [
        TLPopoverComponent
    ],
    declarations: [
        TLPopoverComponent
    ]
})
export class TLPopoverModule {
}