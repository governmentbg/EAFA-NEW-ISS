import { CurrencyPipe } from '@angular/common';

export class LogBookPageProductUtils {

    public static formatTotalProductPrice(pipe: CurrencyPipe, quantityKg: number | undefined, unitPrice: number | undefined): string | null {
        const totalPrice: number | undefined = this.calculateTotalPrice(quantityKg, unitPrice);
        const formattedTotalPrice: string | null = pipe.transform(totalPrice?.toString(), 'BGN', 'symbol', '0.2-2', 'bg-BG');

        return formattedTotalPrice;
    }

    private static calculateTotalPrice(quantityKg: number | undefined, unitPrice: number | undefined): number | undefined {
        if (quantityKg !== null && quantityKg !== undefined && unitPrice !== null && unitPrice !== undefined) {
            return quantityKg * unitPrice;
        }
        else {
            return undefined;
        }
    }
}
