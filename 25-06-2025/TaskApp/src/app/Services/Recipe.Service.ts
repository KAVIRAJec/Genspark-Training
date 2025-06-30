import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { RecipeModel } from "../Models/RecipeModel";
import { Observable, map } from "rxjs";

@Injectable({ providedIn: 'root' })
export class RecipeService {
    private http = inject(HttpClient);

    getRecipe(id: number = 1): Observable<RecipeModel> {
        return this.http.get<any>('https://dummyjson.com/recipes/' + id).pipe(
            map(data => new RecipeModel(
                data.id,
                data.name,
                data.ingredients,
                data.prepTimeMinutes,
                data.cuisine,
                data.difficulty
            ))
        );
    }

    getAllRecipes(): Observable<RecipeModel[]> {
        return this.http.get<any>('https://dummyjson.com/recipes').pipe(
            map(response => (response.recipes || []).map((data: any) =>
                new RecipeModel(
                    data.id,
                    data.name,
                    data.ingredients,
                    data.prepTimeMinutes,
                    data.cuisine,
                    data.difficulty
                )
            ))
        );
    }
}