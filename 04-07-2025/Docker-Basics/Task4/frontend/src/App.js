import React, { useEffect, useState } from 'react';

function App() {
  const [greet, setGreet] = useState('');
  const [error, setError] = useState('');
  const apiUrl = process.env.REACT_APP_API_URL || '/api/greet';

  useEffect(() => {
    fetch(apiUrl)
      .then(res => {
        if (!res.ok) throw new Error('API error');
        return res.json();
      })
      .then(data => setGreet(data.message))
      .catch(err => setError('Could not fetch greeting: ' + err.message));
  }, [apiUrl]);

  return (
    <div style={{ padding: 40, fontFamily: 'sans-serif' }}>
      <h1>React Frontend (Task4)</h1>
      {greet && <p>Backend says: <b>{greet}</b></p>}
      {error && <p style={{color:'red'}}>{error}</p>}
    </div>
  );
}

export default App;
