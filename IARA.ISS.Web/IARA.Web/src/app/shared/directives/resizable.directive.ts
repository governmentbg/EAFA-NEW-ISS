import { Directive, ElementRef, Input, OnDestroy, OnInit } from '@angular/core';

type ResizableSides = 'top' | 'bottom' | 'left' | 'right';

@Directive({
    selector: '[tlResizable]'
})
export class TLResizableDirective implements OnInit, OnDestroy {
    @Input('tlResizable')
    public sides: string = '';

    private readonly minHeight: number;
    private readonly maxHeight: number;
    private readonly minWidth: number;
    private readonly maxWidth: number;

    private elementRef: ElementRef;
    private resizableSides: Map<ResizableSides, boolean> = new Map<ResizableSides, boolean>();
    private resizableHandles: Map<ResizableSides, HTMLElement> = new Map<ResizableSides, HTMLElement>();

    private get element(): HTMLElement {
        return this.elementRef.nativeElement as HTMLElement;
    }

    public constructor(elementRef: ElementRef) {
        this.elementRef = elementRef;

        // perhaps set max heights with height of parent element
        this.minHeight = 50;
        this.maxHeight = document.body.clientHeight * 0.8;
        this.minWidth = 50;
        this.maxWidth = document.body.clientWidth * 0.8;
    }

    public ngOnInit(): void {
        const split: string[] = this.sides.split(' ');

        if (split.includes('top')) {
            this.resizableSides.set('top', true);
        }
        if (split.includes('bottom')) {
            this.resizableSides.set('bottom', true);
        }
        if (split.includes('left')) {
            this.resizableSides.set('left', true);
        }
        if (split.includes('right')) {
            this.resizableSides.set('right', true);
        }

        for (const [key, value] of this.resizableSides) {
            if (value === true) {
                this.createHandle(key);
            }
        }
    }

    private createHandle(side: ResizableSides) {
        const handle: HTMLDivElement = document.createElement('div');

        switch (side) {
            case 'top':
                handle.classList.add('tl-resizable-handle-top');
                handle.addEventListener('mousedown', this.startDraggingHorizontal);
                break;
            case 'bottom':
                handle.classList.add('tl-resizable-handle-bottom');
                handle.addEventListener('mousedown', this.startDraggingHorizontal);
                break;
            case 'right':
                handle.classList.add('tl-resizable-handle-right');
                handle.addEventListener('mousedown', this.startDraggingVertical);
                break;
            case 'left':
                handle.classList.add('tl-resizable-handle-left');
                handle.addEventListener('mousedown', this.startDraggingVertical);
                break;
        }

        const slits: HTMLDivElement[] = [
            document.createElement('div'),
            document.createElement('div'),
            document.createElement('div')
        ];

        for (const slit of slits) {
            slit.style.height = '1px';
            slit.style.width = '30px';
            slit.style.backgroundColor = '#000000';
            slit.style.margin = '3px auto';

            handle.appendChild(slit);
        }

        this.resizableHandles.set(side, handle);
        this.element.prepend(handle);
    }

    public ngOnDestroy(): void {
        for (const [key, value] of this.resizableSides) {
            if (value === true) {
                switch (key) {
                    case 'top':
                    case 'bottom':
                        this.resizableHandles.get(key)!.removeEventListener('mousedown', this.startDraggingHorizontal);
                        break;
                    case 'right':
                    case 'left':
                        this.resizableHandles.get(key)!.removeEventListener('mousedown', this.startDraggingVertical);
                        break;
                }
            }
        }
    }

    private startDraggingHorizontal = (mouseEvent: MouseEvent): void => {
        mouseEvent.preventDefault();
        const startingHeight: number = this.element.clientHeight;

        const mouseDragHandler = (pointerEvent: PointerEvent) => {
            pointerEvent.preventDefault();

            if (pointerEvent.buttons !== 1) {
                document.body.removeEventListener('pointermove', mouseDragHandler);
            }
            else {
                let newHeight: number = startingHeight + (mouseEvent.pageY - pointerEvent.pageY);
                if (newHeight > this.maxHeight) {
                    newHeight = this.maxHeight;
                }
                else if (newHeight < this.minHeight) {
                    newHeight = this.minHeight;
                }
                this.element.style.height = `${newHeight}px`;
            }
        };

        document.body.addEventListener('pointermove', mouseDragHandler);
    }

    private startDraggingVertical = (mouseEvent: MouseEvent): void => {
        mouseEvent.preventDefault();
        const startingWidth: number = this.element.clientWidth;

        const mouseDragHandler = (pointerEvent: PointerEvent) => {
            pointerEvent.preventDefault();

            if (pointerEvent.buttons !== 1) {
                document.body.removeEventListener('pointermove', mouseDragHandler);
            }
            else {
                let newWidth: number = startingWidth + (mouseEvent.pageX - pointerEvent.pageX);
                if (newWidth > this.maxWidth) {
                    newWidth = this.maxWidth;
                }
                else if (newWidth < this.minWidth) {
                    newWidth = this.minWidth;
                }
                this.element.style.width = `${newWidth}px`;
            }
        };

        document.body.addEventListener('pointermove', mouseDragHandler);
    }
}