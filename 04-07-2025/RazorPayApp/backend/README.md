# Razorpay Backend API

This is a Node.js backend server for handling Razorpay payments securely.

## Setup Instructions

### 1. Install Dependencies
```bash
cd backend
npm install
```

### 2. Configure Environment Variables
Update the `.env` file with your actual Razorpay credentials:

```env
RAZORPAY_KEY_ID=your_actual_key_id
RAZORPAY_KEY_SECRET=your_actual_key_secret
PORT=3000
NODE_ENV=development
FRONTEND_URL=http://localhost:4200
```

**Important**: Get your actual Razorpay credentials from:
- Dashboard: https://dashboard.razorpay.com/
- API Keys: https://dashboard.razorpay.com/app/settings/api-keys

### 3. Start the Server

#### Development Mode (with auto-restart)
```bash
npm run dev
```

#### Production Mode
```bash
npm start
```

Server will run on: http://localhost:3000

## API Endpoints

### 1. Health Check
- **GET** `/api/health`
- **Purpose**: Check if server is running
- **Response**: Server status and timestamp

### 2. Get Payment Key
- **GET** `/api/payment/key`
- **Purpose**: Get Razorpay key ID for frontend
- **Response**: `{ success: true, key: "rzp_test_..." }`

### 3. Create Order
- **POST** `/api/payment/create-order`
- **Purpose**: Create a new payment order
- **Body**: 
  ```json
  {
    "amount": 100,
    "currency": "INR",
    "receipt": "receipt_123"
  }
  ```
- **Response**: Order details with order ID

### 4. Verify Payment
- **POST** `/api/payment/verify`
- **Purpose**: Verify payment signature after successful payment
- **Body**:
  ```json
  {
    "razorpay_order_id": "order_xxx",
    "razorpay_payment_id": "pay_xxx",
    "razorpay_signature": "signature_xxx"
  }
  ```
- **Response**: Verification result

### 5. Get Payment Details
- **GET** `/api/payment/:paymentId`
- **Purpose**: Get details of a specific payment
- **Response**: Payment information

### 6. Webhook (Production)
- **POST** `/api/payment/webhook`
- **Purpose**: Handle Razorpay webhooks for real-time payment updates
- **Usage**: Configure in Razorpay Dashboard

## Security Features

1. **CORS Protection**: Only allows requests from configured frontend URL
2. **Signature Verification**: Verifies all payment signatures using HMAC-SHA256
3. **Environment Variables**: Sensitive data stored in environment variables
4. **Input Validation**: Validates all incoming requests
5. **Error Handling**: Proper error responses without exposing sensitive data

## Testing

### Using curl:

#### Create Order:
```bash
curl -X POST http://localhost:3000/api/payment/create-order \
  -H "Content-Type: application/json" \
  -d '{"amount": 100, "currency": "INR"}'
```

#### Get Key:
```bash
curl http://localhost:3000/api/payment/key
```

## Production Deployment

1. Set `NODE_ENV=production` in environment
2. Use actual Razorpay credentials (not test keys)
3. Configure proper CORS origins
4. Set up webhooks in Razorpay Dashboard
5. Use HTTPS in production
6. Consider using PM2 or similar for process management

## Frontend Integration

The Angular frontend will automatically connect to this backend when it's running. Make sure both servers are running:

1. Backend: `http://localhost:3000`
2. Frontend: `http://localhost:4200`

The frontend will use the real Razorpay payment flow when the backend is available.
