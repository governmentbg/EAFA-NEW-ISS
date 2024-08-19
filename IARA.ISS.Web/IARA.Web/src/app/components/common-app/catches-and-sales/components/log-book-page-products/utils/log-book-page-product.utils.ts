import { CurrencyPipe } from '@angular/common';

export class LogBookPageProductUtils {
    public static formatTotalProductPrice(pipe: CurrencyPipe, quantity: number | undefined, unitPrice: number | undefined): string | null {
        const totalPrice: number | undefined = this.calculateTotalPrice(quantity, unitPrice);
        const formattedTotalPrice: string | null = pipe.transform(totalPrice?.toString(), 'BGN', 'symbol', '0.2-2', 'bg-BG');

        return formattedTotalPrice;
    }

    public static calculateTotalPrice(quantity: number | undefined, unitPrice: number | undefined): number | undefined {
        if (unitPrice) {
            if (quantity) {
                return quantity * unitPrice;
            }
        }

        return undefined;
    }
}
