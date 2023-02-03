import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { RecreationalFishingPublicService } from '@app/services/public-app/recreational-fishing-public.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationRegister } from '@app/interfaces/common-app/application-register.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { ApplicationTypeDTO } from '@app/models/generated/dtos/ApplicationTypeDTO';
import { ApplicationsPublicService } from '@app/services/public-app/applications-public.service';
import { RecreationalFishingAssociationPublicService } from '@app/services/public-app/recreational-fishing-association-public.service';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { EditLegalAssociationComponent } from '@app/components/common-app/legal-associations/edit-legal-association/edit-legal-association.component';
import { AddApplicationResultDTO } from '@app/models/generated/dtos/AddApplicationResultDTO';

@Component({
    selector: 'association-picker',
    templateUrl: './association-picker.component.html',
    styleUrls: ['./association-picker.component.scss']
})
export class AssociationPickerComponent implements OnInit {
    @Output()
    public associationChosen = new EventEmitter<NomenclatureDTO<number>>();

    public associations!: NomenclatureDTO<number>[];
    public hasStartedApplication: boolean = false;

    private associationApplicationTypeId!: number;
    private readonly service: RecreationalFishingPublicService;
    private readonly associationsService: RecreationalFishingAssociationPublicService;
    private readonly applicationsService: ApplicationsPublicService;
    private readonly editAssociationDialog: TLMatDialog<EditLegalAssociationComponent>;
    private readonly translate: FuseTranslationLoaderService;
    private readonly router: Router;

    public constructor(
        service: RecreationalFishingPublicService,
        associationsService: RecreationalFishingAssociationPublicService,
        applicationsService: ApplicationsPublicService,
        editAssociationDialog: TLMatDialog<EditLegalAssociationComponent>,
        translate: FuseTranslationLoaderService,
        router: Router
    ) {
        this.service = service;
        this.associationsService = associationsService;
        this.applicationsService = applicationsService;
        this.editAssociationDialog = editAssociationDialog;
        this.translate = translate;
        this.router = router;
    }

    public async ngOnInit(): Promise<void> {
        this.service.currentUserAssociations.subscribe({
            next: (assocs: NomenclatureDTO<number>[]) => {
                this.associations = assocs;

                if (this.associations.length === 1) {
                    this.associationChosen.emit(this.associations[0]);
                }
            }
        });

        const result: [void, ApplicationTypeDTO[], boolean] = await Promise.all([
            this.service.getUserAssociations().toPromise(),
            this.applicationsService.getApplicationTypesForChoice().toPromise(),
            this.service.hasUserStartedAssociationApplication().toPromise()
        ]);

        this.associationApplicationTypeId = result[1].find(x => x.pageCode === PageCodeEnum.Assocs)!.value!;
        this.hasStartedApplication = result[2];
    }

    public chooseAssociation(association: NomenclatureDTO<number>): void {
        this.associationChosen.emit(association);
    }

    public submitApplication(): void {
        this.applicationsService.addApplication(this.associationApplicationTypeId).subscribe({
            next: (data: AddApplicationResultDTO) => {
                this.openAssociationApplicationDialog(data.applicationId!);
            }
        });
    }

    public navigateToApplication(): void {
        this.router.navigate(['/submitted-applications']);
    }

    private openAssociationApplicationDialog(applicationId: number): void {
        const editDialogData: DialogParamsModel = new DialogParamsModel({
            applicationId: applicationId,
            isApplication: true,
            isApplicationHistoryMode: false,
            isReadonly: false,
            viewMode: false,
            service: this.associationsService,
            applicationsService: this.applicationsService,
            showOnlyRegiXData: false,
            showRegiXData: false,
            pageCode: PageCodeEnum.Assocs,
            isThirdCountry: false
        });

        const dialog = this.editAssociationDialog.open({
            title: this.translate.getValue('legal-association.edit-association-application-dialog-title'),
            TCtor: EditLegalAssociationComponent,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction) => {
                    closeFn();
                }
            },
            componentData: editDialogData,
            translteService: this.translate,
            disableDialogClose: true,
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: 'common.cancel',
            },
            rightSideActionsCollection: [{
                id: 'save-draft-content',
                color: 'primary',
                translateValue: 'applications-register.save-draft-content',
                buttonData: { callbackFn: this.saveApplicationDraftContentActionClicked.bind(this) }
            }, {
                id: 'save-and-download-for-sign',
                color: 'accent',
                translateValue: 'applications-register.save-application-and-download-for-sign',
                buttonData: { callbackFn: this.completeApplicationFillingByApplicantActionClicked.bind(this) }
            }]
        }, '1600px');

        dialog.subscribe(() => {
            this.hasStartedApplication = true;
            this.navigateToApplication();
        });
    }

    private saveApplicationDraftContentActionClicked(applicationId: number, model: IApplicationRegister, closeFn: HeaderCloseFunction): void {
        this.applicationsService.saveDraftContent(applicationId, model).subscribe({
            next: () => {
                closeFn();
            }
        });
    }

    private completeApplicationFillingByApplicantActionClicked(applicationId: number): void {
        this.applicationsService.completeApplicationFillingByApplicant(applicationId).subscribe({
            next: () => {
                // nothing to do
            }
        });
    }
}