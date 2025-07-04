const express = require('express');
const cors = require('cors');
const bodyParser = require('body-parser');
const crypto = require('crypto');
const Razorpay = require('razorpay');
require('dotenv').config();

const app = express();
const PORT = process.env.PORT || 3000;

// Middleware
app.use(cors({
  origin: process.env.FRONTEND_URL || 'http://localhost:4200',
  credentials: true
}));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));

// Initialize Razorpay
const razorpay = new Razorpay({
  key_id: process.env.RAZORPAY_KEY_ID,
  key_secret: process.env.RAZORPAY_KEY_SECRET,
});

// Routes

// Health check endpoint
app.get('/api/health', (req, res) => {
  res.json({ 
    status: 'OK', 
    message: 'Razorpay Backend Server is running',
    timestamp: new Date().toISOString()
  });
});

// Get Razorpay key for frontend
app.get('/api/payment/key', (req, res) => {
  try {
    res.json({
      success: true,
      key: process.env.RAZORPAY_KEY_ID
    });
  } catch (error) {
    console.error('Error getting Razorpay key:', error);
    res.status(500).json({
      success: false,
      message: 'Failed to get payment key'
    });
  }
});

// Create Razorpay order
app.post('/api/payment/create-order', async (req, res) => {
  try {
    const { amount, currency = 'INR', receipt } = req.body;

    // Validate request
    if (!amount || amount <= 0) {
      return res.status(400).json({
        success: false,
        message: 'Valid amount is required'
      });
    }

    // Create order options
    const options = {
      amount: amount * 100, // Convert to paise
      currency: currency,
      receipt: receipt || `receipt_${Date.now()}`,
      payment_capture: 1 // Auto capture payment
    };

    // Create order with Razorpay
    const order = await razorpay.orders.create(options);

    console.log('Order created:', order);

    res.json({
      success: true,
      order: {
        id: order.id,
        amount: order.amount,
        currency: order.currency,
        receipt: order.receipt,
        status: order.status
      }
    });

  } catch (error) {
    console.error('Error creating order:', error);
    res.status(500).json({
      success: false,
      message: 'Failed to create order',
      error: process.env.NODE_ENV === 'development' ? error.message : undefined
    });
  }
});

// Verify payment signature
app.post('/api/payment/verify', (req, res) => {
  try {
    const { 
      razorpay_order_id, 
      razorpay_payment_id, 
      razorpay_signature 
    } = req.body;

    // Validate request
    if (!razorpay_order_id || !razorpay_payment_id || !razorpay_signature) {
      return res.status(400).json({
        success: false,
        message: 'Missing required payment verification data'
      });
    }

    // Create signature verification string
    const body = razorpay_order_id + '|' + razorpay_payment_id;
    
    // Generate expected signature
    const expectedSignature = crypto
      .createHmac('sha256', process.env.RAZORPAY_KEY_SECRET)
      .update(body.toString())
      .digest('hex');

    // Verify signature
    const isAuthentic = expectedSignature === razorpay_signature;

    if (isAuthentic) {
      console.log('Payment verified successfully:', {
        order_id: razorpay_order_id,
        payment_id: razorpay_payment_id
      });

      res.json({
        success: true,
        message: 'Payment verified successfully',
        payment: {
          order_id: razorpay_order_id,
          payment_id: razorpay_payment_id,
          verified: true,
          timestamp: new Date().toISOString()
        }
      });
    } else {
      console.error('Payment verification failed:', {
        order_id: razorpay_order_id,
        payment_id: razorpay_payment_id,
        expected: expectedSignature,
        received: razorpay_signature
      });

      res.status(400).json({
        success: false,
        message: 'Payment verification failed'
      });
    }

  } catch (error) {
    console.error('Error verifying payment:', error);
    res.status(500).json({
      success: false,
      message: 'Failed to verify payment',
      error: process.env.NODE_ENV === 'development' ? error.message : undefined
    });
  }
});

// Get payment details
app.get('/api/payment/:paymentId', async (req, res) => {
  try {
    const { paymentId } = req.params;

    if (!paymentId) {
      return res.status(400).json({
        success: false,
        message: 'Payment ID is required'
      });
    }

    // Fetch payment details from Razorpay
    const payment = await razorpay.payments.fetch(paymentId);

    res.json({
      success: true,
      payment: {
        id: payment.id,
        amount: payment.amount,
        currency: payment.currency,
        status: payment.status,
        order_id: payment.order_id,
        method: payment.method,
        created_at: payment.created_at,
        email: payment.email,
        contact: payment.contact
      }
    });

  } catch (error) {
    console.error('Error fetching payment details:', error);
    res.status(500).json({
      success: false,
      message: 'Failed to fetch payment details',
      error: process.env.NODE_ENV === 'development' ? error.message : undefined
    });
  }
});

// Handle payment webhook (for production use)
app.post('/api/payment/webhook', (req, res) => {
  try {
    const webhookSignature = req.headers['x-razorpay-signature'];
    const webhookBody = JSON.stringify(req.body);

    // Verify webhook signature
    const expectedSignature = crypto
      .createHmac('sha256', process.env.RAZORPAY_WEBHOOK_SECRET || '')
      .update(webhookBody)
      .digest('hex');

    if (webhookSignature === expectedSignature) {
      console.log('Webhook verified:', req.body);
      
      // Handle different webhook events
      const event = req.body.event;
      const payment = req.body.payload.payment.entity;

      switch (event) {
        case 'payment.captured':
          console.log('Payment captured:', payment.id);
          // Handle successful payment
          break;
        case 'payment.failed':
          console.log('Payment failed:', payment.id);
          // Handle failed payment
          break;
        default:
          console.log('Unhandled webhook event:', event);
      }

      res.json({ status: 'ok' });
    } else {
      console.error('Invalid webhook signature');
      res.status(400).json({ error: 'Invalid signature' });
    }

  } catch (error) {
    console.error('Webhook error:', error);
    res.status(500).json({ error: 'Webhook processing failed' });
  }
});

// Error handling middleware
app.use((error, req, res, next) => {
  console.error('Unhandled error:', error);
  res.status(500).json({
    success: false,
    message: 'Internal server error',
    error: process.env.NODE_ENV === 'development' ? error.message : undefined
  });
});

// 404 handler
app.use('*', (req, res) => {
  res.status(404).json({
    success: false,
    message: 'Endpoint not found'
  });
});

// Start server
app.listen(PORT, () => {
  console.log(`🚀 Razorpay Backend Server running on http://localhost:${PORT}`);
  console.log(`📱 Frontend URL: ${process.env.FRONTEND_URL}`);
  console.log(`🔑 Razorpay Key ID: ${process.env.RAZORPAY_KEY_ID}`);
  console.log(`🌍 Environment: ${process.env.NODE_ENV}`);
});

module.exports = app;
