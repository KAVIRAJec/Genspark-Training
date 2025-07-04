const express = require('express');
const mongoose = require('mongoose');
const cors = require('cors');

const app = express();
const PORT = 3000;
const MONGO_URL = process.env.MONGO_URL || 'mongodb://localhost:27017/mydb';

app.use(cors());
app.use(express.json());

mongoose.connect(MONGO_URL, { useNewUrlParser: true, useUnifiedTopology: true })
  .then(() => console.log('MongoDB connected'))
  .catch(err => console.error('MongoDB connection error:', err));

const ItemSchema = new mongoose.Schema({
  name: String
});
const Item = mongoose.model('Item', ItemSchema);

// Create
app.post('/api/items', async (req, res) => {
  const item = new Item({ name: req.body.name });
  await item.save();
  res.json(item);
});
// Read all
app.get('/api/items', async (req, res) => {
  const items = await Item.find();
  res.json(items);
});
// Update
app.put('/api/items/:id', async (req, res) => {
  const item = await Item.findByIdAndUpdate(req.params.id, { name: req.body.name }, { new: true });
  res.json(item);
});
// Delete
app.delete('/api/items/:id', async (req, res) => {
  await Item.findByIdAndDelete(req.params.id);
  res.json({ success: true });
});

app.listen(PORT, () => {
  console.log(`Backend running on http://localhost:${PORT}`);
});
