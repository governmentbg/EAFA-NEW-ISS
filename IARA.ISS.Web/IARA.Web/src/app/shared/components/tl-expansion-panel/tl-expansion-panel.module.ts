import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatExpansionModule } from '@angular/material/expansion';
import { BrowserModule } from '@angular/platform-browser';
import { TLHelpModule } from '../tl-help/tl-help.module';
import { TLExpansionPanelDescriptionComponent } from './components/tl-expansion-panel-description.component';

import { TLExpansionPanelComponent } from './tl-expansion-panel.component';

@NgModule({
    imports: [
        BrowserModule,
        CommonModule,
        MatExpansionModule,
        FlexLayoutModule,
        TLHelpModule
    ],
    exports: [
        TLExpansionPanelComponent,
        TLExpansionPanelDescriptionComponent
    ],
    declarations: [
        TLExpansionPanelComponent,
        TLExpansionPanelDescriptionComponent
    ]
})
export class TLExpansionPanelModule {
}