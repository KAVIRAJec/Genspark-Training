import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Recipe } from './recipe';
import { RecipeModel } from '../../Models/RecipeModel';
import { By } from '@angular/platform-browser';

// Dummy recipe data
const mockRecipe = new RecipeModel(
  1,
  'Test Recipe',
  ['Egg', 'Flour'],
  15,
  'French',
  'Easy'
);

describe('Recipe', () => {
  let component: Recipe;
  let fixture: ComponentFixture<Recipe>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Recipe]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Recipe);
    component = fixture.componentInstance;
    component.recipe = mockRecipe;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should receive recipe input and display data', () => {
    expect(component.recipe).toEqual(mockRecipe);
    // Optionally, check for rendered text if template is available
  });

  it('should call HandleClick with correct data', () => {
    spyOn(component, 'HandleClick');
    component.HandleClick(mockRecipe);
    expect(component.HandleClick).toHaveBeenCalledWith(mockRecipe);
  });
});
