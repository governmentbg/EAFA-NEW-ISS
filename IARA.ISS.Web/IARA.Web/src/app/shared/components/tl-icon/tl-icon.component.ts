import { TLIconTypes } from "@app/enums/icon-types.enum";
import { Component, Input } from "@angular/core";
import { AppIcons } from "./icons.enum";
import { MatBadgeModule, MatBadgePosition, MatBadgeSize } from '@angular/material/badge';
import { ThemePalette } from '@angular/material/core';

@Component({
    selector: 'tl-icon',
    templateUrl: './tl-icon.component.html',
})
export class TLIconComponent {
    public _type: TLIconTypes = TLIconTypes.MAT_ICON;
    public _size: number = 1.33;
    public _iconClass: string | undefined;
    public _icon: string = 'accessible';
    public _tooltipText: string | undefined;

    @Input() public set icon(value: string) {
        if (value.startsWith('ic-')) {
            const key = Object.keys(AppIcons.IC_ICONS).find(x => x === value) as string;
            this.iconObj = AppIcons.IcIconsDictionary.get(key);
            this.type = TLIconTypes.IC_ICON;
        } else if (value.startsWith('fa-')) {
            const key = Object.keys(AppIcons.FA_ICONS).find(x => x === value) as string;
            this.iconObj = AppIcons.FaIconsDictionary.get(key);
            this.type = TLIconTypes.FA_ICON;
        } else {
            this.type = TLIconTypes.MAT_ICON;
        }

        this._icon = value;
    }

    @Input('matBadgeSize')
    public badgeSize: MatBadgeSize = 'medium';

    @Input('matBadge')
    public matBadge: string | number | undefined | null;

    @Input('matBadgeDescription')
    public badgeDescription?: string;

    @Input('matBadgePosition')
    public badgePosition: MatBadgePosition = 'above after'

    @Input('matBadgeColor')
    public badgeColor: ThemePalette = 'primary';

    @Input('matBadgeHidden')
    public badgeHidden: boolean = false;

    @Input() public set size(value: number) {
        this._size = value;
    }

    @Input() public set type(value: TLIconTypes) {
        this._type = value;
    }

    @Input() public set iconClass(value: string) {
        this._iconClass = value;
    }

    @Input() public iconColor: string | undefined;

    @Input() public set tooltipText(value: string | undefined) {
        this._tooltipText = value;
    }

    public iconObj: any | undefined;
}