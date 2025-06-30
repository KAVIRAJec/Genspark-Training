import { Component, signal, OnInit } from '@angular/core';
import { RecipeService } from '../../Services/Recipe.Service';
import { RecipeModel } from '../../Models/RecipeModel';
import { CommonModule } from '@angular/common';
import { Recipe } from '../recipe/recipe';

@Component({
  selector: 'app-recipes',
  standalone: true,
  imports: [CommonModule, Recipe],
  templateUrl: './recipes.html',
  styleUrl: './recipes.css'
})
export class Recipes implements OnInit {
  recipes = signal<RecipeModel[]>([]);

  constructor(private recipeService: RecipeService) {}

  ngOnInit() {
    this.recipeService.getAllRecipes().subscribe(data => {
      this.recipes.set(data);
    });
  }
}
