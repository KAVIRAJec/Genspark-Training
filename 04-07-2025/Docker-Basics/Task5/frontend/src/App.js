import React, { useEffect, useState } from 'react';

const API = '/api/items';

function App() {
  const [items, setItems] = useState([]);
  const [name, setName] = useState('');
  const [editId, setEditId] = useState(null);
  const [editName, setEditName] = useState('');
  const [error, setError] = useState('');

  const fetchItems = () => {
    fetch(API)
      .then(res => res.json())
      .then(setItems)
      .catch(() => setError('Failed to fetch items'));
  };

  useEffect(() => { fetchItems(); }, []);

  const addItem = e => {
    e.preventDefault();
    fetch(API, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ name })
    })
      .then(res => res.json())
      .then(() => { setName(''); fetchItems(); })
      .catch(() => setError('Failed to add item'));
  };

  const startEdit = (id, name) => {
    setEditId(id);
    setEditName(name);
  };

  const saveEdit = e => {
    e.preventDefault();
    fetch(`${API}/${editId}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ name: editName })
    })
      .then(res => res.json())
      .then(() => { setEditId(null); setEditName(''); fetchItems(); })
      .catch(() => setError('Failed to update item'));
  };

  const deleteItem = id => {
    fetch(`${API}/${id}`, { method: 'DELETE' })
      .then(() => fetchItems())
      .catch(() => setError('Failed to delete item'));
  };

  return (
    <div style={{ maxWidth: 500, margin: '40px auto', fontFamily: 'sans-serif' }}>
      <h1>Task5 CRUD Items</h1>
      {error && <p style={{color:'red'}}>{error}</p>}
      <form onSubmit={addItem} style={{ marginBottom: 20 }}>
        <input value={name} onChange={e => setName(e.target.value)} placeholder="New item name" required />
        <button type="submit">Add</button>
      </form>
      <ul>
        {items.map(item => (
          <li key={item._id} style={{ marginBottom: 8 }}>
            {editId === item._id ? (
              <form onSubmit={saveEdit} style={{ display: 'inline' }}>
                <input value={editName} onChange={e => setEditName(e.target.value)} required />
                <button type="submit">Save</button>
                <button type="button" onClick={() => setEditId(null)}>Cancel</button>
              </form>
            ) : (
              <>
                {item.name}
                <button onClick={() => startEdit(item._id, item.name)} style={{ marginLeft: 8 }}>Edit</button>
                <button onClick={() => deleteItem(item._id)} style={{ marginLeft: 4 }}>Delete</button>
              </>
            )}
          </li>
        ))}
      </ul>
    </div>
  );
}

export default App;
