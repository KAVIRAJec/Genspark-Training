import { Component } from '@angular/core';
import { WeatherResponse } from '../Models/weatherResponse';
import { WeatherService } from '../services/Weather.Service';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-weather-card-component',
  imports: [MatIcon],
  templateUrl: './weather-card-component.html',
  styleUrl: './weather-card-component.css'
})
export class WeatherCardComponent {
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
