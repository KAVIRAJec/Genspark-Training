import { TestBed } from '@angular/core/testing';
import { Component } from '@angular/core';
import { App } from './app';
import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';

// Dummy mock Recipes component
@Component({
  selector: 'app-recipes',
  standalone: true,
  template: '<div>Mock Recipes Component</div>'
})
class MockRecipes {}

describe('App', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        App,
        MockRecipes
      ],
      providers: []
    })
    // Override the component's imports to use the mock
    .overrideComponent(App, {
      set: {
        imports: [MockRecipes]
      }
    })
    .compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should render the mock recipes component', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Mock Recipes Component');
  });

  it('should have title property as TaskApp', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect((app as any).title).toBe('TaskApp');
  });
});
