import { Component, signal } from '@angular/core';
import { RecipeModel } from '../Models/Recipe';
import { RecipeService } from '../services/recipe.service';
import { Recipe } from "../recipe/recipe";

@Component({
  selector: 'app-recipes',
  imports: [Recipe],
  templateUrl: './recipes.html',
  styleUrl: './recipes.css'
})
export class Recipes {
  recipes = signal<RecipeModel[] | undefined>(undefined);
  errorMessage = signal<string | null>(null);

  constructor(private recipeService: RecipeService) {}

  ngOnInit(): void {
    this.recipeService.getAllRecipes().subscribe(
      {
        next: (data: any) => {
          this.recipes.set(data.recipes as RecipeModel[]);
        },
        error: (error) => {
          console.error("Error loading recipes:", error);
          this.errorMessage.set(error.message || "An error occurred while fetching recipes.");
        },
        complete: () => {
          console.log("Recipes data fetch successfully");
        }
      }
    )
  }
}
