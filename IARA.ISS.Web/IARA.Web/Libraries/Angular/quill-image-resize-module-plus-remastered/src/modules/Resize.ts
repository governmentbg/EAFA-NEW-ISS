import { BaseModule } from './BaseModule';

export class Resize extends BaseModule {
    onCreate = () => {
        // track resize handles
        this.boxes = [];
        this.imageMaxWidth = this.options.imageMaxStyles.width;
        this.imageMinWidth = this.options.imageMinStyles.width;
        // add 4 resize handles
        this.addBox('nwse-resize'); // top left
        this.addBox('nesw-resize'); // top right
        this.addBox('nwse-resize'); // bottom right
        this.addBox('nesw-resize'); // bottom left

        this.positionBoxes();
    };

    onDestroy = () => {
        // reset drag handle cursors
        this.setCursor('');
    };

    positionBoxes = () => {
        const handleXOffset = `${-parseFloat(this.options.handleStyles.width) / 2}px`;
        const handleYOffset = `${-parseFloat(this.options.handleStyles.height) / 2}px`;

        // set the top and left for each drag handle
        [
            { left: handleXOffset, top: handleYOffset },        // top left
            { right: handleXOffset, top: handleYOffset },       // top right
            { right: handleXOffset, bottom: handleYOffset },    // bottom right
            { left: handleXOffset, bottom: handleYOffset },     // bottom left
        ].forEach((pos, idx) => {
            Object.assign(this.boxes[idx].style, pos);
        });
    };

    addBox = (cursor: string): void => {
        // create div element for resize handle
        const box = document.createElement('div');

        // Star with the specified styles
        Object.assign(box.style, this.options.handleStyles);
        box.style.cursor = cursor;

        // Set the width/height to use 'px'
        box.style.width = `${this.options.handleStyles.width}px`;
        box.style.height = `${this.options.handleStyles.height}px`;

        // listen for mousedown on each box
        box.addEventListener('touchstart', this.handleMousedown as any, false);
        box.addEventListener('mousedown', this.handleMousedown as any, false);
        // add drag handle to document
        this.overlay.appendChild(box);
        // keep track of drag handle
        this.boxes.push(box);
    };

    handleMousedown = (evt: { target: any; touches: { clientX: any; }[]; clientX: any; }) => {
        // note which box
        this.dragBox = evt.target;
        // note starting mousedown position
        if (evt.touches) {
            // for mobile devices get clientX of first touch point
            this.dragStartX = evt.touches[0].clientX;
        } else {
            this.dragStartX = evt.clientX;
        }
        // if (evt.type === 'mousedown') {
        // 	this.dragStartX = evt.clientX
        // } else {
        // 	this.dragStartX = evt.touches[0].clientX
        // }

        // store the width before the drag
        this.preDragWidth = this.img.width || this.img.naturalWidth;
        // set the proper cursor everywhere
        this.setCursor(this.dragBox.style.cursor);
        // listen for movement and mouseup
        document.addEventListener('touchmove', this.handleDrag as any, false);
        document.addEventListener('touchend', this.handleMouseup, false);
        document.addEventListener('mousemove', this.handleDrag as any, false);
        document.addEventListener('mouseup', this.handleMouseup, false);
    };

    handleMouseup = () => {
        // reset cursor everywhere
        this.setCursor('');
        // stop listening for movement and mouseup
        document.removeEventListener('touchmove', this.handleDrag as any);
        document.removeEventListener('touchend', this.handleMouseup);
        document.removeEventListener('mousemove', this.handleDrag as any);
        document.removeEventListener('mouseup', this.handleMouseup);
    };

    handleDrag = (evt: { touches: { clientX: number; }[]; clientX: number; }) => {
        if (!this.img) {
            // image not set yet
            return;
        }
        // update image size
        let deltaX;
        if (evt.touches) {
            deltaX = evt.touches[0].clientX - this.dragStartX;
        } else {
            deltaX = evt.clientX - this.dragStartX;
        }
        // let deltaX
        // if (evt.type === 'mousemove') {
        //     deltaX = evt.clientX - this.dragStartX
        // } else {
        //     deltaX = evt.changedTouches[0].clientX - this.dragStartX
        // }

        if (this.dragBox === this.boxes[0] || this.dragBox === this.boxes[3]) {
            // left-side resize handler; dragging right shrinks image
            let width = Math.round(this.preDragWidth - deltaX);
            if (this.preDragWidth > this.imageMinWidth) {
                if (width >= this.imageMinWidth && width <= this.imageMaxWidth) {
                    this.img.width = width;
                }
            } else {
                if (width >= this.preDragWidth && width <= this.imageMaxWidth) {
                    this.img.width = width;
                }
            }

        } else {
            // right-side resize handler; dragging right enlarges image
            let width = Math.round(this.preDragWidth + deltaX);
            if (this.preDragWidth > this.imageMinWidth) {
                if (width >= this.imageMinWidth && width <= this.imageMaxWidth) {
                    this.img.width = width;
                }
            } else {
                if (width >= this.preDragWidth && width <= this.imageMaxWidth) {
                    this.img.width = width;
                }
            }
        }
        this.requestUpdate();
    };

    setCursor = (value: string) => {
        [
            document.body,
            this.img,
        ].forEach((el) => {
            el.style.cursor = value;   // eslint-disable-line no-param-reassign
        });
    };
    boxes: any;
    dragBox: any;
    dragStartX: any;
    preDragWidth: any;
    imageMinWidth: any;
    imageMaxWidth!: number;
}
