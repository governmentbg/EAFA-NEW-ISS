import { NgModule } from '@angular/core';
import { CustomDomSharedStylesHost } from './shared_styles_host';
import { ɵDomSharedStylesHost } from '@angular/platform-browser';
import { MediaMatcher } from '@angular/cdk/layout';
import { CustomMediaMatcher } from './media-matcher';


@NgModule({
    providers: [
        { provide: 'cspMetaSelector', useValue: 'meta[name="CSP-NONCE"]' },
        { provide: ɵDomSharedStylesHost, useClass: CustomDomSharedStylesHost },
        { provide: MediaMatcher, useClass: CustomMediaMatcher }
    ],
})
export class InlineStylesCSPModule { }