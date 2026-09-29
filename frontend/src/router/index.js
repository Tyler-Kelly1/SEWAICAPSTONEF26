import { ref, computed } from 'vue'

const getInitialHash = () => {
  if (typeof window === 'undefined') return '#/'
  return window.location.hash || '#/'
}

const currentHash = ref(getInitialHash())

if (typeof window !== 'undefined') {
  window.addEventListener('hashchange', () => {
    currentHash.value = window.location.hash || '#/'
  })
}

export const useHashRouter = () => {
  const currentRoute = computed(() => {
    const raw = currentHash.value
    if (!raw || raw === '#' || raw === '#/') return '/'
    const normalized = raw.replace(/^#/, '')
    return normalized.startsWith('/') ? normalized : `/${normalized}`
  })

  const navigateTo = (path) => {
    if (typeof window === 'undefined') return
    const formatted = path.startsWith('/') ? path : `/${path}`
    window.location.hash = `#${formatted}`
  }

  return {
    currentHash,
    currentRoute,
    navigateTo
  }
}

export default useHashRouter
