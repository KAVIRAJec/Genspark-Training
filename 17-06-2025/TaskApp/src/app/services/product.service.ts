import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";

@Injectable({
        providedIn: 'root'
})
export class ProductService {
    private http = inject(HttpClient);

    carts: any[] = [];
    addToCart(product: any) {
        const existingProduct = this.carts.find(item => item.id === product.id);
        if (existingProduct) {
            existingProduct.quantity += 1;
        } else {
            product.quantity = 1;
            this.carts.push(product);
        }
        return this.carts;
    }
    removeFromCart(product: any) {
        const index = this.carts.findIndex(item => item.id === product.id);
        if (index !== -1) {
            this.carts[index].quantity -= 1;
            if (this.carts[index].quantity <= 0) {
                this.carts.splice(index, 1);
            }
        } else {
            console.log('Product not found in cart:', product);
        }
        return this.carts;
    }

    getProducts(searchData: string = '', limit: number = 10, skip: number = 0) {
        return this.http.get('https://dummyjson.com/products/search?q='+ searchData + '&limit=' + limit + '&skip=' + skip);
    }

    getProductById(id: string | number) {
        return this.http.get('https://dummyjson.com/products/' + id);
    }

    getAboutInfo() {
        return {
            appName: 'Modern E-Commerce App',
            version: '1.0.0',
            team: ['Alice', 'Bob', 'Charlie'],
            description: 'A modern Angular e-commerce demo with infinite scroll, search, and cart.'
        };
    }

    getCart() {
        return this.carts;
    }
}