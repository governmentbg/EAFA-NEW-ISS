import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MaterialModule } from '../../material.module';
import { TLPipesModule } from '../../pipes/tl-pipes.module';
import { TLInputControlsModule } from '../input-controls/tl-input-controls.module';
import { TLIconModule } from '../tl-icon/tl-icon.module';
import { TLPopoverModule } from '../tl-popover/tl-popover.module';
import { TLAuditComponent } from './tl-audit.component';

@NgModule({
    imports: [
        FormsModule,
        FlexLayoutModule,
        MatTooltipModule,
        MatCardModule,
        TLInputControlsModule,
        CommonModule,
        TLPopoverModule,
        ReactiveFormsModule,
        MaterialModule,
        TLIconModule,
        TLPipesModule
    ],
    exports: [
        TLAuditComponent
    ],
    declarations: [
        TLAuditComponent
    ]
})
export class TLAuditModule {
}