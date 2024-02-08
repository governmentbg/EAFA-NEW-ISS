import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { TLErrorComponent } from './tl-error.component';

@NgModule({
    imports: [
        BrowserModule,
        CommonModule
    ],
    exports: [
        TLErrorComponent
    ],
    declarations: [
        TLErrorComponent
    ]
})
export class TLErrorModule {

}