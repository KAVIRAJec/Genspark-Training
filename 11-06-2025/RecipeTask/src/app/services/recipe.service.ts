import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { RecipeModel } from "../Models/Recipe";

@Injectable()
export class RecipeService {
  private http = inject(HttpClient);

  getRecipe(id: number = 1) {
    return this.http.get<RecipeModel>('https://dummyjson.com/recipes/' + id);
  }

  getAllRecipes(): Observable<RecipeModel[]> {
    return this.http.get<RecipeModel[]>('https://dummyjson.com/recipes');
  }
}