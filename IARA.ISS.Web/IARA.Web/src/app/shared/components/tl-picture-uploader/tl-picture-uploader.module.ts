import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { BrowserModule } from '@angular/platform-browser';
import { TLPictureUploaderComponent } from './tl-picture-uploader.component';

@NgModule({
    imports: [
        BrowserModule,
        CommonModule,
        FlexLayoutModule,
        ReactiveFormsModule,
        MatFormFieldModule
    ],
    exports: [
        TLPictureUploaderComponent
    ],
    declarations: [
        TLPictureUploaderComponent
    ]
})
export class TLPictureUploaderModule {

}