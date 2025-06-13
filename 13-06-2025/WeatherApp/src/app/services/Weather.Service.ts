import { HttpClient } from "@angular/common/http";
import { environment } from "../../environments/environment";
import { BehaviorSubject, catchError, interval, Observable, startWith, switchMap, map, tap } from "rxjs";
import { Injectable } from "@angular/core";
import { WeatherResponse } from '../Models/weatherResponse';

@Injectable({
    providedIn: 'root'
})
export class WeatherService {
    private apiKey: string = environment.weatherApiKey;
    private apiUrl: string = 'https://api.openweathermap.org/data/2.5/weather';
    private defaultCity: string = 'London';

    constructor(private http: HttpClient) {}

    private citySubject = new BehaviorSubject<string>(this.defaultCity);
    city$ = this.citySubject.asObservable();

    weather$: Observable<any> = this.city$.pipe(
        switchMap(city => 
            interval(60000).pipe( // Refresh every minute
                startWith(0),
                switchMap(() => this.getWeatherByCity(city))
            )
        )
    );

    setCity(city: string) {
        this.citySubject.next(city);
    }

    getWeatherByCity(city: string): Observable<WeatherResponse> {
        if (!city) throw new Error("City name is required");
        if (!this.apiKey) throw new Error("API key is not set");
        const url = `${this.apiUrl}?q=${encodeURIComponent(city)}&appid=${this.apiKey}&units=metric`;
        // console.log("response: ", this.http.get(url));
        return this.http.get<any>(url).pipe(
            map(data => new WeatherResponse(
                data.main.temp,
                data.weather[0].main,
                data.main.pressure,
                data.main.humidity,
                data.main.sea_level ?? 0,
                data.wind.speed,
                { lat: data.coord.lat, lon: data.coord.lon }
            )),
        catchError((error) => {
            if (error.status === 404) {
                throw new Error("City not found");
            }
            throw new Error("Failed to fetch weather data");
        })
    );
    }
}