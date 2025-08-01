import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductTableComponent } from '../../../Components/product-table/product-table.component';
import { ModelTableComponent } from '../../../Components/model-table/model-table.component';
import { ColorTableComponent } from '../../../Components/color-table/color-table.component';
import { CategoryTableComponent } from '../../../Components/category-table/category-table.component';

@Component({
  selector: 'app-products',
  imports: [CommonModule, ProductTableComponent, ModelTableComponent, ColorTableComponent, CategoryTableComponent],
  templateUrl: './products.html',
  styleUrl: './products.css'
})
export class Products {
  tabs = [
    { key: 'products' as const, label: 'Products' },
    { key: 'models' as const, label: 'Models' },
    { key: 'colors' as const, label: 'Colors' },
    { key: 'categories' as const, label: 'Categories' }
  ];
  activeTab = signal<'products'|'models'|'colors'|'categories'>('products');
}
