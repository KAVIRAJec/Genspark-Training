# Razorpay Test Application

A modern Angular web application demonstrating UPI payment integration using Razorpay's Test API. This application provides a clean, responsive interface for testing payment flows in a safe test environment.

## 🚀 Features

- **Clean Payment Form**: Responsive form with proper validation
- **Razorpay Integration**: Complete UPI payment flow using test mode
- **Real-time Validation**: Form validation with user-friendly error messages
- **Test Environment**: Pre-configured with test UPI IDs for different scenarios
- **Modern UI**: Bootstrap-powered responsive design with custom styling
- **TypeScript**: Fully typed codebase for better development experience

## 📋 Prerequisites

Before running this application, make sure you have the following installed:

- **Node.js** (v18 or higher)
- **npm** (v8 or higher)
- **Angular CLI** (v20 or higher)

## 🛠️ Installation

1. **Clone the repository**:
   ```bash
   git clone <repository-url>
   cd RazorPayApp
   ```

2. **Install dependencies**:
   ```bash
   npm install
   ```

3. **Install Angular CLI globally** (if not already installed):
   ```bash
   npm install -g @angular/cli
   ```

## 🏃‍♂️ Running the Application

### Development Server

Start the development server:

```bash
npm start
# or
ng serve
```

The application will be available at `http://localhost:4200/`.

### Backend Server

Start the backend server:

```bash
cd backend
npm install
npm run dev
```

The backend server will run on `http://localhost:3000/`.

## 🐳 Docker Deployment

This application is containerized using Docker for easy deployment and consistency across environments.

### Docker Prerequisites

- Docker Engine (v20.10.0+)
- Docker Compose (v2.0.0+)

### Docker Commands

#### Using Docker Compose (Recommended)

The easiest way to run both frontend and backend services:

```bash
# Build and start all services
docker-compose up --build

# Run in detached mode
docker-compose up -d

# Stop all services
docker-compose down
```

The application will be available at `http://localhost:80/`.

#### Using Docker Directly

If you prefer to build and run containers manually:

##### Frontend (Angular App)

```bash
# Build the Docker image
docker build -t razorpay-frontend .

# Run the container
docker run -d -p 80:80 --name razorpay-frontend-app razorpay-frontend
```

##### Backend (Node.js)

```bash
# Build the backend Docker image
cd backend
docker build -t razorpay-backend .

# Run the backend container
docker run -d -p 3000:3000 --name razorpay-backend-service razorpay-backend
```

### Accessing the Dockerized Application

- Frontend: `http://localhost:80/`
- Backend API: `http://localhost:3000/api/`

### Stopping Docker Containers

```bash
# Stop the frontend container
docker stop razorpay-frontend-app
docker rm razorpay-frontend-app

# Stop the backend container
docker stop razorpay-backend-service
docker rm razorpay-backend-service
```

## 🔒 Production Deployment Considerations

When moving to production, consider the following adjustments:

1. **Environment Variables**:
   - Replace test API keys with production Razorpay keys
   - Set `NODE_ENV=production`

2. **SSL/TLS Configuration**:
   - Modify nginx.conf to include SSL certificate configuration
   - Consider using Let's Encrypt for free SSL certificates

3. **Security Enhancements**:
   - Enable Content Security Policy (CSP)
   - Add rate limiting to the backend API
   - Set up proper logging and monitoring

4. **Production Docker Compose**:
   Create a separate `docker-compose.prod.yml` file with:
   ```yaml
   version: '3.8'
   
   services:
     frontend:
       # ...existing config
       environment:
         - NODE_ENV=production
       volumes:
         - ./ssl:/etc/nginx/ssl:ro  # For SSL certificates
       # Add any other production-specific configs
   
     backend:
       # ...existing config
       environment:
         - NODE_ENV=production
         - RAZORPAY_KEY_ID=${RAZORPAY_KEY_ID}  # Use production key
         - RAZORPAY_KEY_SECRET=${RAZORPAY_KEY_SECRET}  # Use production secret
       restart: always  # Ensure always restarts in production
   ```

5. **Running in Production**:
   ```bash
   # Start production setup
   docker-compose -f docker-compose.prod.yml up -d
   ```

