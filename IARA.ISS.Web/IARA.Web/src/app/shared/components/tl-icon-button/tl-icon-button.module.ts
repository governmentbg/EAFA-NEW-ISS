import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatCardModule } from "@angular/material/card";
import { MatTooltipModule } from "@angular/material/tooltip";
import { BrowserModule } from "@angular/platform-browser";
import { TLIconModule } from "../tl-icon/tl-icon.module";
import { TLIconButtonComponent } from "./tl-icon-button.component";

@NgModule({
    imports: [
        MatButtonModule,
        MatTooltipModule,
        MatCardModule,
        BrowserModule,
        CommonModule,
        TLIconModule
    ],
    exports: [
        TLIconButtonComponent
    ],
    declarations: [
        TLIconButtonComponent
    ]
})
export class TLIconButtonModule {
}