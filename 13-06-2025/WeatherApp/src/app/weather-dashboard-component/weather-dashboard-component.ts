import { Component } from '@angular/core';
import { WeatherService } from '../services/Weather.Service';
import { WeatherResponse } from '../Models/weatherResponse';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-weather-dashboard-component',
  imports: [MatIconModule],
  templateUrl: './weather-dashboard-component.html',
  styleUrl: './weather-dashboard-component.css'
})
export class WeatherDashboardComponent {
  protected weatherData : WeatherResponse | null = null;
  protected city : string = '';
  protected errorMessage: string | null = null;
  constructor(private weatherService: WeatherService) { }

  ngOnInit() {
    this.weatherService.city$.subscribe({
      next: city => {
        this.city = city;
        this.errorMessage = null;
      },
      error: error => {
        this.errorMessage = error.message;
      }
    });
    this.weatherService.weather$.subscribe({
      next: data => {
        this.weatherData = data;
        this.errorMessage = null;
      },
      error: error => {
        this.errorMessage = error.message;
      }
    });
  }
}