Navigate to `http://localhost:4200/` in your browser. The application will automatically reload when you make changes to the source files.

### Build for Production

Build the project for production:

```bash
npm run build
# or
ng build
```

The build artifacts will be stored in the `dist/` directory.

## 🧪 Testing Payment Flows

The application comes pre-configured with Razorpay test credentials. Use the following test data:

### Test Payment Scenarios

| Scenario | UPI ID | Expected Result |
|----------|--------|-----------------|
| **Successful Payment** | `success@razorpay` | Payment completes successfully |
| **Failed Payment** | `failure@razorpay` | Payment fails |
| **Pending Payment** | `pending@razorpay` | Payment goes to pending state |

### Sample Test Data

The form comes pre-filled with test data for easier testing:

- **Amount**: ₹100
- **Name**: Test User
- **Email**: test@example.com
- **Contact**: 9876543210

### How to Test

1. **Fill the payment form** with the required details
2. **Click "Pay with UPI"** to initiate the payment
3. **Choose UPI option** in the Razorpay modal
4. **Enter test UPI ID** (e.g., `success@razorpay`)
5. **Complete the payment** to see the result

## 🏗️ Project Structure

```
src/
├── app/
│   ├── payment/                 # Payment component
│   │   ├── payment.ts          # Payment logic
│   │   ├── payment.html        # Payment template
│   │   └── payment.css         # Payment styles
│   ├── services/               # Application services
│   │   ├── payment.service.ts  # Payment service
│   │   └── notification.service.ts # Notification service
│   ├── app.ts                  # Root component
│   ├── app.html               # App template
│   └── app.css                # App styles
├── types/                      # TypeScript declarations
│   └── razorpay.d.ts          # Razorpay type definitions
├── index.html                  # Main HTML file
└── styles.css                 # Global styles
```

## 🎨 Key Features Explained

### Form Validation

- **Amount**: Required, minimum ₹1, maximum ₹1,00,000
- **Name**: Required, 2-50 characters
- **Email**: Required, valid email format
- **Contact**: Required, valid 10-digit Indian mobile number

### Payment Integration

- Uses Razorpay Checkout v1 for payment processing
- Handles success, failure, and dismissal scenarios
- Displays real-time payment status
- Pre-fills test UPI IDs for easy testing

### Responsive Design

- Mobile-first approach
- Bootstrap 5 integration
- Custom CSS with modern gradients and animations
- Accessible design with proper ARIA labels

## 🔧 Configuration

### Razorpay Test Key

The application uses a demo test key. For production use, replace the test key in `payment.ts`:

```typescript
key: 'rzp_test_YOUR_ACTUAL_TEST_KEY_HERE'
```

### Backend Integration

Currently uses dummy order IDs. To integrate with a real backend:

1. Create an API endpoint for order creation
2. Update `PaymentService.createOrder()` method
3. Add proper error handling and validation

## 🧪 Testing (Unit Tests)

Run unit tests:

```bash
npm test
# or
ng test
```

This will execute the unit tests via [Karma](https://karma-runner.github.io).

## 📱 Browser Support

- **Chrome** (latest)
- **Firefox** (latest)
- **Safari** (latest)
- **Edge** (latest)

## 🚨 Important Notes

- **Test Mode Only**: This application is configured for test payments only
- **No Real Money**: All transactions are simulated
- **Security**: Never expose real API keys in frontend code
- **HTTPS**: Razorpay requires HTTPS in production

## 📚 API Documentation

- [Razorpay Checkout Documentation](https://razorpay.com/docs/checkout/)
- [Razorpay Test Cards](https://razorpay.com/docs/payments/test-card-upi-details/)
- [Angular Documentation](https://angular.dev/)

## 🛡️ Security Considerations

- All payment processing happens through Razorpay's secure servers
- No sensitive payment data is stored in the frontend
- Form validation prevents malicious input
- HTTPS is required for production deployment

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Support

If you encounter any issues or have questions:

1. Check the [Razorpay Documentation](https://razorpay.com/docs/)
2. Review the browser console for error messages
3. Ensure all dependencies are properly installed
4. Verify that the development server is running on the correct port

---

**Happy Testing! 🎉**
