import { Component, OnInit } from '@angular/core';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { ApplicationsPublicService } from '@app/services/public-app/applications-public.service';
import { RecreationalFishingPublicService } from '@app/services/public-app/recreational-fishing-public.service';
import { MessageService } from '@app/shared/services/message.service';
import { BasePageComponent } from '../../../../common-app/base-page.component';

@Component({
    selector: 'recreational-fishing-association-applications',
    templateUrl: './recreational-fishing-association-applications.component.html'
})
export class RecreationalFishingAssociationApplicationsComponent extends BasePageComponent implements OnInit {
    public service: RecreationalFishingPublicService;
    public applicationsService: ApplicationsPublicService;
    public showAssociationPicker: boolean = true;

    private translate: FuseTranslationLoaderService;

    public constructor(
        service: RecreationalFishingPublicService,
        applicationsService: ApplicationsPublicService,
        translate: FuseTranslationLoaderService,
        messageService: MessageService
    ) {
        super(messageService);

        this.service = service;
        this.applicationsService = applicationsService;
        this.translate = translate;
    }

    public ngOnInit(): void {
        if (this.service.currentUserChosenAssociation !== undefined) {
            this.showAssociationPicker = false;
            this.setTitle();
        }
    }

    public associationChosen(assoc: NomenclatureDTO<number>): void {
        this.service.currentUserChosenAssociation = assoc;
        this.showAssociationPicker = false;

        this.setTitle();
    }

    private setTitle(): void {
        setTimeout(() => {
            this.messageService.sendMessage(`${this.translate.getValue('navigation.association-issued-tickets')} — ${this.service.currentUserChosenAssociation!.displayName}`);
        });
    }
}
