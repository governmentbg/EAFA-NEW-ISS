import { AfterViewInit, Component, ContentChild, forwardRef, Input, TemplateRef } from '@angular/core';
import { ITranslationService } from '@app/shared/interfaces/translate-service.interface';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { BaseDataColumn } from '../base-data-column';

@Component({
    selector: 'data-template-column',
    templateUrl: './data-column-template.component.html',
    providers: [{ provide: BaseDataColumn, useExisting: forwardRef(() => TLDataColumnTemplateComponent) }]
})
export class TLDataColumnTemplateComponent extends BaseDataColumn implements AfterViewInit {
    @Input()
    public cellClass: string = '';

    @Input()
    public headerClass: string = 'multi-line';

    @ContentChild(TemplateRef)
    public ngContentTest: any;

    public cellTemplate!: TemplateRef<any>;
    public headerTemplate!: TemplateRef<any>;
    public tlTranslate: ITranslationService;

    constructor(tlTranslateService: FuseTranslationLoaderService) {
        super();
        this.tlTranslate = tlTranslateService;
    }

    public ngAfterViewInit(): void {
        setTimeout(() => {
            this.cellTemplate = this.ngContentTest;
        });
    }

}