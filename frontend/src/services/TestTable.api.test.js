import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'
import TestTableApi from './TestTable.api.js'

describe('TestTableApi Service', () => {
  beforeEach(() => {
    vi.stubGlobal('fetch', vi.fn())
  })

  afterEach(() => {
    vi.restoreAllMocks()
  })

  it('getTestItems returns parsed JSON data on HTTP 200', async () => {
    const mockData = [
      { id: 1, value: 'Item 1', insertTimeStamp: '2026-09-14T20:00:00Z' },
      { id: 2, value: 'Item 2', insertTimeStamp: '2026-09-14T20:05:00Z' }
    ]

    fetch.mockResolvedValueOnce({
      ok: true,
      json: async () => mockData
    })

    const result = await TestTableApi.getTestItems()
    expect(fetch).toHaveBeenCalledTimes(1)
    expect(result).toEqual(mockData)
  })

  it('getTestItems throws error on non-ok HTTP status', async () => {
    fetch.mockResolvedValueOnce({
      ok: false,
      status: 500,
      text: async () => 'Internal Server Error'
    })

    await expect(TestTableApi.getTestItems()).rejects.toThrow('Internal Server Error')
  })

  it('createTestItem sends POST payload and returns created item', async () => {
    const newItem = { id: 3, value: 'New Test', insertTimeStamp: '2026-09-14T20:10:00Z' }

    fetch.mockResolvedValueOnce({
      ok: true,
      json: async () => newItem
    })

    const result = await TestTableApi.createTestItem('New Test')
    expect(fetch).toHaveBeenCalledTimes(1)
    expect(fetch).toHaveBeenCalledWith(
      expect.any(String),
      expect.objectContaining({
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ value: 'New Test' })
      })
    )
    expect(result).toEqual(newItem)
  })
})
