import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Recipes } from "./Components/recipes/recipes";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Recipes],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'TaskApp';
}
