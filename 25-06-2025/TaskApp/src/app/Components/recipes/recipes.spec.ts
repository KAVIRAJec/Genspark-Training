import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Recipes } from './recipes';
import { RecipeService } from '../../Services/Recipe.Service';
import { of } from 'rxjs';
import { RecipeModel } from '../../Models/RecipeModel';

// Dummy data
const mockRecipes: RecipeModel[] = [
  new RecipeModel(1, 'Recipe 1', ['Egg', 'Flour'], 10, 'Italian', 'Easy'),
  new RecipeModel(2, 'Recipe 2', ['Rice', 'Chicken'], 20, 'Indian', 'Medium')
];

describe('Recipes', () => {
  let component: Recipes;
  let fixture: ComponentFixture<Recipes>;
  let recipeServiceSpy: jasmine.SpyObj<RecipeService>;

  beforeEach(async () => {
    recipeServiceSpy = jasmine.createSpyObj('RecipeService', ['getAllRecipes']);
    recipeServiceSpy.getAllRecipes.and.returnValue(of(mockRecipes));

    await TestBed.configureTestingModule({
      imports: [Recipes],
      providers: [
        { provide: RecipeService, useValue: recipeServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(Recipes);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should set recipes signal with data from service', () => {
    expect(component.recipes()).toEqual(mockRecipes);
    expect(recipeServiceSpy.getAllRecipes).toHaveBeenCalled();
  });
});
