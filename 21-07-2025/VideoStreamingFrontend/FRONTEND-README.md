# VideoHub Portal - Frontend

A modern, responsive Angular application for streaming training videos with Web3-inspired design.

## Features

- 🎥 **Video Streaming**: Secure video streaming with SAS-based access
- 📱 **Responsive Design**: Mobile-first, fully responsive interface
- 🚀 **Modern UI**: Web3-inspired glassmorphism design with Material Design
- 🎨 **Rich Components**: Video cards, custom player, upload interface
- 🔍 **Search & Filter**: Real-time search and sorting capabilities
- 📊 **Pagination**: Efficient handling of large video collections
- 🎯 **TypeScript**: Full type safety and IntelliSense support

## Tech Stack

- **Framework**: Angular 20
- **UI Library**: Angular Material + Custom Components
- **Icons**: Lucide Angular
- **Notifications**: ngx-toastr
- **Styling**: SCSS with CSS Grid/Flexbox
- **HTTP Client**: Angular HttpClient with interceptors
- **State Management**: RxJS Observables
- **Build Tool**: Angular CLI with Vite

## Prerequisites

- Node.js 18+ 
- npm 9+
- Angular CLI 20+

## Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd VideoStreamingFrontend
   ```

2. **Install dependencies**
   ```bash
   npm install --force
   ```

3. **Update environment configuration**
   Edit `src/environments/environment.ts`:
   ```typescript
   export const environment = {
     production: false,
     apiUrl: 'http://localhost:5136'  // Your .NET API URL
   };
   ```

## Development

1. **Start the development server**
   ```bash
   npm start
   ```

2. **Open browser**
   Navigate to `http://localhost:4200`

3. **API Integration**
   Ensure your .NET API is running on the configured URL

## Build

1. **Production build**
   ```bash
   npm run build
   ```

2. **Build output**
   Files will be generated in `dist/VideoStreamingFrontend/`

## Project Structure

```
src/
├── app/
│   ├── core/                    # Core services and models
│   │   ├── models/             # TypeScript interfaces and models
│   │   ├── services/           # Business logic services
│   │   └── interceptors/       # HTTP interceptors
│   ├── features/               # Feature modules
│   │   ├── video-list/         # Video listing component
│   │   ├── video-upload/       # Video upload component
│   │   └── video-player/       # Video player component
│   ├── shared/                 # Shared components
│   │   └── components/         # Reusable UI components
│   ├── app.config.ts          # App configuration
│   ├── app.routes.ts          # Routing configuration
│   └── app.component.*        # Root component
├── environments/              # Environment configurations
└── styles.css               # Global styles
```

## Key Components

### Video List Component
- Grid and list view modes
- Real-time search and filtering
- Pagination support
- Responsive card layout

### Video Upload Component
- Drag & drop file upload
- File validation and preview
- Progress tracking
- Form validation

### Video Player Component
- Custom video controls
- Fullscreen support
- Share and download functionality
- Video metadata display

### Shared Components
- **Navbar**: Navigation with brand and links
- **Loading Spinner**: Global loading indicator
- **Video Card**: Reusable video display component
- **Footer**: Application footer

## Styling

The application uses a modern Web3-inspired design with:
- **Glassmorphism effects**: Backdrop blur and transparency
- **Gradient backgrounds**: Beautiful color transitions
- **Smooth animations**: CSS transitions and transforms
- **Responsive grid**: CSS Grid and Flexbox layouts
- **Custom Material theme**: Azure blue color scheme

## API Integration

The frontend communicates with the .NET API through:
- **Video Service**: CRUD operations for videos
- **Loading Service**: Global loading state management
- **Notification Service**: Toast notifications
- **HTTP Interceptors**: Error handling and loading states

## Environment Configuration

### Development
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5136'
};
```

### Production
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://your-api-domain.com'
};
```

## Scripts

- `npm start` - Start development server
- `npm run build` - Build for production
- `npm test` - Run unit tests
- `npm run lint` - Run linting
- `npm run e2e` - Run end-to-end tests

## Browser Support

- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

## Performance Features

- **Lazy loading**: Route-based code splitting
- **OnPush change detection**: Optimized rendering
- **Virtual scrolling**: Efficient large lists
- **Image optimization**: Responsive images
- **Bundle optimization**: Tree shaking and minification

## Contributing

1. Follow Angular style guide
2. Use TypeScript strict mode
3. Write unit tests for components
4. Follow component-based architecture
5. Use reactive patterns with RxJS

## Troubleshooting

### Common Issues

1. **API Connection Failed**
   - Check if .NET API is running
   - Verify environment.ts configuration
   - Check CORS settings in API

2. **Build Errors**
   - Run `npm install --force`
   - Clear node_modules and reinstall
   - Check TypeScript version compatibility

3. **Styling Issues**
   - Ensure Angular Material theme is imported
   - Check global styles in styles.css
   - Verify component style encapsulation

## License

MIT License - see LICENSE file for details.
