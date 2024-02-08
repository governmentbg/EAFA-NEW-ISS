import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TLPipesModule } from '../../pipes/tl-pipes.module';
import { TLIconButtonModule } from '../tl-icon-button/tl-icon-button.module';
import { TLFileUploadComponent } from './file-upload.component';

@NgModule({
    imports: [
        MatSnackBarModule,
        CommonModule,
        FormsModule,
        FlexLayoutModule,
        ReactiveFormsModule,
        MatFormFieldModule,
        MatTooltipModule,
        MatIconModule,
        MatProgressBarModule,
        MatInputModule,
        TLPipesModule,
        TLIconButtonModule
    ],
    exports: [
        TLFileUploadComponent
    ],
    declarations: [
        TLFileUploadComponent
    ]
})
export class TLFileUploadModule {
}