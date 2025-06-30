import { Component, Input } from '@angular/core';
import { RecipeModel } from '../../Models/RecipeModel';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-recipe',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './recipe.html',
  styleUrl: './recipe.css'
})
export class Recipe {
  @Input() recipe!: RecipeModel;

  HandleClick(Data: any) {
    console.log('Recipe clicked:', Data);
  }
}
