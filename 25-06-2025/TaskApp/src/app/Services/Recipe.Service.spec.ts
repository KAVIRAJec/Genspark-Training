import { TestBed } from '@angular/core/testing';
import { RecipeService } from './Recipe.Service';
import { HttpClient } from '@angular/common/http';
import { of } from 'rxjs';
import { RecipeModel } from '../Models/RecipeModel';

describe('RecipeService', () => {
  let service: RecipeService;
  let httpClientSpy: jasmine.SpyObj<HttpClient>;

  const mockRecipe = {
    id: 1,
    name: 'Test Recipe',
    ingredients: ['Egg', 'Flour'],
    prepTimeMinutes: 30,
    cuisine: 'Test Cuisine',
    difficulty: 'Easy'
  };

  const mockRecipeModel = new RecipeModel(
    mockRecipe.id,
    mockRecipe.name,
    mockRecipe.ingredients,
    mockRecipe.prepTimeMinutes,
    mockRecipe.cuisine,
    mockRecipe.difficulty
  );

  beforeEach(() => {
    httpClientSpy = jasmine.createSpyObj('HttpClient', ['get']);
    TestBed.configureTestingModule({
      providers: [
        RecipeService,
        { provide: HttpClient, useValue: httpClientSpy }
      ]
    });
    service = TestBed.inject(RecipeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should map getRecipe response to RecipeModel', (done) => {
    httpClientSpy.get.and.returnValue(of(mockRecipe));
    service.getRecipe(1).subscribe(recipe => {
      expect(recipe).toEqual(jasmine.objectContaining(mockRecipeModel));
      done();
    });
  });

  it('should map getAllRecipes response to array of RecipeModel', (done) => {
    httpClientSpy.get.and.returnValue(of({ recipes: [mockRecipe] }));
    service.getAllRecipes().subscribe(recipes => {
      expect(recipes.length).toBe(1);
      expect(recipes[0]).toEqual(jasmine.objectContaining(mockRecipeModel));
      done();
    });
  });
});
