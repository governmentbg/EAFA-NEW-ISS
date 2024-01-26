import {
    AfterViewInit,
    Component,
    ComponentFactoryResolver,
    ComponentRef,
    ElementRef,
    Inject,
    ReflectiveInjector,
    Type,
    ViewChild,
    ViewContainerRef
} from '@angular/core';
import { MatDialogContent, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { ITranslationService } from '@app/shared/interfaces/translate-service.interface';
import { CommonUtils } from '../../utils/common.utils';
import { IActionInfo } from './interfaces/action-info.interface';
import { IDialogComponent } from './interfaces/dialog-content.interface';
import { IDialogData } from './interfaces/dialog-data.interface';
import { IHeaderAuditButton } from './interfaces/header-audit-button.interface';
import { IHeaderCancelButton } from './interfaces/header-cancel-button.interface';
import { DialogWrapperData } from './models/dialog-action-buttons.model';

@Component({
    selector: 'dialog-wrapper',
    templateUrl: './dialog-wrapper.component.html',
    styleUrls: ['./dialog-wrapper.component.scss']
})
export class DialogWrapperComponent<T extends IDialogComponent> implements AfterViewInit {
    public title: string = '';
    public headerTooltip: string = 'Close dialog';
    public headerCloseButtonTooltip: string = '';
    public headerAuditButtonTooltip: string = '';
    public showHeaderCloseBtn: boolean = true;
    public showHeaderAuditBtn: boolean = true;
    public headerCancelButton: IHeaderCancelButton | undefined;
    public leftSideActionsCollection: Array<IActionInfo> = new Array<IActionInfo>();
    public rightSideActionsCollection: Array<IActionInfo> = new Array<IActionInfo>();
    public saveBtn?: IActionInfo;
    public auditInfo: SimpleAuditDTO | undefined;
    public cancelBtn?: IActionInfo;
    public closeBtn?: IActionInfo;
    public headerAuditBtn: IHeaderAuditButton | undefined;
    public viewMode: boolean = false;
    public defaultFullscreen: boolean = false;

    @ViewChild('placeHolder', { read: ViewContainerRef })
    private _placeHolder: ViewContainerRef | undefined;

    @ViewChild(MatDialogContent, { read: ElementRef })
    private dialogContent!: ElementRef;

    private containerElement!: HTMLElement;
    private overlayElement!: HTMLElement;
    private overlayWidth!: string;
    private overlayHeight!: string;
    private overlayTop!: string;
    private overlayLeft!: string;
    public isOverlayFullScreen: boolean = false;

    private translate!: ITranslationService;
    private componentCtor: (new (...args: any[]) => T) | undefined;
    private componentData: any | undefined;
    private componentInstance!: T;
    private router!: Router;

    constructor(private componentFactoryResolver: ComponentFactoryResolver,
        public dialogRef: MatDialogRef<DialogWrapperComponent<T>>,
        @Inject(MAT_DIALOG_DATA)
        data: IDialogData<T>,
        router: Router) {

        if (data != undefined) {
            this.title = data.title;
            this.translate = data.translteService;
            this.headerCloseButtonTooltip = this.translate.getValue('common.close-dialog');
            this.headerAuditButtonTooltip = this.translate.getValue('common.simple-audit-info');
            this.componentData = data.componentData;
            this.componentCtor = data.TCtor;
            this.saveBtn = data.saveBtn;
            this.cancelBtn = data.cancelBtn;
            this.defaultFullscreen = data.defaultFullscreen ?? false;
            this.router = router;

            this.viewMode = data.viewMode ?? false;
            this.closeBtn = data.closeBtn ?? {
                id: 'close',
                color: 'primary',
                translateValue: 'common.close'
            };

            if (data.disableDialogClose !== undefined) {
                dialogRef.disableClose = data.disableDialogClose;
            } else {
                dialogRef.disableClose = true;
            }

            if (data.leftSideActionsCollection !== undefined) {
                this.leftSideActionsCollection = data.leftSideActionsCollection;
            }

            if (data.rightSideActionsCollection !== undefined) {
                this.rightSideActionsCollection = data.rightSideActionsCollection;
            }

            if (!this.isNullOrUndefined(data.headerCancelButton)) {
                this.showHeaderCloseBtn = true;
                this.headerCancelButton = data.headerCancelButton;
                if (!this.isNullOrUndefined(data?.headerCancelButton?.tooltip)) {
                    this.headerCloseButtonTooltip = data?.headerCancelButton?.tooltip as string;
                }
            } else {
                this.showHeaderCloseBtn = false;
            }

            if (!this.isNullOrUndefined(data.headerAuditButton)) {
                this.headerAuditBtn = data.headerAuditButton;
                this.showHeaderAuditBtn = true;
                if (!this.isNullOrUndefined(data?.headerAuditButton?.tooltip)) {
                    this.headerAuditButtonTooltip = data?.headerAuditButton?.tooltip as string;
                }
            } else {
                this.showHeaderAuditBtn = false;
            }
        }
    }

    public ngAfterViewInit(): void {
        const type = (this.componentCtor as Type<T>);
        const component = this.createComponent((this._placeHolder as unknown as ViewContainerRef), type);
        this.componentInstance = component.instance;
        // all inputs/outputs set? add it to the DOM ..
        setTimeout(() => {
            this.placeholder.insert(component.hostView);
        });

        this.containerElement = this.getContainerElement();
        this.overlayElement = this.getOverlayElement();

        if (this.defaultFullscreen === true) {
            this.toggleFullScreen(undefined);
        }
    }

    public closeDialog(actionInfo: IActionInfo | undefined): void {
        if (actionInfo === undefined) {
            this.dialogRef.close();
        } else {
            this.componentInstance.dialogButtonClicked(actionInfo, this.dialogRef.close.bind(this.dialogRef));
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo): void {
        this.componentInstance.cancelBtnClicked(actionInfo, this.dialogRef.close.bind(this.dialogRef));
    }

    public saveBtnClicked(actionInfo: IActionInfo): void {
        this.componentInstance.saveBtnClicked(actionInfo, this.dialogRef.close.bind(this.dialogRef));
    }

    public closeBtnClicked(actionInfo: IActionInfo): void {
        if (this.componentInstance.closeBtnClicked) {
            this.componentInstance.closeBtnClicked(actionInfo, this.dialogRef.close.bind(this.dialogRef));
        }
        else {
            this.dialogRef.close();
        }
    }

    public headerCloseDialog(actionInfo: IHeaderCancelButton | undefined): void {
        if (actionInfo === undefined) {
            this.dialogRef.close(false);
        } else {
            actionInfo.cancelBtnClicked(this.dialogRef.close.bind(this.dialogRef));
        }
    }

    public headerAuditBtnClicked(): void {
        if (this.headerAuditBtn !== undefined) {
            this.headerAuditBtn.getAuditRecordData(this.headerAuditBtn.id).subscribe(result => {
                this.auditInfo = result;
            });
        }
    }

    public headerDetailedAuditBtnClicked(): void {
        const systemLogId: number | undefined = this.headerAuditBtn?.id;
        const systemLogTableName: string | undefined = this.headerAuditBtn?.tableName;

        if (systemLogId !== undefined &&
            !CommonUtils.isNumberNullOrNaN(systemLogId) &&
            !CommonUtils.isNullOrUndefined(systemLogTableName)) {
            this.router.navigateByUrl('/system-log', { state: { tableId: systemLogId!.toString(), tableName: systemLogTableName } });
        }

        this.dialogRef.close();
    }

    public get placeholder(): ViewContainerRef {
        return (this._placeHolder as unknown as ViewContainerRef);
    }

    private getContentElement(): HTMLElement {
        return this.dialogContent.nativeElement;
    }

    private getContainerElement(): HTMLElement {
        const content: HTMLElement = this.getContentElement();
        return content.closest('mat-dialog-container') as HTMLElement;
    }

    private getOverlayElement(): HTMLElement {
        const overlay: HTMLElement = this.containerElement.parentElement!;
        return overlay;
    }

    private createComponent(viewContainerRef: ViewContainerRef, type: Type<T>): ComponentRef<T> {
        const factory = this.componentFactoryResolver.resolveComponentFactory<T>(type);

        // vCref is needed cause of that injector..
        const injector = ReflectiveInjector.fromResolvedProviders([], viewContainerRef.parentInjector);

        // create component without adding it directly to the DOM
        const component = factory.create(injector) as ComponentRef<T>;
        const dialogActionButtons = new DialogWrapperData({
            leftSideActions: this.leftSideActionsCollection,
            rightSideActions: this.rightSideActionsCollection,
            dialogRef: this.dialogRef
        });
        component.instance.setData(this.componentData, dialogActionButtons);

        return component;
    }

    private isNullOrUndefined(value: any): boolean {
        if (value === null || value === undefined) {
            return true;
        }
        else {
            return false;
        }
    }

    // Must be an arrow function so as to bind "this" in addEventListener
    private toggleFullScreen = (event: MouseEvent | undefined): void => {
        if (this.isOverlayFullScreen) {
            this.isOverlayFullScreen = false;

            this.collapseFullScreen();
        }
        else {
            this.isOverlayFullScreen = true;

            this.expandFullScreen();
        }
    }

    public expandFullScreen(): void {
        this.overlayElement.style.top = '0';
        this.overlayElement.style.left = '0';
        this.overlayElement.style.maxWidth = '100%';
        this.overlayElement.style.maxHeight = '100%';
        this.overlayElement.style.width = '100%';
        this.overlayElement.style.height = '100%';
    }

    public collapseFullScreen(): void {
        this.overlayElement.style.width = this.overlayWidth;
        this.overlayElement.style.height = this.overlayHeight;
        this.overlayElement.style.top = this.overlayTop;
        this.overlayElement.style.left = this.overlayLeft;
    }
}