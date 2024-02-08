import { FuseTranslationLoaderService } from '@/@fuse/services/translation-loader.service';
import { Component, ContentChild, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { FormControl } from '@angular/forms';
import { TLCardTitleComponent } from './components/tl-card-title/tl-card-title.component';

@Component({
    selector: 'tl-card',
    templateUrl: './tl-card.component.html',
    styleUrls: ['./tl-card.component.scss']
})
export class TLCardComponent implements OnInit, OnChanges {
    @Input()
    public cardFlex: string = '100';

    @Input()
    public titleSize: number = 1.3;

    @Input()
    public title: string = '';

    @Input()
    public contentLayout: 'row' | 'column' = 'column';

    @Input()
    public contentLayoutGapPx: string = '5px';

    @Input()
    public contentLayoutAlign: string = '';

    @Input()
    public avatar!: unknown;

    @Input()
    public avatarSize: { width: string, height: string } = { width: '50px', height: '50px' };

    @Input()
    public set tooltipResourceName(value: string) {
        this._tooltipResourceName = value;
        if (this._tooltipResourceName !== null && this._tooltipResourceName !== undefined && this._tooltipResourceName.length > 0) {
            this.hasTitleContent = true;
        }
        else {
            this.hasTitleContent = false;
        }
    };

    @Input()
    public hasError: boolean = false;

    public _tooltipResourceName: string = '';
    public hasTitleContent: boolean = false;
    public hasHelpResource: boolean = false;

    public pictureUploaderControl: FormControl;

    @ContentChild(TLCardTitleComponent)
    private set tlCardTitleComponent(value: TLCardTitleComponent) {
        if (value !== null && value !== undefined) {
            this.hasHelpResource = true;
        }
        else {
            this.hasHelpResource = false;
        }
    }


    public constructor() {
        this.pictureUploaderControl = new FormControl();
    }

    public ngOnInit(): void {
        this.pictureUploaderControl.disable();
    }

    public ngOnChanges(changes: SimpleChanges): void {
        if (changes['avatar'] !== null && changes['avatar'] !== undefined && changes['avatar'].previousValue !== changes['avatar'].currentValue) {
            this.pictureUploaderControl.setValue(changes['avatar'].currentValue);
        }
    }
}