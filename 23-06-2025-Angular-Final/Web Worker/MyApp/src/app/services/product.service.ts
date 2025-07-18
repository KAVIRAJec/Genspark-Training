import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";

@Injectable()
export class ProductService{
    private http = inject(HttpClient);

    getProducts(id:number = 1) {
        return this.http.get('https://dummyjson.com/products/'+id)
    }
    getAllProducts(): Observable<any[]> {
        return this.http.get<any[]>('https://dummyjson.com/products');
    }
    getProductSearchResult(searchData:string, limit:number = 10, skip:number = 0)
    {
        return this.http.get(`https://dummyjson.com/products/search?q=${searchData}&limit=${limit}&skip=${skip}`);
    }
}