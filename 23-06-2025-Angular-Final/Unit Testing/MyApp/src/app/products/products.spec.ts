import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Products } from './products';
import { ProductService } from '../services/product.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ProductModel } from '../Models/product';
import { of } from 'rxjs';

class MockProductService {
  getProductSearchResult(query: string, limit?: number, skip?: number) {
    return of({ products: [{ id: 1, title: 'Test Product', price: 100, description: 'desc', thumbnail: '' }], total: 1 });
  }
}

describe('Products', () => {
  let component: Products;
  let fixture: ComponentFixture<Products>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Products, HttpClientTestingModule],
      providers: [
        { provide: ProductService, useClass: MockProductService }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Products);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load products from service', (done) => {
    component.searchSubject.next('');
    setTimeout(() => {
      fixture.detectChanges();
      expect(component.products.length).toBeGreaterThan(0);
      expect(component.products[0].title).toBe('Test Product');
      done();
    }, 3500);
  });
});
