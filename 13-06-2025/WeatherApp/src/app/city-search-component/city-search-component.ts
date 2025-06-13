import { Component } from '@angular/core';
import { WeatherService } from '../services/Weather.Service';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-city-search-component',
  imports: [MatIconModule, MatInputModule, MatButtonModule, MatFormFieldModule, FormsModule],
  templateUrl: './city-search-component.html',
  styleUrl: './city-search-component.css'
})
export class CitySearchComponent {
  protected city: string = '';
  protected searching: boolean = false;
  protected searchHistory: string[] = [];

  constructor(private weatherService: WeatherService) {
    this.loadHistory();
  }

  onSearch() {
    if (!this.city.trim()) {
      console.error("City name cannot be empty");
      return;
    }
    this.searching = true;
    setTimeout(() => {
      this.weatherService.setCity(this.city);
      this.searching = false;
    }, 800);
  }

  addToHistory(city: string) {
    this.searchHistory = this.searchHistory.filter(c => c.toLowerCase() !== city.toLowerCase());
    this.searchHistory.unshift(city);
    if (this.searchHistory.length > 5) this.searchHistory = this.searchHistory.slice(0, 5);
    localStorage.setItem('weather_search_history', JSON.stringify(this.searchHistory));
  }
  
  loadHistory() {
    const data = localStorage.getItem('weather_search_history');
    this.searchHistory = data ? JSON.parse(data) : [];
  }

  selectHistory(city: string) {
    this.city = city;
    this.onSearch();
  }
}
