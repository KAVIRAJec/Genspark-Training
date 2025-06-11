import { Component, Input } from '@angular/core';
import { RecipeModel } from '../Models/Recipe';

@Component({
  selector: 'app-recipe',
  imports: [],
  templateUrl: './recipe.html',
  styleUrl: './recipe.css'
})
export class Recipe {
@Input() recipe : RecipeModel | null = new RecipeModel();
}
