import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it, vi } from 'vitest'
import { SearchBox } from './SearchBox'

describe('SearchBox', () => {
  it('calls onSearch with the trimmed query when submitted', async () => {
    const user = userEvent.setup()
    const onSearch = vi.fn()

    render(<SearchBox onSearch={onSearch} />)

    await user.type(screen.getByLabelText(/search by gene symbol/i), '  ERBB2  ')
    await user.click(screen.getByRole('button', { name: /search/i }))

    expect(onSearch).toHaveBeenCalledWith('ERBB2')
  })
})
