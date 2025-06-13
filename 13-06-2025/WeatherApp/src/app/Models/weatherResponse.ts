export class WeatherResponse {
    constructor(
        public temperature: number,
        public weatherCondition: string,
        public pressure: number,
        public humidity: number,
        public seaLevel: number,
        public windSpeed: number,
        public coordinates: { lat: number, lon: number },
    ) {}
}