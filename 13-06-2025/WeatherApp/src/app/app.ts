import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CitySearchComponent } from "./city-search-component/city-search-component";
import { WeatherCardComponent } from "./weather-card-component/weather-card-component";
import { WeatherDashboardComponent } from "./weather-dashboard-component/weather-dashboard-component";

@Component({
  selector: 'app-root',
  imports: [CitySearchComponent, WeatherCardComponent, WeatherDashboardComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'WeatherApp';
}
