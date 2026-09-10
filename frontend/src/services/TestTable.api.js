const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5245/api/TestTable'

const TestTableApi = {
  async getTestItems() {
    const response = await fetch(API_URL)
    if (!response.ok) {
      const errorText = await response.text()
      throw new Error(errorText || `Failed to fetch records (HTTP ${response.status})`)
    }
    return await response.json()
  },

  async createTestItem(value) {
    const response = await fetch(API_URL, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ value })
    })

    if (!response.ok) {
      const errorText = await response.text()
      throw new Error(errorText || `Failed to create item (HTTP ${response.status})`)
    }

    return await response.json()
  }
}

export default TestTableApi
