import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { TLHelpModule } from '../../tl-help/tl-help.module';
import { TLErrorModule } from '../components/tl-error/tl-error.module';
import { RangeInputComponent } from './range-input.component';
import { TLRangeInputComponent } from './tl-range-input.component';

@NgModule({
    imports: [
        CommonModule,
        ReactiveFormsModule,
        MatFormFieldModule,
        FlexLayoutModule,
        TLHelpModule,
        TLErrorModule
    ],
    exports: [
        TLRangeInputComponent
    ],
    declarations: [
        TLRangeInputComponent,
        RangeInputComponent
    ]
})
export class TLRangeInputModule {

}