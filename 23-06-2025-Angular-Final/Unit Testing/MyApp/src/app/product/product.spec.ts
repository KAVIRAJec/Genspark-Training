import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Product } from './product';
import { ProductService } from '../services/product.service';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { Component } from '@angular/core';
import { ProductModel } from '../Models/product';
import { provideHttpClient } from '@angular/common/http';
import { CurrencyPipe } from '@angular/common';

class mockProductService {
  getProduct(id: number) {
    return {subscribe:() => {}};
  }
}

const mockActiveRoute = {
  snapshot: {
    paramMap: {
      get:(key:string) => {
        if(key=='id') return 1;
        return null;
      }
    }
  }
}

@Component({
  standalone: true,
  imports: [Product],
  template: `<app-product [Product]="product" (addToCart)="onAdd($event)"></app-product>`
})
class HostComponent {
  product = new ProductModel();
  addedProduct: ProductModel | null = null;
  onAdd(product: ProductModel) {
    this.addedProduct = product;
  }
}

describe('Product', () => {
  let component: Product;
  let fixture: ComponentFixture<HostComponent>;
  let hostComponent: HostComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HostComponent],
      providers: [{ provide: ProductService, useClass: mockProductService },
        { provide: 'ActivatedRoute', useValue: mockActiveRoute },
        provideHttpClient(), provideHttpClientTesting(),
        CurrencyPipe
      ]}).compileComponents();

    fixture = TestBed.createComponent(HostComponent);
    hostComponent = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('check render product object input',()=>{
    hostComponent.product = {
      id:1,
      title:'Abc',
      price:90,
      description:'blah blah',
    } as ProductModel;
     fixture.detectChanges();
     const compiled = fixture.nativeElement as HTMLElement;
     expect(compiled.textContent).toContain('Abc');
    })
  });
