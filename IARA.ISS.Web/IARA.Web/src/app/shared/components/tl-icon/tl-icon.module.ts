import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { MatIconModule } from "@angular/material/icon";
import { BrowserModule } from "@angular/platform-browser";
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";
import { IconModule, IconService } from "@visurel/iconify-angular";
import { TLIconComponent } from "./tl-icon.component";
import { AppIcons } from './icons.enum'
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatBadgeModule } from '@angular/material/badge';

@NgModule({
    imports: [
        MatIconModule,
        FontAwesomeModule,
        BrowserModule,
        CommonModule,
        IconModule,
        MatTooltipModule,
        MatBadgeModule
    ],
    exports: [
        TLIconComponent
    ],
    declarations: [
        TLIconComponent
    ]
})
export class TLIconModule {
    constructor(iconService: IconService) {
        iconService.registerAll(AppIcons.IC_ICONS);
    }
}