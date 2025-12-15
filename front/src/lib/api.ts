export const API_URL = ''; 

export const getHeaders = () => {
  const token = localStorage.getItem('riff_token');
  return {
    'Content-Type': 'application/json',
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
  };
};

export const api = {
  async get(endpoint: string) {
    const res = await fetch(`${API_URL}${endpoint}`, { headers: getHeaders() });
    if (!res.ok) throw new Error(await res.text());
    return res.json();
  },

  async post(endpoint: string, body: any) {
    const res = await fetch(`${API_URL}${endpoint}`, {
      method: 'POST',
      headers: getHeaders(),
      body: JSON.stringify(body),
    });
    if (!res.ok) {
        try {
            const err = await res.json();
            throw new Error(err.error || err.detail || 'Error');
        } catch (e) {
             if (e instanceof Error) throw e;
             throw new Error(await res.text());
        }
    }
    try { return await res.json(); } catch { return {}; }
  },
  
  async delete(endpoint: string) {
    const res = await fetch(`${API_URL}${endpoint}`, {
      method: 'DELETE',
      headers: getHeaders(),
    });
    if (!res.ok) throw new Error('Failed to delete');
  }
};